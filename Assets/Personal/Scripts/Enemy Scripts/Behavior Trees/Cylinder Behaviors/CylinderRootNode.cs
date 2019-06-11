﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylinderRootNode : BehaviorNode
{
    public CylinderRootNode(BehaviorNode[] nodeChildren, Dictionary<string, object> nodeContext) : base(nodeChildren, nodeContext)
    {
        context = nodeContext;
        context.Add("root", new ContextItem<BehaviorNode>(this));

        
        BehaviorNode attackSelector = new CylinderAttackSelectorNode(
            new BehaviorNode[]
            {
                new CylinderFireballNode(null, nodeContext),
                new CylinderLaserNode(null, nodeContext)
            },
            nodeContext);

        BehaviorNode moveSequence = new SequenceNode(
                new BehaviorNode[]
                {
                    new CylinderCheckDangerNode(new BehaviorNode[]
                    {
                        new CylinderEscapeNode(null, nodeContext)
                    },
                    nodeContext),
                    new CylinderCheckTetherNullNode(new BehaviorNode[]
                    {
                        new CylinderFindTetherNode(null, nodeContext)
                    },
                    nodeContext),
                    new DecoratorAlwaysFail(new BehaviorNode[]
                    {
                        new CylinderMoveToTetherNode(null, nodeContext)
                    }, 
                    nodeContext)
                },
                nodeContext);

        BehaviorNode attackSequence = new SequenceNode(
                new BehaviorNode[]
                {
                    new DecoratorInverter(new BehaviorNode[] 
                    {
                        new CylinderCheckTokenNode(new BehaviorNode[] {attackSelector}, nodeContext)
                    },
                    nodeContext),
                    new CylinderGetTokenNode(new BehaviorNode[] {attackSelector}, nodeContext)
                },
                nodeContext);

        children = new BehaviorNode[] {moveSequence, attackSequence};
    }

    public override void Update()
    {
        base.Update();
    }

    public override statusValues FixedUpdate()
    {
        return base.FixedUpdate();
    }
}
