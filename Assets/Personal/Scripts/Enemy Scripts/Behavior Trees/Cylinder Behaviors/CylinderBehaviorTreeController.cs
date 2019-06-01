using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylinderBehaviorTreeController : EnemyController
{
    BehaviorNode root;
    BehaviorNode currentNode;
    // Start is called before the first frame update
    protected override void Start()
    {
        Dictionary<string, object> treeContext = new Dictionary<string, object>();
        treeContext.Add("mover", this);

        root = new CylinderRootNode(null, treeContext);
        currentNode = root;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }
}
