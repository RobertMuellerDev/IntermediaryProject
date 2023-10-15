using ExtensionMethods;
using IntermediaryProject.Products;
using IntermediaryProject.Utils;

namespace IntermediaryProject;

static class Game {
    private static readonly List<Intermediary> _intermediaries = new List<Intermediary>();

    private static byte _number_of_intermediaries;
    private static int _day;

    private static readonly List<Product> _availableProducts;
    private static Intermediary _currentIntermediary;

    public static List<Product> ProductList {
        get { return _availableProducts; }
    }

    static Game() {
        _availableProducts = ImportAvailableProducts();
        AskForNumberOfIntermediaries();
        CreateIntermediariesAndAddToList();
        _day = 1;
        _currentIntermediary = _intermediaries[0];
    }

    public static void Play() {
        while (true) {
            foreach (var intermediary in _intermediaries) {
                _currentIntermediary = intermediary;
                PlayRound();
            }
            _day++;
            ChangeIntermediariesOrder();
        }
    }
    private static void ChangeIntermediariesOrder() {
        var firstElement = _intermediaries.Pop(0);
        _intermediaries.Add(firstElement);
    }

    private static void PlayRound() {
        bool roundFinished = false;
        do {
            UI.PrintHeader(_currentIntermediary, _day);
            UI.PrintGameMenu();
            var selectedOption = AskUserForAction();
            roundFinished = ExecuteSelectedAction(selectedOption);

        } while (!roundFinished);

    }

    private static bool ExecuteSelectedAction(GameOption selectedOption) {
        switch (selectedOption) {
            case GameOption.Shopping:
                StartShopping();
                return false;
            case GameOption.Selling:
                StartSelling();
                return false;
            case GameOption.EndRound:
                return true;
            default:
                return false;

        }
    }

    private static void StartSelling() {
        do {
            UI.PrintItemsToSell(_currentIntermediary);
            var input = AskUserForSellingAction();
            if (input.ToLower()[0] == 'z') {
                break;
            }
        } while (true);
    }

    private static string AskUserForSellingAction() {
        var input = ReadAndValidateStringFromReadLine("Wählen Sie eine Option aus: ");

        if (byte.TryParse(input, out byte parsedId) && parsedId > 0 && _currentIntermediary.Inventory.ContainsKey(parsedId)) {
            SellSelectedProduct(parsedId);
        }
        return input;
    }

    private static void SellSelectedProduct(byte parsedId) {
        Console.Write($"Wieviele {_availableProducts[parsedId - 1].Name}n möchten Sie verkaufen? ");
        do {
            var quantity = ReadAndValidateStringFromReadLine("Geben Sie eine gültige Anzahl ein: ");
            if (int.TryParse(quantity, out int parsedQuantity)) {
                if (parsedQuantity > 0) {
                    SellProduct(_availableProducts[parsedId - 1], parsedQuantity);
                }

                break;
            }
        } while (true);
    }

    private static void SellProduct(Product product, int quantity) {
        try {
            _currentIntermediary.Sell(product, quantity);
        } catch (ArgumentOutOfRangeException e) {
            Console.WriteLine(e.Message);
        }
    }

    private static void StartShopping() {
        do {
            UI.PrintShop(_availableProducts);

            var input = AskUserForShoppingAction();
            if (input.ToLower()[0] == 'z') {
                break;
            }
        } while (true);
    }

    private static string AskUserForShoppingAction() {
        var input = ReadAndValidateStringFromReadLine("Wählen Sie eine Option aus: ");

        if (byte.TryParse(input, out byte parsedId) && parsedId > 0 && parsedId <= _availableProducts.Count) {
            BuySelectedProduct(parsedId);
        }
        return input;
    }

