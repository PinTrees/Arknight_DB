using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using CharactorDataSet;
using FireBaseClass;
using TMPro;

namespace UI
{
    public class CanvasScreen
    {
        public bool[] setCanvasScreen(XML xml, CanvasScaler canvasScreen, RectTransform rect)
        {
            string[] androidbarStatus = xml.GetUIStatusBarXML();
            bool statusbar = (androidbarStatus[0].Equals("1"));
            bool navigationbar = (androidbarStatus[1].Equals("1"));

            ApplicationChrome.setSystemUiVisibility(canvasScreen, rect, statusbar, navigationbar);
            bool[] res = { statusbar, navigationbar };
            return res;
        }
    }
    public class RawIconOp
    {
        public GameObject This;
        public RawImage rawicon;
        public RawImage fram;
        public Text name;

        ~RawIconOp()
        {
            This = null;
            rawicon.texture = null;
            rawicon = null;
            rawicon = null;
            fram.texture = null;
            fram = null;
            name = null;
        }
    }
    public class InfraIcon
    {
        public GameObject _this;
        public RectTransform rectTransform;

        public Image nameImg;
        public RawImage icon;
        public Image rareicon;
        public RawImage eliteicon;
        public RawImage[] skillicons;
        public TextMeshProUGUI name;
        public TextMeshProUGUI operName;
        public Text leveltxt;
        public TextMeshProUGUI info;
        public GameObject upBtn;

        public int level;
        public int skill;
        public InfraIcon Initialize(Transform target)
        {
            _this = target.gameObject;
            rectTransform = target.GetComponent<RectTransform>();
            skillicons = new RawImage[_this.transform.GetChild(1).childCount];

            icon = target.GetChild(0).GetChild(0).GetComponent<RawImage>();
            rareicon = target.GetChild(0).GetChild(1).GetComponent<Image>();
            operName = target.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>();

            eliteicon = target.GetChild(3).GetComponentInChildren<RawImage>();
            nameImg = target.GetChild(2).GetComponent<Image>();

            name = target.GetChild(2).GetComponentInChildren<TextMeshProUGUI>();
            leveltxt = target.GetChild(5).GetComponent<Text>();
            info = target.GetChild(6).GetComponent<TextMeshProUGUI>();

            upBtn = target.GetChild(4).gameObject;
            for (int j = 0; j < skillicons.Length; j++)
                skillicons[j] = _this.transform.GetChild(1).GetChild(j).GetComponent<RawImage>();

            return this;
        }
        public InfraSkill RefreshSkill(OperaterClass set, int index, Color color)
        {
            skill = index;
            for (int i = 0; i < skillicons.Length; i++)
                skillicons[i].color = Color.gray;
            skillicons[index].color = Color.white;
            int idx = 0;
            for (int i = 0; i < set.infraData.Count; i++)
            {
                if (set.infraData[i].id.Equals(skill + 1))
                {
                    idx = i;
                    if (set.infraData.Count <= i + 1)
                        upBtn.SetActive(false);
                    else if (!set.infraData[i + 1].id.Equals(skill + 1))
                        upBtn.SetActive(false);
                    else
                        upBtn.SetActive(true);
                    break;
                }
            }

            nameImg.color = color;
            level = 0;
            leveltxt.text = string.Format("Lv{0}", level + 1);
            name.text = set.infraData[idx].name;
            info.text = set.infraData[idx].info;

            RefreshRectSize();

            return set.infraData[idx];
        }
        public InfraSkill Upgrade(OperaterClass set)
        {
            level++;
            info.text = string.Empty;

            int idx = 0;
            for (; idx < set.infraData.Count; idx++)
            {
                if (set.infraData[idx].id.Equals(skill + 1))
                {
                    if (set.infraData.Count <= idx + level)
                    {
                        level = 0;
                        break;
                    }
                    if (set.infraData[idx + level].id.Equals(skill + 1))
                    {
                        idx = idx + level;
                        break;
                    }
                    else
                    {
                        level = 0;
                        break;
                    }
                }
            }
            if (level.Equals(0))
                for (idx = 0; idx < set.infraData.Count; idx++)
                    if (set.infraData[idx].id.Equals(skill + 1))
                        break;

            leveltxt.text = string.Format("Lv{0}", level + 1);
            name.text = set.infraData[idx].name;
            info.text = set.infraData[idx].info;

            RefreshRectSize();

            return set.infraData[idx];
        }
        public void Refresh(OperaterClass set)
        {
            name.text = set.infraData[0].name;
            operName.text = set.name;
            info.text = set.infraData[0].info;
            leveltxt.text = string.Format("Lv{0}", 1);
            skill = 0;
            for (int i = 0; i < set.infraData.Count; i++)
            {
                if (set.infraData[i].id.Equals(skill + 1))
                {
                    if (set.infraData.Count <= i + 1)
                        upBtn.SetActive(false);
                    else if (!set.infraData[i + 1].id.Equals(skill + 1))
                        upBtn.SetActive(false);
                    else
                        upBtn.SetActive(true);
                    break;
                }
            }

            RefreshRectSize();
        }
        public void RefreshGraphic(Texture2D _icon, Texture2D[] skilltextur, Color _rare, Color color)
        {
            icon.texture = _icon;
            rareicon.color = _rare;
            for (int i = 0; i < skillicons.Length; i++)
            {
                if (i >= skilltextur.Length)
                {
                    skillicons[i].gameObject.SetActive(false);
                    continue;
                }
                skillicons[i].texture = skilltextur[i];
                skillicons[i].gameObject.SetActive(true);
                skillicons[i].color = Color.gray;
            }
            skillicons[0].color = Color.white;
            nameImg.color = color;
        }
        void RefreshRectSize()
        {
            if (info.preferredHeight < 50)
                rectTransform.sizeDelta = new Vector2(rectTransform.rect.width, 194);
            else if (info.preferredHeight < 100)
                rectTransform.sizeDelta = new Vector2(rectTransform.rect.width, 220);
            else if (info.preferredHeight < 150)
                rectTransform.sizeDelta = new Vector2(rectTransform.rect.width, 260);
            else if (info.preferredHeight < 200)
                rectTransform.sizeDelta = new Vector2(rectTransform.rect.width, 280);
            else if (info.preferredHeight < 250)
                rectTransform.sizeDelta = new Vector2(rectTransform.rect.width, 320);
        }
    }
    public class IconMT
    {
        public GameObject This;

