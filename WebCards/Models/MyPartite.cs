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
    public partial class Partite
    {

        public void svuota_colerazione(WebCarteContext context)
        {
            var mazzo = (from ma in context.Mazzos
                         where ma.PartitaId == Rowguid
                         select ma).ToList();
            foreach (var item in mazzo)
            {
                context.Mazzos.Remove(item);
            }
            var inTavolo = (from it in context.InTavolos
                            where it.ParitaId == Rowguid
                            select it).ToList();
            foreach(var item in inTavolo)
            {
                context.InTavolos.Remove(item);
            }
            var giocatori =  (from gi in context.Giocatoris
                              where gi.PartiatId == Rowguid
                              select gi).ToList();
            foreach(var item in giocatori)
            {
                var mazzopersonale = (from mp in context.MazzoPersonales
                                      where mp.GiocatoreId == item.Rowguid
                                      select mp).ToList();
                foreach(var carta in mazzopersonale)
                {
                    context.MazzoPersonales.Remove(carta);
                }
                var mano = (from man in context.Manos
                            where man.GiocatoreId == item.Rowguid
                            select man).ToList();
                foreach(var carta in mano)
                {
                    context.Manos.Remove(carta);
                }
                context.Giocatoris.Remove(item);
            }
            context.SaveChanges();
        }











    }
}
