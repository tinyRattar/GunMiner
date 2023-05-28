using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using System;

[Serializable]
public class ClawInfo
{
    float extendSpeed;
    float dragSpeed;

    public ClawInfo(float extendSpeed, float dragSpeed)
    {
        this.extendSpeed = extendSpeed;
        this.dragSpeed = dragSpeed;
    }

    public float ExtendSpeed { get => extendSpeed; set => extendSpeed = value; }
    public float DragSpeed { get => dragSpeed; set => dragSpeed = value; }
}


public class ClawManager : Singleton<ClawManager>
{
    [SerializeField] List<GameObject> listClaws;
    [SerializeField] List<int> listPrices;
    List<List<ClawInfo>> listInfos_allLevel;
    [SerializeField] string csv_name;
    int curClawIdx;
    public void UpdateClawInfo()
    {
        for (int i = 0; i < listClaws.Count; i++)
        {
            Claw claw = listClaws[i].GetComponent<Claw>();
            int curLevel = MinerManager.Instance.GetLevelMiner();
            claw.UpdateInfo(listInfos_allLevel[curLevel][i], curLevel);
        }
    }
    public void SetPrice(int idx, int value)
    {
        listPrices[idx] = value;
    }
    public int GetPrice(int idx)
    {
        return listPrices[idx];
    }
    public void ChangeCurrentClaw(int idx)
    {
        curClawIdx = idx;
        MinerManager.Instance.GetMiner().SetCurClaw(GetCurrentClaw());
        LoadCurrentClaw();
    }
    public int GetCurrentClawIdx()
    {
        return curClawIdx;
    }
    public Claw GetCurrentClaw()
    {
        return listClaws[curClawIdx].GetComponent<Claw>();
    }
    private void LoadCurrentClaw()
    {
        for (int i = 0; i < listClaws.Count; i++)
        {
            if (curClawIdx != i)
            {
                listClaws[i].SetActive(false);
            }
            else
            {
                listClaws[i].SetActive(true);
            }
        }
    }

    private void Init()
    {
        if (csv_name == "")
            return;
        DataTable dataTable;
        try
        {
            dataTable = CSV_util.Read("./" + csv_name + ".csv");
        }
        catch (Exception)
        {
            Debug.LogError("load resources " + csv_name);
            dataTable = CSV_util.LoadFromResources(csv_name);
        }
        listInfos_allLevel = new List<List<ClawInfo>>();
        for (int level = 0; level <= MinerManager.MAX_LEVEL_MINER; level++)
        {
            listInfos_allLevel.Add(new List<ClawInfo>());
            for (int i = 1; i < dataTable.Rows.Count; i++)
            {
                if (dataTable.Rows[i][0].ToString()[0] == '/')
                    continue;
                int _idx = int.Parse(dataTable.Rows[i][0].ToString());
                int _type = int.Parse(dataTable.Rows[i][2].ToString());
                if (_type != 2) continue;
                string rawInfo = dataTable.Rows[i][3 + level].ToString();
                rawInfo = rawInfo.Substring(1, rawInfo.Length - 2);
                string[] infos = rawInfo.Split('_');

                //float _param1 = float.Parse(infos[0]);
                //float _param2 = float.Parse(infos[1]);
                float _dragSpeed = int.Parse(infos[2]);
                float _extendSpeed = float.Parse(infos[5]);
                //listInfos.Add(new WeaponInfo(_damage, _fireInterval, _fireNum, _knowbackPower));
                listInfos_allLevel[level].Add(new ClawInfo(_extendSpeed, _dragSpeed));
            }
        }
    }

    protected override void Awake()
    {
        base.Awake();
        Init();
    }
    // Start is called before the first frame update
    void Start()
    {
        LoadCurrentClaw();
        UpdateClawInfo();
    }



    // Update is called once per frame
    void Update()
    {
        if (MainManager.Instance.InCheatMode() && Input.GetKeyDown(KeyCode.F4))
        {
            curClawIdx++;
            if (curClawIdx == listClaws.Count) curClawIdx = 0;
            ChangeCurrentClaw(curClawIdx);
        }
    }
}
