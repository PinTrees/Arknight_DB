using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CharactorDataSet;
using MaterialData;

using System.IO;
using System.Xml;

using Setting;
using DataClass;
using FilesInfo;

using UnityEngine.Networking;
using FireBaseClass;

namespace DataClass
{
    public class Enemy
    {
        public Texture2D icon;

        public string code;
        public string name;
        public string position;

        public string[] hp;
        public string[] atk;
        public string[] def;
        public string[] mdef;
        public string weight;
        public string atktime;
        public string rang;
        public string movespeed;

        public string[] tag;
        public string info;
        public string skill;

        public string[] resis;
    }
    public class OperatorSpine
    {
        public string code;
        public string made;
        public string name;
        public string parent;        // operator
        public string assetbundleName;
        public string assetlavelName;

        public Vector2 position;
        public Vector2 scale; 

        public string Yurl;
    }
}

public class XML : MonoBehaviour
{
    public static XML This;

    private void Awake()
    {
        This = this;

        List<OperaterClass> tmp = new List<OperaterClass>();
        //Get_OperatorInfo(tmp);
        //SaveIllustFileList(tmp, "OperatorIllustFile.xml", pathForDocumentsFile("Version_Info/Operator/"));
        //Get_Operater_Status(tmp);
        //Get_Operater_SkillData(tmp);
        //SaveXML(tmp, "OperatorIcon.xml", pathForDocumentsFile("Version_Info/Operator/"));
        //SaveXML_AudioFile(tmp, "OperatorAudioFile.xml", pathForDocumentsFile("Version_Info/tmp/"));
        //SaveDB_Audio(tmp, "operator_audiofile_list.xml", pathForDocumentsFile("Version_Info/tmp/"));
    }
    public string pathForDocumentsFile(string filename)
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            string path = Application.dataPath.Substring(0, Application.dataPath.Length - 5);
            path = path.Substring(0, path.LastIndexOf('/'));
            return Path.Combine(Path.Combine(path, "Documents"), filename);
        }
        else if (Application.platform == RuntimePlatform.Android)
        {
            string path = Application.persistentDataPath;
            path = path.Substring(0, path.LastIndexOf('/'));
            return Path.Combine(path, filename);
        }
        else
        {
            string path = Application.dataPath;
            path = path.Substring(0, path.LastIndexOf('/'));
            return Path.Combine(path, filename);
        }
    }
    public void Get_OperatorInfo(List<OperaterClass> set)
    {
        string path = pathForDocumentsFile("Resource/DB/Operator_Info.xml");
        FileInfo fileInfo = new FileInfo(path);
        if (!fileInfo.Exists) return;

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(path);

        int index = 0;
        for (int rare = 6; rare > 0; rare--)
        {
            XmlNodeList nodes = xmlDoc.SelectNodes("ChractorList/rare_" + rare + "/Info");
            for (int i = 0; i < nodes.Count; i++)
            {
                XmlElement now = (XmlElement)nodes[i];
                OperaterClass parent = new OperaterClass(now.GetAttribute("name"), now.GetAttribute("gender"), now.GetAttribute("position"), now.GetAttribute("Class"), now.GetAttribute("rare"));
                parent.id = index++;
                parent.en_name = now.GetAttribute("en_name");
                parent.group = now.GetAttribute("group");
                parent.tags = now.GetAttribute("tags").Split('/');
                parent.trust = now.GetAttribute("trust").Split('/');
                parent.potential = now.GetAttribute("potential").Split('/');
                parent.specificity = now.GetAttribute("specificity");

                List<Ability> ability = new List<Ability>();
                for (int j = 0; j < now.ChildNodes.Count; j++)
                {
                    XmlElement lv = (XmlElement)now.ChildNodes[j];
                    if (lv.Name != "Ability")
                        break;
                    Ability now_av = new Ability();
                    now_av.unlock = lv.GetAttribute("unlock");
                    now_av.name = lv.GetAttribute("name");
                    now_av.info = lv.GetAttribute("info");
                    ability.Add(now_av);
                }
                parent.ability = ability;

                List<Elite> elite = new List<Elite>();
                elite.Add(new Elite());
                for (int j = 0; j < now.ChildNodes.Count; j++)
                {
                    XmlElement lv = (XmlElement)now.ChildNodes[j];
                    if (lv.Name.Equals("Elite1"))
                    {
                        Elite now_e = new Elite();
                        now_e.unlockdata = lv.GetAttribute("info").Split('/');
                        now_e.sub_mat = lv.GetAttribute("submat").Split('/');
                        elite.Add(now_e);
                    }
                    else if (lv.Name.Equals("Elite2"))
                    {
                        Elite now_e = new Elite();
                        now_e.unlockdata = lv.GetAttribute("info").Split('/');
                        now_e.sub_mat = lv.GetAttribute("submat").Split('/');
                        elite.Add(now_e);
                    }
                    else continue;
                }
                parent.eliteData = elite;

                for (int j = 0; j < now.ChildNodes.Count; j++)
                {
                    if (now.ChildNodes[j].Name.Equals("Producer"))
                    {
                        XmlElement Producer = (XmlElement)now.ChildNodes[j];
                        parent.illustrator = Producer.GetAttribute("illustrator");
                        parent.artistlink = Producer.GetAttribute("artistlink");
                        parent.voice = Producer.GetAttribute("voice");
                        parent.voicelink = Producer.GetAttribute("vlink");
                    }
                }
                List<Costume> costume = new List<Costume>();
                for (int j = 0; j < now.ChildNodes.Count; j++)
                {
                    if (now.ChildNodes[j].Name.Equals("Costume"))
                    {
                        XmlElement cos_node = (XmlElement)now.ChildNodes[j];
                        Costume tmp = new Costume();
                        tmp.type = cos_node.GetAttribute("type");
                        tmp.name = cos_node.GetAttribute("name");
                        costume.Add(tmp);
                    }
                }
                parent.costume = costume;

                set.Add(parent);
            }
        }
    }
    public void Get_Operater_Status(List<OperaterClass> tmp_list)
    {
        string path = pathForDocumentsFile("Resource/DB/Operator_DefaultInfo.xml");
        //string path = pathForDocumentsFile("Operator_DefaultInfo.xml");
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(path);
        //xmlDoc.Load(path);
        XmlElement dataList = xmlDoc["ChractorList"];

        for (int i = 0; i < dataList.ChildNodes.Count; i++)
        {
            XmlElement now = (XmlElement)dataList.ChildNodes[i];
            string name = now.GetAttribute("name");
            OperaterClass data = tmp_list.Find(delegate (OperaterClass a) { return a.name == name; });
            if (data != null)
            {
                Status def = new Status(int.Parse(now.GetAttribute("base_hp")), int.Parse(now.GetAttribute("base_atk")), int.Parse(now.GetAttribute("base_def")), int.Parse(now.GetAttribute("base_mdef")));
                Status max = new Status(int.Parse(now.GetAttribute("max_hp")), int.Parse(now.GetAttribute("max_atk")), int.Parse(now.GetAttribute("max_def")), int.Parse(now.GetAttribute("max_mdef")));
                Level_Info def_level = new Level_Info(int.Parse(now.GetAttribute("maxlevel")), def, max);
                def_level.redeploy = int.Parse(now.GetAttribute("redeploy"));
                def_level.cost = int.Parse(now.GetAttribute("cost"));
                def_level.block = int.Parse(now.GetAttribute("block"));
                def_level.attack_time = float.Parse(now.GetAttribute("attack_time"));
                data.Add_LevelInfo(def_level);

                data.eliteData[0].rang = int.Parse(now.GetAttribute("rang"));
            }
        }
        path = pathForDocumentsFile("Resource/DB/Operator_Elite1Info.xml");
        xmlDoc = new XmlDocument();
        xmlDoc.Load(path);
        dataList = xmlDoc["ChractorList"];
        for (int i = 0; i < dataList.ChildNodes.Count; i++)
        {
            XmlElement now = (XmlElement)dataList.ChildNodes[i];
            string name = now.GetAttribute("name");
            OperaterClass data = tmp_list.Find(delegate (OperaterClass a) { return a.name == name; });
            if (data != null)
            {
                Status def = new Status(int.Parse(now.GetAttribute("base_hp")), int.Parse(now.GetAttribute("base_atk")), int.Parse(now.GetAttribute("base_def")), int.Parse(now.GetAttribute("base_mdef")));
                Status max = new Status(int.Parse(now.GetAttribute("max_hp")), int.Parse(now.GetAttribute("max_atk")), int.Parse(now.GetAttribute("max_def")), int.Parse(now.GetAttribute("max_mdef")));
                Level_Info def_level = new Level_Info(int.Parse(now.GetAttribute("maxlevel")), def, max);
                def_level.redeploy = int.Parse(now.GetAttribute("redeploy"));
                def_level.cost = int.Parse(now.GetAttribute("cost"));
                def_level.block = int.Parse(now.GetAttribute("block"));
                def_level.attack_time = float.Parse(now.GetAttribute("attack_time"));
                data.Add_LevelInfo(def_level);

                data.eliteData[1].rang = int.Parse(now.GetAttribute("rang"));
            }
        }
        path = pathForDocumentsFile("Resource/DB/Operator_Elite2Info.xml");
        xmlDoc = new XmlDocument();
        xmlDoc.Load(path);
        dataList = xmlDoc["ChractorList"];
        for (int i = 0; i < dataList.ChildNodes.Count; i++)
        {
            XmlElement now = (XmlElement)dataList.ChildNodes[i];
            string name = now.GetAttribute("name");
            OperaterClass data = tmp_list.Find(delegate (OperaterClass a) { return a.name == name; });
            if (data != null)
            {
                Status def = new Status(int.Parse(now.GetAttribute("base_hp")), int.Parse(now.GetAttribute("base_atk")), int.Parse(now.GetAttribute("base_def")), int.Parse(now.GetAttribute("base_mdef")));
                Status max = new Status(int.Parse(now.GetAttribute("max_hp")), int.Parse(now.GetAttribute("max_atk")), int.Parse(now.GetAttribute("max_def")), int.Parse(now.GetAttribute("max_mdef")));
                Level_Info def_level = new Level_Info(int.Parse(now.GetAttribute("maxlevel")), def, max);
                def_level.redeploy = int.Parse(now.GetAttribute("redeploy"));
                def_level.cost = int.Parse(now.GetAttribute("cost"));
                def_level.block = int.Parse(now.GetAttribute("block"));
                def_level.attack_time = float.Parse(now.GetAttribute("attack_time"));
                data.Add_LevelInfo(def_level);

                data.eliteData[2].rang = int.Parse(now.GetAttribute("rang"));
            }
        }
    }
    public void Get_Operater_SkillData(List<OperaterClass> set)
    {
        string path = pathForDocumentsFile("Resource/DB/Operator_SkillInfo.xml");
        //string path = pathForDocumentsFile("Operator_SkillInfo.xml");
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(path);

        string parent = string.Empty;
        int skillcount = 0;
        for (int rare = 6; rare > 0; rare--)
        {
            XmlNodeList nodes = xmlDoc.SelectNodes("SKillList/rare_" + rare + "/Info");
            //string parent = ((XmlElement)nodes[0]).GetAttribute("OpName");
            for (int i = 0; i < nodes.Count; i++)
            {
                XmlElement now = (XmlElement)nodes[i];
                if (parent.Equals(now.GetAttribute("code")))
                    skillcount++;
                else
                {
                    parent = now.GetAttribute("code");
                    skillcount = 0;
                }
                OperaterClass cur_parent = set.Find(delegate (OperaterClass a) { return a.en_name == parent; });
                Skill tmp = new Skill(int.Parse(now.GetAttribute("id")), now.GetAttribute("name"), now.GetAttribute("type"), now.GetAttribute("tirrger"), now.GetAttribute("need"));

                tmp.rang_idx = int.Parse(now.GetAttribute("rang"));
                tmp.infom = now.GetAttribute("info").Split('/');

                List<SkillLevel> levelData = new List<SkillLevel>();
                for (int j = 0; j < now.ChildNodes.Count; j++)
                {
                    XmlElement lv = (XmlElement)now.ChildNodes[j];
                    SkillLevel nowLevel = new SkillLevel();

                    if (skillcount.Equals(0) && cur_parent != null && j < 7)
                    {
                        string[] data = lv.GetAttribute("sub").Split('/');
                        string[] count = lv.GetAttribute("subC").Split('/');
                        cur_parent.skillmaterial.Add(data);
                        cur_parent.skillmaterialCount.Add(count);
                    }
                    else
                    {
                        nowLevel.submaterial = lv.GetAttribute("sub").Split('/');
                        nowLevel.subCount = lv.GetAttribute("subC").Split('/');
                    }

                    nowLevel.sp = int.Parse(lv.GetAttribute("sp"));
                    nowLevel.startSp = int.Parse(lv.GetAttribute("fsp"));
                    nowLevel.time = lv.GetAttribute("time");

                    nowLevel.efctime = lv.GetAttribute("eft");
                    nowLevel.upgradelv = lv.GetAttribute("needLv");

                    nowLevel.rdata = lv.GetAttribute("data").Split('/');

                    levelData.Add(nowLevel);
                    //Debug.Log(lv.GetAttribute("name") + " " + lv.GetAttribute("level") + " : " + info);
                }
                tmp.SetSkillLevel(levelData);
                tmp.maxlv = tmp.data.Count;
                if (cur_parent != null)
                    cur_parent.Add_SkillData(tmp);
            }
        }
    }
    public void Get_Operatro_InfraSkill(List<OperaterClass> data)
    {
        if (data == null) return;

        string path = pathForDocumentsFile("Resource/DB/Operator_InfraSkill.xml");

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(path);

        XmlNodeList nodes = xmlDoc.SelectNodes("ChractorList/Info");

        for (int i = 0; i < nodes.Count; i++)
        {
            XmlElement now = (XmlElement)nodes[i];
            OperaterClass parent = data.Find(delegate (OperaterClass a) { return a.en_name == now.GetAttribute("en_name"); });

            List<InfraSkill> infraData = new List<InfraSkill>();
            for (int j = 0; j < now.ChildNodes.Count; j++)
            {
                XmlElement dataNode = (XmlElement)now.ChildNodes[j];

                InfraSkill tmp = new InfraSkill();
                tmp.id = int.Parse(dataNode.GetAttribute("id"));
                //tmp.icon_idx = int.Parse(dataNode.GetAttribute("icon"));
                tmp.name = dataNode.GetAttribute("name");
                tmp.unlock = dataNode.GetAttribute("lock");
                tmp.target = dataNode.GetAttribute("target");
                tmp.info = dataNode.GetAttribute("info");

                //Debug.Log(tmp.id + " Icon:" + tmp.icon_idx + " name:" + tmp.name + " lock:" + tmp.unlock + " target:" + tmp.target + " info:" + tmp.info);
                infraData.Add(tmp);
            }
            parent.infraData = infraData;
        }
    }
    public void Get_EnemyData(List<Enemy> data)
    {
        if (data == null)
            return;

        string path = pathForDocumentsFile("Resource/DB/EnemyData.xml");

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(path);

        XmlElement nodes = xmlDoc["DataList"];

        for (int i = 0; i < nodes.ChildNodes.Count; i++)
        {
            XmlElement now = (XmlElement)nodes.ChildNodes[i];
            Enemy tmp = new Enemy();

            tmp.code = now.GetAttribute("code");
            tmp.name = now.GetAttribute("name");
            tmp.position = now.GetAttribute("atktype");
            tmp.tag = now.GetAttribute("type").Split('/');

            tmp.hp = now.GetAttribute("hp").Split('/');
            tmp.atk = now.GetAttribute("atk").Split('/');
            tmp.def = now.GetAttribute("def").Split('/');
            tmp.mdef = now.GetAttribute("mdef").Split('/');

            tmp.atktime = now.GetAttribute("atktime");
            tmp.rang = now.GetAttribute("rang");
            tmp.weight = now.GetAttribute("weight");
            tmp.movespeed = now.GetAttribute("movespeed");

            tmp.resis = new string[2];
            tmp.resis[0] = now.GetAttribute("stunresis");
            tmp.resis[1] = now.GetAttribute("silenceresis");

            tmp.info = now.GetAttribute("info");
            tmp.skill = now.GetAttribute("skill");

            //Debug.Log(tmp.code);
            data.Add(tmp);
        }
    }
    public IEnumerator IE_Operator(List<OperaterClass> set)
    {
        if (set.Count.Equals(0)) yield break;

        string path = pathForDocumentsFile("Resource/DB/Operator_Info.xml");
        //string path = pathForDocumentsFile("Operator_Info.xml");
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(path);

        int index = 0;
        for (int rare = 6; rare > 0; rare--)
        {
            XmlNodeList nodes = xmlDoc.SelectNodes("ChractorList/rare_" + rare + "/Info");
            for (int i = 0; i < nodes.Count; i++)
            {
                XmlElement now = (XmlElement)nodes[i];
                OperaterClass parent = new OperaterClass(now.GetAttribute("name"), now.GetAttribute("gender"), now.GetAttribute("position"), now.GetAttribute("Class"), now.GetAttribute("rare"));
                parent.id = index++;
                parent.en_name = now.GetAttribute("en_name");
                parent.group = now.GetAttribute("group");
                parent.tags = now.GetAttribute("tags").Split('/');
                parent.trust = now.GetAttribute("trust").Split('/');
                parent.potential = now.GetAttribute("potential").Split('/');
                parent.specificity = now.GetAttribute("specificity");

                List<Ability> ability = new List<Ability>();
                for (int j = 0; j < now.ChildNodes.Count; j++)
                {
                    XmlElement lv = (XmlElement)now.ChildNodes[j];
                    if (lv.Name != "Ability")
                        break;
                    Ability now_av = new Ability();
                    now_av.unlock = lv.GetAttribute("unlock");
                    now_av.name = lv.GetAttribute("name");
                    now_av.info = lv.GetAttribute("info");
                    ability.Add(now_av);
                }
                parent.ability = ability;

                List<Elite> elite = new List<Elite>();
                elite.Add(new Elite());
                for (int j = 0; j < now.ChildNodes.Count; j++)
                {
                    XmlElement lv = (XmlElement)now.ChildNodes[j];
                    if (lv.Name.Equals("Elite1"))
                    {
                        Elite now_e = new Elite();
                        now_e.unlockdata = lv.GetAttribute("info").Split('/');
                        now_e.sub_mat = lv.GetAttribute("submat").Split('/');
                        elite.Add(now_e);
                    }
                    else if (lv.Name.Equals("Elite2"))
                    {
                        Elite now_e = new Elite();
                        now_e.unlockdata = lv.GetAttribute("info").Split('/');
                        now_e.sub_mat = lv.GetAttribute("submat").Split('/');
                        elite.Add(now_e);
                    }
                    else continue;
                }
                parent.eliteData = elite;

                for (int j = 0; j < now.ChildNodes.Count; j++)
                {
                    if (now.ChildNodes[j].Name.Equals("Producer"))
                    {
                        XmlElement Producer = (XmlElement)now.ChildNodes[j];
                        parent.illustrator = Producer.GetAttribute("illustrator");
                        parent.artistlink = Producer.GetAttribute("artistlink");
                        //Debug.Log(parent.illustrator);
                    }
                }
                set.Add(parent);
            }
        }

        yield return null;
    }
    public IEnumerator IE_Operater_Status(List<OperaterClass> set)
    {
        if (set.Count.Equals(0)) yield break;

        string path = pathForDocumentsFile("Resource/DB/Operator_DefaultInfo.xml");
        FileInfo fileInfo = new FileInfo(path);
        if (!fileInfo.Exists) yield break;
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(path);
        XmlElement dataList = xmlDoc["ChractorList"];
        for (int i = 0; i < dataList.ChildNodes.Count; i++)
        {
            XmlElement now = (XmlElement)dataList.ChildNodes[i];
            string name = now.GetAttribute("name");
            OperaterClass data = set.Find(delegate (OperaterClass a) { return a.name == name; });
            if (data != null)
            {
                Status def = new Status(int.Parse(now.GetAttribute("base_hp")), int.Parse(now.GetAttribute("base_atk")), int.Parse(now.GetAttribute("base_def")), int.Parse(now.GetAttribute("base_mdef")));
                Status max = new Status(int.Parse(now.GetAttribute("max_hp")), int.Parse(now.GetAttribute("max_atk")), int.Parse(now.GetAttribute("max_def")), int.Parse(now.GetAttribute("max_mdef")));
                Level_Info def_level = new Level_Info(int.Parse(now.GetAttribute("maxlevel")), def, max);
                def_level.redeploy = int.Parse(now.GetAttribute("redeploy"));
                def_level.cost = int.Parse(now.GetAttribute("cost"));
                def_level.block = int.Parse(now.GetAttribute("block"));
                def_level.attack_time = float.Parse(now.GetAttribute("attack_time"));
                data.Add_LevelInfo(def_level);

                data.eliteData[0].rang = int.Parse(now.GetAttribute("rang"));
            }
        }
        // Operator Elite1 =========================================================================
        path = pathForDocumentsFile("Resource/DB/Operator_Elite1Info.xml");
        fileInfo = new FileInfo(path);
        if (!fileInfo.Exists) yield break;
        xmlDoc = new XmlDocument();
        xmlDoc.Load(path);
        dataList = xmlDoc["ChractorList"];
        for (int i = 0; i < dataList.ChildNodes.Count; i++)
        {
            XmlElement now = (XmlElement)dataList.ChildNodes[i];
            string name = now.GetAttribute("name");
            OperaterClass data = set.Find(delegate (OperaterClass a) { return a.name == name; });
            if (data != null)
            {
                Status def = new Status(int.Parse(now.GetAttribute("base_hp")), int.Parse(now.GetAttribute("base_atk")), int.Parse(now.GetAttribute("base_def")), int.Parse(now.GetAttribute("base_mdef")));
                Status max = new Status(int.Parse(now.GetAttribute("max_hp")), int.Parse(now.GetAttribute("max_atk")), int.Parse(now.GetAttribute("max_def")), int.Parse(now.GetAttribute("max_mdef")));
                Level_Info def_level = new Level_Info(int.Parse(now.GetAttribute("maxlevel")), def, max);
                def_level.redeploy = int.Parse(now.GetAttribute("redeploy"));
                def_level.cost = int.Parse(now.GetAttribute("cost"));
                def_level.block = int.Parse(now.GetAttribute("block"));
                def_level.attack_time = float.Parse(now.GetAttribute("attack_time"));
                data.Add_LevelInfo(def_level);

                data.eliteData[1].rang = int.Parse(now.GetAttribute("rang"));
            }
        }
        // Operator Elite2 =========================================================================
        path = pathForDocumentsFile("Resource/DB/Operator_Elite2Info.xml");
        fileInfo = new FileInfo(path);
        if (!fileInfo.Exists) yield break;
        xmlDoc = new XmlDocument();
        xmlDoc.Load(path);
        dataList = xmlDoc["ChractorList"];
        for (int i = 0; i < dataList.ChildNodes.Count; i++)
        {
            XmlElement now = (XmlElement)dataList.ChildNodes[i];
            string name = now.GetAttribute("name");
            OperaterClass data = set.Find(delegate (OperaterClass a) { return a.name == name; });
            if (data != null)
            {
                Status def = new Status(int.Parse(now.GetAttribute("base_hp")), int.Parse(now.GetAttribute("base_atk")), int.Parse(now.GetAttribute("base_def")), int.Parse(now.GetAttribute("base_mdef")));
                Status max = new Status(int.Parse(now.GetAttribute("max_hp")), int.Parse(now.GetAttribute("max_atk")), int.Parse(now.GetAttribute("max_def")), int.Parse(now.GetAttribute("max_mdef")));
                Level_Info def_level = new Level_Info(int.Parse(now.GetAttribute("maxlevel")), def, max);
                def_level.redeploy = int.Parse(now.GetAttribute("redeploy"));
                def_level.cost = int.Parse(now.GetAttribute("cost"));
                def_level.block = int.Parse(now.GetAttribute("block"));
                def_level.attack_time = float.Parse(now.GetAttribute("attack_time"));
                data.Add_LevelInfo(def_level);

                data.eliteData[2].rang = int.Parse(now.GetAttribute("rang"));
            }
        }
        yield return null;
    }
    public IEnumerator IE_Operater_SkillData(List<OperaterClass> set)
    {
        if (set.Count.Equals(0)) yield break;

        string path = pathForDocumentsFile("Resource/DB/Operator_SkillInfo.xml");
        FileInfo fileInfo = new FileInfo(path);
        if (!fileInfo.Exists) yield break;
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(path);

        string parent = string.Empty;
        int skillcount = 0;
        for (int rare = 6; rare > 0; rare--)
        {
            XmlNodeList nodes = xmlDoc.SelectNodes("SKillList/rare_" + rare + "/Info");
            //string parent = ((XmlElement)nodes[0]).GetAttribute("OpName");
            for (int i = 0; i < nodes.Count; i++)
            {
                XmlElement now = (XmlElement)nodes[i];
                if (parent.Equals(now.GetAttribute("code")))
                    skillcount++;
                else
                {
                    parent = now.GetAttribute("code");
                    skillcount = 0;
                }
                OperaterClass cur_parent = set.Find(delegate (OperaterClass a) { return a.en_name == parent; });
                Skill tmp = new Skill(int.Parse(now.GetAttribute("id")), now.GetAttribute("name"), now.GetAttribute("type"), now.GetAttribute("tirrger"), now.GetAttribute("need"));

                tmp.rang_idx = int.Parse(now.GetAttribute("rang"));
                tmp.infom = now.GetAttribute("info").Split('/');

                List<SkillLevel> levelData = new List<SkillLevel>();
                for (int j = 0; j < now.ChildNodes.Count; j++)
                {
                    XmlElement lv = (XmlElement)now.ChildNodes[j];
                    SkillLevel nowLevel = new SkillLevel();

                    if (skillcount.Equals(0) && cur_parent != null && j < 7)
                    {
                        string[] data = lv.GetAttribute("sub").Split('/');
                        string[] count = lv.GetAttribute("subC").Split('/');
                        cur_parent.skillmaterial.Add(data);
                        cur_parent.skillmaterialCount.Add(count);
                    }
                    else
                    {
                        nowLevel.submaterial = lv.GetAttribute("sub").Split('/');
                        nowLevel.subCount = lv.GetAttribute("subC").Split('/');
                    }

                    nowLevel.sp = int.Parse(lv.GetAttribute("sp"));
                    nowLevel.startSp = int.Parse(lv.GetAttribute("fsp"));
                    nowLevel.time = lv.GetAttribute("time");

                    nowLevel.efctime = lv.GetAttribute("eft");
                    nowLevel.upgradelv = lv.GetAttribute("needLv");

                    nowLevel.rdata = lv.GetAttribute("data").Split('/');

                    levelData.Add(nowLevel);
                    //Debug.Log(lv.GetAttribute("name") + " " + lv.GetAttribute("level") + " : " + info);
                }
                tmp.SetSkillLevel(levelData);
                tmp.maxlv = tmp.data.Count;
                if (cur_parent != null)
                    cur_parent.Add_SkillData(tmp);
            }
        }

        yield return null;
    }
    public IEnumerator IE_Operatro_InfraSkill(List<OperaterClass> set)
    {
        if (set.Count.Equals(0)) yield break;

        string path = pathForDocumentsFile("Resource/DB/Operator_InfraSkill.xml");
        FileInfo fileInfo = new FileInfo(path);
        if (!fileInfo.Exists) yield break;
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(path);

        XmlNodeList nodes = xmlDoc.SelectNodes("ChractorList/Info");

        for (int i = 0; i < nodes.Count; i++)
        {
            XmlElement now = (XmlElement)nodes[i];
            OperaterClass parent = set.Find(delegate (OperaterClass a) { return a.en_name == now.GetAttribute("en_name"); });

            List<InfraSkill> infraData = new List<InfraSkill>();
            for (int j = 0; j < now.ChildNodes.Count; j++)
            {
                XmlElement dataNode = (XmlElement)now.ChildNodes[j];

                InfraSkill tmp = new InfraSkill();
                tmp.id = int.Parse(dataNode.GetAttribute("id"));
                //tmp.icon_idx = int.Parse(dataNode.GetAttribute("icon"));
                tmp.name = dataNode.GetAttribute("name");
                tmp.unlock = dataNode.GetAttribute("lock");
                tmp.target = dataNode.GetAttribute("target");
                tmp.info = dataNode.GetAttribute("info");

                //Debug.Log(tmp.id + " Icon:" + tmp.icon_idx + " name:" + tmp.name + " lock:" + tmp.unlock + " target:" + tmp.target + " info:" + tmp.info);
                infraData.Add(tmp);
            }
            parent.infraData = infraData;
        }

        yield return null;
    }
    public List<PO_OperatorClass> Get_PublicOfferTag(string _fileName)
    {
        List<PO_OperatorClass> tmp_list = new List<PO_OperatorClass>();

        TextAsset txtAsset = (TextAsset)Resources.Load("XML/" + _fileName);
        Debug.Log(txtAsset.text);

        XmlDocument xmlDoc = new XmlDocument();
        // XML 로드하고.
        xmlDoc.LoadXml(txtAsset.text);
        XmlElement OperatorListElement = xmlDoc["ChractorList"];

        for (int i = 0; i < OperatorListElement.ChildNodes.Count; i++)
        {
            XmlElement now = (XmlElement)OperatorListElement.ChildNodes[i];

            PO_OperatorClass tmp;
            tmp = new PO_OperatorClass(i, now.GetAttribute("name"), now.GetAttribute("rare"), now.GetAttribute("quality"),
               now.GetAttribute("gender"), now.GetAttribute("Class"), now.GetAttribute("position"));

            tmp.tag = now.GetAttribute("tag").Split('/');
            //Debug.Log(tmp.name + ": " + tmp.tag.Length);
            tmp.set_record(now.GetAttribute("record"));
            tmp_list.Add(tmp);
        }
        return tmp_list;
    }
    public void Get_OperatorSpineAssetList_F(List<OperatorSpine> list)
    {
        string path = Files.Use.DocumentsPath("Resource/DB/spine_assetlist_F.xml");

        FileInfo fileInfo = new FileInfo(path);
        if (!fileInfo.Exists)
        {
            list.Clear();
            return;
        }

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(path);
        XmlElement xmlElement = xmlDoc["data"];
        for (int i = 0; i < xmlElement.ChildNodes.Count; i++)
        {
            OperatorSpine newClass = new OperatorSpine();
            XmlElement curNode = (XmlElement)xmlElement.ChildNodes[i];
            newClass.name = curNode.GetAttribute("name");
            newClass.parent = curNode.GetAttribute("operator");
            newClass.code = curNode.GetAttribute("code");
            newClass.made = curNode.GetAttribute("made");

            float size = float.Parse(curNode.GetAttribute("size"));
            Vector2 scale = new Vector2(size, size);
            newClass.scale = scale;

            string[] pos = curNode.GetAttribute("position").Split(',');
            float posX = float.Parse(pos[0]);
            float posY = float.Parse(pos[1]);
            Vector2 position = new Vector2(posX, posY);
            newClass.position = position;

            XmlElement curChNode = (XmlElement)curNode.ChildNodes[0];
            newClass.assetbundleName = curChNode.GetAttribute("assetbundle");
            newClass.assetlavelName = curChNode.GetAttribute("assetlavel");

            curChNode = (XmlElement)curNode.ChildNodes[1];
            newClass.Yurl = curChNode.GetAttribute("youtube");
            list.Add(newClass);
        }
    }
    public string[] getOperatorLiveServerData(string code)
    {
        string path = Files.Use.DocumentsPath("Resource/DB/liveserver_data_all.xml");

        FileInfo fileInfo = new FileInfo(path);
        if (!fileInfo.Exists)
            return null;

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(path);
        XmlElement xmlElement = xmlDoc["data"];

        XmlElement operatorNode = (XmlElement)xmlElement.ChildNodes[0];
        XmlElement contryNode = null;
        if (code.Equals("KR"))
            contryNode = (XmlElement)operatorNode.ChildNodes[0];
        else if (code.Equals("EN"))
            contryNode = (XmlElement)operatorNode.ChildNodes[1];
        else if (code.Equals("CN"))
            contryNode = (XmlElement)operatorNode.ChildNodes[2];
        else if (code.Equals("JP"))
            contryNode = (XmlElement)operatorNode.ChildNodes[3];

        return contryNode.GetAttribute("data").Split(',');
    }
    public List<string[]> getStageDropDataFromFile(string code)
    {
        string path = Files.Use.DocumentsPath("Resource/DB/stage_dropdata.xml");

        FileInfo fileInfo = new FileInfo(path);
        if (!fileInfo.Exists)
            return null;

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(path);
        XmlElement xmlElement = xmlDoc["data"];

        bool flag = false;
        string[] dropMaterial = null;
        string[] dropCount = null;
        string[] tryCount = null;
        for (int i = 0; i < xmlElement.ChildNodes.Count; i++)
        {
            XmlElement curNode = (XmlElement)xmlElement.ChildNodes[i];
            if (!curNode.GetAttribute("code").Equals(code))
                continue;

            dropMaterial = curNode.GetAttribute("material").Split('/');
            dropCount = curNode.GetAttribute("get").Split('/');
            tryCount = curNode.GetAttribute("try").Split('/');
            flag = true;
        }
        if (flag)
        {
            List<string[]> res = new List<string[]>();
            for (int i = 0; i < dropMaterial.Length; i++)
            {
                string[] line = new string[3];
                line[0] = dropMaterial[i];
                if (!dropCount[0].Equals(string.Empty)) line[1] = dropCount[i];
                else line[1] = "0";
                if (!tryCount[0].Equals(string.Empty)) line[2] = tryCount[i];
                else line[2] = "0";
                //Debug.Log(line[1] + line[2]);
                res.Add(line);
            }
            return res;
        }
        else return null;
    }
    public Dictionary<string, string> get_profileData_from_stringFile(string data)
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(data);
        XmlElement xmlElement = xmlDoc["data"];

        Dictionary<string, string> tmp = new Dictionary<string, string>();
        for (int i = 0; i < xmlElement.ChildNodes.Count; i++)
        {
            XmlElement curNode = (XmlElement)xmlElement.ChildNodes[i];

            string key = curNode.Name;
            string info = curNode.GetAttribute("data");

            tmp.Add(key, info);
        }

        return tmp;
    }
    // ==========================================================================
    public int Get_AudioData(string code, List<Audio> audio)
    {
        if (code == string.Empty)
            return -1;

        string path = pathForDocumentsFile(string.Format("Resource/DB/operator/{0}/{0}_Audio.xml", code));
        string directoryPath = pathForDocumentsFile(string.Format("Resource/DB/operator/{0}", code));

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
            return -1;
        }
        FileInfo fileInfo = new FileInfo(path);
        if (!fileInfo.Exists)
            return -1;

        Debug.Log(code);
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(path);
        XmlElement nodes = xmlDoc["DataList"];

        for (int i = 0; i < nodes.ChildNodes.Count; i++)
        {
            XmlElement infoNode = (XmlElement)nodes.ChildNodes[i];

            for (int j = 0; j < infoNode.ChildNodes.Count; j++)
            {
                XmlElement dataNode = (XmlElement)infoNode.ChildNodes[j];
                Audio tmp = new Audio();
                tmp.code = dataNode.GetAttribute("code");
                tmp.name = dataNode.GetAttribute("name");
                tmp.info = dataNode.GetAttribute("info");
                audio.Add(tmp);
            }
        }

        return 0;
    }
    public int Get_AudioData_Old(string code, List<Audio> audio)
    {
        if (code == string.Empty)
            return -1;

        string path = pathForDocumentsFile(string.Format("Resource/OperatorAudio/{0}/{0}_Audio.xml", code));
        string directoryPath = pathForDocumentsFile(string.Format("Resource/OperatorAudio/{0}", code));

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
            return -1;
        }
        FileInfo fileInfo = new FileInfo(path);
        if (!fileInfo.Exists)
            return -1;

        Debug.Log(code);
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(path);
        XmlElement nodes = xmlDoc["DataList"];

        for (int i = 0; i < nodes.ChildNodes.Count; i++)
        {
            XmlElement infoNode = (XmlElement)nodes.ChildNodes[i];

            for (int j = 0; j < infoNode.ChildNodes.Count; j++)
            {
                XmlElement dataNode = (XmlElement)infoNode.ChildNodes[j];
                Audio tmp = new Audio();
                tmp.code = dataNode.GetAttribute("code");
                tmp.name = dataNode.GetAttribute("name");
                tmp.info = dataNode.GetAttribute("info");
                audio.Add(tmp);
            }
        }

        return 0;
    }
    // ==========================================================================
    public void Get_MaterialData(List<EliteMaterial> data)
    {
        TextAsset txtAsset = (TextAsset)Resources.Load("XML/MaterialData");
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(txtAsset.text);
        XmlElement nodelist = xmlDoc["MaterialList"];

        for (int i = 0; i < nodelist.ChildNodes.Count; i++)
        {
            XmlElement now = (XmlElement)nodelist.ChildNodes[i];
            EliteMaterial tmp = new EliteMaterial();
            tmp.id = int.Parse(now.GetAttribute("id"));
            tmp.code = now.GetAttribute("code");
            tmp.name = now.GetAttribute("name");
            tmp.rare = now.GetAttribute("rare");
            tmp.info = now.GetAttribute("info");
            tmp.record = now.GetAttribute("record");

            tmp.dropstage = now.GetAttribute("drop").Split('/');
            tmp.dropPer = now.GetAttribute("dropPer").Split('/');
            tmp.submaterial = now.GetAttribute("sub").Split('/');
            tmp.subCount = now.GetAttribute("count").Split('/');

            data.Add(tmp);
        }
    }
    public void get_material_xml(List<EliteMaterial> data)
    {
        string path = Files.Use.DocumentsPath("Resource/DB/material_data_1.0.0.xml");

        FileInfo fileInfo = new FileInfo(path);
        if (!fileInfo.Exists)
            return;

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(path);
        XmlElement nodelist = xmlDoc["data"];
        nodelist = nodelist["event"];

        for (int i = 0; i < nodelist.ChildNodes.Count; i++)
        {
            XmlElement now = (XmlElement)nodelist.ChildNodes[i];
            EliteMaterial tmp = new EliteMaterial();
            tmp.id = i;
            tmp.code = now.GetAttribute("code");
            tmp.name = now.GetAttribute("name");
            tmp.rare = now.GetAttribute("rare");
            tmp.info = now.GetAttribute("info");
            tmp.record = now.GetAttribute("record");

            data.Add(tmp);
        }
    }
    public void Get_MaterialChipData(List<MaterialChip> target)
    {
        TextAsset txtAsset = (TextAsset)Resources.Load("XML/" + "MaterialChipData");
        //Debug.Log(txtAsset.text);

        XmlDocument xmlDoc = new XmlDocument();
        // XML 로드하고.
        xmlDoc.LoadXml(txtAsset.text);
        XmlElement nodes = xmlDoc["MaterialList"];

        for (int i = 0; i < nodes.ChildNodes.Count; i++)
        {
            XmlElement node = (XmlElement)nodes.ChildNodes[i];
            MaterialChip tmp = new MaterialChip();
            tmp.id = int.Parse(node.GetAttribute("id"));
            tmp.code = node.GetAttribute("code");
            tmp.name = node.GetAttribute("name");
            tmp.rare = node.GetAttribute("rare");

            target.Add(tmp);
        }
    }
    public IEnumerator IE_MaterialData(List<EliteMaterial> data)
    {
        TextAsset txtAsset = (TextAsset)Resources.Load("XML/MaterialData");
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(txtAsset.text);
        XmlElement nodelist = xmlDoc["MaterialList"];

        for (int i = 0; i < nodelist.ChildNodes.Count; i++)
        {
            XmlElement now = (XmlElement)nodelist.ChildNodes[i];
            EliteMaterial tmp = new EliteMaterial();
            tmp.id = int.Parse(now.GetAttribute("id"));
            tmp.code = now.GetAttribute("code");
            tmp.name = now.GetAttribute("name");
            tmp.rare = now.GetAttribute("rare");
            tmp.info = now.GetAttribute("info");
            tmp.record = now.GetAttribute("record");

            tmp.dropstage = now.GetAttribute("drop").Split('/');
            tmp.dropPer = now.GetAttribute("dropPer").Split('/');
            tmp.submaterial = now.GetAttribute("sub").Split('/');
            tmp.subCount = now.GetAttribute("count").Split('/');

            data.Add(tmp);
        }
        yield return null;
    }
    // ==========================================================================
    public string[] Get_UrlData()
    {
        string path = pathForDocumentsFile("Resource/DB/URL.xml");
        FileInfo fileInfo = new FileInfo(path);
        if (!fileInfo.Exists)
            return null;

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(path);

        XmlElement nodelist = xmlDoc["UrlList"];

        string[] data = new string[nodelist.ChildNodes.Count];
        for (int i = 0; i < nodelist.ChildNodes.Count; i++)
        {
            XmlElement node = (XmlElement)nodelist.ChildNodes[i];
            data[i] = node.GetAttribute("pin");
        }
        return data;
    }
    public void Get_StageData(List<StageList> data)
    {
        string path = pathForDocumentsFile("Resource/DB/StageData.xml");

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(path);

        XmlElement nodelist = xmlDoc["StageList"];

        for (int i = 0; i < nodelist.ChildNodes.Count; i++)
        {
            XmlElement now = (XmlElement)nodelist.ChildNodes[i];
            if (now.Name.Equals("Stage"))
            {
                StageList tmp = new StageList();
                tmp.name = now.GetAttribute("name");
                tmp.code = now.GetAttribute("code");
                data.Add(tmp);
            }
            else
            {
                StageList parent = data.Find(delegate (StageList a) { return a.code == now.GetAttribute("parent"); });
                Stage tmp = new Stage();
                tmp.code = now.GetAttribute("code");
                tmp.name = now.GetAttribute("name");
                tmp.hard = now.GetAttribute("hard");
                tmp.type = int.Parse(now.GetAttribute("type"));

                tmp.ndrop = now.GetAttribute("nd").Split('/');
                tmp.ndropPer = now.GetAttribute("ndp").Split('/');
                tmp.sdrop = now.GetAttribute("sd").Split('/');
                tmp.sdropPer = now.GetAttribute("sdp").Split('/');
                tmp.adrop = now.GetAttribute("ad").Split('/');
                tmp.adropPer = now.GetAttribute("adp").Split('/');

                for (int n = 0; n < now.ChildNodes.Count; n++)
                {
                    XmlElement node = (XmlElement)now.ChildNodes[n];
                    if (node == null)
                        continue;

                    string[] oper = node.GetAttribute("oper").Split('/');
                    string[] elite = node.GetAttribute("elite").Split('/');
                    string[] level = node.GetAttribute("lv").Split('/');
                    string link = node.GetAttribute("link");

                    tmp.atOperator.Add(oper);
                    tmp.atElite.Add(elite);
                    tmp.atLevel.Add(level);
                    tmp.link.Add(link);
                }
                parent.data.Add(tmp);
            }
        }
    }
    public List<StageList> get_event_stage_data()
    {
        string path = pathForDocumentsFile("Resource/DB/stage_event_data_1.0.0.xml");

        FileInfo fileInfo = new FileInfo(path);
        if (!fileInfo.Exists)
            return null;

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(path);

        XmlElement nodelist = xmlDoc["data"];
        nodelist = (XmlElement)nodelist["stage"];

        List<StageList> return_data = new List<StageList>();
        for (int i = 0; i < nodelist.ChildNodes.Count; i++)
        {
            XmlElement now = (XmlElement)nodelist.ChildNodes[i];
            if (now.Name.Equals("parent"))
            {
                StageList tmp = new StageList();
                tmp.name = now.GetAttribute("name");
                tmp.code = now.GetAttribute("code");
                tmp.date_start = now.GetAttribute("start");
                tmp.date_exite = now.GetAttribute("end");
                return_data.Add(tmp);
            }
            else
            {
                StageList parent = return_data.Find(delegate (StageList a) { return a.code == now.GetAttribute("parent"); });
                Stage tmp = new Stage();
                tmp.code = now.GetAttribute("code");
                tmp.name = now.GetAttribute("name");
                tmp.hard = now.GetAttribute("hard");
                tmp.type = 0;

                tmp.ndrop = now.GetAttribute("nd").Split('/');
                tmp.ndropPer = now.GetAttribute("ndp").Split('/');
                tmp.sdrop = now.GetAttribute("sd").Split('/');
                tmp.sdropPer = now.GetAttribute("sdp").Split('/');
                tmp.adrop = now.GetAttribute("ad").Split('/');
                tmp.adropPer = now.GetAttribute("adp").Split('/');

                for (int n = 0; n < now.ChildNodes.Count; n++)
                {
                    XmlElement node = (XmlElement)now.ChildNodes[n];
                    if (node == null)
                        continue;

                    string[] oper = node.GetAttribute("oper").Split('/');
                    string[] elite = node.GetAttribute("elite").Split('/');
                    string[] level = node.GetAttribute("lv").Split('/');
                    string link = node.GetAttribute("link");

                    tmp.atOperator.Add(oper);
                    tmp.atElite.Add(elite);
                    tmp.atLevel.Add(level);
                    tmp.link.Add(link);
                }
                parent.data.Add(tmp);
            }
        }

        return return_data;
    }
    public List<string[]> get_stage_dropLimit(string code)
    {
        string path = pathForDocumentsFile("Resource/DB/stage_dropdata_limit_1.0.0.xml");

        FileInfo fileInfo = new FileInfo(path);
        if (!fileInfo.Exists)
            return null;

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(path);

        XmlElement nodelist = xmlDoc["data"];
        //nodelist = (XmlElement)nodelist["stage"];

        List<string[]> return_data = new List<string[]>();
        for (int i = 0; i < nodelist.ChildNodes.Count; i++)
        {
            XmlElement now = (XmlElement)nodelist.ChildNodes[i];
            if(now.GetAttribute("code").Equals(code))
            {
                string[] lock_where = now.GetAttribute("where").Split('/');
                string[] lock_material = now.GetAttribute("lock").Split('/');
                string[] lock_count = now.GetAttribute("lock_count").Split('/');

                for(int j = 0; j < lock_material.Length; j++)
                {
                    string[] material = new string[3];
                    material[0] = lock_where[j];
                    material[1] = lock_material[j];
                    material[2] = lock_count[j];

                    return_data.Add(material);
                }
            }
        }

        return return_data;
    }
    // ==========================================================================
    public List<IconFile> Get_FileCheck_Icon()
    {
        List<IconFile> data = new List<IconFile>();

        string path = pathForDocumentsFile("Version_Info/OperatorIcon.xml");

        FileInfo fileInfo = new FileInfo(path);
        if (!fileInfo.Exists) return data;

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(path);
        //xmlDoc.Load(path);
        XmlElement OperatorListElement = xmlDoc["ChractorList"];
        foreach (XmlElement set in OperatorListElement.ChildNodes)
        {
            IconFile _new = new IconFile();
            _new.en_name = set.GetAttribute("en_name");
            _new.icon = 0;
            _new.rare = set.GetAttribute("rare");

            if (set.GetAttribute("skill_icon").Equals(""))
                _new.skill = new string[0];
            else
                _new.skill = set.GetAttribute("skill_icon").Split('/');

            data.Add(_new);
        }

        return data;
    }
    public List<FILEInfo> Get_FileInfo(string fileName)
    {
        List<FILEInfo> data = new List<FILEInfo>();

        string path = pathForDocumentsFile("Version_Info/" + fileName);

        FileInfo fileInfo = new FileInfo(path);
        if (!fileInfo.Exists) return data;

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(path);
        XmlElement Filelist = xmlDoc["FileList"];
        foreach (XmlElement set in Filelist.ChildNodes)
        {
            FILEInfo newfile = new FILEInfo();
            newfile.name = set.GetAttribute("name");
            newfile.compulsion = set.GetAttribute("compulsion");
            newfile.fileCount = set.GetAttribute("fileCount");

            newfile.fileName = set.GetAttribute("fileName");
            newfile.folder = set.GetAttribute("folder");

            data.Add(newfile);
        }

        return data;
    }
    // ==========================================================================
    public void Get_VersionInfo(DataBaseSetting set)
    {
        if (Version.Use == null)
        {
            set.resources_ver = "NULL";
            set.audio_ver = "NULL";
            set.apk_ver = "NULL";
            set.DB_ver = "NULL";

            set.apklink = "NULL";
            set.notice = "인터넷 연결을 확인해 주세요.";
            return;
        }

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(Version.Use.versionXML);
        XmlElement xmlElement = xmlDoc["VersionList"];

        set.resources_ver = ((XmlElement)xmlElement.ChildNodes[0]).GetAttribute("resources_ver");
        set.audio_ver = ((XmlElement)xmlElement.ChildNodes[0]).GetAttribute("audio_ver");
        set.apk_ver = ((XmlElement)xmlElement.ChildNodes[0]).GetAttribute("apk_ver");
        set.DB_ver = ((XmlElement)xmlElement.ChildNodes[0]).GetAttribute("DB_ver");
        set.stagedropDB_ver = string.Empty;
        set.stagedropDB_ver = ((XmlElement)xmlElement.ChildNodes[0]).GetAttribute("dropstatis_ver");

        set.apklink = ((XmlElement)xmlElement.ChildNodes[0]).GetAttribute("link");
        set.notice = ((XmlElement)xmlElement.ChildNodes[1]).GetAttribute("info");

        if (PlayerPrefs.HasKey("Version").Equals(false))
        {
            PlayerPrefs.SetInt("Version", 0);
            PlayerPrefs.Save();
        }
    }
    public bool GetUserData(UserCache _user)
    {
        if (new FileInfo(pathForDocumentsFile("cache/user.xml")).Exists.Equals(false))
            return false;

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(pathForDocumentsFile("cache/user.xml"));
        XmlElement xmlElement = xmlDoc["data"];

        _user.Set(((XmlElement)xmlElement.ChildNodes[0]).GetAttribute("u"), ((XmlElement)xmlElement.ChildNodes[0]).GetAttribute("p"),
            ((XmlElement)xmlElement.ChildNodes[0]).GetAttribute("k"), ((XmlElement)xmlElement.ChildNodes[0]).GetAttribute("s"));
        return true;
    }
    public string GetCache_A(string key)
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(Version.Use.cacheXML);
        XmlElement xmlElement = xmlDoc["data"];

        return ((XmlElement)xmlElement.ChildNodes[0]).GetAttribute(key);
    }
    public string GetCache_B(string key)
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(Version.Use.cacheXML);
        XmlElement xmlElement = xmlDoc["data"];

        return ((XmlElement)xmlElement.ChildNodes[1]).GetAttribute(key);
    }
    public int GetStatus(string _data, string _key)
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(_data);
        XmlElement xmlElement = xmlDoc["data"];

        return int.Parse(((XmlElement)xmlElement.ChildNodes[0]).GetAttribute(_key));
    }
    public int GetGitMaxCount(string _data, string _operator)
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(_data);
        XmlElement xmlElement = xmlDoc["data"];

        string data = ((XmlElement)xmlElement.ChildNodes[1]).GetAttribute(_operator);
        if (!data.Equals(string.Empty))
            return int.Parse(data);
        else return 0;
    }
    public void GetCommentGitHub(string _data, CommentList _set)
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(_data);
        XmlElement xmlElement = xmlDoc["data"];
        _set.data.Clear();
        for (int i = 0; i < xmlElement.ChildNodes.Count; i++)
        {
            XmlElement curNode = (XmlElement)xmlElement.ChildNodes[i];
            Comment tmp = new Comment();
            
            tmp.localId = curNode.GetAttribute("localId");
            tmp.userName = curNode.GetAttribute("userName");
            tmp.info = curNode.GetAttribute("info");
            tmp.date = curNode.GetAttribute("date");
            _set.data.Add(curNode.GetAttribute("key"), tmp);
        }
    }
    // ==========================================================================
    public void Get_EventData(List<EventOffer> set)
    {
        if (set == null) return;

        string path = pathForDocumentsFile("Resource/DB/Event_DW.xml");
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(path);

        XmlNodeList nodes = xmlDoc.SelectNodes("ChractorList/Event");
        for (int i = 0; i < nodes.Count; i++)
        {
            XmlElement now = (XmlElement)nodes[i];
            EventOffer tmp = new EventOffer();

            XmlElement cur = (XmlElement)now.ChildNodes[0];
            tmp.endData = cur.GetAttribute("end");
            tmp.date = cur.GetAttribute("date");
            tmp.fileName = cur.GetAttribute("fileName");

            string[] per = cur.GetAttribute("per").Split('/');
            tmp.per = new int[per.Length];
            for (int p = 0; p < per.Length; p++)
                tmp.per[p] = int.Parse(per[p]);

            cur = (XmlElement)now.ChildNodes[1];
            tmp.rare6_up = cur.GetAttribute("rare6").Split('/');
            tmp.rare5_up = cur.GetAttribute("rare5").Split('/');

            cur = (XmlElement)now.ChildNodes[2];
            tmp.rare6 = cur.GetAttribute("rare6").Split('/');
            tmp.rare5 = cur.GetAttribute("rare5").Split('/');
            tmp.rare4 = cur.GetAttribute("rare4").Split('/');
            tmp.rare3 = cur.GetAttribute("rare3").Split('/');

            set.Add(tmp);
        }
    }
    // Save XML ==========================================================================
    public void SaveUserCache(string _path, string _filename, string _uid, string _pin, string _key, string status)
    {
        XmlDocument xmlDoc = new XmlDocument();
        XmlElement xmlElement = xmlDoc.CreateElement("data");
        xmlDoc.AppendChild(xmlElement);


        XmlElement setElement = xmlDoc.CreateElement("usercache");
        setElement.SetAttribute("u", _uid);
        setElement.SetAttribute("p", _pin);
        setElement.SetAttribute("k", _key);
        setElement.SetAttribute("s", status);
        xmlElement.AppendChild(setElement);

        if (!Directory.Exists(_path))
            Directory.CreateDirectory(_path);

        xmlDoc.Save(_path + "/" + _filename);
    }
    public void SaveXML(List<OperaterClass> data, string filename, string path)
    {
        XmlDocument xmlDoc = new XmlDocument();
        XmlElement OperatorListElement = xmlDoc.CreateElement("FileList");
        xmlDoc.AppendChild(OperatorListElement);

        foreach (OperaterClass set in data)
        {
            XmlElement SetElement = xmlDoc.CreateElement("Info");
            SetElement.SetAttribute("folder", set.en_name);
            SetElement.SetAttribute("fileName", "Icon.png");
            OperatorListElement.AppendChild(SetElement);

            for (int i = 0; i < set.skillData.Count; i++)
            {
                XmlElement skillFile = xmlDoc.CreateElement("Info");
                skillFile.SetAttribute("folder", set.en_name);
                skillFile.SetAttribute("fileName", "S" + i.ToString() + ".png");
                OperatorListElement.AppendChild(skillFile);

                if (!set.skillData[i].rang_idx.Equals(0))
                {
                    XmlElement rangFile = xmlDoc.CreateElement("Info");
                    rangFile.SetAttribute("folder", set.en_name);
                    rangFile.SetAttribute("fileName", "S" + i.ToString() + "_R" + ".png");
                    OperatorListElement.AppendChild(rangFile);
                }
            }
        }

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        xmlDoc.Save(path + filename);
    }
    public void SaveIllustFileList(List<OperaterClass> data, string filename, string path)
    {
        XmlDocument xmlDoc = new XmlDocument();
        XmlElement OperatorListElement = xmlDoc.CreateElement("FileList");
        xmlDoc.AppendChild(OperatorListElement);

        for (int i = 0; i < data.Count; i++)
        {
            OperaterClass curData = data[i];
            XmlElement SetElement = xmlDoc.CreateElement("Info");
            SetElement.SetAttribute("name", curData.en_name + ".png");
            SetElement.SetAttribute("compulsion", "0");
            OperatorListElement.AppendChild(SetElement);
            if(curData.eliteData.Count >= 3)
            {
                XmlElement eliteIllust = xmlDoc.CreateElement("Info");
                eliteIllust.SetAttribute("name", curData.en_name + "_elite2.png");
                eliteIllust.SetAttribute("compulsion", "0");
                OperatorListElement.AppendChild(eliteIllust);
            }
            for (int j = 0; j < curData.costume.Count; j++)
            {
                XmlElement costumeIllust = xmlDoc.CreateElement("Info");
                costumeIllust.SetAttribute("name", curData.en_name + "_c" + (j + 1).ToString() + ".png");
                costumeIllust.SetAttribute("compulsion", "0");
                OperatorListElement.AppendChild(costumeIllust);
            }
        }

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        xmlDoc.Save(path + filename);
    }
    public void SaveXML_AudioFile(List<OperaterClass> data, string filename, string path)
    {
        XmlDocument xmlDoc = new XmlDocument();
        XmlElement OperatorListElement = xmlDoc.CreateElement("FileList");
        xmlDoc.AppendChild(OperatorListElement);

        for (int i = 0; i < data.Count; i++)
        {
            List<Audio> tmp = new List<Audio>();
            if (Get_AudioData(data[i].en_name, tmp).Equals(-1))
            {
                continue;
            }

            XmlElement SetElement = xmlDoc.CreateElement("Info");
            SetElement.SetAttribute("folder", data[i].en_name);
            SetElement.SetAttribute("fileName", data[i].en_name + "_Audio.xml");
            OperatorListElement.AppendChild(SetElement);

            for (int j = 0; j < tmp.Count; j++)
            {
                XmlElement node = xmlDoc.CreateElement("Info");
                node.SetAttribute("folder", data[i].en_name);
                node.SetAttribute("fileName", tmp[j].code + ".mp3");
                OperatorListElement.AppendChild(node);
            }
        }

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        xmlDoc.Save(path + filename);
    }
    public void SaveDB_Audio(List<OperaterClass> data, string filename, string path)
    {
        XmlDocument xmlDoc = new XmlDocument();
        XmlElement OperatorListElement = xmlDoc.CreateElement("FileList");
        xmlDoc.AppendChild(OperatorListElement);

        for (int i = 0; i < data.Count; i++)
        {
            List<Audio> tmp = new List<Audio>();
            if (Get_AudioData_Old(data[i].en_name, tmp).Equals(-1))
            {
                continue;
            }

            XmlElement SetElement = xmlDoc.CreateElement("Info");
            SetElement.SetAttribute("folder", data[i].en_name);
            SetElement.SetAttribute("fileName", data[i].en_name + "_Audio.xml");
            SetElement.SetAttribute("compulsion", "0");
            OperatorListElement.AppendChild(SetElement);
        }

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        xmlDoc.Save(path + filename);
    }
    public bool SavePrefs_SelectMaterial(List<MaterialCount> select, List<MaterialCount> ret)
    {
        string _tmpStr = "";
        for (int i = 0; i < select.Count; i++)
        {
            _tmpStr = _tmpStr + select[i].data.id.ToString() + ",";
            _tmpStr = _tmpStr + select[i].data.name + ",";
            _tmpStr = _tmpStr + select[i].count.ToString();

            if (i < select.Count - 1)
            {
                _tmpStr = _tmpStr + ",";
            }
        }
        string retStr = "";
        for (int i = 0; i < ret.Count; i++)
        {
            retStr = retStr + ret[i].data.id.ToString() + ",";
            retStr = retStr + ret[i].data.name + ",";
            retStr = retStr + ret[i].count.ToString();

            if (i < ret.Count - 1)
            {
                retStr = retStr + ",";
            }
        }
        Debug.Log(_tmpStr);
        Debug.Log(retStr);
        PlayerPrefs.SetString("SelectMain", _tmpStr);
        PlayerPrefs.SetString("SelectRet", retStr);
        return true;
    }
    // ========================================================================== PlayerPrefs
    public string[] GetUIStatusBarXML()
    {
        string path = pathForDocumentsFile("cache/ui_set.xml");
        FileInfo fileInfo = new FileInfo(path);
        if (!fileInfo.Exists)
        {
            SetUIStatusBarXML("1,1");
            return "1,1".Split(',');
        }

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(path);
        XmlElement xmlElement = xmlDoc["data"];
        string[] data = ((XmlElement)xmlElement.ChildNodes[0]).GetAttribute("statusbar").Split(',');
        if(data.Length != 2)
        {
            SetUIStatusBarXML("1,1");
            return "1,1".Split(',');
        }
        return data;
    }
    public void SetUIStatusBarXML(string save)
    {
        string path = pathForDocumentsFile("cache");

        XmlDocument newXmlDoc = new XmlDocument();
        XmlElement newElement = newXmlDoc.CreateElement("data");
        newXmlDoc.AppendChild(newElement);

        XmlElement SetElement = newXmlDoc.CreateElement("android");
   
        SetElement.SetAttribute("statusbar", save);
        newElement.AppendChild(SetElement);

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        newXmlDoc.Save(path + "/" + "ui_set.xml");
    }
    public int getTutorialProgress()
    {
        string path = Files.Use.DocumentsPath("cache/tutorial_check.xml");

        FileInfo fileInfo = new FileInfo(path);
        if (!fileInfo.Exists)
        {
            return 0;
        }

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(path);
        XmlElement xmlElement = xmlDoc["data"];
        string data = ((XmlElement)xmlElement.ChildNodes[0]).GetAttribute("check");

        return int.Parse(data);
    }
    public void setTutorialProgress(int data)
    {
        string folderpath = pathForDocumentsFile("cache");
        string filepath = Files.Use.DocumentsPath("cache/tutorial_check.xml");

        //FileInfo fileInfo = new FileInfo(filepath);
        //if (!fileInfo.Exists)
        XmlDocument newXmlDoc = new XmlDocument();
        XmlElement newElement = newXmlDoc.CreateElement("data");
        newXmlDoc.AppendChild(newElement);

        XmlElement SetElement = newXmlDoc.CreateElement("softkey");

        SetElement.SetAttribute("check", data.ToString());
        newElement.AppendChild(SetElement);

        if (!Directory.Exists(folderpath))
            Directory.CreateDirectory(folderpath);

        newXmlDoc.Save(filepath);
    }
    public string getVersionCode(string type)
    {
        string path = Files.Use.DocumentsPath("cache/version.xml");

        FileInfo fileInfo = new FileInfo(path);
        if (!fileInfo.Exists)
        {
            setVersionCode(type, "0");
            return "0";
        }
    
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(path);
        XmlElement xmlElement = xmlDoc["data"];
        string data = ((XmlElement)xmlElement.ChildNodes[0]).GetAttribute("ver");

        return data;
    }
    public void setVersionCode(string type, string data)
    {
        string folderpath = pathForDocumentsFile("cache");
        string filepath = Files.Use.DocumentsPath("cache/version.xml");

        XmlDocument newXmlDoc = new XmlDocument();
        XmlElement newElement = newXmlDoc.CreateElement("data");
        newXmlDoc.AppendChild(newElement);

        XmlElement SetElement = newXmlDoc.CreateElement("stagedropDB");

        SetElement.SetAttribute("ver", data.ToString());
        newElement.AppendChild(SetElement);

        if (!Directory.Exists(folderpath))
            Directory.CreateDirectory(folderpath);

        newXmlDoc.Save(filepath);
    }
    public string[] Get_UISetting()
    {
        if (!PlayerPrefs.HasKey("UI")) // Total_Sum_Play_Score 으로 저장된 값이 있는지 체크
        {
            PlayerPrefs.SetString("UI", "0");
            PlayerPrefs.Save();
        }
        if (PlayerPrefs.GetString("UI") == string.Empty) // Total_Sum_Play_Score 으로 저장된 값이 있는지 체크
        {
            PlayerPrefs.SetString("UI", "0");
            PlayerPrefs.Save();
        }

        string[] arrS = PlayerPrefs.GetString("UI").Split(',');
        return arrS;
    }
    public int Get_TargetMaterial(List<Material_IV> dataS, List<EliteMaterial> data)
    {
        if (!PlayerPrefs.HasKey("MatS")) // Total_Sum_Play_Score 으로 저장된 값이 있는지 체크
        {
            Debug.Log("저장된 목표값이 없습니다.");
            PlayerPrefs.SetString("MatS", "D32,1");
            PlayerPrefs.Save();
        }
        if (PlayerPrefs.GetString("MatS") == string.Empty) // Total_Sum_Play_Score 으로 저장된 값이 있는지 체크
        {
            Debug.Log("저장된 목표값이 없습니다.");
            PlayerPrefs.SetString("MatS", "D32,1");
            PlayerPrefs.Save();
        }

        Debug.Log("저장된 목표값을 읽는 중입니다.");
        string[] arrS = PlayerPrefs.GetString("MatS").Split(',');
        for (int i = 0; i < arrS.Length;)
        {
            Material_IV tmp = new Material_IV();
            string code = arrS[i++];
            tmp.data = data.Find(delegate (EliteMaterial a) { return a.code == code; });
            tmp.count = int.Parse(arrS[i++]);
            dataS.Add(tmp);
        }
        return 0;
    }
    public int Get_InvenMaterial(List<Material_IV> dataI, List<EliteMaterial> data)
    {
        if (!PlayerPrefs.HasKey("MatI") || PlayerPrefs.GetString("MatI") == string.Empty) // Total_Sum_Play_Score 으로 저장된 값이 있는지 체크
        {
            Debug.Log("저장된 목표값이 없습니다.");
            for (int i = 0; i < data.Count; i++)
            {
                Material_IV tmp = new Material_IV();
                tmp.data = data[i];
                tmp.inven = 0;

                dataI.Add(tmp);
            }
            return -1;
        }

        Debug.Log("저장된 목표값을 읽는 중입니다.");
        string[] arrI = PlayerPrefs.GetString("MatI").Split(',');
        for(int i = 0; i < data.Count; i++)
        {
            Material_IV tmp = new Material_IV();
            tmp.data = data[i];
            tmp.inven = 0;

            int index = 0;
            for(; index < arrI.Length; index++)
            {
                if (data[i].code.Equals(arrI[index]))
                {
                    tmp.inven = int.Parse(arrI[index + 1]);
                    break;
                }
            }
            dataI.Add(tmp);
        }
        return 0;
    }
    public void Get_UserSetting(ClientSetting set)
    {
        if (!PlayerPrefs.HasKey("Setting")) // Total_Sum_Play_Score 으로 저장된 값이 있는지 체크
        {
            string saveData = string.Empty;
            saveData += "NULL,"; // 다운로드된 DB 버전
            saveData += "OFF,"; // 온라인 DB 사용 여부 - OFF = 오프라인 DB
            saveData += "OFF"; // DB 자동 업데이트
            PlayerPrefs.SetString("Setting", saveData);
        }

        if (PlayerPrefs.GetString("Setting") == "")
        {
            set.DB_ver = "NULL";
            set.onlineDB_use = "ON";
            return;
        }
        string[] arr = PlayerPrefs.GetString("Setting").Split(',');

        set.DB_ver = arr[0];
        set.onlineDB_use = arr[1];
        set.updateDB = arr[2];
    }
    public void Save_VersionCode(string resouce="null", string audio="null")
    {
        string[] data = Get_VersionCode();

        if (!resouce.Equals("null"))
            data[0] = resouce;
        if (!audio.Equals("null"))
            data[1] = audio;

        string save = string.Format("{0},{1}", data[0], data[1]);
        Debug.Log(save);
        PlayerPrefs.SetString("Version", save);
        PlayerPrefs.SetString("Tutorial", "4");
        PlayerPrefs.Save();
    }
    public string[] Get_VersionCode()
    {
        if (!PlayerPrefs.HasKey("Version"))
        {
            PlayerPrefs.SetString("Version", "0,0");
            PlayerPrefs.Save();
        }
        if (PlayerPrefs.GetString("Version") == "")
        {
            PlayerPrefs.SetString("Version", "0,0");
            PlayerPrefs.Save();
        }

        string[] arr = PlayerPrefs.GetString("Version").Split(',');

        return arr;
    }
    // ==========================================================================
  
    bool isPaused = false;
    void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            isPaused = true;
            /* 앱이 비활성화 되었을 때 처리 */
        }

        else
        {
            if (isPaused)
            {
                isPaused = false;
                setSystemUiVisibilityChange();
                /* 앱이 활성화 되었을 때 처리 */
            }
        }
    }
    IEnumerator setSystemUiVisibilityChange()
    {
        yield return new WaitForSeconds(0.5f);
        ApplicationChrome.setSystemUiVisibilityChange();
    }
}
