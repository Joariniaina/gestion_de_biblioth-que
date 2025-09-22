// LibraryApp.Web/ViewModels/HomeIndexViewModel.cs

using LibraryApp.Web.DTOs;
using LibraryApp.Web.Models;
using System.Collections.Generic;

namespace LibraryApp.Web.ViewModels
{
    public class HomeIndexViewModel
    {
        public IEnumerable<GenreCollectionDto> GenreCollections { get; set; } = new List<GenreCollectionDto>();
        public IEnumerable<Book> RecentBooks { get; set; } = new List<Book>();
    }
}