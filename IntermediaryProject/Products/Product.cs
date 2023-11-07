
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
        public string ToSellingString(int quantity) {
            return $"{_id}) {_name} ({quantity}) ${CalculateSellingPrice()}/Stück";
        }

        public static List<Product> ConvertProductStringEnumerableToProductList(IEnumerable<string> products) {
            var availableProducts = new List<Product>();
            foreach (var match in from product in products
                                  let productPattern = @"(name:)\s*(?<nameValue>\w*)\s*(durability:)\s*(?<durabilityValue>\d*)\s*(baseprice:)\s*(?<basepriceValue>\d*)"
                                  let regex = new Regex(productPattern, RegexOptions.IgnoreCase)
                                  select regex.Match(product.Trim())) {
                var currentMatch = match;
                while (currentMatch.Success) {
                    availableProducts.Add(ConvertMatchToProduct(currentMatch));

                    currentMatch = currentMatch.NextMatch();
                }
            }
            return availableProducts;
        }

        private static Product ConvertMatchToProduct(Match match) {
            var name = match.Groups["nameValue"].Value;
            var durability = match.Groups["durabilityValue"].Value;
            var price = match.Groups["basepriceValue"].Value;
            if (AreValuesValid(name, durability, price) && int.TryParse(durability, out int parsedDurability) && int.TryParse(price, out int parsedPrice)) {
                return new Product(name, parsedDurability, parsedPrice);
            } else {
                Console.WriteLine($"Es trat ein Problem beim Parsen in den Zeilen:\n\"{match.Value}\"\n auf!");
                Console.WriteLine("Programm wird beendet!");
                throw new Exception("ParseError");
            }
        }

        private static bool AreValuesValid(string name, string durability, string price) {

            return !string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(durability) && !string.IsNullOrEmpty(price);
        }

        public static IEnumerable<string> GetEnumerableOfIndividualProductsFromYmlContent(string ymlContent) {
            return ymlContent.Split('-').Where(arrayElement => !string.IsNullOrEmpty(arrayElement));
        }

        public int CalculateSellingPrice() {
            return (int)(Math.Ceiling(_price * 0.8));
        }
    }
}
