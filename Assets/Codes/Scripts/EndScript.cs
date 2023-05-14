using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        MessageManager.Instance.StartMessage("Thank you for testing this game, this is the end of project.");
    }
}
