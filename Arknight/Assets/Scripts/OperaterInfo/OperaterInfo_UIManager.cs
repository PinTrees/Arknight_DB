using System.Collections;
using UnityEngine.UI;
using UnityEngine;

using System.IO;
using CharactorDataSet;
using Spine.Unity;
using System;
using System.Collections.Generic;

public class ILustViewer
{
    public Image ilust;
    public RectTransform pos;
    public float value;
}

public class OperaterInfo_UIManager : MonoBehaviour
{
    [Header("- Data Scripts")]
    public Operater_Data_Hub dataMng;

    [Header("- UI Manager Scripts")]
    public OpLevelStatus_UIClass levelinfo;
    public OpAbility_UIClass abilityUI;
    public OpSkillInfo_UIClass skillUI;
    public AudioUI_Op audioUI;
    public ProfileUI_Op profileUI;
    public SkeletonAnimation spine_animator;
    CommentUIManager commentUI;
    StatusUI_Op statusUI;

    [Header("- UI GameObject")]
    public Canvas iconViewer;
    public Canvas This;
    public GameObject loadPanel;
    public GameObject touchEnable;
    public GameObject menuPanel;

    [Header("- Status Canvas Components")]

    [Header("- UI Object")]
    public GameObject LoadingUI;
    public Image loadbar;
    public Text loadvalue;
    public GameObject Download_btn;
    public RawImage[] costumeBtn;
    public GameObject[] illust_btn;

    [Header("- UI Components")]
    public RawImage illust;
    public Image group;
    public Image Class;
    public Text gender;
    public Text tag;
    public Text rare;
    public Text name;

    [Header("- Status - statusBar components")]
    public GameObject statusTopbar;
    public Text statusTopbarName;

    [Header("- UI Transform")]
    public Transform Mask;

    [Header("- UI Menu Btn Image")]
    public Text[] menubtn_txt;
    public GameObject illust_menu;
    public GameObject statusMenu;

    public RawImage gaussianBlur;
    public Material bulrMaterial;
    public RawImage statusBackground;
    //Color[] btn_colors = new Color[2];
    Color[] text_colors = new Color[2];
    GraphicRaycaster illust_raycaster;
    // =================================
    Text artist_text;
    Text voice_text;
    // =================================
    int illustCount;
    OperaterClass curData;
    public void Initialized()
    {
        //infoRect = this.transform.GetChild(1).GetComponent<RectTransform>();
        //statusRect = GameObject.Find("Status Canvas").transform.GetChild(0).GetComponent<RectTransform>();
        //commentRect = this.transform.GetChild(3).GetComponent<RectTransform>();
        statusUI = GameObject.FindGameObjectWithTag("UIManager").GetComponent<StatusUI_Op>();
        commentUI = GameObject.FindGameObjectWithTag("UIManager").GetComponent<CommentUIManager>();

        loadPanel.SetActive(false);
        touchEnable.SetActive(false);
        gaussianBlur.enabled = false;

        //btn_colors[0] = new Color(50 / 255f, 50 / 255f, 50 / 255f);
        //btn_colors[1] = new Color(32 / 255f, 32 / 255f, 32 / 255f);
        text_colors[0] = new Color(124 / 255f, 124 / 255f, 124 / 255f);
        text_colors[1] = new Color(236 / 255f, 236 / 255f, 236 / 255f);

        illust_menu.SetActive(false);
        statusMenu.SetActive(false);
        statusUI.Initialized();
        Download_btn.SetActive(false);

        artist_text = Mask.GetChild(0).GetChild(0).GetComponent<Text>();
        voice_text = Mask.GetChild(1).GetChild(0).GetComponent<Text>();
        illust_raycaster = this.transform.GetChild(1).GetComponent<GraphicRaycaster>();
        illust_raycaster.enabled = false;
        spine_animator.gameObject.SetActive(false);

        levelinfo.Initialized();
        abilityUI.Initialized();
        skillUI.Initialized();
        audioUI.Initialized();
        profileUI.Initialized();
        commentUI.Initialized(touchEnable, gaussianBlur, spine_animator.gameObject);

        menuPanel.SetActive(true);

        TR_SetActive(false);
    }
    private void OnDestroy()
    {
        curData = null;
        // Scrips =====================
        dataMng = null;
        levelinfo = null;
        abilityUI = null;
        skillUI = null;
        audioUI = null;
        // Components =====================
        for (int i = 0; i < costumeBtn.Length; i++)
            Destroy(costumeBtn[i]);
        Destroy(illust.texture);
        Destroy(illust);
        Destroy(group);
        Destroy(Class);
        Destroy(gender);
        Destroy(tag);
        Destroy(rare);
        Destroy(name);
        Destroy(statusTopbarName);
        Destroy(artist_text);
        // GameObject =====================
        Destroy(loadPanel);
        Destroy(menuPanel);
        Destroy(LoadingUI);
        Destroy(Download_btn);
        for(int i = 0; i < illust_btn.Length; i++)
            Destroy(illust_btn[i]);
        // Canvas =====================
        Destroy(iconViewer);
        Destroy(This);
        Debug.Log("[Operator Info Script] Destroy Memory");
    }
    public void SetData(string name)
    {
        curData = dataMng.GetOperatironData(name);
    }
    public void Clear()
    {
        illustCount = 0;

        for (int i = 0; i < costumeBtn.Length; i++)
            costumeBtn[i].transform.parent.gameObject.SetActive(false);

        for (int i =0; i< illust_btn.Length; i++)
            illust_btn[i].SetActive(false);
    }
    public void ClearCanvas()
    {
        TR_SetCommentUI(false);
    }
    public void clear_canvas()
    {
        TR_SetStatusUI(false);
        TR_AudioUI(false);
        TR_IllustViewer(false);
        TR_SetCommentUI(false);
        tr_set_profileCanvas(false);
    }
    public void Refresh()
    {
        name.text = curData.name;
        artist_text.text = curData.illustrator;
        voice_text.text = curData.voice;

        group.sprite = dataMng.GetGroupIcon(curData.group);
        Class.sprite = dataMng.GetClassIcon(curData.Class);
        gender.text = curData.gender;
        rare.text = curData.rare;
        string tags = string.Empty;
        for (int i = 0; i < curData.tags.Length; i++)
        {
            tags = string.Format("{0} {1} {2}", tags, curData.tags[i], " ");
        }
        tag.text = tags;
    }
    public void RefreshIllust()
    {
        StartCoroutine(SetOperaterIlust(illustCount));
    }


