using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CharactorDataSet;
public class StageList
{
    public string code;
    public string name;
    public List<Stage> data;

    public string date_exite;
    public string date_start;
    public StageList() 
    {
        data = new List<Stage>();
    }
}
public class Stage
{
    public string name;
    public string code;
    public string hard;
    public int type;

    public string[] ndrop;
    public string[] ndropPer;
    public string[] sdrop;
    public string[] sdropPer;
    public string[] adrop;
    public string[] adropPer;

    public List<string> link;
    public List<string[]> atOperator;
    public List<string[]> atElite;
    public List<string[]> atLevel;
    public Stage()
    {
        atOperator = new List<string[]>();
        atElite = new List<string[]>();
        atLevel = new List<string[]>();
        link = new List<string>();
    }
}
public class DataHub_ST : MonoBehaviour
{
    public GameObject singleton;
    public GameObject globalstatic;

    public Texture2D[] eliteIcon;
    public List<StageList> stages;
    public List<OperaterClass> operate;

    List<StageList> event_stage_data;
    public void Initialized()
    {
        event_stage_data = new List<StageList>();
        stages = new List<StageList>();
        operate = new List<OperaterClass>();

        if (XML.This == null)
            GameObject.Instantiate(singleton);
        if (FirebaseDataBase.instance == null)
            GameObject.Instantiate(globalstatic);

        XML.This.Get_StageData(stages);
        XML.This.Get_OperatorInfo(operate);
        event_stage_data = XML.This.get_event_stage_data();

        for (int i = 0; i < operate.Count; i++) // 리소스 - 일러스트 + 아이콘 로딩
        {
            OperaterClass cur = operate[i];
            string path = string.Format("{0}/{1}", Files.Use.DocumentsPath("Resource/OperatorIcon"), cur.en_name);
            cur.icon = Files.Use.GetPNG(path, "Icon.png");
        }
    }
    public void OnDestroy()
    {
        for (int i = 0; i < operate.Count; i++)
        {
            Destroy(operate[0].icon);
            operate.RemoveAt(0);
        }
    }
    public Texture2D GetOperatorIcon(string name)
    {
        OperaterClass data = operate.Find(delegate (OperaterClass a)
        {
            return a.en_name == name;
        });
        if (data == null)
            return null;

        return data.icon;
    }
    public Texture2D GetEliteIcon(string set)
    {
        int index = int.Parse(set);
        return eliteIcon[index];
    }
    public List<StageList> get_event_stage_data()
    {
        return event_stage_data;
    }
}
