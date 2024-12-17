using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    [Header("Instantiate Positions")]
    public Transform pOnePosition;
    public Transform pTwoPosition; 
    public GameObject playerPrefab;

    [Header("Buttons")]
    public GameObject startButton;

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
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        CheckAbleToStart();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);

        CheckAbleToStart();
    }

    void CheckAbleToStart()
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
