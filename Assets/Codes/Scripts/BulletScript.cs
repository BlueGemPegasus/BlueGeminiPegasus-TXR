using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public float damage = 10f;

    public Rigidbody rb;

    private void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        CharacterStat cs;
        
        if(other.TryGetComponent(out cs))
        {
            cs.TakeDamage(damage);
        }


        Destroy(gameObject);
    }
}
