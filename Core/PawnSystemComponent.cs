using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StudioScor.Utilities;


namespace StudioScor.PlayerSystem
{
    public interface IPawnSystem
    {
        public delegate void ControllerStateHandler(IPawnSystem pawn, IControllerSystem controller);
        public delegate void InputStateHandler(IPawnSystem pawn, bool isIgnoreInput);
        public Transform transform { get; }
        public GameObject gameObject { get; }
        public bool IsPlayer { get; }
        public bool IsPossessed { get; }

        public void SetStartPlayer(bool isPlayer);
        public void OnPossess(IControllerSystem controller);
        public void UnPossess();

        public IControllerSystem Controller { get; }

        public Vector3 MoveDirection { get; }
        public float MoveStrength { get; }
        public Vector3 TurnDirection { get; }
        public Vector3 LookPosition { get; }
        public Transform LookTarget { get; }
        public Vector3 LookDirection { get; }


        public event ControllerStateHandler OnPossessedController;
        public event ControllerStateHandler OnUnPossessedController;

        public event InputStateHandler OnChangedMovementInputState;
        public event InputStateHandler OnChangedRotateInputState;
    }

    [DefaultExecutionOrder(PlayerSystemExecutionOrder.MAIN_ORDER)]
    [AddComponentMenu("StudioScor/PlayerSystem/Pawn System Component", order: 1)]
    public class PawnSystemComponent : BaseMonoBehaviour, IPawnSystem
    {
        [Header(" [ Pawn System ] ")]
        [SerializeField] private PlayerManager _playerManager;

        [Header(" [  Controller ] ")]
        [SerializeField] private GameObject _defaultController;
        [SerializeField][SReadOnlyWhenPlaying] private GameObject _currentController;
        [SerializeField] private bool _isStartPlayer = false;
        [SerializeField] private bool _useAutoPossesed = true;

        [Header(" [ Input ] ")]
        [SerializeField] private bool _ignoreMovementInput = false;
        [SerializeField] private bool _ignoreRotateInput = false;

        public IControllerSystem Controller { get; protected set; }
        public bool IgnoreMovementInput => _ignoreMovementInput;
        public bool IgnoreTunrInput => _ignoreRotateInput;

        public bool IsPlayer => IsPossessed && Controller.IsPlayer;
        public bool IsPossessed => Controller is not null;

        public Vector3 MoveDirection => Controller is null || IgnoreMovementInput ? default : Controller.MoveDirection;
        public float MoveStrength => Controller is null || IgnoreMovementInput ? default : Controller.MoveStrength;
        public Vector3 TurnDirection => Controller is null || IgnoreTunrInput ? default : Controller.GetTurnDirection();
        public Vector3 LookPosition => Controller is null ? default : Controller.GetLookPosition();
        public Transform LookTarget => Controller is null ? null : Controller.LookTarget;
        public Vector3 LookDirection => Controller is null ? default : Controller.GetLookDirection();

        public event IPawnSystem.ControllerStateHandler OnPossessedController;
        public event IPawnSystem.ControllerStateHandler OnUnPossessedController;
        
        public event IPawnSystem.InputStateHandler OnChangedMovementInputState;
        public event IPawnSystem.InputStateHandler OnChangedRotateInputState;


        private void Start()
        {
            if (_isStartPlayer)
                ForceSetPlayerPawn();

            TryAutoPossessed();
        }

        public void ForceSetPlayerPawn()
        {
            _playerManager.ForceSetPlayerPawn(this);

            if (_playerManager.HasPlayerController)
            {
                _playerManager.PlayerController.OnPossess(this);
            }
        }
        public void SetStartPlayer(bool isPlayer)
        { 
            _isStartPlayer = isPlayer;
        }
        public void SetAutoPossess(bool useAutoPossess)
        {
            _useAutoPossesed = useAutoPossess;
        }

        private void TryAutoPossessed()
        {
            Log("Try Auto Possessed ");

            if (_currentController && _currentController.TryGetControllerSystem(out IControllerSystem controllerSystem))
            {
                controllerSystem.OnPossess(this);

                return;
            }

            if(!_isStartPlayer && _useAutoPossesed)
            {
                SpawnAndPossessAiController();
            }
        }   

        public void OnPossess(IControllerSystem possessController)
        {
            if (Controller == possessController)
                return;

            Log("On Possess -" + possessController.gameObject.name);

            if (IsPossessed)
            {
                Controller.UnPossess(this);
            }

            Controller = possessController;

            if (Controller is null)
                return;

            _isStartPlayer = Controller.IsPlayer;
            _currentController = Controller.gameObject;

            Invoke_OnPossessedController();
        }

        public void UnPossess()
        {
            if (!IsPossessed)
                return;

            Log("UnPossess -" + Controller.gameObject.name);

            var prevController = Controller;

            Controller = null;
            _currentController = null;

            _isStartPlayer = false;

            prevController.UnPossess(this);

            Invoke_OnUnPossessedController(prevController);
        }

        protected virtual void SpawnAndPossessAiController()
        {
            Log("Spawn And Possess Ai Controller.");

            if (_defaultController != null)
            {
                var controllerInstance = Instantiate(_defaultController);

                Log($"Spawn [{controllerInstance}] ", SUtility.STRING_COLOR_SUCCESS);

                if (controllerInstance.TryGetControllerSystem(out IControllerSystem newController))
                {
                    newController.OnPossess(this);
                }
                else
                {
                    LogError($"{controllerInstance} is Not Has IControllerSystem");
                }
            }
        }

        #region Setter
        public void SetIgnoreMovementInput(bool useMovementInput)
        {
            if (_ignoreMovementInput == useMovementInput)
            {
                return;
            }

            _ignoreMovementInput = useMovementInput;

            Invoke_OnChangedMovementInputState();
        }
        public void SetIgnoreRotateInput(bool useRotateInput)
        {
            if (_ignoreRotateInput == useRotateInput)
            {
                return;
            }

            _ignoreRotateInput = useRotateInput;

            Invoke_OnChangedRotateInputState();
        }

        #endregion

        #region Inovke
        protected void Invoke_OnPossessedController()
        {
            Log($"{nameof(OnPossessedController)} [" + gameObject.name + "] " + Controller);

            OnPossessedController?.Invoke(this, Controller);
        }
        protected void Invoke_OnUnPossessedController(IControllerSystem prevController)
        {
            Log($"{nameof(OnUnPossessedController)} [" + gameObject.name + "] " + prevController.gameObject);

            OnUnPossessedController?.Invoke(this, prevController);
        }

        protected void Invoke_OnChangedMovementInputState()
        {
            Log($"{nameof(OnChangedMovementInputState)} : " + IgnoreMovementInput);

            OnChangedMovementInputState?.Invoke(this, IgnoreMovementInput);
        }

        protected void Invoke_OnChangedRotateInputState()
        {
            Log($"{nameof(OnChangedRotateInputState)}: " + IgnoreTunrInput);

            OnChangedRotateInputState?.Invoke(this, IgnoreTunrInput);
        }

        #endregion
    }

}

