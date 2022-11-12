using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] InputAction moveAction;
    [SerializeField] InputAction sprintAction;
    [SerializeField] InputAction jumpAction;
    [SerializeField] float moveSpeed = 2;
    [SerializeField] float sprintSpeed = 4;
    [SerializeField] float jumpPower = 5;
    [SerializeField] float jumpDuration = 0.5f;

    Rigidbody body;
    Animator animator;

    private void OnEnable()
    {
        moveAction.Enable();
        sprintAction.Enable();
        jumpAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        sprintAction.Disable();
        jumpAction.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        float yaw = moveInput.x;
        Vector3 rotation = transform.eulerAngles;
        rotation.y += yaw;
        transform.eulerAngles = rotation;

        bool movement = Mathf.Abs(moveInput.y) > Mathf.Epsilon;
        bool sprint = sprintAction.IsPressed();

        body.velocity = transform.rotation * Vector3.forward * moveInput.y * (sprint ? sprintSpeed : moveSpeed);

        if (sprint)
        {
            animator.SetBool("IsRunning", true);
        }
        else
        {
            animator.SetBool("IsRunning", false);
        }

        if (movement)
        {
            animator.SetBool("IsWalking", true);
        }
        else
        {
            animator.SetBool("IsWalking", false);
        }

        if (jumpAction.triggered)
        {
            StartCoroutine(Jump());
        }
    }

    IEnumerator Jump()
    {
        animator.SetBool("IsJumping", true);
        float elaspedTime = 0.0f;

        while(elaspedTime < jumpDuration / 2)
        {
            body.velocity = new Vector3(body.velocity.x, Mathf.Lerp(jumpPower, 0, elaspedTime / (jumpDuration / 2)), body.velocity.z);
            elaspedTime += Time.deltaTime;
            yield return null;
        }
        elaspedTime = 0.0f;

        while (elaspedTime < jumpDuration / 2)
        {
            body.velocity = new Vector3(body.velocity.x, Mathf.Lerp(0, -jumpPower, elaspedTime / (jumpDuration / 2)), body.velocity.z);
            elaspedTime += Time.deltaTime;
            yield return null;
        }
        body.velocity = Vector3.zero;

        animator.SetBool("IsJumping", false);
        yield return null;
    }
}
