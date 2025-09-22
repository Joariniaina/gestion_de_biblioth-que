using LibraryApp.Web.Models;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryApp.Web.Repositories
{
    public interface IBookRepository
    {
        // CRUD
        Task<Book> CreateBookAsync(Book book);
        Task<IEnumerable<Book>> GetAllBooksAsync();
        Task<Book?> GetBookByIdAsync(string id);
        Task<Book?> UpdateBookAsync(string id, Book updatedBook);
        Task<bool> DeleteBookAsync(string id);

        // Advanced queries
        Task<IEnumerable<Book>> GetBooksByAuthorGenreYearAsync(string author, string genre, int minYear);
        Task<IEnumerable<Book>> SearchAvailableBooksAsync(string keyword);
        Task<IEnumerable<Book>> GetTopBorrowedBooksAsync();
        Task<List<BsonDocument>> CountBooksPerAuthorAsync();
        Task<List<BsonDocument>> GetAverageBorrowedPerGenreAsync();
        Task<IEnumerable<Book>> GetRecentAvailableBooksAsync();
        Task<IEnumerable<Book>> GetBooksPagedAsync(int page, int pageSize);
        Task<IEnumerable<Book>> GetBooksSortedAsync(string sortBy, bool ascending = true);

        // Updates spécifiques aux livres
        Task AddGenreUniqueAsync(string title, string genre);
        Task MarkRareForOldBooksAsync(int year);

        // Text search
        Task<IEnumerable<Book>> TextSearchAsync(string query);

        // Gestion des copies
        Task<bool> UpdateCopiesAsync(string bookId, List<CopyInfo> copies);

        // gestion de la theme
        // Nouvelle méthode pour obtenir tous les genres uniques
        Task<IEnumerable<string>> GetAllUniqueGenresAsync();

        // Nouvelle méthode pour trouver un livre avec une image de couverture pour un genre donné
        Task<Book?> FindBookByGenreWithCoverImageAsync(string genre);

        // Nouvelle méthode pour obtenir tous les livres d'un genre spécifique
        Task<IEnumerable<Book>> GetBooksByGenreAsync(string genre);


    }
}
