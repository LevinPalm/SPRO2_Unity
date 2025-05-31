using UnityEngine;

public class Movement : MonoBehaviour
{
#pragma warning disable 649

    [SerializeField] CharacterController controller;
    [SerializeField] float speed = 11f;

    Vector2 horizontalInput;

    [SerializeField] float jumpHeight = 3.5f;
    bool jump = false;

    [SerializeField] float gravity = -30f;   //-9.81 feels to small
    Vector3 verticalVelocity = Vector3.zero;
    [SerializeField] LayerMask groundMask;
    bool isGrounded;

    private void Update()
    {
        isGrounded = Physics.CheckSphere(transform.position, 0.1f, groundMask);
        if (isGrounded)
        {
            verticalVelocity.y = 0;
        }

        Vector3 horizontalVelocity = (transform.right * horizontalInput.x + transform.forward * horizontalInput.y) * speed ;
        controller.Move(horizontalVelocity * Time.deltaTime);   //scaling with time between frames to make the motion smooth

        //jumping is not cmopletly integrated yet, for later use
        if (jump)
        {
            if (isGrounded)
            {
                verticalVelocity.y = Mathf.Sqrt(-2f * jumpHeight * gravity);
            }
            jump = false;
        }

        verticalVelocity.y += gravity * Time.deltaTime;
        controller.Move(verticalVelocity * Time.deltaTime);
    }

    public void ReceiveInput(Vector2 _horizontalInput)
    {
        horizontalInput = _horizontalInput;
    }

    //not completly implemented but for future use
    public void OnJumpPressed()
    {
        jump = true;
    }
}
