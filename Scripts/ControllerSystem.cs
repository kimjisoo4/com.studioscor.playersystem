using UnityEngine;
using System.Diagnostics;

namespace KimScor.Pawn
{
    public class ControllerSystem : MonoBehaviour
    {
        #region Events
        public delegate void OnChangedPawnHandler(ControllerSystem controller, PawnSystem pawn);

        public delegate void InputHandler(ControllerSystem controller);

        public delegate void StartMovementInputHandler(ControllerSystem controller, Vector3 direction, float strength);
        public delegate void FinishMovementInputHandler(ControllerSystem controller, Vector3 prevDirection, float prevStrength);

        public delegate void StartRotateInputHander(ControllerSystem controller, Vector3 direction);
        public delegate void FinishRotateInputHander(ControllerSystem controller, Vector3 direction);

        public delegate void LookTargetHandler(ControllerSystem controllerSystem, Transform currentLookTarget, Transform prevLookTarget);
        #endregion

        [Header(" [ Controlled Pawn ] ")]
        [SerializeField] private PawnSystem _Pawn;
        public PawnSystem Pawn => _Pawn;

        [Header(" [ Player Controller] ")]
        [SerializeField] private bool _IsPlayerController = false;
        public bool IsPlayerController => _IsPlayerController;

        [Header(" [ Team ] ")]
        [SerializeField] private EAffiliation _Affiliation = EAffiliation.Hostile;
        public EAffiliation Affiliation => _Affiliation;

        [Header(" [ Use Movement Input ] ")]
        [SerializeField] private bool _UseMovementInput = true;
        public bool UseMovementInput => _UseMovementInput;

        [Header(" [ Use Turn Input ]")]
        [SerializeField] private bool _UseTurnInput = true;
        public bool UseTurnInput => _UseTurnInput;

        [Header(" [ Use Look Input ] ")]
        [SerializeField] private bool _UseLookInput = true;
        public bool UseLookInput => _UseLookInput;

        [Header(" [ Look Target ] ")]
        [SerializeField] private Transform _LookTarget;
        public Transform LookTarget => _LookTarget;

        [Header(" [ Use DebugMode ]")]
        [SerializeField] private bool _UseDebugMode = false;


        private Vector3 _MoveDirection = Vector3.zero;
        public Vector3 MoveDirection => _MoveDirection;

        private float _MoveStrength = 0f;
        public float MoveStrength => _MoveStrength;

        private Vector3 _TurnDirection = Vector3.zero;
        public Vector3 TurnDirection => _TurnDirection;

        private Vector3 _LookDirection = Vector3.zero;
        public Vector3 LookDirection => _LookDirection;


        public event OnChangedPawnHandler OnPossessedPawn;
        public event OnChangedPawnHandler UnPossessedPawn;

        public event InputHandler OnUsedMovementInput;
        public event InputHandler UnUsedMovementInput;

        public event InputHandler OnUsedTurnInput;
        public event InputHandler UnUsedTurnInput;

        public event InputHandler OnUsedLookInput;
        public event InputHandler UnUsedLookInput;

        public event StartMovementInputHandler OnStartedMovementInput;
        public event FinishMovementInputHandler OnFinishedMovementInput;

        public event StartRotateInputHander OnStartedTurntInput;
        public event FinishRotateInputHander OnFinishedTurnInput;

        public event StartRotateInputHander OnStartedLookInput;
        public event FinishRotateInputHander OnFinishedLookInput;

        public event LookTargetHandler OnChangedLookTarget;

        public void OnPossess(PawnSystem pawn)
        {
            if (Pawn == pawn)
            {
                return;
            }

            if (Pawn != null)
            {
                Pawn.UnPossess(this);

                UnPossessPawn();

                _Pawn = null;
            }

            _Pawn = pawn;

            if (Pawn != null)
            {
                Pawn.OnPossess(this);

                OnPossessPawn();
            }
        }

        public void UnPossess(PawnSystem pawn)
        {
            if (_Pawn == pawn)
            {
                Pawn.UnPossess(this);

                _Pawn = null;

                Destroy(gameObject);
            }
        }

