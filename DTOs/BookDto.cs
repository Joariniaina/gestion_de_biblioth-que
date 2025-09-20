using System.Collections.Generic;

namespace LibraryApp.Web.DTOs
{
    public class BookDto
    {
        public string? Id { get; set; } // optional for create
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public int PublicationYear { get; set; }
        public string Genre { get; set; } = string.Empty;
        public string Language { get; set; } = "French";
        public int PageCount { get; set; }
        public int TotalCopies { get; set; }
        public string Summary { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new();
    }
}
