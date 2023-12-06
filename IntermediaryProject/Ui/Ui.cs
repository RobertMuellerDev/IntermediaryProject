using System.Text;
using IntermediaryProject.Products;
using IntermediaryProject.Utils;

namespace IntermediaryProject {
    public interface IUi {
        void Write(string output);
        void WriteLine();
        void WriteLine(string output);
        string? ReadLine();
        string ReadAndValidateStringFromReadLine(string errorMessageForInvalidInput);
        void PrintGameMenuActions();
        void PrintHeader(Intermediary intermediary, int day);
        void PrintDifficultyLevelChoice();
        void PrintShop(List<Product> availableProducts);
        void PrintItemsToSell(Intermediary intermediary, List<Product> products);
        void PrintBankruptcyNotification(Intermediary intermediary);
        void PrintLeaderboard(List<Intermediary> intermediaries);
    }

    public class Ui : IUi {
        public void Write(string output) {
            Console.Write(output);
        }

        public void WriteLine() {
            Console.WriteLine();
        }

        public void WriteLine(string output) {
            Console.WriteLine(output);
        }

        public string? ReadLine() {
            return Console.ReadLine();
        }

        public string ReadAndValidateStringFromReadLine(string errorMessageForInvalidInput) {
            while (true) {
                var input = ReadLine();
                if (!string.IsNullOrWhiteSpace(input)) {
                    return input;
                }

                Write(errorMessageForInvalidInput);
            }
        }

        public void PrintGameMenuActions() {
            Console.WriteLine(BuildGameMenuActionsString());
        }

        private string BuildGameMenuActionsString() {
            StringBuilder stringBuilder = new();
            foreach (var gameAction in Enums.ToList<GameAction>()) {
                stringBuilder.Append($"\n{(char)(gameAction)}) {Util.DetermineDisplaytextByGameAction(gameAction)}");
            }

            return stringBuilder.ToString();
        }

        public void PrintHeader(Intermediary intermediary, int day) {
            Console.WriteLine();
            Console.Write($"{intermediary.Name} von {intermediary.CompanyName} ");
            Console.Write($"| ${intermediary.Capital:n0} ");
            Console.Write($"| Lager: {intermediary.StorageUtilization}/{intermediary.StorageCapacity} ");
            Console.WriteLine($"| Tag {day}");
        }

        public void PrintDifficultyLevelChoice() {
            Console.Write(BuildDifficultyLevelString());
        }

        private string BuildDifficultyLevelString() {
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

        public void PrintShop(List<Product> availableProducts) {
            Console.WriteLine();
            Console.Write(BuildShoppingMenuString(availableProducts));
        }

        private string BuildShoppingMenuString(List<Product> availableProducts) {
            StringBuilder stringBuilder = new();
            stringBuilder.AppendLine("Verfügbare Produkte:");

            foreach (var product in availableProducts) {
                stringBuilder.AppendLine(product.ToString());
            }

            AppendTradingOptions(stringBuilder);
            return stringBuilder.ToString();
        }

        private void AppendTradingOptions(StringBuilder stringBuilder) {
            stringBuilder.AppendLine("\nz) Zurück");
        }

        public void PrintItemsToSell(Intermediary intermediary, List<Product> products) {
            Console.WriteLine();
            Console.Write(BuildSellingMenuString(intermediary, products));
        }

        private string BuildSellingMenuString(Intermediary intermediary, List<Product> products) {
            StringBuilder stringBuilder = new();
            stringBuilder.AppendLine("Produkte im Besitz:");

            var sellableProducts = products.Where(product => intermediary.Inventory.ContainsKey(product.Id));
            foreach (var sellableProduct in sellableProducts) {
                var quantity = intermediary.Inventory[sellableProduct.Id];
                stringBuilder.AppendLine(sellableProduct.CreateSalesString(quantity));
            }

            AppendTradingOptions(stringBuilder);

            return stringBuilder.ToString();
        }

        public void PrintBankruptcyNotification(Intermediary intermediary) {
            Console.WriteLine();
            Console.Write($"{intermediary.Name} von {intermediary.CompanyName} ist Bankrott!");
            Console.WriteLine();
        }

        public void PrintLeaderboard(List<Intermediary> intermediaries) {
            Console.WriteLine();
            Console.WriteLine("Rangliste:");
            for (int i = 0; i < intermediaries.Count; i++) {
                Console.WriteLine(
                    $"{i + 1}. Platz: {intermediaries[i].Name} von {intermediaries[i].CompanyName} mit ${intermediaries[i].Capital:C0}"
                );
            }
        }
    }
}
