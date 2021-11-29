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
        [Route("Game/{idp:Guid}/{idg:Guid}/DrawPlayerDeck")]
        public IActionResult DrawPlayerDeck(Guid idp, Guid idg, DrawPlayerDeckModel drawPlayerDeckModel)
        {
            try
            {
                var giocatoreDerubato = _context.Giocatoris.FirstOrDefault(m => m.Rowguid == Guid.Parse(drawPlayerDeckModel.GiocatoriDerubato));
                var giocatoreAttuale = _context.Giocatoris.FirstOrDefault(m => m.Rowguid == idg);
                var cartaScelta = _context.Cartes.FirstOrDefault(m => m.Rowguid == Guid.Parse(drawPlayerDeckModel.CartaScelta));
                if (giocatoreAttuale.MyTurno)
                {
                    if (giocatoreAttuale != giocatoreDerubato)
                    {
                        if (giocatoreDerubato.GetPrimaCartaMazzoPersonale().Valore == cartaScelta.Valore)
                        {
                            //giocatoreAttuale.MazzoPersonales.Concat(giocatoreDerubato.MazzoPersonales);
                            giocatoreAttuale.Ruba(giocatoreDerubato, _context);
                            giocatoreAttuale.SpostaCarta(cartaScelta, _context);
                            _context.SaveChanges();
                            return Redirect($"/Game/{idp}/{idg}/Next");
                        }
                    }
                }
            }
            catch (ArgumentNullException) 
            { 
            }
            return Redirect($"/Game/{idp}/{idg}");
        }

        [HttpPost]
        [Route("Game/{idp:Guid}/{idg:Guid}/DrawTableCard")]
        public IActionResult DrawTableCard(Guid idp, Guid idg, DrawTableCardModel drawTableCardModel)
        {
            try
            {
                var giocatoreAttuale = _context.Giocatoris.FirstOrDefault(m => m.Rowguid == idg);
                var cartaIntavola = _context.Cartes.FirstOrDefault(m => m.Rowguid == Guid.Parse(drawTableCardModel.CartaSceltaIntavola));
                var cartaGiocatore = _context.Cartes.FirstOrDefault(m => m.Rowguid == Guid.Parse(drawTableCardModel.CartaSceltaMano));
                var tavolo = _context.InTavolos.FirstOrDefault(m => m.ParitaId == idp && m.CarteIdId == cartaIntavola.Rowguid);
                if (giocatoreAttuale.MyTurno)
                {
                    if (cartaIntavola.Valore == cartaGiocatore.Valore)
                    {
                        giocatoreAttuale.SpostaCarta(cartaGiocatore, _context);
                        giocatoreAttuale.addMazzoPersonale(cartaIntavola, _context);
                        _context.InTavolos.Remove(tavolo);
                        _context.SaveChanges();
                        return Redirect($"/Game/{idp}/{idg}/Next");
                    }
                }
            }catch (Exception ex)
            {
            }
            return Redirect($"/Game/{idp}/{idg}");
        }

        [HttpPost]
        [Route("Game/{idp:Guid}/{idg:Guid}/Discard")]
        public IActionResult Discard(Guid idp, Guid idg, DiscardModel discardModel)
        {
            try
            {
                var tavolo = (from ta in _context.InTavolos
                              join ca in _context.Cartes on ta.CarteIdId equals ca.Rowguid
                              where ta.ParitaId == idp
                              select ca.Valore).ToList();
                var giocatoriAversari = _context.Giocatoris.Where(m => m.Rowguid != idg && m.PartiatId == idp).ToList();
                var giocatoreAttuale = _context.Giocatoris.FirstOrDefault(m => m.Rowguid == idg);
                var cartaScelta = giocatoreAttuale.Manos.FirstOrDefault(m => m.CartaId == Guid.Parse(discardModel.CartaScelta)).Carta;
                if (giocatoreAttuale.MyTurno)
                {
                    if (!tavolo.Contains(cartaScelta.Valore))
                    {
                        bool condizione = true;
                        foreach(var item in giocatoreAttuale.Manos)
                        {
                            condizione = condizione && (!tavolo.Contains(item.Carta.Valore));

                            foreach (var gi in giocatoriAversari)
                            {
                                try
                                {
                                    condizione = condizione && (item.Carta.Valore != gi.GetPrimaCartaMazzoPersonale().Valore);
                                }
                                catch (InvalidOperationException)
                                {
                                }
                            }
                        }
                        if (condizione)
                        {
                            giocatoreAttuale.Scarta(cartaScelta, _context);
                            _context.SaveChanges();
                            return Redirect($"/Game/{idp}/{idg}/Next");
                        }
                    }
                }
            } catch (Exception ex)
            {

            }
            return Redirect($"/Game/{idp}/{idg}");

        }
    }
}
