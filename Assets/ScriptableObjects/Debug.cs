using UnityEngine;
using UnityEngine.InputSystem;

namespace DebugDev
{
    public static class DebugFunction
    {
        public delegate void DebugCallback();
        private static DebugCallback debugCallback;
        static InputAction debugAction = null;
        public static void Start()
        {
            debugAction = InputSystem.actions.FindAction("Debug");
        }
        public static void RegisterDebugCallback(DebugCallback debugCallback)
        {
            DebugFunction.debugCallback = debugCallback;
        }
        public static void Update()
        {
            if (debugAction == null)
                return;

            if (debugAction.IsPressed())
                debugCallback?.Invoke();
        }
    };
    public static class DebugDraw
    {
        public static void DrawCheckBox(Vector3 center, Vector3 halfExtents)
        {
            Color color = Color.limeGreen;
            float durationSeconds = 10f;

            Vector3 BFR = center;
            BFR.y -= halfExtents.y;
            BFR.x += halfExtents.x;
            BFR.z += halfExtents.z;

            Vector3 BFL = BFR;
            BFL.x -= halfExtents.x * 2;

            Vector3 BBR = BFR;
            BBR.z -= halfExtents.z * 2;
            Vector3 BBL = BFL;
            BBL.z -= halfExtents.z * 2;

            Vector3 TFR = BFR;
            Vector3 TFL = BFL;
            Vector3 TBR = BBR;
            Vector3 TBL = BBL;
            TFR.y += halfExtents.y * 2;
            TFL.y += halfExtents.y * 2;
            TBR.y += halfExtents.y * 2;
            TBL.y += halfExtents.y * 2;

            Debug.DrawLine(BFR, BFL, color, durationSeconds, false);
            Debug.DrawLine(BFL, BBL, color, durationSeconds, false);
            Debug.DrawLine(BBL, BBR, color, durationSeconds, false);
            Debug.DrawLine(BBR, BFR, color, durationSeconds, false);

            Debug.DrawLine(TFR, TFL, color, durationSeconds, false);
            Debug.DrawLine(TFL, TBL, color, durationSeconds, false);
            Debug.DrawLine(TBL, TBR, color, durationSeconds, false);
            Debug.DrawLine(TBR, TFR, color, durationSeconds, false);

            Debug.DrawLine(BFR, TFR, color, durationSeconds, false);
            Debug.DrawLine(BFL, TFL, color, durationSeconds, false);
            Debug.DrawLine(BBR, TBR, color, durationSeconds, false);
            Debug.DrawLine(BBL, TBL, color, durationSeconds, false);
        }
    };
    public static class UnityDebugVisualise
    {
        public static bool Physics_CheckBox(Vector3 center, Vector3 halfExtents)
        {
            DebugDraw.DrawCheckBox(center, halfExtents);
            return Physics.CheckBox(center, halfExtents);
        }
        public static bool Physics_Raycast(Ray ray, out RaycastHit hitInfo, float maxDistance, int layerMask, int debugRayLength)
        {
            Debug.DrawRay(ray.origin, ray.direction, Color.red, debugRayLength);
            return Physics.Raycast(ray, out hitInfo, maxDistance, layerMask);
        }
    };
};
