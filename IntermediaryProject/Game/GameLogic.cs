using IntermediaryProject.Exceptions;
using IntermediaryProject.Products;

namespace IntermediaryProject;

class GameLogic {
    private readonly IUi _ui;

    public GameLogic(IUi ui) {
        _ui = ui;
    }

    internal bool PerformSelectedAction(
        GameAction selectedAction,
        Intermediary intermediary,
        List<Product> products,
        int day
    ) {
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
            case GameAction.Loan:
                StartLoanTakeout(intermediary, day);
                return false;
            case GameAction.EndRound:
                return true;
            default:
                return false;
        }
    }

    private void StartLoanTakeout(Intermediary intermediary, int day) {
        var loanOptions = GetLoanOptions();
        _ui.PrintLoanOptions(loanOptions);
        do {
            var input = _ui.ReadAndValidateStringFromReadLine("Wählen Sie einen gültigen Kreditbetrag aus: ");
            if (input.ToLower()[0] == 'z') {
                break;
            }

            if (!int.TryParse(input, out var parsedValue) || parsedValue <= 0 || parsedValue > loanOptions.Count) {
                continue;
            }

            try {
                BusinessLogic.TakeOutLoan(intermediary, loanOptions[parsedValue], day);
            } catch (IntermediaryLoanException e) {
                Console.WriteLine(e);
                break;
            }
        } while (true);
    }

    private static Dictionary<int, (int amount, int interest)> GetLoanOptions() {
        var loanOptions =
            new Dictionary<int, (int amount, int interest)>() {
                [1] = (5_000, 3), [2] = (10_000, 5), [3] = (25_000, 8)
            };

        return loanOptions;
    }

    private void StartStorageIncrease(Intermediary intermediary) {
        _ui.Write("Um wie viel soll die Lagerkapazität vergrößert werden? ");
        do {
            var size = _ui.ReadAndValidateStringFromReadLine("Geben Sie eine gültige Zahl ein: ");
            if (!int.TryParse(size, out var parsedSize)) continue;
            if (parsedSize > 0) {
                try {
                    IntermediaryService.ExpandStorage(intermediary, parsedSize);
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
            var input = _ui.ReadAndValidateStringFromReadLine("Wählen Sie eine Option aus: ");
            if (input.ToLower()[0] == 'z') {
                break;
            }

            if (!byte.TryParse((string?)input, out var parsedId) ||
                parsedId <= 0 ||
                !intermediary.Inventory.ContainsKey(parsedId))
                continue;
            var quantity = AskHowMuchOfSelectedProductIsToBeSold(products[parsedId - 1].Name);
            if (quantity > 0) {
                try {
                    BusinessLogic.SellSelectedQuantityOfProduct(intermediary, products[parsedId - 1], quantity);
                } catch (ArgumentOutOfRangeException e) {
                    _ui.WriteLine(e.Message);
                }
            }
        } while (true);
    }

    private int AskHowMuchOfSelectedProductIsToBeSold(string productName) {
        _ui.Write($"Wie viele {productName}n möchten Sie verkaufen? ");
        do {
            var quantity = _ui.ReadAndValidateStringFromReadLine("Geben Sie eine gültige Anzahl ein: ");
            if (!int.TryParse(quantity, out var parsedQuantity)) continue;
            return parsedQuantity;
        } while (true);
    }

    private void StartShoppingAction(Intermediary intermediary, List<Product> products) {
        do {
            _ui.PrintShop(products, intermediary);

            var input = _ui.ReadAndValidateStringFromReadLine("Wählen Sie eine Option aus: ");
            if (input.ToLower()[0] == 'z') {
                break;
            }

            if (!byte.TryParse((string?)input, out var parsedId) || parsedId <= 0 || parsedId > products.Count) {
                continue;
            }

            var quantity = AskHowMuchOfSelectedProductTheyWantToBuy(products[parsedId - 1].Name);

            if (quantity > 0) {
                BeginBuyingProcessForChosenAmountOfSelectedProductWithErrorHandling(
                    intermediary,
                    products[parsedId - 1],
                    quantity
                );
            }
        } while (true);
    }

    private void BeginBuyingProcessForChosenAmountOfSelectedProductWithErrorHandling(
        Intermediary intermediary,
        Product product,
        int quantity
    ) {
        try {
            BusinessLogic.BuyProduct(intermediary, product, quantity);
        } catch (Exception e) when (e is ProductNotAvailableException or IntermediaryBuyException) {
            _ui.WriteLine(e.Message);
        }
    }

    private int AskHowMuchOfSelectedProductTheyWantToBuy(string productName) {
        _ui.Write($"Wie viele {productName}n möchten Sie kaufen? ");
        do {
            var quantity = _ui.ReadAndValidateStringFromReadLine("Geben Sie eine gültige Anzahl ein: ");
            if (!int.TryParse(quantity, out var parsedQuantity)) continue;
            return parsedQuantity;
        } while (true);
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
