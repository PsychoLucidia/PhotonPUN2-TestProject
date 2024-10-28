using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomTracker : MonoBehaviourPunCallbacks
{
    public static RoomTracker Instance;

    public List<RoomInfo> rooms = new List<RoomInfo>();

    int _previousSceneIndex;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex != _previousSceneIndex)
        {
            _previousSceneIndex = SceneManager.GetActiveScene().buildIndex;

        }
    }

    
}
