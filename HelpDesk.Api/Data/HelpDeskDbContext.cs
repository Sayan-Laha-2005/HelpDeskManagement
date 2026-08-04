using HelpDesk.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Api.Data
{
    public class HelpDeskDbContext : DbContext
    {
        public HelpDeskDbContext(DbContextOptions<HelpDeskDbContext> options)
            : base(options)
        {
        }

        public DbSet<Ticket> Tickets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Ticket>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Title).IsRequired().HasMaxLength(200);
                entity.Property(t => t.Priority).IsRequired().HasMaxLength(20);
                entity.Property(t => t.Status).IsRequired().HasMaxLength(20);
                entity.Property(t => t.RaisedBy).IsRequired().HasMaxLength(100);
                entity.Property(t => t.CreatedDate).IsRequired();
            });
        }
    }
}
