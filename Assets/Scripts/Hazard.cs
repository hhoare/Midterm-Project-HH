using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Hazard : MonoBehaviour {



    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("Colliding with player");

        if (collision.CompareTag("Player"))
        {
            Debug.Log("Colliding with player");
            PlayerMovement player = collision.GetComponent<PlayerMovement>();
            player.Respawn();
        }
    }
}
