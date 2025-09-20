using LibraryApp.Web.Models;

namespace LibraryApp.Web.ViewModels
{
    public class BookLoanViewModel
    {
        public Book Book { get; set; } = new Book();
        public List<Loan> Loans { get; set; } = new List<Loan>();
    }
}
