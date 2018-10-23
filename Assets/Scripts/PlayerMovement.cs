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



    [SerializeField]
    private float accelerationForce = 5;
    [SerializeField]
    private float maxSpeed = 5;
    [SerializeField]
    private float jumpForce = 10;

    private bool isGrounded;
    private Collider2D[] groundedResults = new Collider2D[16];


    private Checkpoint currentCheckpoint;

    // Use this for initialization
    void Start () {
        modelTransform = GameObject.Find("Model").GetComponent<Transform>();

        mAnimator = GameObject.Find("player").transform.Find("Model").transform.GetChild(0).gameObject.GetComponent<Animator>();
        mAnimator.SetBool("IsEat", false);
        mAnimator.SetBool("IsWait", true);
    }

    private void InputHandler()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");               ////// ADDED VERTICAL INPUT FOR VERTICAL AXIS FOR THE DUCK AND RUN FASTER STUFF

    }

    // Update is called once per frame
    private void Update () {

        InputHandler();     // initializes input variables
        Jump();
        CheckGrounded();


        AnimationVariables();



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
        isGrounded = groundedTrigger.OverlapCollider(groundContactFilter, groundedResults) > 0;
    }

    private void Move()     // Handles horizontal movement
    {

        rigidBody2D.AddForce(Vector2.right * horizontalInput * accelerationForce);
        Vector2 ClampedVelocity = rigidBody2D.velocity;
        ClampedVelocity.x = Mathf.Clamp(rigidBody2D.velocity.x, 0, maxSpeed);
        rigidBody2D.velocity = ClampedVelocity;

    }


    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rigidBody2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
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
