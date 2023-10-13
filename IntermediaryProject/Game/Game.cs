using System.Text;
using ExtensionMethods;

namespace IntermediaryProject;

enum GameOption {
    a,
    b
}

class Game {
    private readonly List<Intermediary> _intermediaries = new List<Intermediary>();

    /*
    Für beliebig viele habe ich jetzt mal nicht mehr als 255 angenommen,
    aber das kann natürlich mit short, int oder long
    und einer kleinen Anpassung der Abfrage beliebig erhöht werden
    */
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
            PrintHeader(intermediary, _day);
            PrintGameMenu();
            var selectedOption = GetOption();
            roundFinished = ExecuteSelectedAction(selectedOption);

        } while (!roundFinished);

    }

    private static bool ExecuteSelectedAction(GameOption selectedOption) {
        switch (selectedOption) {
            case GameOption.a:
                Console.WriteLine("Option a selected");
                return false;
            case GameOption.b:
                Console.WriteLine();
                return true;
            default:
                return false;

        }
    }

    private static GameOption GetOption() {
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

    private static void PrintGameMenu() {
        Console.WriteLine(BuildGameMenuString());
    }

    private static string BuildGameMenuString() {
        StringBuilder stringBuilder = new();
        stringBuilder.AppendLine($"{GameOption.a}) Nochmal");
        stringBuilder.Append($"{GameOption.b}) Runde beenden");
        return stringBuilder.ToString();
    }

    private static void PrintHeader(Intermediary intermediary, int day) {
        Console.WriteLine($"{intermediary.Name} von {intermediary.CompanyName} | Tag {day}");
    }

    private void CreateIntermediariesAndAddToList() {
        for (int i = 1; i <= _number_of_intermediaries; i++) {

            Console.Write($"Name von Zwischenhändler {i}: ");
            string intermediaryName = ReadAndValidateStringFromReadLine("Geben Sie einen gueltigen Namen ein: ");
            Console.Write($"Name von der Firma von {intermediaryName}: ");
            string intermediaryCompanyName = ReadAndValidateStringFromReadLine("Geben Sie eine gueltige Firma ein: ");

            _intermediaries.Add(new Intermediary(intermediaryName, intermediaryCompanyName));
        }
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
