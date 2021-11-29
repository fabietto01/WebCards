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
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private WebCarteContext _context;


        private List<string> seeds = new List<string> { "Bastoni", "Coppe", "Denari", "Spade" };
        private List<string> values = new List<string> { "A", "2", "3", "4", "5", "6", "7", "F", "C", "R" };

        public HomeController(ILogger<HomeController> logger, WebCarteContext context)
        {
            _logger = logger;
            _context = context;
        }


        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Reset_db()
        {
            //svuota l'intero database
            var manos = (from ma in _context.Manos
                         select ma).ToList();
            foreach (var ma in manos)
            {
                _context.Manos.Remove(ma);
            }
            var mazzopersonales = (from mp in _context.MazzoPersonales
                                   select mp).ToList();
            foreach(var mp in mazzopersonales)
            {
                _context.MazzoPersonales.Remove(mp);
            }
            var mazzos = (from m in _context.Mazzos
                     select m).ToList();
            foreach(var m in mazzos)
            {
                _context.Mazzos.Remove(m);
            }
            var intavolos = (from t in _context.InTavolos
                          select t).ToList();
            foreach (var t in intavolos)
            {
                _context.InTavolos.Remove(t);
            }
            var giocatoris = (from g in _context.Giocatoris
                              select g).ToList();
            foreach(var g in giocatoris)
            {
                _context.Giocatoris.Remove(g);
            }
            var partites = (from p in _context.Partites
                           select p).ToList();
            foreach(var p in partites)
            {
                _context.Partites.Remove(p);
            }
            var cartes = (from c in _context.Cartes
                          select c).ToList();
            foreach(var c in cartes)
            {
                _context.Cartes.Remove(c);
            }
            _context.SaveChanges();
            for (int i = 0; i < seeds.Count; i++)
            {
                for (int j = 0; j < values.Count; j++)
                {
                    var ca = new Carte()
                    {
                        Rowguid = Guid.NewGuid(),
                        Seme = seeds[i],
                        Valore = values[j],
                    };
                    _context.Cartes.Add(ca);
                }
            }
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
