using EOAP.Plugin;
using System.Collections.Generic;
using UnityEngine;

namespace Dirt.Hackit
{
    public static class GOResolver
    {
        private static Dictionary<string, GameObject> s_rootObjectCache;

        static GOResolver()
        {
            s_rootObjectCache = new Dictionary<string, GameObject>();
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
        public static GameObject Resolve(string path)
        {
            string[] hierarchy = path.Split('.');

            if (hierarchy.Length == 0)
            {
                GDebug.LogError("Cannot resolve invalid path");
                return null;
            }

            int index = 0;

            if (s_rootObjectCache.TryGetValue(hierarchy[0], out GameObject current))
            {
                index++;
            }

            for(; index < hierarchy.Length; ++index)
            {
                if (index == 0)
                {
                    current = GameObject.Find(hierarchy[index]);

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
                    for(; j < current.transform.childCount; ++j)
                    {
                        if (current.transform.GetChild(j).gameObject.name == hierarchy[index])
                        {
                            current = current.transform.GetChild(j).gameObject;
                            break;
                        }
                    }
                    if (j >= current.transform.childCount)
                    {
                        GDebug.LogError("Invalid Hierarchy, unknown children {0}", hierarchy[index]);
                        return null;
                    }
                }
            }

            return current;
        }

    }
}
