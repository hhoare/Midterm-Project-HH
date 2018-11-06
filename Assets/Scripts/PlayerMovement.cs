﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour {

    [SerializeField]
    private Rigidbody2D rigidBody2D;
    [SerializeField]
    private Collider2D playerCollider;
    [SerializeField]
    private ContactFilter2D groundContactFilter;
    [SerializeField]
    private Collider2D groundedTrigger;
    [SerializeField]
    private PhysicsMaterial2D movingPhysicsMaterial, stoppingPhysicsMaterial;

    private float horizontalInput;
    private float verticalInput;


    public static Transform modelTransform;
    public static Transform mTransform;
    public static Animator mAnimator;
    private bool isAutoSitSpeed;
    private int intAutoTimer;
    private bool isJumpKey;
    private bool isSitChangeON;

    [SerializeField]
    private float walkAccelerationForce = 5;
    [SerializeField]
    private float walkMaxSpeed = 5;
    [SerializeField]
    private float runAccelerationForce = 7;
    [SerializeField]
    private float runMaxSpeed = 8;

    private float currentAccelerationForce = 5;
    private float currentMaxSpeed = 5;


    [SerializeField]
    private float jumpForce = 10;

    private bool isGrounded;
    private Collider2D[] groundedResults = new Collider2D[16];

    private Component[] spriteRenderers;

    private Checkpoint currentCheckpoint;


    private float modelScaleX;
    private float modelScaleY;
    private float modelScaleZ;

    private float eatTimer = 20f;

    private PlayerState playerState;


    private enum PlayerState
    {
        Still,
        Run,
        Crouch,
        CrouchRun,
        Jump,
        Falling,
        Dash,
        Glide
    }


    void Start() {
        modelTransform = this.GetComponent<Transform>();
        spriteRenderers = this.GetComponentsInChildren<SpriteRenderer>();
        mAnimator = this.GetComponent<Animator>();
        mAnimator.SetBool("IsEat", false);
        mAnimator.SetBool("IsWait", true);

        modelScaleX = modelTransform.localScale.x;
        modelScaleY = modelTransform.localScale.y;
        modelScaleZ = modelTransform.localScale.z;

    }

    private void InputHandler()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");               ////// ADDED VERTICAL INPUT FOR VERTICAL AXIS FOR THE DUCK AND RUN FASTER STUFF

    }

    // Update is called once per frame
    private void Update() {


     //  AnimationUpdate();  // checks player state and updates anims
        InputHandler();     // initializes input variables
        Jump();
        CheckGrounded();


     //   AnimationVariables();



    }

    private void AnimationUpdate()
    {
        /*                  EATING THING, USE COROUTINES AND WAITFORSECONDS FUNCTION
        if (horizontalInput == 0 && verticalInput == 0)
        {

                eatTimer = 20f;
                mAnimator.SetBool("IsEat", true);
                eatTimer -= Time.deltaTime;
                if (eatTimer <= 0.0f)
                {
                    mAnimator.SetBool("IsEat", false);
                    while (eatTimer != 20f)
                    {
                        eatTimer++;
                    }
                }
            
        }
        */

        if (playerState == PlayerState.Run)
        {
            mAnimator.SetBool("IsWalk", false);
            mAnimator.SetBool("IsSitdown", false);
            mAnimator.SetBool("IsRun", false);
        }


    }

    private void FixedUpdate()
    {
        if(isGrounded)
            rigidBody2D.gravityScale = 1;

        UpdatePhysicsMaterial();
        Move();
    }



    private void CheckGrounded()
    {
        isGrounded = groundedTrigger.OverlapCollider(groundContactFilter, groundedResults) > 0;
    }

    private void Move()     // Handles horizontal movement
    {
        
        if (Mathf.Abs(verticalInput) < 0)
        {

            currentAccelerationForce = runAccelerationForce;
            currentMaxSpeed += runMaxSpeed;
        }
        else //     CURRENTLY DONT DIFFERENTIATE BETWEEN WALKING AND RUNNING FOR ANIMATION, ONLY RUNNING AND HEAD DOWN RUNNING
        {
            currentAccelerationForce = walkAccelerationForce;
            currentMaxSpeed += walkMaxSpeed;
        }
        

       // Debug.Log("Horizontal Input: " + horizontalInput);

        rigidBody2D.AddForce(Vector2.right * horizontalInput * currentAccelerationForce);
        Vector2 ClampedVelocity = rigidBody2D.velocity;
        ClampedVelocity.x = Mathf.Clamp(rigidBody2D.velocity.x, -currentMaxSpeed, currentMaxSpeed);
        rigidBody2D.velocity = ClampedVelocity;

        playerState = PlayerState.Run;

        if (horizontalInput > 0)
        {
           // modelTransform.rotation = Quaternion.Euler(0, 0, 0);
            modelTransform.localScale = new Vector3(modelScaleX, modelScaleY, modelScaleZ);
        }
        else if(horizontalInput < 0)
        {
           // modelTransform.rotation = Quaternion.Euler(0, 180f, 180f);
            modelTransform.localScale = new Vector3(-modelScaleX, modelScaleY, modelScaleZ);

        }
    }


    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rigidBody2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
        else
        {
            rigidBody2D.gravityScale = 2;
        }

    }

    private void UpdatePhysicsMaterial()
    {
        if (Mathf.Abs(horizontalInput) > 0)
        {
            playerCollider.sharedMaterial = movingPhysicsMaterial;

        }
        else
        {

            playerCollider.sharedMaterial = stoppingPhysicsMaterial;
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
        if (horizontalInput < 0)
        {
            foreach (SpriteRenderer sprite in spriteRenderers)
            {
                sprite.flipX = true;
            }
        }
        else
        {
            foreach (SpriteRenderer sprite in spriteRenderers)
            {
                sprite.flipX = false;
            }
        }
    }


    public void SetCurrentCheckpoint(Checkpoint newCheckpoint)
    {
        currentCheckpoint.SetIsActivated(false);
        currentCheckpoint = newCheckpoint;
        currentCheckpoint.SetIsActivated(true);

    }

    public void Respawn()
    {
        if (currentCheckpoint == null)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            rigidBody2D.velocity = Vector2.zero;
            transform.position = currentCheckpoint.transform.position;
            
        }
    }



 




}
