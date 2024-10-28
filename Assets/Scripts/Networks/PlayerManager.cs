using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("Instantiate Positions")]
    public Transform pOnePosition;
    public Transform pTwoPosition; 
    public GameObject playerPrefab;

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

    // Update is called once per frame
    void Update()
    {
        
    }
}
