using IntermediaryProject;

class Program {
    private static void Main(string[] args) {
        try {
            var ui = new Ui();
            var gameLogic = new GameLogic(ui);
            var game = new Game(ui, gameLogic);
            game.Play();
        } catch (Exception e) {
            Console.WriteLine("Error: " + e.Message);
            Console.WriteLine("Programm wird beendet!");

            Environment.Exit(0);
        }
    }
}
