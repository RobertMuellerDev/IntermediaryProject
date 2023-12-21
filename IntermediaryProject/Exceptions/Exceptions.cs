namespace IntermediaryProject.Exceptions {
    [Serializable]
    public class ProductNotAvailableException : Exception {
        public ProductNotAvailableException() {
        }

        public ProductNotAvailableException(string message) : base(message) {
        }

        public ProductNotAvailableException(string message, Exception inner) : base(message, inner) {
        }
    }

    [Serializable]
    public class IntermediaryBuyException : Exception {
        public IntermediaryBuyException() {
        }

        public IntermediaryBuyException(string message) : base(message) {
        }

        public IntermediaryBuyException(string message, Exception inner) : base(message, inner) {
        }
    }

    [Serializable]
    public class IntermediarySellException : Exception {
        public IntermediarySellException() {
        }

        public IntermediarySellException(string message) : base(message) {
        }

        public IntermediarySellException(string message, Exception inner) : base(message, inner) {
        }
    }

    [Serializable]
    public class IntermediaryLoanException : Exception {
        public IntermediaryLoanException() {
        }

        public IntermediaryLoanException(string message) : base(message) {
        }

        public IntermediaryLoanException(string message, Exception inner) : base(message, inner) {
        }
    }

    [Serializable]
    public class IntermediaryExpandStorageException : Exception {
        public IntermediaryExpandStorageException() {
        }

        public IntermediaryExpandStorageException(string message) : base(message) {
        }

        public IntermediaryExpandStorageException(string message, Exception inner) : base(message, inner) {
        }
    }

    [Serializable]
    public class EndGameException : Exception {
        public EndGameException() {
        }

        public EndGameException(string message) : base(message) {
        }

        public EndGameException(string message, Exception inner) : base(message, inner) {
        }
    }
}
