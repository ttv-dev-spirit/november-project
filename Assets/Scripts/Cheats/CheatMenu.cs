﻿#nullable enable
using System.Collections.Generic;
using NovemberProject.CommonUIStuff;
using NovemberProject.CoreGameplay;
using NovemberProject.System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace NovemberProject.Cheats
{
    [RequireComponent(typeof(ContentSizeFitter))]
    public sealed class CheatMenu : UIElement<object?>
    {
        private readonly List<CheatButtonInfo> _cheatButtons = new();
        private readonly List<CheatButton> _generatedButtons = new();

        private MoneyController _moneyController = null!;

        [SerializeField]
        private CheatButton _buttonPrefab = null!;

        [SerializeField]
        private Transform _buttonsParent = null!;

        private bool _isSizeFitterRefreshNeeded;

        [Inject]
        private void Construct(MoneyController moneyController)
        {
            _moneyController = moneyController;
        }

        private void Start()
        {
            ClearOldButtons();
            PrepareButtonsList();
            GenerateButtons();
        }

        protected override void OnShow(object? _)
        {
            if (_generatedButtons.Count != 0)
            {
                return;
            }

            Debug.LogWarning("No cheat buttons to show");
            Hide();
        }

        protected override void OnHide()
        {
        }

        private void PrepareButtonsList()
        {
            _cheatButtons.Add(new CheatButtonInfo("Print 10 Money", Print10Money));
            _cheatButtons.Add(new CheatButtonInfo("Add 10 stone", Add10Stone));
            _cheatButtons.Add(new CheatButtonInfo("Win", Win));
            _cheatButtons.Add(new CheatButtonInfo("Add treasure", AddTreasure));
        }

        private void Print10Money()
        {
            _moneyController.PrintMoney(10);
        }

        private void Add10Stone()
        {
            Game.Instance.StoneController.AddStone(10);
        }

        private void AddTreasure()
        {
            Game.Instance.TreasureController.AddTreasures(1);
        }

        private void Win()
        {
            Game.Instance.GameStateMachine.Victory();
        }

        private void GenerateButtons()
        {
            foreach (CheatButtonInfo cheatButtonInfo in _cheatButtons)
            {
                CheatButton button = Instantiate(_buttonPrefab, _buttonsParent);
                button.Show(cheatButtonInfo);
                _generatedButtons.Add(button);
            }
        }

        private void ClearOldButtons()
        {
            _cheatButtons.Clear();
            _generatedButtons.Clear();

            var oldButtons = _buttonsParent.GetComponentsInChildren<CheatButton>();
            if (!HasOldButtons())
            {
                return;
            }

            foreach (CheatButton? button in oldButtons)
            {
                if (button == null)
                {
                    continue;
                }

                Destroy(button.gameObject);
            }

            bool HasOldButtons() => oldButtons is { Length: > 0 };
        }
    }
}