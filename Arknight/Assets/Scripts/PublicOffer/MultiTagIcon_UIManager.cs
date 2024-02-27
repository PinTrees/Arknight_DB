using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.UI;
using CharactorDataSet;
using UI;
public class PublicOfferIcon
{
    public GameObject This;
    public Image icon;
    public RawImage rawicon;
    public Image fram;
    public Text name;
    public PublicOfferIcon() { }
    public PublicOfferIcon(Image _icon, Image _fram, Text _name)
    {
        icon = _icon;
        fram = _fram;
        name = _name;
    }
}

public class OperatorIcon
{
    public GameObject This;
    public Image icon;
    public Image frame;
    public Text name;
}
public class TagUI
{
    public GameObject _this;
    public Text name;
}
public class RareCountUI
{
    public GameObject _this;
    public Text count;
}
public class PO_ResultUI
{
    public GameObject This;
    public TagUI[] tagUI;
    public RareCountUI[] rareUI;
    public TagListm tagData;
    public RawImage RareTag;
    public List<PO_OperatorClass> charactorData;
}

public class MultiTagIcon_UIManager : MonoBehaviour
{
    public MultiTagBtn_UIManager MenuUI;

    [Header("- UI Static Data")]
    public Texture2D[] rareTag;

    [Header("- Manager Scripts")]
    public CharactorData dataMng;

    [Header("- UI GameObject")]
    public GameObject This;
    public GameObject iconViewer;
    public GameObject dropshadow;

    [Header("- UI Transform")]
    public Transform listViewer;
    public Transform icon_tr;

    private List<PO_ResultUI> resUI;
    private List<OperatorIcon> icons;

