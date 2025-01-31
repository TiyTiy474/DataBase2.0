using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataBase2._0
{
    public class Disc
    {
        public int ID { get; set; }
        public string DiscName { get; set; }
        public string ArtistName { get; set; }
        public string Genre { get; set; }
        public decimal Price { get; set; }
        public int ArtistId { get; set; }
        public int GenreId { get; set; }
        public bool IsKaraoke { get; set; }
        public decimal PurchasePrice { get; set; }

        // Навигационные свойства
        public Artist Artist { get; set; }
        public virtual Sale Sale { get; set; }
        public virtual Genre GenreNavigation { get; set; }
    }

    public class Artist
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public virtual List<Disc> Discs { get; set; } = new List<Disc>();
    }

    public class Genre
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        
        public virtual List<Disc> Discs { get; set; } = new List<Disc>();
    }

    public class DiscountCard
    {
        public int ID { get; set; }
        public string CardNumber { get; set; }
        public string OwnerName { get; set; }
        public decimal DiscountPercentage { get; set; }
        public virtual List<Sale> Sales { get; set; } = new List<Sale>();
        public DateTime IssueDate { get; set; }
    }

    public class Employer
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
        
        public virtual List<Sale> Sales { get; set; } = new List<Sale>();
    }

    public class Sale
    {
        public int ID { get; set; }
        public int DiscId { get; set; }
        public int SellerID { get; set; }
        public string Location { get; set; }
        public DateTime SaleDate { get; set; }
        public decimal SalePrice { get; set; }
        public int? DiscountCardID { get; set; } // Делаем поле nullable
    
        // Навигационные свойства
        public virtual Disc SoldDisc { get; set; }
        public virtual Employer Seller { get; set; }
        public virtual DiscountCard DiscountCard { get; set; }
    }
}