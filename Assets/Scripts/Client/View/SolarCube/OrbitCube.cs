using UnityEngine;

namespace CCGP.Client
{
    public class OrbitCube : MonoBehaviour
    {
        [Header("원 운동 설정")]
        public float radius = 5f;
        
        [Header("회전 속도(각속도)")]
        public float speed = 1f;

        [Header("시작 각도 (라디안)")]
        public float startAngle = 0f;

        [Header("y축으로 얼마나 올려놓을지")]
        public float zOffset = 0f;

        private float currentAngle;

        private void Start()
        {
            // 초기 각도 설정
            currentAngle = startAngle;
            SetLocalPosition();
        }

        private void Update()
        {
            // 매 프레임 각도 증가 (speed는 초당 라디안 기준)
            currentAngle += speed * Time.deltaTime;
            SetLocalPosition();
        }

        /// <summary>
        /// 부모의 로컬 좌표계에서 x-y 평면을 기준으로 원궤도 위치를 설정
        /// </summary>
        private void SetLocalPosition()
        {
            // x-y 평면에서 원을 그리도록 계산
            float x = Mathf.Cos(currentAngle) * radius;
            float y = Mathf.Sin(currentAngle) * radius;

            // y값은 별도로 설정 (고정 높이 yOffset)
            transform.localPosition = new Vector3(x, y, zOffset);
        }
    }
}