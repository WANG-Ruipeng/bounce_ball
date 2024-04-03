using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;
using UnityEngine.SceneManagement;

namespace ClawbearGames
{
    public class LevelEditor : EditorWindow
    {
        private LevelManager levelManager = null;

        private float smallBtnWidth = 0;
        private float smallBtnHeight = 20;
        private float bigBtnHeight = 50;
        private float intFieldWidth = 100;
        private int currentLevel = 1;
        private int levelTemp = 0;

        private const string LevelEditorScenePath = "Assets/_Balloon_Popper/Scenes/LevelEditor.unity";

        [MenuItem("Tools/ClawbearGames/Level Editor")]
        public static void ShowWindow()
        {
            // Ask for a change scene confirmation if not on level editor scene
            if (!EditorSceneManager.GetActiveScene().path.Equals(LevelEditorScenePath))
            {
                if (EditorUtility.DisplayDialog(
                        "Open Level Editor",
                        "Do you want to close the current scene and open LevelEditor scene? Unsaved changes in this scene will be discarded.", "Yes", "No"))
                {
                    EditorSceneManager.OpenScene(LevelEditorScenePath);
                    GetWindow(typeof(LevelEditor));
                }
            }
            else
            {
                GetWindow(typeof(LevelEditor));
            }
        }

        private void Update()
        {
            // Check if is in LevelEditor scene.
            Scene activeScene = EditorSceneManager.GetActiveScene();

            // Auto exit if not in level editor scene.
            if (!activeScene.path.Equals(LevelEditorScenePath))
            {
                this.Close();
                return;
            }
        }

