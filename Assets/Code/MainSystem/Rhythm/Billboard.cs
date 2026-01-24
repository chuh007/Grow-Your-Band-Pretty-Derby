using UnityEngine;

namespace Code.MainSystem.Rhythm
{
    public class Billboard : MonoBehaviour
    {
        private Transform _cameraTransform;

        private void Start()
        {
            if (Camera.main != null)
            {
                _cameraTransform = Camera.main.transform;
            }
        }

        private void LateUpdate()
        {
            if (_cameraTransform == null)
            {
                if (Camera.main != null) _cameraTransform = Camera.main.transform;
                else return;
            }

            transform.LookAt(_cameraTransform);

            Vector3 euler = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(0f, euler.y, 0f);
        }
    }
}
