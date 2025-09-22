using LibraryApp.Web.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace LibraryApp.Web.Repositories
{
    public class MongoBookRepository : IBookRepository
    {
        private readonly IMongoCollection<Book> _books;

        public MongoBookRepository(IMongoDatabase database)
        {
            _books = database.GetCollection<Book>("Books");

            // Index texte sur Title et Author
            var keys = Builders<Book>.IndexKeys.Text(b => b.Title).Text(b => b.Author);
            _books.Indexes.CreateOne(new CreateIndexModel<Book>(keys));
        }

        // CRUD
        public async Task<Book> CreateBookAsync(Book book)
        {
            await _books.InsertOneAsync(book);
            return book;
        }

        public async Task<IEnumerable<Book>> GetAllBooksAsync()
            => await _books.Find(Builders<Book>.Filter.Empty).ToListAsync();

        public async Task<Book?> GetBookByIdAsync(string id)
            => await _books.Find(b => b.Id == id).FirstOrDefaultAsync();

public async Task<Book?> UpdateBookAsync(string id, Book updatedBook)
{
    updatedBook.UpdatedAt = DateTime.UtcNow;

    var filter = Builders<Book>.Filter.Eq("_id", ObjectId.Parse(id));
    var res = await _books.ReplaceOneAsync(filter, updatedBook);

    return res.ModifiedCount > 0 ? updatedBook : null;
}

public async Task<bool> DeleteBookAsync(string id)
{
    var filter = Builders<Book>.Filter.Eq("_id", ObjectId.Parse(id));
    var res = await _books.DeleteOneAsync(filter);
    return res.DeletedCount > 0;
}

        // Recherches avancées
        public async Task<IEnumerable<Book>> GetBooksByAuthorGenreYearAsync(string author, string genre, int minYear)
        {
            var filter = Builders<Book>.Filter.Eq(b => b.Author, author) |
                         Builders<Book>.Filter.Eq(b => b.Genre, genre) |
                         Builders<Book>.Filter.Gte(b => b.PublicationYear, minYear);
            return await _books.Find(filter).ToListAsync();
        }

        public async Task<IEnumerable<Book>> SearchAvailableBooksAsync(string keyword)
        {
            var regex = new BsonRegularExpression(keyword, "i");
            var filter = Builders<Book>.Filter.Gt(b => b.CopiesAvailable, 0) &
                         (Builders<Book>.Filter.Regex(b => b.Title, regex) |
                          Builders<Book>.Filter.Regex(b => b.Summary, regex));
            return await _books.Find(filter).ToListAsync();
        }

        // Agrégations
        public async Task<List<BsonDocument>> CountBooksPerAuthorAsync()
            => await _books.Aggregate()
                .Unwind<Book, BsonDocument>(b => b.Copies)
                .Match(Builders<BsonDocument>.Filter.Eq("Copies.Available", true))
                .Group(new BsonDocument { { "_id", "$Author" }, { "total_disponibles", new BsonDocument("$sum", 1) } })
                .Sort(new BsonDocument("total_disponibles", -1))
                .ToListAsync();

        public async Task<List<BsonDocument>> GetAverageBorrowedPerGenreAsync()
            => await _books.Aggregate()
                .Group(new BsonDocument { { "_id", "$Genre" }, { "AverageTimesBorrowed", new BsonDocument("$avg", "$TimesBorrowed") } })
                .ToListAsync();

        public async Task<IEnumerable<Book>> GetRecentAvailableBooksAsync()
        {
            var filter = Builders<Book>.Filter.Gte(b => b.PublicationYear, DateTime.UtcNow.Year - 5) &
                         Builders<Book>.Filter.Gt(b => b.CopiesAvailable, 0);
            return await _books.Find(filter).ToListAsync();
        }

        // Pagination et tri
        public async Task<IEnumerable<Book>> GetBooksPagedAsync(int page, int pageSize)
            => await _books.Find(Builders<Book>.Filter.Empty)
                           .Skip((page - 1) * pageSize)
                           .Limit(pageSize)
                           .ToListAsync();

        public async Task<IEnumerable<Book>> GetBooksSortedAsync(string sortBy, bool ascending = true)
        {
            SortDefinition<Book> sort = sortBy?.ToLower() switch
            {
                "title" => ascending ? Builders<Book>.Sort.Ascending(b => b.Title) : Builders<Book>.Sort.Descending(b => b.Title),
                "author" => ascending ? Builders<Book>.Sort.Ascending(b => b.Author) : Builders<Book>.Sort.Descending(b => b.Author),
                "year" => ascending ? Builders<Book>.Sort.Ascending(b => b.PublicationYear) : Builders<Book>.Sort.Descending(b => b.PublicationYear),
                _ => ascending ? Builders<Book>.Sort.Ascending(b => b.Title) : Builders<Book>.Sort.Descending(b => b.Title),
            };
            return await _books.Find(Builders<Book>.Filter.Empty).Sort(sort).ToListAsync();
        }

        // Updates spécifiques aux livres
        public async Task AddGenreUniqueAsync(string title, string genre)
        {
            var filter = Builders<Book>.Filter.Eq(b => b.Title, title);
            var update = Builders<Book>.Update.AddToSet(b => b.Tags, genre);
            await _books.UpdateOneAsync(filter, update);
        }

        public async Task MarkRareForOldBooksAsync(int year)
        {
            var filter = Builders<Book>.Filter.Lt(b => b.PublicationYear, year);
            var update = Builders<Book>.Update.Set("rare", true);
            await _books.UpdateManyAsync(filter, update);
        }

        // Text search
        public async Task<IEnumerable<Book>> TextSearchAsync(string query)
        {
            var filter = Builders<Book>.Filter.Text(query);
            return await _books.Find(filter).ToListAsync();
        }

        public async Task<IEnumerable<Book>> GetTopBorrowedBooksAsync()
        {
            var sort = Builders<Book>.Sort.Descending(b => b.TimesBorrowed);
            return await _books.Find(Builders<Book>.Filter.Empty).Sort(sort).Limit(5).ToListAsync();
        }

        // Gestion des copies
        public async Task<bool> UpdateCopiesAsync(string bookId, List<CopyInfo> copies)
        {
            var update = Builders<Book>.Update
                .Set(b => b.Copies, copies)
                .Set(b => b.CopiesAvailable, copies.FindAll(c => c.Available).Count)
                .Set(b => b.UpdatedAt, DateTime.UtcNow);

            var res = await _books.UpdateOneAsync(b => b.Id == bookId, update);
            return res.ModifiedCount > 0;
        }
        // gestion de la theme
        // Implémentation : Récupère tous les genres uniques
        public async Task<IEnumerable<string>> GetAllUniqueGenresAsync()
        {
            return await _books.Distinct(b => b.Genre, Builders<Book>.Filter.Empty)
                               .ToListAsync();
        }

        // Implémentation : Trouve le premier livre d'un genre avec une image de couverture
        public async Task<Book?> FindBookByGenreWithCoverImageAsync(string genre)
        {
            var filter = Builders<Book>.Filter.Eq(b => b.Genre, genre) &
                         Builders<Book>.Filter.Ne(b => b.CoverImageUrl, null) &
                         Builders<Book>.Filter.Ne(b => b.CoverImageUrl, string.Empty);

            return await _books.Find(filter).FirstOrDefaultAsync();
        }

        // Implémentation : Récupère tous les livres pour un genre donné
        public async Task<IEnumerable<Book>> GetBooksByGenreAsync(string genre)
        {
            var filter = Builders<Book>.Filter.Eq(b => b.Genre, genre);
            return await _books.Find(filter).ToListAsync();
        }
    }
}
