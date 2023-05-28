using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using System;
using UnityEngine.UI;

[Serializable]
public class MobInfo
{
    [SerializeField] int mobIndex;
    [SerializeField] private int pfbIndex = 0;
    [SerializeField] float health;
    [SerializeField] float speed;
    [SerializeField] int attackMethod;
    [SerializeField] int drop;
    [SerializeField] List<GameObject> listGenPosAnchor;
    [SerializeField] int maxNum;


    public MobInfo(int mobIndex, int pfbIndex, float health, float speed, int attackMethod, int drop, List<GameObject> listGenPosAnchor)
    {
        this.mobIndex = mobIndex;
        this.pfbIndex = pfbIndex;
        this.health = health;
        this.speed = speed;
        this.attackMethod = attackMethod;
        this.drop = drop;
        this.listGenPosAnchor = listGenPosAnchor;
        this.maxNum = 5;
    }

    public int MobIndex { get => mobIndex; set => mobIndex = value; }
    public int PfbIndex { get => pfbIndex; set => pfbIndex = value; }
    public float Health { get => health; set => health = value; }
    public float Speed { get => speed; set => speed = value; }
    public int AttackMethod { get => attackMethod; set => attackMethod = value; }
    public int Drop { get => drop; set => drop = value; }
    public List<GameObject> ListGenPosAnchor { get => listGenPosAnchor; set => listGenPosAnchor = value; }
    public int MaxNum { get => maxNum; set => maxNum = value; }
}


public class MobManager : Singleton<MobManager>
{
    [SerializeField] MobFactory mobFactory;

    [SerializeField] List<MobInfo> mobDataset;
    [SerializeField] int maxValidIdx = 8; // manual adjust
    [SerializeField] List<GameObject> listGenPosAnchors_parents; // init by parent object
    [SerializeField] List<GameObject> listGenPosAnchors_default;
    [SerializeField] List<GameObject> listGenPosAnchors_underSea;
    [SerializeField] List<GameObject> listGenPosAnchors_deepSea;
    List<GameObject> listGenPosUsed = new List<GameObject>();
    [SerializeField] int tryGenMaxTime = 5;
    List<List<float>> listGenProps = new List<List<float>>();

    bool inWork = true;
    [SerializeField] int generateNum; // todo: calc by curLevel
    [SerializeField] float generateInterval;
    [SerializeField] float timerGenerate = 0.0f;

    [Header("动态生成数量")]
    [SerializeField] int minGenNum;
    [SerializeField] int maxGenNum;
    [SerializeField] float timeMaxGenNum;
    float timerMaxGenNum;


    [SerializeField] List<float> genProps;

    [SerializeField] int maxNum;
    List<Mob> listMobs = new List<Mob>();
    List<int> listMobCount = new List<int>();

    [Header("鲣鸟")]
    [SerializeField] List<GameObject> listGenPosAnchors_treasure;
    [SerializeField] float timeTreasureMob;
    float timerTreasureMob;

    [SerializeField] Text txtNextTreasure;

    public void RegistMob(Mob mob)
    {
        if (listMobs.Contains(mob)){
            // do nothing, only happened when preset in scene
        }
        else
        {
            listMobs.Add(mob);
            listMobCount[mob.GetMobIndex()] += 1;
        }
    }

    public void UnregistMob(Mob mob)
    {
        if (listMobs.Contains(mob))
        {
            listMobs.Remove(mob);
            listMobCount[mob.GetMobIndex()] -= 1;
        }
    }

    public List<Mob> GetListMobs()
    {
        return listMobs;
    }

    public void SetGenerateNum(int num)
    {
        generateNum = num;
    }