        public RawImage icon;
        public Image frame;

        public Text name;
        public Text count;

        public void Refresh(Texture _icon, Sprite _frame, string _name, string _count)
        {
            icon.texture = _icon;
            frame.sprite = _frame;
            name.text = _name;
            count.text = _count;
            This.SetActive(true);
        }
        ~IconMT()
        {
            This = null;
            icon = null;
            frame = null;
        }
    }
    public class RawIconMT
    {
        public GameObject _this;

        public RawImage icon;
        public Image frame;

        public TextMeshProUGUI count;
        public void Initialize(Transform target)
        {
            _this = target.gameObject;
            frame = target.GetChild(0).GetComponent<Image>();
            icon = target.GetChild(1).GetComponent<RawImage>();
            count = target.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>();
        }
        public void Refresh(Texture _icon, Sprite _frame, string _count)
        {
            icon.texture = _icon;
            frame.sprite = _frame;
            //name.text = _name;
            count.text = _count;
            _this.SetActive(true);
        }
        ~RawIconMT()
        {
            _this = null;
            icon = null;
            frame = null;
        }
    }
    public class IconMT_1
    {
        public GameObject This;

        public RawImage icon;
        public Image frame;

        public TextMeshProUGUI name;
        public TextMeshProUGUI count;
        public void Initiailize(Transform target)
        {
            This = target.gameObject;
            frame = target.GetChild(0).GetComponent<Image>();
            icon = target.GetChild(1).GetComponent<RawImage>();
            name = target.GetChild(3).GetComponent<TextMeshProUGUI>();
            count = target.GetChild(4).GetComponent<TextMeshProUGUI>();
        }
        public void Refresh(Texture _icon, Sprite _frame, string _name, string _count)
        {
            icon.texture = _icon;
            frame.sprite = _frame;
            name.text = _name;
            count.text = _count;
            This.SetActive(true);
        }
        ~IconMT_1()
        {
            This = null;
            icon = null;
            frame = null;
        }
    }
    public class IconMT_2   //select - switch count
    {
        public GameObject _this;
        public GameObject plus;
        public GameObject mnus;
        public GameObject countP;