        public virtual bool CheckHostile(ControllerSystem targetController)
        {
            switch (Affiliation)
            {
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

            if (UseMovementInput)
                OnUseMovementInput();
            else
                UnUseMovementInput();
        }

        public void SetMovementInput(Vector3 direction, float strength)
        {
            if (!UseMovementInput)
                return;

            Vector3 prevDirection = MoveDirection;
            float prevMoveStrength = MoveStrength;

            _MoveDirection = direction;
            _MoveStrength = strength;

            if (prevDirection == Vector3.zero && direction != Vector3.zero)
            {
                OnStartMovementInput();
            }
            else if (prevDirection != Vector3.zero && direction == Vector3.zero)
            {
                OnFinishMovementInput(prevDirection, prevMoveStrength);
            }
        }

        public void SetUseTurnInput(bool useTurnInput)
        {
            if (_UseTurnInput == useTurnInput)
            {
                return;
            }

            _UseTurnInput = useTurnInput;

            if (UseTurnInput)
                OnUseTurnInput();
            else
                UnUseTurnInput();
        }

        public void SetTurnInput(Vector3 direction)
        {
            if (!UseTurnInput)
                return;

            Vector3 prevDirection = direction;

            _TurnDirection = direction;

            if (prevDirection == Vector3.zero && _TurnDirection != Vector3.zero)
            {
                OnStartTurnInput();
            }
            else if (prevDirection != Vector3.zero && _TurnDirection == Vector3.zero)
            {
                OnFinishTurnInput(prevDirection);
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

            if (UseTurnInput)
                OnUseLookInput();
            else
                UnUseLookInput();
        }
        public void SetLookInput(Vector3 direction)
        {
            if (!UseLookInput)
                return;

            Vector3 prevDirection = direction;

            _LookDirection = direction;

            if (prevDirection == Vector3.zero && _LookDirection != Vector3.zero)
            {
                OnStartLookInput();
            }
            else if (prevDirection != Vector3.zero && _LookDirection == Vector3.zero)
            {
                OnFinishLookInput(prevDirection);
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
            

            OnChangeLookTarget(prevTarget);
        }

        #endregion

        #region EDITOR

        [Conditional("UNITY_EDITOR")]
        protected void Log(string log)
        {
            if (_UseDebugMode)
                UnityEngine.Debug.Log("Controller [" + gameObject.name + "] :" + log);
        }

        private void OnDrawGizmosSelected()
        {
            if (!_UseDebugMode)
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
        }

        #endregion


        #region Callback
        public void OnPossessPawn()
        {
            Log("On Possessed Pawn - " + Pawn.name);

            OnPossessedPawn?.Invoke(this, Pawn);
        }
        public void UnPossessPawn()
        {
            Log("Un Possessed Pawn - " + Pawn.name);

            UnPossessedPawn?.Invoke(this, Pawn);
        }


        public void OnUseMovementInput()
        {
            Log("On Use Movement Input");

            OnUsedMovementInput?.Invoke(this);
        }
        public void UnUseMovementInput()
        {
            Log("Un Use Movement Input");

            UnUsedMovementInput?.Invoke(this);
        }


        public void OnStartMovementInput()
        {
            Log("On Start Movement Input -" + MoveDirection + " * " + MoveStrength);

            OnStartedMovementInput?.Invoke(this, MoveDirection, MoveStrength);
        }
        public void OnFinishMovementInput(Vector3 prevDirection, float prevMoveStrength)
        {
            Log("On Finish Movement Input -" + prevDirection + " * " + prevMoveStrength);

            OnFinishedMovementInput?.Invoke(this, prevDirection, prevMoveStrength);
        }


        public void OnUseTurnInput()
        {
            Log("On Use Turn Input");

            OnUsedTurnInput?.Invoke(this);
        }
        public void UnUseTurnInput()
        {
            Log("Un Use Turn Input");

            UnUsedTurnInput?.Invoke(this);
        }
        public void OnStartTurnInput()
        {
            Log("On Start Turn Input - " + TurnDirection);

            OnStartedTurntInput?.Invoke(this, TurnDirection);
        }
        public void OnFinishTurnInput(Vector3 prevDirection)
        {
            Log("On Finish Turn Input - " + prevDirection);

            OnFinishedTurnInput?.Invoke(this, prevDirection);
        }

        public void OnUseLookInput()
        {
            Log("On Use Look Input");

            OnUsedLookInput?.Invoke(this);
        }
        public void UnUseLookInput()
        {
            Log("Un Use Look Input");

            UnUsedLookInput?.Invoke(this);
        }
        public void OnStartLookInput()
        {
            Log("On Start Look Input - " + TurnDirection);

            OnStartedLookInput?.Invoke(this, TurnDirection);
        }
        public void OnFinishLookInput(Vector3 prevDirection)
        {
            Log("On Finish Look Input - " + prevDirection);

            OnFinishedLookInput?.Invoke(this, prevDirection);
        }

        public void OnChangeLookTarget(Transform prevLookTarget = null)
        {
            Log("On Change Look Target - New Look Target : " + _LookTarget + " Prev Look Target : " + prevLookTarget);

            OnChangedLookTarget?.Invoke(this, _LookTarget, prevLookTarget);
        }
        #endregion
    }

}

