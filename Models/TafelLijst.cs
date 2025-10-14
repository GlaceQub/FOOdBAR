using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.Models
{
    public class TafelLijst
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Reservatie")]
        public int ReservatieId { get; set; }

        [ForeignKey("Tafel")]
        public int TafelId { get; set; }

        // Navigation properties
        [JsonIgnore]
        public virtual Reservatie Reservatie { get; set; }
        [JsonIgnore]
        public virtual Tafel Tafel { get; set; }
    }
}
