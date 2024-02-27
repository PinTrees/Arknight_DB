using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
public class LC_CalculatorMenu_UIManager : MonoBehaviour
{
    [Header("- Static UI Components")]
    public Sprite[] eliteSptire = new Sprite[3];
    public Sprite[] rareSprite = new Sprite[5];

    [Header("- UI GameObject")]
    public GameObject expmade;

    [Header("- UI Components")]
    public InputField[] levelinput = new InputField[2];
    public Slider[] levelSlider = new Slider[2];
    public Text[] material = new Text[4];
    public Text[] expTxt = new Text[1];
    public Text exp_need;
    public Text cost_need;
    public Text getexp;
    public Text getcost;
    public Text expmental;
    public Text costmental;
    public Text getexp_cnt;
    public Text getcost_cnt;
    public Text[] levelValue = new Text[2];
    public Image rareBtn;
    public Image[] eliteBtn = new Image[2];

    private int[] material_cnt = new int[4];
    private int curRare;
    private int[] curLV = new int[2];
    private int[] elite_inx = new int[2];
    private void Start()
    {
        Initialized();
        RefreshUpdate();
    }
    public void Initialized()
    {
        curRare = 6;

        curLV[0] = 1;
        curLV[1] = 2;
        levelSlider[0].value = curLV[0]; levelSlider[1].value = curLV[1];

        elite_inx[0] = 0;
        elite_inx[1] = 0;

        material_cnt[0] = 0;
        material_cnt[1] = 0;
        material_cnt[2] = 0;
        material_cnt[3] = 0;
    }
    public void TR_SetRare()
    {
        curRare--;
        if (curRare < 2)
            curRare = 6;
        rareBtn.sprite = rareSprite[curRare - 2];

        curLV[0] = 1;
        curLV[1] = 2;

        RefreshUpdate();
    }
    public void TR_SetEilte(int where)
    {
        elite_inx[where]++;
        if (elite_inx[where] > 2)
            elite_inx[where] = 0;
        if (where.Equals(0) && elite_inx[1] < elite_inx[0])
            elite_inx[1] = elite_inx[0];

        if (where.Equals(1))
            curLV[1] = 2;
        RefreshUpdate();
    }
    public void TR_SetLevel(int where)
    {
        curLV[where] = (int)levelSlider[where].value;
        RefreshUpdate();
    }
    public void TR_InputLevel(int where)
    {
        curLV[where] = int.Parse(levelinput[where].text);
        levelinput[where].text = string.Empty;

        if (curLV[where] > levelSlider[where].maxValue)
            curLV[where] = (int)levelSlider[where].maxValue;
        else if(curLV[where] < levelSlider[where].minValue)
            curLV[where] = (int)levelSlider[where].minValue;

        RefreshUpdate();
    }
    public void TR_SetMaterialCount(InputField set)
    {
        int where = set.transform.parent.GetSiblingIndex();
        int ob = int.Parse(set.text);
        if (ob < 0)
            material_cnt[where] = 0;
        else material_cnt[where] = ob;

        set.text = string.Empty;

        RefreshUpdate();
    }
    public void RefreshUpdate()
    {
        RefreshEliteSprite();
        RefreshLevel();
        
        int[] set = new int[2];
        set[0] = elite_inx[0]; 
        set[1] = curLV[0];
        
        int[] target = new int[2];
        target[0] = elite_inx[1];
        target[1] = curLV[1];
        ExpCost get = new ExpCost();
        DataHub_Level.instance.GetExpCostData(curRare, set, target, get);

        int madeExp = 0;
        madeExp += material_cnt[0] * 200;
        madeExp += material_cnt[1] * 400;
        madeExp += material_cnt[2] * 1000;
        madeExp += material_cnt[3] * 2000;

        int dropExp = 0; 
        if (!(get.exp <= madeExp))
        {
            dropExp = ((get.exp - madeExp) / 7400);
            if (!((get.exp - madeExp) % 7400).Equals(0))
                dropExp++;
        }
        int needCost = get.cost - dropExp * 360;
        if (needCost < 0) 
            needCost = 0;
        int dropCost = (needCost / 7500);
        if(needCost.Equals(0))
            dropCost = 0;
        else if (!(needCost % 7500).Equals(0))
            dropCost++;

        getexp.text = (dropExp * 7400).ToString();
        expmental.text = (dropExp * 30).ToString();
        getexp_cnt.text = string.Format("{0}{1}", "X ", dropExp);

        getcost.text = string.Format("{0}{1}{2}", dropCost * 7500, "+", dropExp * 360);
        costmental.text = (dropCost * 30).ToString();
        getcost_cnt.text = string.Format("{0}{1}", "X ", dropCost);

        exp_need.text = get.exp.ToString();
        cost_need.text = get.cost.ToString(); 

        if (!madeExp.Equals(0))
        {
            expmade.SetActive(true);
            expTxt[0].text = madeExp.ToString();
        }
        else expmade.SetActive(false);

        for (int i = 0; i < material.Length; i++)
            material[i].text = material_cnt[i].ToString();
    }
    public void RefreshEliteSprite()
    {
        if (elite_inx[1] < elite_inx[0])
            elite_inx[1] = elite_inx[0];
        if (curRare.Equals(6)) ;
        else if (curRare.Equals(5)) ;
        else if (curRare.Equals(4)) ;
        else if (curRare.Equals(3))
        {
            if (elite_inx[0].Equals(2))
                elite_inx[0] = 0;
            if (elite_inx[1].Equals(2))
                elite_inx[1] = 0;
        }
        else if (curRare.Equals(2))
        {
            if (!elite_inx[0].Equals(0))
                elite_inx[0] = 0;
            if (!elite_inx[1].Equals(0))
                elite_inx[1] = 0;
        }
        eliteBtn[0].sprite = eliteSptire[elite_inx[0]];
        eliteBtn[1].sprite = eliteSptire[elite_inx[1]];
    }
    public void RefreshLevel()
    {
        levelSlider[1].minValue = 1;
        if (curRare.Equals(6))
        {
            if (elite_inx[0].Equals(2))
                levelSlider[0].maxValue = 90 - 1;
            else if (elite_inx[0].Equals(1))
                levelSlider[0].maxValue = 80 - 1;
            else if (elite_inx[0].Equals(0))
                levelSlider[0].maxValue = 50 - 1;

            if (elite_inx[1].Equals(2))
                levelSlider[1].maxValue = 90;
            else if (elite_inx[1].Equals(1))
                levelSlider[1].maxValue = 80;
            else if (elite_inx[1].Equals(0))
                levelSlider[1].maxValue = 50;
        }
        else if (curRare.Equals(5))
        {
            if (elite_inx[0].Equals(2))
                levelSlider[0].maxValue = 80 - 1;
            else if (elite_inx[0].Equals(1))
                levelSlider[0].maxValue = 70 - 1;
            else if (elite_inx[0].Equals(0))
                levelSlider[0].maxValue = 50 - 1;

            if (elite_inx[1].Equals(2))
                levelSlider[1].maxValue = 80;
            else if (elite_inx[1].Equals(1))
                levelSlider[1].maxValue = 70;
            else if (elite_inx[1].Equals(0))
                levelSlider[1].maxValue = 50;
        }
        else if (curRare.Equals(4))
        {
            if (elite_inx[0].Equals(2))
                levelSlider[0].maxValue = 70 - 1;
            else if (elite_inx[0].Equals(1))
                levelSlider[0].maxValue = 60 - 1;
            else if (elite_inx[0].Equals(0))
                levelSlider[0].maxValue = 45 - 1;

            if (elite_inx[1].Equals(2))
                levelSlider[1].maxValue = 70;
            else if (elite_inx[1].Equals(1))
                levelSlider[1].maxValue = 60;
            else if (elite_inx[1].Equals(0))
                levelSlider[1].maxValue = 45;
        }
        else if (curRare.Equals(3))
        {
            if (elite_inx[0].Equals(1))
                levelSlider[0].maxValue = 55 - 1;
            else if (elite_inx[0].Equals(0))
                levelSlider[0].maxValue = 40 - 1;

            if (elite_inx[1].Equals(1))
                levelSlider[1].maxValue = 55;
            else if (elite_inx[1].Equals(0))
                levelSlider[1].maxValue = 40;
        }
        else if (curRare.Equals(2))
        {
            if (elite_inx[0].Equals(0))
                levelSlider[0].maxValue = 30 - 1;

            if (elite_inx[1].Equals(0))
                levelSlider[1].maxValue = 30;
        }
        if (elite_inx[0].Equals(elite_inx[1]))
        {
            levelSlider[1].minValue = curLV[0] + 1;
            if (curLV[1] <= curLV[0])
            {
                curLV[1] = curLV[0] + 1;
                levelSlider[1].value = curLV[1];
            }
        }

        levelSlider[0].value = curLV[0];
        levelSlider[1].value = curLV[1];
        levelValue[0].text = curLV[0].ToString();
        levelValue[1].text = curLV[1].ToString();
    }
}
