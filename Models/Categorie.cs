using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.Models
{
    public class Categorie
    {
        [Key]
        public int Id { get; set; }
        public string? Naam { get; set; }
        public bool Actief { get; set; }

        public int TypeId { get; set; }

        // Navigation properties
        [JsonIgnore]
        public virtual CategorieType Type { get; set; }
        [JsonIgnore]
        public virtual ICollection<Product> Producten { get; set; } = new List<Product>();

    }
}
