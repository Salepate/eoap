using Dirt.Hackit;
using EOAP.Plugin.Behaviours;
using Il2CppInterop.Runtime.Runtime;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace EOAP.Plugin.AP
{

    // IMGUI UI
    public class APDebug
    {
        //=============================================================
        // Config: Styling
        //=============================================================
        private GUILayoutOption ButtonMedium;
        private GUILayoutOption ButtonVerySmall;
        private void SetupStyle()
        {
            ButtonVerySmall = GUILayout.Width(20f);
            ButtonMedium = GUILayout.Width(160f);
        }

        private int _window;

        private string[] _windowNames;
        private System.Action[] _windows;

        public APDebug()
        {
            _window = -1;
            _windows = new System.Action[]
            {
                DrawGeneralDebugging,
                DrawHierarchy,
                DrawCanvasRipper,
            };
            _windowNames = ["General", "Hierarchy", "Canvas Ripper"];
        }

        public void DrawWindow(Rect pos)
        {
            if (ButtonMedium == null)
            {
                SetupStyle();
            }
            if (_window < 0 || _window >= _windows.Length)
            {
                return;
            }
            StrippedUI.BeginArea(pos, GUI.skin.box);
            _windows[_window]();
            StrippedUI.EndArea();

        }

        public void DrawUI(Rect pos)
        {
            StrippedUI.BeginArea(pos, GUI.skin.box);
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Game Debug"))
            {
                DebugManager.JAOFCFFEELF.Open();
            }

            int nextWindow = -1;
            for(int i = 0; i < _windowNames.Length; ++i)
            {
                if (GUILayout.Button(_windowNames[i]))
                {
                    nextWindow = i;
                }
            }
            GUILayout.EndHorizontal();
            StrippedUI.EndArea();
            if (nextWindow != -1)
                SwapToWindow(nextWindow);
                
        }

        private void SwapToWindow(int newWindow)
        {
            if (_window != newWindow)
            {
                _window = newWindow;
                InControl.InputManager.enabled = false;
            }
            else
            {
                _window = -1;
                InControl.InputManager.enabled = true;
            }
        }

        //=============================================================
        // Window: General Debugging 
        //=============================================================
        private void DrawGeneralDebugging()
        {
            EOSession session = APBehaviour.GetSession();
            EOPersistent persistent = APBehaviour.GetPersistent();
            GUILayout.BeginVertical(GUI.skin.box);
            bool inputState = InControl.InputManager.Enabled;

            if (GUILayout.Button("Block Inputs: " + (inputState.ToString())))
            {
                InControl.InputManager.Enabled = !inputState;
            }
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            bool guiState = GUI.enabled;
            GUI.enabled = session.Connected;
            if (GUILayout.Button("Sync New Items"))
            {
                session.SyncNewItems(persistent, false);
            }
            GUI.enabled = guiState;
            if (GUILayout.Button("Save Persistent"))
            {
                string persistentData = JsonConvert.SerializeObject(persistent);
                System.IO.File.WriteAllText(EOPersistent.GetFilePath(), persistentData);
            }
            GUILayout.EndVertical();

            //GUILayout.BeginVertical();
            //if (GUILayout.Button("Load DynDB"))
            //{
            //    Builder.Load("dyndb.json");
            //}
            //if (GUILayout.Button("Save DynDB"))
            //{
            //    Builder.Dump("dyndb.json");
            //}
            //GUILayout.EndVertical();
        }

        //=============================================================
        // Window: Canvas Ripper
        //=============================================================
        private bool _includeInactives;
        private void DrawCanvasRipper()
        {
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            _includeInactives = GUILayout.Button($"Include Inactives [{_includeInactives}]") ^ _includeInactives;
            bool swapToInspector = false;
            System.Type targetComp = null;

            GUILayout.Label("Get Objects:");

            for(int i = 0; i < BasicComponents.Length; ++i)
            {
                if (GUILayout.Button(BasicComponents[i].Name))
                {
                    targetComp = BasicComponents[i];
                }
            }

            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            GUILayout.Label(" ");
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            if (targetComp != null)
            {
                Il2CppSystem.Type type = Il2CppInterop.Runtime.Il2CppType.From(targetComp);
                Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<Object> list = GameObject.FindObjectsOfType(type, _includeInactives);
                List<GameObject> gos = new List<GameObject>();
                for(int i = 0; i < list.Count; ++i)
                {
                    Component comp = list[i].TryCast<Component>();
                    if (comp != null)
                        gos.Add(comp.gameObject);
                }
                SetList(gos.ToArray());
                swapToInspector = gos.Count > 0;
            }


            if (swapToInspector)
            {
                SwapToWindow(1);
            }
        }

        private void Ripper_GetRootObjects()
        {
            Il2CppSystem.Collections.Generic.List<GameObject> list = new Il2CppSystem.Collections.Generic.List<GameObject>();
            SceneManager.GetActiveScene().GetRootGameObjects(list);
            _rootObjects = new GameObject[list.Count];
            for (int i = 0; i < list.Count; ++i)
            {
                _rootObjects[i] = list[i];
            }
        }

        //=============================================================
        // Window: Hierarchy Traversal
        //=============================================================
        private const int Pagination = 10;
        private static readonly System.Type[] BasicComponents = [typeof(Camera), typeof(Canvas), typeof(Image), typeof(Text), typeof(TMP_Text), typeof(CanvasGroup)];
        private GameObject _inspectedObject;
        private int _page;
        private string _inspectedPath;
        private GameObject[] _rootObjects;
        private string[] _inspectBasicComponents = System.Array.Empty<string>();
        private List<string> _additionalData = new List<string>();
        private void SetList(GameObject[] list)
        {
            _rootObjects = list;
            _inspectedObject = null;
            _page = 0;
        }
        private void DrawHierarchy()
        {
            if (_rootObjects == null)
            {
                GUILayout.Label("No root list available, use a tool to select one");
                return;
            }

            GameObject targetObject = null;

            bool guiState = GUI.enabled;
            GUILayout.BeginHorizontal();

            GUI.enabled = _rootObjects != null;
            if (GUILayout.Button("Reset"))
            {
                _inspectedObject = null;
            }
            GUI.enabled = guiState;


            if (_inspectedObject != null && _inspectedObject.transform.parent != null)
            {
                if (GUILayout.Button($"Parent ({_inspectedObject.transform.parent.name})"))
                {
                    Hierarchy_InspectObject(_inspectedObject.transform.parent.gameObject);
                }
            }

            GUILayout.Label(_inspectedPath ?? "No Object selected");
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical(GUI.skin.box);
            int count = 0;
            if (_inspectedObject == null)
            {
                count = _rootObjects.Length;

                for(int i = 0; i < _rootObjects.Length && i < Pagination; ++i)
                {
                    int index = _page * Pagination + i;
                    if (index >= count)
                    {
                        GUILayout.Label("");
                    }
                    else
                    {
                        if (GUILayout.Button(_rootObjects[index].name, ButtonMedium))
                        {
                            targetObject = _rootObjects[index];
                        }
                    }
                }
                if (_rootObjects.Length <= 0)
                    GUILayout.Label("Build a list using Canvas Ripper");
            }
            else
            {
                count = _inspectedObject.transform.childCount;

                for (int i = 0; i < count && i < Pagination; ++i)
                {
                    int index = _page * Pagination + i;
                    if (index >= count)
                    {
                        GUILayout.Label("");
                    }
                    else
                    {
                        GameObject childObject = _inspectedObject.transform.GetChild(index).gameObject;
                        if (GUILayout.Button(childObject.name, ButtonMedium))
                        {
                            targetObject = childObject;
                        }
                    }
                }
                if (_inspectedObject.transform.childCount <= 0)
                    GUILayout.Label("This GameObject has no child");
            }

            Hierarchy_Pagination(count);

            GUILayout.EndVertical();
            GUILayout.BeginVertical(GUI.skin.box);
            if (_inspectedObject == null)
            {
                GUILayout.Label("Select a game object to inspect");
            }
            else
            {
                GUILayout.Label("Inspecting " + _inspectedObject.name);
                bool toggleGO = GUILayout.Button("Toggle Active [" + _inspectedObject.activeSelf + "]");
                if (toggleGO)
                    _inspectedObject.SetActive(!_inspectedObject.activeSelf);

                for(int i = 0; i < _inspectBasicComponents.Length; ++i)
                {
                    GUILayout.Label(_inspectBasicComponents[i]);
                }
                for(int i = 0; i < _additionalData.Count; ++i)
                {
                    GUILayout.Label(_additionalData[i]);
                }
            }

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            if (targetObject != null)
            {
                Hierarchy_InspectObject(targetObject);
            }
        }

        private void Hierarchy_InspectObject(GameObject go)
        {
            _inspectedObject = go;
            _inspectedPath = GOResolver.GetPath(go);
            Hierarchy_BasicTestObjects(go);
        }

        private void Hierarchy_Pagination(int count)
        {
            bool guiState = GUI.enabled;

            if (count < Pagination)
                return;

            int pages = count / Pagination;
            if (pages * Pagination < count)
                pages++;


            GUI.enabled = guiState && _page > 0;
            GUILayout.BeginHorizontal(GUI.skin.box);
            if (GUILayout.Button("<", ButtonVerySmall))
                _page--;

            GUI.enabled = guiState;
            for (int i = 0; i < pages; ++i)
            {
                if (GUILayout.Button(i.ToString(), ButtonVerySmall))
                {
                    _page = i;
                }
            }

            GUI.enabled = guiState && _page < pages - 1;
            if (GUILayout.Button(">", ButtonVerySmall))
            {
                _page++;
            }
            GUILayout.EndHorizontal();
        }

        private void Hierarchy_BasicTestObjects(GameObject go)
        {
            Text cText; TMP_Text cText2; Image cImage;

            List<string> foundComponents = new List<string>();
            _additionalData.Clear();
            GDebug.Log(GOResolver.GetPath(go));
            if (go != null)
            {
                for(int i = 0; i < BasicComponents.Length; ++i)
                {
                    Component c = go.GetComponentByName(BasicComponents[i].Name);
                    if (c != null)
                    {
                        foundComponents.Add(BasicComponents[i].Name);
                        if ((cText = c.TryCast<Text>()) != null)
                        {
                            _additionalData.Add($"Text: {cText.text}");
                        }
                        else if ((cText2 = c.TryCast<TMP_Text>()) != null)
                        {
                            _additionalData.Add($"Text (TMP): {cText2.text}");
                        }
                        else if ((cImage = c.TryCast<Image>()) != null && cImage.sprite != null)
                        {
                            _additionalData.Add($"Image: {cImage.sprite.name}");
                            if (!string.IsNullOrEmpty(cImage.sprite.name))
                                GDebug.Log("sprite: " + cImage.sprite.name);
                        }
                    }
                }
            }
            _inspectBasicComponents = foundComponents.ToArray();
        }
    }
}
