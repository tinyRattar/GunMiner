using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : Singleton<EffectManager>
{
    [SerializeField] EffectLootAbsorb effectLootAbsorb;
    [SerializeField] EffectMagnetClaw effectMagnetClaw;
    float timerFastFire;
    [SerializeField] float fastFireSpeedScale;
    float timerDoubleBullet;
    [SerializeField] EffectVisual effectFastClaw;
    [SerializeField] float fastClawSpeedScale;
    float timerFastClaw;

    public float FastFireSpeedScale { get => fastFireSpeedScale; set => fastFireSpeedScale = value; }
    public float FastClawSpeedScale { get => fastClawSpeedScale; set => fastClawSpeedScale = value; }

    public void ActivateLootAbsorb(float lastTime)
    {
        effectLootAbsorb.ActivateEffcet(lastTime);
    }

    public void ActivateMagnetClaw(float lastTime)
    {
        effectMagnetClaw.ActivateEffcet(lastTime);
    }

    public void ActivateFastFire(float lastTime)
    {
        timerFastFire = lastTime;
    }
    public void ActivateDoubleBullet(float lastTime)
    {
        timerDoubleBullet = lastTime;
    }
    public void ActivateFastClaw(float lastTime)
    {
        timerFastClaw = lastTime;
        effectFastClaw.ActivateEffcet(lastTime);
    }

    public float GetTimerFastFire()
    {
        return timerFastFire;
    }

    public float GetTimerDoubleBullet()
    {
        return timerDoubleBullet;
    }

    public float GetTimerFastClaw()
    {
        return timerFastClaw;
    }


    private void Update()
    {
        if (timerFastFire > 0.0f)
        {
            timerFastFire -= Time.deltaTime;
        }

        if (timerDoubleBullet > 0.0f)
        {
            timerDoubleBullet -= Time.deltaTime;
        }

        if (timerFastClaw > 0.0f)
        {
            timerFastClaw -= Time.deltaTime;
        }
    }
}
