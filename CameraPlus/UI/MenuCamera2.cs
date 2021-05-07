using System;
using System.Text.RegularExpressions;
using UnityEngine;
using CameraPlus.Camera2Utils;
using CameraPlus.Behaviours;
using CameraPlus.Utilities;

namespace CameraPlus.UI
{
    internal class MenuCamera2
    {
        internal void DiplayMenu(CameraPlusBehaviour parentBehaviour, ContextMenu contextMenu, Vector2 menuPos)
        {
            GUI.Box(new Rect(menuPos.x, menuPos.y + 25, 300, 120), "Select scene to import from Camera2");
            if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 50, 140, 25), new GUIContent("<")))
                Camera2ConfigExporter.TrySceneSetLast(Camera2ConfigExporter.currentlyScenesSelected);
            if (GUI.Button(new Rect(menuPos.x + 155, menuPos.y + 50, 140, 25), new GUIContent(">")))
                Camera2ConfigExporter.SetSceneNext(Camera2ConfigExporter.currentlyScenesSelected);
            if (GUI.Button(new Rect(menuPos.x + 30, menuPos.y + 80, 230, 60), new GUIContent("Currently Selected:\n" + Camera2ConfigExporter.currentlyScenesSelected)))
                Camera2ConfigExporter.SetSceneNext(Camera2ConfigExporter.currentlyScenesSelected);

            GUI.Box(new Rect(menuPos.x, menuPos.y + 160, 300, 120), "Select profile Export to Scene in Camera2");
            if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 185, 140, 25), new GUIContent("<")))
                CameraUtilities.TrySetLast(CameraUtilities.currentlySelected);
            if (GUI.Button(new Rect(menuPos.x + 155, menuPos.y + 185, 140, 25), new GUIContent(">")))
                CameraUtilities.SetNext(CameraUtilities.currentlySelected);
            if (GUI.Button(new Rect(menuPos.x + 30, menuPos.y + 215, 230, 60), new GUIContent("Currently Selected:\n" + CameraUtilities.currentlySelected)))
                CameraUtilities.SetNext(CameraUtilities.currentlySelected);

            if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 290, 295, 25), new GUIContent("Export to Selected Scene")))
                Camera2ConfigExporter.ExportCamera2Scene();

            if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 320, 295, 25), new GUIContent("Import to New Profile")))
                Camera2ConfigExporter.LoadCamera2Scene();


            if (GUI.Button(new Rect(menuPos.x, menuPos.y + 430, 300, 30), new GUIContent("Close Camera2 Convert Menu")))
                contextMenu.MenuMode = ContextMenu.MenuState.MenuTop;

        }

    }
}
