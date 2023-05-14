using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDoor : MonoBehaviour
{
    [Tooltip("Drop all the enemy before the red door here")]
    public GameObject[] enemyList;
    [Tooltip("Reference the door to open after all enemy dead")]
    public Transform doorToOpen;
    
    [SerializeField] private bool _anyEnemyNotDied = false;

    private Vector3 _doorOpenPosition;

    private void Start()
    {
        _doorOpenPosition = doorToOpen.position;
        _doorOpenPosition.y = doorToOpen.position.y + 10f;
    }

    private void Update()
    {
        // Check if there is still enemy exist
        foreach (GameObject enemy in enemyList)
        {
            if (enemy.activeSelf)
            {
                Debug.Log(enemy.name + "ActiveSelf");
                _anyEnemyNotDied = true;
                break;
            }
            else
            {
                _anyEnemyNotDied = false;
            }
        }

        if (!_anyEnemyNotDied)
        {
            Debug.Log("Open Door");
            // Open door if all enemies are gone
            doorToOpen.position = _doorOpenPosition;
        }
    }
}
