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
    public partial class InTavolo
    {
        public void AddCardOnTable(Carte carta, WebCarteContext context)
        {
            var mazzo = new MazzoPersonale
            {
                CartaId = carta.Rowguid,               
            };

            context.MazzoPersonales.Add(mazzo);

            var mano = (from m in context.Manos
                        where m.CartaId == carta.Rowguid
                        select m).First();

            context.Manos.Remove(mano);

            context.SaveChanges();
        }
    }
}
