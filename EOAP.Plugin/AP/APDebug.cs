using Dirt.Hackit;
using EOAP.Plugin.Behaviours;
using EOAP.Plugin.DB;
using EOAP.Plugin.Dirt;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Master;
using Newtonsoft.Json;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EOAP.Plugin.AP
{
    // IMGUI UI
    public class APDebug
    {
        private APUI _ui;
        private int _menuGeneral;
        private int _menuInspector;
        private int _menuDataRipper;
        private int _menuUI;

        public APDebug(APUI ui)
        {
            _ui = ui;
            _menuGeneral = ui.AddMenu(DrawGeneralDebugging, "Debug Menu");
            _menuInspector = ui.AddMenu(DrawHierarchy, "Hierachy");
            _menuDataRipper = ui.AddMenu(DrawDataRipper, "Data Ripper");
            _menuUI = ui.AddMenu(DrawUI, "UI");
        }

        //=============================================================
        // Window: General Debugging 
        //=============================================================
        private static int _lastActivatedFlag = 0;
        private void DrawGeneralDebugging(Rect pos, APUI ui)
        {
            EOSession session = APBehaviour.GetSession();
            EOPersistent persistent = APBehaviour.GetPersistent();
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical(GUILayout.Width(200f));
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
                APBehaviour.SavePersistentData();
            }
            GUILayout.EndVertical();
            GUILayout.BeginVertical(GUILayout.Width(200f));
            GUI.skin.label.fontStyle = FontStyle.Bold;
            GUILayout.Label("Flags");
            GUI.skin.label.fontStyle = FontStyle.Normal;
            GUILayout.Label($"Last Checked TreasureBox: {_lastTreasureBox}");
            GUILayout.Label($"Last Activated Flag: {_lastActivatedFlag}");
            if (GUILayout.Button("Reset All Known Flags"))
            {
                var flags = Builder.GetTable("Flags");
                for (int i = 0; i < flags.Count; ++i)
                    EventFlagTbl.SetEventFlag(flags[i].ID, false);
            }

            if (GUILayout.Button($"Show Flags in Console: [{EOConfig.PrintActivatedFlags}]"))
            {
                EOConfig.PrintActivatedFlags = !EOConfig.PrintActivatedFlags;
            }

            if (GUILayout.Button($"Show TBox ID in Console: [{EOConfig.PrintTreasureBoxId}]"))
            {
                EOConfig.PrintTreasureBoxId = !EOConfig.PrintTreasureBoxId;
            }
            GUILayout.EndVertical();


            GUILayout.BeginVertical(GUILayout.Width(200f));
            if (GUILayout.Button("Save DynDB"))
            {
                Builder.Dump(EOItems.DynDBPath);
            }
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        public static void PrintActivateFlag(int flagID)
        {
            _lastActivatedFlag = flagID;
            if (EOConfig.PrintActivatedFlags)
                GDebug.Log($"Activated Flag {flagID}");
        }

        public static void PrintTreasureBox(int tboxID)
        {
            _lastTreasureBox = tboxID;
            if (EOConfig.PrintActivatedFlags)
                GDebug.Log($"Last Treasure Box {_lastTreasureBox}");
        }

        //=============================================================
        // Window: Canvas Ripper
        //=============================================================
        private bool _includeInactives;
        private void DrawDataRipper(Rect pos, APUI ui)
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
                _ui.DisplayMenu(_menuInspector);
            }
        }

        //=============================================================
        // Window: Hierarchy Traversal
        //=============================================================
        private const int Pagination = 10;
        private static readonly System.Type[] BasicComponents = [typeof(Camera), typeof(Canvas), typeof(Image), typeof(Text), typeof(TMP_Text), typeof(CanvasGroup),typeof(Button)];
        private GameObject _inspectedObject;
        private int _page;
        private string _inspectedPath;
        private GameObject[] _rootObjects;
        private string[] _inspectBasicComponents = System.Array.Empty<string>();
        private List<string> _additionalData = new List<string>();
        private List<Behaviour> _inspectedBehaviours = new List<Behaviour>();
        private static int _lastTreasureBox;

        private void SetList(GameObject[] list)
        {
            _rootObjects = list;
            _inspectedObject = null;
            _page = 0;
        }
        private void DrawHierarchy(Rect pos, APUI ui)
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
                        if (StyleUI.ButtonM(_rootObjects[index].name))
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
                        if (StyleUI.ButtonM(childObject.name))
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

                for(int i = 0; i < _inspectedBehaviours.Count; ++i)
                {
                    Behaviour bhv = _inspectedBehaviours[i];
                    if (GUILayout.Button($"{bhv.GetIl2CppType().Name} [{bhv.enabled}]"))
                    {
                        bhv.enabled = !bhv.enabled;
                    }
                }

                if (GUILayout.Button("Full Dump"))
                {
                    var comps = _inspectedObject.GetComponents<Component>();
                    for(int i = 0; i < comps.Length; ++i)
                    {
                        GDebug.Log("Comp: " + comps[i].GetIl2CppType().Name);
                    }
                }

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
            _page = 0;
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
            if (StyleUI.ButtonXS("<"))
                _page--;

            GUI.enabled = guiState;
            for (int i = 0; i < pages; ++i)
            {
                if (StyleUI.ButtonXS(i.ToString()))
                {
                    _page = i;
                }
            }

            GUI.enabled = guiState && _page < pages - 1;
            if (StyleUI.ButtonXS(">"))
            {
                _page++;
            }
            GUILayout.EndHorizontal();
            GUI.enabled = guiState;
        }

        private void Hierarchy_BasicTestObjects(GameObject go)
        {
            Text cText; TMP_Text cText2; Image cImage;

            Il2CppArrayBase<Component> allComps = go.GetComponents<Component>();
            _inspectedBehaviours.Clear();
            for(int i = 0; i < allComps.Count; ++i)
            {
                Component comp = allComps[i];
                Behaviour bhv = comp.TryCast<Behaviour>();
                if (bhv != null)
                {
                    _inspectedBehaviours.Add(bhv);
                }
            }

            List<string> foundComponents = new List<string>();
            _additionalData.Clear();
            GDebug.Log(GOResolver.GetPath(go));
            if (go != null)
            {
                string layerName = LayerMask.LayerToName(go.layer);
                if (string.IsNullOrEmpty(layerName))
                    layerName = go.layer.ToString();
                else
                    layerName += $" ({go.layer})";

                _additionalData.Add(layerName);

                RectTransform rectTr = go.GetComponent<RectTransform>();
                if (rectTr != null)
                {
                    _additionalData.Add($"RectTr.anchorMin: {rectTr.anchorMin}");
                    _additionalData.Add($"RectTr.anchorMax: {rectTr.anchorMax}");
                    _additionalData.Add($"RectTr.anchoredPosition: {rectTr.anchoredPosition}");
                    _additionalData.Add($"RectTr.pivot: {rectTr.pivot}");
                    _additionalData.Add($"RectTr.sizeDelta: {rectTr.sizeDelta}");
                    _additionalData.Add($"RectTr.scale: {rectTr.localScale}");
                }

                for (int i = 0; i < BasicComponents.Length; ++i)
                {
                    Component c =  go.GetComponentByName(BasicComponents[i].Name);
                    if (c != null)
                    {
                        if ((cText = c.TryCast<Text>()) != null)
                        {
                            _additionalData.Add($"Text: {cText.text}");
                            _additionalData.Add($"Font: {cText.font.name}");
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

        //=============================================================
        // Window: Game UI
        //=============================================================

        private void DrawUI(Rect pos, APUI ui)
        {
            bool guiState = GUI.enabled;
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(200f));
            GUILayout.Label("Canvas");
            ShowActiveToggle(Shinigami.TitleUI, "Title UI");
            ShowActiveToggle(Shinigami.GameHUD, "Game HUD");
            ShowActiveToggle(Shinigami.InnUI, "Inn");
            ShowActiveToggle(Shinigami.InputTextWindow, "Input Text");
            GUILayout.EndVertical();
            GUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label("Sprites");
            ShowSpriteName(Shinigami.NotificationSprite);
            if (GUILayout.Button("Updates Refs"))
                Shinigami.SetupTownReferences();
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            if (GUILayout.Button("Test Keyboard"))
            {
                // Show Ingame Keyboard
                //TextInput tpInput = Il2CppSystem.Activator.CreateInstance<TextInput>();
                //tpInput.ShowKeyboard(null, null, 1, 1, true, true, "ok");
                Shinigami.InputTextWindow.Open(string.Empty, string.Empty, 64);
            }
            if (GUILayout.Button("Test Notification"))
            {
                switch(Random.Range(1, 3))
                {
                    default:
                        APBehaviour.PushNotification("This is a test notification"); break;
                    case 2:
                        APBehaviour.PushNotification("<b>A</b> <i>test</i> <color=#ffcc00>notification</color>"); break;
                }
            }
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            for (Shinigami.SFX sfx = Shinigami.SFX.SystemOk; sfx <= Shinigami.SFX.ClearMap; ++sfx)
            {
                if (GUILayout.Button(sfx.ToString()))
                {
                    EO1.PlaySFX(sfx);
                }
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUI.enabled = guiState;
        }


        private void ShowActiveToggle(Component comp, string defaultName = "")
        {
            GameObject go = comp != null ? comp.gameObject : null;
            ShowActiveToggle(go, defaultName);
        }

        private void ShowSpriteName(Sprite spr)
        {
            if (spr == null)
                GUILayout.Label("Unloaded Sprite");
            else
                GUILayout.Label(spr.name);
        }
        private void ShowActiveToggle(GameObject go, string defaultName = "")
        {
            bool guiState = GUI.enabled;
            bool goState = go != null ? go.activeSelf : false;
            string objectName = defaultName ?? (go != null ? go.name : "Unknown Object");
            string toggleName = $"{objectName} [{goState}]";
            GUI.enabled = guiState && go != null;
            if (GUILayout.Button(toggleName))
            {
                go.SetActive(!goState);
            }
            GUI.enabled = guiState;
        }
    }
}
