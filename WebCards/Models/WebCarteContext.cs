using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace WebCards.Models
{
    public partial class WebCarteContext : DbContext
    {
        public WebCarteContext()
        {
        }

        public WebCarteContext(DbContextOptions<WebCarteContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Carte> Cartes { get; set; }
        public virtual DbSet<Giocatori> Giocatoris { get; set; }
        public virtual DbSet<InTavolo> InTavolos { get; set; }
        public virtual DbSet<Mano> Manos { get; set; }
        public virtual DbSet<Mazzo> Mazzos { get; set; }
        public virtual DbSet<MazzoPersonale> MazzoPersonales { get; set; }
        public virtual DbSet<Partite> Partites { get; set; }
        public virtual DbSet<Tipi> Tipis { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=185.25.206.129;Persist Security Info=True;User ID=ciro;Password=Ciro2021");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Carte>(entity =>
            {
                entity.HasKey(e => e.Rowguid);

                entity.ToTable("carte");

                entity.Property(e => e.Rowguid)
                    .ValueGeneratedNever()
                    .HasColumnName("rowguid")
                    .HasComment("contiene la chiave univoca della carte");

                entity.Property(e => e.Seme)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("seme")
                    .IsFixedLength(true)
                    .HasComment("contiene il valore del seme delle carte");

                entity.Property(e => e.Valore)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("valore")
                    .IsFixedLength(true)
                    .HasComment("contiene il vaolere numerico del carta");
            });

            modelBuilder.Entity<Giocatori>(entity =>
            {
                entity.HasKey(e => e.Rowguid);

                entity.ToTable("giocatori");

                entity.Property(e => e.Rowguid)
                    .ValueGeneratedNever()
                    .HasColumnName("rowguid")
                    .HasComment("ciave primaria di giocatori");

                entity.Property(e => e.IsBot)
                    .HasColumnName("is_bot")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasMaxLength(25)
                    .HasColumnName("nome")
                    .HasComment("contiene il nome del giocatore");

                entity.Property(e => e.Numero).HasColumnName("numero");

                entity.Property(e => e.PartiatId)
                    .HasColumnName("partiat_id")
                    .HasComment("contiene la relazione tra giocatore e partita");

                entity.HasOne(d => d.Partiat)
                    .WithMany(p => p.Giocatoris)
                    .HasForeignKey(d => d.PartiatId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_giocatori_partite");
            });

            modelBuilder.Entity<InTavolo>(entity =>
            {
                entity.ToTable("in_tavolo");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CarteIdId)
                    .HasColumnName("carteId_id")
                    .HasComment("contiene la relazione tra carte e carte in tavolo");

                entity.Property(e => e.ParitaId)
                    .HasColumnName("parita_id")
                    .HasComment("relazione tra paritta e carte in tavola");

                entity.HasOne(d => d.CarteId)
                    .WithMany(p => p.InTavolos)
                    .HasForeignKey(d => d.CarteIdId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_in_tavolo_carte");

                entity.HasOne(d => d.Parita)
                    .WithMany(p => p.InTavolos)
                    .HasForeignKey(d => d.ParitaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_in_tavolo_partite");
            });

            modelBuilder.Entity<Mano>(entity =>
            {
                entity.ToTable("mano");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CartaId)
                    .HasColumnName("carta_id")
                    .HasComment("ralazione con carta");

                entity.Property(e => e.GiocatoreId)
                    .HasColumnName("giocatore_id")
                    .HasComment("relazione con giocatore");

                entity.HasOne(d => d.Carta)
                    .WithMany(p => p.Manos)
                    .HasForeignKey(d => d.CartaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_mano_carte");

                entity.HasOne(d => d.Giocatore)
                    .WithMany(p => p.Manos)
                    .HasForeignKey(d => d.GiocatoreId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_mano_giocatori");
            });

            modelBuilder.Entity<Mazzo>(entity =>
            {
                entity.ToTable("mazzo");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CarteIdId)
                    .HasColumnName("carteId_id")
                    .HasComment("contiene la colerazione dalle carte a mazzo");

                entity.Property(e => e.PartitaId)
                    .HasColumnName("partita_id")
                    .HasComment("contiene la colerazione tra mazzo e partite");

                entity.HasOne(d => d.CarteId)
                    .WithMany(p => p.Mazzos)
                    .HasForeignKey(d => d.CarteIdId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_mazzo_carte");

                entity.HasOne(d => d.Partita)
                    .WithMany(p => p.Mazzos)
                    .HasForeignKey(d => d.PartitaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_mazzo_partite");
            });

            modelBuilder.Entity<MazzoPersonale>(entity =>
            {
                entity.ToTable("mazzo_personale");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CartaId)
                    .HasColumnName("carta_id")
                    .HasComment("relazione con carte");

                entity.Property(e => e.GiocatoreId)
                    .HasColumnName("giocatore_id")
                    .HasComment("relazione con giocatore");

                entity.HasOne(d => d.Carta)
                    .WithMany(p => p.MazzoPersonales)
                    .HasForeignKey(d => d.CartaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_mazzo_personale_carte");

                entity.HasOne(d => d.Giocatore)
                    .WithMany(p => p.MazzoPersonales)
                    .HasForeignKey(d => d.GiocatoreId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_mazzo_personale_giocatori");
            });

            modelBuilder.Entity<Partite>(entity =>
            {
                entity.HasKey(e => e.Rowguid);

                entity.ToTable("partite");

                entity.Property(e => e.Rowguid)
                    .ValueGeneratedNever()
                    .HasColumnName("rowguid")
                    .HasComment("contiene la chiave primarie per la taballe");

                entity.Property(e => e.Datatime)
                    .HasColumnType("datetime")
                    .HasColumnName("datatime")
                    .HasComment("contiene di creazione della partia");

                entity.Property(e => e.Finita)
                    .HasColumnName("finita")
                    .HasComment("e un bool che tine in memoria lo stato di fine cartita");

                entity.Property(e => e.Inizializata).HasColumnName("inizializata");

                entity.Property(e => e.NumeroGiocatori)
                    .HasColumnName("numero_giocatori")
                    .HasComment("memoriza il numero di giocatori");

                entity.Property(e => e.Url)
                    .HasMaxLength(50)
                    .HasColumnName("url")
                    .HasComment("urls");
            });

            modelBuilder.Entity<Tipi>(entity =>
            {
                entity.HasKey(e => e.Rowguid);

                entity.ToTable("tipi");

                entity.Property(e => e.Rowguid)
                    .ValueGeneratedNever()
                    .HasColumnName("rowguid");

                entity.Property(e => e.Tipo)
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("tipo")
                    .IsFixedLength(true);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
