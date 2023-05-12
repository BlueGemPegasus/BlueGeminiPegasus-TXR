using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.GetComponent<PlayerScript>()._characterController.enabled = false;
            other.transform.position = GameManager.Instance.startPoint.position;
            other.transform.GetComponent<PlayerScript>()._characterController.enabled = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.GetComponent<PlayerScript>()._characterController.enabled = false;
            other.transform.position = GameManager.Instance.startPoint.position;
            other.transform.GetComponent<PlayerScript>()._characterController.enabled = true;
        }
    }
}
