using IntermediaryProject.Exceptions;
using IntermediaryProject.Products;
using IntermediaryProject.Transactions;

namespace IntermediaryProject;

public static class IntermediaryService {
    private static readonly int s_storagePricePerUnit = 50;

    internal static void BuyProduct(Intermediary intermediary, Product product, int quantity) {
        var buyingCosts = product.Price * quantity * (1 - intermediary.Discounts[product.Id] / 100m);
        if (intermediary.Capital < buyingCosts) {
            throw new IntermediaryBuyException(
                $"Es ist nicht genug Kapital vorhanden, um {quantity:n0}-mal {product.Name} zu kaufen!"
            );
        }

        if (quantity > intermediary.AvailableStorageCapacity) {
            throw new IntermediaryBuyException(
                $"Es ist nicht genug Lagerkapazität vorhanden, um {quantity:n0}-mal {product.Name} zu kaufen!"
            );
        }

        intermediary.Capital -= buyingCosts;
        intermediary.StorageUtilization += quantity;
        if (intermediary.Inventory.ContainsKey(product.Id)) {
            intermediary.Inventory[product.Id] += quantity;
        } else {
            intermediary.Inventory.Add(product.Id, quantity);
        }

        intermediary.TransactionsOfTheDay.Add(new Transaction(buyingCosts, TransactionType.Shopping));
    }

    internal static void SellProducts(Intermediary intermediary, Product product, int quantity) {
        if (!intermediary.Inventory.ContainsKey(product.Id)) {
            throw new IntermediarySellException("Dieses Produkt hat der Händler nicht auf Lager!");
        }

        if (intermediary.Inventory[product.Id] < quantity) {
            throw new IntermediarySellException("Die angefragte Menge übersteigt den vorhandenen Lagerbestand!");
        }

        var productSellingRevenue = product.SellingPrice * quantity;

        intermediary.Capital += productSellingRevenue;
        intermediary.StorageUtilization -= quantity;
        if (intermediary.Inventory[product.Id] == quantity) {
            intermediary.Inventory.Remove(product.Id);
        } else {
            intermediary.Inventory[product.Id] -= quantity;
        }

        intermediary.TransactionsOfTheDay.Add(new Transaction(productSellingRevenue, TransactionType.Selling));
    }

    internal static void ExpandStorage(Intermediary intermediary, int storageExpansionSize) {
        var storageExpansionCosts = s_storagePricePerUnit * storageExpansionSize;
        if (intermediary.Capital < storageExpansionCosts) {
            throw new IntermediaryExpandStorageException(
                $"Es ist nicht genug Kapital vorhanden, um {storageExpansionSize:n0} Lagereinheiten zu kaufen!"
            );
        }

        intermediary.Capital -= storageExpansionCosts;
        intermediary.StorageCapacity += storageExpansionSize;
        intermediary.TransactionsOfTheDay.Add(new Transaction(storageExpansionCosts, TransactionType.Storage));
    }

    internal static void PayStorageOperatingCosts(Intermediary intermediary) {
        var storageOperatingCosts = intermediary.StorageUtilization * 5;
        storageOperatingCosts += intermediary.AvailableStorageCapacity * 1;

        intermediary.Capital -= storageOperatingCosts;
        intermediary.TransactionsOfTheDay.Add(new Transaction(storageOperatingCosts, TransactionType.Storage));
    }

    private static bool HasTakenOutLoan(Intermediary intermediary) {
        return intermediary.TakenOutLoan != null;
    }

    internal static void TakeOutALoan(
        Intermediary intermediary,
        int amount,
        int interestRate,
        int day
    ) {
        if (HasTakenOutLoan(intermediary)) {
            throw new IntermediaryLoanException(
                "Der Händler hat bereits einen Kredit aufgenommen.\nEs können keine weiteren Kredite aufgenommen werden."
            );
        }

        intermediary.TakenOutLoan = new Loan(amount, interestRate, day);
        intermediary.TransactionsOfTheDay.Add(new Transaction(amount, TransactionType.TakeLoan));
        intermediary.Capital += amount;
    }

    internal static void PayBackLoan(Intermediary intermediary, int currentDay) {
        if (intermediary.TakenOutLoan == null || intermediary.TakenOutLoan.DayOfRepayment != currentDay) return;
        var amountToBeRepaid = intermediary.TakenOutLoan.Amount * (100 + intermediary.TakenOutLoan.InterestRate) / 100;
        intermediary.Capital -= amountToBeRepaid;
        intermediary.TransactionsOfTheDay.Add(new Transaction(amountToBeRepaid, TransactionType.PayLoan));
        intermediary.TakenOutLoan = null;
    }
}
