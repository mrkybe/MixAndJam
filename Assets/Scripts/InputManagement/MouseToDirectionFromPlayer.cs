using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.InputSystem;
#if UNITY_EDITOR
using UnityEngine.InputSystem.Editor;
#endif
using UnityEngine.SceneManagement;

namespace Assets.InputManagement
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public class MouseToDirectionFromPlayer : InputProcessor<Vector2>
    {
        public float Deadzone;
        public float MaxDistance;
        Transform ReferenceTransform;
        Camera ReferenceCamera;
        Plane GroundPlane;
#if UNITY_EDITOR
        static MouseToDirectionFromPlayer()
        {
            Initialize();
        }
#endif
        [RuntimeInitializeOnLoadMethod]
        static void Initialize()
        {
            InputSystem.RegisterProcessor<MouseToDirectionFromPlayer>();
        }

        public MouseToDirectionFromPlayer()
        {
            GroundPlane = new Plane(Vector3.forward, 0);
        }

        Vector2 result = new Vector2();
        Ray ray;
        float distance;
        public override Vector2 Process(Vector2 value, InputControl control)
        {
            if (ReferenceCamera == null || ReferenceTransform == null)
            {
                ReferenceCamera = Camera.main;
                ReferenceTransform = PlayerController.Instance.transform;
            }
            ray = ReferenceCamera.ScreenPointToRay(value);
            GroundPlane.Raycast(ray, out distance);
            var pos = ray.GetPoint(distance);
            var playerPos = ReferenceTransform.position;
            var dir = pos - playerPos;
            if (dir.magnitude < Deadzone)
            {
                result.x = 0;
                result.y = 0;
            }
            else
            {
                dir = dir - dir.normalized * Deadzone;
                if (dir.magnitude > MaxDistance)
                {
                    dir = dir.normalized * MaxDistance;
                }
            }

            dir /= MaxDistance;
            result.x = dir.x;
            result.y = dir.z;
            return result;
        }
    }

#if UNITY_EDITOR
    public class MouseInputProcessorEditor : InputParameterEditor<MouseToDirectionFromPlayer>
    {
        private GUIContent m_DeadzoneLabel = new GUIContent("Deadzone");
        private GUIContent m_DistanceForMaxValueLabel = new GUIContent("Distance for max value");

        protected override void OnEnable()
        {
            // Put initialization code here. Use 'target' to refer
            // to the instance of MyValueShiftProcessor that is being
            // edited.
        }

        public override void OnGUI()
        {
            // Define your custom UI here using EditorGUILayout.
            target.Deadzone = EditorGUILayout.Slider(m_DeadzoneLabel, target.Deadzone, 0, 5);
            target.MaxDistance = EditorGUILayout.Slider(m_DistanceForMaxValueLabel, target.MaxDistance, target.Deadzone, 10);
        }
    }
#endif
}
