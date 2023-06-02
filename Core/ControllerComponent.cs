using UnityEngine;
using StudioScor.Utilities;

using System.Diagnostics;

namespace StudioScor.PlayerSystem
{
    public delegate void ChangePawnEventHandler(IControllerEvent controller, PawnComponent pawn);

    public delegate void InputStateEventHandler(IControllerEvent controller, bool isUsed);

    public delegate void MoveDirectionEventHandler(IControllerEvent controller, Vector3 direction, float strength);
    public delegate void TurnDirectionEventHandler(IControllerEvent controller, Vector3 direction);

    public delegate void LookPositionEventHandler(IControllerEvent controller, Vector3 position, Vector3 prevPosition);
    public delegate void LookTargetEventHandler(IControllerEvent controllerSystem, Transform currentLookTarget, Transform prevLookTarget);


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
        #region Get Controller Event
        public static IControllerEvent GetControllerEvent(this GameObject gameObject)
        {
            return gameObject.GetComponent<IControllerEvent>();
        }
        public static IControllerEvent GetControllerEvent(this Component component)
        {
            var controller = component as IControllerEvent;

            if (controller is not null)
                return controller;

            return component.gameObject.GetComponent<IControllerEvent>();
        }
        public static bool TryGetControllerEvent(this GameObject gameObject, out IControllerEvent controllerEvent)
        {
            return gameObject.TryGetComponent(out controllerEvent);
        }
        public static bool TryGetControllerEvent(this Component component, out IControllerEvent controllerEvent)
        {
            controllerEvent = component as IControllerEvent;

            if (controllerEvent is not null)
                return true;

            return component.TryGetComponent(out controllerEvent);
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

        public PawnComponent Pawn { get; }
        public bool IsPlayer { get; }
        public bool IsPossess { get; }
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
    public interface IControllerEvent
    {
        public event ChangePawnEventHandler OnPossessedPawn;
        public event ChangePawnEventHandler OnUnPossessedPawn;

        public event InputStateEventHandler OnChangedMovementInputState;
        public event InputStateEventHandler OnChangedRotateInputState;
        public event InputStateEventHandler OnChangedLookInputState;

        public event MoveDirectionEventHandler OnStartedMovementInput;
        public event MoveDirectionEventHandler OnFinishedMovementInput;

        public event TurnDirectionEventHandler OnStartedRotatetInput;
        public event TurnDirectionEventHandler OnFinishedRotateInput;

        public event LookPositionEventHandler OnChangedLookPosition;
        public event LookTargetEventHandler OnChangedLookTarget;
    }

    [DefaultExecutionOrder(PlayerSystemExecutionOrder.MAIN_ORDER)]
    [AddComponentMenu("StudioScor/PlayerSystem/Controller Component", order: 0)]
    public class ControllerComponent : BaseMonoBehaviour, IControllerSystem, IControllerInput, IControllerEvent
    {
        [Header(" [ Controller System ] ")]
        [SerializeField] protected PlayerManager _PlayerManager;
        [SerializeField, SReadOnlyWhenPlaying] protected bool _IsPlayer = false;

        [Header(" [ Team ] ")]
        [SerializeField] protected EAffiliation _Affiliation = EAffiliation.Hostile;
        
        protected PawnComponent _Pawn;

        [Header(" [ Input State ] ")]
        [SerializeField] protected bool _UseMovementInput = true;
        [SerializeField] protected bool _UseTurnInput = true;
        [SerializeField] protected bool _UseLookInput = true;

        [Header(" [ Look Target ] ")]
        [SerializeField] protected Transform _LookTarget;

        private float _MoveStrength = 0f;
        private Vector3 _MoveDirection = Vector3.zero;
        private Vector3 _TurnDirection = Vector3.zero;
        private Vector3 _LookPosition = Vector3.zero;

        public PawnComponent Pawn => _Pawn;
        public bool IsPlayer => _IsPlayer;
        public bool IsPossess => Pawn;
        public EAffiliation Affiliation => _Affiliation;
        public bool UseMovementInput => _UseMovementInput;
        public bool UseTurnInput => _UseTurnInput;
        public bool UseLookInput => _UseLookInput;
        public Vector3 MoveDirection => _MoveDirection;
        public float MoveStrength => _MoveStrength;
        public Vector3 TurnDirection => _TurnDirection;

        public Transform LookTarget => _LookTarget;
        public Vector3 LookPosition => _LookPosition;

        


        public event ChangePawnEventHandler OnPossessedPawn;
        public event ChangePawnEventHandler OnUnPossessedPawn;

        public event InputStateEventHandler OnChangedMovementInputState;
        public event InputStateEventHandler OnChangedRotateInputState;
        public event InputStateEventHandler OnChangedLookInputState;

        public event MoveDirectionEventHandler OnStartedMovementInput;
        public event MoveDirectionEventHandler OnFinishedMovementInput;

        public event TurnDirectionEventHandler OnStartedRotatetInput;
        public event TurnDirectionEventHandler OnFinishedRotateInput;

        public event LookPositionEventHandler OnChangedLookPosition;
        public event LookTargetEventHandler OnChangedLookTarget;

        #region Editor Only

        private void OnDrawGizmosSelected()
        {
#if UNITY_EDITOR
            if (!UseDebug)
                return;

            if (_Pawn == null)
                return;

            Vector3 start = _Pawn.transform.position + Vector3.up;
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(start, MoveDirection * 3f);

            Gizmos.color = Color.red;
            Gizmos.DrawRay(start, TurnDirection * 3f);

            Gizmos.color = Color.yellow;

            Vector3 position = GetLookPosition();

            if(position != default)
            {
                Gizmos.DrawLine(start, position);
                Gizmos.DrawWireSphere(position, 1f);
            }
#endif
        }

        #endregion

        private void Start()
        {
            if (IsPlayer)
                ForceSetPlayerController();
        }

        public void ForceSetPlayerController()
        {
            _PlayerManager.ForceSetPlayerController(this);

            if (_PlayerManager.HasPlayerPawn)
            {
                OnPossess(_PlayerManager.PlayerPawn);
            }
        }

        public void OnPossess(PawnComponent pawn)
        {
            if (_Pawn == pawn)
                return;

            UnPossess(_Pawn);

            _Pawn = pawn;

            if (!_Pawn)
                return;

            _Pawn.OnPossess(this);

            Callback_OnPossessedPawn();
        }
        public void UnPossess(PawnComponent pawn)
        {
            if (!Pawn)
                return;

            if (_Pawn != pawn)
                return;

            var prevPawn = _Pawn;

            _Pawn = null;

            prevPawn.UnPossess();

            Callback_OnUnPossessedPawn(prevPawn);
        }
        
        public virtual EAffiliation CheckAffiliation(ControllerComponent targetController)
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

        public virtual bool CheckHostile(ControllerComponent targetController)
        {
            switch (Affiliation)
            {
                case EAffiliation.Neutral:
                    return false;
                case EAffiliation.Friendly:
                    return targetController.GetHostile();
                case EAffiliation.Hostile:
                    return targetController.GetFriendly();
                default:
                    return false;
            }
        }

        public virtual bool GetHostile() => _Affiliation.Equals(EAffiliation.Hostile);
        public virtual bool GetNeutral() => _Affiliation.Equals(EAffiliation.Neutral);
        public virtual bool GetFriendly() =>_Affiliation.Equals(EAffiliation.Friendly);


        
        public void SetUseMovementInput(bool useMovementInput)
        {
            if (_UseMovementInput == useMovementInput)
            {
                return;
            }

            _UseMovementInput = useMovementInput;

            Callback_OnChangedMovementInputState();
        }

        public void SetMoveDirection(Vector3 direction, float strength)
        {
            if (!UseMovementInput)
                return;

            Vector3 prevDirection = MoveDirection;
            float prevMoveStrength = MoveStrength;

            _MoveDirection = direction;
            _MoveStrength = Mathf.Clamp01(strength);

            if (prevDirection == Vector3.zero && direction != Vector3.zero)
            {
                Callback_OnStartedMovementInput();
            }
            else if (prevDirection != Vector3.zero && direction == Vector3.zero)
            {
                Callback_OnFinishedMovementInput(prevDirection, prevMoveStrength);
            }
        }

        public void SetUseTurnInput(bool useTurnInput)
        {
            if (_UseTurnInput == useTurnInput)
            {
                return;
            }

            _UseTurnInput = useTurnInput;

            Callback_OnChangedTurnInputState();
        }

        public void SetTurnDirection(Vector3 direction)
        {
            if (!UseTurnInput)
                return;

            Vector3 prevDirection = direction;

            _TurnDirection = direction;

            if (prevDirection == Vector3.zero && _TurnDirection != Vector3.zero)
            {
                Callback_OnStartedTurnInput();
            }
            else if (prevDirection != Vector3.zero && _TurnDirection == Vector3.zero)
            {
                Callback_OnFinishedTurnInput(prevDirection);
            }
        }


        #region Look
        public Vector3 GetLookPosition()
        {
            if (!_UseLookInput)
                return default;

            return LookTarget ? LookTarget.position : _LookPosition;
        }

        public void SetUseLookInput(bool useLookInput)
        {
            _UseLookInput = useLookInput;

            Callback_OnChangedLookInputState();
        }
        public void SetLookPosition(Vector3 position)
        {
            var prevPosition = _LookPosition;
            _LookPosition = position;

            Callback_OnChangedLookPosition(prevPosition);
        }
        public void SetLookTarget(Transform newLookTarget)
        {
            if (_LookTarget == newLookTarget)
                return;

            var prevTarget = _LookTarget;
            _LookTarget = newLookTarget;

            Callback_OnChangedLookTarget(prevTarget);
        }

        #endregion


        #region Callback
        protected void Callback_OnPossessedPawn()
        {
            Log("On Possessed Pawn - " + Pawn.name);

            OnPossessedPawn?.Invoke(this, Pawn);
        }
        protected void Callback_OnUnPossessedPawn(PawnComponent prevPawn)
        {
            Log("On UnPossessed Pawn - " + prevPawn.name);

            OnUnPossessedPawn?.Invoke(this, prevPawn);
        }


        protected void Callback_OnChangedMovementInputState()
        {
            Log("On Used Movement Input");

            OnChangedMovementInputState?.Invoke(this, _UseMovementInput);
        }

        protected void Callback_OnStartedMovementInput()
        {
            Log("On Started Movement Input -" + MoveDirection + " * " + MoveStrength);

            OnStartedMovementInput?.Invoke(this, MoveDirection, MoveStrength);
        }
        protected void Callback_OnFinishedMovementInput(Vector3 prevDirection, float prevMoveStrength)
        {
            Log("On Finished Movement Input -" + prevDirection + " * " + prevMoveStrength);

            OnFinishedMovementInput?.Invoke(this, prevDirection, prevMoveStrength);
        }


        protected void Callback_OnChangedTurnInputState()
        {
            Log("On Used Turn Input");

            OnChangedRotateInputState?.Invoke(this, _UseTurnInput);
        }
        protected void Callback_OnStartedTurnInput()
        {
            Log("On Started Turn Input - " + TurnDirection);

            OnStartedRotatetInput?.Invoke(this, TurnDirection);
        }
        protected void Callback_OnFinishedTurnInput(Vector3 prevDirection)
        {
            Log("On Finished Turn Input - " + prevDirection);

            OnFinishedRotateInput?.Invoke(this, prevDirection);
        }

        protected void Callback_OnChangedLookInputState()
        {
            Log("On Used Look Input");

            OnChangedLookInputState?.Invoke(this, _UseLookInput);
        }
        protected void Callback_OnChangedLookPosition(Vector3 prevPosition)
        {
            Log("On Changed LookPosition - " + _LookPosition);

            OnChangedLookPosition?.Invoke(this, _LookPosition, prevPosition);
        }
        protected void Callback_OnChangedLookTarget(Transform prevLookTarget = null)
        {
            Log("On Changed Look Target - New Look Target : " + _LookTarget + " Prev Look Target : " + prevLookTarget);

            OnChangedLookTarget?.Invoke(this, _LookTarget, prevLookTarget);
        }
        #endregion
    }

}

