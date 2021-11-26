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
            return Redirect($"Game/{partia.Rowguid}/create");
        }

        [Route("Game/{id:Guid}")]
        public IActionResult Partita(Guid id) =>
            View(_partite.FirstOrDefault(m => m.Rowguid == id));

        [Route("Game/{id:Guid}/Inizilizate")]
        public IActionResult Inizilizate(Guid id)
        {
            var mazzo = (from cc in _context.Cartes
                         select cc).ToList();
            Random rand = new Random();
            mazzo = mazzo.OrderBy(x => rand.Next()).ToList();
            var list_giocato = (from gi in _context.Giocatoris
                                where gi.PartiatId == id
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

            return Redirect($"Game/{id}");
        }















    }
}
