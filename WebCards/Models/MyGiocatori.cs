using System;
using System.Collections.Generic;

namespace WebCards.Models
{
    public partial class Giocatori
    {
        public void add_mano(Carte carta, WebCarteContext context)
        {
            var mano = new Mano
            {
                GiocatoreId = Rowguid,
                CartaId = carta.Rowguid,
            };
            context.Manos.Add(mano);
        }
    }
}
