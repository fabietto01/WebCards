using System;
using System.Collections.Generic;

#nullable disable

namespace WebCards.Models
{
    public partial class Mazzo
    {
        public int Id { get; set; }
        public Guid CarteIdId { get; set; }
        public Guid PartitaId { get; set; }

        public virtual Carte CarteId { get; set; }
        public virtual Partite Partita { get; set; }
    }
}
