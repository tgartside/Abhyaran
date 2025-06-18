using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapple : MonoBehaviour
{

    [Header("References")]
    public LineRenderer lr;
    public Transform gunTip, cam, player;
    public PlayerMoveScript pm;
    public LayerMask grappleable;

    [Header("Swinging Variables")]
    public float grappleSpring = 4.5f;
    public float grappleDamper = 7f;
    public float grappleMassScale = 4.5f;
    private float maxSwingDistance = 25f;
    private Vector3 swingPoint;
    private Vector3 curGrapplePos;
    private SpringJoint joint;

    [Header("Air Movement Variables")]
    public Transform orientation;
    public Rigidbody rb;
    public float horizontalThrustForce;
    public float forwardThrustForce;
    public float extendRopeSpeed;

    [Header("Input")]
    public KeyCode swingKey = KeyCode.Mouse0;


    private void Update()
    {
        if (Input.GetKeyDown(swingKey)) StartSwing();
        if (Input.GetKeyUp(swingKey)) StopSwing();

        if(joint != null) AirMovement();
    }

    private void LateUpdate()
    {
        DrawRope();
    }

    private void StartSwing()
    {

        pm.grappling = true;

        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxSwingDistance, grappleable))
        {
            swingPoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = swingPoint;

            float distanceFromPoint = Vector3.Distance(player.position, swingPoint);

            // Min and max distance from grapple point
            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.25f;

            joint.spring = grappleSpring;
            joint.damper = grappleDamper;
            joint.massScale = grappleMassScale;

            lr.positionCount = 2;
            curGrapplePos = gunTip.position;
        }
    }

    private void StopSwing()
    {
        pm.grappling = false;
        lr.positionCount = 0;
        Destroy(joint);
    }

    private void DrawRope()
    {
        // Don't draw if not grappling
        if (!joint) {
            return;
        }

        curGrapplePos = Vector3.Lerp(curGrapplePos, swingPoint, Time.deltaTime * 8f);

        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, curGrapplePos);
    }

    private void AirMovement()
    {
        // Move right
        if (Input.GetKey(KeyCode.D))
        {
            rb.AddForce(orientation.right * horizontalThrustForce * Time.deltaTime);
        }

        // Move Left
        if (Input.GetKey(KeyCode.A))
        {
            rb.AddForce(-orientation.right * horizontalThrustForce * Time.deltaTime);
        }

        // Move Forward
        if (Input.GetKey(KeyCode.W))
        {
            rb.AddForce(orientation.forward * horizontalThrustForce * Time.deltaTime);
        }

        // Shorten Rope
        if (Input.GetKey(KeyCode.Space))
        {
            Vector3 directionToPoint = swingPoint - transform.position;
            rb.AddForce(directionToPoint.normalized * forwardThrustForce * Time.deltaTime);

            float distanceFromPoint = Vector3.Distance(transform.position, swingPoint);

            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.25f;
        }

        // Extend Rope
        if (Input.GetKey(KeyCode.S))
        {
            float extendedDistanceFromPoint = Vector3.Distance(transform.position, swingPoint) + extendRopeSpeed;

            joint.maxDistance = extendedDistanceFromPoint * 0.8f;
            joint.minDistance = extendedDistanceFromPoint * 0.25f;
        }
    }

}
