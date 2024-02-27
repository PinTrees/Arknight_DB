using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CharactorDataSet;
using MaterialData;

public class Operater_Data_Hub : MonoBehaviour
{
    public List<OperaterClass> operatorData;
    public List<EliteMaterial> materialData;
    public List<MaterialChip> chips;

    public Texture2D[] costumeIcon;
    public Texture2D[] eliteIcon;
    public Texture2D[] infraskillIcon;
    public Texture2D[] rareIcon;

    private Sprite[] classList;
    private Sprite[] groupList;
    private Sprite[] material_frame;
    private Sprite[] rangIcon;

    Color[] rare_colors = new Color[6];
    public IEnumerator InitailizeData()
    {
        rare_colors[5] = new Color(1, 100 / 255f, 0);
        rare_colors[4] = new Color(1, 150 / 255f, 0);
        rare_colors[3] = new Color(200 / 255f, 100 / 255f, 1);
        rare_colors[2] = new Color(100 / 255f, 150 / 255f, 1);
        rare_colors[1] = new Color(150 / 255f, 1, 100 / 255f);
        rare_colors[0] = new Color(150 / 255f, 150 / 255f, 150 / 255f);

        operatorData = new List<OperaterClass>();
        materialData = new List<EliteMaterial>();
        chips = new List<MaterialChip>();

        XML.This.Get_OperatorInfo(operatorData);
        StartCoroutine(XML.This.IE_Operater_SkillData(operatorData));
        StartCoroutine(XML.This.IE_Operater_Status(operatorData));
        StartCoroutine(XML.This.IE_Operatro_InfraSkill(operatorData));

        StartCoroutine(Set_OperatorPNG(operatorData));
        StartCoroutine(Initialize_Resources());
        StartCoroutine(XML.This.IE_MaterialData(materialData));
        XML.This.Get_MaterialChipData(chips);
        yield return new WaitForEndOfFrame();

        Texture2D[] sprite_mat = Resources.LoadAll<Texture2D>("UI/Icon/Material");
        for (int i = 0; i < materialData.Count; i++)
            materialData[i].icon = sprite_mat[i];

        Texture2D[] sprites_c = Resources.LoadAll<Texture2D>("UI/Icon/Material_Chip");
        for (int i = 0; i < chips.Count; i++)
            chips[i].icon = sprites_c[i];
    }
    private IEnumerator Initialize_Resources()
    {
        material_frame = Resources.LoadAll<Sprite>("UI/Frame/Material");
        rangIcon = Resources.LoadAll<Sprite>("OperatorRang");
        classList = Resources.LoadAll<Sprite>("UI/Icon/Class");
        groupList = Resources.LoadAll<Sprite>("UI/Icon/Group");
       
        yield return null;
    }
    private IEnumerator Set_OperatorPNG(List<OperaterClass> data)
    {
        for (int i = 0; i < data.Count; i++) // 리소스 - 일러스트 + 아이콘 로딩
        {
            OperaterClass cur = data[i];
            string path = string.Format("{0}/{1}", Files.Use.DocumentsPath("Resource/OperatorIcon"), cur.en_name);
            cur.icon = Files.Use.GetPNG(path, "Icon.png");
        }
        yield return null;
    }
    private void OnDestroy()
    {
        for (int i = 0; i < chips.Count; i++)
            chips.RemoveAt(0);
        for (int i = 0; i < materialData.Count; i++)
            materialData.RemoveAt(0);
        for (int i = 0; i < operatorData.Count; i++)
        {
            Destroy(operatorData[0].icon);
            operatorData.RemoveAt(0);
        }
        Debug.Log("[Data Hub] Scripts Destroy Memory");
    }

