using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamDamage : MonoBehaviour
{
    MeshRenderer thisObjectMR;
    Material[] material;

    bool shouldDamage = false;

    CharacterStat characterStat;

    private void Start()
    {
        thisObjectMR = GetComponent<MeshRenderer>();
        material = thisObjectMR.materials;
    }

    private void Update()
    {
        if (material[0].GetFloat("_Metallic") >= 0.8)
        {
            shouldDamage = true;
        }
        else
        {
            shouldDamage = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(shouldDamage)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                other.TryGetComponent(out characterStat);
                characterStat.TakeDamage(10f);
            }
        }
        else
        {
            return;
        }
    }
}
