using System.Text;

namespace IntermediaryProject.Utils {
    public static class Util {
        internal static string ReadFileToString(string fileName) {
            var basePath =
                AppDomain.CurrentDomain.BaseDirectory[..AppDomain.CurrentDomain.BaseDirectory.IndexOf(
                    "bin",
                    StringComparison.Ordinal
                )];
            var path = basePath + fileName;

            using var streamReader = new StreamReader(path, Encoding.UTF8);
            var readContents = streamReader.ReadToEnd();

            return readContents;
        }

        internal static string DetermineDisplaytextByGameAction(GameAction gameAction) {
            return gameAction switch {
                GameAction.Shopping => "Einkaufen",
                GameAction.Selling  => "Verkaufen",
                GameAction.EndRound => "Runde beenden",
                GameAction.Storage  => "Lager vergrößern",
                _                   => throw new Exception($"No GameAction mapping for {gameAction} available!"),
            };
        }
    }
}
