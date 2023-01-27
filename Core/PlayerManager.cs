using UnityEngine;
using System.Collections.Generic;
using System;
using System.Diagnostics;

namespace StudioScor.PlayerSystem
{

    [CreateAssetMenu(menuName ="StudioScor/PlayerManager/new PlayerManager", fileName = "PlayerManager")]
    public class PlayerManager : ScriptableObject
    {
        #region Event
        public delegate void SetPlayerPawnEventHandler(PlayerManager playerManager, PawnComponent currentPawn, PawnComponent prevPawn = null);
        public delegate void SetPlayerContollerEventHandler(PlayerManager playerManager, ControllerComponent currentController, ControllerComponent prevController = null);
        #endregion

        [Header(" [ Player Manager ] ")]
        [SerializeField] private ControllerComponent _DefaultPlayerController;
        [SerializeField] private PawnComponent _DefualtPlayerPawn;

        private ControllerComponent _PlayerController;
        private PawnComponent _PlayerPawn;

        public event SetPlayerPawnEventHandler OnChangedPlayerPawn;
        public event SetPlayerContollerEventHandler OnChangedPlayerController;

        public PawnComponent PlayerPawn => _PlayerPawn;
        public ControllerComponent PlayerController => _PlayerController;

        #region EDITOR ONLY

#if UNITY_EDITOR
        [Header(" [ Use Debug ] ")]
        [SerializeField] private bool _UseDebug;
#endif
        protected void Log(object message, bool useError = false)
        {
#if UNITY_EDITOR
            if (useError)
            {
                UnityEngine.Debug.LogError(GetType().Name + " [ " + name + " ] - " + message, this);
            }
            else
            {
                if (_UseDebug)
                {
                    UnityEngine.Debug.Log(GetType().Name + " [ " + name + " ] - " + message, this);
                }
            }
#endif
        }
        #endregion

        public virtual void SpawnPlayer(Vector3 position, Quaternion rotation)
        {
            Log(nameof(SpawnPlayer));

            SpawnPlayerController(position, rotation);
            SpawnPlayerPawn(position, rotation);
        }
        public virtual void SpawnPlayerController(Vector3 position, Quaternion rotation)
        {
            Log(nameof(SpawnPlayerController));

            Instantiate(_DefaultPlayerController, position, rotation);
        }
        public virtual void SpawnPlayerPawn(Vector3 position, Quaternion rotation)
        {
            Log(nameof(SpawnPlayerPawn));

            Instantiate(_DefualtPlayerPawn, position, rotation);
        }

        public bool TrySetPlayerPawn(PawnComponent pawnComponent)
        {
            if (!pawnComponent || _PlayerPawn != null)
                return false;

            Log(nameof(TrySetPlayerPawn));

            ForceSetPlayerPawn(pawnComponent);

            return true;
        }
        public bool TrySetPlayerController(ControllerComponent controller)
        {
            if (!controller || _PlayerController != null)
                return false;

            Log(nameof(TrySetPlayerController));

            ForceSetPlayerContoller(controller);

            return true;
        }

        public void ForceSetPlayerPawn(PawnComponent pawnComponent = null)
        {
            if (_PlayerPawn == pawnComponent)
                return;

            Log(nameof(ForceSetPlayerPawn));

            var prevPawn = _PlayerPawn;

            _PlayerPawn = pawnComponent;

            if (_PlayerPawn != null)
            {
                if (_PlayerController == null)
                {
                    SpawnPlayerController(pawnComponent.transform.position, pawnComponent.transform.rotation);
                }

                _PlayerController.OnPossess(_PlayerPawn);
            }

            Callback_OnChangedPlayerPawn(prevPawn);
        }

        public void ForceSetPlayerContoller(ControllerComponent controllerComponent = null)
        {
            if (_PlayerController == controllerComponent)
                return;

            Log(nameof(ForceSetPlayerContoller));

            var prevController = _PlayerController;

            if (prevController != null)
            {
                prevController.OnPossessedPawn -= PlayerController_OnPossessedPawn;
            }

            _PlayerController = controllerComponent;

            if (_PlayerController != null)
            {
                _PlayerController.OnPossessedPawn += PlayerController_OnPossessedPawn;

                if (_PlayerPawn != null)
                    _PlayerController.OnPossess(_PlayerPawn);
            }

            Callback_OnChangedPlayerController(prevController);
        }

        private void PlayerController_OnPossessedPawn(ControllerComponent controller, PawnComponent pawn)
        {
            if (_PlayerPawn == pawn)
                return;

            TrySetPlayerPawn(pawn);
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

