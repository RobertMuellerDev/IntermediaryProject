using ExtensionMethods;
using IntermediaryProject.Exceptions;
using IntermediaryProject.Products;
using IntermediaryProject.Utils;

namespace IntermediaryProject {
    static class Game {
        private static readonly List<Intermediary> _intermediaries = new();

        private static int s_numberOfIntermediaries;
        private static int _day;
        private static int s_numberOfDays;

        private static readonly List<Product> _availableProducts;
        private static Intermediary _currentIntermediary;

        public static List<Product> Products {
            get { return _availableProducts; }
        }

        static Game() {
            _availableProducts = ReadAvailableProductsFromFile();
            AskForAmountOfDaysToPlay();
            AskForNumberOfIntermediaries();
            CreateAndSaveIntermediaries();
            _day = 1;
            _currentIntermediary = _intermediaries[0];
            ExecuteProductChangeDayOperations();
        }

        private static List<Product> ReadAvailableProductsFromFile() {
            var ymlContent = Util.ReadFileToString("produkte.yml");
            return ParseYMLContentToProducts(ymlContent);
        }

        public static void Play() {
            while (_day <= s_numberOfDays) {
                foreach (var intermediary in _intermediaries) {
                    _currentIntermediary = intermediary;
                    if (IsBankrupt(intermediary)) {
                        UI.PrintBankruptcyNotification(intermediary);
                        continue;
                    }
                    PlayRound();
                }
                RemoveBankruptIntermediaries();
                if (IsGameOver()) {
                    break;
                }
                ChangeOfDay();
            }
            _intermediaries.Sort();
            UI.PrintLeaderboard(_intermediaries);
        }
        private static void RemoveBankruptIntermediaries() {
            int amountOfDeletedIntermediaries = _intermediaries.RemoveAll(IsBankrupt);
            s_numberOfIntermediaries -= amountOfDeletedIntermediaries;
        }

        private static bool IsBankrupt(Intermediary intermediary) {
            return (intermediary.Capital < 0);
        }

        private static void ChangeOfDay() {
            _day++;
            RotateFirstIntermediaryToTheEnd();
            ExecuteProductChangeDayOperations();
            ChargeStorageOperatingCosts();
        }

        private static bool IsGameOver() {
            return _intermediaries.Count == 0;
        }

        private static void ChargeStorageOperatingCosts() {
            foreach (var intermediary in _intermediaries) {
                intermediary.PayStorageOperatingCosts();
            }
        }

        private static void ExecuteProductChangeDayOperations() {
            foreach (var product in _availableProducts) {
                product.ProduceProduct();
                product.CalculatePurchasePrice();
            }
        }

        private static void RotateFirstIntermediaryToTheEnd() {
            var firstElement = _intermediaries.Pop(0);
            _intermediaries.Add(firstElement);
        }

        private static void PlayRound() {
            var roundFinished = false;
            do {
                UI.PrintHeader(_currentIntermediary, _day);
                UI.PrintGameMenuActions();
                var selectedAction = AskForAction();
                roundFinished = PerformSelectedAction(selectedAction);
            } while (!roundFinished);
        }

        private static bool PerformSelectedAction(GameAction selectedAction) {
            switch (selectedAction) {
                case GameAction.Shopping:
                    StartShoppingAction();
                    return false;
                case GameAction.Selling:
                    StartSellingAction();
                    return false;
                case GameAction.Storage:
                    StartStorageIncrease();
                    return false;
                case GameAction.EndRound:
                    return true;
                default:
                    return false;
            }
        }

        private static void StartStorageIncrease() {
            Console.Write("Um wieviel soll die Lagerkapazität vergrößert werden? ");
            do {
                var size = ReadAndValidateStringFromReadLine("Geben Sie eine gültige Zahl ein: ");
                if (!int.TryParse(size, out var parsedSize)) continue;
                if (parsedSize > 0) {
                    try {
                        _currentIntermediary.IncreaseStorage(parsedSize);
                    } catch (ArgumentOutOfRangeException e) {
                        Console.WriteLine(e.Message);
                    }
                }
                break;
            } while (true);
        }

        private static void StartSellingAction() {
            do {
                UI.PrintItemsToSell(_currentIntermediary);
                var input = AskForSellingAction();
                if (input.ToLower()[0] == 'z') {
                    break;
                }
            } while (true);
        }

        private static string AskForSellingAction() {
            var input = ReadAndValidateStringFromReadLine("Wählen Sie eine Option aus: ");

            if (byte.TryParse(input, out var parsedId) &&
                parsedId > 0 &&
                _currentIntermediary.Inventory.ContainsKey(parsedId)) {
                SellSelectedProduct(parsedId);
            }
            return input;
        }

        private static void SellSelectedProduct(byte parsedId) {
            Console.Write($"Wieviele {_availableProducts[parsedId - 1].Name}n möchten Sie verkaufen? ");
            do {
                var quantity = ReadAndValidateStringFromReadLine("Geben Sie eine gültige Anzahl ein: ");
                if (!int.TryParse(quantity, out var parsedQuantity)) continue;
                if (parsedQuantity > 0) {
                    SellSelectedQuantityOfProduct(_availableProducts[parsedId - 1], parsedQuantity);
                }

                break;
            } while (true);
        }

