
using System.Text.RegularExpressions;

namespace IntermediaryProject.Products {
    public class Product {
        private static int s_idNumberSeed = 1;

        private readonly int _id;
        private readonly string _name;

        private int _durability;

        public int Id {
            get { return _id; }
        }
        public string Name {
            get { return _name; }
        }
        public int Durability {
            get { return _durability; }
        }

        public Product(string name, int durability) {
            _id = s_idNumberSeed++;
            _name = name;
            _durability = durability;
        }

        public override string ToString() {
            return $"{_id}) {_name} ({_durability} Tag{(_durability > 1 ? "e" : "")})";
        }

        public static List<Product> ConvertProductStringEnumerableToProductList(IEnumerable<string> products) {
            var availableProducts = new List<Product>();
            foreach (var (product, regex) in from product in products
                                             let productPattern = @"(name:)\s*(?<nameValue>\w*)\s*(durability:)\s*(?<durabilityValue>\d*)"
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

        private static Product ConvertMatchToProduct(string trimmedProduct, Match match) {
            var name = match.Groups["nameValue"].Value;
            var durability = match.Groups["durabilityValue"].Value;

            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(durability) && int.TryParse(durability, out int parsedDurability)) {
                return (new Product(name, parsedDurability));
            } else {
                Console.WriteLine($"Es trat ein Problem beim Parsen der durability in den Zeilen:\n\"{trimmedProduct}\"\naus der produkte.yml Datei auf!");
                Console.WriteLine("Programm wird beendet!");
                throw new Exception("ParseError");
            }
        }

        public static IEnumerable<string> GetEnumerableOfIndividualProductsFromYmlContent(string ymlContent) {
            return ymlContent.Split('-').Where(arrayElement => !string.IsNullOrEmpty(arrayElement));
        }
    }
}