    public void GenerateMobs()
    {
        float totalProp = 0.0f;
        int curLevel = MainManager.Instance.GetCurLevel();
        genProps = listGenProps[curLevel];
        for (int i = 0; i < maxValidIdx; i++)
        {
            //if (listMobCount[i] < mobDataset[i].MaxNum)
            totalProp += genProps[i];
        }
        int idxMob = -1;
        float ranNum = UnityEngine.Random.Range(0.0f, totalProp);
        for (int k = 0; k < maxValidIdx; k++)
        {
            if (listMobCount[k] < mobDataset[k].MaxNum)
            {
                if (ranNum > genProps[k])
                {
                    ranNum -= genProps[k];
                }
                else
                {
                    idxMob = k;
                    break;
                }
            }
            else
            {
                ranNum -= genProps[k];
                if (ranNum < 0)
                    break;
            }
        }
        if(idxMob < 0)
        {
            return;
        }
        MobInfo mobInfo = mobDataset[idxMob];

        List<GameObject> listGenPosAnchors = mobInfo.ListGenPosAnchor;
        for (int i = 0; i < tryGenMaxTime; i++)
        {
            int idxGenPosAnchor = UnityEngine.Random.Range(0, listGenPosAnchors.Count);
            GameObject _genPos = listGenPosAnchors[idxGenPosAnchor];
            Vector3 genPos = _genPos.transform.position;
            bool _flag = true;
            if (listGenPosUsed.Contains(_genPos)){
                _flag = false;
            }


            if(_flag)
            {
                listGenPosUsed.Add(_genPos);
                GameObject go = mobFactory.GenerateMob(mobInfo, genPos, Quaternion.identity, MainManager.GetParentMobs().transform);
                Mob mob = go.GetComponent<Mob>();
                mob.SetByMobInfo(mobInfo);
                RegistMob(mob);
                break;
            }
        }
        
    }

    public void GenerateTreasureMob(GameObject genAnchor = null)
    {
        int idxMob = 8;
        if (listMobCount[idxMob] >= mobDataset[idxMob].MaxNum)
        {
            return;
        }

        MobInfo mobInfo = mobDataset[idxMob];

        if (genAnchor == null)
        {
            List<GameObject> listGenPosAnchors = mobInfo.ListGenPosAnchor;
            int genIdx = UnityEngine.Random.Range(0, listGenPosAnchors.Count);
            genAnchor = listGenPosAnchors[genIdx];
        }
        GameObject go = mobFactory.GenerateMob(mobInfo, genAnchor.transform.position, Quaternion.identity, MainManager.GetParentMobs().transform);
        TreasureMob tMob = go.GetComponent<TreasureMob>();
        tMob.SetByMobInfo(mobInfo);
        RegistMob(tMob);
        if ((genAnchor.transform.position - MinerManager.Instance.GetMiner().transform.position).x > 0)
            tMob.SetMoveDirect(Vector3.left);
        else
            tMob.SetMoveDirect(Vector3.right);
    }

    public void StartWork(int minGenNum, int maxGenNum, float timeMaxGenNum)
    {
        this.minGenNum = minGenNum;
        this.maxGenNum = maxGenNum;
        this.timeMaxGenNum = timeMaxGenNum;
        timerMaxGenNum = 0.0f;
        inWork = true;
    }

    public void ForceRemoveAll()
    {
        inWork = false;
        while (listMobs.Count>0)
        {
            int lastCount = listMobs.Count;
            if (listMobs[0])
            {
                listMobs[0].ForceRemove();
            }
            else
            {
                Debug.LogError("?");
                listMobs.RemoveAt(0);
            }
            if (listMobs.Count == lastCount)
            {
                listMobs.RemoveAt(0);
                Debug.LogError("!?");
            }
        }
    }

    private List<GameObject> GetGenPosAnchors(int pfbIdx)
    {
        if (pfbIdx == 5) // sneak
            return listGenPosAnchors_underSea;
        if (pfbIdx == 6) // wall
            return listGenPosAnchors_deepSea;
        return listGenPosAnchors_default;
    }

