
namespace IntermediaryProject {
    [AttributeUsage(AttributeTargets.Field)]
    public class EnumOrderAttribute : Attribute {
        public readonly int Order;

        public EnumOrderAttribute(int order) {
            Order = order;
        }
    }
    public enum DifficultyLevel : int {
        [EnumOrder(2)]
        Schwer = 7_000,
        [EnumOrder(1)]
        Normal = 10_000,
        [EnumOrder(0)]
        Einfach = 15_000,
    }
    public enum GameOption {
        [EnumOrder(0)]
        Shopping = 101,
        [EnumOrder(1)]
        EndRound = 98,
    }
    public enum ShoppingOption {
        [EnumOrder(0)]
        Back = 122,

    }
}