namespace IntermediaryProject.Exceptions {
    [Serializable]
    public class ProductNotAvailableException : Exception {
        public ProductNotAvailableException() { }
        public ProductNotAvailableException(string message) : base(message) { }
        public ProductNotAvailableException(string message, Exception inner) : base(message, inner) { }
    }

    [Serializable]
    public class IntermediaryBuyException : Exception {
        public IntermediaryBuyException() { }
        public IntermediaryBuyException(string message) : base(message) { }
        public IntermediaryBuyException(string message, Exception inner) : base(message, inner) { }
    }
}
