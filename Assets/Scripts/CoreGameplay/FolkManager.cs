﻿#nullable enable
using NovemberProject.CommonUIStuff;
using NovemberProject.CoreGameplay.Messages;
using NovemberProject.System;
using UniRx;
using UnityEngine;
using UnityEngine.Assertions;

namespace NovemberProject.CoreGameplay
{
    public sealed class FolkManager : InitializableBehaviour
    {
        private readonly ReactiveProperty<int> _folkCount = new();
        private readonly ReactiveProperty<int> _farmFolk = new();
        private readonly ReactiveProperty<int> _idleFolk = new();
        private readonly ReactiveProperty<int> _marketFolk = new();
        private readonly ReactiveProperty<int> _tax = new();

        [SerializeField]
        private int _startingFolkTax = 2;

        [SerializeField]
        private int _startingFolkCount = 3;

        [SerializeField]
        private int _startingFarmWorkers = 1;

        [SerializeField]
        private int _startingMarketWorkers = 1;

        public IReadOnlyReactiveProperty<int> FolkCount => _folkCount;
        public IReadOnlyReactiveProperty<int> FarmFolk => _farmFolk;
        public IReadOnlyReactiveProperty<int> IdleFolk => _idleFolk;
        public IReadOnlyReactiveProperty<int> MarketFolk => _marketFolk;
        public IReadOnlyReactiveProperty<int> Tax => _tax;

        public void InitializeGameData()
        {
            _folkCount.Value = _startingFolkCount;
            _idleFolk.Value = _startingFolkCount - _startingFarmWorkers - _startingMarketWorkers;
            _tax.Value = _startingFolkTax;
            _farmFolk.Value = _startingFarmWorkers;
            _marketFolk.Value = _startingMarketWorkers;
        }

        public void StartRound()
        {
        }

        public void EndRound()
        {
            KillPoorAndHungry();
            PayTaxes();
            EatFood();
        }

        public void BuyFolkForFood()
        {
            CoreGameplay coreGameplay = Game.Instance.CoreGameplay;
            int newFolkCost = coreGameplay.NewFolkForFoodCost;
            Assert.IsTrue(Game.Instance.FoodController.FolkFood.Value >= newFolkCost);
            Game.Instance.FoodController.SpendFolkFood(newFolkCost);
            _folkCount.Value++;
            _idleFolk.Value++;
        }

        public void AddFolkToFarm()
        {
            Assert.IsTrue(_idleFolk.Value >= 0);
            _idleFolk.Value--;
            _farmFolk.Value++;
        }

        public void RemoveFolkFromFarm()
        {
            Assert.IsTrue(_farmFolk.Value >= 0);
            _farmFolk.Value--;
            _idleFolk.Value++;
        }

        public void AddFolkToMarket()
        {
            Assert.IsTrue(_idleFolk.Value >= 0);
            Assert.IsTrue(_marketFolk.Value == 0);
            _idleFolk.Value--;
            _marketFolk.Value++;
        }

        public void RemoveFolkFromMarket()
        {
            Assert.IsTrue(_marketFolk.Value >= 0);
            _marketFolk.Value--;
            _idleFolk.Value++;
        }

        public void RaiseTax()
        {
            _tax.Value++;
        }

        public void LowerTax()
        {
            Assert.IsTrue(_tax.Value > 1);
            _tax.Value--;
        }

        private void PayTaxes()
        {
            int taxesToPay = _folkCount.Value * _tax.Value;
            if (taxesToPay == 0)
            {
                return;
            }

            Assert.IsTrue(Game.Instance.MoneyController.FolkMoney.Value >= taxesToPay);
            Game.Instance.MoneyController.TransferMoneyFromFolkToGovernment(taxesToPay);
        }

        private void EatFood()
        {
            CoreGameplay coreGameplay = Game.Instance.CoreGameplay;
            int foodToEat = _folkCount.Value * coreGameplay.FoodPerPerson;
            Assert.IsTrue(Game.Instance.FoodController.FolkFood.Value >= foodToEat);
            Game.Instance.FoodController.SpendFolkFood(foodToEat);
        }

        private void KillPoorAndHungry()
        {
            int starvedFolk = CalculateStarvingFolk();
            if (starvedFolk > 0)
            {
                Game.PublishMessage(new FolkStarvedMessage(starvedFolk));
            }

            int executedFolk = CalculatePoorFolk();
            if (executedFolk > 0)
            {
                Game.PublishMessage(new FolkExecutedMessage(executedFolk));
            }

            int folkToKill = Mathf.Max(starvedFolk, executedFolk);
            if (folkToKill > 0)
            {
                KillFolk(folkToKill);
            }
        }

        private int CalculateStarvingFolk()
        {
            FoodController foodController = Game.Instance.FoodController;
            CoreGameplay coreGameplay = Game.Instance.CoreGameplay;
            int foodPerPerson = coreGameplay.FoodPerPerson;
            int folkFood = foodController.FolkFood.Value;
            int maxFolkToEat = folkFood / foodPerPerson;
            if (_folkCount.Value <= maxFolkToEat)
            {
                return 0;
            }
            return _folkCount.Value-maxFolkToEat;
        }

        private int CalculatePoorFolk()
        {
            MoneyController moneyController = Game.Instance.MoneyController;
            int folkMoney = moneyController.FolkMoney.Value;
            int maxFolkToPay = folkMoney / _tax.Value;
            if (_folkCount.Value <= maxFolkToPay)
            {
                return 0;
            }

            return _folkCount.Value - maxFolkToPay;
        }

        private void KillFolk(int numberToExecute)
        {
            Assert.IsTrue(_folkCount.Value >= numberToExecute);
            _folkCount.Value -= numberToExecute;

            KillFolk(_folkCount, ref numberToExecute);
            KillFolk(_farmFolk, ref numberToExecute);
            KillFolk(_marketFolk, ref numberToExecute);
            Assert.IsTrue(ValidateTotalCount());
        }

        private static void KillFolk(IReactiveProperty<int> folkCount, ref int numberToKill)
        {
            // ReSharper disable once ComplexConditionExpression
            if (numberToKill == 0 || folkCount.Value == 0)
            {
                return;
            }

            if (folkCount.Value >= numberToKill)
            {
                folkCount.Value -= numberToKill;
                return;
            }

            numberToKill -= folkCount.Value;
            folkCount.Value = 0;
        }

        private bool ValidateTotalCount()
        {
            return _idleFolk.Value + _farmFolk.Value + _marketFolk.Value == _folkCount.Value;
        }
    }
}