﻿#nullable enable
using NovemberProject.CommonUIStuff;
using NovemberProject.System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace NovemberProject.Buildings.UI
{
    public sealed class BuildingConstructionPanel : UIElement<IConstructableBuilding>
    {
        private readonly CompositeDisposable _sub = new();

        private IConstructableBuilding _building = null!;

        [SerializeField]
        private TMP_Text _constructionTimerText = null!;

        [SerializeField]
        private GameObject _constructionTimerPanel = null!;

        [SerializeField]
        private Button _startConstructionButton = null!;

        [SerializeField]
        private Image _resourceImage = null!;

        [SerializeField]
        private TMP_Text _stoneCountText = null!;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            _startConstructionButton.OnClickAsObservable()
                .TakeUntilDisable(this)
                .Subscribe(OnStartConstructionClicked);
        }

        protected override void OnShow(IConstructableBuilding building)
        {
            _sub.Clear();
            _building = building;
            _building.ConstructableState.Subscribe(UpdateState).AddTo(_sub);
            Game.Instance.StoneController.Stone.Subscribe(OnStoneCountChanged).AddTo(_sub);
        }

        protected override void OnHide()
        {
            _building = null!;
            _sub.Clear();
        }

        private void Update()
        {
            if (!IsConstructing())
            {
                return;
            }

            _constructionTimerText.text =
                Game.Instance.TimeSystem.EstimateSecondsLeftUnscaled(_building.ConstructionTimer) + "s";

            bool IsConstructing() => IsShown && _building.ConstructableState.Value == ConstructableState.IsConstructing;
        }

        private void UpdateState(ConstructableState constructableState)
        {
            switch (constructableState)
            {
                case ConstructableState.NotConstructed:
                    ShowNotConstructedState();
                    break;
                case ConstructableState.IsConstructing:
                    ShowConstructingState();
                    break;
                case ConstructableState.Constructed:
                    Hide();
                    return;
            }
        }

        private void ShowNotConstructedState()
        {
            _constructionTimerPanel.SetActive(false);
            _stoneCountText.text =
                $"{Game.Instance.StoneController.Stone.Value}/{_building.ConstructionCost.ToString()}";
            _resourceImage.sprite = _building.ResourceImage;
        }

        private void ShowConstructingState()
        {
            _constructionTimerPanel.SetActive(true);
        }

        private void OnStartConstructionClicked(Unit _)
        {
            _building.StartConstruction();
        }

        private void OnStoneCountChanged(int stone)
        {
            _startConstructionButton.interactable = stone >= _building.ConstructionCost;
            _stoneCountText.text =
                $"{Game.Instance.StoneController.Stone.Value}/{_building.ConstructionCost.ToString()}";
        }
    }
}