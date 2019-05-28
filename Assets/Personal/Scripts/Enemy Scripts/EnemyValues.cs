using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class EnemyValues : ActorValues
{
    public GeneralValues generalValues;

    [System.Serializable]
    public class GeneralValues : System.Object
    {
        [SerializeField] private float healthValue;
        [SerializeField] private float impactToDamage;
        [SerializeField] private float knockbackModifier;

        public float HealthValue { get { return healthValue; } }
        public float ImpactToDamage { get { return impactToDamage; } }
        public float KnockbackModifier {  get { return knockbackModifier; } }
    }
}
