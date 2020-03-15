using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelManager : MonoBehaviour {


    [SerializeField] float autoLoadNextLevelAfer;


    void Start (){
        if (SceneManager.GetActiveScene().buildIndex == 0 && autoLoadNextLevelAfer != 0)
        {
            Invoke("LoadNextLevel", autoLoadNextLevelAfer);
        }
	}

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (SceneManager.GetActiveScene().buildIndex == 0)
                QuitRequest();
            else
                LoadStartMenu();
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
        loadLevelByNum(currentSceneIndex + 1);
    }

    public void loadLevelByNum(int levelnum)
    {
        SceneManager.LoadScene(levelnum);
    }

    public void LoadStartMenu()
    {
        SceneManager.LoadScene(0);
    }
}
