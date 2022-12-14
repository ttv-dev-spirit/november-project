#nullable enable
using System.Collections.Generic;
using NovemberProject.Core.FolkManagement;
using NovemberProject.GameStates;
using NovemberProject.Rounds.UI;
using NovemberProject.System.Messages;
using NovemberProject.Time;
using UniRx;

namespace NovemberProject.Core
{
    public sealed class CoreGameplay
    {
        private readonly RoundResult _roundResult = new();
        private readonly List<Timer> _timers = new();

        private readonly CoreGameplaySettings _settings;
        private readonly FolkManager _folkManager;
        private readonly ArmyManager _armyManager;
        private readonly TimeSystem _timeSystem;
        private readonly MessageBroker _messageBroker;

        private GameOverType _gameOverType;

        public GameOverType GameOverType => _gameOverType;
        public RoundResult RoundResult => _roundResult;

        public CoreGameplay(CoreGameplaySettings coreGameplaySettings, FolkManager folkManager, ArmyManager armyManager,
            TimeSystem timeSystem,
            MessageBroker messageBroker)
        {
            _settings = coreGameplaySettings;
            _folkManager = folkManager;
            _armyManager = armyManager;
            _timeSystem = timeSystem;
            _messageBroker = messageBroker;
            _messageBroker.Receive<NewGameMessage>().Subscribe(OnNewGame);
            _messageBroker.Receive<ArmyStarvedMessage>().Subscribe(OnArmyStarved);
            _messageBroker.Receive<ArmyDesertedMessage>().Subscribe(OnArmyDeserted);
            _messageBroker.Receive<FolkStarvedMessage>().Subscribe(OnFolkStarved);
            _messageBroker.Receive<FolkExecutedMessage>().Subscribe(OnFolkExecuted);
        }

        private void OnNewGame(NewGameMessage message)
        {
            _gameOverType = GameOverType.None;
        }

        public void StartRound()
        {
            _roundResult.Reset();
            StopTimers();
            StartTimers();
        }

        private void StartTimers()
        {
            Timer folkEatTimer = _timeSystem.CreateTimer(_settings.FolkEatTime, OnFolkEat);
            folkEatTimer.Start();
            _timers.Add(folkEatTimer);
            Timer armyEatTimer = _timeSystem.CreateTimer(_settings.ArmyEatTime, OnArmyEat);
            armyEatTimer.Start();
            _timers.Add(armyEatTimer);
            Timer folkPayTimer = _timeSystem.CreateTimer(_settings.FolkPayTime, OnFolkPay);
            folkPayTimer.Start();
            _timers.Add(folkPayTimer);
            Timer armyPayTimer = _timeSystem.CreateTimer(_settings.ArmyPayTime, OnArmyPay);
            armyPayTimer.Start();
            _timers.Add(armyPayTimer);
            folkEatTimer.Start();
        }

        private void OnFolkEat(Timer timer)
        {
            _timers.Remove(timer);
            _folkManager.EatFood();
        }

        private void OnArmyEat(Timer timer)
        {
            _timers.Remove(timer);
            _armyManager.EatFood();
        }

        private void OnFolkPay(Timer timer)
        {
            _timers.Remove(timer);
            _folkManager.PayTaxes();
        }

        private void OnArmyPay(Timer timer)
        {
            _timers.Remove(timer);
            _armyManager.PaySalary();
        }

        private void StopTimers()
        {
            foreach (Timer timer in _timers)
            {
                timer.Cancel();
            }

            _timers.Clear();
        }

        public void EndRound()
        {
        }

        public bool IsGameOver()
        {
            if (IsNoArmyLeft())
            {
                _gameOverType = GameOverType.NoArmy;
                StopTimers();
                return true;
            }

            if (!IsNoFolkLeft())
            {
                return false;
            }

            _gameOverType = GameOverType.NoFolk;
            StopTimers();
            return true;
        }

        private void OnFolkStarved(FolkStarvedMessage message)
        {
            _roundResult.FolkStarved += message.Count;
        }

        private void OnArmyStarved(ArmyStarvedMessage message)
        {
            _roundResult.ArmyStarved += message.Count;
        }

        private void OnFolkExecuted(FolkExecutedMessage message)
        {
            _roundResult.FolkExecuted += message.Count;
        }

        private void OnArmyDeserted(ArmyDesertedMessage message)
        {
            _roundResult.ArmyDeserted += message.Count;
        }

        private bool IsNoFolkLeft() => _folkManager.IsNoFolkLeft();
        private bool IsNoArmyLeft() => _armyManager.IsNoArmyLeft();
    }
}