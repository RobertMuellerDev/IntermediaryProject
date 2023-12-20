namespace IntermediaryProject;

public class Loan {
    public int Amount { get; }
    public int InterestRate { get; }
    public int DayOfRepayment { get; }

    public Loan(int amount, int interestRate, int day) {
        Amount = amount;
        InterestRate = interestRate;
        DayOfRepayment = day + 7;
    }
}
