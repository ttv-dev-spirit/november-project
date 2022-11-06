﻿#nullable enable
using UniRx;
using UnityEngine;

namespace NovemberProject.System.UI
{
    public abstract class InitializableBehaviour : MonoBehaviour
    {
        protected void OnEnable()
        {
            if (Game.Instance.IsInitialized)
            {
                Initialize();
                return;
            }

            Game.Instance.OnInitialized
                .TakeUntilDisable(this)
                .Subscribe(_ => Initialize());
        }

        protected abstract void Initialize();
    }
}