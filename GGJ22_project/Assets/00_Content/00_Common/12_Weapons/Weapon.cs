using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public bool canMoveDuringAttack = false;
    public bool resetVelocityDuringAttack = false;

    public float attackRadius = 10.0f;

    public GameObject AttackImpactLocation;

    public GameObject HitFX;

    public void Attack()
    {
        HitFX.SetActive(true);

        Collider[] hitColliders = Physics.OverlapSphere(AttackImpactLocation.transform.position, attackRadius);
        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                hitCollider.gameObject.GetComponent<EnemyController>().TakeDamage();
            }
        }
    }
}
