using LibraryApp.Web.DTOs;
using LibraryApp.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LibraryApp.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly AuthService _auth;

        public AccountController(AuthService auth)
        {
            _auth = auth;
        }

        // ================= VUES =================
        [HttpGet]
        public IActionResult Register() => View();

        [HttpGet]
        public IActionResult Login() => View();

        // ================= ACTIONS =================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            // On délègue la logique à AuthService
            var success = await _auth.RegisterAsync(dto);
            if (!success)
            {
                ModelState.AddModelError("", "Un utilisateur avec cet email existe déjà.");
                return View(dto);
            }

            TempData["Message"] = "Inscription réussie ! Connectez-vous.";
            return RedirectToAction(nameof(Login));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var user = await _auth.LoginAsync(dto);
            if (user == null)
            {
                ModelState.AddModelError("", "Email ou mot de passe incorrect.");
                return View(dto);
            }

            // Exemple : gérer session ou cookie (simplifié ici)
            HttpContext.Session.SetString("UserId", user.Id);

            TempData["Message"] = $"Bienvenue, {user.Name} !";
            return RedirectToAction("Index", "Books");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            // Exemple simplifié : vider la session
            HttpContext.Session.Clear();

            TempData["Message"] = "Déconnexion réussie !";
            return RedirectToAction(nameof(Login));
        }
    }
}
