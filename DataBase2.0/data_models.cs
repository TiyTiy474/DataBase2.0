using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataBase2._0
{
    public class Disc
    {
        public int ID { get; set; }
        public string DiscName { get; set; }
        public int ArtistId { get; set; }
        public string ArtistName { get; set; }
        public int GenreId { get; set; }
        public string Genre { get; set; }
        public decimal Price { get; set; }
        public bool IsKaraoke { get; set; }
        public decimal PurchasePrice { get; set; }
        public Artist Artist { get; set; }
        public Sale Sale { get; set; }
    }

    public class Artist
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public ICollection<Disc> Discs { get; set; }
    }

    public class Genre
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }

    public class Sale
    {
        public int ID { get; set; }
        public int DiscId { get; set; }
        public int SellerID { get; set; }
        public string Location { get; set; }
        public DateTime SaleDate { get; set; }
        public decimal SalePrice { get; set; }
        public int? DiscountCardID { get; set; }
        public Disc SoldDisc { get; set; }
        public Employer Seller { get; set; }
        public DiscountCard DiscountCard { get; set; }
    }

    public class Employer
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
        public ICollection<Sale> Sales { get; set; }
    }

    public class DiscountCard
    {
        public int ID { get; set; }
        public string CardNumber { get; set; }
        public string OwnerName { get; set; }
        public decimal DiscountPercentage { get; set; }
        public DateTime IssueDate { get; set; }
        public ICollection<Sale> Sales { get; set; }
    }
}