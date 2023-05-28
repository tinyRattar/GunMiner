using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum UIMobType
{
    exitGame,
    setting
}

public class UIMob : Mob
{
    [SerializeField] UIMobType UIMobType;
    [SerializeField] float recoverDelay;
    float timerDelay;
    [SerializeField] float recoverRate;
    [SerializeField] Image uiHealthBar;
    public override bool OnHit(DamageInfo info)
    {
        timerDelay = recoverDelay;
        return base.OnHit(info);
    }
    public override void StartDead()
    {
        switch (UIMobType)
        {
            case UIMobType.exitGame:
                MainManager.Instance.ExitGame();
                break;
            case UIMobType.setting:
                MainManager.Instance.OpenSettingMenu();
                break;
            default:
                break;
        }
        curState = MobState.dead;
    }
    private void Recover()
    {
        if (timerDelay > 0.0f)
        {
            timerDelay -= Time.deltaTime;
        }
        else
        {
            curHealth += recoverRate * Time.deltaTime;

            if (curHealth > maxHealth) curHealth = maxHealth;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    private void FixedUpdate()
    {
        BasicBehavior();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSR();
        Recover();
        uiHealthBar.fillAmount = curHealth / maxHealth;
    }
}
