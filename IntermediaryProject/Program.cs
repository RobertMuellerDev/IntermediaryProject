using IntermediaryProject;

class Program {
    static void Main(string[] args) {
        try {
            var ui = new Ui();
            var game = new Game(ui, new BusinessLogic(ui));
            game.Play();
        } catch (Exception e) {
            Console.WriteLine("Error: " + e.Message);
            Console.WriteLine("Programm wird beendet!");

            Environment.Exit(0);
        }
    }
}
