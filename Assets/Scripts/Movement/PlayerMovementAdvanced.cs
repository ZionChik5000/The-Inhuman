using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovementAdvanced : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float groundDrag;

    [Header("Jumping")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCooldown;
    [SerializeField] private float airMultiplier;
    private bool readyToJump;

    [Header("Crouching")]
    [SerializeField] private float crouchSpeed;
    [SerializeField] private float crouchYScale;
    private float startYScale;

    [Header("Dashing")]
    [SerializeField] private float dashForce;
    [SerializeField] private float dashDuration;
    [SerializeField] private KeyCode dashKey = KeyCode.V;
    [SerializeField] private float dashCooldown = 1.8f;
    private bool readyToDash = true;

    [Header("Keybinds")]
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Ground Check")]
    [SerializeField] private float playerHeight;
    [SerializeField] private LayerMask whatIsGround;
    private bool grounded;

    [Header("Slope Handling")]
    [SerializeField] private float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    [SerializeField] private Transform orientation;
    private float horizontalInput;
    private float verticalInput;
    private Vector3 moveDirection;
    private Rigidbody rb;

    [Header("UI Elements")]
    [SerializeField] private GameObject dashCooldownPanel;
    [SerializeField] private List<Image> cooldownBars;

    private float dashCooldownTime = 10f;

    private MovementState state;
    public enum MovementState
    {
        Walking,
        Sprinting,
        Crouching,
        Air,
        Dashing
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;
        startYScale = transform.localScale.y;

        dashCooldownPanel.SetActive(false);
    }

    private void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
        ProcessInput();
        ControlSpeed();
        HandleState();
        rb.drag = grounded ? groundDrag : 0;
    }

    private void FixedUpdate() => MovePlayer();

    private void ProcessInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        HandleJumpInput();
        HandleCrouchInput();
        HandleDashInput();
    }

    private void HandleJumpInput()
    {
        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void HandleCrouchInput()
    {
        if (Input.GetKeyDown(crouchKey)) StartCrouch();
        else if (Input.GetKeyUp(crouchKey)) StopCrouch();
    }

    private void HandleDashInput()
    {
        if (Input.GetKeyDown(dashKey) && readyToDash)
        {
            Debug.Log("Dash key pressed"); // Debug log
            StartDash();
        }
    }

    private void StartCrouch()
    {
        transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
    }

    private void StopCrouch()
    {
        transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
    }

    private void HandleState()
    {
        if (state == MovementState.Dashing) return;

        if (IsCrouching())
        {
            SetState(MovementState.Crouching, crouchSpeed);
        }
        else if (IsSprinting())
        {
            SetState(MovementState.Sprinting, sprintSpeed);
        }
        else if (grounded)
        {
            SetState(MovementState.Walking, walkSpeed);
        }
        else
        {
            SetState(MovementState.Air, moveSpeed);
        }
    }

    private bool IsCrouching() => Input.GetKey(crouchKey);
    private bool IsSprinting() => grounded && Input.GetKey(sprintKey);

    private void SetState(MovementState newState, float speed)
    {
        state = newState;
        moveSpeed = speed;
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        if (OnSlope() && !exitingSlope) MoveOnSlope();
        else if (grounded) MoveOnGround();
        else MoveInAir();
        rb.useGravity = !OnSlope();
    }

    private void MoveOnSlope()
    {
        rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);
        if (rb.velocity.y > 0) rb.AddForce(Vector3.down * 80f, ForceMode.Force);
    }

    private void MoveOnGround()
    {
        rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
    }

    private void MoveInAir()
    {
        rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    private void ControlSpeed()
    {
        if (state != MovementState.Dashing) // Ensure speed control doesn't affect dashing
        {
            if (OnSlope() && !exitingSlope) LimitSlopeSpeed();
            else LimitGroundAirSpeed();
        }
    }

    private void LimitSlopeSpeed()
    {
        if (rb.velocity.magnitude > moveSpeed) rb.velocity = rb.velocity.normalized * moveSpeed;
    }

    private void LimitGroundAirSpeed()
    {
        Vector3 flatVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        if (flatVelocity.magnitude > moveSpeed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
        }
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }

    private Vector3 GetSlopeMoveDirection() => Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;

    private void Jump()
    {
        exitingSlope = true;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
        exitingSlope = false;
    }

    private void StartDash()
    {
        Debug.Log("Starting Dash"); // Debug log
        readyToDash = false;
        state = MovementState.Dashing;
        rb.velocity = Vector3.zero; // Stop current movement
        rb.AddForce(moveDirection.normalized * dashForce, ForceMode.VelocityChange);
        Invoke(nameof(StopDash), dashDuration);

        // Показать панель перезарядки
        StartCoroutine(ShowDashCooldown());
    }

    private IEnumerator ShowDashCooldown()
    {
        dashCooldownPanel.SetActive(true);

        // Очищаем полоски
        foreach (Image bar in cooldownBars)
        {
            bar.enabled = false;
        }

        // Включаем полоски по одной каждую секунду
        for (int i = 0; i < cooldownBars.Count; i++)
        {
            cooldownBars[i].enabled = true;
            yield return new WaitForSeconds(0.2f);
        }

        dashCooldownPanel.SetActive(false);
    }

    private void StopDash()
    {
        Debug.Log("Stopping Dash"); // Debug log
        state = MovementState.Walking;
        Invoke(nameof(ResetDash), dashCooldown);
    }

    private void ResetDash()
    {
        Debug.Log("Resetting Dash"); // Debug log
        readyToDash = true;
    }

    public bool IsGrounded() => grounded;
    public MovementState GetState() => state;

    public void SetKeyBindings(KeyCode forward, KeyCode backward, KeyCode left, KeyCode right, KeyCode jump, KeyCode crouch, KeyCode dash)
    {
        // Обновляем клавиши действий
        this.jumpKey = jump;
        this.crouchKey = crouch;
        this.dashKey = dash;

        // Если используете встроенные Input.GetAxisRaw("Horizontal"), измените систему ввода для кастомных клавиш.
    }

}
