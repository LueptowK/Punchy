using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBallProjectileController : MonoBehaviour
{
    private float damage;
    [SerializeField] private int immediateDamage;
    [SerializeField] private float energyBallDuration;
    [SerializeField] private float maxForce;
    private float maxSpeed;
    private Rigidbody rb;
    private float timer;  // give this a Get 
    private float force;
    EnemyAttackTokenPool.Token token;
    EnemyAttackTokenPool tokenPool;
    bool playerInRange;
    float accumulatedDamage;
    Collider otherCollider;

    GameObject player;
    GameObject wizard;
    GameObject energyBallsParent;
    ActorValues actorValues;

    
    // Use this for initialization
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        
    }

    private void Start()
    {
        timer = 0;
        playerInRange = false;
        actorValues = GetComponent<ActorValues>();
        StartCoroutine(DamageOverTime());
    }

    // Update is called once per frame
    void Update()
    {
        if (wizard == null || timer > energyBallDuration) // if wizard has been destroyed
        {
            StartDeathSequence();
        }
        UpdateTrajectory();
    }

    void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;
        //Vector3 newVelocity = new Vector3(rb.velocity.x, rb.velocity.y - gravity * Time.fixedDeltaTime, rb.velocity.z);
        //rb.velocity = newVelocity;
    }

    public void Initialize(GameObject wizardObject, GameObject playerObject, EnemyAttackTokenPool enemyAttackTokenPool, GameObject energyBallsParent) {
        this.wizard = wizardObject;
        this.player = playerObject;
        this.tokenPool = enemyAttackTokenPool;
        this.energyBallsParent = energyBallsParent;
    }

    // this is directly after the energy ball is instatiated
    public void Fire(float speed, float damage, float force, EnemyAttackTokenPool.Token token)
    {     
        Vector3 unitDirection = (player.transform.position - wizard.transform.position).normalized;
        Vector3 velocity = unitDirection * speed;
        rb.velocity = velocity;
        this.damage = damage;
        this.force = force;
        this.maxSpeed = speed;
        this.token = token;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerHealth>().TakeDamage(immediateDamage, rb.velocity.normalized, force);
        }
    }

    // trigger is collider with larger radius that controls damage
    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag != "Enemy" && other.gameObject.tag != "Hitbox")
        {
            if (other.gameObject.tag == "Player")
            {
                playerInRange = true;
                otherCollider = other;
                accumulatedDamage += damage;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerInRange = false;
            otherCollider = null;
            //other.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage, rb.velocity.normalized, force);
        }
    }

    private void UpdateTrajectory()
    {
        Vector3 unitDirection = (player.transform.position - this.transform.position).normalized;
        Vector3 desiredVelocity = unitDirection * maxSpeed;
        Vector3 steering = desiredVelocity - rb.velocity;
        if (steering.magnitude > maxForce) steering = Vector3.ClampMagnitude(steering, maxForce);
        steering = steering / actorValues.impactValues.Mass;
        Vector3 steeredVelocity = rb.velocity + steering;
        if (steeredVelocity.magnitude > maxSpeed) steeredVelocity = Vector3.ClampMagnitude(steeredVelocity, maxSpeed);
        rb.velocity = steeredVelocity;

        Vector3 separation = computeSeparation();
        rb.velocity += separation;

    }

    private Vector3 computeSeparation()
    {
        int neighborCount = 0;
        Vector3 computationVector = new Vector3();
        foreach (Transform agent in energyBallsParent.transform)
        {
            if (agent != this.transform)
            {
                float distanceFromPlayer = (agent.transform.position - player.transform.position).magnitude;
                // weight so that when distancfe from player is high they separate more
                computationVector += (agent.transform.position - this.transform.position)*distanceFromPlayer; 
                neighborCount++;
            }
        }
        if (neighborCount == 0) return computationVector;

        // divide by neighbor count and negate
        computationVector /= neighborCount * -1;
        return computationVector.normalized;
    }

    //private void OnDrawGizmos()
    //{
    //    Ray v = new Ray(this.transform.position, rb.velocity);
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawRay(v);
    //    Debug.DrawRay(this.transform.position, rb.velocity, Color.red, rb.velocity.magnitude);
    //}

    private void Die()
    {
        tokenPool.ReturnToken(SpawnManager.EnemyType.CowardlyWizard, token);
        Destroy(this.gameObject);
    }

    private void StartDeathSequence()
    {
        // wait for some time and/or fade out
        Die();
    }

    IEnumerator DamageOverTime()
    {
        while (true)
        {
            if (playerInRange)
            {
                if (accumulatedDamage < 1f) accumulatedDamage = 1f;
                otherCollider.gameObject.GetComponent<PlayerHealth>().TakeDamage(Mathf.FloorToInt(accumulatedDamage), rb.velocity.normalized, force);
                accumulatedDamage = 0f;
            }

            yield return new WaitForSeconds(2);
        }     
    }
}
