namespace LibraryApp.Web.DTOs
{
    public class LoanOnDto
    {
        public string LoanId { get; set; } = string.Empty;
        public string BookId { get; set; } = string.Empty;
        public DateTime? BorrowedAt { get; set; }
        public DateTime? DueDate { get; set; }        
    }
}
