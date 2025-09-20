using System;

namespace LibraryApp.Web.DTOs
{
    public class LoanCreateDto
    {
        public string BookId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public DateTime DueDate { get; set; } = DateTime.UtcNow.AddDays(14);
    }
}
