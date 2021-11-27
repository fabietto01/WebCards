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

        public IActionResult DrawPlayerDeck(Partite partita, DrawPlayerDeckModel drawPlayerDeckModel, WebCarteContext context)
        {
            var giocatoreAttuale = drawPlayerDeckModel.GiocatoreAttuale;
            var giocatoreDerubato = drawPlayerDeckModel.GiocatoriDerubato;
            if (giocatoreAttuale != giocatoreDerubato)
            {
                for (int i = 0; i < giocatoreAttuale.Manos.Count; i++)
                {
                    if (giocatoreDerubato.MazzoPersonales.First().Carta.Valore == giocatoreAttuale.Manos.ElementAt(i).Carta.Valore)
                    {
                        //giocatoreAttuale.MazzoPersonales.Concat(giocatoreDerubato.MazzoPersonales);
                        giocatoreAttuale.Ruba(giocatoreDerubato, context);
                        giocatoreAttuale.SpostaCarta(giocatoreAttuale.Manos.ElementAt(i).Carta, context);
                        giocatoreDerubato.MazzoPersonales.Clear();
                    }
                }
            }

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
