using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformControl : MonoBehaviour
{
    private void OnCollisionEnter(Collision collisionInfo)
    {
        if (collisionInfo.gameObject.CompareTag("MovingPlatform"))
        {
            if(Vector3.Dot(collisionInfo.GetContact(0).normal, Vector3.up) > 0.5f)
            {
                transform.parent = collisionInfo.collider.transform;
            }
        }
    }

    private void OnCollisionExit(Collision collisionInfo)
    {
        if (collisionInfo.gameObject.CompareTag("MovingPlatform"))
        {
            transform.parent = null;
        }
    }
}
