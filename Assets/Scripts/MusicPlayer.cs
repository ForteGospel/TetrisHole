using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MusicPlayer : MonoBehaviour {

    [SerializeField] AudioClip[] BGClip;

    private AudioSource BGMusic;
    private int lastScene;

    void OnEnable()
    {
        //Tell our 'OnLevelFinishedLoading' function to start listening for a scene change as soon as this script is enabled.
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable()
    {
        //Tell our 'OnLevelFinishedLoading' function to stop listening for a scene change as soon as this script is disabled. Remember to always have an unsubscription for every delegate you subscribe to!
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        Debug.Log(scene.buildIndex);
        if (scene.buildIndex != 3 && lastScene != scene.buildIndex)
        {
            BGMusic.Stop();
            BGMusic.clip = BGClip[scene.buildIndex];
            BGMusic.loop = true;
            BGMusic.Play();
            lastScene = scene.buildIndex;
            
        }
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start () {
            BGMusic = GetComponent<AudioSource>();
            BGMusic.clip = BGClip[0];
            BGMusic.Play();
        lastScene = 0;
	}

    //private void OnLevelWasLoaded(int level)
    //{
    //    BGMusic.Stop();
    //    BGMusic.clip = BGClip[level];
    //    BGMusic.loop = true;
    //    BGMusic.Play();
    //}
}
