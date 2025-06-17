using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    public float sensX;
    public float sensY;

    public Transform orientation;

    public Rigidbody rb;

    private float xRotation;
    private float yRotation;

    private float xRotVel = 0f;
    private float yRotVel = 0f;
    public float smoothTime;

    [SerializeField] private Transform rotHelper;



    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        rotHelper.localRotation = transform.localRotation;
    }

    // Update is called once per frame
    private void Update()
    {
        // get mouse input
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        yRotation += mouseX;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        rotHelper.rotation = Quaternion.Euler(xRotation, yRotation, 0);

        // rotate cam and orientation
        transform.rotation = Quaternion.Euler(Mathf.SmoothDampAngle(transform.eulerAngles.x, rotHelper.eulerAngles.x, ref xRotVel, smoothTime),
                                                Mathf.SmoothDampAngle(transform.eulerAngles.y, rotHelper.eulerAngles.y, ref yRotVel, smoothTime), 0f);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
        rb.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}