        private static void SellSelectedQuantityOfProduct(Product product, int quantity) {
            try {
                _currentIntermediary.SellProducts(product, quantity);
            } catch (ArgumentOutOfRangeException e) {
                Console.WriteLine(e.Message);
            }
        }

        private static void StartShoppingAction() {
            do {
                UI.PrintShop(_availableProducts);

                var input = AskForShoppingAction();
                if (input.ToLower()[0] == 'z') {
                    break;
                }
            } while (true);
        }

        private static string AskForShoppingAction() {
            var input = ReadAndValidateStringFromReadLine("Wählen Sie eine Option aus: ");

            if (byte.TryParse(input, out var parsedId) &&
                parsedId > 0 &&
                parsedId <= _availableProducts.Count) {
                BuySelectedProduct(parsedId);
            }
            return input;
        }

        private static void BuySelectedProduct(byte parsedId) {
            Console.Write($"Wieviele {_availableProducts[parsedId - 1].Name}n möchten Sie kaufen? ");
            do {
                var quantity = ReadAndValidateStringFromReadLine("Geben Sie eine gültige Anzahl ein: ");
                if (!int.TryParse(quantity, out var parsedQuantity)) continue;
                if (parsedQuantity > 0) {
                    BuyProduct(_availableProducts[parsedId - 1], parsedQuantity);
                }

                break;
            } while (true);
        }

        private static void BuyProduct(Product product, int quantity) {
            try {
                product.ReduceAvailabilityWhenBuying(quantity);
                _currentIntermediary.BuyProducts(product, quantity);
            } catch (Exception e) when (e is ProductNotAvailableException || e is IntermediaryBuyException) {
                if (e is IntermediaryBuyException) {
                    product.ReverseBuyingProcess(quantity);
                }
                Console.WriteLine(e.Message);
            }
        }

        private static GameAction AskForAction() {
            GameAction? selectedAction = null;
            do {
                var input = ReadAndValidateStringFromReadLine("Wählen Sie eine Option aus: ");
                foreach (var gameAction in Enum.GetValues(typeof(GameAction))
                                               .Cast<GameAction>()) {
                    if (input.ToLower()[0] == (char)(gameAction)) {
                        selectedAction = gameAction;
                    }
                }
            } while (selectedAction is null);
            return (GameAction)selectedAction;
        }

        private static void CreateAndSaveIntermediaries() {
            for (var i = 1; i <= s_numberOfIntermediaries; i++) {
                Console.Write($"Name von Zwischenhändler {i}: ");
                var intermediaryName = ReadAndValidateStringFromReadLine("Geben Sie einen gültigen Namen ein: ");
                Console.Write($"Name von der Firma von {intermediaryName}: ");
                var intermediaryCompanyName = ReadAndValidateStringFromReadLine("Geben Sie eine gültige Firma ein: ");
                var difficultyLevel = AskForDifficultyLevel();
                _intermediaries.Add(new Intermediary(intermediaryName, intermediaryCompanyName, (int)difficultyLevel));
            }
        }

        private static DifficultyLevel AskForDifficultyLevel() {
            Console.WriteLine();
            Console.WriteLine("Wählen Sie einen Schwierigkeitsgrad aus: ");
            UI.PrintDifficultyLevelChoice();
            return GetValidatedDifficultyLevelFromInput();
        }

        private static DifficultyLevel GetValidatedDifficultyLevelFromInput() {
            DifficultyLevel? difficultyLevel = null;
            do {
                var difficultyLevelInput =
                    ReadAndValidateStringFromReadLine("Geben Sie einen gültigen Schwierigkeitsgrad ein: ");
                difficultyLevel = difficultyLevelInput.ToLower() switch {
                    "a" => DifficultyLevel.Einfach,
                    "b" => DifficultyLevel.Normal,
                    "c" => DifficultyLevel.Schwer,
                };
            } while (difficultyLevel is null);

            return (DifficultyLevel)difficultyLevel;
        }

        private static string ReadAndValidateStringFromReadLine(string errorMessageForInvalidInput) {
            while (true) {
                var input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input)) return input;
                Console.Write(errorMessageForInvalidInput);
            }
        }

        private static void AskForNumberOfIntermediaries() {
            Console.Write("Wieviel Zwischenhändler nehmen teil?: ");
            while (true) {
                var input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input) ||
                    !int.TryParse(input, out s_numberOfIntermediaries) ||
                    s_numberOfIntermediaries <= 0) {
                    Console.Write("Geben Sie eine positive Zahl ein: ");
                    continue;
                }
                break;
            }
        }

        private static void AskForAmountOfDaysToPlay() {
            Console.Write("Wieviele Tage wollen Sie spielen?: ");
            while (true) {
                var input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input) ||
                    !int.TryParse(input, out s_numberOfDays) ||
                    s_numberOfDays <= 0) {
                    Console.Write("Geben Sie eine positive Zahl ein: ");
                    continue;
                }
                break;
            }
        }

        private static List<Product> ParseYMLContentToProducts(string ymlContent) {
            var products = Product.GetEnumerableOfIndividualProductsFromYmlContent(ymlContent);
            return Product.ConvertProductStringEnumerableToProductList(products);
        }
    }
}
