using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeContextObject
{
    public BehaviorNode CurrentlyRunning
    {
        get
        {
            return currentlyRunning;
        }

        set
        {
            currentlyRunning = value;
        }
    }
    public GameObject Actor
    {
        get
        {
            return actor;
        }

        set
        {
            actor = value;
        }
    }
    public GameObject Player
    {
        get
        {
            return player;
        }

        set
        {
            player = value;
        }
    }
    public Transform Transform
    {
        get
        {
            return transform;
        }

        set
        {
            transform = value;
        }
    }
    public BehaviorNode Root
    {
        get
        {
            return root;
        }
        set
        {
            root = value;
        }
    }

    private BehaviorNode root;
    private BehaviorNode currentlyRunning;
    private GameObject actor;
    private GameObject player;
    private Transform transform;
}
