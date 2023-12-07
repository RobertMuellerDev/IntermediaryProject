using System.Text;
using IntermediaryProject.Products;
using IntermediaryProject.Transactions;

namespace IntermediaryProject.Utils {
    public static class Util {
        internal static string ReadFileToString(string fileName) {
            var basePath =
                AppDomain.CurrentDomain.BaseDirectory[..AppDomain.CurrentDomain.BaseDirectory.IndexOf(
                    "bin",
                    StringComparison.Ordinal
                )];
            var path = basePath + fileName;

            using var streamReader = new StreamReader(path, Encoding.UTF8);
            var readContents = streamReader.ReadToEnd();

            return readContents;
        }

        internal static string DetermineDisplaytextByGameAction(GameAction gameAction) {
            return gameAction switch {
                GameAction.Shopping => "Einkaufen",
                GameAction.Selling  => "Verkaufen",
                GameAction.EndRound => "Runde beenden",
                GameAction.Storage  => "Lager vergrößern",
                _                   => throw new Exception($"No GameAction mapping for {gameAction} available!"),
            };
        }

        internal static decimal CalculateTotalAmountOfTheDayForTransactionType(
            IEnumerable<Transaction> transactionsOfThePreviousDay,
            TransactionType transactionType
        ) {
            var shoppingAmount = transactionsOfThePreviousDay.Where(transaction => transaction.Type == transactionType)
                                                             .Aggregate<Transaction, decimal>(
                                                                 0m,
                                                                 (sum, transaction) => sum + transaction.Amount
                                                             );
            return shoppingAmount;
        }

        internal static Dictionary<int, int> CalculateDiscounts(Intermediary intermediary, List<Product> products) {
            var discounts = new Dictionary<int, int>();
            foreach (var id in products.Select(product => product.Id)) {
                var amount = intermediary.Inventory.GetValueOrDefault(id, 0);
                var discount = MapAmountToDiscount(amount);
                discounts.Add(id, discount);
            }

            return discounts;
        }

        private static int MapAmountToDiscount(int amount) {
            if (amount >= 75) {
                return 10;
            } else if (amount >= 50) {
                return 5;
            } else if (amount >= 25) {
                return 2;
            }

            return 0;
        }

        internal static ReportData PrepareReportData(Intermediary intermediary) {
            var shoppingCosts = Util.CalculateTotalAmountOfTheDayForTransactionType(
                intermediary.TransactionsOfTheDay,
                TransactionType.Shopping
            );
            var sellingRevenue = Util.CalculateTotalAmountOfTheDayForTransactionType(
                intermediary.TransactionsOfTheDay,
                TransactionType.Selling
            );
            var storageCosts = Util.CalculateTotalAmountOfTheDayForTransactionType(
                intermediary.TransactionsOfTheDay,
                TransactionType.Storage
            );
            var previousCapital = intermediary.Capital + shoppingCosts - sellingRevenue + storageCosts;
            var currentCapital = intermediary.Capital;
            return new ReportData(
                shoppingCosts,
                sellingRevenue,
                storageCosts,
                previousCapital,
                currentCapital
            );
        }

        public static bool IsBankrupt(Intermediary intermediary) {
            return (intermediary.Capital < 0);
        }
    }
}
