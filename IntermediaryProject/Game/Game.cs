using ExtensionMethods;
using IntermediaryProject.Products;
using IntermediaryProject.Utils;

namespace IntermediaryProject {
    class Game {
        private readonly List<Intermediary> _intermediaries = new();

        private int _numberOfIntermediaries;
        private int _day;
        private int _numberOfDaysToPlay;
        private readonly IUi _ui;

        private readonly List<Product> _availableProducts;
        private Intermediary _currentIntermediary;
        private readonly GameLogic _gameLogic;

        public Game(IUi ui, GameLogic gameLogic) {
            _ui = ui;
            _gameLogic = gameLogic;
            _availableProducts = YamlParser.ReadAvailableProductsFromFile("produkte.yml");
            AskForAmountOfDaysToPlay();
            AskForNumberOfIntermediaries();
            CreateAndSaveIntermediaries();
            _day = 1;
            _currentIntermediary = _intermediaries[0];
            ExecuteProductChangeDayOperations();
        }

        public void Play() {
            while (_day <= _numberOfDaysToPlay) {
                foreach (var intermediary in _intermediaries) {
                    _currentIntermediary = intermediary;
                    ShowReport();
                    if (Util.IsBankrupt(intermediary)) {
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

        private void ShowReport() {
            var reportData = Util.PrepareReportData(_currentIntermediary);
            _ui.PrintReport(reportData);
            while (_ui.ReadKey()
                      .Key
                   != ConsoleKey.Enter) {
            }

            _currentIntermediary.TransactionsOfTheDay.Clear();
        }

        private void RemoveBankruptIntermediaries() {
            int amountOfDeletedIntermediaries = _intermediaries.RemoveAll(Util.IsBankrupt);
            _numberOfIntermediaries -= amountOfDeletedIntermediaries;
        }

        private void ChangeToNextDay() {
            _day++;
            RotateFirstIntermediaryToTheEnd();
            ExecuteProductChangeDayOperations();
            ChargeStorageOperatingCosts();
            _gameLogic.SettleLoans(_intermediaries, _day);
        }

        private bool IsGameOver() {
            return _intermediaries.Count == 0;
        }

        private void ChargeStorageOperatingCosts() {
            foreach (var intermediary in _intermediaries) {
                IntermediaryService.PayStorageOperatingCosts(intermediary);
            }
        }

        private void ExecuteProductChangeDayOperations() {
            foreach (var product in _availableProducts) {
                ProductService.ProduceProduct(product);
                ProductService.CalculatePurchasePrice(product);
            }
        }

        private void RotateFirstIntermediaryToTheEnd() {
            var firstElement = _intermediaries.Pop(0);
            _intermediaries.Add(firstElement);
        }

        private void PlayRound() {
            _currentIntermediary.Discounts = Util.CalculateDiscounts(_currentIntermediary, _availableProducts);
            bool roundFinished;
            do {
                _ui.PrintHeader(_currentIntermediary, _day);
                _ui.PrintGameMenuActions();
                var selectedAction = _gameLogic.AskForAction();
                roundFinished = _gameLogic.PerformSelectedAction(
                    selectedAction,
                    _currentIntermediary,
                    _availableProducts,
                    _day
                );
            } while (!roundFinished);
        }

        private void CreateAndSaveIntermediaries() {
            for (var i = 1; i <= _numberOfIntermediaries; i++) {
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
            _ui.Write("Wie viel Zwischenhändler nehmen teil?: ");
            while (true) {
                var input = _ui.ReadLine();
                if (string.IsNullOrWhiteSpace(input)
                    || !int.TryParse(input, out _numberOfIntermediaries)
                    || _numberOfIntermediaries <= 0) {
                    _ui.Write("Geben Sie eine positive Zahl ein: ");
                    continue;
                }

                break;
            }
        }

        private void AskForAmountOfDaysToPlay() {
            _ui.Write("Wie viele Tage wollen Sie spielen?: ");
            while (true) {
                var input = _ui.ReadLine();
                if (string.IsNullOrWhiteSpace(input)
                    || !int.TryParse(input, out _numberOfDaysToPlay)
                    || _numberOfDaysToPlay <= 0) {
                    _ui.Write("Geben Sie eine positive Zahl ein: ");
                    continue;
                }

                break;
            }
        }
    }
}