        public RawImage icon;
        public RawImage frame;

        public TextMeshProUGUI name;
        public TextMeshProUGUI count;
        public IconMT_2 Initiailize(Transform target)
        {
            _this = target.gameObject;
            frame = target.GetChild(0).GetComponent<RawImage>();
            icon = target.GetChild(1).GetComponent<RawImage>();
            countP = target.GetChild(2).gameObject;
            count = target.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>();

            plus = target.GetChild(3).gameObject;
            mnus = target.GetChild(4).gameObject;

            DisableSelect();

            return this;
        }
        public void Refresh(Texture _icon, Texture _frame)
        {
            icon.texture = _icon;
            frame.texture = _frame;
            icon.color = frame.color = new Color(1f, 1f, 1f);
            count.text = "0";
            _this.SetActive(true);
            countP.SetActive(true);
        }
        public void setCount(int set)
        {
            count.text = set.ToString();
            int c = int.Parse(count.text);
            if (c <= 0)
                DisableSelect();
            else
                ActiveSelect();
        }
        public void addCount(int set)
        {
            int c = int.Parse(count.text);
            c = c + set;
            if (c <= 0)
                DisableSelect();
            else
            {
                count.text = c.ToString();
                ActiveSelect();
            }
        }
        public void ActiveSelect()
        {
            icon.color = frame.color = new Color(1f, 1f, 1f);
            countP.SetActive(true);
            plus.SetActive(true);
            mnus.SetActive(true);
        }
        public void DisableSelect()
        {
            icon.color = frame.color = new Color(0.4f, 0.4f, 0.4f);
            count.text = "0";
            countP.SetActive(false);
            plus.SetActive(false);
            mnus.SetActive(false);
            if (!_this.activeSelf)
                _this.SetActive(true);
        }
        public void DisableSelectMenu()
        {
            mnus.SetActive(false);
            plus.SetActive(false);
        }

        public bool getActive()
        {
            int c = int.Parse(count.text);
            if (c > 0)
                return true;
            else return false;
        }
        public string getCount()
        {
            return count.text;
        }
        ~IconMT_2()
        {
            _this = null;
            icon = null;
            frame = null;
        }
    }
    public class IconMT_3
    {
        public GameObject _this;

        public RawImage icon;
        public RawImage frame;

        public TextMeshProUGUI count;
        public void Initialize(Transform target)
        {
            _this = target.gameObject;
            frame = target.GetChild(0).GetComponent<RawImage>();
            icon = target.GetChild(1).GetComponent<RawImage>();
            count = target.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>();
        }
        public void Refresh(Texture2D _icon, Texture2D _frame, string _count)
        {
            icon.texture = _icon;
            frame.texture = _frame;
            //name.texture = _name;
            count.text = _count;
            _this.SetActive(true);
        }
        ~IconMT_3()
        {
            _this = null;
            icon = null;
            frame = null;
        }
    }
    public class IconDropMT
    {
        public GameObject _this;
        public RawImage frame;
        public RawImage icon;
        public TextMeshProUGUI getCount;
        public TextMeshProUGUI tryCount;
        public TextMeshProUGUI percent;
        public TextMeshProUGUI needEnegy;
        public void Initiailize(Transform target)
        {
            _this = target.gameObject;
            frame = target.GetChild(0).GetComponent<RawImage>();
            icon = target.GetChild(0).GetChild(0).GetComponent<RawImage>();
            getCount = target.GetChild(1).GetComponent<TextMeshProUGUI>();
            tryCount = target.GetChild(2).GetComponent<TextMeshProUGUI>();
            percent = target.GetChild(3).GetComponent<TextMeshProUGUI>();
            needEnegy = target.GetChild(4).GetComponent<TextMeshProUGUI>();

            _this.SetActive(false);
        }
        public void Refresh(Texture2D _frame, Texture2D _icon, string _get, string _try, int _energy)
        {
            frame.texture = _frame;
            icon.texture = _icon;
            getCount.text = _get;
            tryCount.text = _try;

            if (_get.Equals("0")) percent.text = "0%";
            else
            {
                percent.text = ((int.Parse(_get) / float.Parse(_try)) * 100).ToString("N2") + "%";
            }
            if (_get.Equals("0")) needEnegy.text = "???";
            //else   needEnegy.text = ((100 / float.Parse(percent.text)) * _energy).ToString();

            needEnegy.text = "준비중";

            if (!_this.activeSelf)
                _this.SetActive(true);
        }
    }
    public class Icon
    {
        public GameObject This;

