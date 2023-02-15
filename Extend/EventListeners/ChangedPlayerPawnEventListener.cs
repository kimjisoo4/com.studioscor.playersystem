using UnityEngine;
using UnityEngine.Events;
using StudioScor.Utilities;

namespace StudioScor.PlayerSystem
{
    public class ChangedPlayerPawnEventListener : BaseMonoBehaviour
    {
        [Header("[ Changed Player Pawn Event ]")]
        [SerializeField] private PlayerManager _PlayerManager;

        [Space(5f)]
        public UnityEvent<PawnComponent> OnChangedPlayerPawn;


        private void OnEnable()
        {
            _PlayerManager.OnChangedPlayerPawn += PlayerManager_OnChangedPlayerPawn;
        }
        private void OnDisable()
        {
            _PlayerManager.OnChangedPlayerPawn -= PlayerManager_OnChangedPlayerPawn;
        }

        private void PlayerManager_OnChangedPlayerPawn(PlayerManager playerManager, PawnComponent currentPawn, PawnComponent prevPawn = null)
        {
            OnChangedPlayerPawn?.Invoke(currentPawn);
        }
    }

}

