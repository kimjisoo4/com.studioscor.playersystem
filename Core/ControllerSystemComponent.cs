using UnityEngine;
using StudioScor.Utilities;

namespace StudioScor.PlayerSystem
{

    public enum ETurnDirectionState
    {
        Direction,
        MoveDirection,
        LookDirection,
    }
    public interface IControllerSystem
    {
        public delegate void ChangePawnEventHandler(IControllerSystem controller, IPawnSystem pawn);
        public delegate void LookTargetEventHandler(IControllerSystem controller, Transform currentLookTarget, Transform prevLookTarget);
        public delegate void TurnDirectionEventHandler(IControllerSystem controller, ETurnDirectionState current, ETurnDirectionState prev);
        public GameObject gameObject { get; }
        public Transform transform { get; }

        public IPawnSystem Pawn { get; }
        public bool IsPlayer { get; }
        public bool IsPossessed { get; }

        public ETurnDirectionState TurnDirectionState { get; }

        public void Possess(IPawnSystem pawn);
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
        public event TurnDirectionEventHandler OnChangedTurnDirection;
    }


    [DefaultExecutionOrder(PlayerSystemExecutionOrder.MAIN_ORDER)]
    [AddComponentMenu("StudioScor/PlayerSystem/Controller System Component", order: 0)]
    public class ControllerSystemComponent : BaseMonoBehaviour, IControllerSystem
    {
        [Header(" [ Controller System Component ] ")]
        [SerializeField] protected PlayerManager _playerManager;
        [SerializeField] private ETurnDirectionState _turnDirectionState;
        [SerializeField][SReadOnlyWhenPlaying] private bool _isPlayer = false;

        private IPawnSystem _pawn;
        private Vector3 _moveDirection;
        private float _moveStrength;
        private Vector3 _turnDirection;
        private Vector3 _lookPosition;
        private Transform _lookTarget;

        public bool IsPlayer => _isPlayer;
        public ETurnDirectionState TurnDirectionState => _turnDirectionState;
        
        public bool IsPossessed => Pawn is not null;
        public IPawnSystem Pawn => _pawn;
        public Vector3 MoveDirection => _moveDirection;
        public float MoveStrength => _moveStrength;

        public Vector3 TurnDirection => _turnDirection;
        public Vector3 LookPosition => _lookPosition;
        public Transform LookTarget => _lookTarget;

        public event IControllerSystem.ChangePawnEventHandler OnPossessedPawn;
        public event IControllerSystem.ChangePawnEventHandler OnUnPossessedPawn;
        public event IControllerSystem.LookTargetEventHandler OnChangedLookTarget;
        public event IControllerSystem.TurnDirectionEventHandler OnChangedTurnDirection;

        public Vector3 GetLookDirection()
        {
            return IsPossessed? _pawn.transform.Direction(GetLookPosition()) : default;
        }
        
        public Vector3 GetTurnDirection()
        {
            switch (TurnDirectionState)
            {
                case ETurnDirectionState.Direction:
                    return _turnDirection;
                case ETurnDirectionState.MoveDirection:
                    return _moveDirection;
                case ETurnDirectionState.LookDirection:
                    return GetLookDirection();
                default:
                    return default;
            }
        }
        public Vector3 GetLookPosition()
        {
            return _lookTarget ? _lookTarget.position : _lookPosition;
        }


        protected virtual void Start()
        {
            if (_isPlayer)
                ForceSetPlayerController();
        }

        public void ForceSetPlayerController()
        {
            _playerManager.ForceSetPlayerController(this);

            if (_playerManager.HasPlayerPawn)
            {
                Possess(_playerManager.PlayerPawn);
            }
        }

        public void Possess(IPawnSystem possessPawn)
        {
            if (_pawn == possessPawn)
                return;

            UnPossess(_pawn);

            _pawn = possessPawn;

            if (IsPlayer)
            {
                _playerManager.ForceSetPlayerPawn(_pawn);
            }

            if (_pawn is null)
                return;

            OnPossess(_pawn);
            _pawn.OnPossess(this);

            Invoke_OnPossessedPawn();
        }
        public void UnPossess(IPawnSystem unPossessPawn)
        {
            if (!IsPossessed)
                return;

            if (_pawn != unPossessPawn)
                return;

            var prevPawn = _pawn;
            _pawn = null;

            OnUnPossess(prevPawn);
            prevPawn.UnPossess();

            Invoke_OnUnPossessedPawn(prevPawn);
        }

        protected virtual void OnPossess(IPawnSystem possessedPawn)
        {

        }
        protected virtual void OnUnPossess(IPawnSystem unPossessedPawn)
        {

        }
        public void SetMoveDirection(Vector3 direction, float strength)
        {
            _moveDirection = direction;
            _moveStrength = strength;
        }

        public void SetTurnDirection(Vector3 direction)
        {
            _turnDirection = direction;    
        }

        public void SetLookPosition(Vector3 position)
        {
            _lookPosition = position;
        }

        public void SetLookTarget(Transform target)
        {
            if (_lookTarget == target)
                return;

            var prevTarget = _lookTarget;
            _lookTarget = target;

            Inovke_OnChangedLookTarget(prevTarget);
        }

        public void SetTurnDirectionState(ETurnDirectionState newDirectionState)
        {
            if (_turnDirectionState == newDirectionState)
                return;

            var prevDirectionState = _turnDirectionState;
            _turnDirectionState = newDirectionState;

            Invoke_OnChangedTurnDirectionState(prevDirectionState);
        }


        #region Invoke
        private void Invoke_OnPossessedPawn()
        {
            Log($"{nameof(OnPossessedPawn)} - {Pawn.gameObject.name}");

            OnPossessedPawn?.Invoke(this, Pawn);
        }
        private void Invoke_OnUnPossessedPawn(IPawnSystem prevPawn)
        {
            Log($"{nameof(OnUnPossessedPawn)} - {prevPawn.gameObject.name}");

            OnUnPossessedPawn?.Invoke(this, prevPawn);
        }

        private void Inovke_OnChangedLookTarget(Transform prevTarget)
        {
            Log($"{nameof(OnChangedLookTarget)} - Current : {(LookTarget ? LookTarget.name : "Null" )} || Prev : {(prevTarget ? prevTarget.name : "Null")}");

            OnChangedLookTarget?.Invoke(this, LookTarget, prevTarget);
        }

        private void Invoke_OnChangedTurnDirectionState(ETurnDirectionState prevState)
        {
            Log($"{nameof(OnChangedTurnDirection)} - Current : {TurnDirectionState} || Prev : {prevState}");

            OnChangedTurnDirection?.Invoke(this, TurnDirectionState, prevState);
        }


        #endregion
    }

}

