using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylinderCheckReevaluateTether : BehaviorNode
{
    new CylinderContext context;
    float reevaluateTimer;

    public CylinderCheckReevaluateTether(BehaviorNode[] nodeChildren, CylinderContext nodeContext) : base(nodeChildren, nodeContext)
    {
        children = nodeChildren;
        context = nodeContext;
    }

    public override void Update()
    {
        base.Update();
    }

    public override statusValues FixedUpdate()
    {
        reevaluateTimer += Time.fixedDeltaTime;
        if (context.Tether == null || reevaluateTimer >= context.ReevaluateTetherTime)
        {
            reevaluateTimer = 0;
            context.Tether = findBestTether();
        }
        return statusValues.failure;
    }

    private GameObject findBestTether()
    {
        //4 data points to weigh
        //Sight Ratio (visibility)
        //Distance from player - probably want around 20 units of distance
        //Distance from tether
        //Current occupants of tether

        TetherController[] tethers = context.TethersTracker.Tethers;
        int[] weights = new int[tethers.Length];
        int minWeightIndex = -1;
        int minWeight = int.MaxValue;

        for (int i = 0; i < tethers.Length; i++)
        {
            weights[i] += 10 * tethers[i].Occupants ^ 2;
            //weight distance from tether to AI as distance/4
            weights[i] += (int)(Vector3.Distance(tethers[i].gameObject.transform.position, context.Transform.position) / 4);

            //We want distance from tether to player to be 20, so weight the difference of actual distance from that by difference*2
            weights[i] += (int)(Mathf.Abs(
                Vector3.Distance(tethers[i].gameObject.transform.position, context.Player.transform.position) - 20)) * 2;

            //Weight inverse of trace ratio at inverseRatio*50. Perfect visibility weights 0, no visibility weights 50;
            weights[i] += (int)((1 - tethers[i].TraceRatio) * 50);

            //Weight angle between vector from AI to player and vector from player to tether, such that angle over 90 degrees is unfavorable
            float angle = (Vector3.Angle(context.Transform.position - context.Player.transform.position,
                                                tethers[i].gameObject.transform.position - context.Player.transform.position));
            weights[i] += angle > 90 ? (int)angle - 90 : 0;

            if (weights[i] < minWeight)
            {
                minWeightIndex = i;
                minWeight = weights[i];
            }
        }

        if (context.Tether != null)
        {
            context.Tether.GetComponent<TetherController>().incrementOccupantsBy(-1);
        }
        tethers[minWeightIndex].incrementOccupantsBy(1);

        return tethers[minWeightIndex].gameObject;
    }
}
