using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using MaterialData;
using UI;
public class InfoUI_IV : MonoBehaviour
{
    public DataHub_Material dataMng;
    public GameObject UI;

    [Header("- UI Components")]
    public RawImage frame;
    public RawImage icon;
    public Text nameTxt;
    public Text info;
    public Text[] count;    // [0] Need, [1] Inven, [2] Made, [3] Result;

    public Transform viewerM;
    public Transform viewerS;

    private List<RawIcon> iconM;
    private List<Text> iconS;
    private List<Image> iconSimg;

    public Material_IV curData;
    public void Initialized()
    {
        iconS = new List<Text>();
        iconSimg = new List<Image>();
        iconM = new List<RawIcon>();
        for (int i = 0; i < viewerS.childCount; i++)
        {
            Text tmp = viewerS.GetChild(i).GetChild(0).GetComponent<Text>();
            Image img = viewerS.GetChild(i).GetComponent<Image>();
            iconS.Add(tmp);
            iconSimg.Add(img);
        }
        for(int i = 0; i < viewerM.childCount; i++)
        {
            RawIcon tmp = new RawIcon();
            tmp.tempTxt = new Text[1];
            tmp.frame = viewerM.GetChild(i).GetChild(0).GetComponent<RawImage>();
            tmp.icon = viewerM.GetChild(i).GetChild(1).GetComponent<RawImage>();
            tmp.tempTxt[0] = viewerM.GetChild(i).GetChild(2).GetChild(0).GetComponent<Text>();
            iconM.Add(tmp);
        }
        UI.SetActive(false);
    }

    public void Refresh()
    {
        frame.texture = dataMng.GetMaterialFrame(curData.data.rare);
        icon.texture = curData.data.icon;
        nameTxt.text = curData.data.name;

        info.text = string.Format("<b>{0}</b>{1}{1}<i>{2}</i>", curData.data.info, System.Environment.NewLine, curData.data.record);
        count[0].text = curData.count.ToString();
        count[1].text = curData.inven.ToString();
        count[2].text = curData.made.ToString();
        if (curData.count - curData.inven - curData.made < 0)
            count[3].text = 0.ToString();
        else
            count[3].text = (curData.count - curData.inven - curData.made).ToString();

        for(int i = 0; i < iconM.Count; i++)
        {
            if(i >= curData.data.submaterial.Length || curData.data.submaterial[0].Equals(string.Empty))
            {
                iconM[i].icon.transform.parent.gameObject.SetActive(false);
                continue;
            }
            EliteMaterial tmp = dataMng.get_material_all(curData.data.submaterial[i]);
            if (tmp == null) continue;
            iconM[i].icon.texture = tmp.icon;
            iconM[i].frame.texture = dataMng.GetMaterialFrame(tmp.rare);
            iconM[i].tempTxt[0].text = curData.data.subCount[i];
            iconM[i].icon.transform.parent.gameObject.SetActive(true);
        }
        for(int i = 0; i < iconS.Count; i++)
        {
            if (i >= curData.data.dropstage.Length || curData.data.dropstage[0].Equals(string.Empty))
            {
                iconS[i].transform.parent.gameObject.SetActive(false);
                continue;
            }
            if (curData.data.dropPer[i].Equals("항상"))
                iconSimg[i].color = new Color(40 / 255f, 140 / 255f, 40 / 255f);
            else if (curData.data.dropPer[i].Equals("높음"))
                iconSimg[i].color = new Color(100 / 255f, 180 / 255f, 50 / 255f);
            else if (curData.data.dropPer[i].Equals("보통"))
                iconSimg[i].color = new Color(220 / 255f, 130 / 255f, 10 / 255f);
            else if (curData.data.dropPer[i].Equals("낮음"))
                iconSimg[i].color = new Color(200 / 255f, 50 / 255f, 30 / 255f);
            else if (curData.data.dropPer[i].Equals("매우 낮음"))
                iconSimg[i].color = new Color(0.5f, 0, 0);
            iconS[i].text = string.Format("{0}{1}{2}", curData.data.dropstage[i], System.Environment.NewLine, curData.data.dropPer[i]);
            iconS[i].transform.parent.gameObject.SetActive(true);
        }
    }
    public void TR_SetStart(Material_IV set)
    {
        UI.SetActive(true);

        curData = set;
        Refresh();
    }
    public void TR_Close()
    {
        curData = null;
        UI.SetActive(false);
    }
}
