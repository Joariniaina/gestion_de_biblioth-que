using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace LibraryApp.Web.Models
{
    public class Loan
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        // Références aux identifiants (stockées en tant que string pour MongoDB ObjectId)
        public string UserId { get; set; } = string.Empty;
        public string BookId { get; set; } = string.Empty;

        // Code de la copie empruntée (correspond à CopyInfo.Code)
        public string CopyCode { get; set; } = string.Empty;

        // Dates
        public DateTime BorrowedAt { get; set; } = DateTime.UtcNow;
        public DateTime DueDate { get; set; } = DateTime.UtcNow.AddDays(14);
        public DateTime? ReturnedAt { get; set; }

        // Amende éventuelle calculée au moment du retour
        public decimal Fine { get; set; } = 0;
    }
}
