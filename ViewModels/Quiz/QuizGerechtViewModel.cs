namespace Restaurant.ViewModels.Quiz
{
    public class QuizGerechtViewModel
    {
        public int CurrentQuestion { get; set; } = 0;

        public int Zoet { get; set; }
        public int Zout { get; set; }
        public int Bitter { get; set; }
        public int Fris { get; set; }
        public int Pikant { get; set; }
        public int Warm { get; set; }
        public int Koud { get; set; }
        public int Licht { get; set; }
        public int Zwaar { get; set; }
        public int Romig { get; set; }
        public int Fruitig { get; set; }
        public int Kruidig { get; set; }
        public int Exotisch { get; set; }

        // Quiz vragen — één vraag per eigenschap
        public static readonly List<string> Vragen = new()
        {
            "Hou je van zoete gerechten?",          // IsZoet
            "Hou je van zoute gerechten?",          // IsZout
            "Hou je van bittere smaken in je gerecht?", // IsBitter
            "Hou je van frisse gerechten?",         // IsFris
            "Hou je van pikante gerechten?",        // IsPikant
            "Eet je graag warme gerechten?",        // IsWarm
            "Eet je graag koude gerechten?",        // IsKoud
            "Hou je van lichte gerechten?",         // IsLicht
            "Hou je van stevige/zware gerechten?",  // IsZwaar
            "Hou je van romige gerechten?",         // IsRomig
            "Hou je van fruitige smaken in gerechten?", // IsFruitig
            "Hou je van kruidige gerechten?",       // IsKruidig
            "Hou je van exotische gerechten?"       // IsExotisch
        };
    }
}