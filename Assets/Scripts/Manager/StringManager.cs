using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using System;

public class StringManager : Singleton<StringManager>
{
    [SerializeField] string csv_name;
    Dictionary<(int, int, int), string> dicStr = new Dictionary<(int, int, int), string>();
    public string GetText(int triggerIdx, int conditionIdx1, int conditionIdx2)
    {
        return dicStr[(triggerIdx, conditionIdx1, conditionIdx2)];
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
        for (int i = 1; i < dataTable.Rows.Count; i++)
        {
            if (dataTable.Rows[i][0].ToString()[0] == '/')
                break;
            int _idx = int.Parse(dataTable.Rows[i][0].ToString());
            int _triggerIdx = int.Parse(dataTable.Rows[i][1].ToString());
            int _conditionIdx = int.Parse(dataTable.Rows[i][2].ToString());
            int _conditionIdx2 = int.Parse(dataTable.Rows[i][3].ToString());
            string _content = dataTable.Rows[i][4].ToString();
            //listStr.Add(_content);
            dicStr.Add((_triggerIdx, _conditionIdx, _conditionIdx2), _content);
        }
    }

    protected override void Awake()
    {
        base.Awake();
        Init();
    }
}
