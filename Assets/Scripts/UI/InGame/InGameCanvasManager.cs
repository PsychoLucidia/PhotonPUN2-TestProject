using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class InGameCanvasManager : MonoBehaviourPunCallbacks
{
    public GameObject uiWin;

    [PunRPC]
    public void SetActiveWin()
    {
        uiWin.SetActive(true);
    }
}