    public void tr_set_profileCanvas(bool set)
    {
        if (set && !profileUI.mainCanvas.enabled)
        {
            clear_canvas();
            spine_animator.gameObject.SetActive(false);

            profileUI.tr_start_mainUI(curData);
            menubtn_txt[4].color = text_colors[1];
        }
        else
        {
            profileUI.tr_exite_mainUI();
            menubtn_txt[4].color = text_colors[0];
            spine_animator.gameObject.SetActive(true);
        }
    }
    public void TR_SetStatusUI(bool set)
    {
        if (set && !statusUI.activeSelf())
        {
            clear_canvas();
            StartCoroutine(UIStart_StatusCanvas());
        }
        else
        {
            UIExit_StatusCanvas();
        }
    }
    public void TR_AudioUI(bool set)
    {
        if (set && audioUI.IsActive().Equals(false))
        {
            clear_canvas();
            menuPanel.SetActive(false);
            spine_animator.gameObject.SetActive(false);

            audioUI.SetData(curData.en_name);
            StartCoroutine(audioUI.UIStart_Main());
            menubtn_txt[0].color = text_colors[1];
        }
        else
        {
            audioUI.TR_Close();
            menuPanel.SetActive(true);
            menubtn_txt[0].color = text_colors[0];

            spine_animator.gameObject.SetActive(true);
        }
    }
    public void TR_SetCommentUI(bool set)
    {
        if (set && !commentUI.canvas.enabled)
        {
            clear_canvas();
            StartCoroutine(UIStart_CommentCanvas());
        }
        else if (!set && commentUI.canvas.enabled)
        {
            commentUI.ExiteUI_Main();
        }
    }
    // UI IllustMenu Trigger Start ============================================
    public void TR_IllustViewer(bool set)
    {
        if (set && illust_raycaster.enabled.Equals(false))
        {
            clear_canvas();

            menuPanel.SetActive(false);
            illust_raycaster.enabled = true;
            group.enabled = false;
            artist_text.transform.parent.gameObject.SetActive(false);
            voice_text.transform.parent.gameObject.SetActive(false);
            spine_animator.gameObject.SetActive(false);

            illust_menu.SetActive(true);
            menubtn_txt[2].color = text_colors[1];
        }
        else
        {
            menuPanel.SetActive(true);
            illust_raycaster.enabled = false;
            illust_menu.SetActive(false);

            group.enabled = true;
            artist_text.transform.parent.gameObject.SetActive(true);
            voice_text.transform.parent.gameObject.SetActive(true);
            spine_animator.gameObject.SetActive(true);
            menubtn_txt[2].color = text_colors[0];
        }
    }
    public void TR_SetIllustSize(Slider size)
    {
        illust.rectTransform.localScale = new Vector2(size.value, size.value);
    }
    // UI IllustMenu Trigger Exit ============================================
    public void TR_SetActive(bool set)
    {
        if (set)
            StartCoroutine(UIStart_InfoCanvas());
        else
        {
            TR_IllustViewer(false);
            UIExit_InfoCanvas();
        }
    }
    // UI Coroutine Start ============================================
    IEnumerator UIStart_InfoCanvas()
    {
        Clear();
        ClearCanvas();

        illustCount = 0;
        illust.gameObject.SetActive(false);
        group.gameObject.SetActive(false);
        yield return new WaitForEndOfFrame();

        yield return StartCoroutine(LogU.Use.SetLog("오퍼레이터 일러스트 파일 로딩중..."));
        group.sprite = dataMng.GetGroupIcon(curData.group);
        StartCoroutine(SetOperaterIlust(illustCount));

        // Text Components ========================================================
        name.text = curData.name;
        artist_text.text = curData.illustrator;
        voice_text.text = curData.voice;
        Class.sprite = dataMng.GetClassIcon(curData.Class);
        gender.text = curData.gender;
        rare.text = curData.rare;
        string tags = string.Empty;
        for (int i = 0; i < curData.tags.Length; i++)
            tags = string.Format("{0} {1} {2}", tags, curData.tags[i], " ");
        tag.text = tags;
        // Button Menu ========================================================
        yield return StartCoroutine(LogU.Use.SetLog("오퍼레이터 메뉴 출력중..."));
        illust_btn[0].SetActive(true);
        if (curData.eliteData.Count > 2)
            illust_btn[1].SetActive(true);

        for (int i = 0; i < curData.costume.Count; i++)
        {
            Texture2D icon = dataMng.GetCostumeIcon(curData.costume[i].type);
            if (icon == null)
                costumeBtn[i].gameObject.SetActive(false);
            else
            {
                costumeBtn[i].texture = icon;
                costumeBtn[i].gameObject.SetActive(true);
            }
            costumeBtn[i].transform.parent.gameObject.SetActive(true);
        }

        group.gameObject.SetActive(true);
        menuPanel.SetActive(true);

        yield return StartCoroutine(LogU.Use.SetLog("오퍼레이터 정보창 활성화중..."));
        statusMenu.SetActive(true);
        This.enabled = true;

        yield return StartCoroutine(LogU.Use.SetLog("오퍼레이터 스파인 파일 로딩중..."));
        StartCoroutine(UIRefresh_Spine());

        yield return new WaitForEndOfFrame();
    }
    IEnumerator UIStart_StatusCanvas()
    {
        statusTopbarName.text = curData.name + " - 정보";

        levelinfo.SetData(curData);
        //yield return StartCoroutine(LogU.Use.SetLog("스테이터스 레벨 정보 출력중..."));
        StartCoroutine(levelinfo.UIStart_MainCanvas());

        //yield return StartCoroutine(LogU.Use.SetLog("스테이터스 특성 정보 출력중..."));
        StartCoroutine(abilityUI.UIStart_Main(curData));

        //yield return StartCoroutine(LogU.Use.SetLog("스테이터스 스킬 정보 출력중..."));
        StartCoroutine(skillUI.UIStart_MainCanvas(curData));

        //yield return StartCoroutine(LogU.Use.SetLog("스테이터스 창 활성화중..."));
        gaussianBlur.material = bulrMaterial;
        gaussianBlur.enabled = true;
        yield return new WaitForEndOfFrame();

        Destroy(gaussianBlur.texture);
        gaussianBlur.texture = Files.Use.ScreenShot();
        gaussianBlur.material = null;

        spine_animator.gameObject.SetActive(false);

        statusTopbar.SetActive(true);
        statusUI.StartUI_Main(curData);
        menubtn_txt[1].color = text_colors[1];

        if (!This.enabled)
        {
            UIExit_StatusCanvas();
            spine_animator.gameObject.SetActive(false);
        }
        yield return null;
    }
    IEnumerator UIStart_CommentCanvas()
    {
        spine_animator.gameObject.SetActive(false);

        gaussianBlur.material = bulrMaterial;
        gaussianBlur.enabled = true;
        yield return new WaitForEndOfFrame();

        Destroy(gaussianBlur.texture);
        gaussianBlur.texture = Files.Use.ScreenShot();
        gaussianBlur.material = null;

        StartCoroutine(commentUI.StartUI_Main(curData.en_name));
    }
    IEnumerator UIRefresh_Spine()
    {
        // 1. Create the AtlasAsset (needs atlas text asset and textures, and materials/shader);
        // 2. Create SkeletonDataAsset (needs json or binary asset file, and an AtlasAsset)
        // 3. Create SkeletonAnimation (needs a valid SkeletonDataAsset)
        spine_animator.gameObject.SetActive(false);
        yield return new WaitForEndOfFrame();

        TextAsset skeletonJSON = Resources.Load("spine/" + curData.en_name + "/" + curData.en_name + ".skel") as TextAsset;
        TextAsset atlasFile = Resources.Load("spine/" + curData.en_name + "/" + curData.en_name + ".atlas") as TextAsset;
        Material atlasMaterial = Resources.Load("spine/" + curData.en_name + "/" + curData.en_name + "_Material") as Material;
        Material[] materialElement = { atlasMaterial }; // i got only 1 material on this model

        //Debug.Log(skeletonJSON);
        //Debug.Log(atlasFile);
        AtlasAsset runtimeAtlasAsset = AtlasAsset.CreateRuntimeInstance(atlasFile, materialElement, true); // seems to fail
        SkeletonDataAsset runtimeSkeletonDataAsset =  SkeletonDataAsset.CreateRuntimeInstance(skeletonJSON, runtimeAtlasAsset, true);  // create a non-readable SkeletonDataAsset


        Destroy(spine_animator.gameObject.GetComponent<MeshFilter>().mesh);
        //System.GC.Collect();
        //Resources.UnloadUnusedAssets();

        spine_animator.skeletonDataAsset = runtimeSkeletonDataAsset;
        spine_animator.Initialize(true);
        spine_animator.AnimationState.ClearTracks();

        spine_animator.AnimationState.AddAnimation(0, "Start", false, 0f);
        spine_animator.gameObject.SetActive(true);
        spine_animator.AnimationState.AddAnimation(0, "Idle", true, 0f);
        //spine_animator = SkeletonAnimation.NewSkeletonAnimationGameObject(runtimeSkeletonDataAsset);

        yield return null;
    }
    public void UIExit_InfoCanvas()
    {
        clear_canvas();

        spine_animator.skeletonDataAsset.Clear();
        spine_animator.gameObject.SetActive(false);

        statusMenu.SetActive(false);

        illust_raycaster.enabled = false;
        This.enabled = false;
    }
    public void UIExit_StatusCanvas()
    {
        statusTopbar.SetActive(false);

        levelinfo.UIExit_MainCanvas();
        abilityUI.UIExit_Main();
        skillUI.UIExit_MainCanvas();
        statusUI.ExiteUI_Main();
        gaussianBlur.enabled = false;
        spine_animator.gameObject.SetActive(true);

        menubtn_txt[1].color = text_colors[0];
    }
    // UI Coroutine Exit ============================================
    public void TR_SpineNext()
    {
        string curanimation = spine_animator.AnimationName;
        if (curanimation.Equals("Start"))
            return;

        Spine.ExposedList<Spine.Animation> animaitonList = spine_animator.skeletonDataAsset.GetSkeletonData(true).Animations;
        spine_animator.Initialize(true);

        if (curanimation.Equals("Idle"))
        {
            StartCoroutine(LogU.Use.SetLog("공격 애니메이션 재생"));
            spine_animator.AnimationState.ClearTracks();

            List<string> animations = new List<string>();
            animations.Add("Attack");
            animations.Add("Attack_Loop");
            animations.Add("Attack_1");
            animations.Add("Attack_01");
            animations.Add("Skill_Loop");
            animations.Add("Skill_1_Loop");
            animations.Add("Die");

            for (int i = 0; i < animations.Count; i++)
            {
                Spine.Animation result = animaitonList.Find(delegate (Spine.Animation data) { return (animations[i] == data.Name); });
                if (result != null)
                {
                    spine_animator.AnimationState.AddAnimation(0, animations[i], true, 0f);
                    break;
                }
            }
        }
        else if (curanimation.Equals("Attack") 
            || curanimation.Equals("Attack_1")
            || curanimation.Equals("Attack_Loop") 
            || curanimation.Equals("Skill_Loop")
            || curanimation.Equals("Skill_1_Loop")
            || curanimation.Equals("Attack_01"))
        {
            StartCoroutine(LogU.Use.SetLog("사망 애니메이션 재생"));
            spine_animator.AnimationState.ClearTracks();
            List<string> animations = new List<string>();
            animations.Add("Die");
            animations.Add("Start");

            for (int i = 0; i < animations.Count; i++)
            {
                Spine.Animation result = animaitonList.Find(delegate (Spine.Animation data) { return (animations[i] == data.Name); });
                if (result != null)
                {
                    spine_animator.AnimationState.AddAnimation(0, animations[i], true, 0f);
                    break;
                }
            }
        }
        else if (curanimation.Equals("Die"))
        {
            StartCoroutine(LogU.Use.SetLog("애니메이션 초기화"));
            spine_animator.AnimationState.ClearTracks();
            spine_animator.AnimationState.AddAnimation(0, "Start", false, 0f);
            spine_animator.AnimationState.AddAnimation(0, "Idle", true, 0f);
        }
    }
    public void TR_WWWFromIllust()
    {
        Download_btn.SetActive(false);
        StartCoroutine(DownLoad_Illust());
    }
    public void TR_SetIllustCount(int set)
    {
        illustCount = set;
        RefreshIllust();
    }
    public void TR_ArtistLink(int index)
    {
        if (index.Equals(0))
            Application.OpenURL(curData.artistlink);
        else if(index.Equals(1))
            Application.OpenURL(curData.voicelink);
    }
    // Get UI Status ============================================
    public bool GetStatus_CommentUI()
    {
        return commentUI.canvas.enabled;
    }
    // ============================================================
    private IEnumerator DownLoad_Illust()
    {
        LoadingUI.SetActive(true);
        string url = string.Empty;

        string filename = string.Empty;
        if (illustCount == 0)
        {
            url = string.Format("https://github.com/PinTrees/Arknight_DB/raw/master/Resources/OperatorIllust/{0}.png", curData.en_name);
            Debug.Log(url);
            filename = curData.en_name;
        }
        else if (illustCount == 1)
        {
            url = string.Format("https://github.com/PinTrees/Arknight_DB/raw/master/Resources/OperatorIllust/{0}_elite2.png", curData.en_name);
            filename = curData.en_name + "_elite2";
        }
        else if(illustCount >= 2)
        {
            url = string.Format("https://github.com/PinTrees/Arknight_DB/raw/master/Resources/OperatorIllust/{0}_c{1}.png", curData.en_name, illustCount -1);
            filename = string.Format("{0}_c{1}", curData.en_name, illustCount - 1);
        }
        yield return Download.Use.DownLoad(url, Files.Use.DocumentsPath("Resource/OperatorIllust"), filename + ".png", loadbar, loadvalue, "PNG");
   
        RefreshIllust();

        LoadingUI.SetActive(false);
        yield return null;
    }
    public IEnumerator SetOperaterIlust(int index)
    {
        loadPanel.SetActive(true);
        yield return new WaitForEndOfFrame();

        string main_path = Files.Use.DocumentsPath("Resource/OperatorIllust");

        if (!Directory.Exists(main_path))
            Directory.CreateDirectory(main_path);

        if (index.Equals(0))
        {
            Destroy(illust.texture);
            yield return illust.texture = Files.Use.GetPNG(main_path, curData.en_name + ".png");
            illustCount = 0;
        }
        else if (index.Equals(1))
        {
            Destroy(illust.texture);
            yield return illust.texture = Files.Use.GetPNG(main_path, curData.en_name + "_elite2.png");
            illustCount = 1;

        }
        else if (index >= 2)
        {
            Destroy(illust.texture);
            yield return illust.texture = Files.Use.GetPNG(main_path, curData.en_name + "_c" + (index - 1).ToString() + ".png");
        }

        if (illust.texture != null)
        {
            illust.gameObject.SetActive(true);
            Download_btn.SetActive(false);
            loadPanel.SetActive(false);
        }
        else
        {
            loadPanel.SetActive(false);
            illust.gameObject.SetActive(false);
            Download_btn.SetActive(true);
        }

        illust.rectTransform.anchoredPosition = new Vector2(0, 0);
        illust.rectTransform.localScale = Vector2.one;
        illust_menu.transform.GetChild(0).GetComponent<Slider>().value = 1;
        yield return null;
    }
}
