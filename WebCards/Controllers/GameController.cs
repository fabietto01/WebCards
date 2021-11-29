using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WebCards.Models;

namespace WebCards.Controllers
{
    public class GameController : Controller
    {
        private readonly ILogger<GameController> _logger;
        private readonly WebCarteContext _context;

        //trick per non fare la query tutte le volte per le partite (per non ritornare sempre questo valore)
        //facendo così, con l'iqueryable, hai già tutto a disposizione
        protected IQueryable<Partite> _partite
        {
            get { return from p in _context.Partites
                         select p;
            }
        }

        public GameController(ILogger<GameController> logger, WebCarteContext context)
        {
            _logger = logger;
            _context = context;
        }

        //va a creare la schermata "Partita - Crea o Carica" con partite ordinate
        public IActionResult Index()
        {
            ViewData["partite"] = _partite.Where(m => m.Finita == false).OrderByDescending(m => m.Datatime);
            return View();
        }


        //principio overload è un metodo diverso passo modello partite
        [HttpPost]
        public IActionResult Index(Partite data)
        {
            //crea una nuova partita regez per controllo
            Regex regex = new Regex(@"^[1-4]{1}$");
            var numero = data.NumeroGiocatori.ToString();
            if (regex.IsMatch(numero))
            {
                var partia = new Partite()
                {
                    Rowguid = Guid.NewGuid(),
                    Datatime = DateTime.Now,
                    NumeroGiocatori = data.NumeroGiocatori,

                };
                _context.Partites.Add(partia);
                _context.SaveChanges();
                //reindirizzamento a metodo "create" di GiocatoriController
                return Redirect($"User/{partia.Rowguid}/create");
            }
            ViewData["partite"] = _partite;
            return View();
        }

        [Route("Game/{id:Guid}/Inizilizate")]
        public IActionResult Inizilizate(Guid id)
        {
            //prendo mazzo da db e mischio
            var mazzo = (from cc in _context.Cartes
                         select cc).ToList();
            Random rand = new Random();
            mazzo = mazzo.OrderBy(x => rand.Next()).ToList();
            //distribuisco carte ai giocatori
            var list_giocato = (from gi in _context.Giocatoris
                                where gi.PartiatId == id
                                //numero giocatori
                                orderby gi.Numero
                                //lo converti in lista
                                select gi).ToList();

            //ciclare per 3 volte elementi x dare carte in mano. Numero persone x numero carte a testa (fisso a 3)
            for (int i = 0; i < (list_giocato.Count * 3);)
            {
                //ciclo i giocatori
                foreach (Giocatori giocatore in list_giocato)
                { // se il giocatore ciclato è il primo 
                    if (i == 0)
                    {//è il turno di questo giocatore
                        giocatore.MyTurno = true;
                    }
                    //assegno le carte alla mano giocatore e le rimuovo dal mazzo
                    giocatore.add_mano(mazzo.First(), _context);
                    mazzo.RemoveAt(0);
                    i++;
                }
            }

            //salvo questa distribuzione carte
            _context.SaveChanges();

            //istanzio entità tavolo e distribuisco 4 carte
            for (int i = 0; i < 4; i++)
            {
                var intavolo = new InTavolo
                {
                    //il tavolo è di questa partita e le proprie carte
                    CarteIdId = mazzo.First().Rowguid,
                    ParitaId = id
                };

                //le aggiungo al tavolo e le tolgo dal mazzo
                _context.InTavolos.Add(intavolo);
                mazzo.RemoveAt(0);
            }
            //salvo mazzo al netto di carte distribuite sul db
            foreach (var item in mazzo)
            {
                var maz = new Mazzo
                {
                    CarteIdId = item.Rowguid,
                    PartitaId = id,
                };
                _context.Mazzos.Add(maz);
            }
            _context.SaveChanges();

            //ritorna id del primo giocatore per reindirizzare in Game "Partita" (sotto)
            var giocatoreId = list_giocato.First().Rowguid;
            return Redirect($"/Game/{id}/{giocatoreId}");
        }

        //ci entra quando clicchi "elimina" dalla schermata partite
        [Route("Game/Delete/{id:Guid}")]
        public IActionResult DeleteGame(Guid id)
        {
            //elimina una partita
            var partita = (from pa in _partite
                           where pa.Rowguid == id
                           select pa).First();
            partita.svuota_colerazione(_context);
            _context.Partites.Remove(partita);
            _context.SaveChanges();
            return Redirect("/Game");
        }

