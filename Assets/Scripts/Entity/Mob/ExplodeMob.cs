using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeMob : Mob
{
    [Header("×Ô±¬")]
    [SerializeField] GameObject pfbExplode;
    [SerializeField] float explodeDamage;
    [SerializeField] float timePrepare;
    [SerializeField] float timePrepareWaitDead;
    bool alreadyExplode = false;

    [Header("ÒôÐ§")]
    [SerializeField] AudioClip audioExplode;

    public override bool StartKnockback(float kbPower, float kbTime, Vector3 direct)
    {
        bool ret = base.StartKnockback(kbPower, kbTime, direct);
        return ret;
    }
    protected override void StartAttack()
    {
        //canKnockback = false;
        waitDead = true;
        canHit = false;
        dealTouchDamage = false;
        animator.Play("prepareExplode");
        alreadyExplode = false;
        base.StartAttack();
    }

    protected override void Attacking()
    {
        OnKnockback();
        if (timerAttack < timePrepare)
        {
            // do nothing
        }
        else
        {
            if (!alreadyExplode)
            {
                alreadyExplode = true;
                animator.Play("explode");
                GameObject go = GameObject.Instantiate(pfbExplode, this.transform.position, Quaternion.identity);
                DamageCaster_colliders dc = go.GetComponent<DamageCaster_colliders>();
                DamageInfo info = new DamageInfo(explodeDamage, this, DamageType.explode);
                dc.SetDamageInfo(info);
                base.StartDead();
                if (audioExplode) SEManager.Instance.PlaySE(audioExplode, 0.2f);
            }
        }
        timerAttack += Time.fixedDeltaTime;
    }

    public override void StartDead()
    {
        if (!waitDead) {
            StartAttack();
            //base.StartDead();
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
    }
}
