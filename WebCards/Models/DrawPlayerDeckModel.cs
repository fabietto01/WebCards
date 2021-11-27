using System;

namespace WebCards.Models
{
    public class DrawPlayerDeckModel
    {
        public Giocatori GiocatoreAttuale { get; set; }

        public Giocatori GiocatoriDerubato { get; set; }

        public Carte CartaScelta { get; set; }
    }
}
