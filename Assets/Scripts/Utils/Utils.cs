using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
    static public void FlipGameObject(GameObject go, bool flag)
    {
        Vector3 localScale = go.transform.localScale;
        localScale.x = flag ? 1 : -1;
        go.transform.localScale = localScale;
    }

    static public void FlipGameObjectY(GameObject go, bool flag)
    {
        Vector3 localScale = go.transform.localScale;
        localScale.y = flag ? 1 : -1;
        go.transform.localScale = localScale;
    }

    static public ContactFilter2D InitContactFilter(LayerMask targetMask, bool useTrigger)
    {
        ContactFilter2D targetFilter = new ContactFilter2D();
        targetFilter.SetLayerMask(targetMask);
        targetFilter.useTriggers = useTrigger;

        return targetFilter;
    }
}
