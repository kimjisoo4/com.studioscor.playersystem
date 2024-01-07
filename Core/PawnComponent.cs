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

        public void SetStartPlayer(bool isPlayer);
        public void OnPossess(IControllerSystem controller);
        public void UnPossess();

        public IControllerSystem Controller { get; }

        public Vector3 MoveDirection { get; }
        public float MoveStrength { get; }
        public Vector3 TurnDirection { get; }
        public Vector3 LookPosition { get; }
        public Transform LookTarget { get; }

        public event ControllerStateHandler OnPossessedController;
        public event ControllerStateHandler OnUnPossessedController;

        public event InputStateHandler OnChangedMovementInputState;
        public event InputStateHandler OnChangedRotateInputState;
    }

    [DefaultExecutionOrder(PlayerSystemExecutionOrder.MAIN_ORDER)]
    [AddComponentMenu("StudioScor/PlayerSystem/Pawn Component", order: 1)]
    public class PawnComponent : BaseMonoBehaviour, IPawnSystem
    {
        [Header(" [ Pawn System ] ")]
        [SerializeField] protected PlayerManager playerManager;

        [Header(" [  Controller ] ")]
        [SerializeField] private GameObject defaultController;
        [SerializeField][SReadOnlyWhenPlaying] private GameObject currentController;
        [SerializeField] private bool isStartPlayer = false;
        [SerializeField] private bool useAutoPossesed = true;

        [Header(" [ Input ] ")]
        [SerializeField] private bool ignoreMovementInput = false;
        [SerializeField] private bool ignoreRotateInput = false;



        public IControllerSystem Controller { get; protected set; }
        public bool IgnoreMovementInput => ignoreMovementInput;
        public bool IgnoreTunrInput => ignoreRotateInput;

        public bool IsPlayer => IsPossessed && Controller.IsPlayer;
        public bool IsPossessed => Controller is not null;

        public Vector3 MoveDirection => Controller is null || IgnoreMovementInput ? default : Controller.MoveDirection;
        public float MoveStrength => Controller is null || IgnoreMovementInput ? default : Controller.MoveStrength;
        public Vector3 TurnDirection => Controller is null || IgnoreTunrInput ? default : Controller.TurnDirection;
        public Vector3 LookPosition => Controller is null ? default : Controller.GetLookPosition();
        public Transform LookTarget => Controller is null ? null : Controller.LookTarget;

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

            if (currentController && currentController.TryGetControllerSystem(out IControllerSystem controllerSystem))
            {
                controllerSystem.OnPossess(this);

                return;
            }

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
                Controller.UnPossess(this);
            }

            Controller = possessController;

            if (Controller is null)
                return;

            isStartPlayer = Controller.IsPlayer;
            currentController = Controller.gameObject;

            Callback_OnPossessedController();
        }

        public void UnPossess()
        {
            if (!IsPossessed)
                return;

            Log("UnPossess -" + Controller.gameObject.name);

            var prevController = Controller;

            Controller = null;
            currentController = null;

            isStartPlayer = false;

            prevController.UnPossess(this);

            Callback_OnUnPossessedController(prevController);
        }

        protected virtual void SpawnAndPossessAiController()
        {
            Log("Spawn And Possess Ai Controller.");

            if (defaultController != null)
            {
                var controllerInstance = Instantiate(defaultController);
                Log($"Spawn [{controllerInstance}] ", SUtility.NAME_COLOR_GREEN);

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

        #region Callback
        protected void Callback_OnPossessedController()
        {
            Log("On Possessed Controller [" + gameObject.name + "] " + Controller);

            OnPossessedController?.Invoke(this, Controller);
        }
        protected void Callback_OnUnPossessedController(IControllerSystem prevController)
        {
            Log("On UnPossessed Controller [" + gameObject.name + "] " + prevController.gameObject);

            OnUnPossessedController?.Invoke(this, prevController);
        }

        protected void Callback_OnChangedMovementInputState()
        {
            Log("On Changed Ignore Movement Input : " + IgnoreMovementInput);

            OnChangedMovementInputState?.Invoke(this, IgnoreMovementInput);
        }

        protected void Callback_OnChangedRotateInputState()
        {
            Log("On Change Ignore Rotate Input : " + IgnoreTunrInput);

            OnChangedRotateInputState?.Invoke(this, IgnoreTunrInput);
        }

        #endregion
    }

}

