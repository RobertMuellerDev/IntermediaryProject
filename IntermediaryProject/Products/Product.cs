namespace IntermediaryProject.Products {
    public class Product {
        private static byte s_idNumberSeed = 1;
        private readonly int _durability;
        private int _price;
        private int _maxProductionRate;
        private int _availability;

        public byte Id { get; }
        public string Name { get; }
        public int BasePrice { get; }

        public int Price {
            get { return _price; }
            internal set {
                if (value > 3 * BasePrice) {
                    _price = 3 * BasePrice;
                } else if (value < (BasePrice * 0.25)) {
                    _price = (int)Math.Ceiling(BasePrice * 0.25);
                } else {
                    _price = value;
                }
            }
        }

        public int SellingPrice {
            get { return (int)Math.Ceiling(Price * 0.8); }
        }

        public int MinProductionRate { get; }

        public int MaxProductionRate {
            get { return _maxProductionRate; }
            set {
                if (value < 1)
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        "Die maximale Produktionsrate muss größer 0 sein."
                    );
                _maxProductionRate = value;
            }
        }

        public int MaxAvailability { get; }

        public int Availability {
            get { return _availability; }
            set {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        "Die verfügbare Menge kann nicht kleiner 0 sein."
                    );
                _availability = value > MaxAvailability ? MaxAvailability : value;
            }
        }

        public Product(
            string name,
            int durability,
            int price,
            int minProductionRate,
            int maxProductionRate
        ) {
            Id = s_idNumberSeed++;
            Name = name;
            _durability = durability;
            BasePrice = price;
            _price = price;
            MinProductionRate = minProductionRate;
            MaxProductionRate = maxProductionRate;
            _availability = 0;
            MaxAvailability = _maxProductionRate * _durability;
        }

        public override string ToString() {
            return $"{Id}) {Name} ({_availability}) ({_durability} Tag{(_durability > 1 ? "e" : "")}) ${_price}/Stück";
        }

        public string CreateSalesString(int quantity) {
            return $"{Id}) {Name} ({quantity}) ${SellingPrice}/Stück";
        }
    }
}
