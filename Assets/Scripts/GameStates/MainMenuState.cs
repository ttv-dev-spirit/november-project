﻿#nullable enable
using NovemberProject.System;

namespace NovemberProject.GameStates
{
    public sealed class MainMenuState : State
    {
        protected override void OnEnter()
        {
            Game.Instance.CameraController.TurnCameraOff();
            Game.Instance.UIManager.HideRoundTimer();
            Game.Instance.UIManager.HideTimeControls();
            Game.Instance.UIManager.ShowMainMenu();
            Game.Instance.TimeSystem.PauseTime();
        }

        protected override void OnExit()
        {
            Game.Instance.CameraController.TurnCameraOn();
            Game.Instance.UIManager.HideMainMenu();
        }
    }
}