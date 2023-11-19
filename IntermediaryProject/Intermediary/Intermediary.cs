using IntermediaryProject.Exceptions;
using IntermediaryProject.Products;

namespace IntermediaryProject {
    public class Intermediary {
        private static readonly int s_storagePricePerUnit = 50;
        private readonly string _name;
        private readonly string _companyName;
        private int _capital;
        private readonly Dictionary<int, int> _inventory = new();
        private int _storageCapacity;
        private int _storageUtilization;

        private int _availableStorageCapacity {
            get { return _storageCapacity - _storageUtilization; }
        }

        public int Capital {
            get { return _capital; }
        }

        public string Name {
            get { return _name; }
        }

        public string CompanyName {
            get { return _companyName; }
        }

        public Dictionary<int, int> Inventory {
            get { return _inventory; }
        }

        public int StorageCapacity {
            get { return _storageCapacity; }
        }

        public int StorageUtilization {
            get { return _storageUtilization; }
        }

        public Intermediary(string name, string companyName, int startingCapital) {
            _name = name;
            _companyName = companyName;
            _capital = startingCapital;
            _storageCapacity = 100;
            _storageUtilization = 0;
        }

        internal void BuyProducts(Product product, int quantity) {
            if (_capital < (product.Price * quantity)) {
                throw new IntermediaryBuyException(
                                                   $"Es ist nicht genug Kapital vorhanden, um {quantity:n0}-mal {product.Name} zu kaufen!"
                                                  );
            }
            if (quantity > _availableStorageCapacity) {
                throw new IntermediaryBuyException(
                                                   $"Es ist nicht genug Lagerkapazität vorhanden, um {quantity:n0}-mal {product.Name} zu kaufen!"
                                                  );
            }
            _capital -= product.Price * quantity;
            _storageUtilization += quantity;
            if (_inventory.ContainsKey(product.Id)) {
                _inventory[product.Id] += quantity;
            } else {
                _inventory.Add(product.Id, quantity);
            }
        }

        internal void SellProducts(Product product, int quantity) {
            if (!_inventory.ContainsKey(product.Id)) {
                throw new ArgumentOutOfRangeException(
                                                      nameof(product.Id),
                                                      "Dieses Produkt hat der Händler nicht auf Lager!"
                                                     );
            }
            if (_inventory[product.Id] < quantity) {
                throw new ArgumentOutOfRangeException(
                                                      nameof(quantity),
                                                      "Die angefragte Menge übersteigt den vorhandenen Lagerbestand!"
                                                     );
            }
            _capital += product.SellingPrice * quantity;
            _storageUtilization -= quantity;
            if (_inventory[product.Id] == quantity) {
                _inventory.Remove(product.Id);
            } else {
                _inventory[product.Id] -= quantity;
            }
        }

        internal void IncreaseStorage(int storageExpansionSize) {
            if (_capital < (s_storagePricePerUnit * storageExpansionSize)) {
                throw new ArgumentOutOfRangeException(
                                                      nameof(storageExpansionSize),
                                                      $"Es ist nicht genug Kapital vorhanden, um {storageExpansionSize:n0} Lagereinheiten zu kaufen!"
                                                     );
            }
            _capital -= s_storagePricePerUnit * storageExpansionSize;
            _storageCapacity += storageExpansionSize;
        }

        internal void PayStorageOperatingCosts() {
            int storageOperatingCosts = _storageUtilization * 5;
            storageOperatingCosts += _availableStorageCapacity * 1;

            _capital -= storageOperatingCosts;
        }
    }
}
