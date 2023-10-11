using System.Net;

namespace IntermediaryProject;

enum GameOption {
    a,
    b
}

class Game {
    List<Intermediary> intermediaries = new List<Intermediary>();
    byte number_of_intermediaries;

    public Game() {
        GetNumberOfIntermediaries();
        CreateANumberOfIntermediariesAndAddToList();
    }

    public void Play() {
        while (true) {
            foreach (var intermediary in intermediaries) {
                PrintHeader(intermediary);
                PrintGameMenu();
                PlayRound();
            }
        }

    }

    private void PlayRound() {
        bool roundFinished = false;
        do {
            var option = GetOption();
            roundFinished = ExecuteSelectedAction(option);

        } while (!roundFinished);

    }

    private bool ExecuteSelectedAction(GameOption option) {
        switch (option) {
            case GameOption.a:
                Console.WriteLine("Option a selected");
                Play();
                return false;
            case GameOption.b:
                Console.WriteLine("Option  b selected. Runde wird beendet.");
                return true;
            default:
                return true;

        }
    }

    private static GameOption GetOption() {
        GameOption? option = null;
        do {
            var input = GetStringFromReadLine("Wählen Sie eine Option aus: ");
            if (input.ToLower() == "a") {
                option = GameOption.a;
            } else if (input.ToLower() == "b") {
                option = GameOption.b;
            }

        } while (option is null);
        return (GameOption)option;
    }

    private static void PrintGameMenu() {
        // Bitte Formatierung in verbatim string ignorieren. Das ist für die richtige Darstellung des Textes.
        // Das gefiel mir besser als \n dazwischen.
        System.Console.WriteLine
(@"
{0}) Nochmal
{1}) Runde beenden", GameOption.a, GameOption.b
);
    }

    private static void PrintHeader(Intermediary intermediary) {
        System.Console.WriteLine($"{intermediary.Name} von {intermediary.CompanyName}");
    }

    private void CreateANumberOfIntermediariesAndAddToList() {
        for (int i = 1; i <= number_of_intermediaries; i++) {

            Console.Write($"Name von Zwischenhändler {i}: ");
            string intermediaryName = GetStringFromReadLine("Geben Sie einen gueltigen Namen ein: ");
            Console.Write($"Name von der Firma von {intermediaryName}: ");
            string intermediaryCompanyName = GetStringFromReadLine("Geben Sie eine gueltige Firma ein: ");

            intermediaries.Add(new Intermediary(intermediaryName, intermediaryCompanyName));
        }
    }

    private static string GetStringFromReadLine(string value) {
        while (true) {
            var input = Console.ReadLine();
            if (String.IsNullOrWhiteSpace(input)) {
                Console.Write(value);
                continue;
            }
            return input;
        }
    }

    private void GetNumberOfIntermediaries() {
        Console.Write("Wieviel Zwischenhändler nehmen teil?: ");
        while (true) {
            var input = Console.ReadLine();
            if (String.IsNullOrWhiteSpace(input) || !byte.TryParse(input, out number_of_intermediaries) || number_of_intermediaries == 0) {
                Console.Write("Geben Sie eine Zahl von 1 - 255: ");
                continue;
            }
            break;
        }

    }
}
