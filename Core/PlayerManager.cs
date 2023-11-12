using UnityEngine;
using StudioScor.Utilities;

namespace StudioScor.PlayerSystem
{
    [CreateAssetMenu(menuName ="StudioScor/PlayerSystem/new PlayerManager")]
    public class PlayerManager : BaseScriptableObject
    {
        #region Event
        public delegate void SetPlayerPawnEventHandler(PlayerManager playerManager, IPawnSystem currentPawn, IPawnSystem prevPawn = null);
        public delegate void SetPlayerContollerEventHandler(PlayerManager playerManager, IControllerSystem currentController, IControllerSystem prevController = null);
        #endregion

        [Header(" [ Player Manager ]")]
        [SerializeField] private GameObject defaultPlayerController;
        [SerializeField] private GameObject defaultPlayerPawn;

        private IControllerSystem playerController;
        private IPawnSystem playerPawn;

        public IControllerSystem PlayerController => playerController;
        public IPawnSystem PlayerPawn => playerPawn;

        public bool HasPlayerPawn => PlayerPawn != null;
        public bool HasPlayerController => PlayerController != null;

        public event SetPlayerPawnEventHandler OnChangedPlayerPawn;
        public event SetPlayerContollerEventHandler OnChangedPlayerController;

        protected override void OnReset()
        {
            base.OnReset();

            playerController = null;
            playerPawn = null;
        }
        public void SpawnPlayerPawn(Vector3 position, Quaternion rotation)
        {
            var instance = Instantiate(defaultPlayerPawn, position, rotation);

            if (!instance.TryGetPawnSystem(out IPawnSystem pawnSystem))
                LogError($"{instance} is Not Has IPawnSystem");

            pawnSystem.SetStartPlayer(true);

            if (!HasPlayerController)
            {
                SpawnPlayerController();
            }
        }
        public void SpawnPlayerController()
        {
            Instantiate(defaultPlayerController);
        }

        public void ForceSetPlayerPawn(IPawnSystem pawnSystem)
        {
            Log("Force Set Player Pawn : " + pawnSystem);
            Log("Has Player Pawn : " + HasPlayerPawn);
            Log("Current Player Pawn : " + playerPawn);

            if (HasPlayerPawn && playerPawn == pawnSystem)
                return;

            var prevPawn = playerPawn;
            playerPawn = pawnSystem;

            if (HasPlayerController)
            {
                Log(HasPlayerPawn ? "On Possess" : "Un Possess");

                playerController.OnPossess(playerPawn);
            }

            Callback_OnChangedPlayerPawn(prevPawn);
        }

        public void ForceSetPlayerController(IControllerSystem controller)
        {
            Log("Force Set Player Controller : " + controller);
            Log("Has Player Controller : " + HasPlayerController);
            Log("Current Player Controller : " + playerController);

            if (HasPlayerController && playerController == controller)
                return;

            var prevController = playerController;
            playerController = controller;

            if (playerController is not null)
            {
                playerController.OnPossess(playerPawn);
            }

            Callback_OnChangedPlayerController(prevController);
        }

        #region Callback
        protected void Callback_OnChangedPlayerController(IControllerSystem prevController)
        {
            Log("On Changed PlayerController - Current : " + PlayerController + " Prev : " + prevController);

            OnChangedPlayerController?.Invoke(this, playerController, prevController);
        }
        protected void Callback_OnChangedPlayerPawn(IPawnSystem prevPawn)
        {
            Log("On Changed PlayerPawn - Current :" + playerPawn + " Prev : " + prevPawn);   

            OnChangedPlayerPawn?.Invoke(this, playerPawn, prevPawn);
        }
        #endregion
    }

}

