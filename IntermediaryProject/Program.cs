using IntermediaryProject;
using IntermediaryProject.Utils;

class Program {
    static void Main(string[] args) {
        try {
            var game = new Game();
            game.Play();
        } catch (Exception e) {
            if (!(e.Message == "ParseError")) {
                Console.WriteLine("Error: " + e.Message);
            }
            Environment.Exit(0);
        }

    }
}
