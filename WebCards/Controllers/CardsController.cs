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


        [HttpPost]
        [Route("/Game/{idp:Guid}/{idg:Guid}/DrawPlayerDeck")]
        public IActionResult DrawPlayerDeck(Guid idp, Guid idg, DrawPlayerDeckModel drawPlayerDeckModel)
        {
            var giocatoreAttuale = _context.Giocatoris.FirstOrDefault(m => m.Rowguid == idg);
            var giocatoreDerubato = _context.Giocatoris.FirstOrDefault(m => m.Rowguid == Guid.Parse(drawPlayerDeckModel.GiocatoriDerubato));
            if (giocatoreAttuale != giocatoreDerubato)
            {
                for (int i = 0; i < giocatoreAttuale.Manos.Count; i++)
                {
                    if (giocatoreDerubato.MazzoPersonales.First().Carta.Valore == giocatoreAttuale.Manos.ElementAt(i).Carta.Valore)
                    {
                        //giocatoreAttuale.MazzoPersonales.Concat(giocatoreDerubato.MazzoPersonales);
                        giocatoreAttuale.Ruba(giocatoreDerubato, _context);
                        giocatoreAttuale.SpostaCarta(giocatoreAttuale.Manos.ElementAt(i).Carta, _context);
                        giocatoreDerubato.MazzoPersonales.Clear();
                        _context.SaveChanges();
                    }
                }
            }
            return Redirect($"/Game/{idp}/{idg}");
        }

        [HttpPost]
        [Route("/Game/{idp:Guid}/{idg:Guid}/DrawTableCard")]
        public IActionResult DrawTableCard(Guid idp, Guid idg, DrawTableCardModel drawTableCardModel)
        {
            var giocatoreAttuale = _context.Giocatoris.FirstOrDefault(m => m.Rowguid == idg);
            var cartaIntavola = _context.Cartes.FirstOrDefault(m => m.Rowguid == Guid.Parse(drawTableCardModel.CartaSceltaIntavola));
            var cartaGiocatore = _context.Cartes.FirstOrDefault(m => m.Rowguid == Guid.Parse(drawTableCardModel.CartaSceltaMano));
            var tavolo = _context.InTavolos.FirstOrDefault(m => m.ParitaId == idp && m.CarteIdId == cartaIntavola.Rowguid);

            if (cartaIntavola.Valore == cartaGiocatore.Valore)
            {
                giocatoreAttuale.SpostaCarta(cartaGiocatore, _context);
                giocatoreAttuale.add_mano(cartaIntavola, _context);
                _context.InTavolos.Remove(tavolo);
                _context.SaveChanges();
            }
            return Redirect($"/Game/{idp}/{idg}");
        }

        [HttpPost]
        [Route("/Game/{idp:Guid}/{idg:Guid}/Discard")]
        public IActionResult Discard(Guid idp, Guid idg, DiscardModel discardModel)
        {
            var tavolo = (from ta in _context.InTavolos
                          join ca in _context.Cartes on ta.CarteIdId equals ca.Rowguid
                          where ta.ParitaId == idp
                          select ca).ToList();
            var giocatoreAttuale = _context.Giocatoris.FirstOrDefault(m => m.Rowguid == idg);
            var cartaScelta = giocatoreAttuale.Manos.FirstOrDefault(m => m.CartaId == Guid.Parse(discardModel.CartaScelta)).Carta;
            if (!tavolo.Contains(cartaScelta))
            {
                giocatoreAttuale.Scarta(cartaScelta, _context);
            }
            return Redirect($"/Game/{idp}/{idg}");
        }
    }
}
