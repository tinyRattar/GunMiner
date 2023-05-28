using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeMob : Mob
{
    [Header("¹¥»÷")]
    [SerializeField] float minAttackY;
    [Header("µ¯Ä»")]
    [SerializeField] GameObject pfbBullet;
    [SerializeField] float bulletDamage;
    [SerializeField] float bulletSlowTime;
    [SerializeField] float timePrepare_aim;
    [SerializeField] float timePrepare_locked;
    [SerializeField] float timeFire;
    [SerializeField] GameObject goFirepoint;
    [SerializeField] int fireNum;
    [SerializeField] bool inverseSecondShoot;
    int curFireNum; //already shooted
    [SerializeField] float timeFireInterval;
    [SerializeField] bool reAimPerFire;
    Vector3 fireDirect;

    protected override void StartAttack()
    {
        curFireNum = 0;
        fireDirect = (followTarget.transform.position - goFirepoint.transform.position).normalized;
        animator.Play("attack");

        base.StartAttack();
    }

    protected override bool TryStartAttack()
    {
        if (this.transform.position.y < minAttackY) return false;
        return base.TryStartAttack();
    }

    protected override void Attacking()
    {
        if (timerAttack < timePrepare_aim)
        {
            fireDirect = (followTarget.transform.position - goFirepoint.transform.position).normalized;
            Utils.FlipGameObject(topSR, fireDirect.x < 0);
        }
        else if (timerAttack < timePrepare_aim + timePrepare_locked)
        {
            // do nothing
            Utils.FlipGameObject(topSR, fireDirect.x < 0);
        }
        else if (timerAttack < timePrepare_aim + timePrepare_locked + timeFire)
        {
            if (curFireNum < fireNum)
            {
                if (timerAttack > timePrepare_aim + timePrepare_locked + curFireNum * timeFireInterval)
                {
                    if (reAimPerFire)
                        fireDirect = (followTarget.transform.position - goFirepoint.transform.position).normalized;
                    if (curFireNum == 1 && inverseSecondShoot)
                        fireDirect.x = -fireDirect.x;
                    FireOnce();
                    curFireNum++;
                }
            }
        }
        else
        {
            FinishAttack();
        }
        timerAttack += Time.fixedDeltaTime;
    }

    protected override void FinishAttack()
    {
        animator.Play("idle");
        base.FinishAttack();
    }

    protected override void Init()
    {
        //chargeDamageInfo = new DamageInfo(chargeDamage, this, DamageType.touch);
        if (timeFire == 0) timeFire = (fireNum + 1) * timeFireInterval;
        base.Init();
    }
    private void FireOnce()
    {
        GameObject go = GameObject.Instantiate(pfbBullet, goFirepoint.transform.position, Quaternion.identity, MainManager.GetParentBullets().transform);
        Bullet bullet = go.GetComponent<Bullet>();
        bullet.Init(fireDirect);
        DamageInfo info = new DamageInfo(bulletDamage, this, DamageType.bullet);
        info.SlowTime = bulletSlowTime;
        bullet.SetDamageInfo(info);
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
    }
}
