using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager Instance;

    public TextMeshProUGUI connectionStatusText;
    public TMP_InputField usernameInputField;

    public float maxTimeout = 10f;
    public float timeoutTimer = 0;

    [SerializeField] PhotonView[] _pViewArray;

    Coroutine _connectionCoroutine;

    public Coroutine connectToServerCor;
    public Coroutine disconnectFromServerCor;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }

    void Initialization()
    {
        Transform rootObj = UIManager.Instance.popUpPanel.transform;
        Transform image = rootObj.transform.Find("Image");
        connectionStatusText = image.transform.Find("TextStatus").GetComponent<TextMeshProUGUI>();

        Transform menuRoot = null;
        foreach (GameObject gameObject in UIManager.Instance.uiElements)
        {
            if (gameObject.name == "MainMenu")
            {
                menuRoot = gameObject.transform;
            }
        }

        if (menuRoot != null)
        {
            usernameInputField = menuRoot.transform.Find("Username").GetComponent<TMP_InputField>();
        }
        else
        {
            Debug.Log("Main Menu GameObject not found");
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1 || SceneManager.GetActiveScene().buildIndex == 2)
        {
            GetPhotonViews();
            _connectionCoroutine = StartCoroutine(CheckConnection());
        }

        Initialization();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void GetPhotonViews()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        _pViewArray = new PhotonView[players.Length];

        for (int i = 0; i < players.Length; i++)
        {
            if (_pViewArray[i] == null)
            {
                _pViewArray[i] = players[i].GetComponent<PhotonView>();
            }
            else
            {

            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Client State: " + PhotonNetwork.NetworkClientState);
        if (PhotonNetwork.IsConnectedAndReady)
        {
            Debug.Log("Connected and ready");
        }
    }


    public void ConnectToServer()
    {
        connectToServerCor = StartCoroutine(ConnectToServerCoroutine());
    }

    public void DisconnectFromServer()
    {
        disconnectFromServerCor = StartCoroutine(DisconnectFromServerCoroutine());
    }

    public void LeaveRoom()
    {
        StartCoroutine(LeaveRoomCoroutine());
    }

    public void LeaveAndDisconnect()
    {
        StartCoroutine(DisconnectInGameCoroutine());
    }

    #region Photon Callbacks

    /// <summary>
    /// This method is called when the client successfully connects to the Photon server.
    /// It joins the lobby and then calls the base implementation of the method.
    /// </summary>
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to master");

        PhotonNetwork.JoinLobby();

        base.OnConnectedToMaster();
    }

    /// <summary>
    /// This method is called when the client successfully joins the lobby.
    /// It logs a message and then calls the base implementation of the method.
    /// </summary>
    public override void OnJoinedLobby()
    {
        Debug.Log("Joined the lobby: " + PhotonNetwork.CurrentLobby.Name);

        base.OnJoinedLobby();
    }

    /// <summary>
    /// This method is called when the client successfully joins a room.
    /// It logs a message indicating that the client has joined the room and then calls the base implementation of the method.
    /// </summary>
    public override void OnJoinedRoom()
    {
        Debug.Log("Joined the room");

        base.OnJoinedRoom();

        GameSceneManager.Instance.ChangeSceneMultiplayer(1);
    }

    /// <summary>
    /// This method is called when the client successfully creates a room.
    /// It logs a message indicating that the client has created the room and then calls the base implementation of the method.
    /// </summary>
    public override void OnCreatedRoom()
    {
        Debug.Log("Created the room");

        base.OnCreatedRoom();
    }

    /// <summary>
    /// This method is called when the client fails to join a room.
    /// </summary>
    /// <param name="returnCode">The return code from the server.</param>
    /// <param name="message">The error message from the server.</param>
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Join room failed. Return code: " + returnCode + ". Message: " + message);

        base.OnJoinRoomFailed(returnCode, message);
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
    }

    /// <summary>
    /// This method is called when the client gets disconnected from the Photon server.
    /// It logs a message indicating that the client has disconnected from the server and the cause of the disconnection.
    /// </summary>
    /// <param name="cause">The cause of the disconnection.</param>
    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);

        Debug.Log("Disconnected. Cause: " + cause);
    }


    #endregion

    #region Coroutines

    /// <summary>
    /// Coroutine to connect to the Photon server and update the UI elements accordingly.
    /// It initiates a connection request, updates the connection status text and UI
    /// while waiting for the connection to complete. Once connected, it updates the
    /// connection status text and transitions to the room selection screen.
    /// </summary>
    IEnumerator ConnectToServerCoroutine()
    {
        if (usernameInputField.text == "")
        {
            if (RoomManager.Instance.previousUsername != "")
            {
                PhotonNetwork.LocalPlayer.NickName = RoomManager.Instance.previousUsername;
            }
            else
            {
                connectToServerCor = null;
                yield break;
            }
        }
        else
        {
            RoomManager.Instance.previousUsername = usernameInputField.text;
            PhotonNetwork.LocalPlayer.NickName = usernameInputField.text;
        }

        // Connect to the server
        PhotonNetwork.ConnectUsingSettings();
        connectionStatusText.text = null;
        UIManager.Instance.popUpPanel.SetActive(true);
        timeoutTimer = 0;

        Debug.Log("Connection Phase 1");

        while (!PhotonNetwork.IsConnectedAndReady)
        {
            connectionStatusText.text = "Connecting to server...";

            if (timeoutTimer > maxTimeout)
            {
                break;
            }
            else if (PhotonNetwork.NetworkClientState == ClientState.Disconnected)
            {
                break;
            }

            timeoutTimer += Time.deltaTime;
            yield return null;
        }

        Debug.Log("Connection Phase 2");

        if (!PhotonNetwork.IsConnectedAndReady)
        {
            connectionStatusText.text = "Failed to connect to server";

            yield return new WaitForSeconds(1f);
            UIManager.Instance.popUpPanel.SetActive(false);
        }
        else
        {
            connectionStatusText.text = "Connected to server";
            Debug.Log($"Connected to server. Actor ID: {PhotonNetwork.LocalPlayer.ActorNumber}. Name: {PhotonNetwork.LocalPlayer.NickName}");
            
            yield return new WaitForSeconds(1f);
            UIManager.Instance.popUpPanel.SetActive(false);
            UIManager.Instance.uiElements[0].SetActive(false);
            UIManager.Instance.uiElements[1].SetActive(true);
        }

        connectToServerCor = null;
    }

    /// <summary>
    /// Coroutine to disconnect from the server and update UI elements accordingly.
    /// It initiates a disconnect request, updates the connection status text and UI
    /// while waiting for the disconnection to complete. Once disconnected, it resets
    /// the UI to its initial state.
    /// </summary>
    IEnumerator DisconnectFromServerCoroutine()
    {
        PhotonNetwork.Disconnect();
        connectionStatusText.text = null;
        UIManager.Instance.popUpPanel.SetActive(true);

        while (PhotonNetwork.IsConnected)
        {
            connectionStatusText.text = "Disconnecting from server...";
            Debug.Log("Disconnecting from server...");

            yield return null;
        }

        if (!PhotonNetwork.IsConnected)
        {
            UIManager.Instance.popUpPanel.SetActive(false);
            UIManager.Instance.uiElements[0].SetActive(true);
            UIManager.Instance.uiElements[1].SetActive(false);
        }

        disconnectFromServerCor = null;
    }

    IEnumerator LeaveRoomCoroutine()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount <= 1)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;
                PhotonNetwork.CurrentRoom.IsVisible = false;
            }
            Debug.Log("Left room. IsOpen: " + PhotonNetwork.CurrentRoom.IsOpen + ". IsVisible: " + PhotonNetwork.CurrentRoom.IsVisible);
        }

        PhotonNetwork.LeaveRoom();
        StopCoroutine(_connectionCoroutine);
        connectionStatusText.text = null;
        UIManager.Instance.popUpPanel.SetActive(true);

        while (PhotonNetwork.NetworkClientState == ClientState.Leaving)
        {
            connectionStatusText.text = "Leaving room...";
            Debug.Log("Leaving room...");

            yield return null;
        }

        yield return new WaitForSecondsRealtime(0.5f);

        if (PhotonNetwork.NetworkClientState == ClientState.JoinedLobby)
        {
            GameManager.Instance.isMultiplayer = false;
            GameSceneManager.Instance.ChangeSceneMultiplayer(0);

            if (SceneManager.GetActiveScene().buildIndex != 0)
            {
                UIManager.Instance.popUpPanel.SetActive(false);
            }
        }
    }

    IEnumerator DisconnectInGameCoroutine()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount <= 1)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;
                PhotonNetwork.CurrentRoom.IsVisible = false;
                Debug.Log("Left room. IsOpen: " + PhotonNetwork.CurrentRoom.IsOpen + ". IsVisible: " + PhotonNetwork.CurrentRoom.IsVisible);
            }
        }

        PhotonNetwork.Disconnect();
        StopCoroutine(_connectionCoroutine);
        connectionStatusText.text = null;
        UIManager.Instance.popUpPanel.SetActive(true);

        while (PhotonNetwork.NetworkClientState == ClientState.Disconnecting)
        {
            connectionStatusText.text = "Leaving room and disconnecting...";
            Debug.Log("Disconnecting...");
            yield return null;
        }

        yield return new WaitForSecondsRealtime(0.5f);

        if (PhotonNetwork.NetworkClientState == ClientState.Disconnected)
        {
            GameManager.Instance.isMultiplayer = false;
            GameSceneManager.Instance.ChangeSceneMultiplayer(0);
            if (SceneManager.GetActiveScene().buildIndex != 0)
            {
                UIManager.Instance.popUpPanel.SetActive(false);
            }
        }
    }

    #endregion

    IEnumerator CheckConnection()
    {
        Debug.Log("Started checking connection");
        yield return new WaitForSecondsRealtime(1f);

        while (PhotonNetwork.IsConnectedAndReady && GameManager.Instance.isMultiplayer)
        {            
            yield return new WaitForSecondsRealtime(1f);
        }

        while (!PhotonNetwork.IsConnectedAndReady)
        {
            if (!PhotonNetwork.IsConnectedAndReady && !GameManager.Instance.isMultiplayer)
            {
                UIManager.Instance.popUpPanel.SetActive(true);
                connectionStatusText.text = "Returning to main menu...";

                yield return new WaitForSecondsRealtime(1f);
                GameSceneManager.Instance.ChangeScene(0);
                break;
            }
            else if (!PhotonNetwork.IsConnectedAndReady && GameManager.Instance.isMultiplayer)
            {
                UIManager.Instance.popUpPanel.SetActive(true);
                connectionStatusText.text = "Connection lost... Reconnecting...";
                PhotonNetwork.ReconnectAndRejoin();
                break;
            }
            yield return null;
        }
    }
}
