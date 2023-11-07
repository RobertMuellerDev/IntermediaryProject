using IntermediaryProject.Products;

namespace IntermediaryProject;

public class Intermediary {

    private readonly string _name;
    private readonly string _companyName;
    private int _capital;
    private readonly Dictionary<int, int> _inventory = new Dictionary<int, int>();

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

    public Intermediary(string name, string companyName, int startingCapital) {
        _name = name;
        _companyName = companyName;
        _capital = startingCapital;
    }

    internal void BuyProducts(Product product, int quantity) {
        if (_capital < (product.Price * quantity)) {
            throw new InvalidOperationException($"Es ist nicht genug Kapital vorhanden, um {quantity:n0}-mal {product.Name} zu kaufen!");
        }
        _capital -= product.Price * quantity;
        if (_inventory.ContainsKey(product.Id)) {
            _inventory[product.Id] += quantity;
        } else {
            _inventory.Add(product.Id, quantity);
        }
    }

    internal void SellProducts(Product product, int quantity) {
        if (!_inventory.ContainsKey(product.Id)) {
            throw new ArgumentOutOfRangeException(nameof(product.Id), "Dieses Produkt hat der Haendler nicht auf Lager!");
        } else if (_inventory[product.Id] < quantity) {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Die angefragte Menge übersteigt den vorhandenen Lagerbestand!");
        }
        _capital += product.CalculateSellingPrice() * quantity;
        if (_inventory[product.Id] == quantity) {
            _inventory.Remove(product.Id);
        } else {
            _inventory[product.Id] -= quantity;
        }
    }

}
