using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBallProjectileController : MonoBehaviour
{
    private float damage;
    [SerializeField] private int immediateDamage;
    [SerializeField] private float energyBallDuration;
    private float speed;
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

    // Use this for initialization
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        
    }

    private void Start()
    {
        timer = 0;
        playerInRange = false;
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

    // this is directly after the energy ball is instatiated
    public void Fire(GameObject wizardObject, GameObject playerObject, float speed, float damage, float force, EnemyAttackTokenPool.Token token, EnemyAttackTokenPool enemyAttackTokenPool)
    {
        this.wizard = wizardObject;
        this.player = playerObject;
        Vector3 unitDirection = (player.transform.position - wizard.transform.position).normalized;
        Vector3 velocityWithoutArc = unitDirection * speed;
        //float distance = Vector3.Distance(wizardPosition, playerPosition);
        //float timeToPlayer = distance / speed;
        //float initialYVelocity = gravity * timeToPlayer / 2f;
        //Vector3 desiredVelocity = new Vector3(velocityWithoutArc.x, velocityWithoutArc.y + initialYVelocity, velocityWithoutArc.z);
        //rb.velocity = desiredVelocity;
        rb.velocity = velocityWithoutArc;
        this.damage = damage;
        this.force = force;
        this.speed = speed;
        this.token = token;
        this.tokenPool = enemyAttackTokenPool;
        //bit of a stopgap cuz I don't really know how shaders work yet
        // //this.transform.Rotate(0, 90, 0);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Enemy" && other.gameObject.tag != "Hitbox")
        {
            if (other.gameObject.tag == "Player")
            {
                other.gameObject.GetComponent<PlayerHealth>().TakeDamage(immediateDamage, rb.velocity.normalized, force);
            }
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
                //other.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage, rb.velocity.normalized, force);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag != "Enemy" && other.gameObject.tag != "Hitbox")
        {
            if (other.gameObject.tag == "Player")
            {
                playerInRange = false;
                otherCollider = null;
                //other.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage, rb.velocity.normalized, force);
            }
        }
    }

    public void UpdateTrajectory()
    {
        Vector3 unitDirection = (player.transform.position - this.transform.position).normalized;
        Vector3 velocityWithoutArc = unitDirection * speed;
        //float distance = Vector3.Distance(this.transform.position, playerPosition);
        //float timeToPlayer = distance / speed;
        //float initialYVelocity = gravity * timeToPlayer / 2f;
        //Vector3 desiredVelocity = new Vector3(velocityWithoutArc.x, velocityWithoutArc.y + initialYVelocity, velocityWithoutArc.z);
        rb.velocity = velocityWithoutArc;
    }

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

    public float Timer
    {
        get
        {
            return timer;
        }
    }
}
