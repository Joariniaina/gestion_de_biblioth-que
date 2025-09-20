using LibraryApp.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryApp.Web.Repositories
{
    public interface ILoanRepository
    {
        Task<Loan> CreateLoanAsync(Loan loan);
        Task<Loan?> GetLoanByIdAsync(string id);
        Task<IEnumerable<Loan>> GetLoansByUserAsync(string userId);
        Task<IEnumerable<Loan>> GetActiveLoansByBookAsync(string bookId);
        Task<bool> ReturnLoanAsync(string loanId, decimal fine = 0);
        Task<IEnumerable<Loan>> GetLoansByBookIdAsync(string bookId);
        Task<IEnumerable<Loan>> GetOverdueLoansAsync();
    }
}
