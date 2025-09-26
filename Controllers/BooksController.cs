using LibraryApp.Web.DTOs;
using LibraryApp.Web.Models;
using LibraryApp.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using LibraryApp.Web.ViewModels;


namespace LibraryApp.Web.Controllers
{
    public class BooksController : Controller
    {
        private readonly BookService _bookService;
        private readonly LoanService _loanService;
        private readonly CartService _cartService;

        public BooksController(BookService bookService, LoanService loanService,  CartService cartService)
        {
            _bookService = bookService;
            _loanService = loanService;
            _cartService = cartService;
        }

        // ===== CRUD =====
        public async Task<IActionResult> Index()
        {
            var books = await _bookService.GetAllBooksAsync();
            var currentUserId = HttpContext.Session.GetString("UserId");

            var viewModels = new List<BookLoanViewModel>();


            foreach (var book in books)
            {
                // Récupérer uniquement les prêts actifs pour ce livre
                var loans = await _loanService.GetLoansByBookIdAsync(book.Id);

                // Filtrer par utilisateur si besoin
                var userLoans = loans.Where(l => l.UserId == currentUserId).ToList();

                viewModels.Add(new BookLoanViewModel
                {
                    Book = book,
                    Loans = userLoans
                });
            }

            return View(viewModels);
        }


        public IActionResult Create() => View();


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookDto dto)
        {
            if (dto.CoverImageUrl != null)
            {
                // Nom original du fichier (ex: images.jpeg)
                var fileName = Path.GetFileName(dto.CoverImageUrl.FileName);

                // On le sauvegarde dans wwwroot/images/
                var savePath = Path.Combine("wwwroot/images", fileName);

                using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    await dto.CoverImageUrl.CopyToAsync(stream);
                }

            }
            if (!ModelState.IsValid) return View(dto);

            // Création du livre
            var book = new Book
            {
                Title = dto.Title,
                Author = dto.Author,
                ISBN = dto.ISBN,
                Publisher = dto.Publisher,
                PublicationYear = dto.PublicationYear,
                Genre = dto.Genre,
                CoverImageUrl = dto.CoverImageUrl != null ? "/images/" + Path.GetFileName(dto.CoverImageUrl.FileName) : "",
                Language = dto.Language,
                PageCount = dto.PageCount,
                TotalCopies = dto.TotalCopies,
                CopiesAvailable = dto.TotalCopies, // toutes les copies disponibles
                Summary = dto.Summary,
                Tags = dto.Tags
            };

            // Initialisation des copies
            for (int i = 1; i <= dto.TotalCopies; i++)
            {
                book.Copies.Add(new CopyInfo
                {
                    Code = $"C{i:D3}", // par ex. C001, C002, ...
                    Available = true,
                    BorrowedBy = ""
                });
            }

