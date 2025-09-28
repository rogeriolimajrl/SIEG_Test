using System.Collections.Generic;
using System.Reflection.Emit;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SIEG_Test.Models;

namespace SIEG_Test.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Document> Documents { get; set; }
        public DbSet<DocumentSummary> DocumentSummaries { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Document>()
                .HasIndex(d => d.XmlHash)
                .IsUnique();

            modelBuilder.Entity<Document>()
                .HasIndex(d => new { d.IssueDate, d.EmitCnpj });

            base.OnModelCreating(modelBuilder);
        }
    }
}
