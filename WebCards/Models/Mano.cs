using System;
using System.Collections.Generic;

#nullable disable

namespace WebCards.Models
{
    public partial class Mano
    {
        public Guid GiocatoreId { get; set; }
        public Guid CartaId { get; set; }

        public virtual Carte Carta { get; set; }
        public virtual Giocatori Giocatore { get; set; }
    }
}
