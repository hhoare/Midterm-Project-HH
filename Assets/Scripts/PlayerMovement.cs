using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    [SerializeField]
    private Rigidbody2D RigidBody2D;
    [SerializeField]
    private Collider2D playerCollider;
    [SerializeField]
    private ContactFilter2D GroundContactFilter;
    [SerializeField]
    private Collider2D GroundedTrigger;
    [SerializeField]
    private PhysicsMaterial2D MovingPhysicsMaterial, StoppingPhysicsMaterial;


    private float HorizontalInput;

    [SerializeField]
    private float AccelerationForce = 5;
    [SerializeField]
    private float MaxSpeed = 5;
    [SerializeField]
    private float JumpForce = 10;

    private bool IsGrounded;
    private Collider2D[] GroundedResults = new Collider2D[16];

    // Use this for initialization
    void Start () {
		
	}

    private void InputHandler()
    {
        HorizontalInput = Input.GetAxisRaw("Horizontal");
    }

    // Update is called once per frame
    private void Update () {

        InputHandler();     // initializes input variables
        Jump();
        CheckGrounded();

	}

    private void FixedUpdate()
    {
        UpdatePhysicsMaterial();
        Move();
    }

    private enum PlayerState
    {

    }

    private void CheckGrounded()
    {
        IsGrounded = GroundedTrigger.OverlapCollider(GroundContactFilter, GroundedResults) > 0;
    }

    private void Move()     // Handles horizontal movement
    {

        RigidBody2D.AddForce(Vector2.right * HorizontalInput * AccelerationForce);
        Vector2 ClampedVelocity = RigidBody2D.velocity;
        ClampedVelocity.x = Mathf.Clamp(RigidBody2D.velocity.x, 0, MaxSpeed);
        RigidBody2D.velocity = ClampedVelocity;

    }


    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && IsGrounded)
        {
            RigidBody2D.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
        }

    }

    private void UpdatePhysicsMaterial()
    {
        if (Mathf.Abs(HorizontalInput) > 0)
        {
            playerCollider.sharedMaterial = MovingPhysicsMaterial;  
        }
        else
        {
            playerCollider.sharedMaterial = StoppingPhysicsMaterial;
        }
    }

    private void Dash()
    {

    }

    private void Glide()
    {

    }

    private void FlipPlayer()
    {

    }



}
