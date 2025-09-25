using LibraryApp.Web.DTOs;
using LibraryApp.Web.Models;
using LibraryApp.Web.Repositories;
using System.Threading.Tasks;
using System.Linq;

namespace LibraryApp.Web.Services
{
    public class LoanService
    {
        private readonly IBookRepository _bookRepo;
        private readonly ILoanRepository _loanRepo;

        public LoanService(IBookRepository bookRepo, ILoanRepository loanRepo)
        {
            _bookRepo = bookRepo;
            _loanRepo = loanRepo;
        }

        // Emprunter un exemplaire
        public async Task<Loan?> BorrowAsync(LoanCreateDto dto)
        {
            Console.WriteLine($"[BorrowAsync] Start for BookId={dto.BookId}, UserId={dto.UserId}");

            var book = await _bookRepo.GetBookByIdAsync(dto.BookId);
            if (book == null || book.CopiesAvailable <= 0)
            {
                Console.WriteLine("[BorrowAsync] Book not available");
                return null;
            }

            // Choisir une copie dispo
            var copy = book.Copies.Find(c => c.Available);
            if (copy == null)
            {
                Console.WriteLine("[BorrowAsync] No available copy found");
                return null;
            }

            // Marquer la copie comme empruntée
            copy.Available = false;
            copy.BorrowedBy = dto.UserId;
            book.CopiesAvailable--;
            book.TimesBorrowed++;

            // ✅ Vérification update
            var updated = await _bookRepo.UpdateBookAsync(book.Id, book);
            Console.WriteLine(updated == null
                ? "[BorrowAsync] Book update FAILED!"
                : "[BorrowAsync] Book update OK!");

            if (updated == null)
            {
                return null; // Pas de mise à jour, on arrête
            }

            // Créer le prêt
            var loan = new Loan
            {
                BookId = book.Id,
                CopyCode = copy.Code,
                UserId = dto.UserId,
                DueDate = dto.DueDate
            };

            await _loanRepo.CreateLoanAsync(loan);
            Console.WriteLine($"[BorrowAsync] Loan created for BookId={loan.BookId}, CopyCode={loan.CopyCode}");

            return loan;
        }


        // Retourner un exemplaire
        public async Task ReturnAsync(LoanReturnDto dto)
        {
            var loan = await _loanRepo.GetLoanByIdAsync(dto.LoanId);
            if (loan == null) return;

            var book = await _bookRepo.GetBookByIdAsync(loan.BookId);
            if (book == null) return;

            var copy = book.Copies.Find(c => c.Code == loan.CopyCode && !c.Available);
            if (copy != null)
            {
                copy.Available = true;
                copy.BorrowedBy = "";
                book.CopiesAvailable++;
                await _bookRepo.UpdateBookAsync(book.Id, book);
            }

            // Mettre à jour le prêt
            await _loanRepo.ReturnLoanAsync(dto.LoanId, dto.Fine);
        }

        // Récupérer l’historique des prêts d’un livre
        public async Task<IEnumerable<Loan>> GetLoansByBookIdAsync(string bookId)
            => await _loanRepo.GetActiveLoansByBookAsync(bookId);
        


        public async Task<List<LoanHistoryDto>> GetBorrowHistoryAsync(string bookId)
        {
            var loans = await _loanRepo.GetLoansByBookIdAsync(bookId);

            return loans.Select(l => new LoanHistoryDto
            {
                LoanId = l.Id,
                UserId = l.UserId,
                CopyCode = l.CopyCode,
                BorrowedAt = l.BorrowedAt,
                DueDate = l.DueDate,
                ReturnedAt = l.ReturnedAt,
                Fine = l.Fine
            }).ToList();
        }

        public async Task<List<LoanStatusDto>> GetStatusAsync(string userId)
        {
           var loans = await _loanRepo.GetLoansByUserAsync(userId);
            
            return loans.Select(l => new LoanStatusDto
            {
                LoanId = l.Id,
                UserId = l.UserId,
                BookId = l.BookId,
                ReturnedAt = l.ReturnedAt
            }).ToList();
        }
    }
}
