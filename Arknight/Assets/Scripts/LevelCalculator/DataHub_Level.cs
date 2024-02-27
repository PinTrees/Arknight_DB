using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Xml;
public class ExpCost
{
    public int exp;
    public int cost;
}
public class LevelData
{
    public List<ExpCost[]> data;
    public LevelData()
    {
        data = new List<ExpCost[]>();
    }
}
public class DataHub_Level : MonoBehaviour
{
    public LevelData levelData;

    public static DataHub_Level instance;
    public void Awake()
    {
        instance = this;

        Initialized();
    }
    public void Initialized()
    {
        levelData = new LevelData();
        StartCoroutine(Load_OperatorLevelData_XML(levelData, "OFF"));
    }
    public void GetExpCostData(int rare, int[] set, int[] target, ExpCost get)
    {
        if (set[0].Equals(target[0]))
        {
            int elite = target[0];
            int exp = 0; int cost = 0;
            for (int i = set[1] - 1; i < target[1] - 1; i++)
            {
                exp = exp + levelData.data[elite][i].exp;
                cost = cost + levelData.data[elite][i].cost;
            }
            get.exp = exp;
            get.cost = cost;
        }
        else
        {
            int exp = 0; int cost = 0;
            int start = set[0];

            while (start <= target[0])
            {
                int maxlevel = 1; 
                int startlevel = 0;
                
                if (start.Equals(0))
                {
                    if (rare.Equals(2))
                        maxlevel = 30 - 1;
                    else if (rare.Equals(3))
                        maxlevel = 40 - 1;
                    else if (rare.Equals(4))
                        maxlevel = 45 - 1;
                    else if (rare.Equals(5))
                        maxlevel = 50 - 1;
                    else if (rare.Equals(6))
                        maxlevel = 50 - 1;
                }
                else if (start.Equals(1))
                {
                    if (rare.Equals(3))
                        maxlevel = 55 - 1;
                    else if (rare.Equals(4))
                        maxlevel = 60 - 1;
                    else if (rare.Equals(5))
                        maxlevel = 70 - 1;
                    else if (rare.Equals(6))
                        maxlevel = 80 - 1;
                }

                if (start.Equals(target[0]))
                    maxlevel = target[1] - 1;
                if (start.Equals(set[0]))
                    startlevel = set[1] - 1;
                else startlevel = 0;

                for (int i = startlevel; i < maxlevel; i++)
                {
                    exp = exp + levelData.data[start][i].exp;
                    cost = cost + levelData.data[start][i].cost;
                }
                start++;
            }
            get.exp = exp;
            get.cost = cost;
        }
    }
    public IEnumerator Load_OperatorLevelData_XML(LevelData set, string where = "OFF")
    {
        if (where.Equals("OFF"))
        {
            TextAsset txtAsset = (TextAsset)Resources.Load("XML/LevelData/" + "Default_level");
            //Debug.Log(txtAsset.text);

            XmlDocument xmlDoc = new XmlDocument();
            // XML 로드하고.
            xmlDoc.LoadXml(txtAsset.text);
            XmlNodeList nodes = xmlDoc.SelectNodes("LevelList/DefaulteInfo");

            ExpCost[] _default = new ExpCost[49];
            for (int i = 0; i < nodes[0].ChildNodes.Count; i++)
            {
                XmlElement now = (XmlElement)nodes[0].ChildNodes[i];
                ExpCost _new = new ExpCost();
                _new.exp = int.Parse(now.GetAttribute("exp"));
                _new.cost = int.Parse(now.GetAttribute("cost"));
                _default[i] = _new;
            }
            set.data.Add(_default);

            ExpCost[] elite1 = new ExpCost[79];
            for (int i = 0; i < nodes[1].ChildNodes.Count; i++)
            {
                XmlElement now = (XmlElement)nodes[1].ChildNodes[i];
                ExpCost _new = new ExpCost();
                _new.exp = int.Parse(now.GetAttribute("exp"));
                _new.cost = int.Parse(now.GetAttribute("cost"));
                elite1[i] = _new;
            }
            set.data.Add(elite1);
            //for (int i = 0; i < set.elite1.Length; i++)
            //    Debug.Log("exp:" + set.elite1[i].exp + ", cost:" + set.elite1[i].cost);

            ExpCost[] elite2 = new ExpCost[89];
            for (int i = 0; i < nodes[2].ChildNodes.Count; i++)
            {
                XmlElement now = (XmlElement)nodes[2].ChildNodes[i];
                ExpCost _new = new ExpCost();
                _new.exp = int.Parse(now.GetAttribute("exp"));
                _new.cost = int.Parse(now.GetAttribute("cost"));
                elite2[i] = _new;
            }
            set.data.Add(elite2);
            //for (int i = 0; i < set.elite2.Length; i++)
            //    Debug.Log("exp:" + set.elite2[i].exp + ", cost:" + set.elite2[i].cost);
        }
        yield return null;
    }
}
