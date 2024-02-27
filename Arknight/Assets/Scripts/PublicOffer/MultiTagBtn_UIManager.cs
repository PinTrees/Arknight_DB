using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using UnityEngine.UI;
using CharactorDataSet;
using TagManager;

public class TagID
{
    public int id;
    public string name;
}
public class TagListm
{
    public TagID[] data;
}

public class MultiTagBtn_UIManager : MonoBehaviour
{
    [Header("- UI Scripts")]
    public CharactorData data;
    public MultiTagIcon_UIManager multiViewer;

    public List<Image> button_list;
    [Header("- UI GameObject")]
    public ScrollRect menuViewer;
    public GameObject This;
    public GameObject[] clearBtn;   // [0] main, [1] single

    [Header("- UI Transform")]
    public Transform btnlist;

    private Color[] colors;
    private List<Image> btn_list;
    private List<string> btn_nameList;
    private List<TagID> selectTag = new List<TagID>();
    private List<TagListm> tagcombi = new List<TagListm>();
    public void Initialized()
    {
        colors = new Color[3];
        colors[0] = new Color(200 / 255f, 50 / 255f, 100 / 255.0f, 1);
        colors[1] = new Color(0 / 255.0f, 150 / 255.0f, 200 / 255.0f, 255 / 255.0f);
        colors[2] = new Color(35 / 255.0f, 35 / 255.0f, 35 / 255.0f, 255 / 255.0f);

        btn_list = new List<Image>();

        for (int i = 0; i < btnlist.childCount; i++) // 
            btn_list.Add(btnlist.transform.GetChild(i).GetComponent<Image>());

        menuViewer.content.anchoredPosition = new Vector2(menuViewer.content.position.x, 1024);

        clearBtn[0].SetActive(false);
        clearBtn[1].SetActive(false);

        InitializeButtonName();
    }
    public void SetActive(bool set)
    {
        This.SetActive(set);
    }
    public bool GetActive()
    {
        return This.activeSelf;
    }
    private void InitializeButtonName()
    {
        btn_nameList = new List<string>();
        btn_nameList.Add("신입");
        btn_nameList.Add("베테랑");
        btn_nameList.Add("전문가");
        btn_nameList.Add("근거리");
        btn_nameList.Add("원거리");
        btn_nameList.Add("여성");
        btn_nameList.Add("남성");
        btn_nameList.Add("선봉");
        btn_nameList.Add("저격");
        btn_nameList.Add("의료");
        btn_nameList.Add("술사");
        btn_nameList.Add("근위");
        btn_nameList.Add("중장");
        btn_nameList.Add("보조");
        btn_nameList.Add("특수");
        btn_nameList.Add("치료");
        btn_nameList.Add("지원");
        btn_nameList.Add("딜링");
        btn_nameList.Add("광역");
        btn_nameList.Add("슬로우");
        btn_nameList.Add("생존");
        btn_nameList.Add("보호");
        btn_nameList.Add("약화");
        btn_nameList.Add("변위");
        btn_nameList.Add("군중제어");
        btn_nameList.Add("폭발");
        btn_nameList.Add("소환");
        btn_nameList.Add("쾌속부활");
        btn_nameList.Add("비용회복");
        btn_nameList.Add("로봇");
    }
    public void MultyTagCalculator(List<TagID> target, int combi)
    {
        for (int i = 0; i < target.Count; i++)
        {
            if (combi == 1)
            {
                TagListm now = new TagListm();
                now.data = new TagID[1];
                now.data[0] = target[i];
                tagcombi.Add(now);
                continue;
            }
            for (int i2 = i + 1; i2 < target.Count; i2++)
            {
                if (combi == 2)
                {
                    TagListm now = new TagListm();
                    now.data = new TagID[2];
                    now.data[0] = target[i];
                    now.data[1] = target[i2];
                    tagcombi.Add(now);
                    continue;
                }
                for (int i3 = i2 + 1; i3 < target.Count; i3++)
                {
                    if (combi == 3)
                    {
                        //Debug.Log(target[i].name + ", " + target[i2].name + ", " + target[i3].name);
                        TagListm now = new TagListm();
                        now.data = new TagID[3];
                        now.data[0] = target[i];
                        now.data[1] = target[i2];
                        now.data[2] = target[i3];
                        tagcombi.Add(now);
                        continue;
                    }
                    for (int i4 = i3 + 1; i4 < target.Count; i4++)
                    {
                        if (combi == 4)
                        {
                            TagListm now = new TagListm();
                            now.data = new TagID[4];
                            now.data[0] = target[i];
                            now.data[1] = target[i2];
                            now.data[2] = target[i3];
                            now.data[3] = target[i4];
                            tagcombi.Add(now);
                            continue;
                        }
                        for (int i5 = i4 + 1; i5 < target.Count; i5++)
                        {
                            if (combi == 5)
                            {
                                TagListm now = new TagListm();
                                now.data = new TagID[5];
                                now.data[0] = target[i];
                                now.data[1] = target[i2];
                                now.data[2] = target[i3];
                                now.data[3] = target[i4];
                                now.data[4] = target[i5];
                                tagcombi.Add(now);
                                continue;
                            }
                            for (int i6 = i5 + 1; i6 < target.Count; i6++)
                            {
                                if (combi == 5)
                                {
                                    TagListm now = new TagListm();
                                    now.data = new TagID[6];
                                    now.data[0] = target[i];
                                    now.data[1] = target[i2];
                                    now.data[2] = target[i3];
                                    now.data[3] = target[i4];
                                    now.data[4] = target[i5];
                                    now.data[5] = target[i6];
                                    tagcombi.Add(now);
                                    continue;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    public void ResetTag(int type)  // 0 main, 1 single
    {
        if (type.Equals(1))
        {
            RefreshGraphic();
            clearBtn[1].SetActive(false);
        }
        else
        {
            for (int i = 0; i < btn_list.Count; i++)
                BtnColor(btn_list[i], false);
            selectTag.Clear();
            RefreshTag();
            clearBtn[0].SetActive(false);
        }
    }
    public void RefreshTag()
    {
        tagcombi.Clear();
        MultyTagCalculator(selectTag, 1);
        MultyTagCalculator(selectTag, 2);
        MultyTagCalculator(selectTag, 3);

        int count = 0;
        for (int i = 0; i < tagcombi.Count; i++)
        {
            List<PO_OperatorClass> now = new List<PO_OperatorClass>();
            now =  Search(data.Ch_dataList, tagcombi[i]);
            if (now.Count > 0)
            {
                multiViewer.SetListData(tagcombi[i], now, count++);
            }
        }
        if (count == 0)
            multiViewer.SetListData(null, null, -1);
        multiViewer.Refresh();
    }
    public void RefreshGraphic()
    {
        for (int i = 0; i < btn_list.Count; i++)
            btn_list[i].color = colors[2];
        for (int i = 0; i < selectTag.Count; i++)
        {
            int index = btn_nameList.FindIndex(delegate (string a) { return a == selectTag[i].name; });
            btn_list[selectTag[i].id].color = colors[1];
        }
    }
    public void RefreshSelectTag(PO_OperatorClass target)   // Select Operator Tag Viewer
    {
        clearBtn[1].SetActive(true);
        RefreshGraphic();

        if (!target.quality.Equals("나머지") && !target.quality.Equals("NULL"))
        {
            int index = btn_nameList.FindIndex(delegate (string a) { return a == target.quality; });
            btn_list[index].color = colors[0];
        }
        if (!target.position.Equals(string.Empty))
        {
            int index = btn_nameList.FindIndex(delegate (string a) { return a == target.position; });
            btn_list[index].color = colors[0];
        }
        if (!target.Class.Equals(string.Empty))
        {
            int index = btn_nameList.FindIndex(delegate (string a) { return a == target.Class; });
            btn_list[index].color = colors[0];
        }

        for (int i = 0; i < target.tag.Length; i++)
        {
            int index = btn_nameList.FindIndex(delegate (string a) { return a == target.tag[i]; });
            btn_list[index].color = colors[0];
        }
    }
    public void Trigger_TagBtn(Transform set)
    {
        int index = set.GetSiblingIndex();
        int c = selectTag.FindIndex(delegate (TagID a) { return a.name == btn_nameList[index]; });
        if (c != -1)
        {
            selectTag.RemoveAt(c);
            BtnColor(btn_list[index], false);
        }
        else if (selectTag.Count >= 6)
        {
            return;
        }
        else
        {
            TagID New = new TagID();
            New.id = index; New.name = btn_nameList[index];
            selectTag.Add(New);

            selectTag.Sort(delegate (TagID a, TagID b) // 클래스 리스트 특정 요소값으로 정렬 방법
            {
                if (a.id > b.id) return 1;
                else if (a.id < b.id) return -1;
                return 0;
            });

            BtnColor(btn_list[index], true);
            //for (int i = 0; i < selectTag.Count; i++)
            //    Debug.Log(selectTag[i].name);
        }

        RefreshTag();
        clearBtn[0].SetActive(true);
    }
    void BtnColor(Image set, bool tag)
    {
        if (tag)
            set.color = colors[1];
        else
            set.color = colors[2];
    }

    public List<PO_OperatorClass> Search(List<PO_OperatorClass> data, TagListm tagdata)
    {
        bool flag = false;
        List<PO_OperatorClass> ret_list = new List<PO_OperatorClass>();

        for (int i = 0; i < tagdata.data.Length; i++)
        {
            List<PO_OperatorClass> tmp_list = new List<PO_OperatorClass>();

            List<PO_OperatorClass> now = new List<PO_OperatorClass>();
            if (flag) now = ret_list;
            else now = data;

            for (int j = 0; j < now.Count; j++) // 베이스 데이터리스트에서 추출
            {
                if (tagdata.data[i].id < 3 && tagdata.data[i].name == now[j].quality) // quality tag
                {
                    tmp_list.Add(now[j]);
                    flag = true;
                }
                else if (tagdata.data[i].id < 5 && tagdata.data[i].name == now[j].position) // position tag
                {
                    if (!flag && now[j].rare == "6") // 
                        continue;
                    tmp_list.Add(now[j]);
                    flag = true;
                }
                else if (tagdata.data[i].id < 7 && tagdata.data[i].name == now[j].gender) // gender tag
                {
                    if (!flag && now[j].rare == "6") // 
                        continue;
                    tmp_list.Add(now[j]);
                    flag = true;
                }
                else if (tagdata.data[i].id < 15 && tagdata.data[i].name == now[j].Class) // gender tag
                {
                    if (!flag && now[j].rare == "6") // 
                        continue;
                    tmp_list.Add(now[j]);
                    flag = true;
                }
                else if (tagdata.data[i].id < 30) // tags tag
                {
                    if (!flag && now[j].rare == "6") // 
                        continue;
                    for (int k = 0; k < now[j].tag.Length; k++)
                    {
                        if (tagdata.data[i].name == now[j].tag[k])
                        {
                            tmp_list.Add(now[j]);
                            flag = true;
                        }
                    }
                }
            }
            ret_list.Clear();
            for (int j = 0; j < tmp_list.Count; j++) // 데이터 복사
                ret_list.Add(tmp_list[j]);
        }
        ret_list.Sort(delegate (PO_OperatorClass a, PO_OperatorClass b) // 클래스 리스트 특정 요소값으로 정렬 방법
        {
            if (a.id > b.id) return 1;
            else if (a.id < b.id) return -1;
            return 0;
        });
        return ret_list;
    }
}