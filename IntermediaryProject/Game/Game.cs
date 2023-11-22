using ExtensionMethods;
using IntermediaryProject.Exceptions;
using IntermediaryProject.Products;

namespace IntermediaryProject {
    class Game {
        private readonly List<Intermediary> _intermediaries = new();

        private int s_numberOfIntermediaries;
        private int _day;
        private int s_numberOfDays;
        private readonly IUi _ui;

        private readonly List<Product> _availableProducts;
        private Intermediary _currentIntermediary;
        private readonly BusinessLogic _businessLogic;

        public List<Product> Products {
            get { return _availableProducts; }
        }

        public Game(IUi ui, BusinessLogic businessLogic) {
            _ui = ui;
            _businessLogic = businessLogic;
            _availableProducts = YamlParser.ReadAvailableProductsFromFile("produkte.yml");
            AskForAmountOfDaysToPlay();
            AskForNumberOfIntermediaries();
            CreateAndSaveIntermediaries();
            _day = 1;
            _currentIntermediary = _intermediaries[0];
            ExecuteProductChangeDayOperations();
        }

        public void Play() {
            while (_day <= s_numberOfDays) {
                foreach (var intermediary in _intermediaries) {
                    _currentIntermediary = intermediary;
                    if (IsBankrupt(intermediary)) {
                        _ui.PrintBankruptcyNotification(intermediary);
                        continue;
                    }

                    PlayRound();
                }

                RemoveBankruptIntermediaries();
                if (IsGameOver()) {
                    break;
                }

                ChangeToNextDay();
            }

            _intermediaries.Sort();
            _ui.PrintLeaderboard(_intermediaries);
        }

        private void RemoveBankruptIntermediaries() {
            int amountOfDeletedIntermediaries = _intermediaries.RemoveAll(IsBankrupt);
            s_numberOfIntermediaries -= amountOfDeletedIntermediaries;
        }

        private bool IsBankrupt(Intermediary intermediary) {
            return (intermediary.Capital < 0);
        }

        private void ChangeToNextDay() {
            _day++;
            RotateFirstIntermediaryToTheEnd();
            ExecuteProductChangeDayOperations();
            ChargeStorageOperatingCosts();
        }

        private bool IsGameOver() {
            return _intermediaries.Count == 0;
        }

        private void ChargeStorageOperatingCosts() {
            foreach (var intermediary in _intermediaries) {
                intermediary.PayStorageOperatingCosts();
            }
        }

        private void ExecuteProductChangeDayOperations() {
            foreach (var product in _availableProducts) {
                product.ProduceProduct();
                product.CalculatePurchasePrice();
            }
        }

        private void RotateFirstIntermediaryToTheEnd() {
            var firstElement = _intermediaries.Pop(0);
            _intermediaries.Add(firstElement);
        }

        private void PlayRound() {
            bool roundFinished;
            do {
                _ui.PrintHeader(_currentIntermediary, _day);
                _ui.PrintGameMenuActions();
                var selectedAction = _businessLogic.AskForAction();
                roundFinished = _businessLogic.PerformSelectedAction(
                                                                     selectedAction,
                                                                     _currentIntermediary,
                                                                     _availableProducts
                                                                    );
            } while (!roundFinished);
        }

        private void CreateAndSaveIntermediaries() {
            for (var i = 1; i <= s_numberOfIntermediaries; i++) {
                _ui.Write($"Name von Zwischenhändler {i}: ");
                var intermediaryName = _ui.ReadAndValidateStringFromReadLine("Geben Sie einen gültigen Namen ein: ");
                _ui.Write($"Name von der Firma von {intermediaryName}: ");
                var intermediaryCompanyName =
                    _ui.ReadAndValidateStringFromReadLine("Geben Sie eine gültige Firma ein: ");
                var difficultyLevel = AskForDifficultyLevel();
                _intermediaries.Add(new Intermediary(intermediaryName, intermediaryCompanyName, (int)difficultyLevel));
            }
        }

        private DifficultyLevel AskForDifficultyLevel() {
            _ui.WriteLine();
            _ui.WriteLine("Wählen Sie einen Schwierigkeitsgrad aus: ");
            _ui.PrintDifficultyLevelChoice();
            return GetValidatedDifficultyLevelFromInput();
        }

        private DifficultyLevel GetValidatedDifficultyLevelFromInput() {
            DifficultyLevel? difficultyLevel;
            do {
                var difficultyLevelInput =
                    _ui.ReadAndValidateStringFromReadLine("Geben Sie einen gültigen Schwierigkeitsgrad ein: ");
                difficultyLevel = difficultyLevelInput.ToLower() switch {
                    "a" => DifficultyLevel.Einfach,
                    "b" => DifficultyLevel.Normal,
                    "c" => DifficultyLevel.Schwer,
                    _   => null,
                };
            } while (difficultyLevel is null);

            return (DifficultyLevel)difficultyLevel;
        }

        private void AskForNumberOfIntermediaries() {
            _ui.Write("Wieviel Zwischenhändler nehmen teil?: ");
            while (true) {
                var input = _ui.ReadLine();
                if (string.IsNullOrWhiteSpace(input) ||
                    !int.TryParse(input, out s_numberOfIntermediaries) ||
                    s_numberOfIntermediaries <= 0) {
                    _ui.Write("Geben Sie eine positive Zahl ein: ");
                    continue;
                }

                break;
            }
        }

        private void AskForAmountOfDaysToPlay() {
            _ui.Write("Wieviele Tage wollen Sie spielen?: ");
            while (true) {
                var input = _ui.ReadLine();
                if (string.IsNullOrWhiteSpace(input) ||
                    !int.TryParse(input, out s_numberOfDays) ||
                    s_numberOfDays <= 0) {
                    _ui.Write("Geben Sie eine positive Zahl ein: ");
                    continue;
                }

                break;
            }
        }
    }
}
