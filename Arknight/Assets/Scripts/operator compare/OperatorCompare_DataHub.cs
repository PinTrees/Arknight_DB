using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CharactorDataSet;

public class OperatorCompare_DataHub : MonoBehaviour
{
    List<OperaterClass> operators = new List<OperaterClass>();
    // setting recorsce = none;
    // setting value = frame_colors + ;
    Color[] rare_colors = new Color[6];
    public void Initiailized()
    {
        // setting value = frame_colors;
        rare_colors[5] = new Color(1, 100 / 255f, 0);
        rare_colors[4] = new Color(1, 150 / 255f, 0);
        rare_colors[3] = new Color(200 / 255f, 100 / 255f, 1);
        rare_colors[2] = new Color(100 / 255f, 150 / 255f, 1);
        rare_colors[1] = new Color(150 / 255f, 1, 100 / 255f);
        rare_colors[0] = new Color(150 / 255f, 150 / 255f, 150 / 255f);
        // database leveldata[default + elite1 + elite2];
        XML.This.Get_OperatorInfo(operators);
        XML.This.Get_Operater_Status(operators);
        // recorsce icon;
        for (int i = 0; i < operators.Count; i++)
        {
            string path = string.Format("{0}/{1}", Files.Use.DocumentsPath("Resource/OperatorIcon"), operators[i].en_name);
            operators[i].icon = Files.Use.GetPNG(path, "Icon.png");
        }
    }
    public List<OperaterClass> GetOperatorClass()
    {
        return operators;
    }
    public OperaterClass GetOperator(string code)
    {
        for (int i = 0; i < operators.Count; i++)
        {
            if (code == operators[i].en_name)
                return operators[i];
        }
        return null;
    }
    public OperaterClass GetOperator(int index)
    {
        return operators[index];
    }

    public Color GetRareColor(string rare)
    {
        int index = int.Parse(rare);
        return rare_colors[index - 1];
    }
}
