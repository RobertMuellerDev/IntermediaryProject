using IntermediaryProject.Exceptions;
using IntermediaryProject.Products;
using IntermediaryProject.Transactions;
using IntermediaryProject.Utils;
using Xunit;

namespace IntermediaryProject;

public class BusinessLogicTest {
    private Intermediary _intermediary;
    private Product _product;
    private int _startingCapital;
    private int _availableAmountOfProduct;
    private int _productPrice;
    private int quantityToBuy;

    public BusinessLogicTest() {
        _startingCapital = 10_000;
        _availableAmountOfProduct = 10;
        _productPrice = 10;
        quantityToBuy = 10;

        _intermediary = new Intermediary("Testhaendler", "TestFirma", _startingCapital);

        _product = new Product(
            "TestProdukt",
            5,
            _productPrice,
            1,
            100
        ) { Availability = _availableAmountOfProduct };
        _intermediary.Discounts.Add(_product.Id, 0);
    }

    [Fact]
    public void SuccessfullyBuyingAProductCheckIntermediary() {
        BusinessLogic.BuyProduct(_intermediary, _product, quantityToBuy);
        var expectedCapital = _startingCapital - quantityToBuy * _productPrice;
        Assert.Equal(expectedCapital, _intermediary.Capital);
        Assert.Contains(_product.Id, _intermediary.Inventory);
        Assert.Equal(quantityToBuy, _intermediary.Inventory[_product.Id]);
    }

    [Fact]
    public void FailBuyingAProductBecauseCapitalIsToLow() {
        _intermediary.Capital = quantityToBuy * _productPrice - 1;
        var exception = Assert.Throws<IntermediaryBuyException>(
            () => { BusinessLogic.BuyProduct(_intermediary, _product, quantityToBuy); }
        );
        Assert.Contains("Es ist nicht genug Kapital vorhanden", exception.Message);
        Assert.Equal(_availableAmountOfProduct, _product.Availability);
        Assert.Empty(_intermediary.Inventory);
    }

    [Fact]
    public void FailBuyingAProductBecauseProductNotAvailable() {
        quantityToBuy = 11;
        var exception = Assert.Throws<ProductNotAvailableException>(
            () => { BusinessLogic.BuyProduct(_intermediary, _product, quantityToBuy); }
        );
        Assert.Equal("Es kann nicht mehr von einem Produkt gekauft werden, als verfügbar ist.", exception.Message);
        Assert.Equal(_availableAmountOfProduct, _product.Availability);
        Assert.Empty(_intermediary.Inventory);
        Assert.Equal(_startingCapital, _intermediary.Capital);
    }

    [Fact]
    public void SuccessfullyBuyingAProductCheckProductAvailability() {
        BusinessLogic.BuyProduct(_intermediary, _product, quantityToBuy);
        var expectedAvailability = _availableAmountOfProduct - quantityToBuy;
        Assert.Equal(expectedAvailability, _product.Availability);
    }

    [Fact]
    public void SuccessfullyBuyingAProductCheckShoppingCosts() {
        BusinessLogic.BuyProduct(_intermediary, _product, quantityToBuy);
        var shoppingCosts = _intermediary.TransactionsOfTheDay
                                         .Where(transaction => transaction.Type == TransactionType.Shopping)
                                         .Aggregate<Transaction, decimal>(
                                             0m,
                                             (sum, transaction) => sum + transaction.Amount
                                         );
        var expectedShoppingCosts = _productPrice * quantityToBuy;
        Assert.Equal(expectedShoppingCosts, shoppingCosts);
    }
}
