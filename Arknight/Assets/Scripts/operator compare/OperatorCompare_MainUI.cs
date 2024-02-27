using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using CharactorDataSet;
using UI;
using TMPro;

class OperatorCompare_list
{
    public OperaterClass data;
    public GameObject _this;
    public RawImage icon;

    public Image[] status_bar;
    public Image frame;

    public Text status;
    public Text trustValue;
    public Text dpsText;
    public Text[] status_value;
    public Text name;
    public Text level;

    float atkDelay;
    string[] trust;
    public OperatorCompare_list()
    {
        status_bar = new Image[4];
        status_value = new Text[3];
    }
    public void RefreshGraphic(Texture2D _icon, Color framecolor, string _name)
    {
        icon.texture = _icon;
        frame.color = framecolor;
        name.text = _name;
        if (!_this.activeSelf)
            _this.SetActive(true);
    }
    public void Refresh_Static(float atktime, int cost, int block, string[] _trust = null)
    {
        atkDelay = atktime;
        trust = _trust;
        status_value[0].text = string.Format("공격속도  {0}초", atktime.ToString());
        status_value[1].text = string.Format("코스트 {0}", cost.ToString());
        status_value[2].text = string.Format("저지 {0}", block.ToString());
    }
    public void Refresh(int _level, int[] max, int hp, int atk, int def, int mdef, string[] _trust = null, float _atkSpeed = 0f)
    {
        level.text = string.Format("Lv {0}", _level.ToString());
        status.text = string.Format("{1}{0}{2}{0}{3}{0}{4}", System.Environment.NewLine,  hp.ToString(), atk.ToString(), def.ToString(), mdef.ToString());
        status_bar[0].fillAmount = hp / (float)max[0];
        status_bar[1].fillAmount = atk / (float)max[1];
        status_bar[2].fillAmount = def / (float)max[2];
        status_bar[3].fillAmount = mdef / (float)max[3];

        if(_trust != null)
        {
            trustValue.text = string.Empty;
            if (!_trust[0].Equals("0"))
                trustValue.text += "+" +  _trust[0];
            trustValue.text += System.Environment.NewLine;
            if (!_trust[1].Equals("0"))
                trustValue.text += "+" + _trust[1];
            trustValue.text += System.Environment.NewLine;
            if (!_trust[2].Equals("0"))
                trustValue.text += "+" + _trust[2];
            trustValue.text += System.Environment.NewLine;
            if (!_trust[3].Equals("0"))
                trustValue.text += "+" + _trust[3];
        }
        int curAtk = atk;
        if (trust != null)
            curAtk += int.Parse(trust[1]);
        if (_atkSpeed != 0f)
            status_value[0].text = string.Format("공격속도  {0}초", _atkSpeed.ToString());
        dpsText.text = "DPS: " + Mathf.RoundToInt(curAtk / atkDelay).ToString();
    }
}

public class OperatorCompare_MainUI : MonoBehaviour
{
    CanvasScaler canvasScreen;
    RectTransform rect;
    public Canvas operator_menu;

    OperatorCompare_DataHub dataHub;

    public Transform contents_viewer;
    public Transform operatoricon_viewer;

    // ui setting components
    public Slider level_input;
    public Text[] eliteMenu;
    public Text[] sortMenu;

