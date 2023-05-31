using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Weapon : MonoBehaviour
{
    bool inWork = true;
    [SerializeField] int weaponIdx;

    [Header("Ò¡°Ú")]
    [SerializeField] float minSwingDegree;
    [SerializeField] float maxSwingDegree;
    float curSwingDegree = 0.0f;
    [SerializeField] float swingSpeed;
    [SerializeField] float swingSpeedMax;
    [SerializeField] float timeSwingSpeedMax;
    [SerializeField] float timerSwingSpeedMax;
    int lastSwingDirect = 0; // -1=left,0=idle,1=right
    [SerializeField] float swingScale = 1.0f;

    [Header("¿ª»ð")]
    [SerializeField] GameObject pfbBullet;
    [SerializeField] float timeFireInterval;
    float timerFire = 0.0f;
    [SerializeField] float bulletDamage;
    [SerializeField] float toolDamageBouns = 1;
    [SerializeField] GameObject goFirepoint;
    [SerializeField] float doubleFireShift;
    [SerializeField] int fireNum = 1;
    [SerializeField] float maxFireAngle = 0;
    [SerializeField] float bulletKnockbackPower = 0.0f;
    [SerializeField] float bulletKnockbackTime = 0.0f;
    [SerializeField] Transform vfx_fire_parent;
    [SerializeField] GameObject vfx_fire;

    [Header("»Ó¿³")]
    [SerializeField] GameObject goSword;
    [SerializeField] Animator animatorSword;
    [SerializeField] Collider2D hitboxSwordIdle;
    [SerializeField] Collider2D hitboxSwordSwing;
    Collider2D hitboxSword;
    [SerializeField] float slashDamage;
    [SerializeField] float slashKnockbackPower;
    [SerializeField] float slashKnockbackTime;
    [SerializeField] float timeSlashInterval;
    float timerSlashInterval;
    ContactFilter2D targetFilterSlash;
    [SerializeField] LayerMask targetMaskrSlash;
    [SerializeField] bool useTriggerrSlash;

    [Header("ÕÐ¼Ü")]
    ContactFilter2D targetFilterBlock;
    [SerializeField] LayerMask targetMaskBlock;
    [SerializeField] bool useTriggerBlock;
    [SerializeField] bool fireWhenBlock;

    [Header("ÕôÆû")]
    [SerializeField] GameObject pfbSteamBurst;
    [SerializeField] float steamBurstKBPower;
    [SerializeField] float steamBurstKBTime;
    [SerializeField] GameObject goSteamBurstPoint;
    [SerializeField] float steamBurstCD;
    [SerializeField] Text txtCD; // tmp code
    [SerializeField] GameObject pfbClawSteamBurst;
    //[SerializeField] GameObject goClawSteamBurstPoint;
    float timerSteamBurst;

    [SerializeField] GameObject goMainBody;

    [Header("Éý¼¶")]
    [SerializeField] int curLevel = 0;
    [SerializeField] GameObject pfbBullet_Lv2;
    [SerializeField] float maxFireAngle_Lv2;
    [SerializeField] float swordScale_Lv2;

    [Header("ÒôÐ§")]
    [SerializeField] AudioClip audioFire;
    [SerializeField] AudioSource asTurn;
    [SerializeField] AudioClip audioSteam;
    [SerializeField] AudioClip audioSteamFailed;
    [SerializeField] AudioClip audioSteamReady;

    public void SetInWork(bool flag)
    {
        inWork = flag;
    }
    public void ResetDegree()
    {
        curSwingDegree = 0.0f;
    }
    public void UpdateInfo(WeaponInfo info, int level)
    {
        curLevel = level;
        if (goSword)
        {
            slashDamage = info.Damage;
            slashKnockbackPower = info.KnockbackPower;
        }
        else
        {
            bulletDamage = info.Damage;
            timeFireInterval = info.FireInterval;
            fireNum = info.FireNum;
            bulletKnockbackPower = info.KnockbackPower;
        }
        if (curLevel > 0) // unique update
        {
            if (pfbBullet_Lv2)
                pfbBullet = pfbBullet_Lv2;
            if (maxFireAngle > 0)
                maxFireAngle = maxFireAngle_Lv2;
            if (goSword)
                this.transform.localScale = new Vector3(swordScale_Lv2, swordScale_Lv2, 1.0f);
        }
    }
    public void Swing(float xAxis)
    {
        int curSwingDirect;
        if (xAxis < 0)
            curSwingDirect = -1;
        else if(xAxis>0)
            curSwingDirect = 1;
        else
            curSwingDirect = 0;
        if (curSwingDirect!=0 && curSwingDirect == lastSwingDirect)
        {
            timerSwingSpeedMax += Time.fixedDeltaTime;
            if (timerSwingSpeedMax > timeSwingSpeedMax) timerSwingSpeedMax = timeSwingSpeedMax;
        }
        else
        {
            timerSwingSpeedMax = 0.0f;
        }
        lastSwingDirect = curSwingDirect;

        float _swingSpeed = timerSwingSpeedMax / timeSwingSpeedMax * (swingSpeedMax - swingSpeed) + swingSpeed;
        if (xAxis < 0)
        {
            curSwingDegree += _swingSpeed * Time.fixedDeltaTime * swingScale;
            if (curSwingDegree > maxSwingDegree)
            {
                curSwingDegree = maxSwingDegree;
            }
            asTurn.UnPause();
            if (goSword)
            {
                hitboxSword = hitboxSwordSwing;
                animatorSword.Play("swing");
            }
        }
        else if (xAxis > 0)
        {
            curSwingDegree -= _swingSpeed * Time.fixedDeltaTime * swingScale;
            if (curSwingDegree < minSwingDegree)
            {
                curSwingDegree = minSwingDegree;
            }
            asTurn.UnPause();
            if (goSword)
            {
                hitboxSword = hitboxSwordSwing;
                animatorSword.Play("swing");
            }
        }
        else
        {
            asTurn.time = 0.0f;
            asTurn.Pause();
            if (goSword)
            {
                hitboxSword = hitboxSwordIdle;
                animatorSword.Play("idle");
            }
        }

    }

    public void FireOnce(Vector3 genPos, Vector3 bulletDirect)
    {
        for (int i = 0; i < fireNum; i++)
        {
            GameObject go = GameObject.Instantiate(pfbBullet, genPos, Quaternion.identity, MainManager.GetParentBullets().transform);
            Bullet bullet = go.GetComponent<Bullet>();
            Vector3 _bulletDirect = bulletDirect;
            if (maxFireAngle > 0)
            {
                //float angle = UnityEngine.Random.Range(-maxFireAngle, maxFireAngle);
                float angle;
                if (fireNum != 1)
                    angle = -maxFireAngle + 2 * maxFireAngle * i / (fireNum - 1);
                else
                    angle = UnityEngine.Random.Range(-maxFireAngle, maxFireAngle);
                _bulletDirect = Quaternion.Euler(new Vector3(0, 0, angle)) * bulletDirect;
            }
            bullet.Init(_bulletDirect);
            DamageInfo info = new DamageInfo(bulletDamage, MinerManager.Instance.GetMinerEntity());
            info.ToolDamageBonus = toolDamageBouns;
            bullet.SetDamageInfo(info);
            bullet.InitKnockbackInfo(bulletKnockbackPower, bulletKnockbackTime);
        }
        if (vfx_fire) GameObject.Instantiate(vfx_fire, genPos, Quaternion.FromToRotation(Vector3.up, goMainBody.transform.up), vfx_fire_parent);
        if (audioFire) SEManager.Instance.PlaySE(audioFire, 0.15f);
    }

    public void FireLoop()
    {
        if (timeFireInterval>0)
        {
            timerFire -= Time.deltaTime;
            if (timerFire < 0)
            {
                if (EffectManager.Instance.GetTimerDoubleBullet() > 0.0f)
                {
                    Vector3 genPos = goFirepoint.transform.position;
                    Vector3 shootDirect = goMainBody.transform.up;
                    Vector3 offsetVec = new Vector3(-shootDirect.y, shootDirect.x, 0.0f).normalized;
                    FireOnce(genPos + offsetVec * doubleFireShift, goMainBody.transform.up);
                    FireOnce(genPos - offsetVec * doubleFireShift, goMainBody.transform.up);
                }
                else
                {
                    FireOnce(goFirepoint.transform.position, goMainBody.transform.up);
                }
                timerFire = timeFireInterval;
                if (EffectManager.Instance.GetTimerFastFire() > 0.0f)
                    timerFire /= EffectManager.Instance.FastFireSpeedScale;
            }
        }
    }

    private void StartSteamBurst()
    {
        if (timerSteamBurst <= 0.0f)
        {
            GameObject go = GameObject.Instantiate(pfbSteamBurst, goSteamBurstPoint.transform.position, Quaternion.identity, MainManager.GetParentBullets().transform);
            DamageCaster_colliders dc = go.GetComponent<DamageCaster_colliders>();
            if (dc)
            {
                dc.SetDamageInfo(new DamageInfo(0, MinerManager.Instance.GetMinerEntity()));
                dc.InitKnockbackInfo(steamBurstKBPower, steamBurstKBTime);
            }
            timerSteamBurst = steamBurstCD;
            if(audioSteam) SEManager.Instance.PlaySE(audioSteam);
            Vector3 genPos = MinerManager.Instance.GetMiner().GetCurClaw().GetClawHeadPos();
            GameObject go2 = GameObject.Instantiate(pfbClawSteamBurst, genPos, Quaternion.identity, MainManager.GetParentBullets().transform);
            DamageCaster_colliders dc2 = go2.GetComponent<DamageCaster_colliders>();
            if (dc2)
            {
                dc2.SetDamageInfo(new DamageInfo(0, MinerManager.Instance.GetMinerEntity()));
                dc2.InitKnockbackInfo(steamBurstKBPower, steamBurstKBTime);
            }
            MinerManager.Instance.GetMiner().ClearSlow();
        }
        else
        {
            if (audioSteam) SEManager.Instance.PlaySE(audioSteamFailed);
        }
    }

    private void Slash()
    {
        if (hitboxSword)
        {
            List<Collider2D> list_colliders = new List<Collider2D>();
            Physics2D.OverlapCollider(hitboxSword, targetFilterSlash, list_colliders);
            foreach (var item in list_colliders)
            {
                //if (item.tag == "mob")
                {
                    Mob mob = item.GetComponent<Mob>();
                    if (mob)
                    {
                        Entity src = MinerManager.Instance.GetMinerEntity();
                        DamageInfo _info = new DamageInfo(slashDamage, src, DamageType.melee);
                        if (hitboxSword == hitboxSwordIdle)
                            _info.ToolDamageBonus = 0.2f;
                        bool ret = mob.OnHit(_info);
                        if (ret)
                            mob.StartKnockback(slashKnockbackPower, slashKnockbackTime, (mob.transform.position - this.transform.position).normalized);
                    }
                }
            }
        }
    }

    private void TryBlock()
    {
        if (hitboxSword)
        {
            List<Collider2D> list_colliders = new List<Collider2D>();
            Physics2D.OverlapCollider(hitboxSword, targetFilterBlock, list_colliders);
            foreach (var item in list_colliders)
            {
                if (item.tag == "bullet")
                {
                    Bullet bullet = item.GetComponent<Bullet>();
                    if (bullet)
                    {
                        if (bullet.GetDamageInfo().Src.GetEntityType() == EntityType.enemy)
                        {
                            if (fireWhenBlock)
                            {
                                Vector3 shootDirect = goMainBody.transform.up;
                                Entity src = bullet.GetDamageInfo().Src;
                                if (src)
                                    shootDirect = (src.transform.position - goFirepoint.transform.position).normalized;
                                //if (maxFireAngle > 0)
                                //{
                                //    float angle = UnityEngine.Random.Range(-maxFireAngle, maxFireAngle);
                                //    shootDirect = Quaternion.Euler(new Vector3(0, 0, angle)) * shootDirect;
                                //}
                                FireOnce(bullet.transform.position, shootDirect);
                            }
                            Destroy(bullet.gameObject);
                        }
                    }
                }
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        hitboxSword = hitboxSwordIdle;

        if (goSword)
        {
            targetFilterSlash = Utils.InitContactFilter(targetMaskrSlash, useTriggerrSlash);
            targetFilterBlock = new ContactFilter2D();
            targetFilterBlock.SetLayerMask(targetMaskBlock);
            targetFilterBlock.useTriggers = useTriggerBlock;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!inWork) { return; }
        goMainBody.transform.rotation = Quaternion.Euler(new Vector3(0, 0, curSwingDegree));
        FireLoop();
        if (goSword)
        {
            if (timerSlashInterval < 0.0f)
            {
                timerSlashInterval = timeSlashInterval;
                Slash();
            }
            else
            {
                timerSlashInterval -= Time.deltaTime;
            }
        }
        if (timerSteamBurst > 0)
        {
            timerSteamBurst -= Time.deltaTime;
            txtCD.text = timerSteamBurst.ToString("f4");
            if (timerSteamBurst < 0)
            {
                if (audioSteamReady) SEManager.Instance.PlaySE(audioSteamReady);
                txtCD.text = "READY!";
                if (UIManager.Instance)
                    UIManager.Instance.PlayLeftBatteryShine();
            }
        }
        //if (Input.GetKeyDown(KeyCode.S))
        //{
        //    StartSteamBurst();
        //}

        if (goSword)
        {
            if(EffectManager.Instance.GetTimerDoubleBullet() > 0.0f)
                goSword.transform.localScale = new Vector3(1.5f, 1.5f, 0.0f);
            else
                goSword.transform.localScale = new Vector3(1f, 1f, 0.0f);

            if (EffectManager.Instance.GetTimerFastFire() > 0.0f)
                swingScale = 2.0f;
            else
                swingScale = 1.0f;
            if (curLevel > 0)
                swingScale = swingScale * swordScale_Lv2;
        }

        if (UIManager.Instance)
            UIManager.Instance.SetLeftBatteryValue(1 - timerSteamBurst / steamBurstCD);

    }

    private void FixedUpdate()
    {
        //Swing(Input.GetAxis("GunnerX"));
        TryBlock();
        //if (Input.GetAxis("GunnerY") > 0)

    }

    public void SwingByInput(float axis)
    {
        Swing(axis);
    }

    public void SteamByInput()
    {
        StartSteamBurst();
    }

    public int SwingToTargetDegree(float tarDegree) //0:left 1:right 2:none 3:both
    {
        int ret = 2;
        if (curSwingDegree > tarDegree)
        {
            Swing(1);
            ret = 0;
            if (curSwingDegree < tarDegree) curSwingDegree = tarDegree;
        }
        else if (curSwingDegree < tarDegree)
        {
            Swing(-1);
            ret = 1;
            if (curSwingDegree > tarDegree) curSwingDegree = tarDegree;
        }

        return ret;
    }
}
