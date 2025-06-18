using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleRope : MonoBehaviour
{
    //public Grapple grapplingGun;
    //public int quality;
    //public float damper;
    //public float strength;
    //public float velocity;
    //public float waveCount;
    //public float waveHeight;
    //public AnimationCurve effectCurve;

    //private Spring spring;
    //private LineRenderer lr;

    //private Vector3 curGrapplePos;

    //private void Awake()
    //{
    //    lr = GetComponent<LineRenderer>();
    //    spring = new Spring();
    //    spring.SetTarget(0);
    //}

    //private void LateUpdate()
    //{
    //    DrawRope();
    //}

    //private void DrawRope()
    //{
    //    // Don't draw if not grappling
    //    if (!grapplingGun.pm.grappling)
    //    {
    //        curGrapplePos = grapplingGun.gunTip.position;
    //        spring.Reset();
    //        if (lr.positionCount > 0)
    //        {
    //            lr.positionCount = 0;
    //        }
    //        return;
    //    }

    //    if (lr.positionCount == 0)
    //    {
    //        spring.SetVelocity(velocity);
    //        lr.positionCount = quality + 1;
    //    }

    //    spring.SetDamper(damper);
    //    spring.SetStrength(strength);
    //    spring.Update(Time.deltaTime);

    //    Vector3 grapplePoint = grapplingGun.GetGrapplePoint();
    //    Vector3 gunTipPos = grapplingGun.gunTip.position;
    //    Vector3 up = Quaternion.LookRotation((grapplePoint - gunTipPos).normalized) * Vector3.up;

    //    curGrapplePos = Vector3.Lerp(curGrapplePos, grapplePoint, Time.deltaTime * 12f);

    //    for (int i = 0; i < quality + 1; i++)
    //    {
    //        float delta = i / quality;
    //        Vector3 offset = up * waveHeight * Mathf.Sin(delta * waveCount * Mathf.PI) * spring.Value * effectCurve.Evaluate(delta);

    //        lr.SetPosition(i, Vector3.Lerp(gunTipPos, curGrapplePos, delta) + offset);
    //    }
    //}
}
