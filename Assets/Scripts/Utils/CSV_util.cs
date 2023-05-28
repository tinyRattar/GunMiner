using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System.Data;

public class CSV_util
{
    public static DataTable Read(string filename)
    {
        DataTable dt = new DataTable();
        using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
        {
            using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
            {
                string curLine = "";
                bool isFirstLine = true;
                int colNum = 0;
                while((curLine = sr.ReadLine()) != null)
                {
                    if (isFirstLine)
                    {
                        isFirstLine = false;
                        string[] tableHead = curLine.Split(',');
                        colNum = tableHead.Length;
                        for (int i = 0; i < colNum; i++)
                        {
                            DataColumn dc = new DataColumn(tableHead[i]);
                            dt.Columns.Add(dc);
                        }
                    }
                    else
                    {
                        string[] values = curLine.Split(',');
                        if (values.Length != colNum)
                        {
                            Debug.LogError("bad CSV file:" + filename);
                            break;
                        }
                        DataRow dr = dt.NewRow();
                        for (int i = 0; i < colNum; i++)
                        {
                            dr[i] = values[i].Replace("\n","");
                        }
                        dt.Rows.Add(dr);
                    }
                }
                sr.Close();
            }
            fs.Close();
        }

        return dt;
    }

    public static DataTable LoadFromResources(string filename)
    {
        DataTable dt = new DataTable();
        TextAsset textAsset = Resources.Load<TextAsset>(filename);
        string content = textAsset.text;
        string[] lines = content.Split('\r');
        bool isFirstLine = true;
        int colNum = 0;
        foreach (string curLine in lines)
        {
            if (curLine.Length == 1) break;
            if (isFirstLine)
            {
                isFirstLine = false;
                string[] tableHead = curLine.Split(',');
                colNum = tableHead.Length;
                for (int i = 0; i < colNum; i++)
                {
                    DataColumn dc = new DataColumn(tableHead[i]);
                    dt.Columns.Add(dc);
                }
            }
            else
            {
                string[] values = curLine.Split(',');
                if (values.Length != colNum)
                {
                    Debug.Log(curLine);
                    Debug.LogError("bad CSV file:" + filename);
                    //break;
                }
                DataRow dr = dt.NewRow();
                for (int i = 0; i < colNum; i++)
                {
                    dr[i] = values[i].Replace("\n", "");
                }
                dt.Rows.Add(dr);
            }
        }

        return dt;
    }

    public static DataTable LoadFromResources_noCommaSplit(string filename)
    {
        DataTable dt = new DataTable();
        TextAsset textAsset = Resources.Load<TextAsset>(filename);
        string content = textAsset.text;
        string[] lines = content.Split('\r');
        bool isFirstLine = true;
        int colNum = 0;
        foreach (string curLine in lines)
        {
            if (curLine.Length == 1) break;
            if (isFirstLine)
            {
                isFirstLine = false;
                string[] tableHead = curLine.Split(',');
                colNum = tableHead.Length;
                for (int i = 0; i < colNum; i++)
                {
                    DataColumn dc = new DataColumn(tableHead[i]);
                    dt.Columns.Add(dc);
                }
            }
            else
            {
                DataRow dr = dt.NewRow();
                dr[0] = curLine.Replace("\n", "");
                dt.Rows.Add(dr);
            }
        }

        return dt;
    }
}