    public OperaterClass GetOperatironData(string name)
    {
        for(int i = 0; i < operatorData.Count; i++)
        {
            if(name == operatorData[i].name)
            {
                return operatorData[i];
            }
        }
        return null;
    }
    public EliteMaterial GetMaterialData(string code)
    {
        EliteMaterial data = materialData.Find(delegate (EliteMaterial a)
        {
            return a.code == code;
        });
        return data;
    }
    public MaterialChip GetMaterialChip(string code)
    {
        MaterialChip data = chips.Find(delegate (MaterialChip a)
        {
            return a.code == code;
        });
        return data;
    }
    public OperaterClass GetIcon(string name)
    {
        OperaterClass data = operatorData.Find(delegate (OperaterClass a)
        {
            return a.name == name;
        });
        return data;
    }
    public Sprite GetClassIcon(string name)
    {
        if (name == "근위")
            return classList[1];
        if (name.Equals("가드"))
            return classList[1];
        else if(name.Equals("뱅가드"))
            return classList[0];
        else if (name.Equals("메딕"))
            return classList[2];
        else if (name.Equals("스나이퍼"))
            return classList[3];
        else if (name.Equals("캐스터"))
            return classList[4];
        else if (name.Equals("디펜더"))
            return classList[5];
        else if (name.Equals("서포터"))
            return classList[6];
        else if (name.Equals("특수"))
            return classList[7];
        return null;
    }
    public Sprite GetGroupIcon(string name)
    {
        if (name.Equals("용문"))
            return groupList[0];
        else if (name.Equals("빅토리아 왕국"))
            return groupList[1];
        else if (name.Equals("로도스"))
            return groupList[2];
        else if (name.Equals("펭귄 로지스틱스"))
            return groupList[3];
        else if (name.Equals("시에스타"))
            return groupList[4];
        else if (name.Equals("레타니아"))
            return groupList[5];
        else if (name.Equals("라인 랩"))
            return groupList[6];
        else if (name.Equals("라테라노"))
            return groupList[7];
        else if (name.Equals("심해"))
            return groupList[8];
        else if (name.Equals("쉐라그"))
            return groupList[9];
        else if (name.Equals("우르수스 제국"))
            return groupList[10];
        else if (name.Equals("카시미어"))
            return groupList[11];
        else if (name.Equals("블랙스틸"))
            return groupList[12];
        else if (name.Equals("림 빌리톤"))
            return groupList[13];
        return null;
    }
    public Color GetRareColor(string rare)
    {
        int index = int.Parse(rare);
        return rare_colors[index - 1];
    }
    public Sprite GetMaterialFrame(string rare)
    {
        int index = int.Parse(rare);
        return material_frame[index - 1];
    }
    public Sprite GetRangIcon(int index)
    {
        if (index > rangIcon.Length)
            return null;
        return rangIcon[index - 1];
    }
    public Texture2D GetEliteIcon(string name)
    {
        if (name.Equals("E0"))
            return eliteIcon[0];
        if (name.Equals("E1"))
            return eliteIcon[1];
        if (name.Equals("E2"))
            return eliteIcon[2];

        return null;
    }
    public Texture2D GetInfraSkillIcon(string name)
    {
        if (name.Equals("기숙사"))
            return infraskillIcon[0];
        else if (name.Equals("통제실"))
            return infraskillIcon[1];
        else if (name.Equals("발전소"))
            return infraskillIcon[2];
        else if (name.Equals("제조소"))
            return infraskillIcon[3];
        else if (name.Equals("무역소"))
            return infraskillIcon[4];
        else if (name.Equals("가공소"))
            return infraskillIcon[5];
        else if (name.Equals("훈련실"))
            return infraskillIcon[6];
        else if (name.Equals("인사부"))
            return infraskillIcon[7];
        else if (name.Equals("응접실"))
            return infraskillIcon[8];
        return null;
    }
    public Texture2D GetRareIcon(string name)
    {
        if (name.Equals("1"))
            return rareIcon[0];
        else if (name.Equals("2"))
            return rareIcon[1];
        else if (name.Equals("3"))
            return rareIcon[2];
        else if (name.Equals("4"))
            return rareIcon[3];
        else if (name.Equals("5"))
            return rareIcon[4];
        else if (name.Equals("6"))
            return rareIcon[5];

        return null;
    }
    public Texture2D GetCostumeIcon(string name)
    {
        if (name.Equals("0011"))
            return costumeIcon[0];
        else if (name.Equals("비타필드"))
            return costumeIcon[1];
        else if (name.Equals("캠브리언"))
            return costumeIcon[2];
        else if (name.Equals("KFC"))
            return costumeIcon[3];
        else if (name.Equals("테스트"))
            return costumeIcon[4];

        return null;
    }
}
