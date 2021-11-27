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
    public class IndexGameModel
    {
        public IQueryable<Partite> partite { get; set; }
        public Partite partita { get; set; }
    }
}
