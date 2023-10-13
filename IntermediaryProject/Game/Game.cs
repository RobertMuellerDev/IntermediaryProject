using ExtensionMethods;

namespace IntermediaryProject;

class Game {
    private readonly List<Intermediary> _intermediaries = new List<Intermediary>();

    private byte _number_of_intermediaries;
    private int _day;

    public Game() {
        AskForNumberOfIntermediaries();
        CreateIntermediariesAndAddToList();
        _day = 1;
    }

    public void Play() {
        while (true) {
            foreach (var intermediary in _intermediaries) {
                PlayRound(intermediary);
            }
            _day++;
            ChangeIntermediariesOrder();
        }

    }
    private void ChangeIntermediariesOrder() {
        var firstElement = _intermediaries.Pop(0);
        _intermediaries.Add(firstElement);
    }

    private void PlayRound(Intermediary intermediary) {
        bool roundFinished = false;
        do {
            UI.PrintHeader(intermediary, _day);
            UI.PrintGameMenu();
            var selectedOption = AskUserForAction();
            roundFinished = ExecuteSelectedAction(selectedOption);

        } while (!roundFinished);

    }

    private static bool ExecuteSelectedAction(GameOption selectedOption) {
        switch (selectedOption) {
            case GameOption.a:
                Console.WriteLine();
                return false;
            case GameOption.b:
                Console.WriteLine();
                return true;
            default:
                return false;

        }
    }

    private static GameOption AskUserForAction() {
        GameOption? selectedOption = null;
        do {
            var input = ReadAndValidateStringFromReadLine("Wählen Sie eine Option aus: ");
            if (input.ToLower() == "a") {
                selectedOption = GameOption.a;
            } else if (input.ToLower() == "b") {
                selectedOption = GameOption.b;
            }

        } while (selectedOption is null);
        return (GameOption)selectedOption;
    }

    private void CreateIntermediariesAndAddToList() {
        for (int i = 1; i <= _number_of_intermediaries; i++) {

            Console.Write($"Name von Zwischenhändler {i}: ");
            string intermediaryName = ReadAndValidateStringFromReadLine("Geben Sie einen gueltigen Namen ein: ");
            Console.Write($"Name von der Firma von {intermediaryName}: ");
            string intermediaryCompanyName = ReadAndValidateStringFromReadLine("Geben Sie eine gueltige Firma ein: ");
            DifficultyLevel difficultyLevel = AskUserForDifficultyLevel();
            _intermediaries.Add(new Intermediary(intermediaryName, intermediaryCompanyName, (int)difficultyLevel));
        }
    }

    private static DifficultyLevel AskUserForDifficultyLevel() {
        Console.WriteLine($"Wählen Sie einen Schwierigkeitsgrad aus: ");
        UI.PrintDifficultyLevelChoice();
        return GetValidatedDifficultyLevelFromInput();
    }

    private static DifficultyLevel GetValidatedDifficultyLevelFromInput() {
        DifficultyLevel? difficultyLevel = null;
        do {
            string difficultyLevelInput = ReadAndValidateStringFromReadLine("Geben Sie einen gueltigen Schwierigkeitsgrad ein: ");
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

    private void AskForNumberOfIntermediaries() {
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
}