            await _bookService.CreateBookAsync(book);
            return RedirectToAction(nameof(Index));
        }

        //edit
        public async Task<IActionResult> Edit(string id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null) return NotFound();

            var dto = new BookDto
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                ISBN = book.ISBN,
                Publisher = book.Publisher,
                PublicationYear = book.PublicationYear,
                Genre = book.Genre,
                Language = book.Language,
                PageCount = book.PageCount,
                TotalCopies = book.TotalCopies,
                Summary = book.Summary,
                Tags = book.Tags
            };

            return View(dto);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, BookDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            var book = await _bookService.GetBookByIdAsync(id);
            var Total_tmp = book.TotalCopies;

            if (book == null) return NotFound();

            book.Title = dto.Title;
            book.Author = dto.Author;
            book.ISBN = dto.ISBN;
            book.Publisher = dto.Publisher;
            book.PublicationYear = dto.PublicationYear;
            book.Genre = dto.Genre;
            book.Language = dto.Language;
            book.PageCount = dto.PageCount;
            book.CopiesAvailable = dto.TotalCopies - Total_tmp + book.CopiesAvailable;
            book.TotalCopies = dto.TotalCopies;
            book.Summary = dto.Summary;
            book.Tags = dto.Tags;

            for (int i = Total_tmp + 1; i <= dto.TotalCopies; i++)
            {
                book.Copies.Add(new CopyInfo
                {
                    Code = $"C{i:D3}",
                    Available = true,
                    BorrowedBy = ""
                });
            }

            await _bookService.UpdateBookAsync(id, book);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(string id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null) return NotFound();
            return View(book);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _bookService.DeleteBookAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // ===== Recherche et filtres =====
        public async Task<IActionResult> Search(string keyword)
        {
            var results = await _bookService.SearchAvailableBooksAsync(keyword ?? "");
            var bookLoanViewModelsSearch = results.Select(book => new BookLoanViewModel
            {
                Book = book,
                Loans = new List<Loan>() 
            });
            return View("Index", bookLoanViewModelsSearch);
        }

        public async Task<IActionResult> Filter(string author, string genre, int minYear = 0)
        {
            var results = await _bookService.GetBooksByAuthorGenreYearAsync(author ?? "", genre ?? "", minYear);
            var bookLoanViewModelsFilter = results.Select(book => new BookLoanViewModel
            {
                Book = book,
                Loans = new List<Loan>() 
            });
            return View("Index", bookLoanViewModelsFilter);
        }

        // ===== Top / Récent / Pagination / Tri =====
        public async Task<IActionResult> TopBorrowed()
        {
            var books = await _bookService.GetTopBorrowedBooksAsync();
            var bookLoanViewModels = books.Select(book => new BookLoanViewModel
            {
                Book = book,
                // Si tu n'as pas besoin des prêts pour ces actions spécifiques,
                // tu peux initialiser Loans à une liste vide.
                // Sinon, tu devras récupérer les prêts ici si ton BookLoanViewModel les nécessite.
                Loans = new List<Loan>() 
            });
            return View("Index", bookLoanViewModels);
        }
        
        public async Task<IActionResult> Recent()
        {
            var books = await _bookService.GetRecentAvailableBooksAsync();
            var bookLoanViewModels = books.Select(book => new BookLoanViewModel
            {
                Book = book,
                Loans = new List<Loan>() // Idem pour ici
            });
            return View("Index", bookLoanViewModels);
        }
        
        public async Task<IActionResult> Paged(int page = 1, int size = 10)
        {
            var books = await _bookService.GetBooksPagedAsync(page, size);
            var bookLoanViewModels = books.Select(book => new BookLoanViewModel
            {
                Book = book,
                Loans = new List<Loan>() // Idem pour ici
            });
            return View("Index", bookLoanViewModels);
        }
        
        public async Task<IActionResult> Sorted(string sortBy = "Title", bool asc = true)
        {
            var books = await _bookService.GetBooksSortedAsync(sortBy, asc);
            var bookLoanViewModels = books.Select(book => new BookLoanViewModel
            {
                Book = book,
                Loans = new List<Loan>() // Idem pour ici
            });
            return View("Index", bookLoanViewModels);
        }
        

        // ===== Emprunts via LoanService =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Borrow(string bookId, string userId)
        {
            var dto = new LoanCreateDto { BookId = bookId, UserId = userId };
            await _loanService.BorrowAsync(dto);

            var status_borrow = await _loanService.ComputeStatusAsync(userId);
            HttpContext.Session.SetString("Status_borrow", status_borrow);

            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Return(string loanId)
        {
            Console.WriteLine($"Borrow triggered for loanId={loanId}");
            // recuperation du contenu de la session
            var session_borrow = HttpContext.Session.GetString("Status_borrow");
            Console.WriteLine($"Borrow triggered for status={session_borrow}");
            
            session_borrow = "1";
            //mise à jour de session
            HttpContext.Session.SetString("Status_borrow", session_borrow);
            
            var dto = new LoanReturnDto            
            {
                LoanId = loanId,
                Fine = 0 // ou calculer la pénalité si besoin
            };
            await _loanService.ReturnAsync(dto);
            return RedirectToAction(nameof(Index));
        }


        // ===== Détails du livre =====
        public async Task<IActionResult> Details(string id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null) return NotFound();

            // Récupérer l’historique depuis LoanService
            var borrowHistory = await _loanService.GetBorrowHistoryAsync(book.Id);

            var dto = new BookDetailsDto
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                ISBN = book.ISBN,
                Publisher = book.Publisher,
                PublicationYear = book.PublicationYear,
                Genre = book.Genre,
                Language = book.Language,
                CoverImageUrl = book.CoverImageUrl,
                PageCount = book.PageCount,
                Summary = book.Summary,
                Tags = book.Tags,
                TotalCopies = book.TotalCopies,
                CopiesAvailable = book.CopiesAvailable,
                TimesBorrowed = borrowHistory.Count, // 📊 calculé depuis l’historique
                Copies = book.Copies.ConvertAll(c => new CopyInfoDto
                {
                    Code = c.Code,
                    Available = c.Available,
                    BorrowedByUserId = c.BorrowedBy
                }),
                BorrowHistory = borrowHistory // 📜 on alimente enfin l’historique
            };

            return View(dto);
        }

        public async Task<IActionResult> BooksByGenre(string genre)
        {
            if (string.IsNullOrEmpty(genre))
            {
                // Si le genre est vide, redirige vers la page principale des livres
                return RedirectToAction(nameof(Index));
            }

            // Récupère tous les livres pour le genre spécifié
            var books = await _bookService.GetBooksByGenreAsync(genre);

            // Passe le nom du genre à la vue pour l'afficher dans le titre
            ViewBag.CurrentGenre = genre;

            // Retourne la vue "BooksByGenre.cshtml" avec la liste des livres
            return View(books); // Cette vue attendra un IEnumerable<Book>
        }

        // ===== Panier =====
        [HttpPost]
        public async Task<IActionResult> AddToCart(string id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book != null)
            {
                _cartService.AddToCart(book);
            }
            return RedirectToAction("Index"); // Redirige vers la page d'accueil ou de catalogue
        }
        
        [HttpPost]
        public IActionResult RemoveFromCart(string id)
        {
            _cartService.RemoveFromCart(id);
            return RedirectToAction("ViewCart"); // Redirige vers la page du panier
        }

        public IActionResult ViewCart()
        {
            var cartItems = _cartService.GetCartItems();
            return View(cartItems); // Crée une vue pour afficher le panier
        }

        public IActionResult ClearCart()
        {
            var cartItems = _cartService.GetCartItems();
            foreach (var item in cartItems)
            {
                _cartService.RemoveFromCart(item.BookId);
            }
            return RedirectToAction("ViewCart"); // Redirige vers la page du panier
        }

        //=================emprunt de la page panier=================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CartEmprunt(string bookId, string userId)
        {
            Borrow(bookId, userId);
            var status_borrow = await _loanService.ComputeStatusAsync(userId);
            HttpContext.Session.SetString("Status_borrow", status_borrow);
            RemoveFromCart(bookId);
            return RedirectToAction("ViewCart");
        }

        // ======================== Mes emprunts ==================
        public async Task<IActionResult> MyLoans()
        {
            // Récupérer l’ID de l’utilisateur depuis la session
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }
            // Appeler ton service
            var loans = await _loanService.GetLoanOnAsync(userId);
            // Passer la liste à la vue
            return View(loans);
        }

        //================== les emprunts en cours ==================
        public async Task<IActionResult> AllLoans()
        {
            var loans = await _loanService.GetAllLoanOnAsync();
            return View(loans);
        }

        //================== les emprunts en retard ================= 
        public async Task<IActionResult> AllLateLoans()
        {
            var loans =await _loanService.GetLoanLateAsync();
            return View(loans);
        }
    }
}
