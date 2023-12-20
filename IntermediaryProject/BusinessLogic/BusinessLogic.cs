using IntermediaryProject.Exceptions;
using IntermediaryProject.Products;

namespace IntermediaryProject;

static class BusinessLogic {
    internal static void SellSelectedQuantityOfProduct(Intermediary intermediary, Product product, int quantity) {
        IntermediaryService.SellProducts(intermediary, product, quantity);
    }

    internal static void BuyProduct(Intermediary intermediary, Product product, int quantity) {
        try {
            ProductService.ReduceAvailabilityWhenBuying(product, quantity);
            IntermediaryService.BuyProduct(intermediary, product, quantity);
        } catch (IntermediaryBuyException e) {
            ProductService.ReverseBuyingProcess(product, quantity);
            throw;
        }
    }

    public static void TakeOutLoan(Intermediary intermediary, (int amount, int interest) loanOption, int day) {
        IntermediaryService.TakeOutALoan(
            intermediary,
            loanOption.amount,
            loanOption.interest,
            day
        );
    }
}
