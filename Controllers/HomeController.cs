// Controllers/HomeController.cs
using Microsoft.AspNetCore.Mvc;
using LibraryApp.Web.DTOs;
using LibraryApp.Web.Models;
using LibraryApp.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using LibraryApp.Web.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace LibraryApp.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly BookService _bookService;

        public HomeController(BookService bookService)
        {
            _bookService = bookService;
        }

        public async Task<IActionResult> Index()
        {

            var uniqueGenres = await _bookService.GetAllUniqueGenresAsync();
            var genreCollections = new List<GenreCollectionDto>();

            foreach (var genre in uniqueGenres)
            {
                var imageUrl = await _bookService.GetCoverImageUrlForGenreAsync(genre);
                genreCollections.Add(new GenreCollectionDto
                {
                    Name = genre,
                    Description = $"Découvrez les ouvrages passionnants de notre collection {genre}.",
                    ActionUrl = Url.Action("BooksByGenre", "Books", new { genre = genre }),
                    ImageUrl = imageUrl ?? "/images/default_genre_cover.jpg"
                });
            }

            var recentBooks = await _bookService.GetRecentAvailableBooksAsync();

             Console.WriteLine($"on a pour le nouveau livre {recentBooks.Count()} livres récents");

            var viewModel = new HomeIndexViewModel
            {
                GenreCollections = genreCollections,
                RecentBooks = recentBooks
            };

            return View(viewModel);
        }
        
    }
}