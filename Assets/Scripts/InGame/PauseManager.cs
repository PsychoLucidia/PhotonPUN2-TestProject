using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;

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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        PauseMenu();
    }

    public void PauseMenu()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIManager.Instance.pauseMenu.SetActive(!UIManager.Instance.pauseMenu.activeSelf);
        }
    }

    public void ButtonPress()
    {
        UIManager.Instance.pauseMenu.SetActive(!UIManager.Instance.pauseMenu.activeSelf);
    }
}
