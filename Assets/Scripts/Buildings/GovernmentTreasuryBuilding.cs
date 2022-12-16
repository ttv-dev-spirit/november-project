﻿#nullable enable
using NovemberProject.Buildings.UI;
using NovemberProject.CoreGameplay;
using NovemberProject.System;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace NovemberProject.Buildings
{
    public sealed class GovernmentTreasuryBuilding : Building, IMoneyPrinter
    {
        private MoneyController _moneyController = null!;

        [SerializeField]
        private TMP_Text _moneyText = null!;

        [SerializeField]
        private int _moneyToPrint = 10;

        public override BuildingType BuildingType => BuildingType.GovernmentTreasury;
        public IReadOnlyReactiveProperty<bool> CanPrintMoney => Game.Instance.TechController.CanPrintMoney;
        public IReadOnlyReactiveProperty<bool> CanBurnMoney => Game.Instance.TechController.CanBurnMoney;

        [Inject]
        private void Construct(MoneyController moneyController)
        {
            _moneyController = moneyController;
            _moneyController.GovernmentMoney.Subscribe(OnMoneyChanged);
        }

        private void OnMoneyChanged(int money)
        {
            _moneyText.text = money.ToString();
        }

        public void PrintMoney() => _moneyController.PrintMoney(_moneyToPrint);
        public void BurnMoney() => _moneyController.BurnMoney(_moneyToPrint);
    }
}