using LibraryApp.Web.Models;
using LibraryApp.Web.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace LibraryApp.Web.Services
{
    public class BookService
    {
        private readonly IBookRepository _bookRepo;

        public BookService(IBookRepository bookRepo)
        {
            _bookRepo = bookRepo;
        }

        // ===== CRUD =====
        public Task<IEnumerable<Book>> GetAllBooksAsync() => _bookRepo.GetAllBooksAsync();
        public Task<Book?> GetBookByIdAsync(string id) => _bookRepo.GetBookByIdAsync(id);
        public Task<Book> CreateBookAsync(Book book) => _bookRepo.CreateBookAsync(book);
        public Task<Book?> UpdateBookAsync(string id, Book book) => _bookRepo.UpdateBookAsync(id, book);
        public Task<bool> DeleteBookAsync(string id) => _bookRepo.DeleteBookAsync(id);

        // ===== Recherche et filtres =====
        public Task<IEnumerable<Book>> GetBooksByAuthorGenreYearAsync(string author, string genre, int minYear)
            => _bookRepo.GetBooksByAuthorGenreYearAsync(author, genre, minYear);

        public Task<IEnumerable<Book>> SearchAvailableBooksAsync(string keyword)
            => _bookRepo.SearchAvailableBooksAsync(keyword);

        public Task<IEnumerable<Book>> GetTopBorrowedBooksAsync()
            => _bookRepo.GetTopBorrowedBooksAsync();

        public Task<IEnumerable<Book>> GetRecentAvailableBooksAsync()
            => _bookRepo.GetRecentAvailableBooksAsync();

        // ===== Pagination et tri =====
        public Task<IEnumerable<Book>> GetBooksPagedAsync(int page, int size)
            => _bookRepo.GetBooksPagedAsync(page, size);

        public Task<IEnumerable<Book>> GetBooksSortedAsync(string sortBy, bool asc = true)
            => _bookRepo.GetBooksSortedAsync(sortBy, asc);

        // ===== Agrégations =====
        public Task<List<BsonDocument>> CountBooksPerAuthorAsync()
            => _bookRepo.CountBooksPerAuthorAsync();

        public Task<List<BsonDocument>> GetAverageBorrowedPerGenreAsync()
            => _bookRepo.GetAverageBorrowedPerGenreAsync();

        // ===== Mise à jour avancée =====
        public Task AddGenreUniqueAsync(string title, string genre)
            => _bookRepo.AddGenreUniqueAsync(title, genre);

        public Task MarkRareForOldBooksAsync(int year)
            => _bookRepo.MarkRareForOldBooksAsync(year);

        // ===== Recherche textuelle =====
        public Task<IEnumerable<Book>> TextSearchAsync(string query)
            => _bookRepo.TextSearchAsync(query);

        // ====== Gestion des copies =====
        public async Task<IEnumerable<string>> GetAllUniqueGenresAsync()
        {
            return await _bookRepo.GetAllUniqueGenresAsync();
        }

        public async Task<string?> GetCoverImageUrlForGenreAsync(string genre)
        {
            var bookWithCover = await _bookRepo.FindBookByGenreWithCoverImageAsync(genre);
            return bookWithCover?.CoverImageUrl;
        }

        public async Task<IEnumerable<Book>> GetBooksByGenreAsync(string genre)
        {
            return await _bookRepo.GetBooksByGenreAsync(genre);
        }
    }
}
