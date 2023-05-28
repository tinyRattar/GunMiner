using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ParallaxBackground : MonoBehaviour
{
    [SerializeField] Vector2 parallaxMoveScale;
    [SerializeField] bool useRefer;
    [SerializeField] Vector3 referPos;
    Vector3 srcPos;
    Transform cameraTransform;
    Vector3 lastCameraPosition;

    public void SetReferPos()
    {
        referPos = Camera.main.transform.position;
        useRefer = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        cameraTransform = Camera.main.transform;
        lastCameraPosition = cameraTransform.position;
        srcPos = this.transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (useRefer)
        {
            Vector3 tarPosDelta = cameraTransform.position - referPos;
            tarPosDelta = tarPosDelta * parallaxMoveScale;
            this.transform.position = srcPos + tarPosDelta;
        }
        else
        {
            Vector3 deltaMove = cameraTransform.position - lastCameraPosition;
            lastCameraPosition = cameraTransform.position;

            this.transform.Translate(deltaMove * parallaxMoveScale);
        }
        
    }
}
