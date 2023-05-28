using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCaster_colliders : MonoBehaviour
{
    [SerializeField] List<Collider2D> hitboxs;
    [SerializeField] float delay;
    [SerializeField] float lasttime;
    [SerializeField] float lifetime;
    DamageInfo damageInfo;
    float timer = 0.0f;
    bool canhit = true;

    [Header("»÷ÍË")]
    [SerializeField] float knockbackPower = 1.0f;
    [SerializeField] float knockbackTime = 1.0f;
    GameObject knockbackSrc;

    [Header("Ïû³ý×Óµ¯")]
    [SerializeField] bool destroyBullet;

    ContactFilter2D targetFilter;
    [SerializeField] LayerMask targetMask;
    [SerializeField] bool useTrigger;
    List<Entity> listAlreadyHit = new List<Entity>();
    List<Ore> listAlreadyHit_ore = new List<Ore>();

    [SerializeField] bool useDebugDamageInfo;
    [SerializeField] float debugDamage;
    [SerializeField] DamageType damageType;

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

    // Start is called before the first frame update
    void Start()
    {
        timer = 0.0f;
        targetFilter = new ContactFilter2D();
        targetFilter.SetLayerMask(targetMask);
        targetFilter.useTriggers = useTrigger;

        if (useDebugDamageInfo)
        {
            damageInfo = new DamageInfo(debugDamage, MinerManager.Instance.GetMinerEntity(), damageType);
            knockbackSrc = MinerManager.Instance.GetMinerEntity().gameObject;
        }
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (lifetime > 0 && timer > lifetime)
        {
            Destroy(this.gameObject);
        }
        foreach (Collider2D hitbox in hitboxs)
        {
            if (canhit)
            {
                if (timer > delay && (lasttime <= 0 || timer < delay + lasttime))
                {
                    List<Collider2D> list_colliders = new List<Collider2D>();
                    Physics2D.OverlapCollider(hitbox, targetFilter, list_colliders);
                    foreach (var item in list_colliders)
                    {
                        //Debug.Log(item.name);
                        if (item.tag == "entity")
                        {
                            Entity entity = item.GetComponent<Entity>();
                            if (entity && entity != damageInfo.Src)
                            {
                                if (!listAlreadyHit.Contains(entity))
                                {
                                    listAlreadyHit.Add(entity);
                                    bool ret = entity.OnHit(this.GetDamageInfo());
                                    entity.StartKnockback(knockbackPower, knockbackTime, entity.transform.position - this.transform.position);
                                }
                            }
                        }
                        else if (item.tag == "ore")
                        {
                            Ore ore = item.GetComponent<Ore>();
                            if (ore)
                            {
                                if (!listAlreadyHit_ore.Contains(ore))
                                {
                                    listAlreadyHit_ore.Add(ore);
                                    bool ret = ore.OnHit(this.GetDamageInfo());
                                }
                            }
                        }else if (destroyBullet && item.tag == "bullet")
                        {
                            Bullet bullet = item.GetComponent<Bullet>();
                            if (bullet)
                            {
                                Destroy(bullet.gameObject);
                            }
                        }
                    }
                }
            }
        }
    }
}