        private void OnGUI()
        {
            if (levelManager == null)
            {
                levelManager = FindObjectOfType<LevelManager>();
                currentLevel = levelManager.TotalLevel();
            }
            smallBtnWidth = (LevelManager.ScreenshotWidth) / 3.85f;

            // Disable the whole editor window if the game is in playing mode
            EditorGUI.BeginDisabledGroup(Application.isPlaying);

            GUILayout.Space(10);
            GUILayout.BeginVertical("Box", GUILayout.Width(LevelManager.ScreenshotWidth));
            EditorGUILayout.LabelField("TOTAL LEVEL: " + levelManager.TotalLevel().ToString(), EditorStyles.boldLabel);
            GUILayout.Space(10);
            EditorGUILayout.LabelField("                                          CURRENT LEVEL");
            GUILayout.BeginHorizontal();

            //////////////Decrease level number
            EditorGUI.BeginDisabledGroup(currentLevel <= 1); //If current level is 1, disable load previous level
            if (GUILayout.Button("←", GUILayout.Width(smallBtnWidth), GUILayout.Height(smallBtnHeight)))
            {
                currentLevel--;
                GUI.FocusControl(null);
            }
            EditorGUI.EndDisabledGroup();
            //////////////////////////////////

            EditorGUI.BeginDisabledGroup(levelManager.TotalLevel() == 0);
            currentLevel = EditorGUILayout.IntField(currentLevel, GUILayout.Width(intFieldWidth), GUILayout.Height(smallBtnHeight));
            EditorGUI.EndDisabledGroup();

            //////////////Increase level number
            EditorGUI.BeginDisabledGroup(currentLevel == levelManager.TotalLevel()); //If current level is equals the max level, disable load next level
            if (GUILayout.Button("→", GUILayout.Width(smallBtnWidth), GUILayout.Height(smallBtnHeight)))
            {
                currentLevel++;
                GUI.FocusControl(null);
            }
            EditorGUI.EndDisabledGroup();
            //////////////////////////////////

            /////////////Create new level section 
            EditorGUI.BeginDisabledGroup(levelManager.TotalLevel() > 0 && levelManager.IsLevelNullData(levelManager.TotalLevel()));
            if (GUILayout.Button("New Level", GUILayout.Width(smallBtnWidth), GUILayout.Height(smallBtnHeight)))
            {
                if (levelManager.TotalLevel() == 0)
                {
                    //Create new level here
                    levelManager.CreateLevel();
                    currentLevel = levelManager.TotalLevel();
                }
                else if (levelManager.IsLevelNullData(levelManager.TotalLevel()))
                {
                    string title = "Level Unsaved!";
                    string message = "Please save level " + levelManager.TotalLevel().ToString() + " before create a new one!";
                    EditorUtility.DisplayDialog(title, message, "OK");
                }
                else
                {
                    //Create new level here
                    levelManager.CreateLevel();
                    currentLevel = levelManager.TotalLevel();
                }

                GUI.FocusControl(null);
            }
            EditorGUI.EndDisabledGroup();
            //////////////////////////////////


            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            //////////////////////////////////


            /////////////Config parameters section

            GUILayout.Space(5);
            GUILayout.BeginVertical("Box", GUILayout.Width(LevelManager.ScreenshotWidth + 5));

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Ball Amount: ");
            levelManager.BallAmount = EditorGUILayout.IntField(levelManager.BallAmount, GUILayout.Width(intFieldWidth));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Draw Amount: ");
            levelManager.DrawAmount = EditorGUILayout.IntField(levelManager.DrawAmount, GUILayout.Width(intFieldWidth));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Fade Amount: ");
            levelManager.FadeAmount = EditorGUILayout.IntField(levelManager.FadeAmount, GUILayout.Width(intFieldWidth));
            GUILayout.EndHorizontal();

            EditorGUI.BeginChangeCheck();
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Top Background Color: ");
            levelManager.TopBackgroundColor = EditorGUILayout.ColorField(levelManager.TopBackgroundColor, GUILayout.Width(intFieldWidth));
            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Bottom Background Color: ");
            levelManager.BottomBackgroundColor = EditorGUILayout.ColorField(levelManager.BottomBackgroundColor, GUILayout.Width(intFieldWidth));
            GUILayout.EndHorizontal();
            if (EditorGUI.EndChangeCheck())
            {
                levelManager.backgroundMaterial.SetColor("_TopColor", levelManager.TopBackgroundColor);
                levelManager.backgroundMaterial.SetColor("_BottomColor", levelManager.BottomBackgroundColor);
            }


            GUILayout.EndVertical();

            /////////////Show warning section 
            if (levelManager.TotalLevel() == 0)
            {
                GUILayout.BeginVertical("Box", GUILayout.Width(LevelManager.ScreenshotWidth + 6));
                EditorGUILayout.HelpBox("There is no level to load. Please create new level!", MessageType.Warning);
                GUILayout.EndVertical();
            }
            else if (currentLevel <= 0 || currentLevel > levelManager.TotalLevel())
            {
                GUILayout.BeginVertical("Box", GUILayout.Width(LevelManager.ScreenshotWidth));
                EditorGUILayout.HelpBox("Please enter a valid number!", MessageType.Error);
                GUILayout.EndVertical();
            }
            else
            {
                /////////////Actualy load level section 
                if (levelTemp != currentLevel)
                {
                    levelTemp = currentLevel;
                    if (!levelManager.IsLevelNullData(currentLevel))
                    {
                        levelManager.LoadLevel(currentLevel);
                        levelManager.backgroundMaterial.SetColor("_TopColor", levelManager.TopBackgroundColor);
                        levelManager.backgroundMaterial.SetColor("_BottomColor", levelManager.BottomBackgroundColor);
                    }
                }

                /////////////Draw level's screenshot section
                GUILayout.Space(5);
                if (!levelManager.IsLevelNullData(currentLevel))
                {
                    GUILayout.BeginVertical("Box", GUILayout.Width(LevelManager.ScreenshotWidth));

                    //Show the screenshot of the level
                    Texture tex = EditorGUIUtility.Load(LevelManager.ScreenshotPath(currentLevel)) as Texture;
                    if (tex != null)
                    {
                        if (tex.height != LevelManager.ScreenshotHeight)
                        {
                            TextureImporter texImport = TextureImporter.GetAtPath(LevelManager.ScreenshotPath(currentLevel)) as TextureImporter;
                            texImport.textureType = TextureImporterType.Sprite;
                            texImport.alphaSource = TextureImporterAlphaSource.None;
                            AssetDatabase.ImportAsset(LevelManager.ScreenshotPath(currentLevel), ImportAssetOptions.ForceUpdate);
                            AssetDatabase.Refresh();
                        }
                        Rect rect = GUILayoutUtility.GetRect(LevelManager.ScreenshotWidth, LevelManager.ScreenshotHeight);
                        GUI.DrawTexture(rect, tex, ScaleMode.ScaleToFit);
                    }

                    GUILayout.EndVertical();
                }

                /////////////Save and overwrite level section
                GUILayout.Space(5);
                string btnName;
                float width = LevelManager.ScreenshotWidth + 6;
                if (levelManager.IsLevelNullData(currentLevel))
                {
                    btnName = "SAVE LEVEL";
                    GUILayout.BeginVertical("Box", GUILayout.Width(width));
                    EditorGUILayout.HelpBox("Please save level before create a new one!", MessageType.Warning);
                    GUILayout.EndVertical();
                }
                else
                {
                    btnName = "OVERWRITE LEVEL";
                }

                if (GUILayout.Button("CLEAR SCENE", GUILayout.Width(width), GUILayout.Height(bigBtnHeight / 1.5f)))
                {
                    levelManager.ClearScene();
                }
                if (GUILayout.Button(btnName, GUILayout.Width(width), GUILayout.Height(bigBtnHeight / 1.5f)))
                {
                    BallIndicator[] ballIndicators = FindObjectsOfType<BallIndicator>();
                    if (ballIndicators.Length == 0)
                    {
                        string title = "Level Unsaved!";
                        string message = "There is no Ball Indicator object in the level! Make sure that you have only one object with BallIndicator component in it.";
                        EditorUtility.DisplayDialog(title, message, "OK");
                    }
                    else if (ballIndicators.Length > 1)
                    {
                        string title = "Level Unsaved!";
                        string message = "There is more than one Ball Indicator object in the level! Make sure that you have only one object with BallIndicator component in it.";
                        EditorUtility.DisplayDialog(title, message, "OK");
                    }
                    else if (!HasBalloons())
                    {
                        string title = "Level Unsaved!";
                        string message = "There is no balloon object in the level! Make sure that you have one or more object with BalloonController component in it.";
                        EditorUtility.DisplayDialog(title, message, "OK");
                    }
                    else
                    {
                        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
                        levelManager.SaveLevel(currentLevel);
                        AssetDatabase.Refresh();
                    }
                }
            }
            EditorGUI.EndDisabledGroup();
        }


        private bool HasBalloons()
        {
            return FindObjectsOfType<BalloonController>().Length > 0;
        }
    }
}
