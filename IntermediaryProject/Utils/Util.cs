using System.Text;

namespace IntermediaryProject.Utils {
    public static class Util {
        public static string ReadFileToString(string fileName) {
            var basePath = AppDomain.CurrentDomain.BaseDirectory[..AppDomain.CurrentDomain.BaseDirectory.IndexOf("bin")];
            string path = basePath + fileName;
            string readContents;

            using (StreamReader streamReader = new StreamReader(path, Encoding.UTF8)) {
                readContents = streamReader.ReadToEnd();
            }

            return readContents;
        }

        public static string GameOptionEnumDisplayNameMapping(GameOption gameOption) {

            switch (gameOption) {
                case GameOption.Shopping:
                    return "Einkaufen";
                case GameOption.EndRound:
                    return "Runde beenden";
                default:
                    throw new Exception($"No GameOption mapping for {gameOption} available!");
            }

        }
    }
}
