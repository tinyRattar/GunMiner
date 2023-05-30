using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MobState
{
    idle,
    trace,
    attack,
    stun,
    dead
}

public class Mob : Entity
{
    [SerializeField] int mobIndex;
    [SerializeField] float removeDistance = 40.0f;
    protected MobState curState;
    [SerializeField] protected GameObject followTarget;
    [SerializeField] bool moveHorizontal;
    [SerializeField] float followOffsetDistance;
    Vector3 followOffset; // use for normalMove
    bool inTracing = false;
    [SerializeField] float traceDistance; // when less, move direct; otherwise move to offset pos
    [SerializeField] float traceGiveUpDistance; // when larger, reset tracing.
    [SerializeField] float hoverMinDistance;
    [SerializeField] float hoverMaxDistance;
    bool hoverClose = true;
    [SerializeField] protected float speed;
    [SerializeField] protected float hoverSpeed;
    [SerializeField] bool resetPosInSea = true;
    [SerializeField] float seaY = 1.0f;

    [Header("¥•≈ˆ")]
    [SerializeField] protected bool dealTouchDamage = true;
    [SerializeField] float touchDamage;
    [SerializeField] float touchKnockBackPower;
    [SerializeField] float touchKnockBackTime;
    [SerializeField] bool deadAfterTouch;
    DamageInfo touchDamageInfo;

    [SerializeField] bool stunWhenKnockback = true;

    [Header("π•ª˜")]
    [SerializeField] protected float attackDistance;
    [SerializeField] float timeStunAfterAttack;
    protected float timerAttack;

    float timerStun;

    [Header("µÙ¬‰")]
    [SerializeField] int dropIdx;
    [SerializeField] float dropProp;
    [SerializeField] int dropNum = 1;
    [SerializeField] int dropItemCardIdx; // only work when dropIdx == 1
    [SerializeField] SpriteRenderer srChest;

    [Header("œ‘ æ")]
    [SerializeField] protected GameObject topSR;
    [SerializeField] List<SpriteRenderer> listHitSR;
    [SerializeField] protected float timeHitFX;
    [SerializeField] GameObject hitShockSR;
    [SerializeField] GameObject hitRotateSR;
    [SerializeField] float shockCycle;
    [SerializeField] float maxShockOffset;
    protected float timerHitFX;
    [SerializeField] protected bool lookAtTarget;
    [SerializeField] bool freezeSR;
    [SerializeField] protected Animator animator;
    [SerializeField] GameObject vfx_Dead;

    [Header("“Ù–ß")]
    [SerializeField] AudioClip audioStartDead;
    [SerializeField] AudioClip audioOnHit;


    Entity lastHitSrc;
    public void ForceRemove()
    {
        dropProp = 0.0f;
        SwitchState(MobState.dead);
    }
    protected virtual bool TryStartAttack()
    {
        Vector3 distance = followTarget.transform.position - this.transform.position;
        return distance.magnitude < attackDistance;
    }

    protected virtual void StartAttack()
    {
        timerAttack = 0.0f;
        SwitchState(MobState.attack);
    }

    protected virtual void Attacking() // called by fixedUpdate
    {
        FinishAttack();
    }
    protected virtual void OnFinishAttack() // called by SwitchState from attack;
    {
        // do nothing
    }

    protected virtual void FinishAttack() // called by fixedUpdate
    {
        timerStun = timeStunAfterAttack;
        SwitchState(MobState.stun);
    }

    protected void SwitchState(MobState state)
    {
        if (curState == MobState.dead) return;
        if (curState == MobState.attack) OnFinishAttack();
        if(state == MobState.dead)
        {
            if (MobManager.Instance)
            {
                MobManager.Instance.UnregistMob(this);
                for (int i = 0; i < dropNum; i++)
                {
                    float ran = UnityEngine.Random.Range(0, 1.0f);
                    if (ran < dropProp * LootManager.Instance.GetLootMulti())
                    {
                        GameObject goLoot = LootManager.Instance.GenerateOnce(dropIdx, this.transform.position, Quaternion.identity);
                        if (dropIdx == 1)
                        {
                            goLoot.GetComponent<ItemLoot>().SetItemCardIdx(dropItemCardIdx);
                        }
                    }
                }
            }
            if (AchievementManager.Instance)
            {
                if (lastHitSrc == MinerManager.Instance.GetMinerEntity())
                {
                    AchievementManager.Instance.ChangeValue(RecordType.killNum, 1);
                }
            }
        }
        curState = state;
    }

