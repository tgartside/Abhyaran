using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunning : MonoBehaviour
{
    [Header("Wallrunning Variables")]
    public LayerMask wall;
    public LayerMask ground;
    public float wallRunForce;
    public float maxWallRunTime;
    public float wallRunTimer;

    [Header("Input")]
    private float horizontalInput;
    private float verticalInput;

    [Header("Detection")]
    public float wallCheckDistance;
    public float minJumpHeight;
    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;
    private bool wallLeft;
    private bool wallRight;

    [Header("References")]
    public Transform orientation;
    private PlayerMoveScript pm;
    private Rigidbody rb;
    public CustomGravity gravity;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMoveScript>();
        gravity = GetComponent<CustomGravity>();
    }

    private void Update()
    {
        CheckForWall();
        StateMachine();
    }

    private void FixedUpdate()
    {
        if (pm.wallrunning)
        {
            WallRunningMovement();
        }
    }

    private void CheckForWall()
    {
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallCheckDistance, wall);
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallCheckDistance, wall);
    }

    private bool AboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, ground);
    }

    private void StateMachine()
    {
        // Getting Inputs
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // State 1 - Wallrunning
        if ((wallLeft || wallRight) && verticalInput > 0 && AboveGround())
        {
            if (!pm.wallrunning)
            {
                StartWallRun();
            }
        }

        // State 3 - None
        else
        {
            if (pm.wallrunning)
            {
                StopWallRun();
            }
        }

    }

    private void StartWallRun()
    {
        pm.wallrunning = true;
    }

    private void WallRunningMovement()
    {
        gravity.gravityScale = 0;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;

        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if ((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude)
        {
            wallForward = -wallForward;
        }

        // Add forward force
        rb.AddForce(wallForward * wallRunForce, ForceMode.Force);

        // Push to wall force
        if (!(wallLeft && horizontalInput > 0) && !(wallRight && horizontalInput < 0))
        {
            rb.AddForce(-wallNormal * 100, ForceMode.Force);
        }
    }

    private void StopWallRun()
    {
        pm.wallrunning = false;
    }
}
