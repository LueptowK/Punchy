using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylinderAttackSelectorNode : BehaviorNode
{
    bool firing;
    float timeToNextFire;
    float fireTimer;

    public CylinderAttackSelectorNode(BehaviorNode[] nodeChildren, CylinderContext nodeContext) : base(nodeChildren, nodeContext)
    {
        children = nodeChildren;
        context = nodeContext;
        timeToNextFire = context.FiringFrequency + Random.Range(-context.FiringFrequencyRange, context.FiringFrequencyRange);
    }

    public override void Update()
    {
        base.Update();
    }

    public override statusValues FixedUpdate()
    {
        return base.FixedUpdate();
    }

    private void CheckIfFiring()
    {
        fireTimer += Time.fixedDeltaTime;
        if (fireTimer >= timeToNextFire && CheckLineOfSight())
        {
            if (Vector3.Distance(context.Player.transform.position, context.Actor.transform.position) > 20 && Mathf.Abs(context.Player.transform.position.y - context.Transform.position.y) < 0.5f)
            {
                if (TryAttack(1))
                {
                    state = enemyState.laserState;
                    LaserSetup();
                }
            }
            if (state != enemyState.laserState)
            {
                if (TryAttack(0))
                {
                    firing = true;
                    fireTimer = 0;
                }
            }
        }

        if (firing)
        {
            material.color = Color.Lerp(defaultColor, fireColor, fireTimer / context.FireWindupTime);
            //stopgap to prevent firing into walls - cylinder will hold token indefinitely if it can never see player
            if (fireTimer >= fireWindupTime && CheckLineOfSight())
            {
                audioSource.PlayOneShot(firingSound, 0.8f);
                FireProjectile();
            }
        }
    }

    //shoudl probably be a utility on EnemyController?
    private bool CheckLineOfSight()
    {
        RaycastHit seePlayer;
        Ray ray = new Ray(context.Transform.position, context.Player.transform.position - context.Transform.position);

        if (Physics.Raycast(ray, out seePlayer, Mathf.Infinity))
        {
            if (seePlayer.collider.gameObject.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }
}
