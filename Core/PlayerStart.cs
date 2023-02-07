using UnityEngine;
namespace StudioScor.PlayerSystem
{
    [DefaultExecutionOrder(PlayerSystemExecutionOrder.SUB_ORDER)]
    public class PlayerStart : MonoBehaviour
    {
        private void Start()
        {
            if(!PlayerManager.Instance.PlayerPawn)
                PlayerManager.Instance.SpawnPlayerPawn(transform.position, transform.rotation);
        }
    }
}

