namespace Restaurant.ViewModels.Quiz
{
    public class QuizDrankViewModel
    {
        public int IsZoet { get; set; }
        public int IsBitter { get; set; }
        public int IsFris { get; set; }
        public int IsAlcoholisch { get; set; }
        public int IsWarm { get; set; }
        public int IsKoud { get; set; }
        public int IsFruitig { get; set; }
        public int IsKruidig { get; set; }
        public int IsExotisch { get; set; }

        // Quiz vragen
        public static  readonly List<string> Vragen = new() {
        "Hou je van zoete dranken?",
        "Hou je van bittere dranken?",
        "Hou je van frisse dranken?",
        "Drink je graag alcoholische dranken?",
        "Drink je liever warme of koude dranken?",
        "Hou je van fruitige dranken?   ",
        "Hou je van exotische of kruidige dranken?"
        };
    }
}
