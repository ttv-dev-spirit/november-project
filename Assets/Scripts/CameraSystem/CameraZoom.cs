#nullable enable
using DG.Tweening;
using NovemberProject.CommonUIStuff;
using UnityEngine;

namespace NovemberProject.CameraSystem
{
    public sealed class CameraZoom : MonoBehaviour
    {
        private float _zoom;
        private float _zoomDif;
        private bool _isCameraSet;

        private Camera _camera = null!;

        [SerializeField]
        private float _minHeight = .5f;

        [SerializeField]
        private float _maxHeight = 5f;

        [SerializeField]
        private float _maxDistance = 3f;

        [SerializeField]
        private float _zoomSpeed = 120f;

        [SerializeField]
        private float _keysZoomModifier = .02f;

        public float KeysZoomModifier => _keysZoomModifier;
        public float Zoom => _zoom;

        private void LateUpdate()
        {
            if (!_isCameraSet)
            {
                return;
            }

            if (_zoomDif != 0)
            {
                UpdateCameraPosition();
            }
        }

        public void ZoomCamera(float zoomDif)
        {
            _zoomDif += zoomDif;
        }
        
        public void SetCameraZoom(float zoom)
        {
            _zoom = zoom;
            SetCameraPosition();
        }

        public void SetCamera(Camera cameraToSet)
        {
            if (cameraToSet == null)
            {
                return;
            }

            _camera = cameraToSet;
            _isCameraSet = true;
        }

        private void UpdateCameraPosition()
        {
            _zoom += _zoomDif * _zoomSpeed * UnityEngine.Time.deltaTime;
            _zoomDif = 0;
            SetCameraPosition();
        }

        private void SetCameraPosition()
        {
            _zoom = Mathf.Clamp(_zoom, 0, 1);
            float cameraHeight = Mathf.Lerp(_maxHeight, _minHeight, _zoom);
            float cameraDistance = Mathf.Lerp(-_maxDistance, 0, _zoom);
            var cameraPosition = new Vector3(0, cameraHeight, cameraDistance);
            var cameraRotation = new Vector3(Mathf.Lerp(60, 0, _zoom), 0, 0);
            _camera.transform.localPosition = cameraPosition;
            _camera.transform.localRotation = Quaternion.Euler(cameraRotation);
        }

        public void TweenZoom(float targetZoom, float duration)
        {
            targetZoom = Mathf.Clamp(targetZoom, 0, 1);
            float cameraHeight = Mathf.Lerp(_maxHeight, _minHeight, targetZoom);
            float cameraDistance = Mathf.Lerp(-_maxDistance, 0, targetZoom);
            var cameraPosition = new Vector3(0, cameraHeight, cameraDistance);
            var cameraRotation = new Vector3(Mathf.Lerp(60, 0, targetZoom), 0, 0);
            _camera.transform.DOLocalMove(cameraPosition, duration);
            _camera.transform.DOLocalRotate(cameraRotation, duration);
        }
    }
}