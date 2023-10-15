using System.Text;
using IntermediaryProject.Products;
using IntermediaryProject.Utils;

namespace IntermediaryProject {
    public static class UI {

        public static void PrintGameMenu() {
            Console.WriteLine(BuildGameMenuString());
        }

        private static string BuildGameMenuString() {
            StringBuilder stringBuilder = new();
            foreach (var gameOption in Enums.ToList<GameOption>()) {
                stringBuilder.Append($"\n{(char)(gameOption)}) {Util.GameOptionEnumDisplayNameMapping(gameOption)}");
            }
            return stringBuilder.ToString();
        }
        public static void PrintHeader(Intermediary intermediary, int day) {
            Console.WriteLine();
            Console.WriteLine($"{intermediary.Name} von {intermediary.CompanyName} | ${intermediary.Capital:n0} | Tag {day}");
        }

        public static void PrintDifficultyLevelChoice() {
            Console.Write(BuildDifficultyLevelString());
        }

        private static string BuildDifficultyLevelString() {
            StringBuilder stringBuilder = new();
            byte charCounter = 0;
            foreach (var difficultyLevel in Enum.GetValues(typeof(DifficultyLevel)).Cast<DifficultyLevel>().Reverse()) {
                stringBuilder.AppendLine($"{(char)(charCounter + 97)}) Schwierigkeitsgrad: {difficultyLevel} -> Startkapital: ${(int)difficultyLevel:n0}");
                charCounter++;
            }
            return stringBuilder.ToString();
        }

        internal static void PrintShop(List<Product> availableProducts) {
            Console.WriteLine();
            Console.Write(BuildShoppingMenuString(availableProducts));
        }

        private static string BuildShoppingMenuString(List<Product> availableProducts) {
            StringBuilder stringBuilder = new();
            stringBuilder.AppendLine("Verfügbare Produkte:");

            foreach (var product in availableProducts) {
                stringBuilder.AppendLine(product.ToString());
            }
            foreach (var shoppingOption in Enum.GetValues(typeof(ShoppingOption)).Cast<ShoppingOption>().Reverse()) {
                stringBuilder.AppendLine($"\n{(char)(shoppingOption)}) {shoppingOption}");
            }
            return stringBuilder.ToString();
        }
    }
}
