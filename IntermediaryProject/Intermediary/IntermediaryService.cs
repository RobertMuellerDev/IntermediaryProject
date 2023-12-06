using IntermediaryProject.Exceptions;
using IntermediaryProject.Products;

namespace IntermediaryProject;

public static class IntermediaryService {
    private static readonly int s_storagePricePerUnit = 50;

    internal static void BuyProducts(Intermediary intermediary, Product product, int quantity) {
        if (intermediary.Capital < (product.Price * quantity)) {
            throw new IntermediaryBuyException(
                $"Es ist nicht genug Kapital vorhanden, um {quantity:n0}-mal {product.Name} zu kaufen!"
            );
        }

        if (quantity > intermediary.AvailableStorageCapacity) {
            throw new IntermediaryBuyException(
                $"Es ist nicht genug Lagerkapazität vorhanden, um {quantity:n0}-mal {product.Name} zu kaufen!"
            );
        }

        intermediary.Capital -= product.Price * quantity;
        intermediary.StorageUtilization += quantity;
        if (intermediary.Inventory.ContainsKey(product.Id)) {
            intermediary.Inventory[product.Id] += quantity;
        } else {
            intermediary.Inventory.Add(product.Id, quantity);
        }
    }

    internal static void SellProducts(Intermediary intermediary, Product product, int quantity) {
        if (!intermediary.Inventory.ContainsKey(product.Id)) {
            throw new ArgumentOutOfRangeException("Id", "Dieses Produkt hat der Händler nicht auf Lager!");
        }

        if (intermediary.Inventory[product.Id] < quantity) {
            throw new ArgumentOutOfRangeException(
                nameof(quantity),
                "Die angefragte Menge übersteigt den vorhandenen Lagerbestand!"
            );
        }

        intermediary.Capital += product.SellingPrice * quantity;
        intermediary.StorageUtilization -= quantity;
        if (intermediary.Inventory[product.Id] == quantity) {
            intermediary.Inventory.Remove(product.Id);
        } else {
            intermediary.Inventory[product.Id] -= quantity;
        }
    }

    internal static void IncreaseStorage(Intermediary intermediary, int storageExpansionSize) {
        if (intermediary.Capital < (s_storagePricePerUnit * storageExpansionSize)) {
            throw new ArgumentOutOfRangeException(
                nameof(storageExpansionSize),
                $"Es ist nicht genug Kapital vorhanden, um {storageExpansionSize:n0} Lagereinheiten zu kaufen!"
            );
        }

        intermediary.Capital -= s_storagePricePerUnit * storageExpansionSize;
        intermediary.StorageCapacity += storageExpansionSize;
    }

    internal static void PayStorageOperatingCosts(Intermediary intermediary) {
        int storageOperatingCosts = intermediary.StorageUtilization * 5;
        storageOperatingCosts += intermediary.AvailableStorageCapacity * 1;

        intermediary.Capital -= storageOperatingCosts;
    }
}
