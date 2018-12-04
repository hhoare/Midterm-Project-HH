using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour {

    private AudioSource foodSoundFX;

    // Use this for initialization
    void Start () {
        this.GetComponent<SpriteRenderer>().enabled = true;
        this.GetComponent<PolygonCollider2D>().enabled = true;
        foodSoundFX = GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update () {

        if (FoodCounter.foodCount == 0) {
            reset();
        }

	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            foodSoundFX.PlayOneShot(foodSoundFX.clip, 1f);

          //  StartCoroutine(pause());
          //  Destroy(this.gameObject);
            this.GetComponent<SpriteRenderer>().enabled = false;
            this.GetComponent<PolygonCollider2D>().enabled = false;

            FoodCounter.increase();
        }
    }

    public void reset()
    {
        this.GetComponent<SpriteRenderer>().enabled = true;
        this.GetComponent<PolygonCollider2D>().enabled = true;
    }


}
