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
                return null; // Pas de mise à jour
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

        public async Task<string> ComputeStatusAsync(string userId)
        {
            int tmp_status = 0;
            string status_borrow = "0";

            var loans = await _loanRepo.GetLoansByUserAsync(userId); // accès direct repo
            foreach (var loan in loans)
            {
                if (loan.ReturnedAt == null)
                {
                    tmp_status++;
                }
            }

            if (tmp_status < 2)
            {
                status_borrow = "1";
            }

            return status_borrow;
        }

        public async Task<List<LoanOnDto>> GetLoanOnAsync(string userId)
        {
            var loans = await _loanRepo.GetLoansByUserAsync(userId);
            
            return loans.Where(l => l.ReturnedAt == null).Select(l => new LoanOnDto
            {
                LoanId = l.Id,
                BookId = l.BookId,
                BorrowedAt = l.BorrowedAt,
                DueDate = l.DueDate
            }).ToList();
        }

        public async Task<List<LoanAllOnDto>> GetAllLoanOnAsync()
        {
            var loans = await _loanRepo.GetAllLoansAsync();
            return loans.Select(l => new LoanAllOnDto
            {
                LoanId = l.Id,
                UserId = l.UserId,
                BookId = l.BookId,
                BorrowedAt = l.BorrowedAt,
                DueDate = l.DueDate
            }).ToList();
        }



        public async Task<List<LoanLateDto>> GetLoanLateAsync()
        {
            var loans = await _loanRepo.GetOverdueLoansAsync();
            return loans.Select(l => new LoanLateDto
            {
                LoanId = l.Id,
                UserId = l.UserId,
                BookId = l.BookId,
                BorrowedAt = l.BorrowedAt,
                DueDate = l.DueDate,
                LateDuration = l.ReturnedAt == null
                    ? (int)(DateTime.UtcNow.Date - l.DueDate.Date).TotalDays
                    : 0
            }).ToList();
        }
    }
}
