using System.Text;

namespace IntermediaryProject.Utils {
    public static class Util {
        internal static string ReadFileToString(string fileName) {
            var basePath = AppDomain.CurrentDomain.BaseDirectory[..AppDomain.CurrentDomain.BaseDirectory.IndexOf("bin")];
            string path = basePath + fileName;
            string readContents;

            using (StreamReader streamReader = new StreamReader(path, Encoding.UTF8)) {
                readContents = streamReader.ReadToEnd();
            }

            return readContents;
        }

        internal static string GameOptionEnumDisplayNameMapping(GameOption gameOption) {

            switch (gameOption) {
                case GameOption.Shopping:
                    return "Einkaufen";
                case GameOption.Selling:
                    return "Verkaufen";
                case GameOption.EndRound:
                    return "Runde beenden";
                default:
                    throw new Exception($"No GameOption mapping for {gameOption} available!");
            }

        }
    }
}
