using System;

namespace WebCards.Models
{
    public class DrawTableCardModel
    {
        public Guid GiocatoreId { get; set; }  

        public Giocatori GiocatoreAttuale { get; set; }

        public Carte CartaScelta { get; set; }

        public InTavolo Tavolo { get; set; }

    }
}
