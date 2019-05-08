using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPredictor : MonoBehaviour
{
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public static Vector3 FirstOrderIntercept(
        Vector3 shooterPosition,
        Vector3 shooterVelocity,
        float shotSpeed,
        Vector3 targetPosition,
        Vector3 targetVelocity
        )
    {
        Vector3 relativeVelocity = targetVelocity - shooterVelocity;
        Vector3 relativePosition = targetPosition - shooterPosition;
        float time = FirstOrderInterceptTime(shotSpeed, relativePosition, relativeVelocity);
        Debug.Log("intercept time is " + time);
        return targetPosition + time * relativeVelocity;

    }

    public static Vector3 DumbFirstOrderIntercept(Vector3 shooterPosition,
        Vector3 shooterVelocity,
        float shotSpeed,
        Vector3 targetPosition,
        Vector3 targetVelocity)
    {
        float distance = Vector3.Distance(targetPosition, shooterPosition);
        float time = distance / shotSpeed;
        return targetPosition + time * targetVelocity;
    }

    public static float FirstOrderInterceptTime(
        float shotSpeed,
        Vector3 relativePosition,
        Vector3 relativeVelocity
        )
    {
        float vSquared = relativeVelocity.sqrMagnitude;
        if (vSquared < 0.001f) { return 0.0f; } //No relative velocity, not sure why returns 0

        // a of quadratic eq
        float vDiffSquared = vSquared - shotSpeed * shotSpeed;
        // b of quadratic eq
        float b = 2f * Vector3.Dot(relativeVelocity, relativePosition);
        // c of quadratic eq
        float c = relativePosition.sqrMagnitude;

        //velocity too similar
        if (Mathf.Abs(vDiffSquared) < 0.001f)
        {
            float time = -c / b; //not sure what this is doing
            return Mathf.Max(time, 0f); //can't have negative time
        }

        //Solving a quadratic equation
        float determinant = b * b - 4 * vDiffSquared * c;
        if (determinant > 0f) //two root! 
        {
            float root1 = (-b + determinant) / (2f * vDiffSquared);
            float root2 = (-b - determinant) / (2f * vDiffSquared);

            //return the smallest positive root of equation. If there is none, return 0
            if (root1 > 0)
                if (root2 > 0)
                    return Mathf.Min(root1, root2);
                else
                    return root1;
            else
                return Mathf.Max(root2, 0f);
        }
        else if (determinant == 0f) //no root, cant solve. wowow
        {
            return 0f;
        }
        else //one root
        {
            float root = -b / (2f * vDiffSquared);
            // return if positive. if not return 0
            return Mathf.Max(root, 0f);
        }
    }
}