    List<OperatorCompare_list> contents_list;
    List<OperaterClass> contents;
    List<Icon_Operator> operator_icons;
    int cur_elite, cur_level;
    int[] status_max;
    int curSortType;
    public void Initialized()
    {
        canvasScreen = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<CanvasScaler>();
        rect = canvasScreen.transform.GetChild(0).GetComponent<RectTransform>();
        CanvasScreen androidProgram = new CanvasScreen();
        androidProgram.setCanvasScreen(XML.This, canvasScreen, rect);

        status_max = new int[4];
        curSortType = -1;
        dataHub = GameObject.FindWithTag("DataManager").GetComponent<OperatorCompare_DataHub>();
        contents_list = new List<OperatorCompare_list>();
        for (int i = 0; i < contents_viewer.childCount; i++)
        {
            OperatorCompare_list tmp = new OperatorCompare_list();
            tmp._this = contents_viewer.GetChild(i).gameObject;
            tmp.icon = contents_viewer.GetChild(i).GetChild(0).GetComponent<RawImage>();
            tmp.frame = contents_viewer.GetChild(i).GetChild(0).GetChild(0).GetComponent<Image>();
            tmp.name = contents_viewer.GetChild(i).GetChild(0).GetChild(2).GetComponent<Text>();
            tmp.level = contents_viewer.GetChild(i).GetChild(0).GetChild(3).GetChild(0).GetComponent<Text>();

            tmp.status_bar[0] = contents_viewer.GetChild(i).GetChild(2).GetComponent<Image>();
            tmp.status_bar[1] = contents_viewer.GetChild(i).GetChild(3).GetComponent<Image>();
            tmp.status_bar[2] = contents_viewer.GetChild(i).GetChild(4).GetComponent<Image>();
            tmp.status_bar[3] = contents_viewer.GetChild(i).GetChild(5).GetComponent<Image>();
            tmp.status = contents_viewer.GetChild(i).GetChild(6).GetComponent<Text>();

            tmp.status_value[0] = contents_viewer.GetChild(i).GetChild(7).GetComponent<Text>();
            tmp.status_value[1] = contents_viewer.GetChild(i).GetChild(8).GetComponent<Text>();
            tmp.status_value[2] = contents_viewer.GetChild(i).GetChild(9).GetComponent<Text>();
            tmp.dpsText = contents_viewer.GetChild(i).GetChild(10).GetComponent<Text>();
            tmp.trustValue = contents_viewer.GetChild(i).GetChild(11).GetComponent<Text>();

            contents_list.Add(tmp);
        }
        operator_icons = new List<Icon_Operator>();
        for (int i = 0; i < operatoricon_viewer.childCount; i++)
        {
            Icon_Operator tmp = new Icon_Operator();
            tmp._this = operatoricon_viewer.GetChild(i).gameObject;
            tmp.icon = operatoricon_viewer.GetChild(i).GetChild(0).GetComponent<RawImage>();
            tmp.frame = operatoricon_viewer.GetChild(i).GetChild(1).GetComponent<Image>();
            tmp.name = operatoricon_viewer.GetChild(i).GetChild(3).GetComponent<TextMeshProUGUI>();
            operator_icons.Add(tmp);
        }
        // start ui setting
        ClearSortMenu();
        MenuStart_Operator();
        SetMenuOperator(false);
        MenuRefresh_EliteButton(-1);

        level_input.value = 1;
        level_input.maxValue = 1;
        cur_elite = 0;
        contents = new List<OperaterClass>();
   
        //contents = dataHub.GetOperatorClass();
        Refresh(cur_elite);
    }
    void ClearSortMenu()
    {
        for (int i = 0; i < sortMenu.Length; i++)
            sortMenu[i].color = Color.gray;
    }
    public void Refresh(int elite)
    {
        for (int i = 0; i < contents_list.Count; i++)
        {
            int curElite = elite;
            if (i >= contents.Count)
            {
                contents_list[i]._this.SetActive(false);
                continue;
            }
            contents_list[i].RefreshGraphic(contents[i].icon, dataHub.GetRareColor(contents[i].rare), contents[i].name);

            if (elite + 1 > contents[i].level_status.Count)
            {
                if (contents[i].level_status.Count.Equals(2))
                    curElite = 1;
                else if (contents[i].level_status.Count.Equals(1))
                    curElite = 0;
            }
            contents_list[i].Refresh(cur_level, status_max, contents[i].level_status[curElite].def.hp, contents[i].level_status[curElite].def.atk,
                contents[i].level_status[curElite].def.def, contents[i].level_status[curElite].def.m_def, contents[i].trust);
            contents_list[i].Refresh_Static(contents[i].level_status[curElite].attack_time, contents[i].level_status[curElite].cost, contents[i].level_status[curElite].block, contents[i].trust);
        }
    }
    public void RefreshSort()
    {
        for (int i = 0; i < contents_list.Count; i++)
        {
            int now_level = cur_level, now_elite = cur_elite;
            if (i >= contents.Count)
            {
                contents_list[i]._this.SetActive(false);
                continue;
            }
          
            if (cur_elite + 1 > contents[i].level_status.Count)
            {
                if (contents[i].level_status.Count.Equals(2))
                    now_elite = 1;
                else if (contents[i].level_status.Count.Equals(1))
                    now_elite = 0;

                now_level = contents[i].level_status[now_elite].maxlevel;
            }
            else if (cur_level > contents[i].level_status[now_elite].maxlevel)
                now_level = contents[i].level_status[now_elite].maxlevel;

            Level_Info cur = contents[i].level_status[now_elite];
            int hp, atk, def, mdef;
            hp = Mathf.RoundToInt(cur.def.hp + (cur.add_hp * now_level));
            atk = Mathf.RoundToInt(cur.def.atk + (cur.add_atk * now_level));
            def = Mathf.RoundToInt(cur.def.def + (cur.add_def * now_level));
            mdef = Mathf.RoundToInt(cur.def.m_def + (cur.add_m_def * now_level));

            contents_list[i].RefreshGraphic(contents[i].icon, dataHub.GetRareColor(contents[i].rare), contents[i].name);
            contents_list[i].Refresh(now_level, status_max, hp, atk, def, mdef, null, cur.attack_time);
        }
    }
    public void Refresh_Level(int elite, int level)
    {
        for (int i = 0; i < contents_list.Count; i++)
        {
            int now_level = level, now_elite = elite;
            if (i >= contents.Count)
            {
                contents_list[i]._this.SetActive(false);
                continue;
            }
            if (elite + 1 > contents[i].level_status.Count)
            {
                if (contents[i].level_status.Count.Equals(2))
                    now_elite = 1;
                else if (contents[i].level_status.Count.Equals(1))
                    now_elite = 0;

                now_level = contents[i].level_status[now_elite].maxlevel;
            }
            else if (cur_level > contents[i].level_status[elite].maxlevel)
                now_level = contents[i].level_status[elite].maxlevel;

            Level_Info cur = contents[i].level_status[now_elite];
            int hp, atk, def, mdef;
            hp = Mathf.RoundToInt(cur.def.hp + (cur.add_hp * now_level));
            atk = Mathf.RoundToInt(cur.def.atk + (cur.add_atk * now_level));
            def = Mathf.RoundToInt(cur.def.def + (cur.add_def * now_level));
            mdef = Mathf.RoundToInt(cur.def.m_def + (cur.add_m_def * now_level));

            contents_list[i].Refresh(now_level, status_max, hp, atk, def, mdef);
        }
    }

