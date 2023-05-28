using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using System;

[Serializable]
public class WeaponInfo
{
    [SerializeField] float damage;
    [SerializeField] float fireInterval;
    [SerializeField] int fireNum;
    [SerializeField] float knockbackPower;

    public WeaponInfo(float damage, float fireInterval, int fireNum, float knockbackPower)
    {
        this.damage = damage;
        this.fireInterval = fireInterval;
        this.fireNum = fireNum;
        this.knockbackPower = knockbackPower;
    }

    public float Damage { get => damage; set => damage = value; }
    public float FireInterval { get => fireInterval; set => fireInterval = value; }
    public int FireNum { get => fireNum; set => fireNum = value; }
    public float KnockbackPower { get => knockbackPower; set => knockbackPower = value; }
}

public class WeaponManager : Singleton<WeaponManager>
{
    [SerializeField] List<GameObject> listWeapons;
    [SerializeField] List<int> listPrices;
    List<List<WeaponInfo>> listInfos_allLevel;
    [SerializeField] string csv_name;
    int curWeaponIdx;

    [SerializeField] bool showNextList;
    [SerializeField] List<WeaponInfo> listInfos;
    int tmpIndex = 0;

    public void UpdateWeaponInfo()
    {
        for (int i = 0; i < listWeapons.Count; i++)
        {
            Weapon weapon = listWeapons[i].GetComponent<Weapon>();
            int curLevel = MinerManager.Instance.GetLevelGunner();
            weapon.UpdateInfo(listInfos_allLevel[curLevel][i], curLevel);
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

    public void ChangeCurrentWeapon(int idx)
    {
        curWeaponIdx = idx;
        LoadCurrentWeapon();
    }

    public int GetCurrentWeaponIdx()
    {
        return curWeaponIdx;
    }

    public Weapon GetCurrentWeapon()
    {
        return listWeapons[curWeaponIdx].GetComponent<Weapon>();
    }

    private void LoadCurrentWeapon()
    {
        for (int i = 0; i < listWeapons.Count; i++)
        {
            if (curWeaponIdx != i)
            {
                listWeapons[i].SetActive(false);
            }
            else
            {
                listWeapons[i].SetActive(true);
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
        listInfos_allLevel = new List<List<WeaponInfo>>();
        for (int level = 0; level <= MinerManager.MAX_LEVEL_GUNNER; level++)
        {
            listInfos_allLevel.Add(new List<WeaponInfo>());
            for (int i = 1; i < dataTable.Rows.Count; i++)
            {
                if (dataTable.Rows[i][0].ToString()[0] == '/')
                    continue;
                int _idx = int.Parse(dataTable.Rows[i][0].ToString());
                int _type = int.Parse(dataTable.Rows[i][2].ToString());
                if (_type != 1) continue;
                string rawInfo = dataTable.Rows[i][3 + level].ToString();
                rawInfo = rawInfo.Substring(1, rawInfo.Length - 2);
                string[] infos = rawInfo.Split('_');

                float _damage = float.Parse(infos[0]);
                float _fireInterval = float.Parse(infos[1]);
                int _fireNum = int.Parse(infos[3]);
                float _knowbackPower = float.Parse(infos[4]);
                //listInfos.Add(new WeaponInfo(_damage, _fireInterval, _fireNum, _knowbackPower));
                listInfos_allLevel[level].Add(new WeaponInfo(_damage, _fireInterval, _fireNum, _knowbackPower));
            }
        }
        listInfos = listInfos_allLevel[0];
    }

    protected override void Awake()
    {
        base.Awake();
        Init();
    }
    // Start is called before the first frame update
    void Start()
    {
        LoadCurrentWeapon();
        UpdateWeaponInfo();
    }

    // Update is called once per frame
    void Update()
    {
        if (showNextList)
        {
            tmpIndex++;
            if (tmpIndex >= listInfos_allLevel.Count) tmpIndex = 0;
            listInfos = listInfos_allLevel[tmpIndex];
            showNextList = false;
        }
        if (MainManager.Instance.InCheatMode() && Input.GetKeyDown(KeyCode.F3))
        {
            listWeapons[curWeaponIdx].SetActive(false);
            curWeaponIdx++;
            if (curWeaponIdx == listWeapons.Count) curWeaponIdx = 0;
            listWeapons[curWeaponIdx].SetActive(true);
        }
    }
}
