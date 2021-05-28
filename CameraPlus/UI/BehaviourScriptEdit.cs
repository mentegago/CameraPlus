using System;
using System.Collections.Generic;
using UnityEngine;
using CameraPlus.Behaviours;
using CameraPlus.Utilities;
using CameraPlus.Configuration;

namespace CameraPlus.UI
{
    internal class BehaviourScriptEdit
    {
        private CameraPlusBehaviour _parentBehaviour;
        private Rect _windowRect = new Rect(Screen.width / 2 - 305, Screen.height - 345, 610, 325);
        private List<JSONMovement> _movementJson = new List<JSONMovement>();

        internal void DisplayUI(CameraPlusBehaviour parentBehaviour)
        {
            _parentBehaviour = parentBehaviour;
            _windowRect = GUI.Window(0, _windowRect, ScriptEditWindow, "Script Edit");
        }

        private void ScriptEditWindow(int windowID)
        {
            GUI.Box(new Rect(5, 20, 200, 50), "Script");
            GUI.Button(new Rect(5, 40, 100, 30), "Add Section");
            GUI.Button(new Rect(105, 40, 100, 30), "RemoveSelect");

            GUI.Box(new Rect(5, 70, 200, 50), "Script Section");
            GUI.Button(new Rect(5, 90, 50, 30), "<");
            GUI.Box(new Rect(55, 90, 100, 30), "0 / 0");
            GUI.Button(new Rect(155, 90, 50, 30), ">");

            GUI.Box(new Rect(5, 120, 200, 50), "Duration\n0");
            GUI.HorizontalSlider(new Rect(15, 155, 180, 30), 0, 0f, 180f);

            GUI.Box(new Rect(5, 170, 200, 50), "Delay\n0");
            GUI.HorizontalSlider(new Rect(15, 205, 180, 30), 0, 0f, 180f);

            GUI.Box(new Rect(5, 220, 200, 50), "EaseTransition");
            GUI.Button(new Rect(5, 240, 100, 30), "Enable");
            GUI.Button(new Rect(105, 240, 100, 30), "Disable");

            GUI.Box(new Rect(5, 270, 200, 50), "Turn to Head");
            GUI.Button(new Rect(5, 290, 100, 30), "Enable");
            GUI.Button(new Rect(105, 290, 100, 30), "Disable");

            GUI.Box(new Rect(205, 20, 200, 50), "Store pos / rot");
            GUI.Button(new Rect(205, 40, 100, 30), "Set Start");
            GUI.Button(new Rect(305, 40, 100, 30), "Set End");

            GUI.Box(new Rect(405, 20, 200, 50), "Play");
            GUI.Button(new Rect(410, 40, 60, 30), "Stop");
            GUI.Button(new Rect(475, 40, 60, 30), "Section");
            GUI.Button(new Rect(540, 40, 60, 30), "PlayAll");

            GUI.Box(new Rect(205, 70, 200, 50), $"Field of View\n{_parentBehaviour.GetFOV().ToString("F0")}");
            _parentBehaviour.FOV(GUI.HorizontalSlider(new Rect(215, 105, 180, 30), _parentBehaviour.GetFOV(), 0f, 180f));

            GUI.Box(new Rect(405, 80, 200, 120), "Position");
            GUI.Box(new Rect(405, 100, 200, 50), "PositionAmount");
            GUI.Button(new Rect(406, 120, 66, 30), "0.01");
            GUI.Button(new Rect(472, 120, 66, 30), "0.10");
            GUI.Button(new Rect(538, 120, 66, 30), "1.00");
            GUI.Box(new Rect(406, 150, 66, 50), $"x:{_parentBehaviour.ThirdPersonPos.x.ToString("F2")}");
            GUI.Button(new Rect(406, 170, 33, 30), "-");
            GUI.Button(new Rect(439, 170, 33, 30), "+");
            GUI.Box(new Rect(472, 150, 66, 50), $"y:{_parentBehaviour.ThirdPersonPos.y.ToString("F2")}");
            GUI.Button(new Rect(472, 170, 33, 30), "-");
            GUI.Button(new Rect(505, 170, 33, 30), "+");
            GUI.Box(new Rect(538, 150, 66, 50), $"z:{_parentBehaviour.ThirdPersonPos.z.ToString("F2")}");
            GUI.Button(new Rect(538, 170, 33, 30), "-");
            GUI.Button(new Rect(571, 170, 33, 30), "+");

            GUI.Box(new Rect(405, 200, 200, 120), "Rotation");
            GUI.Box(new Rect(405, 220, 200, 50), "RotateAmount");
            GUI.Button(new Rect(405, 240, 40, 30), "0.01");
            GUI.Button(new Rect(445, 240, 40, 30), "0.10");
            GUI.Button(new Rect(485, 240, 40, 30), "1.00");
            GUI.Button(new Rect(525, 240, 40, 30), "10");
            GUI.Button(new Rect(565, 240, 40, 30), "45");
            GUI.Box(new Rect(406, 270, 66, 50), $"x:{_parentBehaviour.ThirdPersonRot.x.ToString("F2")}");
            GUI.Button(new Rect(406, 290, 33, 30), "-");
            GUI.Button(new Rect(439, 290, 33, 30), "+");
            GUI.Box(new Rect(472, 270, 66, 50), $"y:{_parentBehaviour.ThirdPersonRot.y.ToString("F2")}");
            GUI.Button(new Rect(472, 290, 33, 30), "-");
            GUI.Button(new Rect(505, 290, 33, 30), "+");
            GUI.Box(new Rect(538, 270, 66, 50), $"z:{_parentBehaviour.ThirdPersonRot.z.ToString("F2")}");
            GUI.Button(new Rect(538, 290, 33, 30), "-");
            GUI.Button(new Rect(571, 290, 33, 30), "+");

            GUI.DragWindow(new Rect(0, 0, 610, 20));
        }
    }
}
