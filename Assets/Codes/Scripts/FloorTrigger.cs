using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorTrigger : MonoBehaviour
{
    public GameObject floor;

    public BossScript bossScript;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            floor.SetActive(false);
            bossScript.triggerBossFight = true;
            Invoke("FloorUpdate", 5);
        }
    }

    private void FloorUpdate()
    {
        floor.SetActive(true);
    }
}
