using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebCards.Models;

namespace WebCards.Controllers
{
    public class CardsController : Controller
    {
        private readonly ILogger<CardsController> _logger;
        private readonly WebCarteContext _context;

        public CardsController(ILogger<CardsController> logger, WebCarteContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult DrawPlayerDeck(Partite partita)
        {
            return RedirectToAction(actionName: $"{partita.Rowguid}", controllerName: "Game");
        }

        public IActionResult DrawTableCard(Partite partita)
        {
            return View();
        }

        public IActionResult Discard(Partite partita)
        {
            return View();
        }
    }
}
