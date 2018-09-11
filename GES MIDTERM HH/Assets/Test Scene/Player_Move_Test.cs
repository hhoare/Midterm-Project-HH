using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Move_Test : MonoBehaviour {

    public int PlayerSpeed = 10;
    private bool facingRight = false;
    public int playerJumpPower = 1250;

    private float MoveX;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        PlayerMove ();

		
	}

    void PlayerMove()
    {
        //controls
        MoveX = Input.GetAxis("Horizontal");
        if (Input.GetButtonDown("Jump")) {
            Jump();
        }

        //animation

        //player direction
        if (MoveX < 0.0f && facingRight == false) {
            FlipPlayer();
        }
        else if (MoveX > 0.0f && facingRight == true) {
            FlipPlayer();
        }

        //physics

        gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(MoveX * PlayerSpeed, gameObject.GetComponent<Rigidbody2D>().velocity.y);

    }


    void Jump() {

        // jumping code
        GetComponent<Rigidbody2D>().AddForce(Vector2.up * playerJumpPower);
    }

    void FlipPlayer()
    {
        facingRight = !facingRight;
        Vector2 localScale = gameObject.transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

}
