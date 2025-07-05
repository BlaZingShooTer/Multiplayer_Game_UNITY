using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using System;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public static class StartUpSceneLoadup 
{
    static StartUpSceneLoadup() 
    {
        // Check if the scene is already loaded

        EditorApplication.playModeStateChanged += LoadStartUpScene;

    }

    private static void LoadStartUpScene(PlayModeStateChange change)
    {
        if(change == PlayModeStateChange.ExitingEditMode) 
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        }

        if(change == PlayModeStateChange.EnteredPlayMode) 
        {
            // Check if the scene is already loaded
            if (SceneManager.GetActiveScene().buildIndex != 0) 
            {
                // Load the StartUpScene
                SceneManager.LoadScene(0);
            }
        }

    }
}
