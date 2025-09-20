using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace LibraryApp.Web.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // Stocke uniquement le hash; ne jamais stocker le mot de passe en clair
        public string PasswordHash { get; set; } = string.Empty;

        // Rôle basique : "Admin", "Librarian", "User"
        public string Role { get; set; } = "User";

        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime DateJoined { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;
    }
}
