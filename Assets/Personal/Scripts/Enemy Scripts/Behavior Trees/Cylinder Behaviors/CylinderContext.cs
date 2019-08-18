using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CylinderContext : TreeContextObject
{
    private float firingFrequency;
    private float firingFrequencyRange;
    private float fireWindupTime;
    private float runAwayDistance;
    private GameObject tether;
    private TethersTracker tethersTracker;
    private float tetherRadius;
    private float reevaluateTetherTime;
    private NavMeshAgent nav;
    private Vector3 destination;

    public float FiringFrequency
    {
        get
        {
            return firingFrequency;
        }

        set
        {
            firingFrequency = value;
        }
    }
    public float FiringFrequencyRange
    {
        get
        {
            return firingFrequencyRange;
        }

        set
        {
            firingFrequencyRange = value;
        }
    }
    public float FireWindupTime
    {
        get
        {
            return fireWindupTime;
        }

        set
        {
            fireWindupTime = value;
        }
    }
    public float RunAwayDistance
    {
        get
        {
            return runAwayDistance;
        }

        set
        {
            runAwayDistance = value;
        }
    }
    public GameObject Tether
    {
        get
        {
            return tether;
        }

        set
        {
            tether = value;
        }
    }

    public TethersTracker TethersTracker
    {
        get
        {
            return tethersTracker;
        }

        set
        {
            tethersTracker = value;
        }
    }

    public float TetherRadius
    {
        get
        {
            return tetherRadius;
        }

        set
        {
            tetherRadius = value;
        }
    }

    public float ReevaluateTetherTime
    {
        get
        {
            return reevaluateTetherTime;
        }

        set
        {
            reevaluateTetherTime = value;
        }
    }

    public NavMeshAgent Nav
    {
        get
        {
            return nav;
        }

        set
        {
            nav = value;
        }
    }

    public Vector3 Destination
    {
        get
        {
            return destination;
        }

        set
        {
            destination = value;
        }
    }
}