    void MenuRefresh_EliteButton(int index)
    {
        for(int i = 0; i < eliteMenu.Length; i++)
        {
            if (i.Equals(index))
                eliteMenu[i].color = Color.white;
            else
                eliteMenu[i].color = Color.gray;
        }
    }
    public void SetMenuOperator(bool set)
    {
        operator_menu.enabled = set;
    }
    public void MenuStart_Operator()
    {
        List<OperaterClass> operators = dataHub.GetOperatorClass();
        for(int i = 0; i < operator_icons.Count; i++)
        {
            if (i >= operators.Count)
            {
                operator_icons[i]._this.SetActive(false);
                continue;
            }
            operator_icons[i].Refresh(operators[i].icon, dataHub.GetRareColor(operators[i].rare), operators[i].name);
        }
    }

    public void SetElite(int index)
    {
        if (!curSortType.Equals(-1))
            TRSortReset();

        if (contents.Count <= 0)
        {
            StartCoroutine(LogU.Use.SetLog("오퍼레이터를 선택해주세요."));
            return;
        }
        cur_elite = index;

        bool isElite = false;
        for (int i = 0; i < contents.Count; i++)
        {
            if (index + 1 > contents[i].level_status.Count)
                continue;
            else
                isElite = true;
        }
        if (!isElite)
        {
            StartCoroutine(LogU.Use.SetLog("해당 정예화 레벨을 선택할 수 없습니다."));
            SetElite(index - 1);
            return;
        }

        status_max[0] = 0;
        status_max[1] = 0;
        status_max[2] = 0;
        status_max[3] = 0;

        int maxlevel = 0;
        for (int i = 0; i < contents.Count; i++)
        {
            int curIndex = index;
            if (index + 1 > contents[i].level_status.Count)
            {
                if (contents[i].level_status.Count.Equals(2))
                    curIndex = 1;
                else if (contents[i].level_status.Count.Equals(1))
                    curIndex = 0;
            }
            if (status_max[0] < contents[i].level_status[curIndex].max.hp)
                status_max[0] = contents[i].level_status[curIndex].max.hp;
            if (status_max[1] < contents[i].level_status[curIndex].max.atk)
                status_max[1] = contents[i].level_status[curIndex].max.atk;
            if (status_max[2] < contents[i].level_status[curIndex].max.def)
                status_max[2] = contents[i].level_status[curIndex].max.def;
            if (status_max[3] < contents[i].level_status[curIndex].max.m_def)
                status_max[3] = contents[i].level_status[curIndex].max.m_def;

            if (maxlevel < contents[i].level_status[curIndex].maxlevel)
                maxlevel = contents[i].level_status[curIndex].maxlevel;
        }

        level_input.value = 1;
        level_input.maxValue = maxlevel;

        MenuRefresh_EliteButton(index);

        Refresh(cur_elite);
        SetLevel(level_input);
    }
    public void SetLevel(Slider input)
    {
        if (!curSortType.Equals(-1))
            TRSortReset();
        cur_level = (int)input.value;
        Refresh_Level(cur_elite, cur_level);
    }
   
