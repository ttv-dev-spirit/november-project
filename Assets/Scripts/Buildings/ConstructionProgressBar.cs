#nullable enable
using System;
using NovemberProject.CommonUIStuff;
using UniRx;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace NovemberProject.Buildings
{
    public sealed class ConstructionProgressBar : MonoBehaviour
    {
        private IDisposable? _productionSub;
        private bool _isProgressShown;
        private IConstructableBuilding _constructableBuilding = null!;

        [SerializeField]
        private Image _progressImage = null!;

        [SerializeField]
        private GameObject _progressBar = null!;

        [SerializeField]
        private Building _building = null!;

        private void Start()
        {
            _constructableBuilding = (IConstructableBuilding)_building;
            _progressBar.SetActive(false);
            _constructableBuilding.ConstructableState
                .Subscribe(OnConstructableStateChanged);
        }

        private void Update()
        {
            if (!_isProgressShown)
            {
                return;
            }

            Assert.IsNotNull(_constructableBuilding.ConstructionTimer);
            _progressImage.fillAmount = _constructableBuilding.ConstructionTimer!.ProgressRate;
        }

        private void OnConstructableStateChanged(ConstructableState constructableState)
        {
            _isProgressShown = constructableState == ConstructableState.IsConstructing;
            _progressBar.SetActive(_isProgressShown);
        }
    }
}