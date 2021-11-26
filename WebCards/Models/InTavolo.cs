using System;
using System.Collections.Generic;

#nullable disable

namespace WebCards.Models
{
    public partial class InTavolo
    {
        public int Id { get; set; }
        public Guid CarteIdId { get; set; }
        public Guid ParitaId { get; set; }

        public virtual Carte CarteId { get; set; }
        public virtual Partite Parita { get; set; }
    }
}
