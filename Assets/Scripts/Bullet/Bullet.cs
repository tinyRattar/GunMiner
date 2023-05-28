using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum SRDirectType
{
    freeze,
    twoWay,
    fullWay
}

public class Bullet : MonoBehaviour
{
    [SerializeField] protected string bulletTag;
    [SerializeField] protected float speed;
    [SerializeField] protected Vector2 direct;
    [SerializeField] protected float lifetime;
    protected float timer;
    [SerializeField] SRDirectType srDirect = SRDirectType.freeze;
    [SerializeField] GameObject topSR;
    [SerializeField] protected DamageInfo damageInfo;

    [Header("»÷ÍË")]
    [SerializeField] float knockbackPower = 0.0f;
    [SerializeField] float knockbackTime = 0.0f;
    GameObject knockbackSrc;

    [Header("Ð§¹û")]
    [SerializeField] GameObject vfx_hit;

    [SerializeField] bool hitable = true;
    [SerializeField] bool pierce = false;
    public DamageInfo GetDamageInfo()
    {
        return damageInfo;
    }
    public void SetDamageInfo(DamageInfo info)
    {
        damageInfo = info;
    }

    public void InitKnockbackInfo(float kbPower, float kbTime, GameObject src = null)
    {
        knockbackPower = kbPower;
        knockbackTime = kbTime;
        if (src)
        {
            knockbackSrc = src;
        }
        else
        {
            knockbackSrc = this.gameObject;
        }
    }
    public virtual float getSpeed()
    {
        return speed;
    }
    protected virtual void OnTimeout()
    {
        Destroy(this.gameObject);
    }
    protected virtual void OnMove()
    {
        if (timer > 0)
        {
            timer -= Time.fixedDeltaTime;
            this.transform.Translate(speed * direct.normalized * Time.fixedDeltaTime);
        }
        else
        {
            if (lifetime > 0)
                OnTimeout();
        }
    }

    void UpdateSR()
    {
        switch (srDirect)
        {
            case SRDirectType.freeze:
                break;
            case SRDirectType.twoWay:
                if (direct.x > 0)
                {
                    Vector3 localscale = topSR.transform.localScale;
                    localscale.x = -1;
                    topSR.transform.localScale = localscale;
                }
                break;
            case SRDirectType.fullWay:
                topSR.transform.up = direct;
                break;
            default:
                break;
        }
    }

    public virtual void Init()
    {
        timer = lifetime;
        UpdateSR();
    }

    public virtual void Init(Vector2 initDirect)
    {
        direct = initDirect;
        Init();
    }

    public virtual void Init(Vector2 initDirect, float initSpeed)
    {
        if (initSpeed >= 0)
        {
            speed = initSpeed;
        }
        Init(initDirect);
    }

    public virtual void Init(Vector2 initDirect, float initSpeed, float initLifeTime)
    {
        lifetime = initLifeTime;
        Init(initDirect, initSpeed);
    }

    protected virtual void OnHit()
    {
        if (vfx_hit) GameObject.Instantiate(vfx_hit, this.transform.position, Quaternion.identity);
        if (pierce) { }
        else
        {
            hitable = false;
            Destroy(this.gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        OnMove();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hitable)
        {
            if (collision.tag == "entity")
            {
                Entity entity = collision.GetComponent<Entity>();
                if (entity)
                {
                    //if (entity.GetEntityType() == EntityType.Enemy)
                    {
                        bool ret = entity.OnHit(this.GetDamageInfo());
                        if (ret && knockbackTime > 0.0f)
                            entity.StartKnockback(knockbackPower, knockbackTime, direct.normalized);
                        if (ret) { OnHit(); }
                    }
                }
            }
            if (collision.tag == "barrier")
            {
                OnHit();
            }
        }
    }
}
