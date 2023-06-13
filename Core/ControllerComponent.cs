using UnityEngine;
using StudioScor.Utilities;

using System.Diagnostics;

namespace StudioScor.PlayerSystem
{
    public delegate void ChangePawnEventHandler(IControllerSystem controller, IPawnSystem pawn);

    public delegate void InputStateEventHandler(IControllerSystem controller, bool isUsed);

    public delegate void MoveDirectionEventHandler(IControllerSystem controller, Vector3 direction, float strength);
    public delegate void TurnDirectionEventHandler(IControllerSystem controller, Vector3 direction);

    public delegate void LookPositionEventHandler(IControllerSystem controller, Vector3 position, Vector3 prevPosition);
    public delegate void LookTargetEventHandler(IControllerSystem controllerSystem, Transform currentLookTarget, Transform prevLookTarget);


    public static class ControllerSystemUtility
    {
        #region Get Controller System
        public static IControllerSystem GetControllerSystem(this GameObject gameObject)
        {
            return gameObject.GetComponent<IControllerSystem>();
        }
        public static IControllerSystem GetControllerSystem(this Component component)
        {
            var controller = component as IControllerSystem;

            if (controller is not null)
                return controller;

            return component.gameObject.GetComponent<IControllerSystem>();
        }
        public static bool TryGetControllerSystem(this GameObject gameObject, out IControllerSystem controllerSystem)
        {
            return gameObject.TryGetComponent(out controllerSystem);
        }
        public static bool TryGetControllerSystem(this Component component, out IControllerSystem controllerSystem)
        {
            controllerSystem = component as IControllerSystem;

            if (controllerSystem is not null)
                return true;

            return component.TryGetComponent(out controllerSystem);
        }
        #endregion
        #region Get Controller Input
        public static IControllerInput GetControllerInput(this GameObject gameObject)
        {
            return gameObject.GetComponent<IControllerInput>();
        }
        public static IControllerInput GetControllerInput(this Component component)
        {
            var controller = component as IControllerInput;

            if (controller is not null)
                return controller;

            return component.gameObject.GetComponent<IControllerInput>();
        }
        public static bool TryGetControllerInput(this GameObject gameObject, out IControllerInput controllerInput)
        {
            return gameObject.TryGetComponent(out controllerInput);
        }
        public static bool TryGetControllerInput(this Component component, out IControllerInput controllerInput)
        {
            controllerInput = component as IControllerInput;

            if (controllerInput is not null)
                return true;

            return component.TryGetComponent(out controllerInput);
        }
        #endregion

        #region Get Pawn System
        public static IPawnSystem GetPawnSystem(this GameObject gameObject)
        {
            return gameObject.GetComponent<IPawnSystem>();
        }
        public static IPawnSystem GetPawnSystem(this Component component)
        {
            var pawnSystem = component as IPawnSystem;

            if (pawnSystem is not null)
                return pawnSystem;

            return component.GetComponent<IPawnSystem>();
        }
        public static bool TryGetPawnSystem(this GameObject gameObject, out IPawnSystem pawnSystem)
        {
            return gameObject.TryGetComponent(out pawnSystem);
        }
        public static bool TryGetPawnSystem(this Component component, out IPawnSystem pawnSystem)
        {
            pawnSystem = component as IPawnSystem;

            if (pawnSystem is not null)
                return true;

            return component.TryGetComponent(out pawnSystem);
        }
        #endregion
        #region Get Pawn Event
        public static IPawnEvent GetPawnEvent(this GameObject gameObject)
        {
            return gameObject.GetComponent<IPawnEvent>();
        }
        public static IPawnEvent GetPawnEvent(this Component component)
        {
            var pawnSystem = component as IPawnEvent;

            if (pawnSystem is not null)
                return pawnSystem;

            return component.GetComponent<IPawnEvent>();
        }
        public static bool TryGetPawnEvent(this GameObject gameObject, out IPawnEvent pawnEvent)
        {
            return gameObject.TryGetComponent(out pawnEvent);
        }
        public static bool TryGetPawnEvent(this Component component, out IPawnEvent pawnEvent)
        {
            pawnEvent = component as IPawnEvent;

            if (pawnEvent is not null)
                return true;

            return component.TryGetComponent(out pawnEvent);
        }
        #endregion

    }
    public interface IControllerSystem
    {
        public GameObject gameObject { get; }
        public Transform transform { get; }

