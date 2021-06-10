using UnityEngine;
using System.Collections;
using System.Reflection;
using CameraPlus.Utilities;

namespace CameraPlus.Behaviours
{
    /// <summary>
    /// This is the monobehaviour that goes on the camera that handles
    /// displaying the actual feed from the camera to the screen.
    /// </summary>
    public class ScreenCameraBehaviour : MonoBehaviour
    {
        private Camera _cam;
        private RenderTexture _renderTexture;
        private static Material _transparencyShader = null; //load once
        private bool _isBackgroundTransparent = false;
        public bool isBackgroundTransparent
        {
            set
            {
                _isBackgroundTransparent = value;
                if (_isBackgroundTransparent)
                {
                    if (_transparencyShader == null)
                    {
                        byte[] shaderRaw = CustomUtils.GetResource(Assembly.GetCallingAssembly(), "CameraPlus.Resources.alphafilter");
                        AssetBundle bundle = AssetBundle.LoadFromMemory(shaderRaw);
                        _transparencyShader = bundle.LoadAsset<Material>("Assets/Materials/alphafilter.mat");
                        DontDestroyOnLoad(_transparencyShader);
                    }
                }

            }
        }

        public void SetRenderTexture(RenderTexture renderTexture)
        {
            _renderTexture = renderTexture;
        }

        public void SetCameraInfo(Vector2 position, Vector2 size, int layer)
        {
            _cam.pixelRect = new Rect(position, size);
            _cam.depth = layer;
        }

        public void Awake()
        {
            Logger.log.Notice("Created new screen camera behaviour component!");
            DontDestroyOnLoad(gameObject);

            _cam = gameObject.AddComponent<Camera>();
            _cam.clearFlags = CameraClearFlags.Nothing;
            _cam.cullingMask = 0;
            _cam.stereoTargetEye = StereoTargetEyeMask.None;
        }
        
        private void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            if (_renderTexture == null) return;
            if (_isBackgroundTransparent) Graphics.Blit(_renderTexture, dest, _transparencyShader);
            else Graphics.Blit(_renderTexture, dest);
        }
    }
}
