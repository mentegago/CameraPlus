using UnityEngine;
using System.Collections;
using System.IO;
using System.Reflection;

namespace CameraPlus
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
            get{
                return _isBackgroundTransparent;
            }
            set{
                _isBackgroundTransparent = value;
                if(_isBackgroundTransparent)
                {
                    if(_transparencyShader == null)
                    {
                        byte[] shaderRaw = Utils.GetResource(Assembly.GetCallingAssembly(), "CameraPlus.Resources.alphafilter");
                        AssetBundle bundle = AssetBundle.LoadFromMemory(shaderRaw);
                        _transparencyShader = bundle.LoadAsset<Material>("Assets/Materials/alphafilter.mat");
                    }
                }
            }
        }
        private bool isSnapping = true;
        IEnumerator takeSnapshot(float waitTime) //useful for debugging
        {
            isSnapping = true;
            yield return new WaitForSeconds(waitTime);

            RenderTexture.active = _renderTexture;
            Texture2D snap = new Texture2D(_renderTexture.width,_renderTexture.height,TextureFormat.ARGB32, false);
            snap.ReadPixels(new Rect(0,0,_renderTexture.width,_renderTexture.height),0,0);
            RenderTexture.active = null;

            byte[] bytes;
            bytes = snap.EncodeToPNG();
            System.IO.File.WriteAllBytes(string.Format("snap{0}.png",(int)Random.Range(1,100)), bytes );
            isSnapping = false;
        } 

        public void SetRenderTexture(RenderTexture renderTexture)
        {
            _renderTexture = renderTexture;
            //StartCoroutine(takeSnapshot(5));
        }

        public void SetCameraInfo(Vector2 position, Vector2 size, int layer)
        {
            _cam.pixelRect = new Rect(position, size);
            _cam.depth = layer;
        }

        public void Awake()
        {
            Logger.Log("Created new screen camera behaviour component!");
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
