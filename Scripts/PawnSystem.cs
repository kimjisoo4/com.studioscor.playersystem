using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

namespace KimScor.Pawn
{
    public class PawnSystem : MonoBehaviour
    {
        #region Events
        public delegate void ChangedControllerHandler(PawnSystem pawn, ControllerSystem controller);
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
        public bool IsPlayer => _IsPlayer;
        
        public event ChangedControllerHandler OnPossessedController;
        public event ChangedControllerHandler UnPossessedController;

        public event IgnoreInput OnIgnoreMovementInput;
        public event IgnoreInput UnIgnoreMovementInput;

        public event IgnoreInput OnIgnoreRotateInput;
        public event IgnoreInput UnIgnoreRotateInput;

        protected void OnEnable()
        {
            OnInitialization();

            PlayerManager.Instance.AddPawn(this);
        }
        protected void OnDisable()
        {
#if UNITY_EDITOR
            if (!this.gameObject.scene.isLoaded) return;
#endif
            PlayerManager.Instance.RemovePawn(this);
        }

        private void OnInitialization()
        {
            if (_Controller != null)
            {
                _Controller.OnPossess(this);

                return;
            }
            
                
            if (_IsPlayer && PlayerManager.Instance.PlayerCharacter == null)
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

        private void SpawnAndPossessAiController()
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

            return Controller.TurnDirection;
        }
        #endregion

        #region EDITOR
        [Conditional("UNITY_EDITOR")]
        private void Log(string log)
        {
            if (_UseDebugMode)
                UnityEngine.Debug.Log("PawnSystem [" + gameObject.name  + "] : " + log, this);
        }
        #endregion

        #region Callback
        protected void OnPossessController()
        {
            Log("On Possessed Controller [" + gameObject.name + "] " + Controller);

            OnPossessedController?.Invoke(this, Controller);
        }
        protected void UnPossessController()
        {
            Log("Un Possessed Controller [" + gameObject.name + "] " + Controller);

            UnPossessedController?.Invoke(this, Controller);
        }

        protected void OnUseMovementInput()
        {
            Log("On Use MovementInput");

            OnIgnoreMovementInput?.Invoke(this);
        }
        protected void UnUseMovementInput()
        {
            Log("Un Use MovementInput");

            UnIgnoreMovementInput?.Invoke(this);
        }



        protected void OnUseRotateInput()
        {
            Log("On Use Rotate Input");

            OnIgnoreRotateInput?.Invoke(this);
        }
        protected void UnUseRotateInput()
        {
            Log("Un Use Rotate Input");

            UnIgnoreRotateInput?.Invoke(this);
        }

        #endregion
    }

}

