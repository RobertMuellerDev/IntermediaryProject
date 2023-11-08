
using System.Text.RegularExpressions;
using IntermediaryProject.Exceptions;

namespace IntermediaryProject.Products {
    public class Product {
        private static byte s_idNumberSeed = 1;
        private static readonly Random s_rnd = new Random();
        private readonly byte _id;
        private readonly string _name;
        private readonly int _basePrice;
        private readonly int _maxAvailability;
        private int _durability;
        private int _price;
        private int _minProductionRate;
        private int _maxProductionRate;
        private int _availability;

        private static readonly string _productPattern = string.Join("", new string[] {
                                  @"(name:)\s*(?<nameValue>\w*)\s*",
                                  @"(durability:)\s*(?<durabilityValue>\d*)\s*",
                                  @"(baseprice:)\s*(?<basepriceValue>\d*)\s*",
                                  @"(minProductionRate:)\s*(?<minProductionRateValue>[-]?\d*)\s*",
                                  @"(maxProductionRate:)\s*(?<maxProductionRateValue>[-]?\d*)"});

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
            private set {
                if (value > 3 * _basePrice) {
                    _price = 3 * _basePrice;
                } else if (value < (_basePrice * 0.25)) {
                    _price = (int)Math.Ceiling(_basePrice * 0.25);
                } else {
                    _price = value;
                }
            }
        }

        public int SellingPrice {
            get { return (int)Math.Ceiling(Price * 0.8); }
        }

        public int MaxProductionRate {
            set {
                if (value < 1)
                    throw new ArgumentOutOfRangeException(nameof(value),
                    "Die maximale Produktionsrate muss größer 0 sein.");
                _maxProductionRate = value;
            }
        }

        public int Availability {
            get { return _availability; }
            set {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value),
                    "Die verfügbare Menge kann nicht kleiner 0 sein.");
                else if (value > _maxAvailability) {
                    _availability = _maxAvailability;
                } else {
                    _availability = value;
                }
            }
        }

        public Product(string name, int durability, int price, int minProductionRate, int maxProductionRate) {
            _id = s_idNumberSeed++;
            _name = name;
            _durability = durability;
            _basePrice = price;
            _price = price;
            _minProductionRate = minProductionRate;
            MaxProductionRate = maxProductionRate;
            _availability = 0;
            _maxAvailability = _maxProductionRate * _durability;
        }

        public override string ToString() {
            return $"{_id}) {_name} ({_availability}) ({_durability} Tag{(_durability > 1 ? "e" : "")}) ${_price}/Stück";
        }

        public string ToSellingString(int quantity) {
            return $"{_id}) {_name} ({quantity}) ${SellingPrice}/Stück";
        }

        public void ProduceProduct() {
            int producedQuantity = s_rnd.Next(_minProductionRate, _maxProductionRate + 1);
            if (Availability + producedQuantity < 0) {
                Availability = 0;
            } else {
                Availability += producedQuantity;
            }
        }

        public void CalculatePurchasePrice() {
            int changeInPercent;
            if (Availability < (_maxAvailability * 0.25)) {
                changeInPercent = s_rnd.Next(-10, 31);
            } else if (Availability > (_maxAvailability * 0.25) && Availability < (_maxAvailability * 0.8)) {
                changeInPercent = s_rnd.Next(-5, 6);
            } else {
                changeInPercent = s_rnd.Next(-10, 7);
            }
            var priceChange = changeInPercent / 100.0 * _basePrice;
            Price += priceChange < 0 ? (int)Math.Floor(priceChange) : (int)Math.Ceiling(priceChange);
        }

        public void ReduceAvailabilityWhenBuying(int quantity) {
            if (Availability - quantity < 0) {
                throw new ProductNotAvailableException("Es kann nicht mehr von einem Produkt gekauft werden, als verfügbar ist.");
            }
            Availability -= quantity;
        }

        public void ReverseBuyingProcess(int quantity) {
            Availability += quantity;
        }

        public static List<Product> ConvertProductStringEnumerableToProductList(IEnumerable<string> products) {
            var availableProducts = new List<Product>();
            foreach (var match in from product in products
                                  let regex = new Regex(_productPattern, RegexOptions.IgnoreCase)
                                  select regex.Match(product.Trim())) {
                var currentMatch = match;
                while (currentMatch.Success) {
                    try {
                        availableProducts.Add(ConvertMatchToProduct(currentMatch));
                    } catch (ArgumentOutOfRangeException e) {
                        Console.WriteLine(e.Message);
                        throw;
                    }
                    currentMatch = currentMatch.NextMatch();
                }
            }
            return availableProducts;
        }

        private static Product ConvertMatchToProduct(Match match) {
            ExtractMatchedValues(match, out string name, out string durability, out string price, out string minProductionRateValue, out string maxProductionRateValue);
            if (
                AreValuesValid(name, durability, price, minProductionRateValue, maxProductionRateValue) &&
                int.TryParse(durability, out int parsedDurability) &&
                int.TryParse(price, out int parsedPrice) &&
                int.TryParse(minProductionRateValue, out int minProductionRate) &&
                int.TryParse(maxProductionRateValue, out int maxProductionRate)
            ) {
                return new Product(name, parsedDurability, parsedPrice, minProductionRate, maxProductionRate);
            } else {
                Console.WriteLine($"Es trat ein Problem beim Parsen in den Zeilen:\n\"{match.Value}\"\n auf!");
                Console.WriteLine("Programm wird beendet!");
                throw new Exception("ParseError");
            }
        }

        private static void ExtractMatchedValues(Match match, out string name, out string durability, out string price, out string minProductionRateValue, out string maxProductionRateValue) {
            name = match.Groups["nameValue"].Value;
            durability = match.Groups["durabilityValue"].Value;
            price = match.Groups["basepriceValue"].Value;
            minProductionRateValue = match.Groups["minProductionRateValue"].Value;
            maxProductionRateValue = match.Groups["maxProductionRateValue"].Value;
        }

        private static bool AreValuesValid(string name, string durability, string price, string minProductionRateValue, string maxProductionRateValue) {
            return (
                !string.IsNullOrEmpty(name) &&
                !string.IsNullOrEmpty(durability) &&
                !string.IsNullOrEmpty(price) &&
                !string.IsNullOrEmpty(minProductionRateValue) &&
                !string.IsNullOrEmpty(maxProductionRateValue)
            );
        }

        public static IEnumerable<string> GetEnumerableOfIndividualProductsFromYmlContent(string ymlContent) {
            return ymlContent.Split("- ").Where(arrayElement => !string.IsNullOrWhiteSpace(arrayElement));
        }
    }
}
