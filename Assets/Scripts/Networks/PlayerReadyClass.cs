using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PlayerReadyClass : MonoBehaviourPunCallbacks
{
    public const byte CLIENT_READY = 112;

    public override void OnEnable()
    {
        if (PhotonNetwork.NetworkClientState == ClientState.Joined)
        {
            PhotonNetwork.RaiseEvent(CLIENT_READY, null, new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient }, SendOptions.SendReliable);
        }
    }
}
