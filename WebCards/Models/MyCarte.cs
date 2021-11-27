using System;
using System.Collections.Generic;


namespace WebCards.Models
{
    public partial class Carte
    {
        public override string ToString()
        {
            return Valore + "/" + Seme;
        }


    }
}
