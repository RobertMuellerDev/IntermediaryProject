using System.Text;
using IntermediaryProject.Products;
using IntermediaryProject.Utils;

namespace IntermediaryProject {
    public static class UI {
        public static void PrintGameMenuOptions() {
            Console.WriteLine(BuildGameMenuOptionsString());
        }

        private static string BuildGameMenuOptionsString() {
            StringBuilder stringBuilder = new();
            foreach (var gameOption in Enums.ToList<GameOption>()) {
                stringBuilder.Append($"\n{(char)(gameOption)}) {Util.GameOptionEnumDisplayNameMapping(gameOption)}");
            }
            return stringBuilder.ToString();
        }
        public static void PrintHeader(Intermediary intermediary, int day) {
            Console.WriteLine();
            Console.Write($"{intermediary.Name} von {intermediary.CompanyName} ");
            Console.Write($"| ${intermediary.Capital:n0} ");
            Console.Write($"| Lager: {intermediary.StorageUtilization}/{intermediary.StorageCapacity} ");
            Console.WriteLine($"| Tag {day}");

            // Console.WriteLine($"{intermediary.Name} von {intermediary.CompanyName} | ${intermediary.Capital:n0} | Lager: {intermediary.StorageUtilization}/{intermediary.StorageCapacity} | Tag {day}");
        }

        public static void PrintDifficultyLevelChoice() {
            Console.Write(BuildDifficultyLevelString());
        }

        private static string BuildDifficultyLevelString() {
            StringBuilder stringBuilder = new();
            byte charCounter = 0;
            foreach (var difficultyLevel in Enum.GetValues(typeof(DifficultyLevel))
                                                .Cast<DifficultyLevel>()
                                                .Reverse()) {
                stringBuilder.AppendLine(
                                         $"{(char)(charCounter + 97)}) Schwierigkeitsgrad: {difficultyLevel} -> Startkapital: ${(int)difficultyLevel:n0}"
                                        );
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
            AppendTradingOptions(stringBuilder);
            return stringBuilder.ToString();
        }

        private static void AppendTradingOptions(StringBuilder stringBuilder) {
            stringBuilder.AppendLine($"\nz) Zurück");
        }

        internal static void PrintItemsToSell(Intermediary intermediary) {
            Console.WriteLine();
            Console.Write(BuildSellingMenuString(intermediary));
        }

        private static string BuildSellingMenuString(Intermediary intermediary) {
            StringBuilder stringBuilder = new();
            stringBuilder.AppendLine("Produkte im Besitz:");

            var sellableProducts = Game.ProductList.Where(product => intermediary.Inventory.ContainsKey(product.Id));
            foreach (var sellableProduct in sellableProducts) {
                var quantity = intermediary.Inventory[sellableProduct.Id];
                stringBuilder.AppendLine(sellableProduct.ToSellingString(quantity));
            }
            AppendTradingOptions(stringBuilder);

            return stringBuilder.ToString();
        }
    }
}
