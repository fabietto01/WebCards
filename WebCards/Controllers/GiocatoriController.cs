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
    public class GiocatoriController : Controller
    {
        private readonly ILogger<GiocatoriController> _logger;
        private readonly WebCarteContext _context;

        protected IQueryable<Partite> _partite
        {
            get
            {
                return from p in _context.Partites
                       select p;
            }
        }

        public GiocatoriController(ILogger<GiocatoriController> logger, WebCarteContext context)
        {
            _logger = logger;
            _context = context;
        }

        [Route("User/{id:Guid}/create")]
        public IActionResult Inizializa_giocatori(Guid id)
        {
            var nuermo_giocatori = _partite.FirstOrDefault(m => m.Rowguid == id).NumeroGiocatori;
            var arr_giocaotri = new ArrayGiocatoriModel
            {
                giocatori = new Giocatori[(int)nuermo_giocatori]
            };
            ViewBag.id = id;
            return View(arr_giocaotri);
        }

        [HttpPost]
        [Route("User/{id:Guid}/create")]
        public IActionResult Inizializa_giocatori(ArrayGiocatoriModel Model , Guid id)
        {
            byte x = 0;
            if(Model.giocatori.Length == 1)
            {
                string[] bot = new string[] { "Erika", "Fabio", "Marco V.", "Marco M.", "Simone", "Domenico", "Gabriele" };
                CreateGiocatore(Model.giocatori[0].Nome, id, ref x);
                Random r = new Random();
                int rInt = r.Next(0, 7);
                CreateGiocatore(bot[rInt], id, ref x, true);
            }
            else{
                foreach (var item in Model.giocatori)
                {
                    CreateGiocatore(item.Nome, id, ref x);
                }
            }
            _context.SaveChanges();
            return Redirect($"/Game/{id}/Inizilizate");
        }

        public void CreateGiocatore(String nome, Guid idPartita, ref byte numero, bool bot = false )
        {
            var g = new Giocatori
            {
                Rowguid = Guid.NewGuid(),
                Nome = nome,
                PartiatId = idPartita,
                IsBot= bot,
                Numero = numero
                
            };
            _context.Giocatoris.Add(g);
            numero++; 
        }


        [Route("Game/{idp:Guid}/{idg:Guid}/Next")]
        public IActionResult NextGiocatore(Guid idp, Guid idg)
        {
            






            NextGiocatore(idp);
            return Redirect($"/Game/{idp}/{idg}");
        }

        //protected bool condizioni_fine_partita()
        //{
        //    bool condizioni = true;
        //    foreach (Giocatore giocatore in giocatori)
        //    {
        //        condizioni = condizioni && (giocatore.CarteCopertes.Count == 0);
        //    }
        //    condizioni = condizioni && (int_scartate == 0);
        //    return !condizioni;
        //}

        private void NextGiocatore(Guid idp)
        {
            var Giocatori = (from gi in _context.Giocatoris
                             where gi.PartiatId == idp
                             orderby gi.Numero
                             select gi).ToList();
            int z = 0;
            for (int i = 0; i < Giocatori.Count; i++)
            {
                if (Giocatori[i].MyTurno)
                {
                    z = i + 1;
                    Giocatori[i].MyTurno = false;
                    break;
                }
            }
            if (z >= Giocatori.Count)
            {
                z = 0;
            }
            Giocatori[z].MyTurno = true;
            _context.SaveChanges();
            if (Giocatori[z].IsBot)
            {
                var partita = _context.Partites.FirstOrDefault(m => m.Rowguid == idp);
                BotAction(partita, Giocatori[z]);
            }
        }

        public IActionResult SelezioneGiocatore()
        {
            return View();
        }



        private void BotAction(Partite partita, Giocatori giocatore)
        {
            var mano = (from ma in _context.Manos
                        join ca in _context.Cartes on ma.CartaId equals ca.Rowguid
                        where ma.GiocatoreId == giocatore.Rowguid
                        select ca).ToList();
            var giocatoriAversari = (from gi in _context.Giocatoris
                                    where gi.PartiatId == partita.Rowguid && gi.Rowguid != giocatore.Rowguid
                                    select gi).ToList();
            var inTavolo = (from it in _context.InTavolos
                            join ca in _context.Cartes on it.CarteIdId equals ca.Rowguid
                            where it.ParitaId == partita.Rowguid
                            select ca).ToList();
            //quin vao da controlare se posso rubare qualce mazzo      InvalidOperationException
            foreach (var carte in mano)
            {
                foreach(var giocatoreAversario in giocatoriAversari){
                    try
                    {
                        if(giocatoreAversario.GetPrimaCartaMazzoPersonale().Valore == carte.Valore){
                            giocatore.Ruba(giocatoreAversario, _context);
                            giocatore.SpostaCarta(carte, _context);
                            goto Finish;
                        }
                    }catch(InvalidOperationException)
                    { }
                }
            }

            //quan vado a controlare e nel caso fare, la azione di rubare una carta dal tavolo
            foreach(var carte in mano)
            {
                foreach(var item in inTavolo)
                {
                    if(carte.Valore == item.Valore)
                    {
                        giocatore.SpostaCarta(carte, _context);
                        giocatore.addMazzoPersonale(item, _context);
                        var tavolo = _context.InTavolos.FirstOrDefault(m => m.CarteIdId == item.Rowguid && m.ParitaId == partita.Rowguid);
                        _context.InTavolos.Remove(tavolo);
                        goto Finish;
                    }
                }
            }
            //scarta in casso non fa nessuna azione sopra!
            giocatore.Scarta(mano[0], _context);


        Finish:
            _context.SaveChanges();
            NextGiocatore(partita.Rowguid);
        }





    }
}
