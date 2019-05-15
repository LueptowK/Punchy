using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityScript.Steps;

public class CowardlyWizardEnemyController : EnemyController
{
    private enum enemyState { movingState, tetheredState };
    private enemyState state;
    private TethersTracker tetherTracker;
    private GameObject tether;
    private float tetherRadius;
    //EnemyAttackTokenPool.Token token;
    float defaultSpeed;
    float runAwayDistance;
    Vector3 destination;
    private List<GameObject> energyBalls = new List<GameObject>();
    EnemyAttackTokenPool.Token token;
    [SerializeField] float firingFrequency;
    [SerializeField] float firingFrequencyRange;
    [SerializeField] float fireWindupTime;
    [SerializeField] int scoreValue;
    private float timeToNextFire;
    private float fireTimer;
    bool firing;
    bool dead;
    [SerializeField] private GameObject energyBallPrefab;
    NavMeshHit closestHit;
    [SerializeField] float bulletSpeed;
    float bulletForce;
    [SerializeField] float bulletDamage;
    [SerializeField] private float energyBallDuration;


    // Start is called before the first frame update
    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        defaultSpeed = nav.speed;
        tetherTracker = player.gameObject.GetComponentInChildren<TethersTracker>();
        enemyAttackTokenPool = player.GetComponentInChildren<EnemyAttackTokenPool>();
        type = SpawnManager.EnemyType.CowardlyWizard;
        state = enemyState.movingState;
        tether = this.findBestTether();
        destination = FindNewPositionInTether();
        tetherRadius = tether.GetComponent<TetherController>().Radius;
        dead = false;
        runAwayDistance = 20f;
        bulletForce = 0f;

        //this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 3);

        //if (NavMesh.SamplePosition(this.transform.position, out closestHit, 500, 0)) {
        //    this.transform.position = closestHit.position;
        //}
        //else Debug.LogError("Could not place CowardlyWizardEnemy on the navmesh");
    }

    protected override void Update()
    {
        if (!firing) fireTimer += Time.deltaTime;
        UpdateEnergyBalls();
    }

    // Update is called once per frame
    protected override void FixedUpdate()
    {
        if (dead)
        {
            //stateTimer += Time.unscaledDeltaTime;
            //if (stateTimer >= maxDeadTime)
            //{
            //    Destroy(this.gameObject);
            //}
        }

        if (!dead && nav.enabled)
        {
            //nav.SetDestination(player.transform.position);
            //stateTimer += Time.unscaledDeltaTime;
            switch (state)
            {
                case enemyState.movingState:
                    state = MoveToTether();
                    CheckIfFiring();
                    break;
                case enemyState.tetheredState:
                    state = TetheredBehavior();
                    CheckIfFiring();
                    break;
            }
        }
    }

    private enemyState MoveToTether()
    {
        //PROBLEM: tether = this.findBestTether();
        // check if position is in radius of tether

        nav.speed = defaultSpeed * 1.5f; // run faster when trying to get to tether
        if (Vector3.Distance(tether.transform.position, this.transform.position) <= tetherRadius)
        {
            //stateTimer = 0;
            return enemyState.tetheredState;
        }
        else
        {
            nav.SetDestination(destination);
            return enemyState.movingState;
        }
    }

    private enemyState TetheredBehavior()
    {
        nav.speed = defaultSpeed;
        //if (fireTimer >= firingFrequency) FireProjectile();
        // if player gets too close re-evaluate or wait the appropriate amount of time to evaluate next tether
        if (playerTooClose) //|| stateTimer >= reevaluateTetherTime)
        {
            GameObject newTether = this.findBestTether();
            if (newTether == tether)
            {
                nav.SetDestination(this.FindNewPositionInTether());
                return enemyState.tetheredState;
            }
            else
            {
                tether = newTether;
                tetherRadius = tether.GetComponent<TetherController>().Radius;
                destination = FindNewPositionInTether();
                return enemyState.movingState;
            }
        }
        return enemyState.tetheredState;
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
            weights[i] += (int)(Vector3.Distance(tethers[i].gameObject.transform.position, gameObject.transform.position) / 4);

            //We want distance from tether to player to be 60, so weight the difference of actual distance from that by difference*2
            weights[i] += (int)(Mathf.Abs(
                Vector3.Distance(tethers[i].gameObject.transform.position, player.transform.position) - 60)) * 2;

            //Weight inverse of trace ratio at inverseRatio*50. Perfect visibility weights 0, no visibility weights 50;
            weights[i] += (int)((1 - tethers[i].TraceRatio) * 50);

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

    private void CheckIfFiring()
    {
        fireTimer += Time.fixedDeltaTime;
        if (!firing && fireTimer >= timeToNextFire && CheckLineOfSight())
        { 
            if (TryAttack(0)) // only one kind of attack
            {
                firing = true;
                fireTimer = 0;
            }
        }

        if (firing)
        {
            //stopgap to prevent firing into walls - cylinder will hold token indefinitely if it can never see player
            if (fireTimer >= fireWindupTime && CheckLineOfSight())
            {
                //audioSource.PlayOneShot(firingSound, 0.8f);
                FireProjectile();
            }
        }
    }

    private bool TryAttack(int attackType)
    {
        token = enemyAttackTokenPool.RequestToken(this.gameObject, attackType);
        return (token != null);
    }

    private void EndAttack()
    {
        //enemyAttackTokenPool.ReturnToken(this.type, token);
        token = null;
        firing = false;
        timeToNextFire = firingFrequency + Random.Range(-firingFrequencyRange, firingFrequencyRange);
        fireTimer = 0;
    }

    private void FireProjectile()
    {
        fireTimer = 0;
        transform.LookAt(player.gameObject.transform.position);
        GameObject energyBall = Instantiate(energyBallPrefab, this.transform.position, this.transform.rotation);
        energyBall.GetComponent<EnergyBallProjectileController>().Fire(this.transform.position, player.transform.position, bulletSpeed, bulletDamage, bulletForce, token, enemyAttackTokenPool);
        energyBalls.Add(energyBall);
        EndAttack();

    }

    private void UpdateEnergyBalls()
    {
        EnergyBallProjectileController projectileController;
        if (energyBalls.Count == 0) return;
        foreach (GameObject ball in energyBalls)
        {
            projectileController = ball.GetComponent<EnergyBallProjectileController>();
            if (projectileController.timer > energyBallDuration)
            {
                Destroy(ball);
            }
            projectileController.UpdateTrajectory(player.transform.position);

        }
    }

    public override void takeDamage(Vector3 point)
    {
        Die();
    }

    private void Die()
    {
        playerCamera.gameObject.GetComponent<ScoreTracker>().ChangeScore(scoreValue, transform.position);

        foreach (GameObject ball in energyBalls)
        {
            ball.GetComponent<EnergyBallProjectileController>().Die();
        }
        Destroy(this.gameObject);
        //DestroyThis();
    }


    // might want to consider refactoring CylinderEnemyController and CowardlyWizardEnemyController into a parent class
    // since I'm duplicating a lot of the cylinder behavior


    // currently, tokens aren't working. Enemy is firing too often and too many balls are on the field at once
}
