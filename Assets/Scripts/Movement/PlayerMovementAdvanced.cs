using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementAdvanced : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed, sprintSpeed, crouchSpeed, crouchYScale, jumpForce, jumpCooldown, airMultiplier, groundDrag, playerHeight, maxSlopeAngle, damage = 10f, range = 100f;
    public LayerMask whatIsGround, enemyLayer;
    public Camera fpsCam;
    public Transform orientation;
    public KeyCode jumpKey = KeyCode.Space, sprintKey = KeyCode.LeftShift, crouchKey = KeyCode.LeftControl;

    private float moveSpeed, startYScale, horizontalInput, verticalInput;
    private Vector3 moveDirection;
    private Rigidbody rb;
    private RaycastHit slopeHit;
    public bool grounded, readyToJump = true, exitingSlope;

    private enum MovementState { walking, sprinting, crouching, air }
    private MovementState state;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Prevent player from tipping over due to physics
        startYScale = transform.localScale.y; // Store original player height
    }

    public void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround); // Check if player is grounded
        
        HandleInput();  // Manage input for movement and actions
        SpeedControl();  // Control max speed
        StateHandler();  // Set movement state (walking, sprinting, etc.)
        
        rb.drag = grounded ? groundDrag : 0;  // Adjust drag based on whether the player is grounded
    }

    private void FixedUpdate() => MovePlayer(); // Handle physics-based movement

    private void HandleInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // Jump if conditions are met
        if (Input.GetKey(jumpKey) && readyToJump && grounded) { Jump(); }

        // Handle crouching
        if (Input.GetKeyDown(crouchKey)) Crouch(true);
        if (Input.GetKeyUp(crouchKey)) Crouch(false);

        // Shooting
        if (Input.GetButtonDown("Fire1")) Shoot();
    }

    private void StateHandler()
    {
        // Set state to crouching if crouch key is held
        if (Input.GetKey(crouchKey)) SetState(MovementState.crouching, crouchSpeed);
        // Set state to sprinting if sprint key is held and player is on the ground
        else if (grounded && Input.GetKey(sprintKey)) SetState(MovementState.sprinting, sprintSpeed);
        // Set state to walking if grounded but not sprinting or crouching
        else if (grounded) SetState(MovementState.walking, walkSpeed);
        // Set state to air if not grounded
        else state = MovementState.air;
    }

    private void SetState(MovementState newState, float newSpeed)
    {
        state = newState;
        moveSpeed = newSpeed;
    }

    private void MovePlayer()
    {
        // Calculate direction based on input and player orientation
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // Move differently if player is on a slope
        if (OnSlope() && !exitingSlope) ApplyForce(GetSlopeMoveDirection(), moveSpeed * 20f, rb.velocity.y > 0 ? -80f : 0);
        else ApplyForce(moveDirection.normalized, grounded ? moveSpeed * 10f : moveSpeed * 10f * airMultiplier);

        rb.useGravity = !OnSlope();  // Disable gravity on slopes to prevent sliding
    }

    private void ApplyForce(Vector3 direction, float force, float verticalForce = 0)
    {
        rb.AddForce(direction * force, ForceMode.Force);
        if (verticalForce != 0) rb.AddForce(Vector3.down * verticalForce, ForceMode.Force);
    }

    private void SpeedControl()
    {
        // Limit speed on slopes
        if (OnSlope() && rb.velocity.magnitude > moveSpeed) LimitSpeed(rb.velocity.normalized * moveSpeed);
        // Limit horizontal speed on flat ground or in the air
        else if (new Vector3(rb.velocity.x, 0f, rb.velocity.z).magnitude > moveSpeed) LimitSpeed(new Vector3(rb.velocity.x, rb.velocity.y, rb.velocity.z).normalized * moveSpeed);
    }

    private void LimitSpeed(Vector3 limitedVelocity) => rb.velocity = limitedVelocity;

    private void Jump()
    {
        readyToJump = false;
        exitingSlope = true;  // Avoid unwanted slope interactions during jump
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);  // Reset vertical velocity before jump
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);  // Apply upward force for jump
        Invoke(nameof(ResetJump), jumpCooldown);  // Set cooldown before the next jump
    }

    private void ResetJump()
    {
        readyToJump = true;
        exitingSlope = false;
    }

    private void Crouch(bool isCrouching)
    {
        // Change player height when crouching
        transform.localScale = new Vector3(transform.localScale.x, isCrouching ? crouchYScale : startYScale, transform.localScale.z);
        if (isCrouching) rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);  // Apply downward force for quick crouch
    }

    private bool OnSlope()
    {
        // Check if the player is standing on a slope
        return Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f) && Vector3.Angle(Vector3.up, slopeHit.normal) < maxSlopeAngle;
    }

    private Vector3 GetSlopeMoveDirection() => Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;

    private void Shoot()
    {
        // Raycast to detect enemy within shooting range
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out RaycastHit hit, range, enemyLayer))
        {
            // Deal damage to enemy if hit
            if (hit.transform.GetComponent<Enemy>() is Enemy enemy)
            {
                float finalDamage = state == MovementState.air ? damage * 1.5f : damage;  // Increase damage if player is in the air
                enemy.TakeDamage(finalDamage);  // Apply damage to the enemy
            }
        }
    }
}
