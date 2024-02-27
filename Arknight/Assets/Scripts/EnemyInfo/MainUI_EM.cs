using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
using DataClass;
using UnityEngine.UI;
using UI;
public class MainUI_EM : MonoBehaviour
{
    [Header("- Data Scripts")]
    public DataHub_EM dataHub;

    [Header("- UI Components Icon Viewer")]
    public Transform contens;
    private GameObject[] parents;
    private RawImage[] icons;
    private Text[] namecode;

    [Header("- UI Components StatusPanel")]
    public GameObject[] statusUI;
    public RawImage icon;
    public Text name_txt;
    public Text subStatus;
    public Text resise;
    public Text info;
    public GameObject skillpanel;
    public Text skill;
    public Text code;
    public Text[] rank;
    public Text[] status;

    CanvasScaler canvasScreen;
    RectTransform rect;
    public void setCanvasScale()
    {
        canvasScreen = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<CanvasScaler>();
        rect = canvasScreen.transform.GetChild(0).GetComponent<RectTransform>();
        CanvasScreen androidProgram = new CanvasScreen();
        androidProgram.setCanvasScreen(XML.This, canvasScreen, rect);
    }
    public void Initialized()
    {
        parents = new GameObject[contens.childCount];
        icons = new RawImage[contens.childCount];
        namecode = new Text[contens.childCount];

        for (int i = 0; i < contens.childCount; i++)
        {
            parents[i] = contens.GetChild(i).gameObject;
            icons[i] = parents[i].transform.GetChild(2).GetComponent<RawImage>();
            namecode[i] = parents[i].transform.GetChild(4).GetComponent<Text>();
        }

        Clear();
        CloseStatusUI();
        Refresh("START");
    }
    public void Clear()
    {
        for (int i = 0; i < parents.Length; i++)
            parents[i].SetActive(false);
    }
    public void Refresh(string type, List<Enemy> set = null)
    {
        List<Enemy> data;
        if (type.Equals("START"))
            data = dataHub.data;
        else
            data = set;

        for (int i = 0; i < data.Count; i++)
        {
            if (parents.Length <= i)
                break;

            if (data[i].icon == null)
                icons[i].enabled = false;
            else
            {
                icons[i].texture = data[i].icon;
                icons[i].enabled = true;
            }
            namecode[i].text = data[i].code;
            parents[i].SetActive(true);
        }
    }
    public void SetStatusUI(Transform set)
    {
        StartCoroutine(Refresh_StatusUI(set.GetSiblingIndex()));
    }
    private IEnumerator Refresh_StatusUI(int index)
    {
        statusUI[1].SetActive(true);
        yield return new WaitForSeconds(0.01f);
        while (!statusUI[1].activeSelf)
        {
            yield return new WaitForSeconds(0.01f);
            statusUI[1].SetActive(true);
        }

        Enemy data = dataHub.data[index];
        if (data == null)
        {
            yield return null;
            statusUI[1].SetActive(false);
        }

        icon.texture = data.icon;
        name_txt.text = data.name;
        code.text = data.code;

        subStatus.text = string.Format("{0}공격 속도</color>  {1}초{2}{0}이동 속도</color>  {3}{2}{0}사정 거리</color>  {4}타일{2}{0}유닛 무게</color>  {5}",
    "<color=#A6A6A6>", data.atktime, System.Environment.NewLine, data.movespeed, data.rang, data.weight);

        resise.text = string.Empty;
        if (data.resis[0].Equals("T"))
            resise.text = string.Format("충격 면역{0}",System.Environment.NewLine);
        if (data.resis[1].Equals("T"))
            resise.text = string.Format("{0}침묵 면역{1}", resise.text, System.Environment.NewLine);

        info.text = string.Format("<color=#A6A6A6>공격 타입</color>  {0}{1}{1}{2}", data.position, System.Environment.NewLine, data.info);

        rank[0].text = data.hp[0];
        rank[1].text = data.atk[0];
        rank[2].text = data.def[0];
        rank[3].text = data.mdef[0];
        status[0].text = string.Format("<color=#A6A6A6>체력</color>  {0}", data.hp[1]);
        status[1].text = string.Format("<color=#A6A6A6>공격력</color>  {0}", data.atk[1]);
        status[2].text = string.Format("<color=#A6A6A6>방어력</color>  {0}", data.def[1]);
        status[3].text = string.Format("<color=#A6A6A6>마법 저항</color>  {0}", data.mdef[1]);

        if (!data.skill.Equals(string.Empty))
        {
            skill.text = data.skill;
            skillpanel.SetActive(true);
        }
        statusUI[0].SetActive(true);
        yield return new WaitForEndOfFrame();

        statusUI[1].SetActive(false);
    }
    public void CloseStatusUI()
    {
        icon.texture = null;
        name_txt.text = string.Empty;
        code.text = string.Empty;

        subStatus.text = string.Empty;
        resise.text = string.Empty;

        info.text = string.Empty;
        skill.text = string.Empty;

        rank[0].text = string.Empty;
        rank[1].text = string.Empty;
        rank[2].text = string.Empty;
        rank[3].text = string.Empty;
        status[0].text = string.Empty;
        status[1].text = string.Empty;
        status[2].text = string.Empty;
        status[3].text = string.Empty;

        statusUI[0].SetActive(false);
        skillpanel.SetActive(false);
        statusUI[1].SetActive(false);
    }
    public void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (statusUI[0].activeSelf)
                CloseStatusUI();
            else
                SceneManager.LoadScene("Main");
        }
    }
}
