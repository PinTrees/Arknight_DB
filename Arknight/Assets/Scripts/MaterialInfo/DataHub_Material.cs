using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MaterialData;

namespace MaterialData
{
    public class SubMaterial
    {
        public string name;
        public int count;
        public SubMaterial(string _name, int _count) { name = _name; count = _count; }
    }
    public class DropStage // 재료의 드랍 스테이지 데이터
    {
        public string name;
        public string per;
        public DropStage(string _name, string _per) { name = _name; per = _per; }
    }
    public class UpgradeMaterial
    {
        public int id;
        public string name;
        public string rare;
        public List<DropStage> dropstage;
        public List<SubMaterial> submaterial;
        public List<UpgradeMaterial> add_list;

        public Sprite icon;
        public string info;
        public string record;

        public int count; // 보유 개수 또는 선택 개수를 누적시킬때만 사용
        public UpgradeMaterial() { }
        public UpgradeMaterial(int _id, string _name, string _rare, string _info, string _rec)
        {
            add_list = new List<UpgradeMaterial>();
            count = 0;
            id = _id; name = _name; rare = _rare; info = _info; record = _rec;
        }
        public void SetMadeList(List<SubMaterial> set) { submaterial = set; }
        public void SetDropList(List<DropStage> set) { dropstage = set; }
        public void AddMaterial(UpgradeMaterial set) { add_list.Add(set); }
    }
    public class EliteMaterial
    {
        public int id;
        public string code;
        public string name;
        public string rare;
        public string[] dropstage;
        public string[] dropPer;
        public string[] submaterial;
        public string[] subCount;
        public List<EliteMaterial> SubMaterial;

        public Texture2D icon;
        public string info;
        public string record;

        public int count;
        ~EliteMaterial()
        {
            icon = null;
            SubMaterial.Clear();
            Debug.Log("<Material Class> Dstroy Memory");
        }
    }
    public class MaterialChip
    {
        public int id;
        public string code;
        public string name;
        public string rare;

        public string[] dropstage;
        public Texture2D icon;
        ~MaterialChip()
        {
            icon = null;
        }
    }
    public class Material_Op
    {
        public EliteMaterial data;
        public int count;    
    }
    public class Material_IV
    {
        public EliteMaterial data; // 데이터
        public int count;    // 재료의 필요 개수
        public int countR;    // 최종 필요 개수값
        public int inven;   //  인벤토리에 남아있는값 - 상위재료로 조합할때 소모됨
        public int made;    // 제작한 개수
        public int countUse;    // 사용한 재료 개수
    }
    public class MaterialCount
    {
        public UpgradeMaterial data;
        public string name; // 상위 접근용 변수 
        public int id; // 정렬용 변수;
        public int count;
    }
}
public class DataHub_Material : MonoBehaviour
{
    public List<EliteMaterial> eliteMaterialData;
    public List<EliteMaterial> material_event;

    public List<MaterialChip> chipMaterialData;
    [Header("- UI Static Components")]
    public Texture2D[] materialFrame;
    public Texture2D[] materialIcon;

    public List<EliteMaterial> material_all;
    public void Initialized()
    {
        material_event = new List<EliteMaterial>();
        material_all = new List<EliteMaterial>();
        eliteMaterialData = new List<EliteMaterial>();
        XML.This.Get_MaterialData(eliteMaterialData);
        XML.This.Get_MaterialData(material_all);
        XML.This.get_material_xml(material_event);

        Texture2D[] sprites = Resources.LoadAll<Texture2D>("UI/Icon/Material");
        for (int i = 0; i < eliteMaterialData.Count; i++)
        {
            eliteMaterialData[i].icon = sprites[i];
            material_all[i].icon = sprites[i];
        }
     
        chipMaterialData = new List<MaterialChip>();
        XML.This.Get_MaterialChipData(chipMaterialData);
        Texture2D[] texture_chip = Resources.LoadAll<Texture2D>("UI/Icon/Material_Chip");
        for (int i = 0; i < chipMaterialData.Count; i++)
            chipMaterialData[i].icon = texture_chip[i];

        for(int i = 0; i < material_event.Count; i++)
        {
            Texture2D cur_texture = get_material_icon(material_event[i].code);
            material_event[i].icon = cur_texture;
            material_all.Add(material_event[i]);
        }
    }
    public EliteMaterial get_material_all(string code)
    {
        EliteMaterial set = material_all.Find(delegate (EliteMaterial a)
        {
            return a.code == code;
        });
        return set;
    }
    public MaterialChip GetMaterialChip(string code)
    {
        MaterialChip set = chipMaterialData.Find(delegate (MaterialChip a)
        {
            return a.code == code;
        });
        return set;
    }
    public Texture2D GetMaterialFrame(string rare)
    {
        return materialFrame[int.Parse(rare) - 1];
    }
    public Texture2D getMaterialIcon(string code)
    {
        if (code.Equals("furniture"))
            return materialIcon[0];
        else return null;
    }
    Texture2D get_material_icon(string code)
    {
        if (code.Equals("furniture"))
            return materialIcon[0];
        if(code.Equals("af_qf"))
            return materialIcon[1];
        else return null;
    }
}
