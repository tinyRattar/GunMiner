using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAdaptScreen : MonoBehaviour
{
    [SerializeField] float defaultSize;
    Camera _camera;
    // Start is called before the first frame update
    void Start()
    {
        _camera = this.GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        float tarScale;
        float scale = (float)Screen.height / (float)Screen.width;
        if (scale > 9.0f / 16.0f)
        {
            tarScale = defaultSize / 9.0f * 16.0f * scale;
        }
        else
        {
            tarScale = defaultSize;
        }
        _camera.orthographicSize = tarScale;
    }
}
