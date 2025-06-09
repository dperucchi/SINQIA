using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SINQIA.Models;

public partial class SinqiaContext : DbContext
{
    public SinqiaContext()
    {
    }

    public SinqiaContext(DbContextOptions<SinqiaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cotacao> Cotacaos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cotacao>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Cotacao__3213E83F43738C76");

            entity.ToTable("Cotacao");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Data).HasColumnName("data");
            entity.Property(e => e.Indexador)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("indexador");
            entity.Property(e => e.Valor)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("valor");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
