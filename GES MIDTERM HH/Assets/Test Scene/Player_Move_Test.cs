using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Move_Test : MonoBehaviour {

    public int PlayerSpeed = 10;
    public bool facingRight = true;
    public int playerJumpPower = 1250;

    public float MoveX;

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


        //animation

        //player direction
        if (MoveX < 0.0f && facingRight == false) {
            FlipPlayer();
        }
        else if (MoveX < 0.0f && facingRight == true) {
            FlipPlayer();
        }

        //physics

        gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(MoveX * PlayerSpeed, gameObject.GetComponent<Rigidbody2D>().velocity.y);

    }


    void Jump() {

        // jumping code

    }

    void FlipPlayer()
    {

    }

}