    private int currselect_idx;
    private int currindex;
    private Image curr_btn;

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
        currindex = 0;
        resUI = new List<PO_ResultUI>();
        for(int i = 0; i < listViewer.childCount - 2; i++)
        {
            PO_ResultUI _new = new PO_ResultUI();
            _new.This = listViewer.GetChild(i).gameObject;
            _new.tagUI = new TagUI[3];
            for(int j = 0; j < _new.tagUI.Length; j++)
            {
                Transform now = listViewer.GetChild(i).GetChild(0).GetChild(0).transform;
                _new.tagUI[j] = new TagUI();
                _new.tagUI[j]._this = now.GetChild(j).gameObject;
                _new.tagUI[j].name = now.GetChild(j).GetChild(0).GetComponent<Text>();
            }
            _new.rareUI = new RareCountUI[5];
            for (int j = 0; j < _new.rareUI.Length; j++)
            {
                Transform now = listViewer.GetChild(i).GetChild(1).GetChild(0).transform;
                _new.rareUI[j] = new RareCountUI();
                _new.rareUI[j]._this = now.GetChild(4 - j).gameObject;
                _new.rareUI[j].count = now.GetChild(4 - j).GetChild(0).GetComponent<Text>();
            }

            _new.RareTag = listViewer.GetChild(i).GetChild(3).GetComponent<RawImage>();
            resUI.Add(_new);
        }
        icons = new List<OperatorIcon>();
        for (int i = 0; i < icon_tr.childCount; i++)
        {
            OperatorIcon _new = new OperatorIcon();
            _new.This = icon_tr.GetChild(i).gameObject;
            _new.icon = icon_tr.GetChild(i).GetComponent<Image>();
            _new.frame = icon_tr.GetChild(i).GetChild(0).GetComponent<Image>();
            _new.name = icon_tr.GetChild(i).GetChild(1).GetComponent<Text>();

            icons.Add(_new);
        }
        SetClear();
        SetActive(true);
    }
    public void SetClear()
    {
        for(int i = 0; i < resUI.Count; i++)
        {
            for(int j = 0; j < resUI[i].tagUI.Length; j++)
                resUI[i].tagUI[j]._this.SetActive(false);

            for (int j = 0; j < resUI[i].rareUI.Length; j++)
                resUI[i].rareUI[j]._this.SetActive(false);

            resUI[i].RareTag.gameObject.SetActive(false);
            resUI[i].This.SetActive(false);
        }
        SetClearIcon();
        
        if(curr_btn != null)
        {
            curr_btn.color = new Color(200 / 255f, 200 / 255f, 200 / 255f);
            //curr_btn.transform.GetChild(0).GetComponent<Text>().color = Color.black;
        }

        iconViewer.SetActive(false);
        dropshadow.SetActive(false);
        currselect_idx = -1;
    }
    public void SetClearIcon()
    {
        for (int i = 0; i < icons.Count; i++)
            icons[i].This.SetActive(false);
    }
    public void SetActive(bool set)
    {
        This.SetActive(set);
    }
    public void SetListData(TagListm tags, List<PO_OperatorClass> charactor, int index)
    {
        if (index == -1)
        {
            currindex = 0;
            return;
        }
        resUI[index].tagData = tags;
        resUI[index].charactorData = charactor;
        currindex = index + 1;
     }
    public void Refresh()
    {
        SetClear();
        for (int i = 0; i < currindex; i++)
        {
            for(int j = 0; j < resUI[i].tagData.data.Length; j++)
            {
                resUI[i].tagUI[j].name.text = ConvertTagName(resUI[i].tagData.data[j].name);
                resUI[i].tagUI[j]._this.SetActive(true);
            }

            resUI[i].RareTag.gameObject.SetActive(false);

            bool flag = false;
            for (int j = 0; j < resUI[i].rareUI.Length; j++)
            {
                int count = GetRareCount(resUI[i].charactorData, (j + 2).ToString()); // 2성
                if (count > 0)
                {
                    resUI[i].rareUI[j].count.text = count.ToString();
                    resUI[i].rareUI[j]._this.SetActive(true);

                    if (flag.Equals(false) && j >= 2)
                    {
                        resUI[i].RareTag.texture = rareTag[j - 2];
                        resUI[i].RareTag.gameObject.SetActive(true);
                    }
                    flag = true;
                }
            }
            resUI[i].This.SetActive(true);
        }
    }
    public int GetRareCount(List<PO_OperatorClass> target, string rare)
    {
        int count = target.Count((PO_OperatorClass a) => a.rare == rare);
        return count;
    }
    public void TR_SelectIcon(Transform set)
    {
        int index = set.GetSiblingIndex();
        PO_OperatorClass tmp = resUI[currselect_idx].charactorData[index];

        MenuUI.RefreshSelectTag(tmp);
    }
    public void Trigger_SetIcons(Image target)
    {
        iconViewer.SetActive(false);
        dropshadow.SetActive(false);
        iconViewer.transform.SetAsLastSibling();
        dropshadow.transform.SetAsLastSibling();

        if (curr_btn == target)
        {
            curr_btn.color = new Color(200 / 255.0f, 200 / 255.0f, 200 / 255.0f);
            //curr_btn.transform.GetChild(0).GetComponent<Text>().color = Color.black;
            curr_btn = null;
        }
        else
        {
            target.color = Color.black;
            //target.transform.GetChild(0).GetComponent<Text>().color = Color.white;
            if (curr_btn != null)
            {
                curr_btn.color = new Color(200 / 255.0f, 200 / 255.0f, 200 / 255.0f);
                //curr_btn.transform.GetChild(0).GetComponent<Text>().color = Color.black;
            }
            curr_btn = target;
        }

        int index = target.transform.parent.GetSiblingIndex();
        if (currselect_idx == index)
        {
            currselect_idx = -1;
            SetClearIcon();
            iconViewer.SetActive(false);
            return;
        }
        currselect_idx = index;

        SetClearIcon();
        for (int i = 0; i < resUI[currselect_idx].charactorData.Count; i++)
        {
            PO_OperatorClass now = resUI[currselect_idx].charactorData[i];
            //Debug.Log(i + ", " + now.name + ", " + now.rare);
            icons[i].frame.sprite = dataMng.GetOperatorFrame(now.rare);
            icons[i].icon.sprite = now.icon;
            icons[i].name.text = now.name;
            icons[i].This.SetActive(true);
        }

        iconViewer.transform.SetSiblingIndex(currselect_idx + 1);
        dropshadow.transform.SetSiblingIndex(currselect_idx);
        iconViewer.SetActive(true);
        dropshadow.SetActive(true);
    }
    private string ConvertTagName(string name)
    {
        if (name == "신입") return "신입";
        else if(name == "베테랑") return "특별채용";
        else if(name == "전문가") return "고급특채";

        else if (name == "근거리") return "근거리";
        else if (name == "원거리") return "원거리";

        else if (name == "여성") return "여성대원";
        else if (name == "남성") return "남성대원";

        else if(name == "선봉") return "뱅가드";
        else if (name == "저격") return "스나이퍼";
        else if (name == "의료") return "메딕";
        else if (name == "술사") return "캐스터";
        else if (name == "근위") return "가드";
        else if (name == "중장") return "디펜더";
        else if (name == "보조") return "서포터";
        else if (name == "특수") return "스페셜리스트";

        else if (name == "치료") return "힐링";
        else if (name == "지원") return name;
        else if (name == "딜링") return "딜러";
        else if (name == "광역") return "범위공격";
        else if (name == "슬로우") return "감속";
        else if (name == "생존") return "생존형";
        else if (name == "보호") return "방어형";
        else if (name == "약화") return "디버프";
        else if (name == "변위") return "강제이동";
        else if (name == "의료") return "메딕";
        else if (name == "군중제어") return "제어형";
        else if (name == "폭발") return "누커";
        else if (name == "소환") return "소환";
        else if (name == "쾌속부활") return "쾌속부활";
        else if (name == "비용회복") return "코스트+";
        else if (name.Equals("로봇")) return name;
        return "알수없음";
    }
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            SceneManager.LoadScene("Main");
        }
    }
}
