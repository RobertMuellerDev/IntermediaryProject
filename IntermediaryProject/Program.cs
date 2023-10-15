using IntermediaryProject;
using IntermediaryProject.Utils;

class Program {
    static void Main(string[] args) {
        try {
            Game.Play();
        } catch (Exception e) {
            if (!(e.Message == "ParseError")) {
                Console.WriteLine("Error: " + e.Message);
            }
            Environment.Exit(0);
        }

    }
}
