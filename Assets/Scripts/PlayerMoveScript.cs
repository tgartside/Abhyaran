using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMoveScript : MonoBehaviour
{

    [Header("Movement Variables")]
    public float maxSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public float groundAcceleration;
    public float groundDrag;
    public float moveSpeed;

    public float desiredMoveSpeed;
    public float lastDesiredMoveSpeed;

    public float airWalkSpeed;
    public float airSprintSpeed;
    public float airAcceleration;
    public float airDrag;

    public float wallrunSpeed;

    public float swingSpeed;

    public Transform orientation;

    [Header("Sliding Variables")]
    public float slideSpeed;
    public float speedIncreaseMultiplier;
    public float slopeIncreaseMultiplier;

    private float horizontalInput;
    private float verticalInput;
    private Vector3 moveDirection;
    private Rigidbody rb;

    [Header("Jump Variables")]
    public float jumpForce = 5f;
    public float fallMultiplier = 5f;
    public float lowJumpFallMultiplier = 7f;
    public float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;
    public CustomGravity gravity;
    public Transform jumpCheckPosition;
    public bool isGrounded = true;

    [Header("Crouching Variables")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;

    [Header("Keybinds")]
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Slope Handling")]
    public float playerHeight;
    public float maxSlopeAngle;
    private RaycastHit slopeHit;

    public MovementState state;
    public enum MovementState
    {
        walkingGround,
        sprintingGround,
        walkingAir,
        sprintingAir,
        crouching,
        sliding,
        wallrunning,
        grappling
    }

    public bool sprinting;
    public bool sliding;
    public bool wallrunning;
    public bool grappling;

    // Start is called before the first frame update
    void Start()
    {
        gravity = GetComponent<CustomGravity>();
        rb = GetComponent<Rigidbody>();
        rb.rotation = Quaternion.Euler(0, 90, 0);
        rb.freezeRotation = true;

        startYScale = transform.localScale.y;
        desiredMoveSpeed = 0f;
        moveSpeed = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        // Check if Grounded
        if (Physics.OverlapSphere(jumpCheckPosition.position, 0.2f, LayerMask.GetMask("Ground")).Length > 0)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

        // Start jump buffer
        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        // Start crouching
        if (Input.GetKeyDown(crouchKey) && !sprinting)
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        // Stop crouching
        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }

        if (state == MovementState.sprintingGround)
        {
            sprinting = true;
        }
        else
        {
            sprinting = false;
        }

        SpeedControl();
        StateHandler();

    }

    void FixedUpdate()
    {
        if (!grappling)
        {
            MovePlayer();
            Jump();
            if (!isGrounded && !wallrunning)
            {
                FallMultiplier();
            }
        }
    }

    private void StateHandler()
    {
        // Grappling
        if (grappling)
        {
            state = MovementState.grappling;
            desiredMoveSpeed = swingSpeed;
            rb.drag = 0f;
        }

        // Wallrunning
        else if (wallrunning)
        {
            state = MovementState.wallrunning;
            desiredMoveSpeed = wallrunSpeed;
        }

        // Sliding
        else if (sliding)
        {
            state = MovementState.sliding;

            if(OnSlope() && rb.velocity.y < 0.1f)
            {
                desiredMoveSpeed = slideSpeed;
            }

            else
            {
                desiredMoveSpeed = sprintSpeed;
            }
        }
        // Crouching
        else if (Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
            desiredMoveSpeed = crouchSpeed;
        }

        // Sprinting on ground
        else if (isGrounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprintingGround;
            desiredMoveSpeed = sprintSpeed;
        }

        // Walking on ground
        else if (isGrounded)
        {
            state = MovementState.walkingGround;
            desiredMoveSpeed = walkSpeed;
        }

        // Sprintng in air
        else if(Input.GetKey(sprintKey))
        {
            state = MovementState.sprintingAir;
            desiredMoveSpeed = airSprintSpeed;
        }

        // Walking in air
        else
        {
            state = MovementState.walkingAir;
            desiredMoveSpeed = airWalkSpeed;
        }

        // Check if desiredMoveSpeed has changed drastically
        if(Mathf.Abs(desiredMoveSpeed - lastDesiredMoveSpeed) > 12f && moveSpeed != 0)
        {
            StopAllCoroutines();
            StartCoroutine(SmoothlyLerpMoveSpeed());
        }

        else
        {
            moveSpeed = desiredMoveSpeed;
        }

        lastDesiredMoveSpeed = desiredMoveSpeed;

    }

    public IEnumerator SmoothlyLerpMoveSpeed()
    {
        // Smoothly lerp movementSpeed to desired value
        float time = 0;
        float diff = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        while (time < diff)
        {
            if (!Input.anyKey)
            {
                time = diff;
                rb.drag = 500;

            }
            else
            {
                moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / diff);

                if (OnSlope())
                {
                    float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                    float slopeAngleIncrease = 1 + (slopeAngle / 90f);

                    time += Time.deltaTime * speedIncreaseMultiplier * slopeIncreaseMultiplier * slopeAngleIncrease;
                }
                else
                {
                    time += Time.deltaTime * speedIncreaseMultiplier;
                }
                yield return null;
            }
        }
        moveSpeed = desiredMoveSpeed;
    }

    private void MovePlayer()
    {

        if (grappling)
        {
            return;
        }

        // Calculate player movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        gravity.gravityScale = 2;

        // Player is grounded
        if (isGrounded)
        {
            // Player is on a slope
            if (OnSlope())
            {
                rb.AddForce(GetSlopeMoveDirection(moveDirection) * moveSpeed * 10f, ForceMode.Force);

                gravity.gravityScale = 0;
                rb.drag = groundDrag;
            }
            else
            {
                rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
                rb.drag = groundDrag;
                gravity.gravityScale = 2f;
            }
            
        }
        // Player is in the air
        else if (!isGrounded) 
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
            rb.drag = airDrag;
        }

    }

    private void Jump()
    {
        if (isGrounded)
        {
            if (jumpBufferCounter > 0f)
            {
                rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
                rb.AddForce(new Vector3(0f, jumpForce, 0f), ForceMode.Impulse);
            }
        }
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // Cap max velocity
        if(flatVel.magnitude > maxSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * maxSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void FallMultiplier()
    {
        if(rb.velocity.y < 0)
        {
            gravity.gravityScale = fallMultiplier;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            gravity.gravityScale = lowJumpFallMultiplier;
        }
        else
        {
            gravity.gravityScale = 2f;
        }
    }

    public bool OnSlope()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }

}
