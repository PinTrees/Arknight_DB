using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using CharactorDataSet;
using UnityEngine.UI;

namespace CharactorDataSet
{
    public class PO_OperatorClass
    {
        public Sprite icon;
        // 속성 (데이터)
        public int id;
        public string name;
        public string rare;
        public string position;
        public string Class;
        public string gender;

        public string record;

        public string quality;
        public string[] tag;
        public PO_OperatorClass() { }
        public PO_OperatorClass(int _id, string _name, string _rare, string _j, string _sex, string _class, string _pos)
        {
            id = _id;
            name = _name;
            rare = _rare;
            quality = _j;
            gender = _sex;
            Class = _class;
            position = _pos;
        }
        public void SetSprite(Sprite set)
        {
            icon = set;
        }
        public void set_record(string _record) { record = _record; }
        ~PO_OperatorClass()
        {
            icon = null;
        }
    }
    public class Status
    {
        public int hp;
        public int atk;
        public int def;
        public int m_def;
        public Status(int _hp, int _atk, int _def, int _m_def)
        {
            hp = _hp; 
            atk = _atk; 
            def = _def; 
            m_def = _m_def;
        }
        public Status Instantiate()
        {
            Status tmp = new Status(hp, atk, def, m_def);
            return tmp;
        }
    }
    public class Level_Info
    {
        public int maxlevel;
        public Status def;
        public Status max;
        public int redeploy;
        public int cost;
        public int block;
        public float attack_time;

        public float add_hp;
        public float add_atk;
        public float add_def;
        public float add_m_def;
        public Level_Info(int _maxlevel, Status _def, Status _max)
        {
            maxlevel = _maxlevel;
            def = _def;
            max = _max;
            add_hp = (max.hp - def.hp) / (maxlevel - 1.0f);
            add_atk = (max.atk - def.atk) / (maxlevel - 1.0f);
            add_def = (max.def - def.def) / (maxlevel - 1.0f);
            add_m_def = (max.m_def - def.m_def) / (maxlevel - 1.0f);
        }
        public void setStaticData(int _red, int _cost, int _block, float _atk_t)
        {
            redeploy = _red;
            cost = _cost;
            block = _block;
            attack_time = _atk_t;
        }
    }
    public class SkillLevel
    {
        public string upgradelv;
        public string[] rdata; // 스킬의 변동 데이터
        public string efctime;
        public int sp;      // 소모 SP
        public int startSp;   // 기초 SP

        public string[] submaterial;
        public string[] subCount;
        public string time;            // 스킬 업그레이드 소모 시간
    }
    public class Skill
    {
        public int id;   // ID
        public string name; // 이름
        public int maxlv;

        public int rang_idx;

        public string chargetype;        //스킬 타입 / 공격형, 방어형, 지원형
        public string trigger;     // 스킬 타겟 지정 방식 / 자동,  수동
        public string needLv;     // 스킬의 해금 레벨
        public string[] infom;

        public List<SkillLevel> data;
        public Skill(int _id, string _name, string _type, string _trigger, string _needLV)
        {
            id = _id; name = _name; chargetype = _type; trigger = _trigger; needLv = _needLV;
        }
        public void SetSkillLevel(List<SkillLevel> set) { data = set; }
    }
    public class Ability
    {
        public string unlock;
        public string name;
        public string info;
    }
    public class Elite
    {
        public int rang;
        public string[] unlockdata;
        public string[] sub_mat;
    }
    public class InfraSkill
    {
        public int id;

        public Texture2D icon;
        public int icon_idx;

        public string name;
        public string unlock;
        public string target;
        public string info;
    }
    public class Costume
    {
        public string name;
        public string type;
    }
    public class OperaterClass
    {
        public int id;
        public string name;
        public string en_name;
        public string gender;
        public string position;
        public string Class;
        public string rare;

        public string group;
        public string[] tags;
        public string[] trust;  // 신뢰도 능력치 체력/공격/방어/저항
        public string[] potential; // 잠재력 보상
        public string specificity;  // 캐릭터 전용 패시브

        public string illustrator;
        public string artistlink;
        public string voice;
        public string voicelink;

        public List<Level_Info> level_status;
        public List<Skill> skillData;
        public List<string[]> skillmaterial;
        public List<string[]> skillmaterialCount;

