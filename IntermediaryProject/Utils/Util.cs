using System.Text;

namespace IntermediaryProject.Utils {
    public static class Util {
        internal static string ReadFileToString(string fileName) {
            var basePath =
                AppDomain.CurrentDomain.BaseDirectory[..AppDomain.CurrentDomain.BaseDirectory.IndexOf("bin", StringComparison.Ordinal)];
            var path = basePath + fileName;

            using var streamReader = new StreamReader(path, Encoding.UTF8);
            var readContents = streamReader.ReadToEnd();

            return readContents;
        }

        internal static string GameOptionEnumDisplayNameMapping(GameOption gameOption) {
            return gameOption switch {
                GameOption.Shopping => "Einkaufen",
                GameOption.Selling  => "Verkaufen",
                GameOption.EndRound => "Runde beenden",
                GameOption.Storage  => "Lager vergrößern",
                _                   => throw new Exception($"No GameOption mapping for {gameOption} available!"),
            };
        }
    }
}