        public IPawnSystem Pawn { get; }
        public bool IsPlayer { get; }
        public bool IsPossess { get; }

        public EAffiliation Affiliation { get; }
        public EAffiliation CheckAffiliation(IControllerSystem target);

        public void OnPossess(IPawnSystem pawn);
        public void UnPossess(IPawnSystem pawn);

        public event ChangePawnEventHandler OnPossessedPawn;
        public event ChangePawnEventHandler OnUnPossessedPawn;
    }

    public interface IControllerInput
    {
        public void SetMoveDirection(Vector3 direction, float strength);
        public void SetTurnDirection(Vector3 direction);
        public void SetLookPosition(Vector3 position);
        public void SetLookTarget(Transform target);

        public Vector3 GetLookPosition();
        public Vector3 LookPosition { get; }
        public Transform LookTarget { get; }
        public Vector3 MoveDirection { get; }
    }

    [DefaultExecutionOrder(PlayerSystemExecutionOrder.MAIN_ORDER)]
    [AddComponentMenu("StudioScor/PlayerSystem/Controller Component", order: 0)]
    public class ControllerComponent : BaseMonoBehaviour, IControllerSystem
    {
        [Header(" [ Controller System ] ")]
        [SerializeField] protected PlayerManager playerManager;
        [SerializeField, SReadOnlyWhenPlaying] protected bool isPlayer = false;

        [Header(" [ Team ] ")]
        [SerializeField] protected EAffiliation affiliation = EAffiliation.Hostile;
        
        private IPawnSystem controlPawn;
        public IPawnSystem Pawn => controlPawn;
        public bool IsPlayer => isPlayer;
        public bool IsPossess => Pawn is not null;
        public EAffiliation Affiliation => affiliation;

        public event ChangePawnEventHandler OnPossessedPawn;
        public event ChangePawnEventHandler OnUnPossessedPawn;


        private void Start()
        {
            if (IsPlayer)
                ForceSetPlayerController();
        }

        public void ForceSetPlayerController()
        {
            playerManager.ForceSetPlayerController(this);

            if (playerManager.HasPlayerPawn)
            {
                OnPossess(playerManager.PlayerPawn);
            }
        }

        public void OnPossess(IPawnSystem possessPawn)
        {
            if (controlPawn == possessPawn)
                return;

            UnPossess(controlPawn);

            controlPawn = possessPawn;

            if (controlPawn is null)
                return;

            controlPawn.OnPossess(this);

            Callback_OnPossessedPawn();
        }
        public void UnPossess(IPawnSystem unPossessPawn)
        {
            if (!IsPossess)
                return;

            if (controlPawn != unPossessPawn)
                return;

            var prevPawn = controlPawn;

            controlPawn = null;

            prevPawn.UnPossess();

            Callback_OnUnPossessedPawn(prevPawn);
        }
        
        public virtual EAffiliation CheckAffiliation(IControllerSystem targetController)
        {
            if (Affiliation == EAffiliation.Neutral || targetController.Affiliation == EAffiliation.Neutral)
                return EAffiliation.Neutral;

            if (Affiliation == targetController.Affiliation)
            {
                return EAffiliation.Friendly;
            }
            else
            {
                return EAffiliation.Hostile;
            }
        }


        #region Callback
        protected void Callback_OnPossessedPawn()
        {
            Log("On Possessed Pawn - " + Pawn.gameObject.name);

            OnPossessedPawn?.Invoke(this, Pawn);
        }
        protected void Callback_OnUnPossessedPawn(IPawnSystem prevPawn)
        {
            Log("On UnPossessed Pawn - " + prevPawn.gameObject.name);

            OnUnPossessedPawn?.Invoke(this, prevPawn);
        }

        #endregion
    }

}