    protected virtual void Move()
    {
        Vector3 distance = followTarget.transform.position - this.transform.position;
        if (resetPosInSea && this.transform.position.y < seaY)
        {
            this.transform.Translate((distance + followOffset).normalized * Time.fixedDeltaTime * speed, Space.World);
        }
        else
        {
            if (distance.magnitude < traceDistance) inTracing = true;
            if (hoverMinDistance > 0 && distance.magnitude < hoverMinDistance)
            {
                this.transform.Translate(-distance.normalized * Time.fixedDeltaTime * speed, Space.World);
                hoverClose = false;
            }
            else if (distance.magnitude < hoverMaxDistance)
            {
                if (hoverClose)
                {
                    if (distance.magnitude > hoverMinDistance)
                    {
                        this.transform.Translate(distance.normalized * Time.fixedDeltaTime * hoverSpeed, Space.World);
                    }
                    else
                    {
                        hoverClose = false;
                    }
                }
                else
                {
                    if (distance.magnitude < hoverMaxDistance)
                    {
                        this.transform.Translate(-distance.normalized * Time.fixedDeltaTime * hoverSpeed, Space.World);
                    }
                    else
                    {
                        hoverClose = true;
                    }
                }
            }
            else if (inTracing)
            {
                if (distance.magnitude > traceGiveUpDistance)
                {
                    ResetFollowOffset();
                    inTracing = false;
                }
                else
                {
                    this.transform.Translate(distance.normalized * Time.fixedDeltaTime * speed, Space.World);
                }
            }
            else
            {
                if (hoverMaxDistance > 0 && distance.magnitude > hoverMaxDistance)
                {
                    hoverClose = true;
                }
                this.transform.Translate((distance + followOffset).normalized * Time.fixedDeltaTime * speed, Space.World);
            }
        }

        if (!freezeSR && !lookAtTarget) {
            Utils.FlipGameObject(topSR, distance.x < 0);
        }
    }

