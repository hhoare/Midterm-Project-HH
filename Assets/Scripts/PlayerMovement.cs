using System;
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

    [SerializeField]
    private float maxAccelerationForce = 5;

    private float currentAccelerationForce = 5;
    private float currentMaxSpeed = 5;


    [SerializeField]
    private float jumpForce = 10;

    private bool isGrounded;
    private Collider2D[] groundedResults = new Collider2D[16];

    private Component[] spriteRenderers;

    public Checkpoint currentCheckpoint = null;


    private float modelScaleX;
    private float modelScaleY;
    private float modelScaleZ;

    private float eatTimer = 20f;

    private PlayerState playerState;

    private AudioSource deathSoundFX;

    private bool dying = false;

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
        dying = false;
        modelTransform = this.GetComponent<Transform>();
        spriteRenderers = this.GetComponentsInChildren<SpriteRenderer>();
        mAnimator = this.GetComponent<Animator>();
        // mAnimator.SetBool("IsEat", false);
        // mAnimator.SetBool("IsWait", true);

        modelTransform.position = new Vector3(modelTransform.position.x, modelTransform.position.y, -1);

        modelScaleX = modelTransform.localScale.x;
        modelScaleY = modelTransform.localScale.y;
        modelScaleZ = modelTransform.localScale.z;
        deathSoundFX = GetComponent<AudioSource>();
    }

    private void InputHandler()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");               ////// ADDED VERTICAL INPUT FOR VERTICAL AXIS FOR THE DUCK AND RUN FASTER STUFF

    }

    // Update is called once per frame
    private void Update() {
       // Debug.Log(currentAccelerationForce);

        AnimationUpdate();  // checks player state and updates anims
        InputHandler();     // initializes input variables
        CheckGrounded();
        Jump();
        Dash();
        ScreenBottomDie();
      //  if (dying) {
         //   this.transform.Rotate(new Vector3(10f, 10f, 10f));
      //  }
      //  Debug.Log(playerState);
     
    }

    private void FixedUpdate()
    {
        if (isGrounded)
        {
            rigidBody2D.gravityScale = 1;
            currentAccelerationForce = 15;

        }
        else
        {
            mAnimator.SetBool("isGrounded", false);
            currentAccelerationForce = 12;

        }
        UpdatePhysicsMaterial();
        Move();


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
          mAnimator.SetBool("isRunning", true);
        }
        if (playerState == PlayerState.Still)
        {
            mAnimator.SetBool("isRunning", false);
        }
        if (playerState == PlayerState.Jump)
        {
            mAnimator.SetBool("isJumping", true);
         //   currentAccelerationForce = 1;

        }

        if (playerState == PlayerState.Dash)
        {
            mAnimator.SetBool("isDashing", true);
        }

    }





    private void CheckGrounded()
    {
        isGrounded = groundedTrigger.OverlapCollider(groundContactFilter, groundedResults) > 0;
        mAnimator.SetBool("isGrounded", true);

        if (!isGrounded) {
       //     playerState = PlayerState.Jump;
        }
    }

    private void Move()     // Handles horizontal movement
    {
        
        if (Mathf.Abs(verticalInput) < 0)
        {

         //   currentAccelerationForce = runAccelerationForce;
            currentMaxSpeed += runMaxSpeed;
        }
        else //     CURRENTLY DONT DIFFERENTIATE BETWEEN WALKING AND RUNNING FOR ANIMATION, ONLY RUNNING AND HEAD DOWN RUNNING
        {
       //     currentAccelerationForce = walkAccelerationForce;
            currentMaxSpeed += walkMaxSpeed;
        }


        // Debug.Log("Horizontal Input: " + horizontalInput);
        if (isGrounded)
        {
            rigidBody2D.AddForce(Vector2.right * horizontalInput * currentAccelerationForce);
            Vector2 ClampedVelocity = rigidBody2D.velocity;
            ClampedVelocity.x = Mathf.Clamp(rigidBody2D.velocity.x, -currentMaxSpeed, currentMaxSpeed);
            rigidBody2D.velocity = ClampedVelocity;
        }

        if (horizontalInput > 0)
        {
            // modelTransform.rotation = Quaternion.Euler(0, 0, 0);
            playerState = PlayerState.Run;
          //  mAnimator.SetBool("isRunning", true);

            modelTransform.localScale = new Vector3(modelScaleX, modelScaleY, modelScaleZ);
        }
        else if(horizontalInput < 0)
        {
            // modelTransform.rotation = Quaternion.Euler(0, 180f, 180f);
            playerState = PlayerState.Run;
           // mAnimator.SetBool("isRunning", true);

            modelTransform.localScale = new Vector3(-modelScaleX, modelScaleY, modelScaleZ);

        }


        if (horizontalInput == 0 && isGrounded)
        {
            playerState = PlayerState.Still;
           // mAnimator.SetBool("isRunning", false);

        }

        if (!isGrounded && playerState != PlayerState.Dash)
        {
            playerState = PlayerState.Jump;
        }

    }


    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rigidBody2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            playerState = PlayerState.Jump;
            mAnimator.SetBool("isJumping", true);
        }
        else
        {
            rigidBody2D.gravityScale = 2;
            mAnimator.SetBool("isJumping", false);

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
        if (playerState==PlayerState.Jump)
      {
            if (Input.GetButtonDown("DashRight"))
            {
                rigidBody2D.AddForce(Vector2.right * 5f, ForceMode2D.Impulse);
                modelTransform.localScale = new Vector3(modelScaleX, modelScaleY, modelScaleZ);

                playerState = PlayerState.Dash;
               // Debug.Log("Dashing");
            }

         if (Input.GetButtonDown("DashLeft") )
        {
            rigidBody2D.AddForce(Vector2.left * 5f, ForceMode2D.Impulse);
            modelTransform.localScale = new Vector3(-modelScaleX, modelScaleY, modelScaleZ);
                playerState = PlayerState.Dash;
             //   Debug.Log("Dashing");
        }
      }
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
        if(currentCheckpoint!=null)
            currentCheckpoint.SetIsActivated(false);

        currentCheckpoint = newCheckpoint;
        currentCheckpoint.SetIsActivated(true);

    }

    public void Respawn()
    {
        dying = false;
        FoodCounter.reset();
        deathSoundFX.PlayOneShot(deathSoundFX.clip, 1f);


        //  this.transform.Rotate(new Vector3(0f, 0f, 3f));


        if (currentCheckpoint == null)
        {
            StartCoroutine(Pause());
        }
        else
        {
            rigidBody2D.velocity = Vector2.zero;
            transform.position = new Vector3(currentCheckpoint.transform.position.x, currentCheckpoint.transform.position.y, -1);
            
        }


    }

    public IEnumerator Pause() {
        Debug.Log("PAUSING");

        this.transform.localScale = new Vector3(0.7f,-0.7f,0.7f);
        
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);


    }


    void ScreenBottomDie()
    {
        if (this.transform.position.y < -10) {

            Respawn();
        }

    }
 




}
