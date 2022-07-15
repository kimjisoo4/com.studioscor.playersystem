using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KimScor.Pawn
{
    public class PawnSystem : MonoBehaviour
    {
        #region Events
        public delegate void ChangedControllerHandler(PawnSystem pawn, ControllerSystem controller);
        public delegate void DeadPawnHandler(PawnSystem pawn);
        public delegate void IgnoreInput(PawnSystem pawn);
        #endregion

        [Header(" [ Use Player Controller ] ")]
        [SerializeField] private bool _IsPlayer = false;

        [Header(" [ Default Ai Controller ] ")]
        [SerializeField] private ControllerSystem _DefaultController;

        [SerializeField] private ControllerSystem _Controller;
        public ControllerSystem Controller => _Controller;

        [SerializeField] private bool _UseAutoPossesed = true;

        [Header(" [ Ignore Movement Input ] ")]
        [SerializeField] private bool _IgnoreMovementInput = false;
        public bool IgnoreMovementInput => _IgnoreMovementInput;

        [Header(" [ Ignore Rotate Input ]")]
        [SerializeField] private bool _IgnoreRotateInput = false;
        public bool IgnoreRotateInput => _IgnoreRotateInput;

        [Header(" [ Use DebugMode] ")]
        [SerializeField] private bool _UseDebugMode = false;
        [SerializeField] private bool _IsDead = false;

        public bool IsDead => _IsDead;
        public bool IsAlive => !_IsDead;
        public bool IsPlayer => _IsPlayer;
        
        public event ChangedControllerHandler OnPossessedController;
        public event ChangedControllerHandler UnPossessedController;

        public event DeadPawnHandler OnDeadPawn;

        public event IgnoreInput OnIgnoreMovementInput;
        public event IgnoreInput UnIgnoreMovementInput;

        public event IgnoreInput OnIgnoreRotateInput;
        public event IgnoreInput UnIgnoreRotateInput;

        private void Awake()
        {
            if (_IsPlayer)
            {
                PlayerManager.Instance.PlayerController.OnPossess(this);
            }
            else
            {
                if (_UseAutoPossesed)
                {
                    SpawnAndPossessAiController();
                }
            }
        }

        public void OnPossess(ControllerSystem controller)
        {
            if (_Controller == controller)
            {
                return;
            }

            if (_Controller != null)
            {
                _Controller.UnPossess(this);
            }

            _Controller = controller;

            if (_Controller is not null)
            {
                _Controller.OnPossess(this);

                if (_Controller.IsPlayerController)
                {
                    _IsPlayer = true;
                }
            }
        }

        public void UnPossess(ControllerSystem controller)
        {
            if (_Controller == controller) 
            {
                _Controller = null;

                if (controller.IsPlayerController)
                {
                    _IsPlayer = false;

                    if(_UseAutoPossesed)
                    {
                        SpawnAndPossessAiController();
                    }
                    
                }
            }
        }

        public void SpawnAndPossessAiController()
        {
            if (_DefaultController != null)
            {
                _Controller = Instantiate(_DefaultController);

                _Controller.OnPossess(this);
            }
        }

        #region Setter
        public void SetUseMovementInput(bool useMovementInput)
        {
            if (_IgnoreMovementInput == useMovementInput)
            {
                return;
            }

            _IgnoreMovementInput = useMovementInput;

            if (IgnoreMovementInput)
                OnUseMovementInput();
            else
                UnUseMovementInput();
        }
        public void SetUseRotateInput(bool useRotateInput)
        {
            if (_IgnoreRotateInput == useRotateInput)
            {
                return;
            }

            _IgnoreRotateInput = useRotateInput;

            if (IgnoreRotateInput)
                OnUseRotateInput();
            else
                UnUseRotateInput();
        }
        #endregion

        #region Getter
        public Vector3 GetMoveDirection()
        {
            if (IgnoreMovementInput || !Controller)
                return Vector3.zero;

            return Controller.MoveDirection;
        }
        public float GetMoveStrength()
        {
            if (IgnoreMovementInput || !Controller)
                return 0;

            return Controller.MoveStrength;
        }
        public Vector3 GetRotateDirection()
        {
            if (IgnoreRotateInput || !Controller)
                return Vector3.zero;

            return Controller.RotateDirection;
        }
        #endregion


        #region Callback
        public void OnPossessController()
        {
            if (_UseDebugMode)
                Debug.Log("OnPossessedController [" + gameObject.name + "] " + Controller);

            OnPossessedController?.Invoke(this, Controller);
        }
        public void UnPossessController()
        {
            if (_UseDebugMode)
                Debug.Log("UnPossessedController [" + gameObject.name + "] " + Controller);

            UnPossessedController?.Invoke(this, Controller);
        }

        public void OnDie()
        {
            if (_UseDebugMode)
                Debug.Log("OnDeadCharacter [" + gameObject.name + "]");

            if (!IsAlive)
            {
                return;
            }

            _IsDead = true;

            OnDeadPawn?.Invoke(this);
        }

        public void OnUseMovementInput()
        {
            if (_UseDebugMode)
                Debug.Log("OnUseMovementInput [" + gameObject.name + "]");

            OnIgnoreMovementInput?.Invoke(this);
        }
        public void UnUseMovementInput()
        {
            if (_UseDebugMode)
                Debug.Log("UnUseMovementInput [" + gameObject.name + "]");

            UnIgnoreMovementInput?.Invoke(this);
        }



        public void OnUseRotateInput()
        {
            if (_UseDebugMode)
                Debug.Log("OnUseRotateInput [" + gameObject.name + "]");

            OnIgnoreRotateInput?.Invoke(this);
        }
        public void UnUseRotateInput()
        {
            if (_UseDebugMode)
                Debug.Log("UnUseRotateInput [" + gameObject.name + "]");

            UnIgnoreRotateInput?.Invoke(this);
        }

        #endregion
    }

}

