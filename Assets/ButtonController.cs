using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    public void ChangeScene(string scene)
    {
        SceneManager.LoadScene(scene);

    }


    public void ExitGame() {

        Application.Quit();

    }

}
