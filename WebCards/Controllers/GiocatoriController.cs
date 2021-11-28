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
            var nuermo_giocatori = (_partite).FirstOrDefault(m => m.Rowguid == id).NumeroGiocatori;
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
            foreach (var item in Model.giocatori)
            {
                var g = new Giocatori
                {
                    Rowguid = Guid.NewGuid(),
                    Nome = item.Nome,
                    PartiatId = id,
                    Numero = x
                };
                _context.Giocatoris.Add(g);
                x++;
            }
            _context.SaveChanges();
            return Redirect($"/Game/{id}/Inizilizate");
        }


        [Route("Game/{idp:Guid}/{idg:Guid}/Next")]
        public IActionResult NextGiocatore(Guid idp, Guid idg)
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
                }
            }
            if (z >= Giocatori.Count)
            {
                z = 0;
            }
            Giocatori[z].MyTurno = true;
            if (Giocatori[z].IsBot)
            {
                
            }
           
            return Redirect($"/Game/{idp}/{idg}");
        }

    }
}
