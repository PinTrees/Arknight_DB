using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

using TagManager;
using CharactorDataSet;

namespace TagManager
{
    public class Tags
    {
        public string name;
        public bool info;
        public Tags(string _name, bool set)
        {
            name = _name; info = set;
        }
        public void SetData(string _name, bool set)
        {
            name = _name; info = set;
        }
    }
    public class TagList
    {
        public List<Tags> rare;
        public Tags position;
        public Tags Gender;
        public Tags Class;
        public TagList()
        {
            rare = new List<Tags>();
            position = new Tags(string.Empty, false);
            Gender = new Tags(string.Empty, false);
            Class = new Tags(string.Empty, false);
            rare.Add(new Tags("1", false));
            rare.Add(new Tags("2", false));
            rare.Add(new Tags("3", false));
            rare.Add(new Tags("4", false));
            rare.Add(new Tags("5", false));
            rare.Add(new Tags("6", false));
        }
        public List<string> GetBoolRare()
        {
            List<string> set = new List<string>();
            bool flag = false;
            for (int i = 0; i < rare.Count; i++)
                if (rare[i].info)
                {
                    set.Add(rare[i].name);
                    flag = true;
                }
            if (flag)
                return set;
            else return null;
        }
    }
    public class TagClass
    {
        public List<string> rare;
        public List<string> Class;

        public TagClass()
        {
            rare = new List<string>();
            Class = new List<string>();
        }
        public bool Add_Rare(string set)
        {
            int c = rare.FindIndex(delegate (string a) { return a == set; });
            if(!c.Equals(-1))
            {
                rare.RemoveAt(c);
                return false;
            }
            rare.Add(set);
            return true;
        }
        public bool Add_Class(string set)
        {
            int c = Class.FindIndex(delegate (string a) { return a == set; });
            if (!c.Equals(-1))
            {
                Class.RemoveAt(c);
                return false;
            }
            Class.Add(set);
            return true;
        }
    }
}

public class OperaterTagSearch_UI_Manager : MonoBehaviour
{
    public Operater_Data_Hub dataHub;
    public OperaterIcon_UI_Manager iconUI;

    public Transform tr_rare_btn_menu;
    public Transform tr_class_btn;

    private List<Image> btn_list;
    private List<string> btn_name_list;
    
    private TagClass select_tag;
    private string curClass;
    private void Start()
    {
        curClass = string.Empty;
        select_tag = new TagClass();
        btn_list = new List<Image>();
        
        for (int i = 0; i < tr_rare_btn_menu.childCount; i++) // 아이콘 리스트 생성
            btn_list.Add(tr_rare_btn_menu.transform.GetChild(i).GetComponent<Image>());

        for (int i = 0; i < tr_class_btn.childCount; i++)
            btn_list.Add(tr_class_btn.transform.GetChild(i).GetComponent<Image>());

        InitializeBtnName();
    }
    public void InitializeBtnName()
    {
        btn_name_list = new List<string>();
        btn_name_list.Add("1");
        btn_name_list.Add("2");
        btn_name_list.Add("3");
        btn_name_list.Add("4");
        btn_name_list.Add("5");
        btn_name_list.Add("6");
        btn_name_list.Add("뱅가드");
        btn_name_list.Add("스나이퍼");
        btn_name_list.Add("메딕");
        btn_name_list.Add("캐스터");
        btn_name_list.Add("가드");
        btn_name_list.Add("디펜더");
        btn_name_list.Add("서포터");
        btn_name_list.Add("특수");
    }
    public void Refresh()
    {
        List<OperaterClass> ret = Search(dataHub.operatorData, select_tag);
        iconUI.Refresh(ret);
    }
    public void Refresh(List<OperaterClass> data)
    {
        if (data.Count < 1)
            iconUI.Refresh(dataHub.operatorData);
        else
            iconUI.Refresh(data);
    }
    public void SetClassTag(string name)
    {
        int index = btn_name_list.FindIndex(delegate (string a) { return a == name; });
        if (select_tag.Add_Class(name))
        {
            btn_list[index].color = new Color(255 / 255f, 0, 77 / 255f);
        }
        else
        {
            btn_list[index].color = Color.white;
        }
        Refresh();
    }
    public void TR_SingleClassTag(string name)
    {
        List<OperaterClass> ret;

        if (curClass.Equals(name))
        {
            name = string.Empty;
            ret = Search(dataHub.operatorData);
        }
        else
            ret = Search(dataHub.operatorData, name);

        curClass = name;

        for (int i = 0; i < btn_list.Count; i++)
        {
            if(btn_name_list[i].Equals(name))
                btn_list[i].color = new Color(255 / 255f, 0, 77 / 255f);
            else
                btn_list[i].color = Color.white;
        }

        Refresh(ret);
    }
    public void SetRareTag(string name)
    {
        int index = btn_name_list.FindIndex(delegate (string a) { return a == name; });
        if (select_tag.Add_Rare(name))
        {
            btn_list[index].color = Color.black;
        }
        else
        {
            btn_list[index].color = new Color(150 / 255f, 150 / 255f, 150 / 255f);
        }
        Refresh();
    }
    public List<OperaterClass> Search(List<OperaterClass> data, TagClass tagdata)
    {
        bool flag = false;
        List<OperaterClass> ret_list = new List<OperaterClass>();
        for (int i = 0; i < tagdata.rare.Count; i++)
        {
            flag = true;
            for (int j = 0; j < data.Count; j++)
            {
                if (tagdata.rare[i].Equals(data[j].rare))
                {
                    ret_list.Add(data[j]);
                }
            }
        }
        if (tagdata.Class.Count > 0)
        {
            List<OperaterClass> target;
            List<OperaterClass> ret = new List<OperaterClass>();

            if (flag)
            {
                target = ret_list;
            }
            else
            {
                target = data;
                flag = true;
            }
            for (int tagcount = 0; tagcount < tagdata.Class.Count; tagcount++)
            {
                //Debug.Log(tagdata.Class[tagcount]);
                for (int index = 0; index < target.Count; index++)
                {
                    //Debug.Log(target[index].Class);
                    if (tagdata.Class[tagcount].Equals(target[index].Class))
                    {
                        ret.Add(target[index]);
                    }
                }
            }
            ret_list = ret;
        }

        if (!flag)
            ret_list = data;

        ret_list.Sort(delegate (OperaterClass a, OperaterClass b) // 클래스 리스트 특정 요소값으로 정렬 방법
        {
            if (a.id > b.id) return 1;
            else if (a.id < b.id) return -1;
            return 0;
        });

        return ret_list;
    }
    public List<OperaterClass> Search(List<OperaterClass> data, string _class = null)
    {
        List<OperaterClass> ret_list = new List<OperaterClass>();

        if (_class == null)
            return ret_list;

        for (int i = 0; i < data.Count; i++)
            if (data[i].Class.Equals(_class))
                ret_list.Add(data[i]);

        ret_list.Sort(delegate (OperaterClass a, OperaterClass b) // 클래스 리스트 특정 요소값으로 정렬 방법
        {
            if (a.id > b.id) return 1;
            else if (a.id < b.id) return -1;
            return 0;
        });

        return ret_list;
    }
}