        //reindirizzamento da 2 metodi più sopra
        [Route("Game/{idp:Guid}/{idg:Guid}")]

        //passi id giocatore e id partita come parametri
        public IActionResult Partita(Guid idp, Guid idg)
        {
            //gestione eccezione problema chiamate simultanee al database (TENTATIVO)
            lock (_context.Manos)
            {
                //query per questa partita tra tutte quelle in db
                var partita = _partite.FirstOrDefault(m => m.Rowguid == idp);
                if (partita.Finita)
                {
                    return RedirectToAction("FinePartita", "game");
                }

                //se non è finita, carico
                _context.Manos.Load();
                _context.Cartes.Load();
                _context.InTavolos.Load();
                _context.MazzoPersonales.Load();

                //giocatore utente che sta facendo richiesta al server, chi sta visualizzando la pagina
                var player = (from pa in _partite
                              join gi in _context.Giocatoris on pa.Rowguid equals gi.PartiatId
                              where gi.Rowguid == idg && pa.Rowguid == idp
                              select gi).FirstOrDefault();
                
                // richiamo avversari
                var player_aversari = (from pa in _partite
                                       join gi in _context.Giocatoris on pa.Rowguid equals gi.PartiatId
                                       where gi.Rowguid != idg && pa.Rowguid == idp
                                       select gi).ToList();

                //richiamo mazzo conid di questa partita
                var mazzo = (from mm in _context.Mazzos
                             where mm.PartitaId == idp
                             select mm).ToList();

                //controllo generale se ci sono ancora carte
                if (mazzo.Count > 0)
                {
                    //distribuzione di nuovo alle carte giocatori
                    foreach (var item in partita.Giocatoris)
                    {
                        if (item.Manos.Count == 0)
                        {   
                            //3 carte a tutti
                            for (int i = 0; i < 3; i++)
                            {
                                //il carte id in realta e un' istanza di carte, che è stata salvata durante lo scaffolding come carta id, ma l'id della carta è salvato come cartaidid (?). Aggiungere e togliere dal db. 
                                item.add_mano(mazzo.First().CarteId, _context);
                                _context.Mazzos.Remove(mazzo.First());
                                mazzo.RemoveAt(0);
                            }
                        }
                    }
                }

                //refresh pagina (polling)
                Response.Headers.Add("Refresh", "30");

                //inviare dati al view. Inviare dati come view data
                ViewData["player"] = player;
                ViewData["partita"] = partita;
                ViewData["player_aversari"] = player_aversari;
                _context.SaveChanges();
                return View();

                //salva e renderizza view Partita
            }
        }
   


        [Route("Game/Load/{idp:Guid}/")]
        public IActionResult LoadPartita(Guid idp)
        {
            var giocatori = (from gi in _context.Giocatoris
                             where gi.PartiatId == idp && gi.IsBot == false
                             select gi).ToList();
            if (giocatori.Count == 1) 
            {
                return Redirect($"/Game/{idp}/{giocatori[0].Rowguid}");
            }
            ViewData["ListaGiocatori"] = giocatori;
            return View();
        }

        [HttpPost]
        [Route("Game/Load/{idp:Guid}/")]
        public IActionResult LoadPartita(Guid idp, Giocatori model)
        {
            return Redirect($"/Game/{idp}/{model.Rowguid}");
        }

        [Route("Game/FinePartita/{idp:Guid}/")]
        public IActionResult FinePartita(Guid idp)
        {
            var vinciote = (from gi in _context.Giocatoris
                            join mp in _context.MazzoPersonales on gi.Rowguid equals mp.GiocatoreId
                            where gi.PartiatId == idp
                            group gi by gi.Nome into giGroup
                            select new
                            {
                                giocatori = giGroup.Key,
                                Count = giGroup.Count(),
                            }).OrderByDescending(x => x.Count).First();
            var ilvincitore = new WinnerWinnerChickenDinnerModel
            {
                giocatori = vinciote.giocatori,
                count = vinciote.Count.ToString(),
            };
            return View(ilvincitore);
        }

      


















    }
}
