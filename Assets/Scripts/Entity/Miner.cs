using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Miner : Entity
{
    [SerializeField] GameObject warpper;
    [SerializeField] Claw claw;

    [Header("限制移动")]
    [SerializeField] bool freezeScreen;
    [SerializeField] float freezeScreen_minX;
    [SerializeField] float freezeScreen_maxX;

    [Header("漂浮")]
    [SerializeField] bool forceFloat;
    [SerializeField] float minFloatY;
    [SerializeField] float maxFloatY;
    [SerializeField] float timeFloatCycle;
    float timerFloat = 0.0f;

    [Header("护盾")]
    [SerializeField] float maxShield;
    float curShield = 0;
    [SerializeField] GameObject goShieldFx;

    [Header("无敌星")]
    [SerializeField] float timerInvincibleStar;
    [SerializeField] GameObject goInvincibleStarFX;

    [SerializeField] int speedUpFrame = 8;
    [SerializeField] float speed_accelerate_UP; //calc by speed up time
    [SerializeField] int speedDownFrame = 4;
    [SerializeField] float speed_accelerate_Down; //calc by speed up time
    [SerializeField] float speed = 1.0f;
    [SerializeField] float curSpeed = 0.0f;

    [Header("减速buff")]
    [SerializeField] float timerSlow;
    [SerializeField] float slowScale = 0.5f;
    [SerializeField] GameObject goSlowFX;

    [Header("加速buff")]
    [SerializeField] float timerSpeedUp;
    [SerializeField] float speedUpScale = 1.5f;
    [SerializeField] GameObject goSpeedUpFx;

    [Header("受击击退")]
    [SerializeField] float onTouchKnockbackPower = 1.0f;
    [SerializeField] float onTouchKnockbackTime = 1.0f;


    [Header("受击无敌帧")]
    [SerializeField] float timeHitInvincible;
    [SerializeField] GameObject goInvincibleFX; //tmp code
    float timerInvincible;

    [Header("拾取")]
    [SerializeField] Collider2D colliderPick;
    [SerializeField] LayerMask targetMaskPick;
    [SerializeField] bool useTriggerPick;
    ContactFilter2D targetFilterPick;

    [Header("音效")]
    [SerializeField] AudioClip audioOnHit;
    [SerializeField] AudioClip audioStartSlow;
    [SerializeField] AudioSource asBGM;
    [SerializeField] AudioClip audioGameover;
    //[SerializeField] AudioSource asLowHealth;
    [SerializeField] AudioClip audioLowHealth;
    [SerializeField] AudioSource asMove;
    [SerializeField] AudioClip audioMove;
    [SerializeField] AudioClip audioMoveSlow;
    [SerializeField] AudioClip audioInvincibleStarOnHit;
    [SerializeField] AudioClip audioOnHeal;
    float timerInvincibleSEDiasble;
    bool firstDeath = true;

    [Header("动画")]
    [SerializeField] Animator animator;

    public override bool OnHit(DamageInfo info)
    {
        if (freezeScreen)
            return false;
        info.DamageValue = 1; // tmp code
        if (info.DType == DamageType.touch)
        {
            Vector3 direct = info.Src.transform.position - this.transform.position;
            info.Src.StartKnockback(onTouchKnockbackPower, onTouchKnockbackTime, direct);
            if (timerInvincibleStar > 0.0f)
            {
                info.Src.OnHit(new DamageInfo(999, this, DamageType.touch));
            }
        }
        if (timerInvincibleStar > 0.0f)
        {
            if (timerInvincibleSEDiasble <= 0)
            {
                if (audioInvincibleStarOnHit) SEManager.Instance.PlaySE(audioInvincibleStarOnHit);
                timerInvincibleSEDiasble = 0.1f;
            }
            return true;
        }
        if (timerInvincible > 0.0f)
        {
            if (timerInvincibleSEDiasble <= 0)
            {
                if (audioInvincibleStarOnHit) SEManager.Instance.PlaySE(audioInvincibleStarOnHit);
                timerInvincibleSEDiasble = 0.1f;
            }
            return true;
        }
        if(info.DType == DamageType.bullet)
            AchievementManager.Instance.ChangeValue(RecordType.onRangeHitNum, 1);
        if (audioOnHit) SEManager.Instance.PlaySE(audioOnHit);
        if (info.SlowTime > 0.0f)
        {
            timerSlow = info.SlowTime;
            if (audioStartSlow) SEManager.Instance.PlaySE(audioStartSlow);
            asMove.clip = audioMoveSlow;
        }
        timerInvincible = timeHitInvincible;

        bool ret = false;
        if (curShield > 0)
        {
            ChangeShield(-info.DamageValue);
            ret = true;
            if (info.DType == DamageType.touch)
            {
                info.Src.OnHit(new DamageInfo(9, this, DamageType.touch));
            }
        }
        else
        {
            ret = base.OnHit(info);
        }

        if (curHealth == 1)
        {
            //asLowHealth.UnPause();
            if (audioLowHealth) SEManager.Instance.PlaySE(audioLowHealth);
            UIManager.Instance.SetCompassLight(true);
        }
        else
        {
            //asLowHealth.Pause();
            UIManager.Instance.SetCompassLight(false);
        }
        return ret;
    }

    public override bool OnHeal(int value)
    {
        float tmpHealth = curHealth;
        bool ret = base.OnHeal(value);
        if (curHealth != 1)
            UIManager.Instance.SetCompassLight(false);
        if (tmpHealth != curHealth)
            if (audioOnHeal) SEManager.Instance.PlaySE(audioOnHeal);
        return ret;
    }



    public override void StartDead()
    {
        if (firstDeath)
        {
            if (audioGameover) { asBGM.clip = audioGameover; asBGM.Play(); asBGM.loop = false; }
            firstDeath = false;
            MainManager.Instance.StartGameOver();
            animator.Play("dead");
            AchievementManager.Instance.ChangeValue(RecordType.deadNum, 1);
        }
        // do nothing now
    }

    public override bool StartKnockback(float kbPower, float kbTime, Vector3 direct)
    {
        direct.y = 0;
        return base.StartKnockback(kbPower, kbTime, direct);
    }

    protected override bool OnKnockback()
    {
        if (inKnockback)
        {
            timerKnockback -= Time.fixedDeltaTime;
            if (timerKnockback <= 0) { inKnockback = false; timerKnockback = 0.0f; }
            float _scale = timerKnockback / timeKnockback;
            warpper.transform.Translate(knockbackDirect * knockbackPower * _scale * Time.fixedDeltaTime);
            return true;
        }
        return false;
    }

    public void ClearSlow()
    {
        timerSlow = 0.0f;
    }
    public float GetCurSpeed()
    {
        return curSpeed;
    }
    public float GetCurShield()
    {
        return curShield;
    }
    public float GetMaxShield()
    {
        return maxShield;
    }
    public Claw GetCurClaw()
    {
        return claw;
    }
    public void SetCurClaw(Claw claw)
    {
        this.claw = claw;
    }
    public void ChangeShield(float value)
    {
        curShield += value;
        if (curShield > maxShield) curShield = maxShield;

        if (curShield < 0) curShield = 0;

        if (curShield > 0)
            goShieldFx.SetActive(true);
        else
            goShieldFx.SetActive(false);
        UIManager.Instance.SetShieldValue(curShield / maxShield);
    }
    public void StartSpeedUp(float lastTime)
    {
        timerSpeedUp = lastTime;
    }
    public void StartInvincibleStar(float lastTime)
    {
        timerInvincibleStar = lastTime;
    }

    public void SetFreezeScreen(bool flag)
    {
        freezeScreen = flag;
        if (!flag)
        {
            this.transform.localPosition = new Vector3(0, 0, 0);
        }
    }

    public void SetMaxFloat(float value)
    {
        maxFloatY = value;
    }

    private Vector3 MoveInFreezeScreen(Vector3 moveVec)
    {
        Vector3 newPosition = this.transform.position;
        newPosition += moveVec;
        Vector3 distanceToWarpper = newPosition - warpper.transform.position;
        if (distanceToWarpper.x < freezeScreen_minX)
            distanceToWarpper.x = freezeScreen_minX;
        else if (distanceToWarpper.x > freezeScreen_maxX)
            distanceToWarpper.x = freezeScreen_maxX;

        newPosition = distanceToWarpper + warpper.transform.position;
        moveVec = newPosition - this.transform.position;
        this.transform.position = newPosition;

        return moveVec;
    }

    private void Move(float xAxis)
    {
        if (xAxis > 0)
        {
            if (curSpeed < speed)
            {
                curSpeed += speed_accelerate_UP;
                curSpeed = curSpeed > speed ? speed : curSpeed;
            }
        }
        else if (xAxis < 0)
        {
            if (curSpeed > -speed)
            {
                curSpeed -= speed_accelerate_UP;
                curSpeed = curSpeed < -speed ? -speed : curSpeed;
            }
        }
        else
        {
            if (curSpeed > 0)
            {
                curSpeed -= speed_accelerate_Down;
                curSpeed = curSpeed < 0 ? 0 : curSpeed;
            }
            else if (curSpeed < 0)
            {
                curSpeed += speed_accelerate_Down;
                curSpeed = curSpeed > 0 ? 0 : curSpeed;
            }
        }



        Vector3 moveVec = new Vector3(curSpeed, 0, 0) * Time.fixedDeltaTime;
        if (timerSlow > 0.0f)
            moveVec = moveVec * slowScale;
        if (timerSpeedUp > 0.0f)
            moveVec = moveVec * speedUpScale;
        if (freezeScreen)
            moveVec = MoveInFreezeScreen(moveVec);
        else
            warpper.transform.Translate(moveVec);
        claw.TryCoMoveDrag(this.gameObject, moveVec);
        if (moveVec.x > 0)
            goSpeedUpFx.transform.localScale = new Vector3(1, 1, 1);
        else if (moveVec.x < 0)
            goSpeedUpFx.transform.localScale = new Vector3(-1, 1, 1);
    }

    private void TryPickUpLoot()
    {
        List<Collider2D> list_colliders = new List<Collider2D>();
        Physics2D.OverlapCollider(colliderPick, targetFilterPick, list_colliders);
        foreach (var item in list_colliders)
        {
            if (item.tag == "loot")
            {
                ItemLoot loot = item.GetComponent<ItemLoot>();
                if (loot)
                {
                    loot.OnGotcha();
                }
            }
        }
    }

    private void FloatOnSea()
    {
        if (forceFloat || !freezeScreen)
        {
            Vector3 pos = this.transform.localPosition;
            timerFloat += Time.fixedDeltaTime;
            if (timerFloat > timeFloatCycle) timerFloat -= timeFloatCycle;
            float scale = timerFloat / timeFloatCycle;
            //int x = (int)((timeHitFX - timerHitFX)/shockCycle);
            //float phase = (timeHitFX - timerHitFX - shockCycle * x)/shockCycle;
            float offset = Mathf.Sin(scale * 360f) * (maxFloatY - minFloatY) + minFloatY;
            pos.y = offset;
            this.transform.localPosition = pos;
        }
        else
        {
            Vector3 pos = this.transform.localPosition;
            pos.y = 0.0f;
            this.transform.localPosition = pos;
        }
    }

    private void Awake()
    {
        Init();
    }
    // Start is called before the first frame update
    void Start()
    {
        speed_accelerate_UP = speed / (float)(speedUpFrame);
        speed_accelerate_Down = speed / (float)(speedDownFrame);
        //asLowHealth.Pause();
        targetFilterPick = new ContactFilter2D();
        targetFilterPick.SetLayerMask(targetMaskPick);
        targetFilterPick.useTriggers = useTriggerPick;
    }

    // Update is called once per frame
    void Update()
    {
        timerInvincibleSEDiasble -= Time.deltaTime;
        TryPickUpLoot();
        if (timerInvincibleStar > 0.0f)
        {
            goInvincibleStarFX.SetActive(true);
            timerInvincibleStar -= Time.deltaTime;
        }
        else
        {
            goInvincibleStarFX.SetActive(false);
        }

        if (timerSlow > 0.0f)
        {
            goSlowFX.SetActive(true);
            timerSlow -= Time.deltaTime;
            if (timerSlow <= 0.0f)
                asMove.clip = audioMove;
        }
        else
            goSlowFX.SetActive(false);

        if (timerSpeedUp > 0.0f)
        {
            goSpeedUpFx.SetActive(true);
            timerSpeedUp -= Time.deltaTime;
        }
        else
        {
            goSpeedUpFx.SetActive(false);
        }

        if (curSpeed != 0)
        {
            if (slowScale < 1.0f)
                asMove.UnPause();
            else
                asMove.UnPause();
        }
        else
        {
            asMove.time = 0.0f;
            asMove.Pause();
        }

        if (timerInvincible > 0.0f)
        {
            goInvincibleFX.SetActive(true);
            timerInvincible -= Time.deltaTime;
        }
        else
            goInvincibleFX.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (firstDeath)
            FloatOnSea();
        //if (!OnKnockback())
        //{
        //    Move(Input.GetAxis("MinerX"));
        //}
    }

    public void MoveByInput(float xAxis)
    {
        if (!OnKnockback())
        {
            Move(xAxis);
        }
    }

}
