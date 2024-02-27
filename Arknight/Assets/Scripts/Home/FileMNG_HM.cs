using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UI;
using UnityEngine.UI;
public class FileMNG_HM : MonoBehaviour
{
    public CanvasScaler canvas;
    public RectTransform test;
    public GameObject UI;

    [Header("- UI Components")]
    public Text illust_path;
    public Text audioPath;

    public Toggle[] androidbarTog;
    bool initialize;

    public void Initialized()
    {
        initialize = true;
        CanvasScreen androidProgram = new CanvasScreen();
        bool[] active = androidProgram.setCanvasScreen(XML.This, canvas, test);

        androidbarTog[0].isOn = active[0];
        androidbarTog[1].isOn = active[1];

        illust_path.text = Files.Use.DocumentsPath("Resource/OperatorIllust/en_name + elite2.png");
        audioPath.text = Files.Use.DocumentsPath("Resource/OperatorAudio/character_name/audio.mp3");
        UI.SetActive(false);
        initialize = false;
    }

    public void TR_DeleteCashe()
    {
        bool success = Caching.ClearCache();
        if (success)
            Log_Manager.instance.Add_Log("캐시가 삭제되었습니다.");
        else Log_Manager.instance.Add_Log("캐시를 삭제할 수 없습니다.");
    }
    public void setAndroidSoftkey(Toggle target)
    {
        if (initialize)
            return;
        ApplicationChrome.setSystemUiVisibility(canvas, test, androidbarTog[0].isOn, androidbarTog[1].isOn);

        string save = string.Empty;
        if (androidbarTog[0].isOn) save += "1";
        else save += "0";
        if (androidbarTog[1].isOn) save += ",1";
        else save += ",0";

        XML.This.SetUIStatusBarXML(save);
    }
}
