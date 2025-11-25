using System;
using System.Collections.Generic;
using UnityEngine;

namespace EOAP.Plugin.AP
{
    public static class APCanvasRipper
    {
        private static Dictionary<int, Canvas> s_CanvasList;
        static APCanvasRipper()
        {
            s_CanvasList = new Dictionary<int, Canvas>();
        }


        public static void RenameAllCanvas()
        {
            //var newCanvases = GameObject.FindObjectsOfType<Canvas>();
            //for(int i = 0; i < newCanvases.Length; ++i)
            //{
            //    GameObject canvasGO = newCanvases[i].gameObject;
            //    if (!s_CanvasList.ContainsKey(canvasGO.GetInstanceID()))
            //    {
            //        string oldName = canvasGO.name;
            //        canvasGO.name = "Canvas_" + s_CanvasList.Count.ToString();
            //        s_CanvasList.Add(canvasGO.GetInstanceID(), newCanvases[i]);
            //        GDebug.Log($"Found Canvas {oldName}: Renamed to {canvasGO.name}");
            //    }
            //}
        }

        internal static void PrintCanvas()
        {
            var newCanvases = GameObject.FindObjectsOfType<Canvas>();
            for (int i = 0; i < newCanvases.Length; ++i)
            {
                GDebug.Log(newCanvases[i].gameObject.name + " (Active: "+ newCanvases[i].gameObject.activeSelf +" )");
            }
        }
    }
}
