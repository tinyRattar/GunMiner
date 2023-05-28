using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvBubble : MonoBehaviour
{
    [SerializeField] float seaY;
    [SerializeField] float ySpeed;
    [SerializeField] float timeChangeDirectMin;
    [SerializeField] float timeChangeDirectMax;
    float timerChangeDirect;
    [SerializeField] float xSpeedMin;
    [SerializeField] float xSpeedMax;
    float xSpeed;
    [SerializeField] Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator.speed = ySpeed / (seaY - this.transform.position.y);
    }

    // Update is called once per frame
    void Update()
    {
        if (this.transform.position.y >= seaY)
            Destroy(this.gameObject);
        else
        {
            if (timerChangeDirect < 0.0f)
            {
                timerChangeDirect = UnityEngine.Random.Range(timeChangeDirectMin, timeChangeDirectMax);
                xSpeed = UnityEngine.Random.Range(xSpeedMin, xSpeedMax);
                xSpeed = UnityEngine.Random.Range(0, 2) == 0 ? xSpeed : -xSpeed;
            }
            else
            {
                timerChangeDirect -= Time.deltaTime;
            }

            this.transform.Translate(new Vector3(xSpeed, ySpeed, 0) * Time.deltaTime);
        }
    }
}
