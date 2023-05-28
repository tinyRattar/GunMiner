using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectVisual : MonoBehaviour
{
    [SerializeField] GameObject goVisualFX;
    float timerWork;

    public void ActivateEffcet(float lastTime)
    {
        timerWork = lastTime;
        goVisualFX.SetActive(true);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (timerWork > 0)
        {
            timerWork -= Time.deltaTime;
            Claw claw = MinerManager.Instance.GetMiner().GetCurClaw();
            this.transform.position = claw.GetClawHeadPos();
            this.transform.rotation = claw.transform.rotation;
        }
        else
        {
            goVisualFX.SetActive(false);

        }
    }
}
