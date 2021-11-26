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

        [Route("Game/{id:Guid}/create")]
        public IActionResult Inizializa_giocatori(Guid id)
        {
            var nuermo_giocatori = (_partite).FirstOrDefault(m => m.Rowguid == id).NumeroGiocatori;
            var arr_giocaotri = new ArrayGiocatoriModel
            {
                giocatori = new Giocatori[(int)nuermo_giocatori]
            };
            ViewBag.id = id;
            return View(arr_giocaotri);
        }

        [HttpPost]
        [Route("Game/{id:Guid}/create")]
        public IActionResult Inizializa_giocatori(ArrayGiocatoriModel Model , Guid id)
        {
            foreach (var item in Model.giocatori)
            {
                var g = new Giocatori
                {
                    Rowguid = Guid.NewGuid(),
                    Nome = item.Nome,
                    IsBot = item.IsBot,
                    PartiatId = id
                };
                _context.Giocatoris.Add(g);
            }
            _context.SaveChanges();
            return Redirect($"/Game/{id}/Inizilizate");
        }
    }
}
