using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using System;

public enum RecordType
{
    killNum,
    earnMoney,
    fishGotchaNum,
    itemUsedNum,
    clamDestroyNum,
    junkGotchaNum,
    spendMoney,
    onRangeHitNum,
    winNum,
    deadNum,
    levelClear,
    end
}

public class Achievement
{
    int index;
    string nickname;
    bool showFlag;
    RecordType recordType;
    int tarValue;
    int reward;
    int spriteIndex = 0;
    string rewardInfo = "";

    public Achievement(int index, string nickname, bool showFlag, RecordType recordType, int tarValue, int reward)
    {
        this.index = index;
        this.nickname = nickname;
        this.showFlag = showFlag;
        this.recordType = recordType;
        this.tarValue = tarValue;
        this.reward = reward;
    }

    public int Index { get => index; set => index = value; }
    public string Nickname { get => nickname; set => nickname = value; }
    public bool ShowFlag { get => showFlag; set => showFlag = value; }
    public RecordType RecordType { get => recordType; set => recordType = value; }
    public int TarValue { get => tarValue; set => tarValue = value; }
    public int Reward { get => reward; set => reward = value; }
    public int SpriteIndex { get => spriteIndex; set => spriteIndex = value; }
    public string RewardInfo { get => rewardInfo; set => rewardInfo = value; }
}

public class AchievementManager : Singleton<AchievementManager>
{
    Dictionary<RecordType, int> dicValues;
    List<Achievement> listAchievements;
    [SerializeField] string csv_name;
    List<int> listRewardGot;
    List<int> listRewardNew;

    [SerializeField] List<StickerBehavior> listStickers;
    [SerializeField] Transform anchorStickerEnter;
    [SerializeField] GameObject vfx_message;
    [SerializeField] Transform anchorMessage;

    [Header("“Ù–ß")]
    [SerializeField] AudioClip audioGetReward;

    [Header("µ˜ ‘")]
    [SerializeField] int debug_rewardIdx;
    [SerializeField] bool debug_gotReward;

    public void ChangeValue(RecordType key, int value)
    {
        dicValues[key] += value;
        if (key == RecordType.deadNum)
            SaveManager.Instance.SaveDeadNum(dicValues[key]);
    }

    public int GetValue(RecordType key)
    {
        return dicValues[key];
    }

    public List<int> GetListRewardNew()
    {
        List<int> ret = new List<int>(listRewardNew);
        listRewardNew.Clear();
        return ret;
    }


    public Achievement CheckAchievementSingleType(RecordType rType)
    {
        Achievement ret = null;
        int maxValue = 0;

        foreach (Achievement achievement in listAchievements)
        {
            if(achievement.RecordType == rType)
            {
                if (dicValues[rType] >= achievement.TarValue)
                {
                    if (achievement.Reward >= 0)
                    {
                        //Debug.Log(achievement.Nickname + " " + achievement.Reward);
                        TryGetReward(achievement.Reward);
                    }
                    if (achievement.TarValue > maxValue)
                    {
                        ret = achievement;
                        maxValue = achievement.TarValue;
                    }
                }
            }
        }

        return ret;
    }

    public List<Achievement> CheckAchievement()
    {
        List<Achievement> listRet = new List<Achievement>();
        for (int i = 0; i < (int)RecordType.end; i++)
        {
            Achievement ret = CheckAchievementSingleType((RecordType)i);
            if (ret != null)
                if (ret.ShowFlag)
                    listRet.Add(ret);
        }

        return listRet;
    }

    public int GetScore(out bool isHighScore)
    {
        int ret = 0;

        ret += dicValues[RecordType.killNum] * 10;
        ret += dicValues[RecordType.earnMoney] * 10;
        ret += dicValues[RecordType.itemUsedNum] * 30;
        ret += dicValues[RecordType.clamDestroyNum] * 50;
        ret += dicValues[RecordType.levelClear] * 500;

        isHighScore = SaveManager.Instance.TryUpdateHighScore(ret);

        return ret;
    }

    public void StartRewardEnterAnim(int idx)
    {
        listStickers[idx].OnGot(anchorStickerEnter.position);
        for (int i = 0; i < listAchievements.Count; i++)
        {
            if (listAchievements[i].Reward == idx)
            {
                GameObject go = GameObject.Instantiate(vfx_message, anchorMessage.position, Quaternion.identity, anchorMessage);
                go.GetComponent<FX_Message>().Init(listAchievements[i].RewardInfo);
            }
        }
        if (audioGetReward) SEManager.Instance.PlaySE(audioGetReward);
    }

    private void TryGetReward(int idx)
    {
        if (listRewardGot.Contains(idx))
        {
            return;
        }
        else
        {
            listRewardGot.Add(idx);
            listRewardNew.Add(idx);
            SaveRewardStatus(idx);
        }
    }
    private void SaveRewardStatus(int idx)
    {
        SaveManager.Instance.SaveRewardGot(idx);
    }

    private void LoadRewardStatus()
    {
        foreach (int idx in SaveManager.Instance.LoadListRewardGot())
        {
            listRewardGot.Add(idx);
            listStickers[idx].SetShowAtLoad();
        }
    }

    private void Init()
    {
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

        int showCount = 0;
        for (int i = 1; i < dataTable.Rows.Count; i++)
        {
            if (dataTable.Rows[i][0].ToString()[0] == '/')
                break;
            int _idx = int.Parse(dataTable.Rows[i][0].ToString());
            string _nickname = dataTable.Rows[i][1].ToString();
            bool _showFlag = (dataTable.Rows[i][2].ToString().ToUpper() == "TRUE");
            RecordType _recordType = (RecordType)(int.Parse(dataTable.Rows[i][3].ToString()) - 1);
            int _targetValue = int.Parse(dataTable.Rows[i][4].ToString());
            string _stickname = dataTable.Rows[i][5].ToString();
            string _stickinfo = dataTable.Rows[i][6].ToString();

            int _reward = -1;
            if (_stickname != "")
            {
                _reward = int.Parse(_stickname.Split('_')[1]) - 1;
            }
            Achievement achieve = new Achievement(_idx, _nickname, _showFlag, _recordType, _targetValue, _reward);
            if (_reward >= 0)
            {
                achieve.RewardInfo = _stickinfo;
            }
            if (_showFlag)
            {
                achieve.SpriteIndex = showCount;
                showCount++;
            }
            listAchievements.Add(achieve);
        }

        listRewardGot = new List<int>();
        listRewardNew = new List<int>();
    }

    protected override void Awake()
    {
        base.Awake();
        listAchievements = new List<Achievement>();
        Init();
        dicValues = new Dictionary<RecordType, int>();
        for (int i = 0; i < (int)RecordType.end; i++)
        {
            dicValues.Add((RecordType)i, 0);
            //int value = UnityEngine.Random.Range(0, 10000);
            //dicValues.Add((RecordType)i, value);
        }
    }

    private void Start()
    {
        LoadRewardStatus();
        dicValues[RecordType.deadNum] = SaveManager.Instance.LoadDeadNum();
    }

    private void Update()
    {
        if (debug_gotReward)
        {
            debug_gotReward = false;
            TryGetReward(debug_rewardIdx);
            StartRewardEnterAnim(debug_rewardIdx);
        }
    }

}
