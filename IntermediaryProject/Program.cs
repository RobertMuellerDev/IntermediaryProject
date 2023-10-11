using IntermediaryProject;
using Microsoft.VisualBasic;

class Program {
    static void Main(string[] args) {
        var intermediaries = new List<Intermediary>();
        byte number_of_intermediaries = getNumberOfIntermediaries();

        createANumberOfIntermediariesAndAddToList(intermediaries, number_of_intermediaries);

        System.Console.WriteLine();
        foreach (var Intermediary in intermediaries) {
            System.Console.WriteLine($"Name: {Intermediary.Name} CompanyName: {Intermediary.CompanyName}");
        }
    }

    private static void createANumberOfIntermediariesAndAddToList(List<Intermediary> intermediaries, byte number_of_intermediaries) {
        for (int i = 1; i <= number_of_intermediaries; i++) {

            Console.Write($"Name von Zwischenhändler {i}: ");
            string intermediaryName = getStringFromReadLine("Geben Sie einen gueltigen Namen ein: ");
            Console.Write($"Name von der Firma von {intermediaryName}: ");
            string intermediaryCompanyName = getStringFromReadLine("Geben Sie eine gueltige Firma ein: ");

            intermediaries.Add(new Intermediary(intermediaryName, intermediaryCompanyName));
        }
    }

    private static string getStringFromReadLine(string value) {
        while (true) {
            var input = Console.ReadLine();
            if (String.IsNullOrWhiteSpace(input)) {
                Console.Write(value);
                continue;
            }
            return input;
        }
    }

    private static byte getNumberOfIntermediaries() {
        byte number_of_intermediaries;
        Console.Write("Wieviel Zwischenhändler nehmen teil?: ");
        while (true) {
            var input = Console.ReadLine();
            if (String.IsNullOrWhiteSpace(input) || !byte.TryParse(input, out number_of_intermediaries) || number_of_intermediaries == 0) {
                Console.Write("Geben Sie eine Zahl von 1 - 255: ");
                continue;
            }
            break;
        }

        return number_of_intermediaries;
    }
}
