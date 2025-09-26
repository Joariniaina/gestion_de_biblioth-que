namespace LibraryApp.Web.DTOs
{
    public class LoanAllOnDto
    {
        public string LoanId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string BookId { get; set; } = string.Empty;
        public DateTime? BorrowedAt { get; set; }
        public DateTime? DueDate { get; set; }        
    }
}
