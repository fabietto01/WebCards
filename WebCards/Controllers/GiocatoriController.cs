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

        //reindirizzato da GameController 
        [Route("User/{id:Guid}/create")]
        public IActionResult Inizializa_giocatori(Guid id)
        {
            //recupero il numero giocatori scritto in Game-index
            var nuermo_giocatori = _partite.FirstOrDefault(m => m.Rowguid == id).NumeroGiocatori;
            //creo modello con dentro array giocatori con numero corrispondente all'input ma di valore null
            var arr_giocaotri = new ArrayGiocatoriModel
            {
                //int perchè database restituisce byte, quindi cast
                giocatori = new Giocatori[(int)nuermo_giocatori]
            };
            //vado a restituire il modello
            return View(arr_giocaotri);
        }

        [HttpPost]
        [Route("User/{id:Guid}/create")]
        public IActionResult Inizializa_giocatori(ArrayGiocatoriModel Model , Guid id)
        {
            try
            {   //serve per conteggio giocatori, gestire i turni e definire il giocatore specifico
                byte x = 0;

                // entra se partita 1v1
                if (Model.giocatori.Length == 1)
                {   
                    string[] bot = new string[] { "Erika", "Fabio", "Marco V.", "Marco M.", "Simone", "Domenico" };
                    CreateGiocatore(Model.giocatori[0].Nome, id, ref x);
                    Random r = new Random();
                    int rInt = r.Next(0, 6);
                    CreateGiocatore(bot[rInt], id, ref x, true);
                }
                else
                {// se partita con più giocatori gli passo il modello giocatori
                    foreach (var item in Model.giocatori)
                    {   //per creare giocatori nel database
                        CreateGiocatore(item.Nome, id, ref x);
                    }
                }

                //torna nel GameController
                _context.SaveChanges();
                return Redirect($"/Game/{id}/Inizilizate");
            }

            // se casini,restituisce model e ricarica la pagina come prima
            catch (Exception) 
            {
                return View(Model);
            };
            
        }

        //metodo creato a parte e poi richiamato dove serve (nb: bot = false nel parametro opzionale, se lo setti a true è bot)
        public void CreateGiocatore(String nome, Guid idPartita, ref byte numero, bool bot = false )
        {   //nuovo giocatore e lo assegni a g
            var g = new Giocatori
            { //assegni valore alle proprietà al singolo giocatore
                Rowguid = Guid.NewGuid(),
                Nome = nome,
                PartiatId = idPartita,
                IsBot= bot,
                Numero = numero
                
            };
            _context.Giocatoris.Add(g);
            numero++;
            //viene chiamato nel metodo sopra 3 volte
        }


        [Route("Game/{idp:Guid}/{idg:Guid}/Next")]
        public IActionResult NextGiocatore(Guid idp, Guid idg)
        {
            var giocatori = (from gi in _context.Giocatoris
                             where gi.PartiatId == idp
                             orderby gi.Numero
                             select gi).ToList();
            var mazzo = (from ma in _context.Mazzos
                            where ma.PartitaId == idp
                            select ma).ToList();
            int z = 0;
            for (int i = 0; i < giocatori.Count; i++)
            {
                if (giocatori[i].MyTurno)
                {
                    z = i + 1;
                    if (z >= giocatori.Count)
                    {
                        z = 0;
                    }
                    break;
                }
            }
            if (condizioniFinePartita(giocatori, giocatori[z], mazzo.Count))
            {
                var partita = _partite.FirstOrDefault(m => m.Rowguid == idp);
                partita.Finita = true;
                _context.SaveChanges();
                return Redirect($"/Game/FinePartita/{partita.Rowguid}");
            }
            NextGiocatore(idp);
            return Redirect($"/Game/{idp}/{idg}");
        }

        protected bool condizioniFinePartita(List<Giocatori> giocatori,Giocatori giocatoreSucesivo, int mazzo)
        {
            bool condizioni = true;
            foreach (Giocatori giocatore in giocatori)
            {
                if (giocatore.Rowguid != giocatoreSucesivo.Rowguid)
                {
                    condizioni = condizioni && (giocatore.Manos.Count == 0);
                }
                else
                {
                    condizioni = condizioni && (giocatoreSucesivo.Manos.Count == 1);
                }
                if (!condizioni)
                {
                    break;
                }
            }
            condizioni = condizioni && (mazzo == 0);
            return condizioni;
        }

        private void NextGiocatore(Guid idp)
        {
            var giocatori = (from gi in _context.Giocatoris
                             where gi.PartiatId == idp
                             orderby gi.Numero
                             select gi).ToList();
            int z = 0;
            for (int i = 0; i < giocatori.Count; i++)
            {
                if (giocatori[i].MyTurno)
                {
                    z = i + 1;
                    giocatori[i].MyTurno = false;
                    break;
                }
            }
            if (z >= giocatori.Count)
            {
                z = 0;
            }
            giocatori[z].MyTurno = true;
            _context.SaveChanges();
            if (giocatori[z].IsBot)
            {
                var partita = _context.Partites.FirstOrDefault(m => m.Rowguid == idp);
                BotAction(partita, giocatori[z]);
            }
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
