using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField roomNameInputField;

    public TextMeshProUGUI connectionStatusText;
    public bool isCheckingRoom = false;
    bool isCreatingRoom = false;

    public List<RoomInfo> rooms = new List<RoomInfo>();

    void Start()
    {
        Transform rootObj = UIManager.Instance.popUpPanel.transform;
        Transform image = rootObj.transform.Find("Image");
        connectionStatusText = image.transform.Find("TextStatus").GetComponent<TextMeshProUGUI>();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);

        rooms.Clear();
        foreach (RoomInfo room in roomList)
        {
            rooms.Add(room);
        }

        Debug.Log("Room List Updated. Room Count: " + rooms.Count);
    }

    #region Button Actions
    public void CreateRoom()
    {
        StartCoroutine(CreateCheckRoom());
    }

    public void JoinRoom()
    {
        StartCoroutine(JoinCheckRoom());
    }
    #endregion

    #region Coroutines

    IEnumerator CreateCheckRoom()
    {

        UIManager.Instance.popUpPanel.SetActive(true);
        connectionStatusText.text = null;
        
        if (string.IsNullOrWhiteSpace(roomNameInputField.text) || roomNameInputField.text.Length < 4)
        {
            connectionStatusText.text = "Please enter a valid room name (at least 4 characters).";

            yield return new WaitForSecondsRealtime(1f);
            UIManager.Instance.popUpPanel.SetActive(false);

            yield break;
        }
        else
        {
            isCheckingRoom = true;
        }

        while (isCheckingRoom)
        {
            connectionStatusText.text = "Checking if room exists...";
            Debug.Log("Checking if room exists...");
            yield return new WaitForSecondsRealtime(0.2f);

            if (rooms.Count > 0)
            {
                foreach (RoomInfo room in rooms)
                {
                    if (room.Name == roomNameInputField.text && room.IsOpen && room.IsVisible)
                    {
                        Debug.Log("Room already exists. Is open: " + room.IsOpen + ". Is visible: " + room.IsVisible);
                        connectionStatusText.text = "Room already exists.";
                        isCheckingRoom = false;
                    }
                    else if (room.Name == roomNameInputField.text && !room.IsOpen && !room.IsVisible)
                    {
                        Debug.Log("Room does not exist. Creating: " + roomNameInputField.text);

                        connectionStatusText.text = "Room does not exist. Creating: " + roomNameInputField.text;
                        isCheckingRoom = false;
                        isCreatingRoom = true;

                        yield return new WaitForSecondsRealtime(0.5f);
                        PhotonNetwork.CreateRoom(roomNameInputField.text);
                        GameManager.Instance.isMultiplayer = true;
                    }
                }
            }
            else
            {
                Debug.Log("Lobby does not have rooms.");
                connectionStatusText.text = "Lobby does not have rooms. Creating: " + roomNameInputField.text;
                isCheckingRoom = false;
                isCreatingRoom = true;

                yield return new WaitForSecondsRealtime(0.5f);
                PhotonNetwork.CreateRoom(roomNameInputField.text);
                GameManager.Instance.isMultiplayer = true;
            }
            yield return null;
        }
        
        if (!isCreatingRoom)
        {
            yield return new WaitForSecondsRealtime(0.5f);
            UIManager.Instance.popUpPanel.SetActive(false);
        }
    }

    IEnumerator JoinCheckRoom()
    {
        UIManager.Instance.popUpPanel.SetActive(true);
        connectionStatusText.text = null;

        if (string.IsNullOrWhiteSpace(roomNameInputField.text) || roomNameInputField.text.Length < 4)
        {
            connectionStatusText.text = "Please enter a valid room name (at least 4 characters).";

            yield return new WaitForSecondsRealtime(1f);
            UIManager.Instance.popUpPanel.SetActive(false);

            yield break;
        }
        else
        {
            isCheckingRoom = true;
        }

        while (isCheckingRoom)
        {
            connectionStatusText.text = "Checking if room exists...";
            Debug.Log("Checking if room exists...");
            yield return new WaitForSecondsRealtime(0.2f);

            if (rooms.Count > 0)
            {
                foreach (RoomInfo room in rooms)
                {
                    if (room.Name == roomNameInputField.text && room.IsOpen)
                    {
                        connectionStatusText.text = "Room exists. Joining: " + roomNameInputField.text;
                        isCheckingRoom = false;
                        isCreatingRoom = true;

                        yield return new WaitForSecondsRealtime(0.5f);
                        PhotonNetwork.JoinRoom(roomNameInputField.text);
                        GameManager.Instance.isMultiplayer = true;
                    }
                    else
                    {
                        connectionStatusText.text = "Room does not exist.";
                        isCheckingRoom = false;
                    }
                }
            }
            else
            {
                connectionStatusText.text = "Lobby does not have rooms.";
                isCheckingRoom = false;
            }
            yield return null;
        }

        if (!isCheckingRoom)
        {
            yield return new WaitForSecondsRealtime(0.5f);
            UIManager.Instance.popUpPanel.SetActive(false);
        }
    }

    #endregion
}
