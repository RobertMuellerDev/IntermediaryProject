namespace IntermediaryProject.Transactions;

public class Transaction {
    public decimal Amount { get; }
    public TransactionType Type { get; }

    public Transaction(decimal amount, TransactionType type) {
        Amount = amount;
        Type = type;
    }
}
