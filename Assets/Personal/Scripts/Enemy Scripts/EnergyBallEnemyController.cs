using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityScript.Steps;

public class SphereEnemyController : EnemyController
{
    //private enum enemyState { movingState, tetheredState };
    //private enemyState state;
    //private TethersTracker tetherTracker;
    //private GameObject tether;
    //private float tetherRadius;
    //EnemyAttackTokenPool.Token token;
    //float defaultSpeed;
    //float runAwayDistance;

    //[SerializeField] float firingFrequency;
    //[SerializeField] float firingFrequencyRange;
    //private float timeToNextFire;
    //private float fireTimer;
    //bool firing;
    //[SerializeField] private GameObject energyBallPrefab;


    //void Start()
    //{
    //    nav = GetComponent<NavMeshAgent>();
    //    defaultSpeed = nav.speed;
    //    tetherTracker = player.gameObject.GetComponentInChildren<TethersTracker>();
    //    enemyAttackTokenPool = player.GetComponentInChildren<EnemyAttackTokenPool>();
    //    type = SpawnManager.EnemyType.Sphere;
    //}

    //void Update()
    //{
        
    //}


    //private Vector3 FindNewPositionInTether()
    //{
    //    return tether.transform.position + (Vector3)Random.insideUnitCircle * tetherRadius;
    //}

    //private bool TryAttack(int attackType)
    //{
    //    token = enemyAttackTokenPool.RequestToken(this.gameObject, attackType);
    //    return (token != null);
    //}

    //private void EndAttack()
    //{
    //    enemyAttackTokenPool.ReturnToken(this.type, token);
    //    token = null;
    //    firing = false;
    //    timeToNextFire = firingFrequency + Random.Range(-firingFrequencyRange, firingFrequencyRange);
    //    fireTimer = 0;
    //}

    //private void FireProjectile()
    //{
    //    transform.LookAt(player.gameObject.transform.position);
    //    GameObject energyBall = Instantiate(energyBallPrefab, this.transform.position, this.transform.rotation);
    //    //bullet.GetComponent<Projectile>().Fire(this.transform.position, player.transform.position, bulletSpeed, bulletDamage, bulletForce);
    //    // creating the energyBall will be different than bullet probably
    //    EndAttack();
    //}

    //private bool playerTooClose
    //{
    //    get
    //    {
    //        return (Vector3.Distance(player.transform.position, this.transform.position) < runAwayDistance);
    //    }
    //}

    //private GameObject findBestTether()
    //{
    //    //4 data points to weigh
    //    //Sight Ratio (visibility)
    //    //Distance from player - probably want around 60 units of distance
    //    //Distance from tether
    //    //Current occupants of tether

    //    TetherController[] tethers = tetherTracker.Tethers;
    //    int[] weights = new int[tethers.Length];
    //    int minWeightIndex = -1;
    //    int minWeight = int.MaxValue;

    //    for (int i = 0; i < tethers.Length; i++)
    //    {
    //        weights[i] += 10 * tethers[i].Occupants ^ 2;
    //        //weight distance from tether to AI as distance/4
    //        weights[i] += (int)(Vector3.Distance(tethers[i].gameObject.transform.position, gameObject.transform.position) / 4);

    //        //We want distance from tether to player to be 60, so weight the difference of actual distance from that by difference*2
    //        weights[i] += (int)(Mathf.Abs(
    //            Vector3.Distance(tethers[i].gameObject.transform.position, player.transform.position) - 60)) * 2;

    //        //Weight inverse of trace ratio at inverseRatio*50. Perfect visibility weights 0, no visibility weights 50;
    //        weights[i] += (int)((1 - tethers[i].TraceRatio) * 50);

    //        if (weights[i] < minWeight)
    //        {
    //            minWeightIndex = i;
    //            minWeight = weights[i];
    //        }
    //    }

    //    if (tether != null)
    //    {
    //        tether.GetComponent<TetherController>().incrementOccupantsBy(-1);
    //    }
    //    tethers[minWeightIndex].incrementOccupantsBy(1);

    //    return tethers[minWeightIndex].gameObject;
    //}


}
