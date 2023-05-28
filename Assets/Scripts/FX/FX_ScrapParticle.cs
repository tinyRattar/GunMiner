using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FX_ScrapParticle : MonoBehaviour
{
    [SerializeField] protected float speed;
    [SerializeField] protected Vector2 direct;
    [SerializeField] protected float lifetime;
    protected float timer;
    [SerializeField] SRDirectType srDirect = SRDirectType.freeze;
    [SerializeField] GameObject topSR;
    [SerializeField] SpriteRenderer sr;
    [SerializeField] Vector3 addonMove;
    [SerializeField] Vector3 addonMoveDelta;

    [SerializeField] GameObject rotateObj;
    [SerializeField] float initRotateMin;
    [SerializeField] float initRotateMax;
    [SerializeField] float rotateSpeed;

    bool hitable = true;

    public void SetSprite(Sprite sp)
    {
        sr.sprite = sp;
    }

    protected virtual void OnTimeout()
    {
        Destroy(this.gameObject);
    }
    protected virtual void OnMove()
    {
        if (timer > 0)
        {
            addonMove += addonMoveDelta * Time.fixedDeltaTime;
            timer -= Time.fixedDeltaTime;
            this.transform.Translate(speed * direct.normalized * Time.fixedDeltaTime);
            this.transform.Translate(addonMove * Time.fixedDeltaTime);
        }
        else
        {
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
        if (rotateObj) rotateObj.transform.rotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(initRotateMin, initRotateMax));
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
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        OnMove();
    }
    void OnHit()
    {
        hitable = false;
        Destroy(this.gameObject);
    }

    private void Update()
    {
        sr.color = new Color(1.0f, 1.0f, 1.0f, timer / lifetime);
        if (rotateObj) rotateObj.transform.Rotate(0, 0, rotateSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hitable)
        {
            if (collision.tag == "barrier")
            {
                OnHit();
            }
        }

    }
}

