// LibraryApp.Web/Services/CartService.cs
using LibraryApp.Web.DTOs;
using LibraryApp.Web.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Text.Json; // Pour la sérialisation/désérialisation

namespace LibraryApp.Web.Services
{
    public class CartService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string CartSessionKey = "Cart";

        public CartService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ISession? Session => _httpContextAccessor.HttpContext?.Session;

        public List<CartItemDto> GetCartItems()
        {
            var cartJson = Session?.GetString(CartSessionKey);
            return string.IsNullOrEmpty(cartJson) ? new List<CartItemDto>() : JsonSerializer.Deserialize<List<CartItemDto>>(cartJson) ?? new List<CartItemDto>();
        }

        public void AddToCart(Book book)
        {
            var cart = GetCartItems();
            
            // Vérifie si le livre n'est pas déjà dans le panier (pour éviter les doublons)
            if (!cart.Any(item => item.BookId == book.Id))
            {
                cart.Add(new CartItemDto
                {
                    BookId = book.Id,
                    Title = book.Title,
                    Author = book.Author,
                    CoverImageUrl = book.CoverImageUrl
                });
                SaveCart(cart);
            }
        }

        public void RemoveFromCart(string bookId)
        {
            var cart = GetCartItems();
            var itemToRemove = cart.FirstOrDefault(item => item.BookId == bookId);
            if (itemToRemove != null)
            {
                cart.Remove(itemToRemove);
                SaveCart(cart);
            }
        }

        private void SaveCart(List<CartItemDto> cart)
        {
            var cartJson = JsonSerializer.Serialize(cart);
            Session?.SetString(CartSessionKey, cartJson);
        }
    }
}