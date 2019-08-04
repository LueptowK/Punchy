using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DifficultyValues : MonoBehaviour
{
    [SerializeField] EnemyAttackTokenPool.EnemyTypeTokens[] tokenPoolsEnemyTypes;
    [SerializeField] SpawnValues spawnDifficultyValues;
    [SerializeField] bool isDefaultValues;

    private void Awake()
    {
        
    }

    public DifficultyValues DestroyOnLoad()
    {
        this.gameObject.DoDestroyOnLoad();
        return this;
    }

    public EnemyAttackTokenPool.EnemyTypeTokens[] TokenPoolsEnemyTypes
    {
        get
        {
            return tokenPoolsEnemyTypes;
        }
    }

    public SpawnValues SpawnDifficultyValues
    {
        get
        {
            return spawnDifficultyValues;
        }
    }

    public bool isDefault
    {
        get
        {
            return isDefaultValues;
        }
    }

    [System.Serializable]
    public class SpawnValues : object
    {
        [SerializeField] int[] baseSpawnLimits;
        [SerializeField] float[] spawnLimitsGrowth;

        public int[] BaseSpawnLimits
        {
            get
            {
                return baseSpawnLimits;
            }
        }

        public float[] SpawnLimitsGrowth
        {
            get
            {
                return spawnLimitsGrowth;
            }
        }
    }
}
