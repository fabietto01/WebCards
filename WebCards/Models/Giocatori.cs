using System;
using System.Collections.Generic;

#nullable disable

namespace WebCards.Models
{
    public partial class Giocatori
    {
        public Giocatori()
        {
            Manos = new HashSet<Mano>();
            MazzoPersonales = new HashSet<MazzoPersonale>();
        }

        public Guid Rowguid { get; set; }
        public string Nome { get; set; }
        public Guid PartiatId { get; set; }
        public bool IsBot { get; set; }
        public byte Numero { get; set; }
        public bool MyTurno { get; set; }

        public virtual Partite Partiat { get; set; }
        public virtual ICollection<Mano> Manos { get; set; }
        public virtual ICollection<MazzoPersonale> MazzoPersonales { get; set; }
    }
}
