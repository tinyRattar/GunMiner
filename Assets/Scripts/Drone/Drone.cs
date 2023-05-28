using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : MonoBehaviour
{
    [Header("持续时间")]
    [SerializeField] float timeLast;
    float timerLast;

    [Header("开火")]
    [SerializeField] GameObject pfbBullet;
    [SerializeField] float timeFireInterval;
    float timerFire = 0.0f;
    [SerializeField] float bulletDamage;
    [SerializeField] GameObject goFirepoint;
    [SerializeField] float detectDistance;

    [Header("音效")]
    [SerializeField] AudioClip audioFire;

    public void Init(float timeLast)
    {
        this.timeLast = timeLast;
        timerLast = this.timeLast;
    }
    public void FireOnce()
    {
        List<Mob> listMobs = MobManager.Instance.GetListMobs();
        float minDistance = 65535.0f;
        Vector3 shootDirect = Vector3.zero;
        foreach (Mob mob in listMobs)
        {
            Vector3 distance = mob.transform.position - goFirepoint.transform.position;
            if (detectDistance < distance.magnitude)
            {
                continue;
            }
            if (distance.magnitude < minDistance)
            {
                minDistance = distance.magnitude;
                shootDirect = distance.normalized;
            }
        }

        if (shootDirect == Vector3.zero) return;

        GameObject go = GameObject.Instantiate(pfbBullet, goFirepoint.transform.position, Quaternion.identity, MainManager.GetParentBullets().transform);
        Bullet bullet = go.GetComponent<Bullet>();
        bullet.Init(shootDirect);
        DamageInfo info = new DamageInfo(bulletDamage, MinerManager.Instance.GetMinerEntity());
        bullet.SetDamageInfo(info);
        if (audioFire) SEManager.Instance.PlaySE(audioFire);
    }

    public void FireLoop()
    {
        timerFire -= Time.deltaTime;
        if (timerFire < 0)
        {
            FireOnce();
            timerFire = timeFireInterval;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        timerLast = timeLast;
    }

    // Update is called once per frame
    void Update()
    {

        if (timerLast > 0.0f)
        {
            FireLoop();
            timerLast -= Time.deltaTime;
            if (timerLast < 0.0f)
            {
                Destroy(this.gameObject);
            }
        }
    }
}
