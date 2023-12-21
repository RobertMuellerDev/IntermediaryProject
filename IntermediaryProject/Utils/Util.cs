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
                GameAction.Loan     => "Kredit aufnehmen",
                _                   => throw new Exception($"No GameAction mapping for {gameAction} available!"),
            };
        }

        private static decimal CalculateTotalAmountOfTheDayForTransactionType(
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
            return amount switch {
                >= 75 => 10,
                >= 50 => 5,
                >= 25 => 2,
                _     => 0
            };
        }

        internal static ReportData PrepareReportData(Intermediary intermediary) {
            var shoppingCosts = CalculateTotalAmountOfTheDayForTransactionType(
                intermediary.TransactionsOfTheDay,
                TransactionType.Shopping
            );
            var sellingRevenue = CalculateTotalAmountOfTheDayForTransactionType(
                intermediary.TransactionsOfTheDay,
                TransactionType.Selling
            );
            var storageCosts = CalculateTotalAmountOfTheDayForTransactionType(
                intermediary.TransactionsOfTheDay,
                TransactionType.Storage
            );
            var loanCosts = CalculateTotalAmountOfTheDayForTransactionType(
                intermediary.TransactionsOfTheDay,
                TransactionType.PayLoan
            );

            var takenOutLoan = CalculateTotalAmountOfTheDayForTransactionType(
                intermediary.TransactionsOfTheDay,
                TransactionType.TakeLoan
            );

            var previousCapital = intermediary.Capital
                                  - takenOutLoan
                                  + shoppingCosts
                                  - sellingRevenue
                                  + storageCosts
                                  + loanCosts;
            return new ReportData(
                shoppingCosts,
                sellingRevenue,
                storageCosts,
                previousCapital,
                intermediary.Capital,
                loanCosts
            );
        }

        public static bool IsBankrupt(Intermediary intermediary) {
            return (intermediary.Capital < 0);
        }
    }
}
