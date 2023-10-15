
using System.Text.RegularExpressions;

namespace IntermediaryProject.Products {
    public class Product {
        private static byte s_idNumberSeed = 1;

        private readonly byte _id;
        private readonly string _name;

        private int _durability;

        private int _price;

        public byte Id {
            get { return _id; }
        }
        public string Name {
            get { return _name; }
        }
        public int Durability {
            get { return _durability; }
        }

        public int Price {
            get { return _price; }
        }

        public Product(string name, int durability, int price) {
            _id = s_idNumberSeed++;
            _name = name;
            _durability = durability;
            _price = price;
        }

        public override string ToString() {
            return $"{_id}) {_name} ({_durability} Tag{(_durability > 1 ? "e" : "")}) ${_price}/Stück";
        }

        public static List<Product> ConvertProductStringEnumerableToProductList(IEnumerable<string> products) {
            var availableProducts = new List<Product>();
            foreach (var (product, regex) in from product in products
                                             let productPattern = @"(name:)\s*(?<nameValue>\w*)\s*(durability:)\s*(?<durabilityValue>\d*)\s*(baseprice:)\s*(?<basepriceValue>\d*)"
                                             let regex = new Regex(productPattern, RegexOptions.IgnoreCase)
                                             select (product.Trim(), regex)) {
                var match = regex.Match(product);
                while (match.Success) {
                    availableProducts.Add(ConvertMatchToProduct(product, match));

                    match = match.NextMatch();
                }
            }
            return availableProducts;
        }

        private static Product ConvertMatchToProduct(string product, Match match) {
            var name = match.Groups["nameValue"].Value;
            var durability = match.Groups["durabilityValue"].Value;
            var price = match.Groups["basepriceValue"].Value;
            if (AreValuesValid(name, durability, price, out int parsedDurability, out int parsedPrice)) {
                return new Product(name, parsedDurability, parsedPrice);
            } else {
                Console.WriteLine($"Es trat ein Problem beim Parsen in den Zeilen:\n\"{product}\"\n auf!");
                Console.WriteLine("Programm wird beendet!");
                throw new Exception("ParseError");
            }
        }

        private static bool AreValuesValid(string name, string durability, string price, out int parsedDurability, out int parsedPrice) {
            parsedDurability = 0;
            parsedPrice = 0;
            return !string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(durability) && !string.IsNullOrEmpty(price) && int.TryParse(durability, out parsedDurability) && int.TryParse(price, out parsedPrice);
        }

        public static IEnumerable<string> GetEnumerableOfIndividualProductsFromYmlContent(string ymlContent) {
            return ymlContent.Split('-').Where(arrayElement => !string.IsNullOrEmpty(arrayElement));
        }
    }
}
