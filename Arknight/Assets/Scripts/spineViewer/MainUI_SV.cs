using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Spine.Unity;
using TMPro;
using DataClass;
using UI;
class OperatorIconSV
{
    public GameObject _this;
    public RawImage icon;
    public Image downloadImg;
    public TextMeshProUGUI name;
    public TextMeshProUGUI youtobeUrl;
    public OperatorSpine data;
    public bool fileC;
    public void Initialize(Transform target)
    {
        _this = target.gameObject;
        icon = target.GetChild(0).GetChild(0).GetComponent<RawImage>();
        name = target.GetChild(1).GetComponent<TextMeshProUGUI>();
        youtobeUrl = target.GetChild(3).GetComponent<TextMeshProUGUI>();
        downloadImg = target.GetChild(5).GetComponent<Image>();
    }
    public void Refresh(OperatorSpine set, Texture2D _icon, bool filecheck)
    {
        Debug.Log(set.assetbundleName);
        data = set;
        icon.texture = _icon;
        name.text = data.made;
        youtobeUrl.text = data.Yurl;
        if (filecheck) downloadImg.color = new Color(100 / 255f, 1, 1);
        else downloadImg.color = new Color(1, 100 / 255f, 100 / 255f);

        fileC = filecheck;
        if (!_this.activeSelf)
            _this.SetActive(true);
    }
    public void RefreshDownloadImg(bool flag)
    {
        if (flag) downloadImg.color = new Color(100 / 255f, 1, 1);
        else downloadImg.color = new Color(1, 100 / 255f, 100 / 255f);

        fileC = flag;
    }
}
public class MainUI_SV : MonoBehaviour
{
    public GameObject downloadCheckPanel;
    public GameObject downloadPanel;
    public Image bar;
    public TextMeshProUGUI value;

    public GameObject spinePanel;
    public Transform buttonViewerTr;
    public Transform viewerTr;
    public SkeletonAnimation spineAnimator;
    List<OperatorSpine> contentsData;
    List<OperatorIconSV> contentsUI;

    List<GameObject> aniButton;
    CanvasScaler canvasScreen;
    RectTransform rect;
    int downloadIndex;
    void Start()
    {
        Initialized();
    }
    public void Initialized()
    {
        aniButton = new List<GameObject>();
        contentsData = new List<OperatorSpine>();
        contentsUI = new List<OperatorIconSV>();

        canvasScreen = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<CanvasScaler>();
        rect = canvasScreen.transform.GetChild(0).GetComponent<RectTransform>();

# if UNITY_ANDROID
        CanvasScreen androidProgram = new CanvasScreen();
        androidProgram.setCanvasScreen(XML.This, canvasScreen, rect);
#endif
        XML.This.Get_OperatorSpineAssetList_F(contentsData);

        for(int i = 0; i < viewerTr.childCount; i++)
        {
            OperatorIconSV tmp = new OperatorIconSV();
            tmp.Initialize(viewerTr.GetChild(i));
            contentsUI.Add(tmp);
        }
        for (int i = 0; i < buttonViewerTr.childCount; i++)
        {
            aniButton.Add(buttonViewerTr.GetChild(i).gameObject);
        }
        //StartCoroutine(Files.Use.getSpineFile(Files.Use.DocumentsPath("Resource/Spine/yamin_ceylon"), "ceylon", spineAnimator));
        //Clear();
        Refresh();
        spineAnimator.gameObject.SetActive(false);
        spinePanel.SetActive(false);
    }
    public void Clear()
    {
        for (int i = 0; i < contentsUI.Count; i++)
            contentsUI[i]._this.SetActive(false);
    }
    public void Refresh()
    {
        for (int i = 0; i < contentsUI.Count; i++)
        {
            if (i >= contentsData.Count)
            {
                contentsUI[i]._this.SetActive(false);
                continue;
            }
            string path = string.Format("{0}/{1}", Files.Use.DocumentsPath("Resource/OperatorIcon"), contentsData[i].parent);
            Texture2D icon = Files.Use.GetPNG(path, "Icon.png");
            bool fileC = Files.Use.FileCheck(Files.Use.DocumentsPath("Resource/Spine"), contentsData[i].assetbundleName);
            contentsUI[i].Refresh(contentsData[i], icon, fileC);
        }
    }

    public void StartUI_SpineAnimation(Transform target)
    {
        int index = target.GetSiblingIndex();

        bool fileC = Files.Use.FileCheck(Files.Use.DocumentsPath("Resource/Spine"), contentsData[index].assetbundleName);
        if (!fileC)
        {
            StartCoroutine(StartDownloadAndStart(index));
            return;
        }
        StartSpineAnimation(index);
    }
    public void StartSpineAnimation(int index)
    {
        spinePanel.SetActive(true);
        StartCoroutine(Files.Use.getSpineFile(Files.Use.DocumentsPath("Resource/Spine/" + contentsData[index].assetbundleName), contentsData[index].assetlavelName, spineAnimator));
        spineAnimator.transform.localScale = contentsData[index].scale;
        spineAnimator.transform.position = contentsData[index].position;
        spineAnimator.gameObject.SetActive(true);

        Spine.ExposedList<Spine.Animation> animaitonList = spineAnimator.skeletonDataAsset.GetSkeletonData(true).Animations;
        for (int i = 0; i < aniButton.Count; i++)
        {
            if (i >= animaitonList.Count) aniButton[i].SetActive(false);
            else aniButton[i].SetActive(true);
        }
    }
    public void ExiteUI_SpineAnimation()
    {
        spineAnimator.gameObject.SetActive(false);
        spinePanel.SetActive(false);
    }
    IEnumerator StartDownloadAndStart(int index)
    {
        downloadPanel.SetActive(true);
        yield return StartCoroutine(Download.Use.getAssetBundleFromGitHub("https://raw.githubusercontent.com/PinTrees/Arknight_DB/master/asset/spine_user/" + contentsData[index].assetbundleName
            , contentsData[index].assetbundleName, bar, value));
        downloadPanel.SetActive(false);
        bool fileC = Files.Use.FileCheck(Files.Use.DocumentsPath("Resource/Spine"), contentsData[index].assetbundleName);
        contentsUI[index].RefreshDownloadImg(fileC);
        StartSpineAnimation(index);
    }
    public void TR_DownloadAsset(Transform target)
    {
        downloadIndex = target.GetSiblingIndex();
        bool fileC = Files.Use.FileCheck(Files.Use.DocumentsPath("Resource/Spine"), contentsData[downloadIndex].assetbundleName);
        if (fileC)
            downloadCheckPanel.SetActive(true);
    }
    public void TR_Download()
    {
        downloadCheckPanel.SetActive(false);
        StartCoroutine(StartDownloadAndStart(downloadIndex));
    }
    public void TR_YouTubeURL(Transform target)
    {
        int index = target.GetSiblingIndex();
        Application.OpenURL(contentsData[index].Yurl);
    }
    public void TR_Live2dURL()
    {
        Application.OpenURL("https://pinforest.imweb.me/live2d");
    }
    public void TR_SetAnimation(Transform target)
    {
        int index = target.GetSiblingIndex();
        Spine.ExposedList<Spine.Animation> animaitonList = spineAnimator.skeletonDataAsset.GetSkeletonData(true).Animations;

        spineAnimator.AnimationState.ClearTracks();
        spineAnimator.AnimationState.SetAnimation(0, animaitonList.Items[index].Name, true);
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (spinePanel.activeSelf)
            {
                spineAnimator.gameObject.SetActive(false);
                spinePanel.SetActive(false);
            }
            else
                UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
        }
    }
}
