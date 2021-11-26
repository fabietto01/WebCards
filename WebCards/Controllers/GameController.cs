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

        public IActionResult Index()
        {


            return View();
        }

        [Route("Game/{id:Guid}")]
        public IActionResult Partita(Guid id) =>
            View(_partite.FirstOrDefault(m => m.Rowguid == id));
        

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

            var giocatori = new Giocatori()
            {
                Rowguid = Guid.NewGuid(),
                PartiatId = partia.Rowguid, 
                IsBot = false
            };

            _context.Giocatoris.Add(giocatori);
            _context.SaveChanges();
            return View(partia);
        }
    }
}