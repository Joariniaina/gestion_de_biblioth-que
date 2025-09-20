using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using LibraryApp.Web.Models; 

namespace LibraryApp.Web.Models
{
    public class Book
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public int PublicationYear { get; set; }
        public string Genre { get; set; } = string.Empty;
        public string Language { get; set; } = "French";
        public int PageCount { get; set; } = 0;

        public int TotalCopies { get; set; } = 1;
        public int CopiesAvailable { get; set; } = 1;

        public string Location { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new();

        public int TimesBorrowed { get; set; } = 0;

        public List<CopyInfo> Copies { get; set; } = new();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [BsonIgnore]
        public int Year => PublicationYear;

        [BsonIgnore]
        public bool IsAvailable => CopiesAvailable > 0;
    }

    public class CopyInfo
    {
        public string Code { get; set; } = string.Empty;
        public bool Available { get; set; } = true;
        public string BorrowedBy { get; set; } = string.Empty; // user id ou vide
    }
}
