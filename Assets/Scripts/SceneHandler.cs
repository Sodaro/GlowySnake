using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum BuildScene { LevelSelector, LevelEditor }

public class SceneHandler : MonoBehaviour
{
    private static SceneHandler _instance;

    public static SceneHandler Instance => _instance;

    public void RestartScene()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.buildIndex);
    }

    public void LoadScene(BuildScene buildScene)
    {
        if ((int)buildScene >= Enum.GetValues(typeof(BuildScene)).Length)
            throw new System.Exception("INVALID SCENE PASSED TO SCENEHANDLER");

        SceneManager.LoadScene((int)buildScene);
    }

    public void LoadNextScene()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        if (activeScene.buildIndex + 1 >= Enum.GetValues(typeof(BuildScene)).Length)
            return;

        SceneManager.LoadScene(activeScene.buildIndex + 1);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
			            Application.Quit();
#endif
    }

    private void Awake()
    {
        //if (_instance != null && _instance != this)
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
}
