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
        private readonly WebCarteContext _Context;

        public GameController(ILogger<GameController> logger)
        {
            _logger = logger;
            _Context = context;
        }

        public IActionResult Game()
        {

            

            return View();
        }


        public IActionResult Now()
        {
            



            return View();
        }

        public IActionResult Load()
        {




            return View();
        }

        public IActionResult Partita()
        {




            return View();
        }


       
    }
}