    public void TRSortReset()
    {
        ClearSortMenu();
        curSortType = -1;
    }
    public void TRSort(int index)
    {
        Sort(index, 0);
    }
    void Sort(int type, int _where)
    {
        if (contents.Count.Equals(0))
            return;
        ClearSortMenu();
        sortMenu[type].color = Color.white;
        curSortType = type;
        switch (type)
        {
            case 0:
                contents.Sort(delegate (OperaterClass a, OperaterClass b)
                {
                    int[] A = GetEliteLevel(a);
                    int[] B = GetEliteLevel(b);

                    Level_Info curA = a.level_status[A[0]];
                    Level_Info curB = b.level_status[B[0]];
                    if (curA.def.hp + (curA.add_hp * A[1]) > curB.def.hp + (curB.add_hp * B[1])) return -1;
                    else if (curA.def.hp + (curA.add_hp * A[1]) < curB.def.hp + (curB.add_hp * B[1])) return 1;
                    return 0;
                });
                break;
            case 1:
                contents.Sort(delegate (OperaterClass a, OperaterClass b)
                {
                    int[] A = GetEliteLevel(a);
                    int[] B = GetEliteLevel(b);

                    Level_Info curA = a.level_status[A[0]];
                    Level_Info curB = b.level_status[B[0]];
                    if (curA.def.atk + (curA.add_atk * A[1]) > curB.def.atk + (curB.add_atk * B[1])) return -1;
                    else if (curA.def.atk + (curA.add_atk * A[1]) < curB.def.atk + (curB.add_atk * B[1])) return 1;
                    return 0;
                });
                break;
            case 2:
                contents.Sort(delegate (OperaterClass a, OperaterClass b)
                {
                    int[] A = GetEliteLevel(a);
                    int[] B = GetEliteLevel(b);

                    Level_Info curA = a.level_status[A[0]];
                    Level_Info curB = b.level_status[B[0]];
                    if (curA.def.def + (curA.add_def * A[1]) > curB.def.def + (curB.add_def * B[1])) return -1;
                    else if (curA.def.def + (curA.add_def * A[1]) < curB.def.def + (curB.add_def * B[1])) return 1;
                    return 0;
                });
                break;
            case 3:
                contents.Sort(delegate (OperaterClass a, OperaterClass b)
                {
                    int[] A = GetEliteLevel(a);
                    int[] B = GetEliteLevel(b);

                    Level_Info curA = a.level_status[A[0]];
                    Level_Info curB = b.level_status[B[0]];
                    if (curA.def.m_def + (curA.add_m_def * A[1]) > curB.def.m_def + (curB.add_m_def * B[1])) return -1;
                    else if (curA.def.m_def + (curA.add_m_def * A[1]) < curB.def.m_def + (curB.add_m_def * B[1])) return 1;
                    return 0;
                });
                break;
            case 4:
                contents.Sort(delegate (OperaterClass a, OperaterClass b)
                {
                    int[] A = GetEliteLevel(a);
                    int[] B = GetEliteLevel(b);

                    Level_Info curA = a.level_status[A[0]];
                    Level_Info curB = b.level_status[B[0]];
                    float atkSpeedA = curA.attack_time;
                    float atkSpeedB = curB.attack_time;

                    if (atkSpeedA > atkSpeedB)
                        return 1;
                    else if (atkSpeedA < atkSpeedB)
                        return -1;
                    return 0;
                });
                break;
            default:
                break;
        }

        RefreshSort();
    }
    
    public void SetAddOperator(Transform target)
    {
        int index = target.GetSiblingIndex();
        OperaterClass tmp = dataHub.GetOperator(index);

        bool flag = false;
        for (int i = 0; i < contents.Count; i++)
        {
            if (tmp == contents[i])
            {
                flag = true;
                break;
            }
        }
        if (flag)
            StartCoroutine(LogU.Use.SetLog("이미 추가된 오퍼레이터 입니다."));
        else
        {
            contents.Add(tmp);
            if (!curSortType.Equals(-1))
                Sort(curSortType, 1);
            SetElite(cur_elite);
        }
    }
    public void SetDeleteOperator(Transform target)
    {
        int index = target.GetSiblingIndex();
        contents.RemoveAt(index);
       
        SetElite(cur_elite);
    }

    public int[] GetEliteLevel(OperaterClass a)
    {
        int[] ret = new int[2];
        ret[0] = cur_elite;
        if (cur_elite + 1 > a.level_status.Count)
        {
            if (a.level_status.Count.Equals(2))
                ret[0] = 1;
            else if (a.level_status.Count.Equals(1))
                ret[0] = 0;
            ret[1] = a.level_status[ret[0]].maxlevel;
        }
        else if (cur_level > a.level_status[ret[0]].maxlevel)
            ret[1] = a.level_status[ret[0]].maxlevel;
        else ret[1] = cur_level;

        return ret;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
        }
    }
}
