using UnityEngine;
using StudioScor.Utilities;

namespace StudioScor.PlayerSystem
{
    [DefaultExecutionOrder(PlayerSystemExecutionOrder.SUB_ORDER)]
    public class PlayerStart : BaseMonoBehaviour
    {
        [Header(" [ Player Start ] ")]
        [SerializeField] private PlayerManager _PlayerManager;

        private void Start()
        {
            if (!_PlayerManager.HasPlayerPawn)
            {
                Log(" Player Manager Not Has Player Pawn. Create New Palyer Pawn. ");

                _PlayerManager.SpawnPlayerPawn(transform.position, transform.rotation);
            }
            else
            {
                Log(" Player Manager Has Player Pawn. Not Create Pawn. ");
                
                if (!_PlayerManager.HasPlayerController)
                {
                    Log(" Player Manager Not Has Player Controller. Create New Player Controller. ");

                    _PlayerManager.SpawnPlayerController();
                }    
            }
        }
    }

}

