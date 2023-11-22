using System.Text.RegularExpressions;
using IntermediaryProject.Exceptions;

namespace IntermediaryProject.Products {
    public class Product {
        private static byte s_idNumberSeed;
        private static readonly Random s_rnd;
        private readonly byte _id;
        private readonly string _name;
        private readonly int _basePrice;
        private readonly int _maxAvailability;
        private readonly int _durability;
        private readonly int _minProductionRate;
        private int _price;
        private int _maxProductionRate;
        private int _availability;

        public byte Id {
            get { return _id; }
        }

        public string Name {
            get { return _name; }
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
                    throw new ArgumentOutOfRangeException(
                                                          nameof(value),
                                                          "Die maximale Produktionsrate muss größer 0 sein."
                                                         );
                _maxProductionRate = value;
            }
        }

        private int Availability {
            get { return _availability; }
            set {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(
                                                          nameof(value),
                                                          "Die verfügbare Menge kann nicht kleiner 0 sein."
                                                         );
                _availability = value > _maxAvailability ? _maxAvailability : value;
            }
        }

        public Product(
            string name,
            int durability,
            int price,
            int minProductionRate,
            int maxProductionRate
        ) {
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
        static Product() {
            s_idNumberSeed = 1;
            s_rnd = new Random();
        }

        public override string ToString() {
            return
                $"{_id}) {_name} ({_availability}) ({_durability} Tag{(_durability > 1 ? "e" : "")}) ${_price}/Stück";
        }

        public string CreateSalesString(int quantity) {
            return $"{_id}) {_name} ({quantity}) ${SellingPrice}/Stück";
        }

        public void ProduceProduct() {
            var producedQuantity = s_rnd.Next(_minProductionRate, _maxProductionRate + 1);
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
            } else if (Availability > (_maxAvailability * 0.25) &&
                       Availability < (_maxAvailability * 0.8)) {
                changeInPercent = s_rnd.Next(-5, 6);
            } else {
                changeInPercent = s_rnd.Next(-10, 7);
            }
            var priceChange = changeInPercent / 100.0 * _basePrice;
            Price += priceChange < 0 ? (int)Math.Floor(priceChange) : (int)Math.Ceiling(priceChange);
        }

        public void ReduceAvailabilityWhenBuying(int quantity) {
            if (Availability - quantity < 0) {
                throw new ProductNotAvailableException(
                                                       "Es kann nicht mehr von einem Produkt gekauft werden, als verfügbar ist."
                                                      );
            }
            Availability -= quantity;
        }

        public void ReverseBuyingProcess(int quantity) {
            Availability += quantity;
        }
    }
}
