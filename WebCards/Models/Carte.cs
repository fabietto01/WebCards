using System;
using System.Collections.Generic;

#nullable disable

namespace WebCards.Models
{
    public partial class Carte
    {
        public Guid Rowguid { get; set; }
        public string Seme { get; set; }
        public string Valore { get; set; }
    }
}
