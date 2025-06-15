using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMoveScript : MonoBehaviour
{

    private Rigidbody rb;
    private bool jumpPressed = false;

    public float jumpForce = 5;
    public float maxSpeed = 5;
    public float rotationSpeed = 5;
    public Transform jumpCheckPosition;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            jumpPressed = true;
        }
    }

    void FixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 newVelocity = transform.forward * vertical * maxSpeed;
        newVelocity.y = rb.velocity.y;
        rb.velocity = newVelocity;

        if (jumpPressed && Physics.OverlapSphere(jumpCheckPosition.position, 0.05f, LayerMask.GetMask("Ground")).Length> 0)
        {
            jumpPressed = false;
            rb.AddForce(new Vector3(0f, jumpForce, 0f), ForceMode.Impulse);
        }

        transform.Rotate(Vector3.up, horizontal * rotationSpeed * Time.fixedDeltaTime, 0f);
    }
}
