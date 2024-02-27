using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DataClass;
public class DataHub_EM : MonoBehaviour
{
    public GameObject singletone;
    public List<Enemy> data;

    public void Initialized()
    {
        if (XML.This == null)
            GameObject.Instantiate(singletone);

        data = new List<Enemy>();

        XML.This.Get_EnemyData(data);

        for(int i = 0; i < data.Count; i++)
        {
            string path = Files.Use.DocumentsPath("Resource/EnemyIcon");
            data[i].icon = Files.Use.GetPNG(path, data[i].code + ".png");
            //if (data[i].icon == null)
            //    Debug.Log(path + "/" + data[i].code + ".png");
        }
    }
}
