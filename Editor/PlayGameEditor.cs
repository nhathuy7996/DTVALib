using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq; 
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DTVA.Editor
{

    [InitializeOnLoad]
    public static class PlayGameEditor
    {
        const string KeyOldScene = "OLD_SCENE";
        static PlayGameEditor()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            ToolBarHnn.OnGUIRightAction += () => ShowBtnLoadScene();

            ToolBarHnn.OnGUILeftAction += () => SelectScene();
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {

            if (state == PlayModeStateChange.EnteredEditMode && EditorPrefs.HasKey(KeyOldScene))
            {
                string oldScene = EditorPrefs.GetString(KeyOldScene);
                EditorSceneManager.OpenScene(oldScene);
                EditorPrefs.DeleteKey(KeyOldScene);
            }
        }


        static void AddMenuItemForScenes(GenericMenu menu, string menuPath, string value, bool isSelected = false)
        {
            // the menu item is marked as selected if it matches the current value of m_Color
            menu.AddItem(new GUIContent(menuPath), isSelected, OnDropBoxSceneItemClick, value);
        }

        static void OnDropBoxSceneItemClick(object item)
        {
            EditorSceneManager.OpenScene(item.ToString());
        }

        static void ShowBtnLoadScene()
        {
            if (GUILayout.Button("Play Game", GUILayout.Width(170)))
            {
                if (!EditorUtility.DisplayDialog("Attention", "Do you want start game with all playerPrefb data before?", "Yes", "No"))
                    PlayerPrefs.DeleteAll();

                if (SceneUtility.GetScenePathByBuildIndex(0) != "")
                {
                    string currentScene = SceneManager.GetActiveScene().path;
                    EditorPrefs.SetString(KeyOldScene, currentScene);
                    EditorSceneManager.OpenScene(EditorBuildSettings.scenes[0].path);
                }
                UnityEditor.EditorApplication.isPlaying = true;
            }
        }


        static void SelectScene()
        {
            if (EditorGUILayout.DropdownButton(content: new GUIContent(EditorSceneManager.GetActiveScene().path), FocusType.Passive, GUILayout.Width(170)) && EditorBuildSettings.scenes.Count() > 0)
            {
                GenericMenu menu = new GenericMenu();

                string[] scenes = UnityEditor.AssetDatabase.FindAssets("t:Scene");

                foreach (string scene in scenes)
                {
                    string path = UnityEditor.AssetDatabase.GUIDToAssetPath(scene);
                    AddMenuItemForScenes(menu, path, path, SceneManager.GetActiveScene().path.Equals(scene));
                }


                menu.ShowAsContext();
            }
        }
    }
}
