using UnityEngine;

namespace KimScor.Pawn
{
    public class PlayerManager : MonoBehaviour
    {
        private static PlayerManager _Instance = null;

        [Header(" [ Default Player Controller ] ")]
        [SerializeField] private ControllerSystem _DefaultPlayerController;

        [Header(" [ Instance ] ")]
        [SerializeField] private ControllerSystem _PlayerController;
        public PawnSystem PlayerCharacter => PlayerController.Pawn;

        public static PlayerManager Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = FindObjectOfType<PlayerManager>();
                }

                return _Instance;
            }
        }

        public ControllerSystem PlayerController
        {
            get
            {
                if (_PlayerController == null)
                {
                    _PlayerController = Instantiate(_DefaultPlayerController);
                }

                return _PlayerController;
            }
        }

        void Awake()
        {
            if (_Instance == null)
            {
                _Instance = this;
            }
        }
    }

}