    private static void BuySelectedProduct(byte parsedId) {
        Console.Write($"Wieviele {_availableProducts[parsedId - 1].Name}n möchten Sie kaufen? ");
        do {
            var quantity = ReadAndValidateStringFromReadLine("Geben Sie eine gültige Anzahl ein: ");
            if (int.TryParse(quantity, out int parsedQuantity)) {
                if (parsedQuantity > 0) {
                    BuyProduct(_availableProducts[parsedId - 1], parsedQuantity);
                }

                break;
            }
        } while (true);
    }

    private static void BuyProduct(Product product, int quantity) {
        try {
            _currentIntermediary.Buy(product, quantity);
        } catch (InvalidOperationException e) {
            Console.WriteLine(e.Message);
        }
    }

    private static GameOption AskUserForAction() {
        GameOption? selectedOption = null;
        do {
            var input = ReadAndValidateStringFromReadLine("Wählen Sie eine Option aus: ");
            foreach (var gameOption in Enum.GetValues(typeof(GameOption)).Cast<GameOption>()) {
                if (input.ToLower()[0] == (char)(gameOption)) {
                    selectedOption = gameOption;
                }
            }

        } while (selectedOption is null);
        return (GameOption)selectedOption;
    }

    private static void CreateIntermediariesAndAddToList() {
        for (int i = 1; i <= _number_of_intermediaries; i++) {

            Console.Write($"Name von Zwischenhändler {i}: ");
            string intermediaryName = ReadAndValidateStringFromReadLine("Geben Sie einen gültigen Namen ein: ");
            Console.Write($"Name von der Firma von {intermediaryName}: ");
            string intermediaryCompanyName = ReadAndValidateStringFromReadLine("Geben Sie eine gültige Firma ein: ");
            DifficultyLevel difficultyLevel = AskUserForDifficultyLevel();
            _intermediaries.Add(new Intermediary(intermediaryName, intermediaryCompanyName, (int)difficultyLevel));
        }
    }

    private static DifficultyLevel AskUserForDifficultyLevel() {
        Console.WriteLine();
        Console.WriteLine($"Wählen Sie einen Schwierigkeitsgrad aus: ");
        UI.PrintDifficultyLevelChoice();
        return GetValidatedDifficultyLevelFromInput();
    }

    private static DifficultyLevel GetValidatedDifficultyLevelFromInput() {
        DifficultyLevel? difficultyLevel = null;
        do {
            string difficultyLevelInput = ReadAndValidateStringFromReadLine("Geben Sie einen gültigen Schwierigkeitsgrad ein: ");
            if (difficultyLevelInput.ToLower() == "a")
                difficultyLevel = DifficultyLevel.Einfach;
            else if (difficultyLevelInput.ToLower() == "b")
                difficultyLevel = DifficultyLevel.Normal;
            else if (difficultyLevelInput.ToLower() == "c")
                difficultyLevel = DifficultyLevel.Schwer;
        } while (difficultyLevel is null);

        return (DifficultyLevel)difficultyLevel;
    }

    private static string ReadAndValidateStringFromReadLine(string errorMessageForInvalidInput) {
        while (true) {
            var input = Console.ReadLine();
            if (String.IsNullOrWhiteSpace(input)) {
                Console.Write(errorMessageForInvalidInput);
                continue;
            }
            return input;
        }
    }

    private static void AskForNumberOfIntermediaries() {
        Console.Write("Wieviel Zwischenhändler nehmen teil?: ");
        while (true) {
            var input = Console.ReadLine();
            if (String.IsNullOrWhiteSpace(input) || !byte.TryParse(input, out _number_of_intermediaries) || _number_of_intermediaries == 0) {
                Console.Write("Geben Sie eine Zahl von 1 - 255: ");
                continue;
            }
            break;
        }

    }

    private static List<Product> ImportAvailableProducts() {
        var ymlContent = Util.ReadFileToString("produkte.yml");
        IEnumerable<string> products = Product.GetEnumerableOfIndividualProductsFromYmlContent(ymlContent);
        return Product.ConvertProductStringEnumerableToProductList(products);

    }
}
