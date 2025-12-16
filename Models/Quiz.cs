namespace Restaurant.Models
{
    public class Quiz
    {
        public int Id { get; set; }
        public int ProductId { get; set; }

        // Smaak- en producteigenschappen (0 = niet, 1 = beetje, 2 = veel)
        public int Zoet { get; set; }
        public int Zout { get; set; }
        public int Bitter { get; set; }
        public int Fris { get; set; }
        public int Pikant { get; set; }
        public int Alcoholisch { get; set; }
        public int Warm { get; set; }
        public int Koud { get; set; }
        public int Licht { get; set; }
        public int Zwaar { get; set; }
        public int Romig { get; set; }
        public int Fruitig { get; set; }
        public int Kruidig { get; set; }
        public int Exotisch { get; set; }

        // Navigatie naar het bijbehorende product
        public Product Product { get; set; }
    }
}
