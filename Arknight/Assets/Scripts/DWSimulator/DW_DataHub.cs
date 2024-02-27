using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CharactorDataSet;
public class EventOffer
{
    public string fileName;
    public string date;
    public string endData;

    public int[] per;
    public string[] rare6_up;
    public string[] rare5_up;

    public string[] rare6;
    public string[] rare5;
    public string[] rare4;
    public string[] rare3;
}
public class DW_DataHub : MonoBehaviour
{
    [Header("- UI Static Object")]
    public Texture2D[] rareIcon;

    public List<OperaterClass> operatorData;
    public List<EventOffer> eventData;

    Color[] rare_colors = new Color[6];

    public int stack;
    public void Initialized()
    {
        stack = 0;
        rare_colors[5] = new Color(1, 100 / 255f, 0);
        rare_colors[4] = new Color(1, 150 / 255f, 0);
        rare_colors[3] = new Color(200 / 255f, 100 / 255f, 1);
        rare_colors[2] = new Color(100 / 255f, 150 / 255f, 1);
        rare_colors[1] = new Color(150 / 255f, 1, 100 / 255f);
        rare_colors[0] = new Color(150 / 255f, 150 / 255f, 150 / 255f);

        eventData = new List<EventOffer>();
        operatorData = new List<OperaterClass>();

        XML.This.Get_OperatorInfo(operatorData);
        XML.This.Get_EventData(eventData);

        for (int i = 0; i < operatorData.Count; i++) // 리소스 - 일러스트 + 아이콘 로딩
        {
            OperaterClass cur = operatorData[i];
            string path = string.Format("{0}/{1}", Files.Use.DocumentsPath("Resource/OperatorIcon"), cur.en_name);
            cur.icon = Files.Use.GetPNG(path, "Icon.png");
        }
    }
    private void OnDestroy()
    {
        for (int i = 0; i < operatorData.Count; i++)
            operatorData.RemoveAt(0);
    }
    public Texture2D GetRareTexture(string rare)
    {
        return rareIcon[int.Parse(rare) - 1];
    }
    public Color GetRareColor(string rare)
    {
        int index = int.Parse(rare);
        return rare_colors[index - 1];
    }

    public OperaterClass GetRandomOperator(EventOffer data)
    {
        int add_per;
        if (stack >= 50)
        {
            int tmp = (stack - 50) / 10;
            add_per = ((tmp + 1) * 2);
        }
        else
            add_per = 0;

        float rare = Random.Range(0, 100);
        if (rare < data.per[0] + add_per)
        {
            stack = 0;
            if(data.rare6[0].Equals(string.Empty))// Up
            {
                int ch = Random.Range(0, data.rare6_up.Length);
                OperaterClass tmp = operatorData.Find(delegate (OperaterClass a) { return a.name == data.rare6_up[ch]; });
                return tmp;
            }
            else if (Random.Range(0, 2).Equals(0)) // Up
            {
                int ch = Random.Range(0, data.rare6_up.Length);
                OperaterClass tmp = operatorData.Find(delegate (OperaterClass a) { return a.name == data.rare6_up[ch]; });
                return tmp;
            }
            else
            {
                int ch = Random.Range(0, data.rare6.Length);
                OperaterClass tmp = operatorData.Find(delegate (OperaterClass a) { return a.name == data.rare6[ch]; });
                return tmp;
            }
        }

        stack++;
        rare = Random.Range(data.per[0] + add_per, 100);
        if (rare < add_per + data.per[0] + data.per[1])
        {
            if (data.rare5[0].Equals(string.Empty)) // Up
            {
                int ch = Random.Range(0, data.rare5_up.Length);
                OperaterClass tmp = operatorData.Find(delegate (OperaterClass a) { return a.name == data.rare5_up[ch]; });
                return tmp;
            }
            else if (Random.Range(0, 2).Equals(0)) // Up
            {
                int ch = Random.Range(0, data.rare5_up.Length);
                OperaterClass tmp = operatorData.Find(delegate (OperaterClass a) { return a.name == data.rare5_up[ch]; });
                return tmp;
            }
            else
            {
                int ch = Random.Range(0, data.rare5.Length);
                OperaterClass tmp = operatorData.Find(delegate (OperaterClass a) { return a.name == data.rare5[ch]; });
                return tmp;
            }
        }
        else if(rare < add_per + data.per[0] + data.per[1] + data.per[2])
        {
            int ch = Random.Range(0, data.rare4.Length);
            OperaterClass tmp = operatorData.Find(delegate (OperaterClass a) { return a.name == data.rare4[ch]; });
            return tmp;
        }
        else if(rare < add_per + data.per[0] + data.per[1] + data.per[2] + data.per[3])
        {
            int ch = Random.Range(0, data.rare3.Length);
            OperaterClass tmp = operatorData.Find(delegate (OperaterClass a) { return a.name == data.rare3[ch]; });
            return tmp;
        }
        return null;
    }
    public OperaterClass GetOperatironData(string name)
    {
        for (int i = 0; i < operatorData.Count; i++)
        {
            if (name == operatorData[i].name)
            {
                return operatorData[i];
            }
        }
        return null;
    }

}
