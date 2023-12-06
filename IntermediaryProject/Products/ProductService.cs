using IntermediaryProject.Exceptions;

namespace IntermediaryProject.Products;

public static class ProductService {
    private static readonly Random s_rnd = new Random();

    public static void ProduceProduct(Product product) {
        var producedQuantity = s_rnd.Next(product.MinProductionRate, product.MaxProductionRate + 1);
        if (product.Availability + producedQuantity < 0) {
            product.Availability = 0;
        } else {
            product.Availability += producedQuantity;
        }
    }

    public static void CalculatePurchasePrice(Product product) {
        int changeInPercent;
        if (product.Availability < (product.MaxAvailability * 0.25)) {
            changeInPercent = s_rnd.Next(-10, 31);
        } else if (product.Availability > (product.MaxAvailability * 0.25) &&
                   product.Availability < (product.MaxAvailability * 0.8)) {
            changeInPercent = s_rnd.Next(-5, 6);
        } else {
            changeInPercent = s_rnd.Next(-10, 7);
        }

        var priceChange = changeInPercent / 100.0 * product.BasePrice;
        product.Price += priceChange < 0 ? (int)Math.Floor(priceChange) : (int)Math.Ceiling(priceChange);
    }

    public static void ReduceAvailabilityWhenBuying(Product product, int quantity) {
        if (product.Availability - quantity < 0) {
            throw new ProductNotAvailableException(
                "Es kann nicht mehr von einem Produkt gekauft werden, als verfügbar ist."
            );
        }

        product.Availability -= quantity;
    }

    public static void ReverseBuyingProcess(Product product, int quantity) {
        product.Availability += quantity;
    }
}
