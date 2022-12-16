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
    public sealed class ArmyTreasuryBuilding : Building, IResourceStorage, ISalaryController
    {
        private MoneyController _moneyController = null!;

        [SerializeField]
        private TMP_Text _moneyText = null!;

        [SerializeField]
        private Sprite _moneySprite = null!;

        [SerializeField]
        private string _moneyTitle = "Money";

        public override BuildingType BuildingType => BuildingType.ArmyTreasury;
        public Sprite SpriteIcon => _moneySprite;
        public string ResourceTitle => _moneyTitle;
        public IReadOnlyReactiveProperty<int> ResourceCount => _moneyController.ArmyMoney;
        public IReadOnlyReactiveProperty<bool> CanRaiseSalary => Game.Instance.TechController.CanRaiseSalary;
        public IReadOnlyReactiveProperty<bool> CanLowerSalary => Game.Instance.TechController.CanLowerSalary;

        [Inject]
        private void Construct(MoneyController moneyController)
        {
            _moneyController = moneyController;
            _moneyController.ArmyMoney.Subscribe(OnMoneyChanged);
        }

        private void OnMoneyChanged(int money)
        {
            _moneyText.text = money.ToString();
        }
        
        public void RaiseSalary() => Game.Instance.ArmyManager.RaiseSalary();
        public void LowerSalary() => Game.Instance.ArmyManager.LowerSalary();
    }
}