    private void Init()
    {
        listGenPosAnchors_default = new List<GameObject>();
        for (int i = 0; i < listGenPosAnchors_parents[0].transform.childCount; i++)
            listGenPosAnchors_default.Add(listGenPosAnchors_parents[0].transform.GetChild(i).gameObject);
        listGenPosAnchors_underSea = new List<GameObject>();
        for (int i = 0; i < listGenPosAnchors_parents[1].transform.childCount; i++)
            listGenPosAnchors_underSea.Add(listGenPosAnchors_parents[1].transform.GetChild(i).gameObject);
        listGenPosAnchors_deepSea = new List<GameObject>();
        for (int i = 0; i < listGenPosAnchors_parents[2].transform.childCount; i++)
            listGenPosAnchors_deepSea.Add(listGenPosAnchors_parents[2].transform.GetChild(i).gameObject);

        DataTable dtMobs;
        try
        {
            dtMobs = CSV_util.Read("./mobInfo.csv");
        }
        catch (Exception)
        {
            Debug.LogError("load resources");
            dtMobs = CSV_util.LoadFromResources("mobInfo");
        }
        for (int i = 1; i < dtMobs.Rows.Count; i++)
        {
            if (dtMobs.Rows[i][0].ToString()[0] == '/')
                break;
            int _pfbIdx = int.Parse(dtMobs.Rows[i][0].ToString()) - 1;
            float _health = float.Parse(dtMobs.Rows[i][2].ToString());
            float _speed = float.Parse(dtMobs.Rows[i][3].ToString());
            int _attackMethod = int.Parse(dtMobs.Rows[i][14].ToString());
            int _drop = int.Parse(dtMobs.Rows[i][15].ToString());
            int _maxNum = int.Parse(dtMobs.Rows[i][16].ToString());
            List<GameObject> _listGenPosAnchor = GetGenPosAnchors(_pfbIdx);
            MobInfo _mobInfo = new MobInfo(i - 1, _pfbIdx, _health, _speed, _attackMethod, _drop, _listGenPosAnchor);
            _mobInfo.MaxNum = _maxNum;
            //if (i - 1 == 6) //wall
            //{
            //    _mobInfo.MaxNum = 1;
            //}
            mobDataset.Add(_mobInfo);
            listMobCount.Add(0);
        }
        for (int i = 0; i < MainManager.MaxLevel; i++)
        {
            List<float> _genPropSingleLevel = new List<float>();
            for (int j = 1; j < maxValidIdx + 1; j++)
            {
                _genPropSingleLevel.Add(float.Parse(dtMobs.Rows[j][4 + i].ToString()));
            }
            listGenProps.Add(_genPropSingleLevel);
        }

        MobInfo _treasureMobInfo = new MobInfo(8, 8, 1, 5, 0, 1, listGenPosAnchors_treasure);
        _treasureMobInfo.MaxNum = 1;
        mobDataset.Add(_treasureMobInfo);
        listMobCount.Add(0);
    }

    protected override void Awake()
    {
        base.Awake();
        Init();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (inWork)
        {
            timerMaxGenNum += Time.deltaTime;
            if (timerMaxGenNum > timeMaxGenNum) timerMaxGenNum = timeMaxGenNum;
            generateNum = (int)Mathf.Lerp(minGenNum, maxGenNum, timerMaxGenNum / timeMaxGenNum);
            timerGenerate -= Time.deltaTime;
            if (timerGenerate < 0.0f)
            {
                int _generateNum = Mathf.Min(generateNum, maxNum - listMobs.Count);
                listGenPosUsed.Clear();
                for (int i = 0; i < _generateNum; i++)
                {
                    GenerateMobs();
                }
                timerGenerate = generateInterval;
            }

            if (listMobCount[8] <= 0)
            {
                timerTreasureMob += Time.deltaTime;
                if (timerTreasureMob > timeTreasureMob)
                {
                    timerTreasureMob = 0.0f;
                    GenerateTreasureMob();
                }
            }
            else
            {
                timerTreasureMob = 0.0f;
            }
            txtNextTreasure.text = (timeTreasureMob - timerTreasureMob).ToString("F2") + "s [BlueBird]";
        }
    }
}
