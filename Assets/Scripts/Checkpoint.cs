using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour {

    private bool isActivated;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerMovement player = collision.GetComponent<PlayerMovement>();
            player.SetCurrentCheckpoint(this);

            //add visual effect to let player know that they've activated the checkpoint
        }
    }

    public void SetIsActivated(bool value)
    {
        isActivated = value;

    }



}
