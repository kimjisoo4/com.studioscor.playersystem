using UnityEngine;
using System.Diagnostics;

namespace StudioScor.PlayerSystem
{
    [DefaultExecutionOrder(PlayerSystemExecutionOrder.MAIN_ORDER)]
    [AddComponentMenu("StudioScor/PlayerSystem/Controller Component", order: 0)]
    public class ControllerComponent : MonoBehaviour
    {
        #region Events
        public delegate void OnChangedPawnHandler(ControllerComponent controller, PawnComponent pawn);

        public delegate void InputStateHandler(ControllerComponent controller, bool isUsed);

        public delegate void StartMovementInputHandler(ControllerComponent controller, Vector3 direction, float strength);
        public delegate void FinishMovementInputHandler(ControllerComponent controller, Vector3 prevDirection, float prevStrength);

        public delegate void StartRotateInputHander(ControllerComponent controller, Vector3 direction);
        public delegate void FinishRotateInputHander(ControllerComponent controller, Vector3 direction);

        public delegate void LookTargetHandler(ControllerComponent controllerSystem, Transform currentLookTarget, Transform prevLookTarget);
        #endregion

        [Header(" [ Controller System ] ")]

        [Header(" [ Controlled Pawn ] ")]
        [SerializeField] protected PawnComponent _Pawn;
        public PawnComponent Pawn => _Pawn;

        [Header(" [ Player Controller] ")]
        [SerializeField] protected bool _IsPlayerController = false;
        public bool IsPlayerController => _IsPlayerController;

        [Header(" [ Team ] ")]
        [SerializeField] protected EAffiliation _Affiliation = EAffiliation.Hostile;
        public EAffiliation Affiliation => _Affiliation;

        [Header(" [ Use Movement Input ] ")]
        [SerializeField] protected bool _UseMovementInput = true;
        public bool UseMovementInput => _UseMovementInput;

        [Header(" [ Use Turn Input ]")]
        [SerializeField] protected bool _UseTurnInput = true;
        public bool UseTurnInput => _UseTurnInput;

        [Header(" [ Use Look Input ] ")]
        [SerializeField] protected bool _UseLookInput = true;
        public bool UseLookInput => _UseLookInput;

        [Header(" [ Look Target ] ")]
        [SerializeField] protected Transform _LookTarget;
        public Transform LookTarget => _LookTarget;

        private Vector3 _MoveDirection = Vector3.zero;
        public Vector3 MoveDirection => _MoveDirection;

        private float _MoveStrength = 0f;
        public float MoveStrength => _MoveStrength;

        private Vector3 _TurnDirection = Vector3.zero;
        public Vector3 TurnDirection => _TurnDirection;

        private Vector3 _LookDirection = Vector3.zero;
        public Vector3 LookDirection => _LookDirection;

        public bool IsPossess => Pawn;

        public event OnChangedPawnHandler OnPossessedPawn;
        public event OnChangedPawnHandler OnUnPossessedPawn;

        public event InputStateHandler OnChangedMovementInputState;
        public event InputStateHandler OnChangedRotateInputState;
        public event InputStateHandler OnChangedLookInputState;

        public event StartMovementInputHandler OnStartedMovementInput;
        public event FinishMovementInputHandler OnFinishedMovementInput;

        public event StartRotateInputHander OnStartedRotatetInput;
        public event FinishRotateInputHander OnFinishedRotateInput;

        public event StartRotateInputHander OnStartedLookInput;
        public event FinishRotateInputHander OnFinishedLookInput;

        public event LookTargetHandler OnChangedLookTarget;


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
                UnityEngine.Debug.Log(GetType().Name + " [" + name + "] -" + message, this);
            }
#endif
        }

        private void OnDrawGizmosSelected()
        {
#if UNITY_EDITOR
            if (!_UseDebug)
                return;

            if (_Pawn == null)
                return;

            Vector3 start = _Pawn.transform.position + Vector3.up;
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(start, MoveDirection * 3f);

            Gizmos.color = Color.red;
            Gizmos.DrawRay(start, TurnDirection * 3f);

            Gizmos.color = Color.yellow;
            if (LookTarget == null)
            {
                Gizmos.DrawRay(start, LookDirection * 3f);
            }
            else
            {
                Gizmos.DrawRay(start, GetLookDirection() * 3f);
                Gizmos.DrawWireSphere(LookTarget.position + Vector3.up, 1f);
            }
#endif
        }

        #endregion

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

        public void SetMovementInput(Vector3 direction, float strength)
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

        public void SetTurnInput(Vector3 direction)
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

        public Vector3 GetLookDirection()
        {
            if (Pawn == null)
                return Vector3.zero;

            if (LookTarget != null)
            {
                return (LookTarget.position - Pawn.transform.position).normalized;
            }
            else
            {
                return LookDirection;
            }
        }

        public void SetUseLookInput(bool useLookInput)
        {
            if (_UseLookInput == useLookInput)
            {
                return;
            }

            _UseLookInput = useLookInput;

            Callback_OnChangedLookInputState();
        }
        public void SetLookInput(Vector3 direction)
        {
            if (!UseLookInput)
                return;

            Vector3 prevDirection = direction;

            _LookDirection = direction;

            if (prevDirection == Vector3.zero && _LookDirection != Vector3.zero)
            {
                Callback_OnStartedLookInput();
            }
            else if (prevDirection != Vector3.zero && _LookDirection == Vector3.zero)
            {
                Callback_OnFinishedLookInput(prevDirection);
            }
        }
        public void SetLookInput(Transform target)
        {
            if (!UseLookInput)
                return;

            if (transform == null)
                return;

            if (Pawn == null)
                return;

            Vector3 direction = target.transform.position - Pawn.transform.position;

            direction.Normalize();

            SetLookInput(direction);
        }

        public void SetLookInputToTarget()
        {
            if (LookTarget == null)
            {
                SetLookInput(Vector3.zero);

                return;
            }

            if (!Pawn)
                return;

            Vector3 direction = LookTarget.transform.position - Pawn.transform.position;

            SetLookInput(direction.normalized);
        }

        public void SetLookTarget(Transform newLookTarget)
        {
            if (_LookTarget == newLookTarget)
            {
                return;
            }

            var prevTarget = _LookTarget;

            _LookTarget = newLookTarget;


            if (_LookTarget == null) 
            {
                SetLookInput(prevTarget);
            }
            else
            {
                SetLookInput(_LookTarget);
            }
            

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
        protected void Callback_OnStartedLookInput()
        {
            Log("On Started Look Input - " + TurnDirection);

            OnStartedLookInput?.Invoke(this, TurnDirection);
        }
        protected void Callback_OnFinishedLookInput(Vector3 prevDirection)
        {
            Log("On Finished Look Input - " + prevDirection);

            OnFinishedLookInput?.Invoke(this, prevDirection);
        }

        protected void Callback_OnChangedLookTarget(Transform prevLookTarget = null)
        {
            Log("On Changed Look Target - New Look Target : " + _LookTarget + " Prev Look Target : " + prevLookTarget);

            OnChangedLookTarget?.Invoke(this, _LookTarget, prevLookTarget);
        }
        #endregion
    }

}

