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
        public UnityEvent<Transform> OnChangedCurrentPlayerPawn;
        public UnityEvent<Transform> OnChangedPrevPlayerPawn;


        private void OnEnable()
        {
            if (_PlayerManager.HasPlayerController)
            {
                Callback_OnChangedPlayerPawn(_PlayerManager.PlayerController.transform, null);
            }

            _PlayerManager.OnChangedPlayerPawn += PlayerManager_OnChangedPlayerPawn;
        }
        private void OnDisable()
        {
            _PlayerManager.OnChangedPlayerPawn -= PlayerManager_OnChangedPlayerPawn;
        }

        private void PlayerManager_OnChangedPlayerPawn(PlayerManager playerManager, PawnComponent currentPawn, PawnComponent prevPawn = null)
        {
            OnChangedPrevPlayerPawn?.Invoke(prevPawn ? prevPawn.transform : null);
            OnChangedCurrentPlayerPawn?.Invoke(currentPawn ? currentPawn.transform : null);

            Transform prev = prevPawn ? prevPawn.transform : null;
            Transform current = currentPawn ? currentPawn.transform : null;
            
            Callback_OnChangedPlayerPawn(current, prev);
        }

        private void Callback_OnChangedPlayerPawn(Transform current, Transform prev = null)
        {
            OnChangedPrevPlayerPawn?.Invoke(prev);
            OnChangedCurrentPlayerPawn?.Invoke(current);
        }
    }

}

