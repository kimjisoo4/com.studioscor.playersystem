using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StudioScor.Utilities;


namespace StudioScor.PlayerSystem
{
    public delegate void ControllerStateHandler(IPawnSystem pawn, IControllerSystem controller);
    public delegate void InputStateHandler(IPawnSystem pawn, bool isIgnoreInput);
    public interface IPawnSystem
    {
        public Transform transform { get; }
        public GameObject gameObject { get; }
        public bool IsPlayer { get; }
        public bool IsPossessed { get; }

        public void OnPossess(IControllerSystem controller);
        public void UnPossess();

        public void SetMoveDirection(Vector3 direction, float strength = 0f);
        public void SetTurnDirection(Vector3 turnDirection);

        public IControllerSystem Controller { get; }

        public Vector3 MoveDirection { get; }
        public float MoveStrength { get; }
        public Vector3 TurnDirection { get; }
    }
    public interface IPawnEvent
    {
        public event ControllerStateHandler OnPossessedController;
        public event ControllerStateHandler OnUnPossessedController;

        public event InputStateHandler OnChangedMovementInputState;
        public event InputStateHandler OnChangedRotateInputState;
    }

    [DefaultExecutionOrder(PlayerSystemExecutionOrder.MAIN_ORDER)]
    [AddComponentMenu("StudioScor/PlayerSystem/Pawn Component", order: 1)]
    public class PawnComponent : BaseMonoBehaviour, IPawnSystem, IPawnEvent
    {
        [Header(" [ Pawn System ] ")]
        [SerializeField] protected PlayerManager playerManager;

        [Header(" [  Controller ] ")]
        [SerializeField] private bool isStartPlayer = false;
        [SerializeField] private GameObject defaultController;
        [SerializeField] private bool useAutoPossesed = true;

        [Header(" [ Input ] ")]
        [SerializeField] private bool ignoreMovementInput = false;
        [SerializeField] private bool ignoreRotateInput = false;


        private IControllerSystem controller;
        private Vector3 moveDirection;
        private float moveStrength;
        
        private Vector3 turnDirection;

        public GameObject DefaultController => defaultController;
        public IControllerSystem Controller => controller;

        public bool IgnoreMovementInput => ignoreMovementInput;
        public bool IgnoreRotateInput => ignoreRotateInput;

        public bool IsPlayer => IsPossessed && Controller.IsPlayer;
        public bool IsPossessed => controller is not null;
        
        public event ControllerStateHandler OnPossessedController;
        public event ControllerStateHandler OnUnPossessedController;

        public event InputStateHandler OnChangedMovementInputState;
        public event InputStateHandler OnChangedRotateInputState;


        private void Start()
        {
            if (isStartPlayer)
                ForceSetPlayerPawn();

            TryAutoPossessed();
        }

        public void ForceSetPlayerPawn()
        {
            playerManager.ForceSetPlayerPawn(this);

            if (playerManager.HasPlayerController)
            {
                playerManager.PlayerController.OnPossess(this);
            }
        }
        public void SetStartPlayer(bool isPlayer)
        { 
            isStartPlayer = isPlayer;
        }
        public void SetAutoPossess(bool useAutoPossess)
        {
            useAutoPossesed = useAutoPossess;
        }

        private void TryAutoPossessed()
        {
            Log("Try Auto Possessed ");
/*
            if (currentController != null && currentController.TryGetControllerSystem(out controller))
            {
                controller.OnPossess(this);

                return;
            }
*/

            if(!isStartPlayer && useAutoPossesed)
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
                controller.UnPossess(this);
            }

            controller = possessController;

            if (Controller is null)
                return;

            isStartPlayer = Controller.IsPlayer;

            Callback_OnPossessedController();
        }

        public void UnPossess()
        {
            if (!IsPossessed)
                return;

            Log("UnPossess -" + controller.gameObject.name);

            var prevController = controller;

            controller = null;

            isStartPlayer = false;

            Callback_OnUnPossessedController(prevController);
        }

        protected virtual void SpawnAndPossessAiController()
        {
            Log("Spawn And Possess Ai Controller.");

            if (defaultController != null)
            {
                var controllerInstance = Instantiate(defaultController);

                if (controllerInstance.TryGetControllerSystem(out controller))
                {
                    controller.OnPossess(this);
                }
                else
                {
                    Log($"{controllerInstance} is Not Has IControllerSystem", true);
                }
            }
        }

        #region Setter
        public void SetMoveDirection(Vector3 newDirection, float newMoveStrength = 0f)
        {
            moveDirection = newDirection;
            moveStrength = newMoveStrength;
        }
        public void SetTurnDirection(Vector3 newTurnDirection) 
        {
            turnDirection = newTurnDirection;
        }

        public void SetIgnoreMovementInput(bool useMovementInput)
        {
            if (ignoreMovementInput == useMovementInput)
            {
                return;
            }

            ignoreMovementInput = useMovementInput;

            Callback_OnChangedMovementInputState();
        }
        public void SetIgnoreRotateInput(bool useRotateInput)
        {
            if (ignoreRotateInput == useRotateInput)
            {
                return;
            }

            ignoreRotateInput = useRotateInput;

            Callback_OnChangedRotateInputState();
        }

        #endregion

        #region Getter
        public Vector3 MoveDirection
        {
            get
            {
                if (IgnoreMovementInput)
                    return default;

                return moveDirection;
            }
        }
        public float MoveStrength
        {
            get
            {
                if (IgnoreMovementInput)
                    return 0f;

                return moveStrength;
            }
        }
        public Vector3 TurnDirection
        {
            get
            {
                if (IgnoreRotateInput)
                    return default;

                return turnDirection;
            }
        }

        #endregion

        #region Callback
        protected void Callback_OnPossessedController()
        {
            Log("On Possessed Controller [" + gameObject.name + "] " + Controller);

            OnPossessedController?.Invoke(this, Controller);
        }
        protected void Callback_OnUnPossessedController(IControllerSystem prevController)
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

