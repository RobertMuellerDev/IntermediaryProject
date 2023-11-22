using IntermediaryProject.Exceptions;
using IntermediaryProject.Products;

namespace IntermediaryProject;

class BusinessLogic {
    private readonly IUi _ui;

    public BusinessLogic(IUi ui) {
        _ui = ui;
    }

    internal bool PerformSelectedAction(GameAction selectedAction, Intermediary intermediary, List<Product> products) {
        switch (selectedAction) {
            case GameAction.Shopping:
                StartShoppingAction(intermediary, products);
                return false;
            case GameAction.Selling:
                StartSellingAction(intermediary, products);
                return false;
            case GameAction.Storage:
                StartStorageIncrease(intermediary);
                return false;
            case GameAction.EndRound:
                return true;
            default:
                return false;
        }
    }

    private void StartStorageIncrease(Intermediary intermediary) {
        _ui.Write("Um wieviel soll die Lagerkapazität vergrößert werden? ");
        do {
            var size = _ui.ReadAndValidateStringFromReadLine("Geben Sie eine gültige Zahl ein: ");
            if (!int.TryParse(size, out var parsedSize)) continue;
            if (parsedSize > 0) {
                try {
                    intermediary.IncreaseStorage(parsedSize);
                } catch (ArgumentOutOfRangeException e) {
                    _ui.WriteLine(e.Message);
                }
            }

            break;
        } while (true);
    }

    private void StartSellingAction(Intermediary intermediary, List<Product> products) {
        do {
            _ui.PrintItemsToSell(intermediary, products);
            var input = AskForSellingAction();
            if (input.ToLower()[0] == 'z') {
                break;
            }

            if (!byte.TryParse((string?)input, out var parsedId) ||
                parsedId <= 0 ||
                !intermediary.Inventory.ContainsKey(parsedId))
                continue;
            var quantity = AskHowMuchOfSelectedProductIsToBeSold(products[parsedId - 1].Name);
            if (quantity > 0) {
                SellSelectedQuantityOfProduct(intermediary, products[parsedId - 1], quantity);
            }
        } while (true);
    }

    private string AskForSellingAction() {
        var input = _ui.ReadAndValidateStringFromReadLine("Wählen Sie eine Option aus: ");
        return input;
    }

    private int AskHowMuchOfSelectedProductIsToBeSold(string productName) {
        _ui.Write($"Wieviele {productName}n möchten Sie verkaufen? ");
        do {
            var quantity = _ui.ReadAndValidateStringFromReadLine("Geben Sie eine gültige Anzahl ein: ");
            if (!int.TryParse(quantity, out var parsedQuantity)) continue;
            return parsedQuantity;
        } while (true);
    }

    private void SellSelectedQuantityOfProduct(Intermediary intermediary, Product product, int quantity) {
        try {
            intermediary.SellProducts(product, quantity);
        } catch (ArgumentOutOfRangeException e) {
            _ui.WriteLine(e.Message);
        }
    }

    private void StartShoppingAction(Intermediary intermediary, List<Product> products) {
        do {
            _ui.PrintShop(products);

            var input = AskForShoppingAction();
            if (input.ToLower()[0] == 'z') {
                break;
            }

            if (!byte.TryParse((string?)input, out var parsedId) ||
                parsedId <= 0 ||
                parsedId > products.Count) {
                continue;
            }

            var quantity = AskHowMuchOfSelectedProductTheyWantToBuy(products[parsedId - 1].Name);

            if (quantity > 0) {
                BuyProduct(intermediary, products[parsedId - 1], quantity);
            }
        } while (true);
    }

    private string AskForShoppingAction() {
        var input = _ui.ReadAndValidateStringFromReadLine("Wählen Sie eine Option aus: ");
        return input;
    }

    private int AskHowMuchOfSelectedProductTheyWantToBuy(string productName) {
        _ui.Write($"Wieviele {productName}n möchten Sie kaufen? ");
        do {
            var quantity = _ui.ReadAndValidateStringFromReadLine("Geben Sie eine gültige Anzahl ein: ");
            if (!int.TryParse(quantity, out var parsedQuantity)) continue;
            return parsedQuantity;
        } while (true);
    }

    private void BuyProduct(Intermediary intermediary, Product product, int quantity) {
        try {
            product.ReduceAvailabilityWhenBuying(quantity);
            intermediary.BuyProducts(product, quantity);
        } catch (Exception e) when (e is ProductNotAvailableException || e is IntermediaryBuyException) {
            if (e is IntermediaryBuyException) {
                product.ReverseBuyingProcess(quantity);
            }

            _ui.WriteLine(e.Message);
        }
    }

    internal GameAction AskForAction() {
        GameAction? selectedAction = null;
        do {
            var input = _ui.ReadAndValidateStringFromReadLine("Wählen Sie eine Option aus: ");
            foreach (var gameAction in Enum.GetValues(typeof(GameAction))
                                           .Cast<GameAction>()) {
                if (input.ToLower()[0] == (char)(gameAction)) {
                    selectedAction = gameAction;
                }
            }
        } while (selectedAction is null);

        return (GameAction)selectedAction;
    }
}
