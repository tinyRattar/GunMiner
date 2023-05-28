using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShock : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] float shockTime;
    float timer;
    
    public void SetInShock(bool flag)
    {
        if (flag)
        {
            timer = shockTime;
            animator.Play("shock");
        }
        else
            animator.Play("idle");
    }

    private void Update()
    {
        if (timer > 0.0f)
        {
            timer -= Time.deltaTime;
            if (timer < 0.0f)
                SetInShock(false);
        }
    }
}