    protected virtual void OnDying()
    {
        if (vfx_Dead)
            GameObject.Instantiate(vfx_Dead, this.transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }

    protected virtual void BasicBehavior() // called every fixed update
    {
        Vector3 distance = followTarget.transform.position - this.transform.position;

        switch (curState)
        {
            case MobState.idle:
                OnKnockback();
                SwitchState(MobState.trace);
                break;
            case MobState.trace:
                if (!OnKnockback())
                {
                    if (TryStartAttack())
                        StartAttack();
                    else
                        Move();
                }
                break;
            case MobState.attack:
                Attacking();
                break;
            case MobState.stun:
                OnKnockback();
                timerStun -= Time.fixedDeltaTime;
                if (timerStun < 0.0f)
                {
                    SwitchState(MobState.idle);
                }
                break;
            case MobState.dead:
                OnDying();
                break;
            default:
                break;
        }

        if (Mathf.Abs(distance.x) > removeDistance)
        {
            OnExceedRemoveDistance();
        }
        
    }

    protected virtual void OnExceedRemoveDistance()
    {
        ForceRemove();
    }

    public override bool OnHit(DamageInfo info)
    {
        if (curState == MobState.dead) return true;
        if(info.DType == DamageType.remove)
        {
            dropProp = 0.0f;
        }
        lastHitSrc = info.Src;
        if (audioOnHit) SEManager.Instance.PlaySE(audioOnHit);
        timerHitFX = timeHitFX;
        bool ret = base.OnHit(info);
        if (ret)
        {
            if (LootManager.Instance)
            {
                if (LootManager.Instance.GetLootMulti() > 1 && dropIdx == 0)
                    LootManager.Instance.GenerateOnce(dropIdx, this.transform.position, Quaternion.identity);
            }
        }
        return ret;
    }

    public override bool StartKnockback(float kbPower, float kbTime, Vector3 direct)
    {
        bool ret = base.StartKnockback(kbPower, kbTime, direct);
        if (stunWhenKnockback && ret)
        {
            SwitchState(MobState.stun);
            timerStun = kbTime;
        }
        return ret;
    }

    public override void StartDead()
    {
        if (audioStartDead)
            SEManager.Instance.PlaySE(audioStartDead);
        SwitchState(MobState.dead);
        //base.StartDead();
    }

    protected override void Init()
    {
        if (topSR == null)
        {
            topSR = this.GetComponentInChildren<SpriteRenderer>().gameObject;
        }
        if (followTarget == null)
        {
            followTarget = MinerManager.Instance.GetMinerEntity().gameObject;
        }
        if(animator == null)
        {
            animator = this.GetComponent<Animator>();
        }
        if (dropIdx == 1)
        {
            //dropItemCardIdx = UnityEngine.Random.Range(0, 12);
            dropItemCardIdx = LootManager.Instance.GetRandomIndexForMob();
            if (srChest)
                srChest.sprite = ItemManager.Instance.GetMiniSprite(dropItemCardIdx);
        }
        ResetFollowOffset();
        touchDamageInfo = new DamageInfo(touchDamage, this, DamageType.touch);
        //MobManager.Instance.RegistMob(this);
        base.Init();
    }

    public void SetByMobInfo(MobInfo info)
    {
        maxHealth = info.Health;
        curHealth = maxHealth;
        speed = info.Speed;
        mobIndex = info.MobIndex;
        dropNum = info.Drop;
    }

    public int GetMobIndex()
    {
        return mobIndex;
    }

    protected void ResetFollowOffset()
    {
        float ranAngle;
        if ((followTarget.transform.position - this.transform.position).x > 0)
        {
            ranAngle = UnityEngine.Random.Range(90.0f, 180.0f - 30.0f) / 180.0f * Mathf.PI;
        }
        else
        {
            ranAngle = UnityEngine.Random.Range(0.0f + 30.0f, 90.0f) / 180.0f * Mathf.PI;

        }
        followOffset = new Vector3(Mathf.Cos(ranAngle), Mathf.Sin(ranAngle)) * followOffsetDistance;
    }

    protected void UpdateSR()
    {
        if (timerHitFX > 0.0f) timerHitFX -= Time.deltaTime;
        if (timerHitFX < 0.0f) timerHitFX = 0.0f;
        float scale = timerHitFX / timeHitFX;
        foreach (SpriteRenderer sr in listHitSR)
        {
            sr.color = new Color(1.0f, 1.0f - scale, 1.0f - scale);
        }
        if (hitShockSR)
        {
            float x = (timeHitFX - timerHitFX) / shockCycle;
            float phase = (x - (int)x) / shockCycle;
            //int x = (int)((timeHitFX - timerHitFX)/shockCycle);
            //float phase = (timeHitFX - timerHitFX - shockCycle * x)/shockCycle;
            float offset = Mathf.Sin(phase * 360f) * maxShockOffset;
            hitShockSR.transform.localPosition = new Vector3(offset, 0, 0);
        }
        if (hitRotateSR)
        {
            hitRotateSR.transform.localRotation = Quaternion.Euler(0, 0, 360.0f * timerHitFX / timeHitFX);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (lookAtTarget)
            this.transform.right = (this.transform.position - followTarget.transform.position);
        UpdateSR();
    }

    private void FixedUpdate()
    {
        BasicBehavior();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (dealTouchDamage)
        {
            if (collision.tag == "entity")
            {
                Entity entity = collision.GetComponent<Entity>();
                if (entity)
                {
                    if (entity.GetEntityType() == EntityType.player)
                    {
                        bool ret = entity.OnHit(touchDamageInfo);
                        if (touchKnockBackPower > 0)
                        {
                            Vector3 knockbackDirect = new Vector3(1, 0, 0);
                            if ((entity.transform.position - this.transform.position).x < 0)
                            {
                                knockbackDirect.x = -1;
                            }
                            entity.StartKnockback(touchKnockBackPower, touchKnockBackTime, knockbackDirect);
                        }
                        if (deadAfterTouch) StartDead();
                    }
                }
            }
        }

    }


}
