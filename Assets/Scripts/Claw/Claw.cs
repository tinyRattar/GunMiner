using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Claw : MonoBehaviour
{
    enum ClawState
    {
        swing,
        extend,
        drag
    }
    ClawState curState;

    [SerializeField] int clawIdx;

    [SerializeField] GameObject goClaw;
    [SerializeField] GameObject goClawHead;

    [Header("摇摆")]
    [SerializeField] float minSwingDegree;
    [SerializeField] float maxSwingDegree;
    [SerializeField] float curSwingDegree = 0.0f;
    [SerializeField] float swingSpeed;
    bool swingLeft2Right = true;

    [Header("伸缩")]
    [SerializeField] float minLength;
    [SerializeField] float dragFinishLength;
    [SerializeField] float maxLength;
    [SerializeField] float extendSpeed;
    [SerializeField] float dragSpeed;
    [SerializeField] float dragScale = 1.0f;
    [SerializeField] float minDragScale = 0.1f;
    [SerializeField] float coMoveDragScale = 1.0f;
    [SerializeField] float coMoveDragScale_close = 1.0f;
    float curLength;

    [Header("抓取")]
    [SerializeField] int maxDrag;
    [SerializeField] float minDragingOreDistance;
    [SerializeField] float absorbDragingOreSpeed;
    List<Ore> listOnDragOres = new List<Ore>();
    List<Vector3> listOnDragDistance = new List<Vector3>();


    [Header("钩爪绳索")]
    [SerializeField] LineRenderer clawLine;
    [SerializeField] GameObject clawLineSrc;
    [SerializeField] GameObject clawLineTar;

    [Header("捕网")]
    [SerializeField] bool isNetClaw;
    bool netReady = true;
    [SerializeField] Collider2D hitboxNet;
    ContactFilter2D targetFilterNet;
    [SerializeField] LayerMask targetMaskNet;
    [SerializeField] bool useTriggerNet;
    [SerializeField] GameObject goNetFX;

    [Header("破坏")]
    [SerializeField] bool isRockBreak;
    [SerializeField] float breakForce;
    [SerializeField] float luckBonus;
    bool canBreak = true;

    [Header("升级")]
    [SerializeField] int curLevel = 0;
    [SerializeField] float clawHeadScale_Lv2 = 1.0f;
    [SerializeField] float minLength_Lv2 = 1.0f;
    [SerializeField] float breakForce_Lv2 = 0.0f;
    [SerializeField] List<float> luckBonus_perLv;
    [SerializeField] int maxDrag_Lv2 = 1;

    [Header("音效")]
    [SerializeField] AudioClip audioGotcha;
    [SerializeField] AudioClip audioStartDrag;

    public void UpdateInfo(ClawInfo info, int level)
    {
        curLevel = level;
        dragSpeed = info.DragSpeed;
        extendSpeed = info.ExtendSpeed;
        if (curLevel > 0)
        {
            goClawHead.transform.localScale = new Vector3(clawHeadScale_Lv2, clawHeadScale_Lv2, 1.0f);
            minLength = minLength_Lv2;
            if (curLength < minLength) curLength = minLength;
            if (breakForce > 0)
                breakForce = breakForce_Lv2;
            if (curLevel < luckBonus_perLv.Count)
                luckBonus = luckBonus_perLv[curLevel];
            maxDrag = maxDrag_Lv2;
        }
    }
    public Vector3 GetClawHeadPos()
    {
        return goClawHead.transform.position;
    }
    public void StartExtend()
    {
        if (curState == ClawState.swing)
        {
            SwitchState(ClawState.extend);
            canBreak = true;
        }
        else if (curLevel > 0 && netReady)
        {
            ActivateNetCapture();
        }
    }



    public void TryDrag(Ore ore, bool centerDrag = false)
    {
        if (listOnDragOres.Contains(ore))
            return;
        if (curState == ClawState.extend)
        {
            if (isRockBreak && canBreak)
            {
                StartDragOre(ore, centerDrag);
                if (ore.OnHit(new DamageInfo(breakForce, MinerManager.Instance.GetMinerEntity(), DamageType.melee))){
                    canBreak = false;
                    if(listOnDragOres.Count != 0)
                    {
                        TryNetCapture();
                        SwitchState(ClawState.drag);
                    }
                }
                else
                {
                    TryNetCapture();
                    SwitchState(ClawState.drag);
                }
            }
            else {
                StartDragOre(ore, centerDrag);
                TryNetCapture();
                SwitchState(ClawState.drag);
            }
        }
        else if(curState == ClawState.drag)
        {
            if (listOnDragOres.Count < maxDrag) {
                if (isRockBreak && canBreak)
                {
                    StartDragOre(ore, centerDrag);
                    if (ore.OnHit(new DamageInfo(breakForce, MinerManager.Instance.GetMinerEntity(), DamageType.melee, luckBonus))){
                        canBreak = false;
                    }
                    else
                    {
                        TryNetCapture();
                    }
                }
                else
                {
                    StartDragOre(ore, centerDrag);
                    TryNetCapture();
                }
            }
        }

    }
    public void StartDragOre(Ore ore, bool centerDrag = false)
    {
        if (listOnDragOres.Contains(ore))
            return;
        listOnDragOres.Add(ore);
        if (centerDrag)
            listOnDragDistance.Add(new Vector3(0, 0, 0));
        else
            listOnDragDistance.Add(ore.transform.position - goClawHead.transform.position);
        ore.StartOnDrag(this);
        CalcDragScale();
        if (audioStartDrag) SEManager.Instance.PlaySE(audioStartDrag);
    }

    public void ActivateNetCapture()
    {
        if (curState == ClawState.extend || curState == ClawState.drag)
        {
            TryNetCapture();
            if (!netReady)
                SwitchState(ClawState.drag);
        }
        

    }
    private void TryNetCapture()
    {
        if (isNetClaw && netReady)
        {
            List<Collider2D> list_colliders = new List<Collider2D>();
            Physics2D.OverlapCollider(hitboxNet, targetFilterNet, list_colliders);
            foreach (var item in list_colliders)
            {
                if (item.tag == "ore")
                {
                    Ore ore = item.GetComponent<Ore>();
                    if (ore)
                    {
                        if (ore.CanDragByNet())
                        {
                            this.StartDragOre(ore);
                        }
                    }
                }
            }
            goNetFX.SetActive(true);
            netReady = false;
        }
    }

    private void Swing()
    {
        if (curSwingDegree > 180)
        {
            curSwingDegree = curSwingDegree - 360.0f;
        }
        if (curSwingDegree < minSwingDegree) curSwingDegree = minSwingDegree;
        if (curSwingDegree > maxSwingDegree) curSwingDegree = maxSwingDegree;
        if (swingLeft2Right)
        {
            curSwingDegree += swingSpeed * Time.fixedDeltaTime;
            if (curSwingDegree > maxSwingDegree)
            {
                curSwingDegree = maxSwingDegree;
                swingLeft2Right = false;
            }
        }
        else
        {
            curSwingDegree -= swingSpeed * Time.fixedDeltaTime;
            if (curSwingDegree < minSwingDegree)
            {
                curSwingDegree = minSwingDegree;
                swingLeft2Right = true;
            }
        }
    }

    private void Extend()
    {
        curLength += extendSpeed * Time.fixedDeltaTime;
        if (curLength >= maxLength)
        {
            curLength = maxLength;
            SwitchState(ClawState.drag);
        }
    }

    public void TryCoMoveDrag(GameObject src, Vector3 srcMove)
    {
        float _coMoveDragScale = coMoveDragScale;
        float _coMoveDragScale_close = coMoveDragScale_close;

        Vector3 clawHeadPos = this.transform.position + Quaternion.Euler(0,0,curSwingDegree) * Vector3.down * curLength;
        //Debug.Log(goClawHead.transform.position);

        if (dragScale == 0)
        {
            Vector3 newPosition = clawHeadPos - srcMove;
            float tmpLength = (newPosition - this.transform.position).magnitude;
            curLength = tmpLength;
            curSwingDegree = Quaternion.FromToRotation(Vector3.up, goClaw.transform.position - newPosition).eulerAngles.z;
            _coMoveDragScale = 1.0f;
            _coMoveDragScale_close = 1.0f;
        }
        else
        {
            if (curState == ClawState.drag)
            {
                Vector3 distance = clawHeadPos - src.transform.position;
                Vector3 newPosition;
                if (distance.x * srcMove.x > 0)
                    newPosition = clawHeadPos - srcMove * _coMoveDragScale;
                else
                    newPosition = clawHeadPos - srcMove * _coMoveDragScale_close;
                float tmpLength = (newPosition - this.transform.position).magnitude;
                if (dragScale != 0)
                {
                    if (tmpLength < curLength) curLength = tmpLength;
                }
                curSwingDegree = Quaternion.FromToRotation(Vector3.up, this.transform.position - newPosition).eulerAngles.z;
            }
        }
    }
    public void TryRefreshDragingOre(Ore ore, OreOnHitBehavior behavior)
    {
        if(behavior == OreOnHitBehavior.none || behavior == OreOnHitBehavior.canDrag)
        {
            // do nothing
        }
        else if (listOnDragOres.Contains(ore))
        {
            int index = listOnDragOres.IndexOf(ore);
            listOnDragOres.Remove(ore);
            listOnDragDistance.RemoveAt(index);
        }
        CalcDragScale();
    }

    public void OnHitObject(IClawHitable obj)
    {
        if (curState == ClawState.extend)
        {
            if (obj.OnClawHit())
                SwitchState(ClawState.drag);
        }
    }

    public void OnHitEntity(Entity tar)
    {
        if (curState == ClawState.extend)
        {
            if (tar.OnClawHit())
                SwitchState(ClawState.drag);
        }
    }

    private void Drag()
    {
        float scale = dragScale;
        if (scale == 0)
            return;
        if (EffectManager.Instance.GetTimerFastClaw() > 0.0f)
            scale = scale * EffectManager.Instance.FastClawSpeedScale;
        curLength -= dragSpeed * scale * Time.fixedDeltaTime;
        if (curLength <= dragFinishLength)
        {
            curLength = minLength;
            GotchaOres();
            SwitchState(ClawState.swing);
            if (isNetClaw)
            {
                goNetFX.SetActive(false);
                netReady = true;
            }
        }
        DragOre();
        
    }

    private void SwitchState(ClawState state)
    {
        curState = state;
    }

    private void CalcDragScale()
    {
        // 抓钩缩回速度 = 基础速度 - 矿物数量 * (1 - e^(-0.5 * (矿物总重量 / 矿物数量)))，缩回速度<1则=1，-0.5作为调节系数。
        float totalWeight = 0.0f;
        bool canNotDrag = false;
        dragScale = 1.0f;
        if (listOnDragOres.Count > 0)
        {
            foreach (Ore ore in listOnDragOres)
            {
                if (ore.IsCannotDrag())
                {
                    canNotDrag = true;
                    break;
                }
                else
                {
                    totalWeight += ore.GetWeight();
                }
            }
            //float speed = dragSpeed - listOnDragOres.Count * (1 - Mathf.Exp(-0.5f * (totalWeight / listOnDragOres.Count)));
            //dragScale = speed / dragSpeed;
            //dragScale = 1.0f - listOnDragOres.Count * (1.0f - Mathf.Exp(-0.5f * (totalWeight / listOnDragOres.Count)));
            dragScale = Mathf.Exp(-0.5f * (totalWeight));
            if (dragScale < minDragScale) dragScale = minDragScale;
            if (canNotDrag)
                dragScale = 0.0f;
        }
        else
        {
            dragScale = 1.0f;
        }
    }

    private void DragOre()
    {
        for (int i = 0; i < listOnDragOres.Count; i++)
        {
            if (listOnDragDistance[i].magnitude > minDragingOreDistance)
            {
                float newMagnitue = listOnDragDistance[i].magnitude - absorbDragingOreSpeed * Time.fixedDeltaTime;
                listOnDragDistance[i] = listOnDragDistance[i].normalized * newMagnitue;
            }
            listOnDragOres[i].transform.position = goClawHead.transform.position + listOnDragDistance[i];
        }
        //foreach (Ore ore in listOnDragOres)
        //{
        //    ore.transform.position = goClawHead.transform.position;
        //}
    }

    private void GotchaOres()
    {
        if(listOnDragOres.Count>0)
            if (audioGotcha) SEManager.Instance.PlaySE(audioGotcha);
        foreach (Ore ore in listOnDragOres)
        {
            ore.OnGotcha();
        }
        listOnDragOres.Clear();
        listOnDragDistance.Clear();
        CalcDragScale();
    }
    // Start is called before the first frame update
    void Start()
    {
        // debug code; use Init()
        curLength = minLength;
        targetFilterNet = Utils.InitContactFilter(targetMaskNet, useTriggerNet);
    }

    // Update is called once per frame
    void Update()
    {
        

        // debug code
        //if (Input.GetKeyDown(KeyCode.DownArrow))
        //{
        //    StartExtend();
        //}
    }

    private void FixedUpdate()
    {
        goClawHead.transform.localPosition = new Vector3(0, -curLength, 0);
        goClaw.transform.rotation = Quaternion.Euler(new Vector3(0, 0, curSwingDegree));
        clawLine.SetPosition(0, clawLineSrc.transform.position);
        clawLine.SetPosition(1, clawLineTar.transform.position);
        switch (curState)
        {
            case ClawState.swing:
                Swing();
                break;
            case ClawState.extend:
                Extend();
                break;
            case ClawState.drag:
                Drag();
                break;
            default:
                break;
        }
    }

    public void StartExtendByInput()
    {
        StartExtend();
    }
}
