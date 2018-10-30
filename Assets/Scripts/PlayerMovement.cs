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

    // Use this for initialization
    void Start() {
        modelTransform = this.GetComponent<Transform>();
        spriteRenderers = this.GetComponentsInChildren<SpriteRenderer>();
        mAnimator = this.GetComponent<Animator>();
        mAnimator.SetBool("IsEat", false);
        mAnimator.SetBool("IsWait", true);
    }

    private void InputHandler()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");               ////// ADDED VERTICAL INPUT FOR VERTICAL AXIS FOR THE DUCK AND RUN FASTER STUFF

    }

    // Update is called once per frame
    private void Update() {

        InputHandler();     // initializes input variables
        Jump();
        CheckGrounded();


        AnimationVariables();



    }

    private void FixedUpdate()
    {
        if(isGrounded)
            rigidBody2D.gravityScale = 1;

        UpdatePhysicsMaterial();
        Move();
    }

    private enum PlayerState
    {

    }

    private void CheckGrounded()
    {
        isGrounded = groundedTrigger.OverlapCollider(groundContactFilter, groundedResults) > 0;
    }

    private void Move()     // Handles horizontal movement
    {
        
        if (Mathf.Abs(verticalInput) < 0)
        {
            AutoAnimationCancel();
            isAutoSitSpeed = true;
            if (!isSitChangeON)
            {
                StartCoroutine(SitChangeOFF());
                isSitChangeON = true;
                mAnimator.SetBool("IsSitdown", true);
 
            }
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

    }


    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rigidBody2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isJumpKey = true;

        }
        else
        {
            isJumpKey = false;
            rigidBody2D.gravityScale = 2;
        }

    }

    private void UpdatePhysicsMaterial()
    {
        if (Mathf.Abs(horizontalInput) > 0)
        {
            AutoAnimationCancel();
            playerCollider.sharedMaterial = movingPhysicsMaterial;
            mAnimator.SetBool("IsRun", true);
            mAnimator.SetBool("IsWalk", false);
        }
        else
        {
            mAnimator.SetBool("IsRun", false);
            mAnimator.SetBool("IsWalk", false);
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



    IEnumerator SitChangeOFF()
    {
        yield return new WaitForSeconds(0.5f);
        isSitChangeON = false;
    }

    private IEnumerator AutoAnimation()
    {
        yield return new WaitForSeconds(1.3f);
        if (!isAutoSitSpeed && !mAnimator.GetBool("IsWalk") && !mAnimator.GetBool("IsRun") && !mAnimator.GetBool("IsJumpUp") && !mAnimator.GetBool("IsJumpDown"))
        {
            intAutoTimer++;
            if (intAutoTimer >= 10)
            {
                intAutoTimer = 1;
                mAnimator.SetBool("IsEat", true);
                mAnimator.SetBool("IsWait", false);
            }
            else if (intAutoTimer >= 7)
            {
                mAnimator.SetBool("IsEat", false);
                mAnimator.SetBool("IsWait", true);
            }
            else if (intAutoTimer >= 5)
            {
                mAnimator.SetBool("IsEat", true);
                mAnimator.SetBool("IsWait", false);
            }
            else if (intAutoTimer >= 2)
            {
                mAnimator.SetBool("IsEat", false);
                mAnimator.SetBool("IsWait", true);
            }
        }
        else
        {
            AutoAnimationCancel();
        }
        StartCoroutine(AutoAnimation());
    }
    private void AutoAnimationCancel()
    {
        mAnimator.SetBool("IsEat", false);
        mAnimator.SetBool("IsWait", false);
        intAutoTimer = 0;
    }

    private void AnimationVariables()
    {

        if (rigidBody2D.velocity.y > 0.1f)
        {

            if (isAutoSitSpeed)
            {
                mAnimator.SetBool("IsDive", true);
            }
            else
            {
                mAnimator.SetBool("IsDive", false);
                mAnimator.SetBool("IsJumpUp", true);
                mAnimator.SetBool("IsJumpDown", false);
            }
        }
        else if (rigidBody2D.velocity.y < -0.1f)
        {

            if (isAutoSitSpeed)
            {
                mAnimator.SetBool("IsDive", true);
            }
            else
            {
                mAnimator.SetBool("IsDive", false);
                mAnimator.SetBool("IsJumpUp", false);
                mAnimator.SetBool("IsJumpDown", true);
            }
        }
        else
        {
            mAnimator.SetBool("IsJumpUp", false);
            mAnimator.SetBool("IsJumpDown", false);
            mAnimator.SetBool("IsDive", false);
        }
    }





}
