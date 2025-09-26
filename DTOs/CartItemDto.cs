// LibraryApp.Web/DTOs/CartItemDto.cs
namespace LibraryApp.Web.DTOs
{
    public class CartItemDto
    {
        public string BookId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string CoverImageUrl { get; set; } = string.Empty;
    }
}