using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : CharacterStat
{
    public override void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            var _characterController = GetComponent<CharacterController>();
            _characterController.enabled = false;
            _characterController.transform.position = GameManager.Instance.startPoint.position;
            _characterController.enabled = true;

            currentHealth = maxHealth;
        }
    }
}
