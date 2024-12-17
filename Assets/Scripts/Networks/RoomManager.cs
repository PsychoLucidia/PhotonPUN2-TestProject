using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager Instance;

    public string previousUsername;

    public TMP_InputField roomNameInputField;

    public TextMeshProUGUI connectionStatusText;
    public bool isCheckingRoom = false;
    bool isCreatingRoom = false;

    public List<RoomInfo> rooms = new List<RoomInfo>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        Initialization(SceneManager.GetSceneByBuildIndex(0), SceneManager.GetActiveScene());
    }

    public override void OnEnable()
    {
        SceneManager.activeSceneChanged += Initialization;
    }

    public override void OnDisable()
    {
        SceneManager.activeSceneChanged -= Initialization;
    }

    void Initialization(Scene previous, Scene current)
    {
        Transform rootObj = UIManager.Instance.popUpPanel.transform;
        Transform image = rootObj.transform.Find("Image");
        connectionStatusText = image.transform.Find("TextStatus").GetComponent<TextMeshProUGUI>();

        if (current.buildIndex == 0)
        {
            GetRoomNameInputField();
        }
    }

    public void GetRoomNameInputField()
    {
        if (roomNameInputField == null)
        {
            Transform roomRoot = null;

            foreach (GameObject objs in UIManager.Instance.uiElements)
            {
                if (objs.name == "RoomSelect")
                {
                    roomRoot = objs.transform;
                }
                else
                {
                    continue;
                }
            }

            if (roomRoot != null)
            {
                roomNameInputField = roomRoot.transform.Find("InputField").GetComponent<TMP_InputField>();

                Button createRoomButton = roomRoot.transform.Find("CreateRoomBtn").GetComponent<Button>();
                createRoomButton.onClick.AddListener(CreateRoom);

                Button joinRoomButton = roomRoot.transform.Find("JoinRoomBtn").GetComponent<Button>();
                joinRoomButton.onClick.AddListener(JoinRoom);
            }
            else
            {
                Debug.LogWarning("RoomSelect GameObject not found.");
                return;
            }
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);
        
        rooms.Clear();
        rooms.AddRange(roomList);

        Debug.Log("Room List Updated. Room Count: " + rooms.Count);
    }

    #region Button Actions

    /// <summary>
    /// Create a room with the name given in the roomNameInputField.
    /// </summary>
    /// <remarks>
    /// If the roomNameInputField is null, it will search for a GameObject with the name "RoomSelect" in the UIManager's uiElements list,
    /// and then get the TMP_InputField component from the "InputField" child of the "RoomSelect" GameObject.
    /// After that, it will start a coroutine to check if the room already exists.
    /// If roomNameInputField is null, returns.
    /// </remarks>
    public void CreateRoom()
    {
        if (roomNameInputField != null)
        {
            StartCoroutine(CreateCheckRoom());
        }
        else
        {
            Debug.LogWarning("RoomNameInputField is null.");
            return;
        }
    }

    public void JoinRoom()
    {
        if (roomNameInputField != null)
        {
            StartCoroutine(JoinCheckRoom());
        }
        else
        {
            Debug.LogWarning("RoomNameInputField is null.");
            return;
        }
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

                        if (PhotonNetwork.IsConnectedAndReady)
                        {
                            PhotonNetwork.CreateRoom(roomNameInputField.text);
                            GameManager.Instance.isMultiplayer = true;
                        }
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

                if (PhotonNetwork.IsConnectedAndReady)
                {
                    PhotonNetwork.CreateRoom(roomNameInputField.text);
                    GameManager.Instance.isMultiplayer = true;
                }
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
