using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using System;

[Serializable]
public class OreInfo
{
    [SerializeField] private int pfbIndex = 0;
    [SerializeField] private int moneyValue = 0;
    [SerializeField] private int scoreValue = 0;
    [SerializeField] private float weight = 1.0f;

    public OreInfo(int moneyValue, int scoreValue, float weight)
    {
        this.moneyValue = moneyValue;
        this.scoreValue = scoreValue;
        this.weight = weight;
    }

    public int MoneyValue { get => moneyValue;  }
    public int ScoreValue { get => scoreValue;  }
    public float Weight { get => weight; }
    public int PfbIndex { get => pfbIndex; set => pfbIndex = value; }
}

public class OreManager : Singleton<OreManager>
{


    //[Serializable]
    //private class OreGenProp
    //{
    //    private List<float> listProp = new List<float>();

    //    public List<float> ListProp { get => listProp; set => listProp = value; }
    //}

    [SerializeField] OreFactory oreFactory;

    [SerializeField] List<OreInfo> oreDataset;
    [SerializeField] int maxValidIdx = 8; // manual adjust
    [SerializeField] List<GameObject> listGenArea;
    List<List<float>> listGenProps = new List<List<float>>();
    [SerializeField] List<GameObject> listGenAreaDeep;
    List<List<float>> listGenPropsDeep = new List<List<float>>();
    List<Vector2> listGeneratedPos = new List<Vector2>();
    List<Ore> listOres = new List<Ore>(); // all available ore
    [Header("矿石生成")]
    [SerializeField] int tryGenMaxTime = 5;
    [SerializeField] float genGap = 2.0f;

    [Header("调试")]
    [SerializeField] int genNumPerArea; // tmp code
    [SerializeField] int genNumPerAreaDeep; // tmp code
    [SerializeField] bool debug_startGen = false;
    [SerializeField] List<float> genProps;

    public void RegistOre(Ore ore)
    {
        if (listOres.Contains(ore))
        {
            // do nothing, only happened when preset in scene
        }
        else
        {
            listOres.Add(ore);
        }
    }

    public void UnregistOre(Ore ore)
    {
        if (listOres.Contains(ore))
        {
            listOres.Remove(ore);
        }
    }

    private void Init()
    {
        DataTable dtOres;
        try
        {
            dtOres = CSV_util.Read("./oreInfo.csv");
        }
        catch (Exception)
        {
            dtOres = CSV_util.LoadFromResources("oreInfo");
        }
        for (int i = 1; i < dtOres.Rows.Count; i++)
        {
            if (dtOres.Rows[i][0].ToString()[0] == '/')
                break;
            int _moneyValue = int.Parse(dtOres.Rows[i][3].ToString());
            int _scoreValue = int.Parse(dtOres.Rows[i][3].ToString());
            float _weight = float.Parse(dtOres.Rows[i][2].ToString());
            OreInfo _oreInfo = new OreInfo(_moneyValue, _scoreValue, _weight);
            _oreInfo.PfbIndex = (i - 1);
            oreDataset.Add(_oreInfo);
        }
        for (int i = 0; i < MainManager.MaxLevel; i++)
        {
            listGenProps.Add(new List<float>());
            listGenPropsDeep.Add(new List<float>());
            //List<float> _genPropSingleLevel = new List<float>();
            for (int j = 1; j < maxValidIdx + 1; j++)
            {
                //_genPropSingleLevel.Add(float.Parse(dtOres.Rows[j][4 + i].ToString()));
                String[] rawStrs = dtOres.Rows[j][4 + i].ToString().Split('_');
                listGenProps[i].Add(float.Parse(rawStrs[0].Substring(1, rawStrs[0].Length - 1)));
                listGenPropsDeep[i].Add(float.Parse(rawStrs[1].Substring(0, rawStrs[1].Length - 1)));

            }
            //listGenProps.Add(_genPropSingleLevel);
        }
    }

    public GameObject GenerateOre(OreInfo oreInfo, Vector3 pos)
    {
        float maxDistance = Mathf.Abs(listGenArea[0].transform.position.x - pos.x);
        int genAreaIdx = 0;
        for (int i = 1; i < listGenArea.Count; i++)
        {
            float distance = Mathf.Abs(listGenArea[i].transform.position.x - pos.x);
            if (distance < maxDistance)
            {
                maxDistance = distance;
                genAreaIdx = i;
            }
        }
        GameObject go = oreFactory.GenerateOre(oreInfo, pos, Quaternion.identity, listGenArea[genAreaIdx].transform);
        return go;
    }

    public void GenerateOres(List<GameObject> genAreas, List<float> genProps, int genNum)
    {
        float totalProp = 0.0f;
        for (int i = 0; i < maxValidIdx; i++)
        {
            totalProp += genProps[i];
        }
        for (int i = 0; i < genAreas.Count; i++)
        {
            for (int j = 0; j < genNum; j++)
            {
                BoxCollider2D areaBox = genAreas[i].GetComponent<BoxCollider2D>();
                float startX = genAreas[i].transform.position.x + areaBox.offset.x;
                float startY = genAreas[i].transform.position.y + areaBox.offset.y;
                int idxOre = maxValidIdx - 1;
                float ranNum = UnityEngine.Random.Range(0.0f, totalProp);
                for (int k = 0; k < maxValidIdx; k++)
                {
                    if (ranNum > genProps[k])
                        ranNum -= genProps[k];
                    else
                    {
                        idxOre = k;
                        break;
                    }
                }
                UnityEngine.Random.Range(0.0f, totalProp);
                OreInfo oreInfo = oreDataset[idxOre];
                for (int t = 0; t < tryGenMaxTime; t++)
                {

                    float ranX = UnityEngine.Random.Range(startX - areaBox.size.x / 2, startX + areaBox.size.x / 2);
                    float ranY = UnityEngine.Random.Range(startY - areaBox.size.y / 2, startY + areaBox.size.y / 2);

                    bool _flag = true;
                    foreach (Vector2 pos in listGeneratedPos)
                    {
                        if (Vector2.Distance(pos, new Vector2(ranX, ranY)) < genGap)
                        {
                            _flag = false;
                            break;
                        }
                    }
                    if (_flag)
                    {
                        oreFactory.GenerateOre(oreInfo, new Vector3(ranX, ranY, 0), Quaternion.identity, listGenArea[i].transform);
                        listGeneratedPos.Add(new Vector2(ranX, ranY));
                        break;
                    }
                }
            }
        }
    }
    public void ForceRemoveAll()
    {
        while (listOres.Count > 0)
        {
            int lastCount = listOres.Count;
            if (listOres[0])
            {
                listOres[0].ForceRemove();
            }
            else
            {
                Debug.LogError("?");
                listOres.RemoveAt(0);
            }
            if (listOres.Count == lastCount)
            {
                listOres.RemoveAt(0);
                Debug.LogError("!?");
            }
        }
    }

    /// <summary>
    /// called at level begin
    /// </summary>
    public void StartGenerate()
    {
        int curLevel = MainManager.Instance.GetCurLevel();
        listGeneratedPos.Clear();
        GenerateOres(listGenArea, listGenProps[curLevel], genNumPerArea);
        GenerateOres(listGenAreaDeep, listGenPropsDeep[curLevel], genNumPerAreaDeep);
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
        if (debug_startGen)
        {
            debug_startGen = false;
            StartGenerate();
        }
    }
}
