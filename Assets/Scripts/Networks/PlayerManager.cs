using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public int playersReady;
    public bool isGameStarted;

    [Header("Instantiate Positions")]
    public Transform pOnePosition;
    public Transform pTwoPosition; 
    public GameObject playerPrefab;

    [Header("Buttons")]
    public GameObject startButton;

    [Header("Colliders")]
    public GameObject colliders;

    [Header("Lists")]
    private List<PhotonView> _playerList = new List<PhotonView>();
    public List<PhotonView> playerList = new List<PhotonView>();


    void Awake()
    {
        if (GameManager.Instance.isMultiplayer)
        {
            Debug.Log("PlayerManager is running. Actor number: " + PhotonNetwork.LocalPlayer.ActorNumber);

            if (PhotonNetwork.LocalPlayer.ActorNumber == 1)
            {
                PhotonNetwork.Instantiate("Prefabs/" + playerPrefab.name, pOnePosition.position, pOnePosition.rotation, 0);
            }
            else if (PhotonNetwork.LocalPlayer.ActorNumber >= 2)
            {
                PhotonNetwork.Instantiate("Prefabs/" + playerPrefab.name, pTwoPosition.position, pTwoPosition.rotation, 0);
            }
        }

        playersReady = 0;
        isGameStarted = false;
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            colliders = GameObject.Find("Colliders");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.Instance.isMultiplayer)
        {
            if (SceneManager.GetActiveScene().buildIndex == 1)
            {
                CheckAbleToStart();
            }
            if (SceneManager.GetActiveScene().buildIndex == 2)
            {
                
            }
        }
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            UpdatePlayerList();
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            CheckAbleToStart();
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);

        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            CheckAbleToStart();
        }
    }

    #region Lobby Methods

    void CheckAbleToStart()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (PhotonNetwork.PlayerList.Length == 2)
            {
                startButton.SetActive(true);
            }
            else if (PhotonNetwork.PlayerList.Length <= 1)
            {
                startButton.SetActive(false);
            }
        }
    }

    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Starting Game. AutomaticallySyncScene: " + PhotonNetwork.AutomaticallySyncScene);

            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            GameSceneManager.Instance.ChangeSceneMultiplayer(2);
        }
    }

    #endregion

    #region In-game Methods

    public void UpdatePlayerList()
    {
        if (!isGameStarted) { return; }
        if (_playerList == playerList) { return; }

        photonView.RPC("PlayerListHandler", RpcTarget.All);
    }

    [PunRPC]
    public void PlayerListHandler()
    {
        _playerList = playerList;

        if (playerList.Count == 1)
        {
            Debug.Log("Win");
        }
    }

    [PunRPC]
    public void GetAllPhotonViews()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        PhotonView[] views = new PhotonView[players.Length];

        for (int i = 0; i < players.Length; i++)
        {
            views[i] = players[i].GetPhotonView();
        }

        playerList = new List<PhotonView>(views);
    }

    [PunRPC]
    public void RemoveColliders()
    {
        colliders.SetActive(false);
        isGameStarted = true;
    }

    IEnumerator StartGameCoroutine()
    {
        Debug.LogWarning("GameStarted");
        yield return new WaitForSeconds(3f);

        photonView.RPC("RemoveColliders", RpcTarget.All);
        
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == 112)
        {
            playersReady++;
            Debug.Log("PlayersReady: " + playersReady + "/" + PhotonNetwork.PlayerList.Length);

            if (playersReady == PhotonNetwork.PlayerList.Length)
            {
                StartCoroutine(StartGameCoroutine());
            }
        }
    }

    #endregion
}
