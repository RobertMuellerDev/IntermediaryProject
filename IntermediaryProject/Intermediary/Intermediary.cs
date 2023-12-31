using IntermediaryProject.Transactions;

namespace IntermediaryProject {
    public class Intermediary : IComparable<Intermediary> {
        public int AvailableStorageCapacity {
            get { return StorageCapacity - StorageUtilization; }
        }

        public decimal Capital { get; set; }
        public string Name { get; }
        public string CompanyName { get; }
        public Dictionary<int, int> Inventory { get; } = new();
        public int StorageCapacity { get; set; }
        public int StorageUtilization { get; set; }
        internal List<Transaction> TransactionsOfTheDay { get; } = new();
        public Dictionary<int, int> Discounts { get; set; } = new();
        public Loan? TakenOutLoan { get; set; }

        public Intermediary(string name, string companyName, int startingCapital) {
            Name = name;
            CompanyName = companyName;
            Capital = startingCapital;
            StorageCapacity = 100;
            StorageUtilization = 0;
            TakenOutLoan = null;
        }

        public int CompareTo(Intermediary? compareIntermediary) {
            // A null value means that this object is greater.
            if (compareIntermediary == null)
                return -1;

            else
                return -(Capital.CompareTo(compareIntermediary.Capital));
        }
    }
}
