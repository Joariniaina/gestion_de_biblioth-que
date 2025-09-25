using LibraryApp.Web.DTOs;
using LibraryApp.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LibraryApp.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly AuthService _auth;
        private readonly LoanService _loanService;

        public AccountController(AuthService auth, LoanService loanService)
        {
            _auth = auth;
            _loanService = loanService;
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
            var loans = await _loanService.GetStatusAsync(user.Id);
            int tmp_status = 0;
            string status_borrow = "0"; //status de book <<0:false , 1:true>>

            // gestion de login
            if (user == null)
            {
                ModelState.AddModelError("", "Email ou mot de passe incorrect.");
                return View(dto);
            }

            // gestion de status
            foreach (var loan in loans)
            {
                if (loan.ReturnedAt == null) 
                { 
                    tmp_status = tmp_status + 1;
                }
            }

            if(tmp_status <2)
            {
                status_borrow = "1";
            }
    

            //cookie
            HttpContext.Session.SetString("UserId", user.Id);
            HttpContext.Session.SetString("Status_borrow", status_borrow);

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
