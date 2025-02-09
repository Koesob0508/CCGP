using UnityEngine;

namespace CCGP.Client
{
    /// <summary>
    /// 3D 공간에서 부모(이 스크립트를 부착한 객체)의 위치/회전을 기준으로
    /// 원 궤도를 한 번 그려주는 스크립트. (Start에서만 실행)
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    public class OrbitLineDrawer3D : MonoBehaviour
    {
        [Header("Orbit Settings")]
        [Tooltip("원 궤도의 반지름")]
        public float radius = 5f;

        [Tooltip("틸트")]
        public float zOffset;

        [Tooltip("원을 구성할 선분 개수 (값이 높을수록 더 매끄러운 원)")]
        public int lineSegments = 50;

        [Tooltip("라인 두께")]
        public float lineWidth = 0.05f;

        [Tooltip("라인 색상")]
        public Color orbitColor = Color.gray;

        private LineRenderer lineRenderer;

        void Start()
        {
            // LineRenderer 초기 설정
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.useWorldSpace = true; // 월드 좌표계로 그리기
            lineRenderer.loop = true;          // 원을 닫기
            lineRenderer.positionCount = lineSegments + 1;
            lineRenderer.widthMultiplier = lineWidth;

            // 간단한 머티리얼 셰이더 설정(필요에 따라 교체 가능)
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));

            lineRenderer.startColor = orbitColor;
            lineRenderer.endColor = orbitColor;

            // 부모가 움직이지도 않고 반경이 변하지 않으므로, Start에서 단 한 번만 궤도 계산

            var orbitData = GetComponent<OrbitCube>();
            radius = orbitData.radius;
            zOffset = orbitData.zOffset;

            DrawCircleAroundParent();
        }

        /// <summary>
        /// parentTransform.position을 중심으로
        /// x-y 평면에 radius 반경 원을 한 번 그려준다.
        /// </summary>
        private void DrawCircleAroundParent()
        {
            var transform = GetComponentInParent<Transform>();
            if (transform == null)
            {
                Debug.LogError("Parent Transform이 할당되지 않았습니다!");
                return;
            }

            Vector3 centerPos = transform.position;
            float angleStep = 2f * Mathf.PI / lineSegments;
            Vector3[] positions = new Vector3[lineSegments + 1];

            for (int i = 0; i <= lineSegments; i++)
            {
                float angle = angleStep * i;
                float x = Mathf.Cos(angle) * radius;
                float y = Mathf.Sin(angle) * radius;
                // z=0, x-y 평면
                positions[i] = centerPos + new Vector3(x, y, zOffset);
            }

            lineRenderer.SetPositions(positions);
        }
    }
}