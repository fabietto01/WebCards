using System;
using System.Collections.Generic;

#nullable disable

namespace WebCards.Models
{
    public partial class Mazzo
    {
        public Guid CarteIdId { get; set; }
        public Guid PartitaId { get; set; }

        public virtual Carte Partita { get; set; }
        public virtual Partite PartitaNavigation { get; set; }
    }
}
