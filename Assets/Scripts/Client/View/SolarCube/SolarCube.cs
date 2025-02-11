using UnityEngine;

namespace CCGP.Client
{
    public class SolarCube : MonoBehaviour
    {
        [SerializeField] private float RotationSpeed = 10.0f;

        void Update()
        {
            // x축을 기준으로 rotationSpeed만큼 초당 회전시킴
            transform.rotation *= Quaternion.Euler(0f, 0f, RotationSpeed * Time.deltaTime);
        }
    }
}