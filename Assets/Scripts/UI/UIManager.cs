using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject[] uiElements;
    public GameObject popUpPanel;
    public GameObject pauseMenu;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("UI Manager created");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        CheckSceneInitial();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CheckSceneInitial()
    {
        switch (SceneManager.GetActiveScene().buildIndex)
        {
            case 0: MainMenu(); Debug.Log("Main Menu"); break;
            case 1: InGame(); break;
            default: Debug.LogWarning("No scene found in index"); break;
        }
    }

    void MainMenu()
    {
        if (PhotonNetwork.NetworkClientState == ClientState.JoinedLobby)
        {
            uiElements[0].SetActive(false);
            uiElements[1].SetActive(true);
            popUpPanel.SetActive(false);
            StartCoroutine(ReconnectCoroutine());
        }
        else
        {
            uiElements[0].SetActive(true);
            uiElements[1].SetActive(false);
            popUpPanel.SetActive(false);
        }
    }

    void InGame()
    {
        popUpPanel.SetActive(false);
        pauseMenu.SetActive(false);
    }

    IEnumerator ReconnectCoroutine()
    {
        yield return new WaitForSecondsRealtime(0.02f);

        NetworkManager.Instance.DisconnectFromServer();
        yield return new WaitUntil(() => NetworkManager.Instance.disconnectFromServerCor == null);

        NetworkManager.Instance.ConnectToServer();
        yield return new WaitUntil(() => NetworkManager.Instance.connectToServerCor == null);
    }
}