        public List<Ability> ability;
        public List<InfraSkill> infraData;
        public List<Elite> eliteData;

        public List<Costume> costume;
        public Texture2D icon;

        public OperaterClass(string _name, string _sex, string _position, string _class, string _rare)
        {
            skillData = new List<Skill>();
            level_status = new List<Level_Info>();
            skillmaterial = new List<string[]>();
            skillmaterialCount = new List<string[]>();

            name = _name;
            gender = _sex;
            position = _position;
            Class = _class;
            rare = _rare;
        }
        public void setID(int _id) { id = _id; }
        public void setLevel_Info(List<Level_Info> set)
        {
            level_status = set;
        }
        public void Add_LevelInfo(Level_Info set) { level_status.Add(set); }
        public void Add_SkillData(Skill set) { skillData.Add(set); }
        ~OperaterClass()
        {
            icon = null;

            for (int i = 0; i < level_status.Count; i++)
                level_status.RemoveAt(0);
            for (int i = 0; i < skillData.Count; i++)
                skillData.RemoveAt(0);
            for (int i = 0; i < ability.Count; i++)
                ability.RemoveAt(0);
            for (int i = 0; i < infraData.Count; i++)
                infraData.RemoveAt(0);
            for (int i = 0; i < eliteData.Count; i++)
                eliteData.RemoveAt(0);
            for (int i = 0; i < costume.Count; i++)
                costume.RemoveAt(0);

            for (int i = 0; i < skillmaterial.Count; i++)
                skillmaterial.RemoveAt(0);
            for (int i = 0; i < skillmaterialCount.Count; i++)
                skillmaterialCount.RemoveAt(0);
        }
    }
}

public class CharactorData : MonoBehaviour
{
    //public List<Sprite> icon;
    public List<PO_OperatorClass> Ch_dataList = new List<PO_OperatorClass>();
    public Sprite[] operatorFrmae;
    public Color[] colors;
    void Start()
    {
        operatorFrmae = Resources.LoadAll<Sprite>("UI/Frame/Operator");
        Sprite[] sprites = Resources.LoadAll<Sprite>("PublicOffer_Icon");

        Ch_dataList = XML.This.Get_PublicOfferTag("OperatorData_publicoffer");
        for (int i = 0; i < Ch_dataList.Count; i++)
            Ch_dataList[i].SetSprite(sprites[i]);

        colors = new Color[6];
        colors[0] = new Color(0, 0, 0);
        colors[1] = new Color(255 / 255.0f, 75 / 255.0f, 0 / 255.0f);
        colors[2] = new Color(255 / 255.0f, 150 / 255.0f, 0 / 255.0f);
        colors[3] = new Color(150 / 255.0f, 100 / 255.0f, 255 / 255.0f);
        colors[4] = new Color(100 / 255.0f, 150 / 255.0f, 255 / 255.0f);
        colors[5] = new Color(100 / 255.0f, 150 / 255.0f, 100 / 255.0f);
    }
    public PO_OperatorClass GetOperatorData(string name)
    {
        PO_OperatorClass data = Ch_dataList.Find(delegate (PO_OperatorClass a)
        {
            return a.name == name;
        });
        return data;
    }
    public PO_OperatorClass get_data(string _name)
    {
        for(int i=0; i < Ch_dataList.Count; i++)
        {
            if(Ch_dataList[i].name == _name)
            {
                return Ch_dataList[i];
            }
        }
        return null;
    }
    public Sprite GetIcon(string name)
    {
        Sprite tmp;
        PO_OperatorClass data = Ch_dataList.Find(delegate (PO_OperatorClass a)
        {
            return a.name == name;
        });
        tmp = data.icon;
        return tmp;
    }
    public Color GetFrameColor (string name)
    {
        PO_OperatorClass data = Ch_dataList.Find(delegate (PO_OperatorClass a)
        {
            return a.name == name;
        });

        if (data.rare.Equals("6"))
            return colors[1];
        else if (data.rare.Equals("5"))
            return colors[2];
        else if (data.rare.Equals("4"))
            return colors[3];
        else if (data.rare.Equals("3"))
            return colors[4];
        else if (data.rare.Equals("2"))
            return colors[5];
        return colors[0];
    }
    public Sprite GetOperatorFrame(string rare)
    {
        return operatorFrmae[int.Parse(rare) - 1];
    }
}

