using UnityEngine;

namespace StudioScor.PlayerSystem
{
    public class PlayerStart : MonoBehaviour
    {
        [Header(" [ Player Start ] ")]
        [SerializeField] private PlayerManager _PlayerManager;

        private void Start()
        {
            _PlayerManager.SpawnPlayer(transform.position, transform.rotation);
        }
    }

}

