using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMoveScript : MonoBehaviour
{

    [Header("Movement")]
    public float maxSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public float groundAcceleration;
    public float groundDrag;
    public float airWalkSpeed;
    public float airSprintSpeed;
    public float airAcceleration;
    public float airDrag;
    public Transform orientation;

    private float horizontalInput;
    private float verticalInput;
    private Vector3 moveDirection;
    private Rigidbody rb;

    [Header("Jump Variables")]
    public float jumpForce = 5f;
    public float fallMultiplier = 5f;
    public float lowJumpFallMultiplier = 7f;
    public float jumpBufferTime = 0.2f;
    public float jumpBufferCounter;
    public CustomGravity gravity;
    public Transform jumpCheckPosition;

    [Header("Keybinds")]
    public KeyCode sprintKey = KeyCode.LeftShift;

    private bool isGrounded = true;

    public MovementState state;
    public enum MovementState
    {
        walkingGround,
        sprintingGround,
        walkingAir,
        sprintingAir
    }

    // Start is called before the first frame update
    void Start()
    {
        gravity = GetComponent<CustomGravity>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        if (Physics.OverlapSphere(jumpCheckPosition.position, 0.05f, LayerMask.GetMask("Ground")).Length > 0)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        SpeedControl();
        StateHandler();

    }

    void FixedUpdate()
    {
        MovePlayer();
        if (!isGrounded)
        {
            FallMultiplier();
        }
        
    }

    private void StateHandler()
    {
        // Sprinting on ground
        if(isGrounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprintingGround;
            groundAcceleration = sprintSpeed;
        }

        // Walking on ground
        else if (isGrounded)
        {
            state = MovementState.walkingGround;
            groundAcceleration = walkSpeed;
        }

        // Sprintng in air
        else if(Input.GetKey(sprintKey))
        {
            state = MovementState.sprintingAir;
            airAcceleration = airSprintSpeed;
        }

        // Walking in air
        else
        {
            state = MovementState.walkingAir;
            airAcceleration = airWalkSpeed;
        }
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // Player is grounded
        if (isGrounded)
        {
            rb.AddForce(moveDirection.normalized * groundAcceleration * 10f, ForceMode.Force);
            rb.drag = groundDrag;
            if (jumpBufferCounter > 0f)
            {
                rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
                //jumpPressed = false;
                rb.AddForce(new Vector3(0f, jumpForce, 0f), ForceMode.Impulse);
            }
        }
        // Player is in the air
        else
        {
            rb.AddForce(moveDirection.normalized * airAcceleration * 10f, ForceMode.Force);
            rb.drag = airDrag;
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

}
