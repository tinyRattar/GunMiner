using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using System;

public class LootManager : Singleton<LootManager>
{
    [SerializeField] List<GameObject> pfbLoots;
    List<ItemLoot> listLoots = new List<ItemLoot>();
    float lootMulti = 1.0f;
    float timerGoldFeverMulti;
    [SerializeField] List<float> listPropOre;
    [SerializeField] List<float> listPropMob;
    [SerializeField] List<float> listLastTime = new List<float>();
    bool hasInit = false;

    public int GetRandomIndexForOre()
    {
        if (!hasInit) Init();
        float totalProp = 0.0f;
        for (int i = 0; i < listPropOre.Count; i++)
        {
            totalProp += listPropOre[i];
        }
        float ranNum = UnityEngine.Random.Range(0.0f, totalProp);
        int idx = -1;
        for (int k = 0; k < listPropOre.Count; k++)
        {
            if (ranNum > listPropOre[k])
                ranNum -= listPropOre[k];
            else
            {
                idx = k;
                break;
            }
        }
        return idx;
    }
    public int GetRandomIndexForMob()
    {
        if (!hasInit) Init();
        float totalProp = 0.0f;
        for (int i = 0; i < listPropMob.Count; i++)
        {
            totalProp += listPropMob[i];
        }
        float ranNum = UnityEngine.Random.Range(0.0f, totalProp);
        int idx = -1;
        for (int k = 0; k < listPropMob.Count; k++)
        {
            if (ranNum > listPropMob[k])
                ranNum -= listPropMob[k];
            else
            {
                idx = k;
                break;
            }
        }
        return idx;
    }
    public float GetItemLastTime(int idx)
    {
        return listLastTime[idx];
    }

    public void RegistLoot(ItemLoot loot)
    {
        if (listLoots.Contains(loot))
        {
            // do nothing, only happened when preset in scene
        }
        else
        {
            listLoots.Add(loot);
        }
    }

    public void UnregistLoot(ItemLoot loot)
    {
        if (listLoots.Contains(loot))
        {
            listLoots.Remove(loot);
        }
    }

    public float GetLootMulti()
    {
        if (timerGoldFeverMulti > 0.0f)
            return lootMulti * 2.0f;
        return lootMulti;
    }
    public void ActivateGoldFever(float lastTime)
    {
        timerGoldFeverMulti = lastTime;
    }


    public GameObject GenerateOnce(int idx, Vector3 position, Quaternion rotation)
    {
        GameObject go = GameObject.Instantiate(pfbLoots[idx], position, rotation, MainManager.GetParentLoots().transform);
        ItemLoot loot = go.GetComponent<ItemLoot>();
        loot.StartShift();
        RegistLoot(loot);
        return go;
    }
    public void ForceRemoveAll()
    {
        while (listLoots.Count > 0)
        {
            int lastCount = listLoots.Count;
            if (listLoots[0])
            {
                listLoots[0].ForceRemove();
            }
            else
            {
                Debug.LogError("?");
                listLoots.RemoveAt(0);
            }
            if (listLoots.Count == lastCount)
            {
                listLoots.RemoveAt(0);
                Debug.LogError("!?");
            }
        }
    }

    private void Init()
    {
        DataTable dtProps;
        try
        {
            dtProps = CSV_util.Read("./lootProps.csv");
        }
        catch (Exception)
        {
            dtProps = CSV_util.LoadFromResources("lootProps");
        }

        for (int i = 1; i < dtProps.Rows.Count; i++)
        {
            if (dtProps.Rows[i][0].ToString()[0] == '/')
                continue;
            int _dropType = int.Parse(dtProps.Rows[i][3].ToString());
            float _dropRate = float.Parse(dtProps.Rows[i][4].ToString());
            float _lastTime = float.Parse(dtProps.Rows[i][6].ToString());

            if (_dropType == 0)
            {
                listPropMob.Add(_dropRate);
                listPropOre.Add(_dropRate);
            }
            else if (_dropType == 1)
            {
                listPropMob.Add(0);
                listPropOre.Add(_dropRate);
            }
            else if (_dropType == 2)
            {
                listPropMob.Add(_dropRate);
                listPropOre.Add(0);
            }
            listLastTime.Add(_lastTime);
        }

        hasInit = true;
    }

    protected override void Awake()
    {
        base.Awake();
        Init();
    }

    private void Start()
    {

    }

    private void Update()
    {
        if (timerGoldFeverMulti > 0.0f)
        {
            timerGoldFeverMulti -= Time.deltaTime;
        }
    }

}
