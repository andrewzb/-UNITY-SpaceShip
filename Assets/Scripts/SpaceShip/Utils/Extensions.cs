using System.Collections.Generic;
using UnityEngine;

namespace SpaceShip.Utils
{
    public static class Extensions
    {
        public static bool IsInCircle(this Vector2 p, Vector2 c, float r)
        {
            return (p - c).sqrMagnitude < r * r;
        }

        public static bool IsInSphere(this Vector3 p, Vector3 c, float r)
        {
            return (p - c).sqrMagnitude < r * r;
        }
        
        public static float GetAngle(this Vector2 p)
        {
            return Mathf.Atan2(p.y, p.x) * 180 / Mathf.PI;
        }

        public static float GetAngle(this Vector3 p)
        {
            return Mathf.Atan2(p.y, p.x) * 180 / Mathf.PI;
        }

        public static float ClampAngle(this float angle, float min, float max)
        {
            var a = angle;
            if (angle < -360)
                angle += 360;
            if (angle > 360)
                angle -= 360;
            return Mathf.Clamp(angle, min, max);
        }

        public static int ClampListIndex<T>(this List<T> list, int index)
        {
            var count = list.Count;
            var relativeCount = index % count;
            return relativeCount > -1
                ? relativeCount
                : count + relativeCount;
        }

        public static T ClampAndPull<T>(this List<T> list, int index)
        {
            var count = list.Count;
            var relativeCount = index % count;
            var clampIndex = relativeCount > -1
                ? relativeCount
                : count + relativeCount;
            return list[clampIndex];
        }

        public static Vector2 GetOrthographicCameraSize(this Camera camera)
        {
            var y = 2f * camera.orthographicSize;
            var x = y * camera.aspect;
            return new Vector2(x, y);
        }
    }
}