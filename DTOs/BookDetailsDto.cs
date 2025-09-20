using System;
using System.Collections.Generic;

namespace LibraryApp.Web.DTOs
{
    public class BookDetailsDto
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public int PublicationYear { get; set; }
        public string Genre { get; set; } = string.Empty;
        public string Language { get; set; } = "French";
        public int PageCount { get; set; }
        public string Summary { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new();

        // Copies
        public int TotalCopies { get; set; }
        public int CopiesAvailable { get; set; }
        public List<CopyInfoDto> Copies { get; set; } = new();

        // Statistiques
        public int TimesBorrowed { get; set; }

        // Historique des emprunts (rempli depuis LoanService)
        public List<LoanHistoryDto> BorrowHistory { get; set; } = new();
    }

    public class CopyInfoDto
    {
        public string Code { get; set; } = string.Empty;
        public bool Available { get; set; }
        public string BorrowedByUserId { get; set; } = string.Empty;
    }

    public class LoanHistoryDto
    {
        public string LoanId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string CopyCode { get; set; } = string.Empty;
        public DateTime BorrowedAt { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnedAt { get; set; }
        public decimal Fine { get; set; }
    }
}
