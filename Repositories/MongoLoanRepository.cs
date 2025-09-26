using LibraryApp.Web.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryApp.Web.Repositories
{
    public class MongoLoanRepository : ILoanRepository
    {
        private readonly IMongoCollection<Loan> _loans;

        public MongoLoanRepository(IMongoDatabase database)
        {
            _loans = database.GetCollection<Loan>("Loans");
        }

        // Créer un nouvel emprunt
        public async Task<Loan> CreateLoanAsync(Loan loan)
        {
            await _loans.InsertOneAsync(loan);
            return loan;
        }

        // Récupérer un emprunt par Id
        public async Task<Loan?> GetLoanByIdAsync(string id)
            => await _loans.Find(l => l.Id == id).FirstOrDefaultAsync();

        // Récupérer tous les emprunts d'un utilisateur
        public async Task<IEnumerable<Loan>> GetLoansByUserAsync(string userId)
            => await _loans.Find(l => l.UserId == userId).ToListAsync();

        // Récupérer tous les emprunts actifs d'un livre spécifique
        public async Task<IEnumerable<Loan>> GetActiveLoansByBookAsync(string bookId)
        {
            var filter = Builders<Loan>.Filter.Eq(l => l.BookId, bookId) &
                         Builders<Loan>.Filter.Eq(l => l.ReturnedAt, null);

            return await _loans.Find(filter).ToListAsync();
        }

        // Retour d'un emprunt
        public async Task<bool> ReturnLoanAsync(string loanId, decimal fine = 0)
        {
            var filter = Builders<Loan>.Filter.Eq(l => l.Id, loanId);
            var update = Builders<Loan>.Update
                .Set(l => l.ReturnedAt, DateTime.UtcNow)
                .Set(l => l.Fine, fine);

            var result = await _loans.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        public async Task<IEnumerable<Loan>> GetLoansByBookIdAsync(string bookId)
        {
            return await _loans.Find(l => l.BookId == bookId).ToListAsync();
        }

        //recuperer tous les emprunts en cours
        public async Task<IEnumerable<Loan>> GetAllLoansAsync()
        {
            var now = DateTime.UtcNow;
            var filter = Builders<Loan>.Filter.Eq(l => l.ReturnedAt, null) &
                         Builders<Loan>.Filter.Gt(l => l.DueDate, now);
            return await _loans.Find(filter).ToListAsync();
        }


        // Récupérer tous les emprunts en retard
        public async Task<IEnumerable<Loan>> GetOverdueLoansAsync()
        {
            var now = DateTime.UtcNow;
            var filter = Builders<Loan>.Filter.Lt(l => l.DueDate, now) &
                         Builders<Loan>.Filter.Eq(l => l.ReturnedAt, null);

            return await _loans.Find(filter).ToListAsync();
        }
    }
}