using System;
using System.Collections.Generic;

#nullable disable

namespace WebCards.Models
{
    public partial class Partite
    {
        public Partite()
        {
            Giocatoris = new HashSet<Giocatori>();
            InTavolos = new HashSet<InTavolo>();
            Mazzos = new HashSet<Mazzo>();
        }

        public Guid Rowguid { get; set; }
        public DateTime Datatime { get; set; }
        public bool Inizializata { get; set; }
        public bool Finita { get; set; }
        public byte NumeroGiocatori { get; set; }
        public string Url { get; set; }

        public virtual ICollection<Giocatori> Giocatoris { get; set; }
        public virtual ICollection<InTavolo> InTavolos { get; set; }
        public virtual ICollection<Mazzo> Mazzos { get; set; }
    }
}
