using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebCards.Models;

namespace WebCards.Views
{
    public class GameController : Controller
    {
        private readonly ILogger<GameController> _logger;
        private readonly WebCarteContext _context;

        public GameController(ILogger<GameController> logger, WebCarteContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Game()
        {
            
            return View();
        }

        [HttpPost]
        public IActionResult New(Partite data)
        {
            //crea una nuova partita
            var partia = new Partite()
            {
                Rowguid = Guid.NewGuid(),
                Datatime = DateTime.Now,
                NumeroGiocatori = data.NumeroGiocatori,
            };
            _context.Partites.Add(partia);
            _context.SaveChanges();
            return RedirectToAction("Game");
        }

        public IActionResult Load()
        {




            return View();
        }

    }
}