        public Image icon;
        public Image frame;
        public RawImage icon_raw;
        public RawImage frame_raw;

        public Text name_txt;
        public Text txt_count; // 개수를 표시해야할 아이콘일 경우에만 사용
        public Text txt_madecount;
        public Text txt_invencount;

        public GameObject required_tag;  // 필수 항목을 표시할 필요가 있을때 사용
        public GameObject end_tag;

        public string name;
        ~Icon()
        {
            This = null;
            icon = null;
            frame = null;
            icon_raw = null;
            frame_raw = null;

            required_tag = null;
            end_tag = null;
        }
    }
    public class IconIV
    {
        public GameObject This;

        public RawImage icon;
        public RawImage frame;

        public Text name_txt;
        public Text txt_count;
        public Text txt_madecount;

        public GameObject[] temp;
        public GameObject required_tag;
        public GameObject end_tag;
    }
    public class RawIcon
    {
        public RawImage icon;
        public RawImage frame;

        public Text[] tempTxt;     // tmp
        public Text per;
    }
    public class RawIconIV
    {
        public RawImage icon;
        public RawImage frame;
        public GameObject selectUI;
        public Text per;
    }
    public class RawIcon_Op
    {
        public RawImage icon;
        public RawImage elite;
        public Text level;
    }
    public class Icon_Operator
    {
        public GameObject _this;
        public RawImage icon;
        public Image frame;
        public TextMeshProUGUI name;

        public void Refresh(Texture2D _icon, Color framecolor, string _name)
        {
            icon.texture = _icon;
            frame.color = framecolor;
            name.text = _name;
            if (!_this.activeSelf)
                _this.SetActive(true);
        }
    }
    public class CommentIcon
    {
        public GameObject _this;
        public GameObject menu;
        public RectTransform tr;
        public Text userName;
        public Text info;
        public Text date;
        public Text good;
        public Text bad;
        public Image goodBtn;
        public Image badBtn;

        public PointUser pointUser;
        public Comment data;
        public string key;
        public CommentIcon Initialize(Transform target)
        {
            _this = target.gameObject;
            tr = target.GetComponent<RectTransform>();
            userName = target.GetChild(0).GetComponent<Text>();
            info = target.GetChild(1).GetComponent<Text>();
            date = target.GetChild(2).GetComponent<Text>();
            good = target.GetChild(3).GetComponent<Text>();
            bad = target.GetChild(4).GetComponent<Text>();
            data = null;
            menu = target.GetChild(7).gameObject;
            goodBtn = target.GetChild(7).GetChild(0).GetComponent<Image>();
            badBtn = target.GetChild(7).GetChild(1).GetComponent<Image>();

            return this;
        }
        public void Refresh(Comment _set, string _key)
        {
            SetActiveMenu(null, false);

            key = _key;
            data = _set;
            userName.text = "Dr. " + _set.userName;
            info.text = _set.info;
            date.text = _set.date;
            good.text = " -";
            bad.text = " -";
            if (_this.activeSelf.Equals(false))
                _this.SetActive(true);

            if (info.preferredHeight < 50)
                tr.sizeDelta = new Vector2(tr.rect.width, 165);
            else if (info.preferredHeight < 78)
                tr.sizeDelta = new Vector2(tr.rect.width, 220);
            else if (info.preferredHeight < 120)
                tr.sizeDelta = new Vector2(tr.rect.width, 260);
            else if (info.preferredHeight < 160)
                tr.sizeDelta = new Vector2(tr.rect.width, 280);
            else if (info.preferredHeight < 200)
                tr.sizeDelta = new Vector2(tr.rect.width, 320);
        }
        public void RefreshPoint(Point _set)
        {
            if (_set == null)
            {
                good.text = " 0";
                bad.text = " 0";
            }
            else
            {
                good.text = " " + _set.good.ToString();
                bad.text = " " + _set.bad.ToString();
            }
        }
        public void SetActiveMenu(PointUser _set, bool sw)
        {
            pointUser = _set;
            if (sw)
            {
                goodBtn.color = Color.gray;
                badBtn.color = Color.gray;
                if (_set != null)
                {
                    if (_set.type == 1)
                        goodBtn.color = Color.black;
                    if (_set.type == 2)
                        badBtn.color = Color.black;
                }
                menu.SetActive(true);
            }
            else
            {
                if(menu.activeSelf)
                    menu.SetActive(false);
            }
        }
    }
}

