using UnityEngine;

public static class Vector3Overload
{
    public static Vector3 Multiply(this Vector3 v0, Vector3 v1)
    {
        float x = v0.x * v1.x;
        float y = v0.y * v1.y;
        float z = v0.z * v1.z;
        return new Vector3(x, y, z);
    }
}
