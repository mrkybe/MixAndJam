
using UnityEngine;

namespace Assets.Scripts
{
    class CameraController : MonoBehaviour
    {
        [SerializeField]
        public GameObject Player;

        [SerializeField]
        public float ShakeIntensity;

        [SerializeField]
        public float GhostlyScreenshakeMaxDistance = 20f;

        [SerializeField]
        public float GhostlyScreenshakeIntensity = 5f;

        private void Update()
        {
            if (PlayerController.Instance.IsSprinting)
            {
                SetShake(1f * PlayerController.Instance.RemainingSprintPercent);
            }
            else
            {
                SetShake(0f);
            }
        }

        private float CalculateGhostlyScreenshake()
        {
            float totalScreenshake = 0f;
            foreach (GameObject g in PlayerController.Instance.GhostlyScreenShakeGhosts)
            {
                float distance = (g.transform.position - PlayerController.Instance.transform.position).magnitude;
                totalScreenshake += (Mathf.Clamp(GhostlyScreenshakeMaxDistance - distance, 0, GhostlyScreenshakeMaxDistance) / GhostlyScreenshakeMaxDistance) * GhostlyScreenshakeIntensity;
            }
            return totalScreenshake;
        }

        private void SetShake(float s)
        {
            s += CalculateGhostlyScreenshake();
            ShakeIntensity = s / 100f;
        }

        private void LateUpdate()
        {
            transform.position = Vector3.Lerp(Player.transform.position.WithZ(-10), transform.position.WithZ(-10), 1f - Time.deltaTime * 4f) + (Random.insideUnitCircle * ShakeIntensity).ToV3XY();
        }
    }
}