namespace UI.ScrollViewItem
{
    public class RankingIT
    {
        public GameObject _this;
        public Text userName;
        public Text point;

        public RankingIT() { }
        public RankingIT(Transform target)
        {
            _this = target.gameObject;
            userName = target.GetChild(0).GetComponent<Text>();
            point = target.GetChild(1).GetComponent<Text>();

            if (_this.activeSelf)
                _this.SetActive(false);
        }
        public void Refresh(string _userName, string _point)
        {
            userName.text = _userName;
            point.text = _point + " point S";

            if (!_this.activeSelf)
                _this.SetActive(true);
        }
        public void Disable()
        {
            if (_this.activeSelf)
                _this.SetActive(false);
        }
    }
}
namespace UI.Button
{
    public class stage_button_1
    {
        public GameObject _this;
        public Text name_txt;
        public Image cur_state_img;
        public TextMeshProUGUI cur_state_txt;
        public stage_button_1() { }
        public stage_button_1(Transform target)
        {
            _this = target.gameObject;
            name_txt = target.GetChild(0).GetComponent<Text>();

            cur_state_img = target.GetChild(1).GetComponent<Image>();
            cur_state_txt = target.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();

            if (_this.activeSelf)
                _this.SetActive(false);
        }
        public void Refresh(string _name, string _start_date, string _exite_date)
        {
            name_txt.text = _name;

            System.DateTime cur_date = System.DateTime.Now;
            System.DateTime start_date = System.Convert.ToDateTime(_start_date);
            System.DateTime exite_date = System.Convert.ToDateTime(_exite_date);

            System.TimeSpan timeCal = start_date - cur_date;

            Debug.Log(cur_date + ", " + _start_date);
            Debug.Log(timeCal.Days + ", " + timeCal.Hours + ", " + timeCal.Minutes);

            if (timeCal.Days > 0) set_state(0);
            else if (timeCal.Hours > 0) set_state(0);
            else if (timeCal.Minutes > 0) set_state(0);
            else
            {
                System.TimeSpan timeCal_end = exite_date - cur_date;
                if (timeCal_end.Days > 0) set_state(1);
                else if (timeCal_end.Hours > 0) set_state(1);
                else if (timeCal_end.Minutes > 0) set_state(1);
                else set_state(2);
            }

            if (!_this.activeSelf)
                _this.SetActive(true);
        }
        public void Refresh(string _name, int type)
        {
            name_txt.text = _name;
            
            set_state(type);

            if (!_this.activeSelf)
                _this.SetActive(true);
        }

        void set_state(int type)
        {
            switch (type)
            {
                case 0:
                    cur_state_img.color = new Color(0.8f, 0.7f, 0.2f);
                    cur_state_txt.text = "대기중";
                    break;
                case 1:
                    cur_state_img.color = new Color(0.3f, 0.6f, 0.2f);
                    cur_state_txt.text = "오픈됨";
                    break;
                case 2:
                    cur_state_img.color = new Color(0.75f, 0.25f, 0.25f);
                    cur_state_txt.text = "종료됨";
                    break;
            }
        }
    }
}
public class U_GUI : MonoBehaviour
{

}
