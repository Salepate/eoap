using EOAP.Plugin;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Dirt.Hackit
{
    public static class GOResolver
    {
        private static Dictionary<string, GameObject> s_rootObjectCache;
        private static GameObject[] _sceneRootObjects;
        private static int _activeScene;
        static GOResolver()
        {
            s_rootObjectCache = new Dictionary<string, GameObject>();
            _activeScene = -1;
        }

        public static string GetPath(GameObject go)
        {
            string path = string.Empty;

            while(go != null)
            {
                if (!string.IsNullOrEmpty(path))
                    path = '.' + path;

                path = go.name + path;
                Transform parent = go.transform.parent;
                go = parent != null ? parent.gameObject : null;
            }
            
            return path;
        }
        public static T Resolve<T>(string path) where T: Component
        {
            GameObject targetObject = Resolve(path);
            T comp = null;
            if (targetObject != null)
            {
                comp = targetObject.GetComponent<T>();
                if (comp == null)
                {
                    GDebug.LogError("Could not find component {0} on {1}", typeof(T).Name, targetObject.name);
                }
            }

            return comp;
        }

        public static void ResetScene() => _activeScene = -1;

        public static GameObject Resolve(string path)
        {
            string[] hierarchy = path.Split('.');

            if (hierarchy.Length == 0)
            {
                GDebug.LogError("Cannot resolve invalid path");
                return null;
            }

            int index = 0;

            if (_activeScene != SceneManager.GetActiveScene().buildIndex)
            {
                s_rootObjectCache.Clear();
                _sceneRootObjects = GetSceneRootObjects();
                _activeScene = SceneManager.GetActiveScene().buildIndex;
            }

            if (s_rootObjectCache.TryGetValue(hierarchy[0], out GameObject current))
            {
                index++;
            }

            for (; index < hierarchy.Length; ++index)
            {
                if (index == 0)
                {
                    bool found = false;
                    for(int j = 0; !found && j < _sceneRootObjects.Length; ++j)
                    {
                        if (_sceneRootObjects[j].name == hierarchy[index])
                        {
                            current = _sceneRootObjects[j];
                            found = true;
                        }
                    }

                    if (!found)
                    {
                        GameObject[] dontDestroyObjects = GetDontDestroyOnLoadObjects();
                        for(int j = 0; !found && j < dontDestroyObjects.Length; ++j)
                        {
                            if (dontDestroyObjects[j].name == hierarchy[index])
                            {
                                current = dontDestroyObjects[j];
                                found = true;
                            }
                        }
                    }

                    if (current != null)
                    {
                        if (current.transform.parent == null)
                        {
                            s_rootObjectCache.Add(hierarchy[index], current);
                        }
                    }
                    else
                    {
                        GDebug.LogError("Invalid Hierarchy, unknown root object {0}", hierarchy[index]);
                        break;
                    }
                }

                if (index > 0)
                {
                    int j = 0;
                    int childCount = current.transform.childCount;
                    for(; j < childCount; ++j)
                    {
                        if (current.transform.GetChild(j).gameObject.name == hierarchy[index])
                        {
                            current = current.transform.GetChild(j).gameObject;
                            break;
                        }
                    }
                    if (j >= childCount)
                    {
                        GDebug.LogError("Invalid Hierarchy, unknown children {0}", hierarchy[index]);
                        return null;
                    }
                }
            }

            return current;
        }


        public static GameObject[] GetDontDestroyOnLoadObjects()
        {
            GameObject temp = null;
            try
            {
                Il2CppSystem.Collections.Generic.List<GameObject> list = new Il2CppSystem.Collections.Generic.List<GameObject>();
                temp = new GameObject();
                Object.DontDestroyOnLoad(temp);
                UnityEngine.SceneManagement.Scene dontDestroyOnLoad = temp.scene;
                Object.DestroyImmediate(temp);
                temp = null;

                dontDestroyOnLoad.GetRootGameObjects(list);
                GameObject[] gos = new GameObject[list.Count];
                for (int i = 0; i < list.Count; ++i)
                {
                    gos[i] = list[i];
                }
                return gos;
            }
            finally
            {
                if (temp != null)
                    Object.DestroyImmediate(temp);
            }
        }


        public static GameObject[] GetSceneRootObjects()
        {
            Il2CppSystem.Collections.Generic.List<GameObject> list = new Il2CppSystem.Collections.Generic.List<GameObject>();
            SceneManager.GetActiveScene().GetRootGameObjects(list);
            GameObject[] gos = new GameObject[list.Count];
            for (int i = 0; i < list.Count; ++i)
            {
                gos[i] = list[i];
            }
            return gos;
        }
    }
}
