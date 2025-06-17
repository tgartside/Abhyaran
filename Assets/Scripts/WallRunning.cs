using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunning : MonoBehaviour
{
    [Header("Wallrunning Variables")]
    public LayerMask wall;
    public LayerMask ground;
    public float wallRunForce;
    public float wallJumpUpForce;
    public float wallJumpSideForce;
    public float maxWallRunTime;
    public float wallRunTimer;
    public bool canWallRun;

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

    [Header("Exiting")]
    private bool exitingWall;
    public float exitWallTime;
    private float exitWallTimer;

    [Header("Graavity")]
    public bool useGravity;
    public float gravityCounterforce;

    [Header("References")]
    public Transform orientation;
    private PlayerMoveScript pm;
    private Rigidbody rb;
    public CustomGravity gravity;
    public PlayerCam cam;

    private void Awake()
    {
        cam.DoFov(80f);
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMoveScript>();
        gravity = GetComponent<CustomGravity>();
        canWallRun = true;
    }

    private void Update()
    {
        if (pm.isGrounded)
        {
            canWallRun = true;
        }
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
        if ((wallLeft || wallRight) && verticalInput > 0 && AboveGround() && !exitingWall && canWallRun)
        {
            if (!pm.wallrunning)
            {
                StartWallRun();
            }

            // Wallrun timer
            if(wallRunTimer > 0)
            {
                wallRunTimer -= Time.deltaTime;
            }

            if(wallRunTimer <= 0)
            {
                exitingWall = true;
                canWallRun = false;
                exitWallTimer = exitWallTime;
            }

            // Wall jump
            if (Input.GetButtonDown("Jump"))
            {
                WallJump();
            }
        }

        // State 2 - Exiting
        else if (exitingWall)
        {
            if (pm.wallrunning)
            {
                StopWallRun();
            }

            if(exitWallTimer > 0)
            {
                exitWallTimer -= Time.deltaTime;
            }

            if(exitWallTimer <= 0)
            {
                exitingWall = false;
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
        wallRunTimer = maxWallRunTime;

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // Apply camera efects
        cam.DoFov(90f);
        if (wallLeft) cam.DoTilt(-5f);
        if (wallRight) cam.DoTilt(5f);
    }

    private void WallRunningMovement()
    {
        if (useGravity)
        {
            gravity.gravityScale = 2f;
        }
        else 
        {
            gravity.gravityScale = 0f;
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        }

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

        // Weaken gravity
        if (useGravity)
        {
            rb.AddForce(transform.up * gravityCounterforce, ForceMode.Force);
        }
    }

    private void StopWallRun()
    {
        pm.wallrunning = false;

        // Reset camera effects
        cam.DoFov(80f);
        cam.DoTilt(0f);
    }

    private void WallJump()
    {
        // Enter exiting wall state
        exitingWall = true;
        exitWallTimer = exitWallTime;

        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;

        Vector3 forceToApply = transform.up * wallJumpUpForce + wallNormal * wallJumpSideForce;

        // Add force
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(forceToApply, ForceMode.Impulse);

        if (Mathf.Abs(pm.desiredMoveSpeed - pm.lastDesiredMoveSpeed) > 6f && pm.moveSpeed != 0)
        {
            StopAllCoroutines();
            StartCoroutine(pm.SmoothlyLerpMoveSpeed());
        }

        else
        {
            pm.moveSpeed = pm.desiredMoveSpeed;
        }

    }
}
