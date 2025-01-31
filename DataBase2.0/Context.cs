using Microsoft.EntityFrameworkCore;

namespace DataBase2._0
{
    public class MusicStoreContext : DbContext
    {
        public DbSet<Disc> Discs { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<DiscountCard> DiscountCards { get; set; }
        public DbSet<Employer> Employers { get; set; }
        public DbSet<Sale> Sales { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source = C:/Users/purpl/RiderProjects/DataBase2.0/DataBase2.0/music.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Sale>()
                .Property(s => s.DiscountCardID)
                .IsRequired(false);

            base.OnModelCreating(modelBuilder);

            // Конфигурация Disc
            modelBuilder.Entity<Disc>(entity =>
            {
                entity.HasKey(d => d.ID);
                
                entity.Property(d => d.Price)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                entity.Property(d => d.DiscName)
                    .IsRequired();

                entity.Property(d => d.Genre)
                    .IsRequired();

                // Связь с Artist (один-ко-многим)
                entity.HasOne(d => d.Artist)
                    .WithMany(a => a.Discs)
                    .HasForeignKey(d => d.ArtistId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Связь с Sale (один-к-одному)
                entity.HasOne(d => d.Sale)
                    .WithOne(s => s.SoldDisc)
                    .HasForeignKey<Sale>(s => s.DiscId);
            });

            // Конфигурация Artist
            modelBuilder.Entity<Artist>(entity =>
            {
                entity.HasKey(a => a.ID);
                
                entity.Property(a => a.Name)
                    .IsRequired();

                // Связь с Disc определена выше
            });

            // Конфигурация DiscountCard
            modelBuilder.Entity<DiscountCard>(entity =>
            {
                entity.HasKey(dc => dc.ID);
                
                entity.Property(dc => dc.CardNumber)
                    .IsRequired();

                entity.Property(dc => dc.DiscountPercentage)
                    .HasColumnType("decimal(5,2)")
                    .IsRequired();

                // Связь с Sale (один-ко-многим)
                entity.HasMany(dc => dc.Sales)
                    .WithOne(s => s.DiscountCard)
                    .HasForeignKey(s => s.DiscountCardID)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Конфигурация Employer
            modelBuilder.Entity<Employer>(entity =>
            {
                entity.HasKey(e => e.ID);
                
                entity.Property(e => e.Name)
                    .IsRequired();

                entity.Property(e => e.Position)
                    .IsRequired();

                // Связь с Sale (один-ко-многим)
                entity.HasMany(e => e.Sales)
                    .WithOne(s => s.Seller)
                    .HasForeignKey(s => s.SellerID)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Конфигурация Sale
            modelBuilder.Entity<Sale>(entity =>
            {
                entity.HasKey(s => s.ID);
                
                entity.Property(s => s.Location)
                    .IsRequired();

                entity.Property(s => s.SaleDate)
                    .IsRequired();

                entity.Property(s => s.SalePrice)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                
            });
        }
    }
}