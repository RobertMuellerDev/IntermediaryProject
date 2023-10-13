using System.Text;

namespace IntermediaryProject {
    public static class UI {

        public static void PrintGameMenu() {
            Console.WriteLine(BuildGameMenuString());
        }

        private static string BuildGameMenuString() {
            StringBuilder stringBuilder = new();
            stringBuilder.AppendLine($"{GameOption.a}) Nochmal");
            stringBuilder.Append($"{GameOption.b}) Runde beenden");
            return stringBuilder.ToString();
        }
        public static void PrintHeader(Intermediary intermediary, int day) {
            Console.WriteLine($"{intermediary.Name} von {intermediary.CompanyName} | ${intermediary.Capital:n0} | Tag {day}");
        }

        public static void PrintDifficultyLevelChoice() {
            Console.Write(BuildDifficultyLevelString());
        }

        private static string BuildDifficultyLevelString() {
            StringBuilder stringBuilder = new();
            byte charCounter = 0;
            foreach (var item in Enum.GetValues(typeof(DifficultyLevel)).Cast<DifficultyLevel>().Reverse()) {
                stringBuilder.AppendLine($"{(char)(charCounter + 97)}) Schwierigkeitsgrad: {item} -> Startkapital: ${(int)item:n0}");
                charCounter++;
            }
            return stringBuilder.ToString();
        }
    }
}
