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
    public class MouseToWorldPosition : InputProcessor<Vector2>
    {
        Transform ReferenceTransform;
        Camera ReferenceCamera;
        Plane GroundPlane;
#if UNITY_EDITOR
        static MouseToWorldPosition()
        {
            Initialize();
        }
#endif
        [RuntimeInitializeOnLoadMethod]
        static void Initialize()
        {
            InputSystem.RegisterProcessor<MouseToWorldPosition>();
        }

        public MouseToWorldPosition()
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
            result.x = pos.x;
            result.y = pos.y;
            //Debug.Log(result);
            return result;
        }
    }
}
