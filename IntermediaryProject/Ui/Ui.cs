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
        ConsoleKeyInfo ReadKey();
        void PrintGameMenuActions();
        void PrintHeader(Intermediary intermediary, int day);
        void PrintDifficultyLevelChoice();
        void PrintShop(List<Product> availableProducts, Intermediary intermediary);
        void PrintItemsToSell(Intermediary intermediary, List<Product> products);
        void PrintBankruptcyNotification(Intermediary intermediary);
        void PrintLeaderboard(List<Intermediary> intermediaries);
        void PrintReport(ReportData reportData);
        void PrintLoanOptions(Dictionary<int, (int amount, int interest)> loanOptions);
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

        public ConsoleKeyInfo ReadKey() {
            return Console.ReadKey();
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
            Console.WriteLine("\n");
            Console.Write($"{intermediary.Name} von {intermediary.CompanyName} ");
            Console.Write($"| ${intermediary.Capital:n2} ");
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

        public void PrintShop(List<Product> availableProducts, Intermediary intermediary) {
            Console.WriteLine();
            Console.Write(BuildShoppingMenuString(availableProducts, intermediary));
        }

        private string BuildShoppingMenuString(List<Product> availableProducts, Intermediary intermediary) {
            StringBuilder stringBuilder = new();
            stringBuilder.AppendLine("Verfügbare Produkte:");

            foreach (var product in availableProducts) {
                stringBuilder.Append(product.ToString());
                stringBuilder.AppendLine($" {intermediary.Discounts[product.Id]}% Rabatt");
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
                    $"{i + 1}. Platz: {intermediaries[i].Name} von {intermediaries[i].CompanyName} mit ${intermediaries[i].Capital:n2}"
                );
            }
        }

        public void PrintReport(ReportData reportData) {
            Console.WriteLine();
            Console.WriteLine("Kontostand zu Beginn des letzten Tags: {0:n2}", reportData.PreviousCapital);
            Console.WriteLine("Ausgabe für Einkäufe des letzten Tags: {0:n2}", reportData.ShoppingCosts);
            Console.WriteLine("Einnahmen für Verkäufe des letzten Tags: {0:n2}", reportData.SellingRevenue);
            Console.WriteLine("Lagerkosten des letzten Tags: {0:n2}", reportData.StorageCosts);

            if (reportData.LoanCost > 0) {
                Console.WriteLine("Es wurde ein Kredit zurückgezahlt: {0:n2}", reportData.LoanCost);
            }

            Console.WriteLine("Aktueller Kontostand: {0:n2}", reportData.CurrentCapital);
            Console.Write("\nZum Bestätigen Enter drücken.");
        }

        public void PrintLoanOptions(Dictionary<int, (int amount, int interest)> loanOptions) {
            Console.WriteLine();
            Console.WriteLine("Wählen Sie den Kreditbetrag aus, welchen Sie aufnehmen möchten.");
            foreach (var (loanKey, loanOption) in loanOptions) {
                Console.WriteLine($"{loanKey}.) {loanOption.amount:n0} mit {loanOption.interest}% Zinsen");
            }

            Console.WriteLine("\nz) Zurück");
        }
    }
}
