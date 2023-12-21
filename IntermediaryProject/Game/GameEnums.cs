namespace IntermediaryProject {
    [AttributeUsage(AttributeTargets.Field)]
    public class EnumOrderAttribute : Attribute {
        public readonly int Order;

        public EnumOrderAttribute(int order) {
            Order = order;
        }
    }

    public enum DifficultyLevel {
        [EnumOrder(2)]
        Schwer = 7_000,

        [EnumOrder(1)]
        Normal = 10_000,

        [EnumOrder(0)]
        Einfach = 15_000,
    }

    public enum GameAction {
        [EnumOrder(0)]
        Shopping = 101,

        [EnumOrder(1)]
        Selling = 118,

        [EnumOrder(2)]
        Storage = 108,

        [EnumOrder(3)]
        Loan = 107,

        EndRound = 98,
    }

    public enum TransactionType {
        Shopping,
        Selling,
        Storage,
        PayLoan,
        TakeLoan
    }
}
