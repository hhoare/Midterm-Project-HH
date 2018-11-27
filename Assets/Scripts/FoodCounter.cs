using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FoodCounter : MonoBehaviour {


    public static int foodCount;

    Text text;


    // Use this for initialization
    void Start () {
        text = this.GetComponent<Text>();

    }

    // Update is called once per frame
    void Update () {
        text.text = "Bread Gotten: " + foodCount.ToString();
    }

    public static void increase() {

        foodCount++;
    }

    public static void reset()
    {
        foodCount = 0;

    }

}
