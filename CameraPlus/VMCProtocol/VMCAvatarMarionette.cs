using UnityEngine;

namespace CameraPlus.VMCProtocol
{
#if WithVMCAvatar
    public class VMCAvatarMarionette : MonoBehaviour
    {
        public Vector3 position;
        public Quaternion rotate;
        public float fov;
        public bool receivedData = false;
        public virtual void OnEnable()
        {
            if (Plugin.cameraController.existsVMCAvatar)
            {
                var vmcProtocol = GameObject.Find("VMCProtocol");
                if (vmcProtocol != null)
                {
                    var marionette = vmcProtocol.GetComponent<Marionette>();
                    marionette.OnCameraTransformAndFov.AddListener(OnCameraPosition);
                }
            }
        }

        private void OnCameraPosition(Vector3 _position, Quaternion _rotate, float _fov)
        {
            position = _position;
            rotate = _rotate;
            fov = _fov;
            receivedData = true;
        }
    }
#endif
}
