using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class Audio
{
    public string code;
    public string name;
    public string info;
}

public class AudioUI_Op : MonoBehaviour
{
    private Canvas UI;

    public ScrollRect scrollRect;
    public Transform viewer;
    public GameObject Menu;
    public Text info;
    public Image progressBar;

    [Header("- UI Transform")]
    public Transform menuTr;

    [Header("- UI Components")]
    public Slider soundSld;
    public RawImage sound;
    public RawImage play;

    [Header("- Loding UI Components")]
    public GameObject loadUI;
    public Text loadValue;
    public Image loadBar;

    [Header("- UI Static")]
    public Texture2D playIcon;
    public Texture2D pauseIcon;
    public Texture2D soundIcon;
    public Texture2D muteIcon;

    private Text[] audiobtn_txt;
    private Image[] audiobtn_img;

    private List<Audio> audioData;

    public string[] parentName;       // [0] now, [1] old;

    [Header("- UI Object")]
    public GameObject warning;

    public AudioSource audioSource;
    public Audio curData;

    private bool isPlay;
    private bool isMute;
    private bool isRefresh = false;
    private Coroutine coroutine;

    private Color[] colorsA;           // [0] img, [1] txt
    private Color[] colorsN;           // [0] img, [1] txt
    public void Initialized()
    {
        isRefresh = false;

        parentName = new string[2];
        parentName[0] = string.Empty;
        parentName[1] = string.Empty;

        colorsA = new Color[2];
        colorsA[0] = new Color(100 / 255f, 100 / 255f, 100 / 255f, 0.4f);
        colorsA[1] = new Color(200 / 255f, 200 / 255f, 200 / 255f, 0.4f);
        colorsN = new Color[2];
        colorsN[0] = new Color(100 / 255f, 100 / 255f, 100 / 255f, 1f);
        colorsN[1] = new Color(200 / 255f, 200 / 255f, 200 / 255f, 1f);

        audioData = new List<Audio>();
        audiobtn_txt = new Text[viewer.childCount];
        audiobtn_img = new Image[viewer.childCount];
        for (int i = 0; i < viewer.childCount; i++)
        {
            audiobtn_img[i] = viewer.GetChild(i).GetComponent<Image>();
            audiobtn_txt[i] = viewer.GetChild(i).GetChild(0).GetComponent<Text>();
        }
        // private GetComponents Data ===============================================
        UI = this.gameObject.GetComponent<Canvas>();
        // =============================================================
        // Refresh Data ===============================================
        isPlay = true;
        isMute = false;
        soundSld.value = 1;
        // Refresh UI Data ===============================================
        loadUI.SetActive(false);
        UI.enabled = false;
        Menu.SetActive(false);
        info.transform.parent.gameObject.SetActive(false);
    }
    public void Clear()
    {
        audioData.Clear();
    }
    public void SetData(string set)
    {
        parentName[1] = parentName[0];
        parentName[0] = set;
    }
    public void TR_Close()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
        curData = null;
        audioSource.clip = null;
        UI.enabled = false;
    }
    // UI Corutine Start ================================================================
    public IEnumerator UIStart_Main()
    {
        Clear();
        warning.SetActive(false);

        int rest = XML.This.Get_AudioData(parentName[0], audioData);
        if (rest.Equals(-1))
        {
            warning.SetActive(true);
            Menu.SetActive(false);
            info.transform.parent.gameObject.SetActive(false);
            UI.enabled = true;
            yield break;
        }

        for (int i = 0; i < audiobtn_txt.Length; i++)
        {
            if (i >= audioData.Count)
            {
                audiobtn_txt[i].transform.parent.gameObject.SetActive(false);
                continue;
            }
            audiobtn_txt[i].text = audioData[i].name;
            audiobtn_txt[i].transform.parent.gameObject.SetActive(true);
        }
        scrollRect.content.anchoredPosition = new Vector2(scrollRect.content.position.x, 0);
        // Refresh Data ===============================================
        isPlay = true;
        audioSource.volume = soundSld.value;
        // Refresh UI Data ============================================
        if (!isMute && soundSld.value > 0) sound.texture = soundIcon;
        else sound.texture = muteIcon;
        if (isPlay) play.texture = pauseIcon;
        else play.texture = playIcon;

        Menu.SetActive(true);
        info.transform.parent.gameObject.SetActive(false);
        UI.enabled = true;

        isRefresh = true;
        StartCoroutine(UIRefresh_Button());
        yield return null;
    }
    IEnumerator UIRefresh_Button()
    {
        if (!isRefresh)
            yield break;

        for (int i = 0; i < audioData.Count; i++)
        {
            if (curData == audioData[i])
            {
                audiobtn_img[i].color = Color.black;
                audiobtn_txt[i].color = Color.white;
                continue;
            }
            audiobtn_img[i].color = colorsN[0];
            audiobtn_txt[i].color = colorsN[1];
        }

        yield return new WaitForEndOfFrame();
        isRefresh = false;
    }
    // UI Corutine Exit ================================================================
    public void TR_SetSoundMenu(int type)
    {
        if (type.Equals(0))         // play, pause
        {
            isPlay = !isPlay;
            if (!isPlay)
                audioSource.Pause();
            else audioSource.Play();
        }
        else if (type.Equals(1))    //  mute
        {
            isMute = !isMute;
            audioSource.mute = isMute;
        }

        if (!isMute && soundSld.value > 0) sound.texture = soundIcon;
        else sound.texture = muteIcon;
        if (isPlay) play.texture = pauseIcon;
        else play.texture = playIcon;
    }
    public void TR_SetVolume(Slider slider)
    {
        audioSource.volume = slider.value;
        if (slider.value < 0)
            sound.texture = muteIcon;
        else
            audioSource.mute = false;
        // Refresh UI Data ============================================
        if (!isMute && soundSld.value > 0) sound.texture = soundIcon;
        else sound.texture = muteIcon;
    }
    public void TR_AudioStart(Transform set)
    {
        Destroy(audioSource.clip);

        int index = set.GetSiblingIndex();
        curData = audioData[index];

        for (int i = 0; i < audioData.Count; i++)
        {
            if (i != index)
            {
                audiobtn_img[i].color = colorsA[0];
                audiobtn_txt[i].color = colorsA[1];
            }
            else
            {
                audiobtn_img[i].color = Color.black;
                audiobtn_txt[i].color = Color.white;
            }
        }

        isPlay = true;
        if (isPlay) play.texture = pauseIcon;
        else play.texture = playIcon;

        if (coroutine != null)
            StopCoroutine(coroutine);

        coroutine = StartCoroutine(StartAudio());
        
        isRefresh = true;
    }
    public IEnumerator LoadAudio()
    {
        string code = curData.code;
        string path = string.Format("{0}/{1}", Files.Use.DocumentsPath("Resource/OperatorAudio"), parentName[0]);

        yield return Files.Use.GetAudioFile(path, code + ".mp3", audioSource);
        if (audioSource.clip == null)
        {
            loadUI.SetActive(true);
            yield return new WaitForEndOfFrame();

            string url = string.Format("{0}/{1}/{2}.mp3", "https://raw.githubusercontent.com/PinTrees/Arknight_DB/master/Resources/OperatorAudio", parentName[0], code);
            string savePath = string.Format("{0}/{1}", Files.Use.DocumentsPath("Resource/OperatorAudio"), parentName[0]);

            UnityEngine.Networking.UnityWebRequest request = UnityEngine.Networking.UnityWebRequest.Get(url);

            loadBar.fillAmount = 0; loadValue.text = "0%";
            request.SendWebRequest();
            while (!request.isDone)
            {
                loadValue.text = string.Format("{0}{1}", Mathf.RoundToInt(request.downloadProgress * 100), "%");
                loadBar.fillAmount = request.downloadProgress / 1.0f;
                yield return null;
            }
            loadBar.fillAmount = 1; loadValue.text = "100%";

            if (!System.IO.Directory.Exists(savePath))
                System.IO.Directory.CreateDirectory(savePath);

            System.IO.File.WriteAllBytes(savePath + "/" + code + ".mp3", request.downloadHandler.data);
            
            loadUI.SetActive(false);
            yield return Files.Use.GetAudioFile(path, code + ".mp3", audioSource);
        }
    }
    public IEnumerator StartAudio()
    {
        yield return StartCoroutine(LoadAudio());
        info.text = curData.info;
        info.transform.parent.gameObject.SetActive(true);

        if (audioSource.clip == null)
        {
            coroutine = null;
            yield break;
        }
        audioSource.Play();

        progressBar.gameObject.SetActive(true);
        while (audioSource.time < audioSource.clip.length)
        {
            progressBar.fillAmount = (audioSource.time / audioSource.clip.length);
            yield return new WaitForSeconds(0.01f);
        }
        progressBar.gameObject.SetActive(false);

        Destroy(audioSource.clip);
        coroutine = null;
        yield return null;
    }
    public bool IsActive()
    {
        return UI.enabled;
    }

}
