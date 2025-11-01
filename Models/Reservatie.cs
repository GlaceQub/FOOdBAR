using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.Models
{
    public class Reservatie
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("CustomUser")]
        public string KlantId { get; set; }

        [Column(TypeName = "date")]
        public DateTime? Datum { get; set; }

        public int AantalPersonen { get; set; }

        [ForeignKey("TijdSlot")]
        public int TijdSlotId { get; set; }

        public string? Opmerking { get; set; }
        public bool Bestaald { get; set; }
        public bool IsAanwezig { get; set; }

        [Column(TypeName = "date")]
        public DateTime? WelkomstmailVerstuurdOp { get; set; }

        public int EvaluatieAantalSterren { get; set; }
        public string? EvaluatieOpmerkingen { get; set; }

        // Navigation properties
        [JsonIgnore]
        public virtual CustomUser CustomUser { get; set; }
        [JsonIgnore]
        public virtual Tijdslot Tijdslot { get; set; }
        [JsonIgnore]
        public virtual ICollection<TafelLijst> Tafellijsten { get; set; } = new List<TafelLijst>();
        [JsonIgnore]
        public virtual ICollection<Bestelling> Bestellingen { get; set; } = new List<Bestelling>();

    }
}
