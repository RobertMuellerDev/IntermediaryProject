using System.Text.RegularExpressions;
using IntermediaryProject.Exceptions;
using IntermediaryProject.Utils;

namespace IntermediaryProject.Products {
    public static class YamlParser {
        private static readonly string s_productPattern = string.Join(
            "",
            @"(name:)\s*(?<nameValue>\w*)\s*",
            @"(durability:)\s*(?<durabilityValue>\d*)\s*",
            @"(baseprice:)\s*(?<basepriceValue>\d*)\s*",
            @"(minProductionRate:)\s*(?<minProductionRateValue>[-]?\d*)\s*",
            @"(maxProductionRate:)\s*(?<maxProductionRateValue>[-]?\d*)"
        );

        public static List<Product> ReadAvailableProductsFromFile(string filename) {
            var ymlContent = Util.ReadFileToString(filename);
            return ParseYMLContentToProducts(ymlContent);
        }

        private static List<Product> ParseYMLContentToProducts(string ymlContent) {
            var products = YamlParser.GetEnumerableOfIndividualProductsFromYmlContent(ymlContent);
            return YamlParser.ConvertProductStringEnumerableToProductList(products);
        }

        private static List<Product> ConvertProductStringEnumerableToProductList(IEnumerable<string> products) {
            var availableProducts = new List<Product>();
            foreach (var match in from product in products
                     let regex = new Regex(s_productPattern, RegexOptions.IgnoreCase)
                     select regex.Match(product.Trim())) {
                var currentMatch = match;
                while (currentMatch.Success) {
                    try {
                        availableProducts.Add(ConvertMatchToProduct(currentMatch));
                    } catch (ArgumentOutOfRangeException e) {
                        throw new EndGameException(
                            $"Folgender Fehler trat für das Produkt: {currentMatch.Value} auf: " + e.Message
                        );
                    }

                    currentMatch = currentMatch.NextMatch();
                }
            }

            return availableProducts;
        }

        private static Product ConvertMatchToProduct(Match match) {
            ExtractMatchedValues(
                match,
                out var name,
                out var durability,
                out var price,
                out var minProductionRateValue,
                out var maxProductionRateValue
            );
            if (AreValuesValid(
                    name,
                    durability,
                    price,
                    minProductionRateValue,
                    maxProductionRateValue
                ) &&
                CreateNewProduct(
                    durability,
                    price,
                    minProductionRateValue,
                    maxProductionRateValue,
                    name,
                    out var product
                ))
                return product;

            throw new EndGameException(
                $"ParserError: Es trat ein Problem beim Parsen in den Zeilen:\n\"{match.Value}\"\n auf!"
            );
        }

        private static bool CreateNewProduct(
            string durability,
            string price,
            string minProductionRateValue,
            string maxProductionRateValue,
            string name,
            out Product product
        ) {
            if (!int.TryParse(durability, out var parsedDurability) ||
                !int.TryParse(price, out var parsedPrice) ||
                !int.TryParse(minProductionRateValue, out var minProductionRate) ||
                !int.TryParse(maxProductionRateValue, out var maxProductionRate)) {
                product = null;
                return false;
            }

            product = new Product(
                name,
                parsedDurability,
                parsedPrice,
                minProductionRate,
                maxProductionRate
            );
            return true;
        }

        private static void ExtractMatchedValues(
            Match match,
            out string name,
            out string durability,
            out string price,
            out string minProductionRateValue,
            out string maxProductionRateValue
        ) {
            name = match.Groups["nameValue"].Value;
            durability = match.Groups["durabilityValue"].Value;
            price = match.Groups["basepriceValue"].Value;
            minProductionRateValue = match.Groups["minProductionRateValue"].Value;
            maxProductionRateValue = match.Groups["maxProductionRateValue"].Value;
        }

        private static bool AreValuesValid(
            string name,
            string durability,
            string price,
            string minProductionRateValue,
            string maxProductionRateValue
        ) {
            return (!string.IsNullOrEmpty(name) &&
                    !string.IsNullOrEmpty(durability) &&
                    !string.IsNullOrEmpty(price) &&
                    !string.IsNullOrEmpty(minProductionRateValue) &&
                    !string.IsNullOrEmpty(maxProductionRateValue));
        }

        private static IEnumerable<string> GetEnumerableOfIndividualProductsFromYmlContent(string ymlContent) {
            return ymlContent.Split("- ")
                             .Where(arrayElement => !string.IsNullOrWhiteSpace(arrayElement));
        }
    }
}
