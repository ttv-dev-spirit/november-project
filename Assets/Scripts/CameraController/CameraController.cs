#nullable enable
using System;
using NovemberProject.CommonUIStuff;
using UnityEngine;

namespace NovemberProject.CameraController
{
    [RequireComponent(typeof(CameraMovement))]
    [RequireComponent(typeof(CameraZoom))]
    public class CameraController : InitializableBehaviour
    {
        private CameraMovement _cameraMovement = null!;
        private CameraZoom _cameraZoom = null!;

        [SerializeField]
        private Camera _mainCamera = null!;

        public float KeysZoomModifier => _cameraZoom.KeysZoomModifier;
        public float MouseMoveSpeed => _cameraMovement.MouseMoveSpeed;
        public Camera MainCamera => _mainCamera;

        private void Awake()
        {
            _cameraMovement = GetComponent<CameraMovement>();
            _cameraZoom = GetComponent<CameraZoom>();
        }

        protected override void Initialize()
        {
            base.Initialize();
            _cameraZoom.SetCamera(_mainCamera);
        }

        public void MoveCamera(Vector2 direction) => _cameraMovement.MoveCamera(direction);
        public void ZoomCamera(float zoomDif) => _cameraZoom.ZoomCamera(zoomDif);
        public void SetBounds(Rect bounds) => _cameraMovement.SetBounds(bounds);
    }
}