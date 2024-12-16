using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviourPunCallbacks
{
    public static GameSceneManager Instance;


    // Start is called before the first frame update
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

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeSceneMultiplayer(int sceneIndex)
    {
        StartCoroutine(LoadSceneMultiplayerCoroutine(sceneIndex));
    }

    public void ChangeScene(int sceneIndex)
    {
        StartCoroutine(LoadSceneCoroutine(sceneIndex));
    }

    IEnumerator LoadSceneMultiplayerCoroutine(int sceneIndex)
    {  
        PhotonNetwork.LoadLevel(sceneIndex);

        while (PhotonNetwork.LevelLoadingProgress < 1f)
        {
            Debug.Log("Loading Progress: " + PhotonNetwork.LevelLoadingProgress);
            yield return null;
        }
    }

    IEnumerator LoadSceneCoroutine(int sceneIndex)
    {

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneIndex);

        while (!asyncOperation.isDone)
        {

            yield return null;
        }
    }
}
