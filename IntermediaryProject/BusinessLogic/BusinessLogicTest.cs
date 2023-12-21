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

    [Fact]
    public void SuccessfullyTakeOutLoan() {
        _intermediary.Capital = 10_000;
        BusinessLogic.TakeOutLoan(_intermediary, (10_000, 10), 1);
        var expectedCapitalAfterTakingLoan = 20_000;
        Assert.Equal(expectedCapitalAfterTakingLoan, _intermediary.Capital);
    }

    [Fact]
    public void SuccessfullyPayBackLoan() {
        _intermediary.Capital = 11_000;
        BusinessLogic.TakeOutLoan(_intermediary, (10_000, 10), 1);
        var expectedCapitalAfterTakingLoan = 21_000;
        Assert.Equal(expectedCapitalAfterTakingLoan, _intermediary.Capital);

        BusinessLogic.PayBackLoan(_intermediary, 8);
        var expectedCapitalAfterPayingBackLoan = 10_000;
        Assert.Equal(expectedCapitalAfterPayingBackLoan, _intermediary.Capital);
    }

    [Fact]
    public void FailTakingOutLoanSecondLoan() {
        _intermediary.Capital = 10_000;
        BusinessLogic.TakeOutLoan(_intermediary, (10_000, 10), 1);
        var expectedCapitalAfterTakingLoan = 20_000;
        Assert.Equal(expectedCapitalAfterTakingLoan, _intermediary.Capital);

        var exception = Assert.Throws<IntermediaryLoanException>(
            () => { BusinessLogic.TakeOutLoan(_intermediary, (5_000, 5), 3); }
        );
        Assert.Equal(
            "Der Händler hat bereits einen Kredit aufgenommen.\nEs können keine weiteren Kredite aufgenommen werden.",
            exception.Message
        );

        var expectedCapitalAfterTryingToTakeSecondLoan = 20_000;
        Assert.Equal(expectedCapitalAfterTryingToTakeSecondLoan, _intermediary.Capital);
    }

    [Fact]
    public void SuccessfullyPayBackLoanCheckLoanCost() {
        _intermediary.Capital = 11_000;
        BusinessLogic.TakeOutLoan(_intermediary, (10_000, 10), 1);
        var expectedCapitalAfterTakingLoan = 21_000;
        Assert.Equal(expectedCapitalAfterTakingLoan, _intermediary.Capital);

        BusinessLogic.PayBackLoan(_intermediary, 8);
        var expectedCapitalAfterPayingBackLoan = 10_000;
        Assert.Equal(expectedCapitalAfterPayingBackLoan, _intermediary.Capital);

        var loanCosts = _intermediary.TransactionsOfTheDay
                                     .Where(transaction => transaction.Type == TransactionType.PayLoan)
                                     .Aggregate<Transaction, decimal>(
                                         0m,
                                         (sum, transaction) => sum + transaction.Amount
                                     );
        var expectedLoanCosts = 11_000;
        Assert.Equal(expectedLoanCosts, loanCosts);
    }

    [Fact]
    public void SuccessfullySellingAProductCheckIntermediary() {
        BusinessLogic.BuyProduct(_intermediary, _product, 10);
        var expectedCapital = 9_900;
        Assert.Equal(expectedCapital, _intermediary.Capital);
        Assert.Contains(_product.Id, _intermediary.Inventory);
        Assert.Equal(quantityToBuy, _intermediary.Inventory[_product.Id]);
        Assert.Equal(10, _intermediary.StorageUtilization);

        BusinessLogic.SellSelectedQuantityOfProduct(_intermediary, _product, 10);
        var expectedCapitalAfterSelling = 9_900 + (int)Math.Ceiling(10 * 0.8) * 10;
        Assert.Equal(expectedCapitalAfterSelling, _intermediary.Capital);
        Assert.DoesNotContain(_product.Id, _intermediary.Inventory);
        Assert.Equal(0, _intermediary.StorageUtilization);
    }

    [Fact]
    public void SuccessfullySellingAProductCheckSellingRevenue() {
        BusinessLogic.BuyProduct(_intermediary, _product, 10);
        BusinessLogic.SellSelectedQuantityOfProduct(_intermediary, _product, 10);
        var sellingRevenue = _intermediary.TransactionsOfTheDay
                                          .Where(transaction => transaction.Type == TransactionType.Selling)
                                          .Aggregate<Transaction, decimal>(
                                              0m,
                                              (sum, transaction) => sum + transaction.Amount
                                          );
        var expectedSellingRevenue = (int)Math.Ceiling(10 * 0.8) * 10;
        Assert.Equal(expectedSellingRevenue, sellingRevenue);
    }

    [Fact]
    public void FailSellingAProductBecauseProductNotAvailable() {
        var exception = Assert.Throws<IntermediarySellException>(
            () => { BusinessLogic.SellSelectedQuantityOfProduct(_intermediary, _product, 1); }
        );
        Assert.Equal("Dieses Produkt hat der Händler nicht auf Lager!", exception.Message);
        Assert.Empty(_intermediary.Inventory);
        Assert.Equal(10_000, _intermediary.Capital);
    }

    [Fact]
    public void SuccessfullyExpandStorage() {
        BusinessLogic.ExpandStorage(_intermediary, 100);
        var expectedCapital = 5_000;
        var expectedStorageCapacity = 200;
        Assert.Equal(expectedCapital, _intermediary.Capital);
        Assert.Equal(expectedStorageCapacity, _intermediary.StorageCapacity);
    }

    [Fact]
    public void FailToExpandStorage() {
        var exception = Assert.Throws<IntermediaryExpandStorageException>(
            () => { BusinessLogic.ExpandStorage(_intermediary, 1000); }
        );

        var expectedCapital = 10_000;
        var expectedStorageCapacity = 100;
        Assert.Equal("Es ist nicht genug Kapital vorhanden, um 1.000 Lagereinheiten zu kaufen!", exception.Message);
        Assert.Equal(expectedCapital, _intermediary.Capital);
        Assert.Equal(expectedStorageCapacity, _intermediary.StorageCapacity);
    }
}
