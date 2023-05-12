using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject player;

    public bool isPaused = false;
    public bool mouseLocked = false;

    public void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    public void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    public void Update()
    {
        if (isPaused)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    public void TriggerUnlock()
    {
        player.GetComponent<InputCapture>().cursorLocked = false;
    }

    public void TriggerLock()
    {
        player.GetComponent<InputCapture>().cursorLocked = true;

    }
}
