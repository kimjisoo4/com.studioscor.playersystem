using UnityEngine;
using StudioScor.Utilities;

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
        #region Get Controller
        public static IControllerSystem GetController(this GameObject gameObject)
        {
            var controllerSystem = gameObject.GetControllerSystem();

            if (controllerSystem is null)
            {
                var pawnSystem = gameObject.GetPawnSystem();

                controllerSystem = pawnSystem.Controller;
            }

            return controllerSystem;
        }
        public static IControllerSystem GetController(this Component component)
        {
            return component.gameObject.GetController();
        }
        #endregion

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

    }

    public enum ETurnDirectionState
    {
        Direction,
        MoveDirection,
        LookDirection,
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

        public ETurnDirectionState TurnDirectionState { get; }

        public void OnPossess(IPawnSystem pawn);
        public void UnPossess(IPawnSystem pawn);


        public void SetMoveDirection(Vector3 direction, float strength);
        public void SetTurnDirection(Vector3 direction);
        public void SetLookPosition(Vector3 position);
        public void SetLookTarget(Transform target);

        public void SetTurnDirectionState(ETurnDirectionState newDirectionState);

        public Vector3 GetLookPosition();
        public Vector3 GetTurnDirection();
        public Vector3 GetLookDirection();

        public Vector3 LookPosition { get; }
        public Transform LookTarget { get; }
        public Vector3 TurnDirection { get; }
        public Vector3 MoveDirection { get; }
        public float MoveStrength { get; }

        public event ChangePawnEventHandler OnPossessedPawn;
        public event ChangePawnEventHandler OnUnPossessedPawn;
        public event LookTargetEventHandler OnChangedLookTarget;
    }

    [DefaultExecutionOrder(PlayerSystemExecutionOrder.MAIN_ORDER)]
    [AddComponentMenu("StudioScor/PlayerSystem/Controller Component", order: 0)]
    public class ControllerComponent : BaseMonoBehaviour, IControllerSystem
    {
        [Header(" [ Controller System ] ")]
        [SerializeField] protected PlayerManager playerManager;
        [field: SerializeField][field: SReadOnlyWhenPlaying] public bool IsPlayer { get; protected set; } = false;
        [field: SerializeField] public ETurnDirectionState TurnDirectionState { get; protected set; }

        [field : Header(" [ Team ] ")]
        [field: SerializeField] public EAffiliation Affiliation { get; protected set; } = EAffiliation.Hostile;
        
        public bool IsPossess => Pawn is not null;
        public IPawnSystem Pawn { get; private set; }

        public Vector3 MoveDirection { get; private set; }
        public float MoveStrength { get; private set; }


        public Vector3 TurnDirection { get; private set; }

        public Vector3 LookPosition { get; private set; }
        public Transform LookTarget { get; private set; }
        public Vector3 GetLookDirection()
        {
            return IsPossess? Pawn.transform.Direction(GetLookPosition()) : default;
        }
        
        public Vector3 GetTurnDirection()
        {
            switch (TurnDirectionState)
            {
                case ETurnDirectionState.Direction:
                    return TurnDirection;
                case ETurnDirectionState.MoveDirection:
                    return MoveDirection;
                case ETurnDirectionState.LookDirection:
                    return GetLookDirection();
                default:
                    return default;
            }
        }

        public event ChangePawnEventHandler OnPossessedPawn;
        public event ChangePawnEventHandler OnUnPossessedPawn;
        public event LookTargetEventHandler OnChangedLookTarget;

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
            if (Pawn == possessPawn)
                return;

            UnPossess(Pawn);

            Pawn = possessPawn;

            if (Pawn is null)
                return;

            Pawn.OnPossess(this);

            Callback_OnPossessedPawn();
        }
        public void UnPossess(IPawnSystem unPossessPawn)
        {
            if (!IsPossess)
                return;

            if (Pawn != unPossessPawn)
                return;

            var prevPawn = Pawn;

            Pawn = null;

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

        public void SetMoveDirection(Vector3 direction, float strength)
        {
            MoveDirection = direction;
            MoveStrength = strength;
        }

        public void SetTurnDirection(Vector3 direction)
        {
            TurnDirection = direction;    
        }

        public void SetLookPosition(Vector3 position)
        {
            LookPosition = position;
        }

        public void SetLookTarget(Transform target)
        {
            if (LookTarget == target)
                return;

            var prevTarget = LookTarget;
            LookTarget = target;

            Callback_OnChangedLookTarget(prevTarget);
        }

        public Vector3 GetLookPosition()
        {
            return LookTarget ? LookTarget.position : LookPosition;
        }

        public void SetTurnDirectionState(ETurnDirectionState newDirectionState)
        {
            if (TurnDirectionState == newDirectionState)
                return;

            var prevDirectionState = TurnDirectionState;
            TurnDirectionState = newDirectionState;
        }


        #region Callback
        protected virtual void Callback_OnPossessedPawn()
        {
            Log("On Possessed Pawn - " + Pawn.gameObject.name);

            OnPossessedPawn?.Invoke(this, Pawn);
        }
        protected virtual void Callback_OnUnPossessedPawn(IPawnSystem prevPawn)
        {
            Log("On UnPossessed Pawn - " + prevPawn.gameObject.name);

            OnUnPossessedPawn?.Invoke(this, prevPawn);
        }

        protected virtual void Callback_OnChangedLookTarget(Transform prevTarget)
        {
            Log($"On Changed Look Target - Current : {(LookTarget ? LookTarget.name : "Null" )} || Prev : {(prevTarget ? prevTarget.name : "Null")}");

            OnChangedLookTarget?.Invoke(this, LookTarget, prevTarget);
        }


        #endregion
    }

}

