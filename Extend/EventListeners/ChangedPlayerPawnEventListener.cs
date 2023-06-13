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

        private void PlayerManager_OnChangedPlayerPawn(PlayerManager playerManager, IPawnSystem currentPawn, IPawnSystem prevPawn = null)
        {
            Transform prev = prevPawn is not null ? prevPawn.transform : null;
            Transform current = currentPawn is not null ? currentPawn.transform : null;

            Callback_OnChangedPlayerPawn(current, prev);
        }

        private void Callback_OnChangedPlayerPawn(Transform current, Transform prev = null)
        {
            OnChangedPrevPlayerPawn?.Invoke(prev);
            OnChangedCurrentPlayerPawn?.Invoke(current);
        }
    }

}

