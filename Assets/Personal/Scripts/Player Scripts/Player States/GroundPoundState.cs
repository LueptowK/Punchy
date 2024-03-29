﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundPoundState : PlayerState
{
    PlayerMover playerMover;
    Vector3 move;
    private float groundPoundHopSpeed;
    bool grounded = false;
    float gravityMultiplier;
    float speedMaximum;
    float speedMinimumToPound;
    float physicsMaxForce;
    float height;
    bool charging;
    RaycastHit hit;
    private LayerMask enemyMask;
    ChargeController chargeController;
    AudioSource audioSource;
    AudioClip groundPoundSound;
    ParticleSystem groundPoundParticles;

    public GroundPoundState(PlayerMover pm) : base(pm)
    {
        playerMover = pm;
        groundPoundHopSpeed = playerMover.playerValues.groundPoundStateValues.GroundPoundHopSpeed;
        gravityMultiplier = playerMover.playerValues.groundPoundStateValues.GravityMultiplier;
        speedMaximum = playerMover.playerValues.groundPoundStateValues.SpeedMaximum;
        speedMinimumToPound = playerMover.playerValues.groundPoundStateValues.SpeedMinimumToPound;
        physicsMaxForce = playerMover.playerValues.groundPoundStateValues.PhysicsMaxForce;
        groundPoundParticles = playerMover.playerValues.groundPoundStateValues.GroundPoundParticles;
        groundPoundSound = playerMover.playerValues.groundPoundStateValues.GroundPoundSound;
        audioSource = playerMover.GetComponent<AudioSource>();
        height = playerMover.GetComponent<CharacterController>().height;

        vulnerable = false;
        chargeController = playerMover.ChargeController;
        enemyMask = LayerMask.GetMask("Enemy", "Spiders");
    }


    public override PlayerState FixedUpdate()
    {
        chargeController.Charge(charging);

        if (grounded)
        {
            Pound();
            return new GroundState(playerMover);
        }
        
        move += Physics.gravity * gravityMultiplier * Time.fixedDeltaTime;
        if (-move.y > speedMaximum)
        {
            move = new Vector3(move.x, -speedMaximum, move.z);
        }
        playerMover.Move(move);

        MouseLookFixedUpdate();
        return null;
    }

    public override void Update()
    {
        grounded = playerMover.isGrounded() || isGrounded();
        charging = Input.GetButton("Fire1");
        MouseLookUpdate();
    }

    public override void Enter()
    {
        move = new Vector3(0, groundPoundHopSpeed, 0);
        charging = Input.GetButton("Fire1");
    }

    private void Pound()
    {

        float speed = -move.y;
        if (speed > speedMinimumToPound)
        {
            float damageRange = (speed - speedMinimumToPound) /3;
            float physicsRange = (speed - speedMinimumToPound) /2;
            DealPoundDamage(damageRange);
            DealPoundImpacts(physicsRange);
            DealPoundPhysics(physicsRange);
            groundPoundParticles.Play();
            audioSource.PlayOneShot(groundPoundSound);
        }
    }

    private void DealPoundPhysics(float range)
    {
        Collider[] colliders = Physics.OverlapSphere(playerMover.transform.position, range, LayerMask.GetMask("Debris"));
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.attachedRigidbody;
            float forceMultiplier = physicsMaxForce / range;
            if (rb != null)
            {
                rb.AddExplosionForce(forceMultiplier*(Vector3.Distance(hit.gameObject.transform.position, 
                    playerMover.transform.position)), playerMover.transform.position, range, 0F, ForceMode.Impulse);
            }
        }
    }

    private void DealPoundDamage(float range)
    { 
        Collider[] colliders = Physics.OverlapSphere(playerMover.transform.position, range, enemyMask);
        foreach (Collider hit in colliders)
        {
            hit.gameObject.GetComponent<EnemyController>().takeDamage(Vector3.zero);
        }
    }

    private void DealPoundImpacts(float range)
    {
        Collider[] colliders = Physics.OverlapSphere(playerMover.transform.position, range, enemyMask);
        foreach (Collider hit in colliders)
        {
            float maxRange = (speedMaximum - speedMinimumToPound) / 2;
            float forceMultiplier = physicsMaxForce * range / maxRange;
            Vector3 direction = (hit.gameObject.transform.position + Vector3.down * height / 2) - playerMover.transform.position;
            float knockbackModifier = hit.gameObject.GetComponent<EnemyValues>().generalValues.KnockbackModifier;
            //adds an impulse relative to how close they are to the center of the impact
            hit.gameObject.GetComponent<ImpactReceiver>().AddImpact(direction, forceMultiplier * knockbackModifier);
        }
    }

    //Secondary isGrounded is necessary since the CharacterController's isGrounded is occasionally slow to update for some reason
    //this second isGrounded makes ground pound land more consistently snappy on flat surfaces
    private bool isGrounded()
    {
        return Physics.Raycast(playerMover.gameObject.transform.position, new Vector3(0, -1, 0), 1.15f, LayerMask.GetMask("Enemy", "Default"));
    }
}
