namespace Restaurant.ViewModels.Quiz
{
    public class QuizGerechtViewModel
    {
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

        // Quiz vragen
        public static readonly List<string> Vragen = new()
        {
            "Hou je van zoete gerechten?",
            "Hou je van zoute gerechten?",
            "Hou je van bittere smaken in je gerecht",
            "Hou je van frisse gerechten?",
            "Hou je van pikante gerechten?",
            "Eet je graag warme gerechten?",
            "Eet je graag koude gerechten?",
            "Heb je liever lichte of zware gerechten?",
            "Hou je van romige gerechten?",
            "Hou je van exotische of kruidige gerechten?"
        };
    }
}
