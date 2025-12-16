namespace Restaurant.ViewModels.Quiz
{
    public class QuizGerechtViewModel
    {
        public int CurrentQuestion { get; set; } = 0;

        public int IsZoet { get; set; }
        public int IsZout { get; set; }
        public int IsBitter { get; set; }
        public int IsFris { get; set; }
        public int IsPikant { get; set; }
        public int IsWarm { get; set; }
        public int IsKoud { get; set; }
        public int IsLicht { get; set; }
        public int IsZwaar { get; set; }
        public int IsRomig { get; set; }
        public int IsFruitig { get; set; }
        public int IsKruidig { get; set; }
        public int IsExotisch { get; set; }

        // Quiz vragen — nu 14 vragen, één per eigenschap (volgorde = properties)
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