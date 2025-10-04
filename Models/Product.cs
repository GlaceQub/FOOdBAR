using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public string? Naam { get; set; }
        public string? Beschrijving { get; set; }
        public string? AllergenenInfo { get; set; }

        [ForeignKey("Categorie")]
        public int CategorieId { get; set; }

        public bool Actief { get; set; }
        public bool IsSuggestie { get; set; }

        // Navigation properties
        [JsonIgnore]
        public virtual Categorie Categorie { get; set; }
        [JsonIgnore]
        public virtual ICollection<PrijsProduct> PrijsProducten { get; set; } = new List<PrijsProduct>();
        [JsonIgnore]
        public virtual ICollection<Bestelling> Bestellingen { get; set; } = new List<Bestelling>();

    }
}
