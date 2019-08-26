using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityScript.Steps;

public class BirdieEnemyController : EnemyController
{
    private enum enemyState { wanderingState, preparingState, divingState, attackingState, returningState, dodgingState};
    private enemyState state;
    private bool hitPlayer;
    EnemyAttackTokenPool.Token token;
    Vector3 escapeRoute;
    [SerializeField] float steeringMaxWandering;
    [SerializeField] float steeringMaxPreparing;
    [SerializeField] float steeringMaxDiving;
    [SerializeField] float steeringMaxAttacking;
    [SerializeField] float steeringMaxReturning;
    [SerializeField] float steeringMaxDodging;
    [SerializeField] float dodgeDistance;
    [SerializeField] float defaultSpeed;
    [SerializeField] int scoreValue;
    [SerializeField] AudioClip attackingSound;
    [SerializeField] GameObject fractures;
    [SerializeField] ParticleSystem explosion;
    Color defaultColor;
    Color fireColor = Color.magenta;
    Material material;
    MeshRenderer cachedRenderer;

    AudioSource audioSource;
    private float stateTimer;
    bool dead;
    NavMeshHit closestHit;



    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        audioSource = gameObject.GetComponent<AudioSource>();
        cachedRenderer = gameObject.GetComponent<MeshRenderer>();
        material = cachedRenderer.material;
        defaultColor = material.color;
        nav = GetComponent<NavMeshAgent>();
        nav.enabled = false;
        enemyAttackTokenPool = player.GetComponentInChildren<EnemyAttackTokenPool>();
        type = SpawnManager.EnemyType.Birdie;
        state = enemyState.wanderingState;
        dead = false;
        stateTimer = 0f;
        escapeRoute = Vector3.forward;
        old_velocity = Vector3.up;
    }

    // Update is called once per frame
    protected override void FixedUpdate()
    {
        if (!dead)
        {
            stateTimer += Time.unscaledDeltaTime;
            if (this.old_velocity != Vector3.zero)
            {
                RaycastHit hit;
                Physics.Raycast(this.transform.position, this.old_velocity, out hit, Mathf.Infinity, LayerMask.GetMask("Default"));
                if (hit.distance < dodgeDistance)
                {
                    state = enemyState.dodgingState;
                }
            }
            switch (state)
            {
                case enemyState.wanderingState:
                    state = Wander();
                    break;
                case enemyState.preparingState:
                    state = MoveToPlayer();
                    break;
                case enemyState.divingState:
                    state = DiveDown();
                    break;
                case enemyState.attackingState:
                    state = AttackPlayer();
                    break;
                case enemyState.returningState:
                    state = ReturnUp();
                    break;

                case enemyState.dodgingState:
                    state = Dodge();
                    break;

            }
        }
    }
    private void moveWithSteering(Vector3 targetDirection, float steeringForce)
    {
        Vector3 desiredDirection = Vector3.Normalize(targetDirection);
        Vector3 currentDirection = Vector3.Normalize(this.old_velocity);
        Vector3 steeringFull = desiredDirection - currentDirection;
        Vector3 steeringLimited = Vector3.ClampMagnitude(steeringFull, steeringForce);

        Vector3 velocity = defaultSpeed * (Vector3.Normalize(currentDirection + steeringLimited));
        this.old_velocity = velocity;
        this.transform.rotation = Quaternion.LookRotation(velocity);
        enemyMover.Move(velocity);
    }

    private enemyState Wander()
    {
        if (TryAttack(0))
        {
            return enemyState.preparingState;
        }
        else
        {
            moveWithSteering(this.old_velocity, steeringMaxWandering);
            return enemyState.wanderingState;
        }
    }

    private enemyState MoveToPlayer()
    {
        Vector3 birdieToAboveTarget = player.transform.position + Vector3.up * 15 - this.transform.position;
        Vector3 xzBirdieToAboveTarget = new Vector3(birdieToAboveTarget.x, 0, birdieToAboveTarget.z);
        Vector3 birdieToClosestAttackPoint = birdieToAboveTarget - 20 * Vector3.Normalize(xzBirdieToAboveTarget);
        if (Vector3.Magnitude(birdieToClosestAttackPoint) < defaultSpeed)
        {
            Debug.Log("Close to strike!");
            return enemyState.divingState;
        }
        moveWithSteering(birdieToClosestAttackPoint, steeringMaxPreparing);
        return enemyState.preparingState;
    }

    private enemyState DiveDown()
    {
        if (this.old_velocity.normalized.y < -0.99)
        {
            Debug.Log("Finished dive");
            material.color = fireColor;
            audioSource.PlayOneShot(attackingSound);
            return enemyState.attackingState;
        }
        moveWithSteering(Vector3.down, steeringMaxDiving);
        return enemyState.divingState;
    }

    private enemyState AttackPlayer()
    {
        Vector3 birdieToTarget = player.transform.position + Vector3.up - this.transform.position;

        if(Vector3.Dot(birdieToTarget,this.old_velocity)<0)
        {
            Debug.Log("Returning");
            EndAttack();
            return enemyState.returningState;
        }
        else if (hitPlayer)
        {
            hitPlayer = false;
            state = enemyState.returningState;
            EndAttack();
            return enemyState.returningState;
        }
        else
        {
            moveWithSteering(birdieToTarget, steeringMaxAttacking);
            return enemyState.attackingState;
        }
    }

    private enemyState ReturnUp()
    {
        hitPlayer = false;
        if (this.old_velocity.normalized.y > 0.99)
        {
            Debug.Log("Finished Return");
            return enemyState.wanderingState;
        }
        moveWithSteering(Vector3.up, steeringMaxReturning);
        return enemyState.returningState;
    }

    private enemyState Dodge()
    {
        if (this.token != null)
        {
            EndAttack();
        }

        Vector3 currentDirection = Vector3.Normalize(this.old_velocity);
        Vector3 idealDirection = escapeRoute;
        float idealDistance = 0;

        RaycastHit hit;
        Physics.Raycast(this.transform.position, this.escapeRoute, out hit, Mathf.Infinity, LayerMask.GetMask("Default"));
        if (hit.distance < dodgeDistance)
        {
            List<Vector3> possibleDirections = GetEightNormals(this.old_velocity);
            RaycastHit newhit;
            foreach (Vector3 option in possibleDirections)
            {
                Physics.Raycast(this.transform.position, option, out newhit, Mathf.Infinity, LayerMask.GetMask("Default"));
                if (hit.distance > idealDistance)
                {
                    idealDistance = newhit.distance;
                    idealDirection = option;
                }
            }
        }
        escapeRoute = idealDirection;
        moveWithSteering(idealDirection, steeringMaxDodging);

        return enemyState.wanderingState;
    }
    private bool TryAttack(int attackType)
    {
        token = enemyAttackTokenPool.RequestToken(this.gameObject, attackType);
        return (token != null);
    }
    private void EndAttack()
    {
        material.color = defaultColor;
        enemyAttackTokenPool.ReturnToken(this.type, token);
        token = null;
    }
    public override void takeDamage(Vector3 direction)
    {
        if (this.token != null)
        {
            EndAttack();
        }
        playerCamera.gameObject.GetComponent<ScoreTracker>().ChangeScore(scoreValue, transform.position);
        GameObject fractureInstance = Instantiate(fractures, transform.position, transform.rotation);
        fractureInstance.GetComponent<AudioSpeedByTime>().AssignTimeScaleManager(player.GetComponentInChildren<TimeScaleManager>());
        Rigidbody[] fractureCells = fractureInstance.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody cell in fractureCells)
        {
            cell.velocity = old_velocity.magnitude * direction;
        }
        Instantiate(explosion, transform.position, transform.rotation);
        explosion.Play();
        DestroyThis(); // Tells the spawnmanager to delete
    }
    public override void Die()
    {
        if (this.token != null)
        {
            EndAttack();
        }
        playerCamera.gameObject.GetComponent<ScoreTracker>().ChangeScore(scoreValue, transform.position);
        GameObject fractureInstance = Instantiate(fractures, transform.position, transform.rotation);
        fractureInstance.GetComponent<AudioSpeedByTime>().AssignTimeScaleManager(player.GetComponentInChildren<TimeScaleManager>());
        Rigidbody[] fractureCells = fractureInstance.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody cell in fractureCells)
        {
            cell.velocity = old_velocity;
        }
        Instantiate(explosion, transform.position, transform.rotation);
        explosion.Play();
        DestroyThis(); // Tells the spawnmanager to delete
    }
    protected override void OnControllerColliderHit(ControllerColliderHit hit)
    {
        float impact = -Vector3.Dot(hit.normal, old_velocity);

        if (hit.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            hit.gameObject.GetComponent<EnemyController>().takeDamage(old_velocity);
            hit.gameObject.GetComponent<ImpactReceiver>().AddImpact(old_velocity, impact);
        }
        if (hit.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            hit.gameObject.GetComponent<PlayerHealth>().TakeDamage(1, old_velocity, impact);
            if (state == enemyState.attackingState)
            {
                hitPlayer = true;
            }
        }
    }

    private List<Vector3> GetEightNormals (Vector3 direction)
    {
        direction = Vector3.Normalize(direction);
        List<Vector3> normals = new List<Vector3>();
        Vector3 xCross = Vector3.Cross(direction, Vector3.right);
        Vector3 yCross = Vector3.Cross(direction, Vector3.up);
        Vector3 zCross = Vector3.Cross(direction, Vector3.forward);

        for (int i=0; i < 8; i++)
        {
            Vector3 normal = Vector3.zero;
            if (i % 2 < 1) { normal += xCross; } else { normal -= xCross; }
            if (i % 4 < 2) { normal += yCross; } else { normal -= yCross; }
            if (i % 8 < 4) { normal += zCross; } else { normal -= zCross; }
            normals.Add(normal);
        }
        return normals;
    }
}
