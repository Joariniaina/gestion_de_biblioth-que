namespace LibraryApp.Web.DTOs
{
    public class LoanReturnDto
    {
        public string LoanId { get; set; } = string.Empty; // ID du prêt à retourner
        public decimal Fine { get; set; } = 0;             // pénalité éventuelle
    }
}
