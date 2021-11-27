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
            var giocatoreAttuale = drawPlayerDeckModel.GiocatoreAttuale;
            var giocatoreDerubato = drawPlayerDeckModel.GiocatoriDerubato;
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
                    }
                }
            }
            return Redirect($"/Game/{idp}/{idg}");
        }

        public IActionResult DrawTableCard(Partite partita, DrawTableCardModel drawTableCardModel)
        {
            var tavolo = drawTableCardModel.Tavolo;
            var giocatoreAttuale = drawTableCardModel.GiocatoreAttuale;
            for (int i = 0; i < giocatoreAttuale.Manos.Count; i++)
            {
                for (int j = 0; j < tavolo.CarteId.Manos.Count; j++)
                {
                    if (giocatoreAttuale.Manos.ElementAt(i).Carta.Valore == tavolo.CarteId.InTavolos.ElementAt(j).CarteId.Valore)
                    {
                        giocatoreAttuale.SpostaCarta(tavolo.CarteId.Manos.ElementAt(j).Carta, _context);
                        giocatoreAttuale.SpostaCarta(giocatoreAttuale.Manos.ElementAt(i).Carta, _context);
                    }
                }
            }
            //return Redirect($"/Game/{id}/{giocatoreId}");
            return RedirectToAction(actionName: $"{partita.Rowguid}", controllerName: "Game");
        }

        public IActionResult Discard(Partite partita, DiscardModel discardModel)
        {
            var tavolo = discardModel.Tavolo;
            var giocatoreAttuale = discardModel.GiocatoreAttuale;
            bool discardPossible = false;
            //tabel.CarteId.InTavolos.Add(giocatoreAttuale.SpostaCarta(,context))
            for(int i = 0; i < giocatoreAttuale.Manos.Count; i++)
            {
                for(int j = 0; j < tavolo.CarteId.Manos.Count; j++)
                {
                    if(giocatoreAttuale.Manos.ElementAt(i).Carta.Valore != tavolo.CarteId.InTavolos.ElementAt(j).CarteId.Valore)
                    {
                        discardPossible = true;
                    }
                }
                if (discardPossible)
                {
                    tavolo.AddCardOnTable(giocatoreAttuale.Manos.ElementAt(i).Carta, _context);
                }
            }
            //return Redirect($"/Game/{id}/{giocatoreId}");
            return RedirectToAction(actionName: $"{partita.Rowguid}", controllerName: "Game");
        }
    }
}
