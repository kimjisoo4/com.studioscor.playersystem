using UnityEngine;

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

        [Header(" [ Use Rotate Input ]")]
        [SerializeField] private bool _UseRotateInput = true;
        public bool UseRotateInput => _UseRotateInput;

        [Header(" [ Look Target ] ")]
        [SerializeField] private Transform _LookTarget;
        public Transform LookTarget => _LookTarget;

        [Header(" [ Use DebugMode ]")]
        [SerializeField] private bool _UseDebugMode = false;


        private Vector3 _MoveDirection = Vector3.zero;
        public Vector3 MoveDirection => _MoveDirection;

        private float _MoveStrength = 0f;
        public float MoveStrength => _MoveStrength;

        private Vector3 _RotateDirection = Vector3.zero;
        public Vector3 RotateDirection => _RotateDirection;


        public event OnChangedPawnHandler OnPossessedPawn;
        public event OnChangedPawnHandler UnPossessedPawn;

        public event InputHandler OnUsedMovementInput;
        public event InputHandler UnUsedMovementInput;

        public event InputHandler OnUsedRotateInput;
        public event InputHandler UnUsedRotateInput;

        public event StartMovementInputHandler OnStartedMovementInput;
        public event FinishMovementInputHandler OnFinishedMovementInput;

        public event StartRotateInputHander OnStartedRotatetInput;
        public event FinishRotateInputHander OnFinishedRotateInput;

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

                ResetPossessPawn();

                UnPossessPawn();

                _Pawn = null;
            }

            _Pawn = pawn;

            if (Pawn != null)
            {
                Pawn.OnPossess(this);

                SetupPossessPawn();

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

        private void SetupPossessPawn()
        {
            Pawn.OnDeadPawn += Pawn_OnDeadPawn;
        }
        private void ResetPossessPawn()
        {
            Pawn.OnDeadPawn -= Pawn_OnDeadPawn;
        }

        private void Pawn_OnDeadPawn(PawnSystem pawn)
        {
            Destroy(gameObject, Time.deltaTime);
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

        public void SetUseRotateInput(bool useRotateInput)
        {
            if (_UseRotateInput == useRotateInput)
            {
                return;
            }

            _UseRotateInput = useRotateInput;

            if (UseRotateInput)
                OnUseRotateInput();
            else
                UnUseRotateInput();
        }

        public void SetRotateInput(Vector3 direction)
        {
            if (!UseRotateInput)
                return;

            Vector3 prevDirection = direction;

            _RotateDirection = direction;

            if (prevDirection == Vector3.zero && _RotateDirection != Vector3.zero)
            {
                OnStartRotateInput();
            }
            else if (prevDirection != Vector3.zero && _RotateDirection == Vector3.zero)
            {
                OnFinishRotateInput(prevDirection);
            }
        }

        public void SetLookTarget(Transform newLookTarget)
        {
            if (_LookTarget == newLookTarget)
            {
                return;
            }

            var prevTarget = _LookTarget;

            _LookTarget = newLookTarget;

            OnChangeLookTarget(prevTarget);
        }


        #region Callback
        public void OnPossessPawn()
        {
            if (_UseDebugMode)
                Debug.Log("OnPossessedPawn [" + gameObject.name + "] " + Pawn);

            OnPossessedPawn?.Invoke(this, Pawn);
        }
        public void UnPossessPawn()
        {
            if (_UseDebugMode)
                Debug.Log("UnPossessPawn [" + gameObject.name + "] " + Pawn);

            UnPossessedPawn?.Invoke(this, Pawn);
        }


        public void OnUseMovementInput()
        {
            if (_UseDebugMode)
                Debug.Log("OnUseMovementInput [" + gameObject.name + "]");

            OnUsedMovementInput?.Invoke(this);
        }
        public void UnUseMovementInput()
        {
            if (_UseDebugMode)
                Debug.Log("UnUseMovementInput [" + gameObject.name + "]");

            UnUsedMovementInput?.Invoke(this);
        }


        public void OnStartMovementInput()
        {
            if (_UseDebugMode)
                Debug.Log("OnStartMovementInput [" + gameObject.name + "] " + MoveDirection + " * " + MoveStrength);

            OnStartedMovementInput?.Invoke(this, MoveDirection, MoveStrength);
        }
        public void OnFinishMovementInput(Vector3 prevDirection, float prevMoveStrength)
        {
            if (_UseDebugMode)
                Debug.Log("OnFinishMovementInput [" + gameObject.name + "] " + prevDirection + " * " + prevMoveStrength);

            OnFinishedMovementInput?.Invoke(this, prevDirection, prevMoveStrength);
        }


        public void OnUseRotateInput()
        {
            if (_UseDebugMode)
                Debug.Log("OnUseRotateInput [" + gameObject.name + "]");

            OnUsedRotateInput?.Invoke(this);
        }
        public void UnUseRotateInput()
        {
            if (_UseDebugMode)
                Debug.Log("UnUseRotateInput [" + gameObject.name + "]");

            UnUsedRotateInput?.Invoke(this);
        }

        public void OnStartRotateInput()
        {
            if (_UseDebugMode)
                Debug.Log("OnStartRotateInput [" + gameObject.name + "] " + RotateDirection);

            OnStartedRotatetInput?.Invoke(this, RotateDirection);
        }
        public void OnFinishRotateInput(Vector3 prevDirection)
        {
            if (_UseDebugMode)
                Debug.Log("OnFinishRotateInput [" + gameObject.name + "] " + prevDirection);

            OnFinishedRotateInput?.Invoke(this, prevDirection);
        }

        public void OnChangeLookTarget(Transform prevLookTarget = null)
        {
            if (_UseDebugMode)
                Debug.Log("OnChangeLookTarget [" + gameObject.name + "] New Look Target : " + _LookTarget + " Prev Look Target : " + prevLookTarget);

            OnChangedLookTarget?.Invoke(this, _LookTarget, prevLookTarget);
        }
        #endregion
    }

}

