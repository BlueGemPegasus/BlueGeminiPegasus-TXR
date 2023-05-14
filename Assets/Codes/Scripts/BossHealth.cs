using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHealth : CharacterStat
{
    public GameObject triggerFloorBox;
    public Transform afterBossSpawnpoint;
    public GameObject player;

    private CharacterController cc;

    public override void TakeDamage(float damage)
    {
        currentHealth -= damage;    

        if (currentHealth <= 0)
        {
            triggerFloorBox.SetActive(false);
            player.TryGetComponent(out cc);
            cc.enabled = false;
            player.transform.position = afterBossSpawnpoint.position;
            cc.enabled = true;
            Destroy(gameObject);
        }
    }
}
