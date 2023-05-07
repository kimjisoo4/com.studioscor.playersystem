using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StudioScor.Utilities;


namespace StudioScor.PlayerSystem
{
    public interface IPawnSystem
    {
        public Vector3 MoveDirection { get; }
        public float MoveStrength { get; }
        public Vector3 TurnDirection { get; }
        public Vector3 LookPosition { get; }
        public Transform LookTarget { get; }
        public Vector3 GetLookPosition();
    }
    public interface IPawnSystemEvent
    {

    }

    [DefaultExecutionOrder(PlayerSystemExecutionOrder.MAIN_ORDER)]
    [AddComponentMenu("StudioScor/PlayerSystem/Pawn Component", order: 1)]
    public class PawnComponent : BaseMonoBehaviour, IPawnSystem
    {
        #region Events
        public delegate void ControllerStateHandler(PawnComponent pawn, ControllerComponent controller);
        public delegate void InputStateHandler(PawnComponent pawn, bool isIgnoreInput);

        #endregion
        [Header(" [ Pawn System ] ")]
        [SerializeField] protected PlayerManager _PlayerManager;

        [Header(" [  Controller ] ")]
        [SerializeField] private bool _IsStartPlayer = false;
        [SerializeField] private ControllerComponent _DefaultController;
        [SerializeField][SReadOnlyWhenPlaying] private ControllerComponent _CurrentController;
        [SerializeField] private bool _UseAutoPossesed = true;

        [Header(" [ Input ] ")]
        [SerializeField] private bool _IgnoreMovementInput = false;
        [SerializeField] private bool _IgnoreRotateInput = false;

        public ControllerComponent DefaultController => _DefaultController;
        public ControllerComponent Controller => _CurrentController;

        public bool IgnoreMovementInput => _IgnoreMovementInput;
        public bool IgnoreRotateInput => _IgnoreRotateInput;

        public bool IsPlayer => IsPossessed && Controller.IsPlayer;
        public bool IsPossessed => Controller;
        
        public event ControllerStateHandler OnPossessedController;
        public event ControllerStateHandler OnUnPossessedController;

        public event InputStateHandler OnChangedMovementInputState;
        public event InputStateHandler OnChangedRotateInputState;


        private void Start()
        {
            if (_IsStartPlayer)
                ForceSetPlayerPawn();

            TryAutoPossessed();
        }

        public void ForceSetPlayerPawn()
        {
            _PlayerManager.ForceSetPlayerPawn(this);

            if (_PlayerManager.HasPlayerController)
            {
                _PlayerManager.PlayerController.OnPossess(this);
            }
        }
        public void SetStartPlayer(bool isPlayer)
        {
            _IsStartPlayer = isPlayer;
        }

        private void TryAutoPossessed()
        {
            Log("Try Auto Possessed ");

            if (_CurrentController != null)
            {
                _CurrentController.OnPossess(this);

                return;
            }

            if(!_IsStartPlayer && _UseAutoPossesed)
            {
                SpawnAndPossessAiController();
            }
        }   

        public void OnPossess(ControllerComponent controller)
        {
            if (Controller == controller)
                return;

            Log("On Possess -" + controller.name);

            if (_CurrentController)
            {
                _CurrentController.UnPossess(this);
            }

            _CurrentController = controller;

            if (!Controller)
                return;

            _IsStartPlayer = Controller.IsPlayer;

            Callback_OnPossessedController();
        }

        public void UnPossess()
        {
            if (!_CurrentController)
                return;

            Log("UnPossess -" + _CurrentController.name);

            var prevController = _CurrentController;

            _CurrentController = null;

            _IsStartPlayer = false;

            Callback_OnUnPossessedController(prevController);
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
                if (IgnoreMovementInput || !Controller)
                    return Vector3.zero;

                return Controller.MoveDirection;
            }
        }
        public float MoveStrength
        {
            get
            {
                if (IgnoreMovementInput || !Controller)
                    return 0;

                return Controller.MoveStrength;
            }
        }
        public Vector3 TurnDirection
        {
            get
            {
                if (IgnoreRotateInput || !Controller)
                    return Vector3.zero;

                return Controller.TurnDirection;
            }
        }
        public Vector3 LookPosition
        {
            get
            {
                if (!Controller)
                    return Vector3.forward;

                return Controller.LookPosition;
            }
        }
        public Transform LookTarget
        {
            get
            {
                if (!Controller)
                    return null;

                return Controller.LookTarget;
            }
        }

        public Vector3 GetLookPosition()
        {
            return Controller.GetLookPosition();
        }
        #endregion

        #region Callback
        protected void Callback_OnPossessedController()
        {
            Log("On Possessed Controller [" + gameObject.name + "] " + Controller);

            OnPossessedController?.Invoke(this, Controller);
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

