using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactReceiver : MonoBehaviour {

    float mass;
    float groundFriction;
    float airFriction;
    float recoveryImpact;
    float bounceRatio;
    Vector3 impact = Vector3.zero;
    CharacterController character;
    bool frozen;
    bool impactActive;
    public bool isImpactActive { get { return impactActive; } }
 
    void Start()
    {
        frozen = false;
        impactActive = false;
        ActorValues actorValues = GetComponent<ActorValues>();
        mass = actorValues.impactValues.Mass;
        groundFriction = actorValues.impactValues.GroundFriction;
        airFriction = actorValues.impactValues.AirFriction;
        recoveryImpact = actorValues.impactValues.RecoveryImpact;
        bounceRatio = actorValues.impactValues.BounceRatio;
        character = GetComponent<CharacterController>();
    }

    // call this function to add an impact force:
    public void AddImpact(Vector3 direction, float force)
    {
        direction.Normalize();
        if (direction.y < 0 && character.isGrounded)
        {
            direction.y = 0; //prevents player from being launched into the air and unable to jump?
        }
        impact += direction.normalized * force / mass;
        impactActive = true;
    }

    public void Reflect(Vector3 normal)
    {
        impact = impact - (1+bounceRatio) * (Vector3.Dot(impact, normal) * normal);
    }

    public void Freeze()
    {
        frozen = true;
    }

    public void Unfreeze()
    {
        frozen = false;
    }

    void FixedUpdate()
    {
        if (!frozen && impactActive)
        {
            // set impact to zero if below recovery threshold
            if (impact.magnitude <= recoveryImpact)
            {
                impact = Vector3.zero;
                character.Move(impact);
                impactActive = false;
                return;
            }

            impact += Physics.gravity * Time.deltaTime;

            if (character.isGrounded)
            {
                //Apply ground friction
                impact = Vector3.Lerp(impact, Vector3.zero, Time.fixedDeltaTime * groundFriction);
            }
            else
            {
                //Apply air friction
                impact = Vector3.Lerp(impact, Vector3.zero, Time.fixedDeltaTime * airFriction);
            }
            character.Move(impact * Time.fixedDeltaTime);
        }
    }
}
