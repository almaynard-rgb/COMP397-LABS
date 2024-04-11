using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public static class Vector3Extension
{
    public static Vector3 With(this Vector3 vector, float? x =null, float? y = null, float? z = null)
    {
        //?? means if it is nul then
        return new Vector3(x ?? vector.x, y ?? vector.y, z ?? vector.z);
    }
}
