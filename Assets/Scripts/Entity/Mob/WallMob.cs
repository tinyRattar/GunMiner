using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallMob : Mob
{
    //[SerializeField] float maxDepth;
    //[SerializeField] float minDepth;
    //[SerializeField] float diveSpeed;
    //float curDepth = 0;
    //float oriY;
    [SerializeField] float timeDying;
    float timerDying;

    public override bool OnHit(DamageInfo info)
    {
        if (timerHitFX > 0) return true;
        bool ret = base.OnHit(info);
        return ret;
    }

    public override bool OnClawHit()
    {
        return false;
    }

    public override void StartDead()
    {
        timerHitFX = timeHitFX;
        animator.Play("exit");
        base.StartDead();
    }
    protected override void OnDying()
    {
        if (timerDying >= timeDying)
            base.OnDying();
        else
            timerDying += Time.fixedDeltaTime;
    }

    protected override void Move()
    {
        //if (curDepth < maxDepth) curDepth += diveSpeed * Time.fixedDeltaTime;
        //if (curDepth > maxDepth) curDepth = maxDepth;
    }
    void Start()
    {
        Init();
        //oriY = this.transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (lookAtTarget)
            this.transform.right = (this.transform.position - followTarget.transform.position);
    }

    private void FixedUpdate()
    {
        BasicBehavior();
        UpdateSR();
        //Vector3 pos = this.transform.position;
        //pos.y = oriY + curDepth;
        //this.transform.position = pos;
    }


}
