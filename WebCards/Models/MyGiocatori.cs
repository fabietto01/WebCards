﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebCards.Models;

namespace WebCards.Models
{
    public partial class Giocatori
    {
        public void add_mano(Carte carta, WebCarteContext context)
        {
            var mano = new Mano
            {
                GiocatoreId = Rowguid,
                CartaId = carta.Rowguid,
            };
            context.Manos.Add(mano);
            context.SaveChanges();
        }
        
        public void Ruba(Giocatori giocatoreRubato, WebCarteContext context)
        {
            var carteGiocatore = (from gg in context.MazzoPersonales
                                 where gg.GiocatoreId == giocatoreRubato.Rowguid
                                 select gg).ToList();
            foreach (var item in carteGiocatore)
            {
                item.GiocatoreId = Rowguid;
            }
            context.SaveChanges();
        }

        public void SpostaCarta(Carte carta, WebCarteContext context)
        {
            //sposata la carta dalla mano al mazzo personale
            var mazzo = new MazzoPersonale
            {
                CartaId = carta.Rowguid,
                GiocatoreId = Rowguid
            };

            context.MazzoPersonales.Add(mazzo);

            var mano = (from m in context.Manos
                        where m.CartaId == carta.Rowguid && m.GiocatoreId == Rowguid
                        select m).First();

            context.Manos.Remove(mano);

            context.SaveChanges();
        }

        public void Scarta(Carte carta, WebCarteContext context)
        {
            var intavolo = new InTavolo
            {
                CarteIdId = carta.Rowguid,
                ParitaId = PartiatId,
            };
            context.InTavolos.Add(intavolo);
            var mano = context.Manos.FirstOrDefault(m => m.CartaId == carta.Rowguid && m.GiocatoreId == Rowguid);
            context.Manos.Remove(mano);
        }
        public Carte GetPrimaCartaMazzoPersonale()
        {
            return MazzoPersonales.Last().Carta;
        }


        public override string ToString()
        {
            return Nome;
        }


    }
}
