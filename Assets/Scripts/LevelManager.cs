using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelManager : MonoBehaviour {


    [SerializeField] float autoLoadNextLevelAfer;


    void Start (){
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            //Invoke("LoadNextLevel", autoLoadNextLevelAfer);
        }
	}

	public void LoadLevel(string name){
		Debug.Log ("New Level load: " + name);
        SceneManager.LoadScene(name);
	}

	public void QuitRequest(){
		Debug.Log ("Quit requested");
		Application.Quit ();
	}

    public void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1);
    }
}
