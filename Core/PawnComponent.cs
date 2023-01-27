using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;


namespace StudioScor.PlayerSystem
{

    [DefaultExecutionOrder(PlayerSystemExecutionOrder.MAIN_OREDER)]
    [AddComponentMenu("StudioScor/PlayerSystem/Pawn Component", order: 1)]
    public class PawnComponent : MonoBehaviour
    {
        #region Events
        public delegate void ControllerStateHandler(PawnComponent pawn, ControllerComponent controller);
        public delegate void InputStateHandler(PawnComponent pawn, bool isIgnoreInput);

        #endregion
        [Header(" [ Pawn System ] ")]
        [SerializeField] protected PlayerManager _PlayerManager;

        [Header(" [ Use Player Controller ] ")]
        [SerializeField] private bool _IsPlayer = false;

        [Header(" [ Default Ai Controller ] ")]
        [SerializeField] private ControllerComponent _DefaultController;

        [SerializeField] private ControllerComponent _CurrentController;
        public ControllerComponent DefaultController => _DefaultController;
        public ControllerComponent CurrentController => _CurrentController;

        [SerializeField] private bool _UseAutoPossesed = true;

        [Header(" [ Ignore Movement Input ] ")]
        [SerializeField] private bool _IgnoreMovementInput = false;
        public bool IgnoreMovementInput => _IgnoreMovementInput;

        [Header(" [ Ignore Rotate Input ]")]
        [SerializeField] private bool _IgnoreRotateInput = false;


        public bool IgnoreRotateInput => _IgnoreRotateInput;
        public bool IsPlayer => _IsPlayer;
        public bool IsPossessed => CurrentController;
        
        public event ControllerStateHandler OnPossessedController;
        public event ControllerStateHandler OnUnPossessedController;

        public event InputStateHandler OnChangedMovementInputState;
        public event InputStateHandler OnChangedRotateInputState;


        #region EDITOR ONLY
#if UNITY_EDITOR
        [Header(" [ Use Debug ] ")]
        [SerializeField] protected bool _UseDebug = false;
#endif

        [Conditional("UNITY_EDITOR")]
        protected void Log(object message, bool isError = false)
        {
#if UNITY_EDITOR
            if (isError)
            {
                UnityEngine.Debug.LogError(GetType().Name + " [" + name + "] -" + message, this);
            }
            else if (_UseDebug)
            {
                UnityEngine.Debug.Log(GetType().Name + " [" + name +"] -" + message, this);
            }
#endif
        }
        #endregion

        protected void Start()
        {
            TryAutoPossessd();

            if (_PlayerManager.PlayerPawn != this)
                _IsPlayer = false;
        }

        private void TryAutoPossessd()
        {
            Log(nameof(TryAutoPossessd));

            if (_CurrentController != null)
            {
                _CurrentController.OnPossess(this);

                return;
            }

            if (_UseAutoPossesed)
            {
                if (IsPlayer)
                {
                    _PlayerManager.TrySetPlayerPawn(this);

                    if (_PlayerManager.PlayerController == null)
                    {
                        _PlayerManager.SpawnPlayerController(transform.position, transform.rotation);
                    }
                }
                else
                {
                    SpawnAndPossessAiController();
                }
            }
        }   

        internal void OnPossess(ControllerComponent controller)
        {
            if (CurrentController == controller)
                return;

            Log(nameof(OnPossess) + "-" + controller.name);

            if (_CurrentController)
            {
                _CurrentController.UnPossess(this);
            }

            _CurrentController = controller;

            if (!CurrentController)
                return;

            if (CurrentController.IsPlayerController)
            {
                _IsPlayer = true;
            }

            Callback_OnPossessedController();
        }

        internal void UnPossess()
        {
            if (!_CurrentController)
                return;

            Log(nameof(UnPossess) + "-" + _CurrentController.name);

            var controller = _CurrentController;

            _CurrentController = null;

            if (controller.IsPlayerController)
            {
                _IsPlayer = false;

                if (_UseAutoPossesed)
                {
                    SpawnAndPossessAiController();
                }
            }

            Callback_OnUnPossessedController(controller);
        }

        protected virtual void SpawnAndPossessAiController()
        {
            Log("Spawn And Possess Ai Controller.");

            if (_DefaultController != null)
            {
                var controller = Instantiate(_DefaultController);

                controller.OnPossess(this);
            }
        }

        #region Setter
        public void SetIgnoreMovementInput(bool useMovementInput)
        {
            if (_IgnoreMovementInput == useMovementInput)
            {
                return;
            }

            _IgnoreMovementInput = useMovementInput;

            Callback_OnChangedMovementInputState();
        }
        public void SetIgnoreRotateInput(bool useRotateInput)
        {
            if (_IgnoreRotateInput == useRotateInput)
            {
                return;
            }

            _IgnoreRotateInput = useRotateInput;

            Callback_OnChangedRotateInputState();
        }
        #endregion

        #region Getter
        public Vector3 MoveDirection
        {
            get
            {
                if (IgnoreMovementInput || !CurrentController)
                    return Vector3.zero;

                return CurrentController.MoveDirection;
            }
        }
        public float MoveStrength
        {
            get
            {
                if (IgnoreMovementInput || !CurrentController)
                    return 0;

                return CurrentController.MoveStrength;
            }
        }
        public Vector3 TurnDirection
        {
            get
            {
                if (IgnoreRotateInput || !CurrentController)
                    return Vector3.zero;

                return CurrentController.TurnDirection;
            }
        }
        #endregion

        #region Callback
        protected void Callback_OnPossessedController()
        {
            Log("On Possessed Controller [" + gameObject.name + "] " + CurrentController);

            OnPossessedController?.Invoke(this, CurrentController);
        }
        protected void Callback_OnUnPossessedController(ControllerComponent prevController)
        {
            Log("On UnPossessed Controller [" + gameObject.name + "] " + prevController);

            OnUnPossessedController?.Invoke(this, prevController);
        }

        protected void Callback_OnChangedMovementInputState()
        {
            Log("On Changed Ignore Movement Input : " + IgnoreMovementInput);

            OnChangedMovementInputState?.Invoke(this, IgnoreMovementInput);
        }

        protected void Callback_OnChangedRotateInputState()
        {
            Log("On Change Ignore Rotate Input : " + IgnoreRotateInput);

            OnChangedRotateInputState?.Invoke(this, IgnoreRotateInput);
        }

        #endregion
    }

}

