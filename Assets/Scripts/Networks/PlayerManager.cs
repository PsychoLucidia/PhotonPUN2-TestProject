using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    [Header("Instantiate Positions")]
    public Transform pOnePosition;
    public Transform pTwoPosition; 
    public GameObject playerPrefab;

    [Header("Buttons")]
    public GameObject startButton;

    [Header("Lists")]
    private List<PhotonView> _playerList = new List<PhotonView>();
    public List<PhotonView> playerList = new List<PhotonView>();

    // Start is called before the first frame update
    void Start()
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

            if (SceneManager.GetActiveScene().buildIndex == 1)
            {
                PhotonNetwork.AutomaticallySyncScene = false;
                CheckAbleToStart();
            }
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
            PhotonNetwork.AutomaticallySyncScene = true;
            GameSceneManager.Instance.ChangeSceneMultiplayer(2);
        }
    }

    #endregion

    #region In-game Methods

    public void UpdatePlayerList()
    {
        if (_playerList == playerList) { return; }

        photonView.RPC("PlayerListHandler", RpcTarget.All);
    }

    [PunRPC]
    public void PlayerListHandler()
    {
        _playerList = playerList;

        if (playerList.Count == 1)
        {
            
        }
    }

    #endregion
}
