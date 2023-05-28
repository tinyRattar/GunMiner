using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeMob : Mob
{
    Vector3 chargeStartOffset; // use for prepare next charge
    GameObject chargeTarget;
    [Header("≥Â∑Ê")]
    [SerializeField] float chargeSpeed;
    [SerializeField] float timePrepare_aim;
    [SerializeField] float timePrepare_locked;
    [SerializeField] float timeCharge;
    [SerializeField] GameObject pfbHint;
    [SerializeField] float timeHintDelay;
    [SerializeField] float chargePredScale;
    bool prepareHint = false;
    bool prepareAttackSE = false;
    Vector3 chargeDirect;
    bool firstCharge = true; //select nearest positon for first time;
    [SerializeField] bool deadAfterCharge;
    //[SerializeField] int chargeDamage;
    //DamageInfo chargeDamageInfo;

    [Header("“Ù–ß")]
    [SerializeField] AudioClip audioStartCharge;

    [SerializeField] bool hasSkyMode;

    protected override bool TryStartAttack()
    {
        if (firstCharge)
            return base.TryStartAttack();
        else
        {
            return false;
        }
    }
    protected override void StartAttack()
    {
        if (pfbHint) prepareHint = true;
        prepareAttackSE = true;
        chargeTarget = followTarget; // use followTarget as default
        chargeDirect = (chargeTarget.transform.position - this.transform.position).normalized;
        base.StartAttack();
    }

    protected override void Attacking()
    {
        if (timerAttack < timePrepare_aim)
        {
            Vector3 distance = chargeTarget.transform.position - this.transform.position;
            if (chargePredScale<=0)
                chargeDirect = distance.normalized;
            else
            {
                float preTime = distance.magnitude / chargeSpeed;
                preTime += timePrepare_aim + timePrepare_locked - timerAttack;
                preTime = preTime * chargePredScale;
                Vector3 offset = new Vector3(MinerManager.Instance.GetCurSpeed() * preTime, 0, 0);
                chargeDirect = (distance + offset).normalized;
            }
        }
        else if (timerAttack < timePrepare_aim + timePrepare_locked)
        {
            // do nothing
        }
        else if (timerAttack < timePrepare_aim + timePrepare_locked + timeCharge)
        {
            this.transform.Translate(chargeDirect.normalized * Time.fixedDeltaTime * chargeSpeed, Space.World);
            lookAtTarget = false;
            if (hasSkyMode)
                Utils.FlipGameObjectY(topSR, chargeDirect.x < 0);
            else
                Utils.FlipGameObject(topSR, (followTarget.transform.position - this.transform.position).x < 0);

        }
        else
        {
            FinishAttack();
            if (deadAfterCharge)
            {
                SwitchState(MobState.dead);
            }
        }
        if (prepareHint)
        {
            if (timerAttack >= timePrepare_aim)
            {
                GameObject go = GameObject.Instantiate(pfbHint, this.transform.position, Quaternion.FromToRotation(Vector3.up, chargeDirect));
                Destroy(go, timePrepare_aim + timePrepare_locked - timerAttack + timeHintDelay);
                prepareHint = false;
            }
        }
        if (prepareAttackSE)
        {
            if (timerAttack >= timePrepare_aim + timePrepare_locked)
            {
                if (audioStartCharge) SEManager.Instance.PlaySE(audioStartCharge);
                prepareAttackSE = false;
            }
        }
        timerAttack += Time.fixedDeltaTime;
    }

    protected override void OnFinishAttack()
    {
        firstCharge = false;
        //select next charge start point
        float ranAngle;
        if ((followTarget.transform.position - this.transform.position).x > 0)
        {
            ranAngle = UnityEngine.Random.Range(90.0f, 180.0f - 30.0f) / 180.0f * Mathf.PI;
        }
        else
        {
            ranAngle = UnityEngine.Random.Range(0.0f + 30.0f, 90.0f) / 180.0f * Mathf.PI;

        }
        chargeStartOffset = new Vector3(Mathf.Cos(ranAngle), Mathf.Sin(ranAngle)) * attackDistance;
    }

    public override bool StartKnockback(float kbPower, float kbTime, Vector3 direct)
    {
        if (curState == MobState.attack && kbTime < 0.2f)
        {
            return false;
        }
        bool ret = base.StartKnockback(kbPower, kbTime, direct);
        return ret;
    }

    protected override void Move()
    {
        if (firstCharge)
        {
            base.Move();
        }
        else
        {
            Vector3 chargeStartPoint = followTarget.transform.position + chargeStartOffset;
            Vector3 distance = chargeStartPoint - this.transform.position;
            Vector3 moveVec = distance.normalized * Time.fixedDeltaTime * speed;
            if (distance.magnitude < moveVec.magnitude)
            {
                this.transform.Translate(distance);
                StartAttack();
            }
            else
            {
                this.transform.Translate(moveVec);
            }

            if (hasSkyMode)
                Utils.FlipGameObjectY(topSR, (followTarget.transform.position - this.transform.position).x < 0);
            else
                Utils.FlipGameObject(topSR, (followTarget.transform.position - this.transform.position).x < 0);
        }
    }



    protected override void Init()
    {
        //chargeDamageInfo = new DamageInfo(chargeDamage, this, DamageType.touch);
        base.Init();
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
        if (lookAtTarget)
            this.transform.right = (this.transform.position - followTarget.transform.position);
        if (hasSkyMode)
        {
            if (this.transform.position.y > 0)
                animator.Play("inSky");
            else
                animator.Play("idle");
        }
        UpdateSR();
    }
}
