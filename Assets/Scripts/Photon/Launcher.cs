using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace LW.Photon
{
    public class Launcher : MonoBehaviourPunCallbacks
    {
        #region Private Serializable Fields
        [Tooltip("The prefab to use for representing the player")]
        public GameObject playerPrefab;
        [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
        [SerializeField]
        private byte maxPlayersPerRoom = 4;
        [Tooltip("The UI Label to inform the user that the connection is in progress")]
        [SerializeField]
        private GameObject connectingCanvas;

        #endregion

        #region Private fields

        /// <summary>
        /// This client's version nnumber. Users are separated from each other by gameVersion (which allows you to make breaking changes)
        /// </summary>
        string gameVersion = "1";
        /// <summary>
        /// Keep track of the current process. Since connection is asynchronous and is based on several callbacks from Photon,
        /// we need to keep track of this to properly adjust the behavior when we receive call back by Photon.
        /// Typically this is used for the OnConnectedToMaster() callback.
        /// </summary>
        bool isConnecting;

        #endregion

        #region Monobehavior Callbacks

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during early initialization phase.
        /// </summary>

        private void Awake()
        {
            // #Critical
            // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        void Start()
        {
            connectingCanvas.SetActive(false);
            Connect();
        }

        #endregion


        #region Public Methods

        public void Connect()
        {
            connectingCanvas.SetActive(true);
            // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
            if (PhotonNetwork.IsConnected)
            {
                // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                // #Critical, we must first and foremost connect to Photon Online Server.
                isConnecting = PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = gameVersion;
            }
        }

        #endregion


        #region MonoBehaviorPunCallbacks Callbacks

        public override void OnConnectedToMaster()
        {
            Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN");

            // #Critical: The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnJoinRandomFailed()
            if (isConnecting)
            {
                PhotonNetwork.JoinRandomRoom();
                isConnecting = false;
            }
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            isConnecting = false;

            Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

            // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
        }

        public override void OnJoinedRoom()
        {
            //Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");

            //// #Critical: We only load if we are the first player, else we rely on `PhotonNetwork.AutomaticallySyncScene` to sync our instance scene.
            //if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            //{
            //    Debug.Log("We load the 'Room for 1' ");


            //    // #Critical
            //    // Load the Room Level.
            //    PhotonNetwork.LoadLevel("Room for 1");
            //}

            base.OnJoinedRoom();
            // TODO remove logs
            Debug.Log("\nPhotonLobby.OnJoinedRoom()");
            Debug.Log("Current room name: " + PhotonNetwork.CurrentRoom.Name);
            Debug.Log("Other players in room: " + PhotonNetwork.CountOfPlayersInRooms);
            Debug.Log("Total players in room: " + (PhotonNetwork.CountOfPlayersInRooms + 1));
            CreatePlayer();
            connectingCanvas.SetActive(false);
        }

        #endregion

        private void CreatePlayer()
        {
            var player = PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity);
            Debug.Log("Instantiated playerPrefab"); // TODO remove
        }

    }

}
