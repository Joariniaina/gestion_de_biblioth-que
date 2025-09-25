namespace LibraryApp.Web.DTOs
{
    public class LoanStatusDto
    {
        public string LoanId { get; set; } = string.Empty; 
        public string UserId { get; set; } = string.Empty;
        public string BookId { get; set; } = string.Empty;
        public DateTime? ReturnedAt { get; set; }        
    }
}
