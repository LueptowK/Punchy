using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityScript.Steps;

public class BirdieEnemyController : EnemyController
{
    private enum enemyState { movingState, divingState, attackingState };
    private enemyState state;
    private TethersTracker tetherTracker;
    private GameObject tether;
    private float tetherRadius;
    //EnemyAttackTokenPool.Token token;
    float runAwayDistance;
    Vector3 destination;
    EnemyAttackTokenPool.Token token;
    [SerializeField] float steeringForce;
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
        tetherTracker = player.gameObject.GetComponentInChildren<TethersTracker>();
        enemyAttackTokenPool = player.GetComponentInChildren<EnemyAttackTokenPool>();
        type = SpawnManager.EnemyType.Birdie;
        state = enemyState.movingState;
        tether = this.findBestTether();
        destination = FindNewPositionInTether();
        tetherRadius = tether.GetComponent<TetherController>().Radius;
        dead = false;
        runAwayDistance = 20f;
        stateTimer = 0f;
        reevaluateTetherTime = 0f;
    }

    // Update is called once per frame
    protected override void FixedUpdate()
    {
        if (!dead)
        {
            stateTimer += Time.unscaledDeltaTime;
            switch (state)
            {
                case enemyState.movingState:
                    state = MoveToPlayer();
                    break;
            }
        }
    }

    private enemyState MoveToPlayer()
    {
        Vector3 birdieToAboveTarget = player.transform.position + Vector3.up * 10 - this.transform.position;
        Vector3 xzBirdieToAboveTarget = new Vector3(birdieToAboveTarget.x, 0, birdieToAboveTarget.z);
        Vector3 birdieToClosestAttackPoint = birdieToAboveTarget - 10 * Vector3.Normalize(xzBirdieToAboveTarget);
        if (Vector3.Magnitude(birdieToClosestAttackPoint) < defaultSpeed)
        {
            Debug.Log("Close to strike!");
        }
        else
        {
            Vector3 desiredDirection = Vector3.Normalize(birdieToClosestAttackPoint);
            Vector3 steeringFull = desiredDirection - Vector3.Normalize(this.old_velocity);
            Vector3 steeringLimited = Vector3.ClampMagnitude(steeringFull, steeringForce);

            Vector3 velocity = defaultSpeed * (Vector3.Normalize(Vector3.Normalize(this.old_velocity) + steeringLimited));
            this.old_velocity = velocity;
            this.transform.rotation = Quaternion.LookRotation(velocity);
            enemyMover.Move(velocity);
        }

        return enemyState.movingState;
    }

    private Vector3 FindNewPositionInTether()
    {
        return tether.transform.position + (Vector3)Random.insideUnitCircle * tetherRadius;
    }

    private bool playerTooClose
    {
        get
        {
            return (Vector3.Distance(player.transform.position, this.transform.position) < runAwayDistance);
        }
    }

    private GameObject findBestTether()
    {
        //4 data points to weigh
        //Sight Ratio (visibility)
        //Distance from player - probably want around 60 units of distance
        //Distance from tether
        //Current occupants of tether

        TetherController[] tethers = tetherTracker.Tethers;
        int[] weights = new int[tethers.Length];
        int minWeightIndex = -1;
        int minWeight = int.MaxValue;

        for (int i = 0; i < tethers.Length; i++)
        {
            weights[i] += 10 * tethers[i].Occupants ^ 2;
            //weight distance from tether to AI as distance/4
            int distanceToTether = (int)(Vector3.Distance(tethers[i].gameObject.transform.position, gameObject.transform.position) / 4);
            weights[i] += distanceToTether;

            //We want distance from tether to player to be 60, so weight the difference of actual distance from that by difference*2
            weights[i] += (int)(Mathf.Abs(
                Vector3.Distance(tethers[i].gameObject.transform.position, player.transform.position) - 60)) * 2;

            //Weight trace ratio at Ratio*50. Perfect visibility weights 50, no visibility weights 0;
            weights[i] += (int)((tethers[i].TraceRatio) * 50);

            if (weights[i] < minWeight)
            {
                minWeightIndex = i;
                minWeight = weights[i];              
            }
        }

        if (tether != null)
        {
            tether.GetComponent<TetherController>().incrementOccupantsBy(-1);
        }
        tethers[minWeightIndex].incrementOccupantsBy(1);

        return tethers[minWeightIndex].gameObject;
    }

    public override void Die()
    {
        playerCamera.gameObject.GetComponent<ScoreTracker>().ChangeScore(scoreValue, transform.position);

        //foreach (GameObject ball in energyBalls)
        //{
        //    ball.GetComponent<EnergyBallProjectileController>().Die();
        //}
        //Destroy(this.gameObject);
        DestroyThis(); // Tells the spawnmanager to delete
    }

    // might want to consider refactoring CylinderEnemyController and CowardlyWizardEnemyController into a parent class
    // since I'm duplicating a lot of the cylinder behavior

    // should make it unable to spawn projectile if too close to player
    // should never choose a tether past the player
}
