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
    Vector3 destination;
    EnemyAttackTokenPool.Token token;
    [SerializeField] float steeringMaxWandering;
    [SerializeField] float steeringMaxPreparing;
    [SerializeField] float steeringMaxDiving;
    [SerializeField] float steeringMaxAttacking;
    [SerializeField] float steeringMaxReturning;
    [SerializeField] float steeringMaxDodging;
    [SerializeField] float dodgeDistance;
    [SerializeField] float defaultSpeed;
    [SerializeField] int scoreValue;
    private float stateTimer;
    private float reevaluateTetherTime;
    bool dead;
    NavMeshHit closestHit;



    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        nav = GetComponent<NavMeshAgent>();
        nav.enabled = false;
        enemyAttackTokenPool = player.GetComponentInChildren<EnemyAttackTokenPool>();
        type = SpawnManager.EnemyType.Birdie;
        state = enemyState.wanderingState;
        dead = false;
        stateTimer = 0f;
        reevaluateTetherTime = 0f;
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
                    Debug.Log("Dodging " + stateTimer);
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
        //moveWithSteering(this.old_velocity, steeringMaxWandering);
        token = enemyAttackTokenPool.RequestToken(this.gameObject, 0);
        if (token != null)
        {
            return enemyState.preparingState;
        }
        else
        {
            Dodge();
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
            enemyAttackTokenPool.ReturnToken(this.type, token);
            return enemyState.returningState;
        }

        moveWithSteering(birdieToTarget, steeringMaxAttacking);
        if (hitPlayer)
        {
            enemyAttackTokenPool.ReturnToken(this.type, token);
            return enemyState.returningState;
        }
        else
        {
            return enemyState.attackingState;
        }
    }

    private enemyState ReturnUp()
    {
        hitPlayer = false;
        if (this.old_velocity.normalized.y > 0.99)
        {
            Debug.Log("Finished Return");
            return enemyState.preparingState;
        }
        moveWithSteering(Vector3.up, steeringMaxReturning);
        return enemyState.returningState;
    }

    private enemyState Dodge()
    {
        Vector3 currentDirection = Vector3.Normalize(this.old_velocity);
        Vector3 idealDirection = Vector3.zero;
        float idealDistance = 0;

        //Note this generation of options should probably be changed
        List<Vector3> possibleDirections = new List<Vector3>();
        possibleDirections.Add(Vector3.forward);
        possibleDirections.Add(Vector3.back);
        possibleDirections.Add(Vector3.left);
        possibleDirections.Add(Vector3.right);
        possibleDirections.Add(Vector3.up);
        possibleDirections.Add(Vector3.down);

        RaycastHit hit;
        foreach (Vector3 option in possibleDirections)
        {
            Physics.Raycast(this.transform.position, option, out hit, Mathf.Infinity, LayerMask.GetMask("Default"));
            if (hit.distance > idealDistance)
            {
                idealDistance = hit.distance;
                idealDirection = option;
            }
        }
        moveWithSteering(idealDirection, steeringMaxDodging);

        //NOT IMPLEMENTED YET
        return enemyState.wanderingState;
    }

    public override void Die()
    {
        playerCamera.gameObject.GetComponent<ScoreTracker>().ChangeScore(scoreValue, transform.position);
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
            hitPlayer = true;
        }
    }
}
