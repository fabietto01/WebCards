using Microsoft.AspNetCore.Http;
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
    public class GameController : Controller
    {
        private readonly ILogger<GameController> _logger;
        private readonly WebCarteContext _context;

        protected IQueryable<Partite> _partite
        {
            get { return from p in _context.Partites
                         select p;
            }
        }

        protected IQueryable<Giocatori> _giocatori
        {
            get
            {
                return from gi in _context.Giocatoris
                       select gi;
            }
        }

        public GameController(ILogger<GameController> logger, WebCarteContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var cookieValue = Request.Cookies;



            ViewData["partite"] = _partite;
            return View();
        }

        [HttpPost]
        public IActionResult Index(Partite data)
        {
            //crea una nuova partita
            Console.WriteLine(data);
            var partia = new Partite()
            {
                Rowguid = Guid.NewGuid(),
                Datatime = DateTime.Now,
                NumeroGiocatori = data.NumeroGiocatori,

            };
            _context.Partites.Add(partia);
            _context.SaveChanges();
            return Redirect($"User/{partia.Rowguid}/create");
        }

        [Route("Game/{id:Guid}/Inizilizate")]
        public IActionResult Inizilizate(Guid id)
        {
            //prendo e miscio io mazzo
            var mazzo = (from cc in _context.Cartes
                         select cc).ToList();
            Random rand = new Random();
            mazzo = mazzo.OrderBy(x => rand.Next()).ToList();
            //distribuisco carde hai giocatori
            var list_giocato = (from gi in _context.Giocatoris
                                where gi.PartiatId == id
                                orderby gi.Numero
                                select gi).ToList();
            for (int i = 0; i < (list_giocato.Count * 3);)
            {
                foreach (Giocatori giocatore in list_giocato)
                {
                    giocatore.add_mano(mazzo.First(), _context);
                    mazzo.RemoveAt(0);
                    i++;
                }
            }
            _context.SaveChanges();
            for (int i = 0; i < 4 ; i++)
            {
                var intavolo = new InTavolo
                {
                    CarteIdId = mazzo.First().Rowguid,
                    ParitaId = id
                };
                _context.InTavolos.Add(intavolo);
                mazzo.RemoveAt(0);
            }
            //aggioco carte al mazzo
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
            var giocatoreId = list_giocato.First().Rowguid;
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(30),
                HttpOnly = true,
            };
            return Redirect($"/Game/{id}/{giocatoreId}");
        }

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

        [Route("Game/{idp:Guid}/{idg:Guid}")]
        public IActionResult Partita(Guid idp, Guid idg) 
        {
            var partita = _partite.FirstOrDefault(m => m.Rowguid == idp);
            var player = (from pa in _partite
                          join gi in _context.Giocatoris on pa.Rowguid equals gi.PartiatId
                          where gi.Rowguid == idg && pa.Rowguid == idp
                          select gi).FirstOrDefault();
            var player_aversari = (from pa in _partite
                                   join gi in _context.Giocatoris on pa.Rowguid equals gi.PartiatId
                                   where gi.Rowguid != idg && pa.Rowguid == idp
                                   select gi).ToList();
            ViewData["player"] = player;
            ViewData["partita"] = partita;
            ViewData["player_aversari"] = player_aversari;
            return View(); 
        }
            

        















    }
}
