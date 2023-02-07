using UnityEngine;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using StudioScor.Utilities;

namespace StudioScor.PlayerSystem
{
    public class PlayerManager : Singleton<PlayerManager>
    {
        #region Event
        public delegate void SetPlayerPawnEventHandler(PlayerManager playerManager, PawnComponent currentPawn, PawnComponent prevPawn = null);
        public delegate void SetPlayerContollerEventHandler(PlayerManager playerManager, ControllerComponent currentController, ControllerComponent prevController = null);
        #endregion

        [Header(" [ Player Manager ] ")]
        [SerializeField] private ControllerComponent _DefaultPlayerController;
        [SerializeField] private PawnComponent _DefualtPlayerPawn;

        private ControllerComponent _PlayerController = null;
        private PawnComponent _PlayerPawn = null;

        public event SetPlayerPawnEventHandler OnChangedPlayerPawn;
        public event SetPlayerContollerEventHandler OnChangedPlayerController;

        public PawnComponent PlayerPawn => _PlayerPawn;
        public ControllerComponent PlayerController => _PlayerController;

        private void Awake()
        {
            SpawnPlayerController();
        }

        public virtual void SpawnPlayerController()
        {
            Log("Spawn Player Controller");

            var controller = Instantiate(_DefaultPlayerController);

            ForceSetPlayerContoller(controller);
        }
        public virtual void SpawnPlayerPawn(Vector3 position, Quaternion rotation)
        {
            Log("Spawn Player Pawn ");

            var pawn = Instantiate(_DefualtPlayerPawn, position, rotation);

            ForceSetPlayerPawn(pawn);
        }

        public void ForceSetPlayerPawn(PawnComponent pawnComponent = null)
        {
            if (_PlayerPawn == pawnComponent)
                return;

            var prevPawn = _PlayerPawn;
            _PlayerPawn = pawnComponent;

            if (_PlayerController)
            {
                _PlayerController.OnPossess(_PlayerPawn);
            }

            Callback_OnChangedPlayerPawn(prevPawn);
        }

        public void ForceSetPlayerContoller(ControllerComponent controllerComponent = null)
        {
            if (_PlayerController == controllerComponent)
                return;

            var prevController = _PlayerController;
            _PlayerController = controllerComponent;

            if (_PlayerController && _PlayerPawn)
            {
                _PlayerController.OnPossess(_PlayerPawn);
            }

            Callback_OnChangedPlayerController(prevController);
        }

        #region Callback
        protected void Callback_OnChangedPlayerController(ControllerComponent prevController)
        {
            Log("On Changed PlayerController");

            OnChangedPlayerController?.Invoke(this, _PlayerController, prevController);
        }
        protected void Callback_OnChangedPlayerPawn(PawnComponent pawnComponent)
        {
            Log("On Changed PlayerPawn");

            OnChangedPlayerPawn?.Invoke(this, _PlayerPawn, pawnComponent);
        }
        #endregion
    }

}

