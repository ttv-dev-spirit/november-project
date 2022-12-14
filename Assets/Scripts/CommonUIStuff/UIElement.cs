#nullable enable
using System;
using UniRx;
using UnityEngine;

namespace NovemberProject.CommonUIStuff
{
    public abstract class UIElement<T> : MonoBehaviour
    {
        private readonly Subject<UIElement<T>> _onShown = new();
        private readonly Subject<UIElement<T>> _onHidden = new();

        public IObservable<UIElement<T>> OnShown => _onShown;
        public IObservable<UIElement<T>> OnHidden => _onHidden;

        public bool IsShown { get; private set; }

        public void Show(T value)
        {
            gameObject.SetActive(true);
            OnShow(value);
            IsShown = true;
            _onShown.OnNext(this);
        }

        public void Hide()
        {
            OnHide();
            IsShown = false;
            gameObject.SetActive(false);
            _onHidden.OnNext(this);
        }

        protected abstract void OnShow(T value);
        protected abstract void OnHide();
    }
}