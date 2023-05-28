using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using System;

public class NPC : MonoBehaviour
{
    [SerializeField] DialogBehavior dialogBehavior;
    //[SerializeField] string csv_name;
    //[SerializeField] List<string> listStr;
    //Dictionary<(int, int, int), string> dicStr = new Dictionary<(int, int, int), string>();

    //public void ShowText(int idx)
    //{
    //    dialogBehavior.SetContent(listStr[idx]);
    //    dialogBehavior.SetShow(true);
    //}

    //public string GetText(int triggerIdx, int conditionIdx1, int conditionIdx2)
    //{
    //    return dicStr[(triggerIdx, conditionIdx1, conditionIdx2)];
    //}
    public bool IsDialogTotalHide()
    {
        return dialogBehavior.IsTotalHide();
    }

    public void CloseDialog()
    {
        dialogBehavior.SetShow(false);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="triggerIdx">0=level, 1=equip, 2=upgrade</param>
    /// <param name="conditionIdx1">0:level; 1:shopId; 2:shopId</param>
    /// <param name="conditionIdx2">0:-; 1:-; 2:equiplevel</param>
    public void ShowText(int triggerIdx, int conditionIdx1, int conditionIdx2)
    {
        bool ret;
        ret = dialogBehavior.SetContent(StringManager.Instance.GetText(triggerIdx, conditionIdx1, conditionIdx2));
        //if (triggerIdx == 1 && conditionIdx1 == -1 && conditionIdx2 == 10)
        //    ret = dialogBehavior.SetContent("ƒ„Õ®πÿ¿≤£°");
        //else
        //    ret = dialogBehavior.SetContent(StringManager.Instance.GetText(triggerIdx, conditionIdx1, conditionIdx2));
        if (ret)
        {
            int idxAudio = 0;
            if (triggerIdx == 2) idxAudio = 1;
            if (triggerIdx == 4) idxAudio = 2;
            if (triggerIdx == 5) idxAudio = 3;
            dialogBehavior.SetShow(true, idxAudio);
        }
    }

    //private void Init()
    //{
    //    DataTable dataTable;
    //    try
    //    {
    //        dataTable = CSV_util.Read("./" + csv_name + ".csv");
    //    }
    //    catch (Exception)
    //    {
    //        Debug.LogError("load resources " + csv_name);
    //        dataTable = CSV_util.LoadFromResources(csv_name);
    //    }
    //    for (int i = 1; i < dataTable.Rows.Count; i++)
    //    {
    //        if (dataTable.Rows[i][0].ToString()[0] == '/')
    //            break;
    //        int _idx = int.Parse(dataTable.Rows[i][0].ToString());
    //        int _triggerIdx = int.Parse(dataTable.Rows[i][1].ToString());
    //        int _conditionIdx = int.Parse(dataTable.Rows[i][2].ToString());
    //        int _conditionIdx2 = int.Parse(dataTable.Rows[i][3].ToString());
    //        string _content = dataTable.Rows[i][4].ToString();
    //        //listStr.Add(_content);
    //        dicStr.Add((_triggerIdx, _conditionIdx, _conditionIdx2), _content);
    //    }
    //}

    private void Awake()
    {
        //Init();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
