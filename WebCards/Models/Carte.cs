using System;
using System.Collections.Generic;



namespace WebCards.Models
{
    public partial class Carte
    {
        public Carte()
        {
            InTavolos = new HashSet<InTavolo>();
            Manos = new HashSet<Mano>();
            MazzoPersonales = new HashSet<MazzoPersonale>();
            Mazzos = new HashSet<Mazzo>();
        }

        public Guid Rowguid { get; set; }
        public string Seme { get; set; }
        public string Valore { get; set; }

        public virtual ICollection<InTavolo> InTavolos { get; set; }
        public virtual ICollection<Mano> Manos { get; set; }
        public virtual ICollection<MazzoPersonale> MazzoPersonales { get; set; }
        public virtual ICollection<Mazzo> Mazzos { get; set; }
    }
}
