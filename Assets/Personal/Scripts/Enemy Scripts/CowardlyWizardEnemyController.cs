﻿using System.Collections;
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
    [SerializeField] float bulletDamage;
    [SerializeField] private int maxNumberOfEnergyBalls;
    [SerializeField] float bulletSpeed;
    [SerializeField] private GameObject energyBallPrefab;
    [SerializeField] private GameObject energyBallsParent;
    private float timeToNextFire;
    private float fireTimer;
    private float stateTimer;
    private float reevaluateTetherTime;
    bool firing;
    bool dead;
    NavMeshHit closestHit;
    float bulletForce;
 


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

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
        stateTimer = 0f;
        reevaluateTetherTime = 2f;

       
        //this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 3);

        //if (NavMesh.SamplePosition(this.transform.position, out closestHit, 500, 0)) {
        //    this.transform.position = closestHit.position;
        //}
        //else Debug.LogError("Could not place CowardlyWizardEnemy on the navmesh");
    }

    protected override void Update()
    {
        if (!firing) fireTimer += Time.deltaTime;
        energyBalls.RemoveAll(ball => ball == null);// lambda expression, removes all items in list that are null
        //UpdateEnergyBalls();
    }

    // Update is called once per frame
    protected override void FixedUpdate()
    {
        if (dead)
        {
            // handled by TakeDamage and Die() because one hit kills
        }

        if (!dead && nav.enabled)
        {
            stateTimer += Time.unscaledDeltaTime;
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
        nav.speed = defaultSpeed * 1.5f; // run faster when trying to get to tether
        if (Vector3.Distance(tether.transform.position, this.transform.position) <= tetherRadius)
        {
            stateTimer = 0;
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
        if (playerTooClose || stateTimer >= reevaluateTetherTime)
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
        if (Vector3.Distance(this.transform.position, player.transform.position) < 10f) return false; // don't fire when right next to player
        if (energyBalls.Count >= maxNumberOfEnergyBalls) return false;
        token = enemyAttackTokenPool.RequestToken(this.gameObject, attackType);
        return (token != null);
    }

    private void EndAttack()
    {
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
        energyBall.transform.parent = energyBallsParent.transform;
        energyBalls.Add(energyBall);
        energyBall.GetComponent<EnergyBallProjectileController>().Initialize(this.gameObject, player, enemyAttackTokenPool, energyBallsParent);
        energyBall.GetComponent<EnergyBallProjectileController>().Fire(bulletSpeed, bulletDamage, bulletForce, token);
        EndAttack();
    }

    private void UpdateEnergyBalls()
    {
       
        //EnergyBallProjectileController projectileController;
        //foreach (GameObject ball in energyBalls)
        //{
        //    if (ball == null)
        //    {
        //        energyBalls.Remove(ball); // remove balls that no longer exist
        //    }
            //projectileController = ball.GetComponent<EnergyBallProjectileController>();
            //if (projectileController.Timer > energyBallDuration)
            //{
            //    projectileController.Die();
            //    energyBalls.Remove(ball); // might not be necessary?
            //    return;
            //}
            //projectileController.UpdateTrajectory(player.transform.position);

        //}
    }

    public override void takeDamage(Vector3 point)
    {
        Die();
    }

    private void Die()
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
