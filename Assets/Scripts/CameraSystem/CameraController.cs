#nullable enable
using NovemberProject.CommonUIStuff;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace NovemberProject.CameraSystem
{
    [RequireComponent(typeof(CameraMovement))]
    [RequireComponent(typeof(CameraZoom))]
    public sealed class CameraController : InitializableBehaviour
    {
        private CameraMovement _cameraMovement = null!;
        private CameraZoom _cameraZoom = null!;

        [SerializeField]
        private Camera _mainCamera = null!;

        [SerializeField, Range(0, 1)]
        private float _startingZoom = .6f;

        public float KeysZoomModifier => _cameraZoom.KeysZoomModifier;
        public float MouseMoveSpeed => _cameraMovement.MouseMoveSpeed;
        public Camera MainCamera => _mainCamera;

        private void Awake()
        {
            _cameraMovement = GetComponent<CameraMovement>();
            _cameraZoom = GetComponent<CameraZoom>();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            _cameraZoom.SetCamera(_mainCamera);
        }

        public void MoveCamera(Vector2 direction) => _cameraMovement.MoveCamera(direction);
        public void ZoomCamera(float zoomDif) => _cameraZoom.ZoomCamera(zoomDif);
        public void SetBounds(Rect bounds) => _cameraMovement.SetBounds(bounds);

        public void InitializeGameData()
        {
            _cameraZoom.SetCameraZoom(_startingZoom);
        }
    }
}