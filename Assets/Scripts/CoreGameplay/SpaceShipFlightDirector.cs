﻿#nullable enable
using System;
using DG.Tweening;
using NovemberProject.Buildings;
using NovemberProject.CommonUIStuff;
using NovemberProject.System;
using UniRx;
using UnityEngine;

namespace NovemberProject.CoreGameplay
{
    public sealed class SpaceShipFlightDirector : InitializableBehaviour
    {
        private readonly Subject<Unit> _onFinishedPlaying = new();

        [SerializeField]
        private Transform _cameraPosition = null!;

        [SerializeField]
        private float _cameraZoom = 0.8f;

        [SerializeField]
        private Transform _castlePosition = null!;

        [SerializeField]
        private float _cameraMoveDuration = 2f;

        [SerializeField]
        private float _castleFlightDuration = 5f;

        public IObservable<Unit> OnFinishedPlaying => _onFinishedPlaying;

        public void StartSequence()
        {
            Tween cameraTween =
                Game.Instance.CameraController.MoveTo(_cameraPosition, _cameraMoveDuration, _cameraZoom);
            cameraTween.onComplete += OnMoveFinished;
        }

        private void OnMoveFinished()
        {
            Building arena = Game.Instance.BuildingsController.GetBuilding(BuildingType.Arena);

            var tween = arena.transform.DOMove(_castlePosition.position, _castleFlightDuration);
            tween.SetEase(Ease.InQuad);
            tween.onComplete += () => _onFinishedPlaying.OnNext(Unit.Default);
        }
    }
}