namespace Restaurant.Models
{
    public class Quiz
    {
        public int Id { get; set; }
        public int ProductId { get; set; }

        // Smaak- en producteigenschappen (0 = niet, 1 = beetje, 2 = veel)
        public int IsZoet { get; set; }
        public int IsZout { get; set; }
        public int IsBitter { get; set; }
        public int IsFris { get; set; }
        public int IsPikant { get; set; }
        public int IsAlcoholisch { get; set; }
        public int IsWarm { get; set; }
        public int IsKoud { get; set; }
        public int IsLicht { get; set; }
        public int IsZwaar { get; set; }
        public int IsRomig { get; set; }
        public int IsFruitig { get; set; }
        public int IsKruidig { get; set; }
        public int IsExotisch { get; set; }

        // Navigatie naar het bijbehorende product
        public Product Product { get; set; }
    }
}
