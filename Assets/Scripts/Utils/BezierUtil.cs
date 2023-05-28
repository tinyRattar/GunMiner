using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierUtil
{
    public static Vector3 BezierLerp(Vector3 src, Vector3 mid, Vector3 tar, float scale)
    {
        Vector3 aa = src + (mid - src) * scale;
        Vector3 bb = mid + (tar - mid) * scale;
        Vector3 ret = aa + (bb - aa) * scale;
        return ret;
    }
}
