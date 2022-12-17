﻿#nullable enable
using NovemberProject.GameStates;
using NovemberProject.System;
using UnityEngine;
using Zenject;

namespace NovemberProject.Input
{
    public sealed class EscapeToMainMenuHandler : InputHandler
    {
        // TODO (Stas): Hack. I just don't want to deal with this right now.
        private GameStateMachine _gameStateMachine = null!;

        [Inject]
        private void Construct(GameStateMachine gameStateMachine)
        {
            _gameStateMachine = gameStateMachine;
        }

        public override void HandleInput()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
            {
                _gameStateMachine.MainMenu();
            }
        }
    }
}