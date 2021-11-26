using Microsoft.AspNetCore.Mvc;
using WebCards.Models;

namespace WebCards.Controllers
{
    public class CardsController : Controller
    {
        private readonly WebCarteContext _context;

        public CardsController(WebCarteContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult DrawPlayerDeck()
        {
            return View();
        }

        public IActionResult DrawTableCard()
        {
            return View();
        }

        public IActionResult Discard()
        {
            return View();
        }
    }
}
