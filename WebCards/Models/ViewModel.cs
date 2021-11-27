using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebCards.Models;

namespace WebCards.Models
{
    public class ViewModel
    {
        public DrawPlayerDeckModel DrawPlayerDeckModel {get; set;}

        public DrawTableCardModel DrawTableCardModel {get; set;}

        public DiscardModel DiscardModel { get; set; }
    }
}
