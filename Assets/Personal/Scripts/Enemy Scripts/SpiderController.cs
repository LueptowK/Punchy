﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SpiderController : EnemyController
{
    float neighborRadius;
    float cohesionWeight;
    float separationWeight;
    float goalWeight;
    bool frozen;
    Vector3 velocity;
    Vector3 heading;
    Vector2 horizontalHeading;
    Vector2 horizontalVelocity;
    Collider wall;
    [SerializeField] CharacterController controller;
    [SerializeField] SpiderSwarmController swarmController;
    [SerializeField] float maxSpeed;
    float maxSteering;

    // Start is called before the first frame update
    protected override void Start()
    {
        velocity = transform.forward * maxSpeed;
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (!frozen && horizontalHeading != Vector2.zero)
        {
            if (wall == null)
            {
                Vector2 horizontalDesiredVelocity = horizontalHeading * maxSpeed;
                Vector2 steering = horizontalDesiredVelocity - horizontalVelocity;
                steering = Vector2.ClampMagnitude(steering, maxSteering);

                horizontalVelocity = Vector2.ClampMagnitude(horizontalVelocity + steering, maxSpeed);
                //velocity += heading;
                //velocity = velocity.normalized * maxSpeed;
                velocity.x = horizontalVelocity.x;
                velocity.z = horizontalVelocity.y;
                if (!controller.isGrounded)
                {
                    velocity.y -= 9.8f * Time.deltaTime;
                }
                else
                {
                    velocity.y = 0f;
                }
                controller.Move(velocity * Time.deltaTime);
                //this.transform.position = this.transform.position + velocity*Time.deltaTime;
                if (wall == null)
                {
                    this.transform.rotation = Quaternion.LookRotation(new Vector3(horizontalVelocity.x, 0, horizontalVelocity.y));
                }
            }
            else
            {
                //move up wall
                controller.Move(Vector3.up * maxSpeed * Time.deltaTime);
                RaycastHit hit;
                if (Physics.Raycast(transform.position, -transform.up, out hit, 1f, LayerMask.GetMask("Default")))
                {
                    if (hit.collider != wall)
                    {
                        Debug.Log("no longer on wall, but made collision");
                        wall = null;
                        transform.rotation = Quaternion.LookRotation(new Vector3(horizontalHeading.x, 0, horizontalHeading.y), Vector3.up);
                    }
                }
                else
                {
                    Debug.Log("no longer on wall");
                    wall = null;
                    transform.rotation = Quaternion.LookRotation(new Vector3(horizontalHeading.x, 0, horizontalHeading.y), Vector3.up);
                }
                /*
                Collider[] environmentColliders = Physics.OverlapSphere(transform.position, neighborRadius, LayerMask.GetMask("Default"));
                if (!environmentColliders.Contains(wall) && wall != null)
                {
                    {
                        Debug.Log("no longer on wall");
                        wall = null;
                        transform.rotation = Quaternion.LookRotation(heading, Vector3.up);
                    }
                }
                */
            }
        }
    }

    public void SetHeading(Vector3 goal, Vector3 centerOfMass)
    {
        Vector2 goalVector = (new Vector2(goal.x, goal.z) - new Vector2(transform.position.x, transform.position.z)).normalized;
        Collider[] neighbors = GetNeighbors();
        Vector2 separationVector = Vector2.zero;
        Vector2 neighborCenter = Vector2.zero;
        foreach(Collider neighbor in neighbors)
        {
            separationVector.x += neighbor.transform.position.x - transform.position.x;
            separationVector.y += neighbor.transform.position.z - transform.position.z;
            neighborCenter.x += neighbor.transform.position.x;
            neighborCenter.y += neighbor.transform.position.z;
        }
        neighborCenter.x /= neighbors.Length;
        neighborCenter.y /= neighbors.Length;
        separationVector.x *= -1;
        separationVector.y *= -1;
        separationVector = separationVector.normalized;
        Vector2 cohesionVector = neighborCenter - new Vector2(transform.position.x, transform.position.z);
        cohesionVector = cohesionVector.normalized;
        horizontalHeading = separationVector * separationWeight + cohesionVector * cohesionWeight + goalVector * goalWeight;
        horizontalHeading = horizontalHeading.normalized;
    }

    public void SetController(SpiderSwarmController controller)
    {
        swarmController = controller;
        neighborRadius = swarmController.NeighborRadius;
        cohesionWeight = swarmController.CohesionWeight;
        separationWeight = swarmController.SeparationWeight;
        goalWeight = swarmController.GoalWeight;
        maxSteering = swarmController.MaxSteering;
    }

    private Collider[] GetNeighbors()
    {
        return Physics.OverlapSphere(transform.position, neighborRadius, LayerMask.GetMask("Spiders"));
    }

    public override void freeze()
    {
        controller.enabled = false;
        frozen = true;
    }

    void OnControllerColliderHit(ControllerColliderHit collision)
    {
        if (wall == null && collision.collider.gameObject.layer == LayerMask.NameToLayer("Default")) {
            Vector3 surfaceNormal = collision.normal;
            if (!(new Vector3(Mathf.Round(surfaceNormal.x), Mathf.Round(surfaceNormal.y), Mathf.Round(surfaceNormal.z)) == Vector3.up))
            {
                wall = collision.collider;
                transform.rotation = Quaternion.LookRotation(Vector3.up, surfaceNormal);
                Debug.Log("on wall " + wall + " with normal " + surfaceNormal);
            }
        }
    }
    /*
    void OnCollisionExit(Collision collision)
    {
        Debug.Log(collision, collision.collider.gameObject);
        if (collision.collider == wall)
        {
            Debug.Log("no longer on wall");
            wall = null;
            transform.rotation = Quaternion.LookRotation(heading, Vector3.up);
        }
    }
    */
}