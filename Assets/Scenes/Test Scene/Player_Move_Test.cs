using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Move_Test : MonoBehaviour {

    public Animator animator;


    public int PlayerSpeed = 10;
    private bool facingRight = true;
    public int playerJumpPower = 1250;
    private float MoveX;

    public Vector2 DashForce;

    public DashState dashState;
    public float dashTimer;
    public float maxDash = 20f;

    public Vector2 savedVelocity; 


    private bool IsDashing
    {
        get { return dashState == DashState.Dashing; }
    }

    

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        PlayerMove ();
        Dash();

    }

    private void FixedUpdate()
    {
        
    }

    void PlayerMove()
    {
        animator.SetFloat("Speed", Mathf.Abs(MoveX));
        animator.SetBool("Dashing", IsDashing);


        //controls
        MoveX = Input.GetAxis("Horizontal");
        if (Input.GetButtonDown("Jump")) {
            Jump();
        }

        //animation

        //player direction
        if (MoveX > 0.0f && facingRight == false) {
            FlipPlayer();
        }
        else if (MoveX < 0.0f && facingRight == true) {
            FlipPlayer();
        }

        //physics

        gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(MoveX * PlayerSpeed, gameObject.GetComponent<Rigidbody2D>().velocity.y);

        
    }

    public enum DashState
    {
        Ready,
        Dashing,
        Cooldown
    }

    void Jump() {

        // jumping code
        GetComponent<Rigidbody2D>().AddForce(Vector2.up * playerJumpPower);
    }


   public void Dash() {

        switch (dashState)
        {
            case DashState.Ready:
                var isDashKeyDown = Input.GetKeyDown(KeyCode.Z);
                if (isDashKeyDown)
                {
                    savedVelocity = DashForce;

                    dashState = DashState.Dashing;
                }
                break;
            case DashState.Dashing:
                dashTimer += Time.deltaTime * 300;
                if (dashTimer >= maxDash)
                {
                    dashTimer = maxDash;
                    if (facingRight)
                        GetComponent<Rigidbody2D>().AddForce(Vector2.right * savedVelocity);
                    if (!facingRight)
                        GetComponent<Rigidbody2D>().AddForce(Vector2.left * savedVelocity);
                    dashState = DashState.Cooldown;
                }
                break;
            case DashState.Cooldown:
                dashTimer -= Time.deltaTime*10;
                if (dashTimer <= 0)
                {
                    dashTimer = 0;
                    dashState = DashState.Ready;
                }
                break;
        }

    }

    void FlipPlayer()
    {
        facingRight = !facingRight;
        Vector2 localScale = gameObject.transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

}
