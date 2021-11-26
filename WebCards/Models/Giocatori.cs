using System;
using System.Collections.Generic;

#nullable disable

namespace WebCards.Models
{
    public partial class Giocatori
    {
        public Guid Rowguid { get; set; }
        public string Nome { get; set; }
        public Guid PartiatId { get; set; }
        public bool IsBot { get; set; }

        public virtual Partite Partiat { get; set; }
    }
}
