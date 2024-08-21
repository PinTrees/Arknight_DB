using FilesInfo;
using Setting;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;


namespace FilesInfo
{
    public class IconFile
    {
        public string rare;
        public string en_name;
        public int icon;
        public string[] skill;
    }
    public class FILEInfo
    {
        public string name;
        public string folder;
        public string fileName;
        public string compulsion;
       
        public string fileCount;
    }
}

namespace Setting
{
    public class DataBaseSetting
    {
        public string resources_ver;
        public string audio_ver;
        public string stagedropDB_ver;

        public string DB_ver;
        public string apk_ver;
        public int isupdate;
        public int db_status;
        public string notice;
        public string apklink;
    }
    public class ClientSetting
    {
        public string DB_ver;
        public string onlineDB_use;
        public string updateDB;
    }

    public class Version
    {
        public string version;
    }
}

public class Download : MonoBehaviour
{
    public static Download Use;
    
    // Start is called before the first frame update
    void Awake()
    {
        Use = this;
    }
    private string pathForDocumentsFile(string filename)
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
    // ========================================================================================================
    // ========================================================================================================
    public IEnumerator ResourcesDel(string ver, Image bar_main, Image bar, Text fileName, Text value, Text value_main, GameObject UI)
    {
        bool success = Caching.ClearCache();
        if (!StatusChecking.instance.InternetNetworkStatus())
        {
            Log_Manager.instance.Add_Log("인터넷 연결을 확인해 주세요.");
            yield return null;
        }

        UI.SetActive(true);
        yield return new WaitForEndOfFrame();

       string fileUrl = string.Format("{0}{1}", "https://raw.githubusercontent.com/PinTrees/Arknight_DB/master/Version/Info/", "OpIconFile.xml");
        string filePath = pathForDocumentsFile("Version_Info");

        yield return StartCoroutine(DownLoad_VersionInfo(bar, fileName, value));
        yield return StartCoroutine(DownLoad(fileUrl, filePath, "OpIconFile.xml", bar, value, "NULL"));

        List<FILEInfo> opIconFile = XML.This.Get_FileInfo("OpIconFile.xml");
        List<FILEInfo> enemyFile = XML.This.Get_FileInfo("EnemyFile.xml");
        List<FILEInfo> evnetFile = XML.This.Get_FileInfo("EventFile.xml");
        FileDelete(enemyFile, "Enemy");
        FileDelete(evnetFile, "Event");
        FileCheck_OpIcon(opIconFile, "Del");

        yield return Resources(ver, bar_main, bar, fileName, value, value_main, UI);
    }
    public IEnumerator Resources(string ver, Image bar_main, Image bar, Text fileName, Text value, Text value_main, GameObject UI)
    {
        bool success = Caching.ClearCache();
        if (!StatusChecking.instance.InternetNetworkStatus())
        {
            Log_Manager.instance.Add_Log("인터넷 연결을 확인해 주세요.");
            yield break;
        }
        UI.SetActive(true);
        yield return new WaitForEndOfFrame();

        for (int i = 1; i <= 6; i++)
        {
            string p = string.Format("{0}/{1}", pathForDocumentsFile("Resource/OperatorIcon"), i);
            Files.Use.DeleteFolders(p, null);
        }
        yield return new WaitForEndOfFrame();

        UI.SetActive(true);

        fileName.text = "다운로드 : 파일 정보";
        yield return StartCoroutine(DownLoad_VersionInfo(bar, fileName, value));
        yield return new WaitForSeconds(.1f);

        string fileUrl = string.Format("{0}{1}", "https://raw.githubusercontent.com/PinTrees/Arknight_DB/master/Version/Info/", "OpIconFile.xml");
        string filePath = pathForDocumentsFile("Version_Info");
        fileName.text = "다운로드 : 오퍼레이터 아이콘 파일 정보";
        yield return StartCoroutine(DownLoad(fileUrl, filePath, "OpIconFile.xml", bar, value, "NULL"));
        yield return new WaitForSeconds(.1f);
        // ----------------------------------------------------------------------------------------------------
        int curCount = 0; int fileCount = 0;

        fileName.text = "오퍼레이터 아이콘 파일 확인중...";
        yield return new WaitForSeconds(.1f);
        List<FILEInfo> downloadFiles = XML.This.Get_FileInfo("OpIconFile.xml");

        FileCheck_OpIcon(downloadFiles);
        string url = "https://raw.githubusercontent.com/PinTrees/Arknight_DB/master/Resources/OperatorIcon";
        string path = pathForDocumentsFile("Resource/OperatorIcon");

        for (int i = 0; i < downloadFiles.Count; i++)
        {
            if (downloadFiles[i].compulsion == "1")
                fileCount++;
        }

        fileName.text = "오퍼레이터 아이콘 다운로드중...";
        yield return new WaitForSeconds(.1f);
        for (int i = 0; i < downloadFiles.Count; i++)
        {
            if (downloadFiles[i].compulsion != "1")
                continue;

            string curUrl = string.Format("{0}/{1}/{2}", url, downloadFiles[i].folder, downloadFiles[i].fileName);
            string curPath = string.Format("{0}/{1}", path, downloadFiles[i].folder);
            StartCoroutine(DownLoad(curUrl, curPath, downloadFiles[i].fileName, bar, value, "PNG"));

            curCount++;
            value_main.text = string.Format("{0}%", Mathf.RoundToInt((curCount / (float)fileCount) * 100));
            bar_main.fillAmount = curCount / (float)fileCount;
            yield return new WaitForSeconds(.005f);
        }

        // DownLoad File Check ----------------------------------------------------------------------------------------------------
        curCount = 0;
        fileName.text = "오퍼레이터 아이콘 파일 설치 확인중...";
        yield return new WaitForSeconds(.1f);
        while (curCount < fileCount)
        {
            curCount = 0;
            for (int i = 0; i < downloadFiles.Count; i++)
            {
                if (downloadFiles[i].compulsion != "1")
                    continue;

                string curPath = string.Format("{0}/{1}/{2}", path, downloadFiles[i].folder, downloadFiles[i].fileName);

                FileInfo fileInfo = new FileInfo(curPath);
                if (!fileInfo.Exists)
                    continue;

                curCount++;
                value_main.text = string.Format("{0}%", Mathf.RoundToInt((curCount / (float)fileCount) * 100));
                bar_main.fillAmount = curCount / (float)fileCount;
            }
            yield return new WaitForSeconds(.03f);
        }
        yield return new WaitForEndOfFrame();
        // ----------------------------------------------------------------------------------------------------
        string resourcesUrl = "https://raw.githubusercontent.com/PinTrees/Arknight_DB/master/Resources";
        string enemyiconPath = StatusChecking.instance.pathForDocumentsFile("Resource/EnemyIcon");
        string event_path = StatusChecking.instance.pathForDocumentsFile("Resource/EventBanner");
        // EventFile Download =======================================================================================
        string banner_url = string.Format("{0}/Event/{1}", resourcesUrl, "Banner.png");
        string banner_path = event_path;
        fileName.text = "배너 이미지 다운로드중...";
        yield return new WaitForSeconds(.1f);
        yield return StartCoroutine(DownLoad(banner_url, banner_path, "Banner.png", bar, value, "PNG"));
        // EnemyFile Download =======================================================================================
        curCount = 0; fileCount = 0;
        fileName.text = "적 아이콘 파일 확인중...";
        yield return new WaitForSeconds(.1f);
        List<FILEInfo> enemyFile = XML.This.Get_FileInfo("EnemyFile.xml");
        bool check = FileCheck(enemyFile, "Enemy");

        for (int i = 0; i < enemyFile.Count; i++)
            if (enemyFile[i].compulsion.Equals("1"))
                fileCount++;

        fileName.text = "적 아이콘 파일 다운로드중...";
        yield return new WaitForSeconds(.1f);

        for (int i = 0; i < enemyFile.Count; i++)
        {
            if (!enemyFile[i].compulsion.Equals("1"))
                continue;

            string curUrl = string.Format("{0}/{1}/{2}", resourcesUrl, "EnemyIcon", enemyFile[i].name);
            string curPath = Files.Use.DocumentsPath("Resource/EnemyIcon");
            StartCoroutine(DownLoad(curUrl, curPath, enemyFile[i].name, bar, value, "NULL"));

            curCount++;
            value_main.text = string.Format("{0}%", Mathf.RoundToInt((curCount / (float)fileCount) * 100));
            bar_main.fillAmount = curCount / (float)fileCount;
            yield return new WaitForSeconds(.01f);
        }

        curCount = 0;
        fileName.text = "적 아이콘 파일 설치 확인중...";
        yield return new WaitForSeconds(.1f);
        while (curCount < fileCount)
        {
            curCount = 0;
            for (int i = 0; i < enemyFile.Count; i++)
            {
                if (enemyFile[i].compulsion != "1")
                    continue;

                string curPath = string.Format("{0}/{1}", enemyiconPath, enemyFile[i].name);

                FileInfo fileInfo = new FileInfo(curPath);
                if (!fileInfo.Exists)
                    continue;

                curCount++;
                value_main.text = string.Format("{0}%", Mathf.RoundToInt((curCount / (float)fileCount) * 100));
                bar_main.fillAmount = curCount / (float)fileCount;
            }
            yield return new WaitForSeconds(.03f);
        }
        yield return new WaitForEndOfFrame();
        // =============================================================================================================
        XML.This.Save_VersionCode(ver, "null");

        UI.SetActive(false);
        yield return null;
    }
    public IEnumerator OperatorIllust(string ver, Image bar_main, Image bar, Text fileName, Text value, Text value_main, GameObject UI)
    {
        bool success = Caching.ClearCache();

        if (!StatusChecking.instance.InternetNetworkStatus())
        {
            Log_Manager.instance.Add_Log("인터넷 연결을 확인해 주세요.");
            yield return null;
        }

        UI.SetActive(true);
        yield return StartCoroutine(DownLoad_IlustFileInfo(bar, fileName, value));
        yield return new WaitForSeconds(.01f);

        string resources_url = "https://raw.githubusercontent.com/PinTrees/Arknight_DB/master/Resources";
        string illust_path = StatusChecking.instance.pathForDocumentsFile("Resource/OperatorIllust");

        int fileCount = 0;
        int curCount = 0;

        fileName.text = "일러스트 파일 확인중...";
        yield return new WaitForSeconds(.1f);
        List<FILEInfo> downloadFiles = XML.This.Get_FileInfo("OperatorIllustFile.xml");
        yield return StartCoroutine(FileCheckIllust(downloadFiles, ver, bar_main, value_main));
        if (ver.Equals("ALL"))
            for (int i = 0; i < downloadFiles.Count; i++)
                downloadFiles[i].compulsion = "1";

        fileName.text = "일러스트 파일 다운로드중...";
        yield return new WaitForSeconds(.1f);
        for (int i = 0; i < downloadFiles.Count; i++)
            if (downloadFiles[i].compulsion.Equals("1"))
                fileCount++;

        for (int i = 0; i < downloadFiles.Count; i++)
        {
            if (!downloadFiles[i].compulsion.Equals("1"))
                continue;

            string cur_url = string.Format("{0}/OperatorIllust/{1}", resources_url, downloadFiles[i].name);
            StartCoroutine(DownLoad(cur_url, illust_path, downloadFiles[i].name, bar, value, "PNG"));

            curCount++;
            value_main.text = string.Format("{0}%", Mathf.RoundToInt((curCount / (float)fileCount) * 100));
            bar_main.fillAmount = curCount / (float)fileCount;
            yield return new WaitForSeconds(.01f);
        }

        fileName.text = "일러스트 파일 설치중...";
        curCount = 0;
        while (curCount < fileCount)
        {
            curCount = 0;
            for (int i = 0; i < downloadFiles.Count; i++)
            {
                if (!downloadFiles[i].compulsion.Equals("1"))
                    continue;

                string curPath = string.Format("{0}/{1}", pathForDocumentsFile("Resource/OperatorIllust"), downloadFiles[i].name);
                FileInfo fileInfo = new FileInfo(curPath);
                if (!fileInfo.Exists)
                    continue;

                curCount++;
                fileName.text = "파일 설치 완료 :  " + downloadFiles[i].name + " PNG";

                value_main.text = string.Format("{0}%", Mathf.RoundToInt((curCount / (float)fileCount) * 100));
                bar_main.fillAmount = curCount / (float)fileCount;
            }
            yield return new WaitForSeconds(.1f);
        }
        UI.SetActive(false);
        yield return null;
    }
    private bool FileCheck_OpIcon(List<FILEInfo> data, string type="NULL")
    {
        string main_path = pathForDocumentsFile("Resource/OperatorIcon");
        if (!Directory.Exists(main_path))
            Directory.CreateDirectory(main_path);

        if(type.Equals("Del"))
        {
            for (int i = 0; i < data.Count; i++)
            {
                string cur_path = string.Format("{0}/{1}", main_path, data[i].folder);
                if (!Directory.Exists(cur_path))
                    Directory.CreateDirectory(cur_path);

                string filePath = string.Format("{0}/{1}/{2}", main_path, data[i].folder, data[i].fileName);
                FileInfo fileInfo = new FileInfo(filePath);

                if (fileInfo.Exists)
                    File.Delete(filePath);
            }

            return true;
        }

        for (int i = 0; i < data.Count; i++)
        {
            string cur_path = string.Format("{0}/{1}", main_path, data[i].folder);
            if (!Directory.Exists(cur_path))
                Directory.CreateDirectory(cur_path);

            string filePath = string.Format("{0}/{1}/{2}", main_path, data[i].folder, data[i].fileName);
            FileInfo fileInfo = new FileInfo(filePath);

            if (!fileInfo.Exists)
            {
                data[i].compulsion = "1";
                continue;
            }
            else
            {
                if (data[i].compulsion.Equals("1"))
                    continue;
                else
                    data[i].compulsion = "0";
            }
        }

        return true;
    }
    private bool FileCheck(List<FILEInfo> data, string type, Image bar=null, Text value=null)
    {
        string main_path = string.Empty;
        if (type.Equals("Event"))
            main_path = StatusChecking.instance.pathForDocumentsFile("Resource/EventBanner");
        else if (type.Equals("Enemy"))
            main_path = StatusChecking.instance.pathForDocumentsFile("Resource/EnemyIcon");
        else if (type.Equals("DB"))
            main_path = pathForDocumentsFile("Resource/DB");

        if (!Directory.Exists(main_path))
            Directory.CreateDirectory(main_path);

        for (int i = 0; i < data.Count; i++)
        {
            string cur_path = string.Format("{0}/{1}", main_path, data[i].name);

            FileInfo fileInfo = new FileInfo(cur_path);
            if (!fileInfo.Exists)
            {
                data[i].compulsion = "1";
                continue;
            }
            else
            {
                if (data[i].compulsion.Equals("1"))
                {
                    File.Delete(cur_path);
                    continue;
                }
                else
                    data[i].compulsion = "0";
            }
        }
        return true;
    }
    private void FileDelete(List<FILEInfo> data, string type)
    {
        string main_path = string.Empty;
        if (type.Equals("Event"))
            main_path = StatusChecking.instance.pathForDocumentsFile("Resource/EventBanner");
        else if (type.Equals("Enemy"))
            main_path = StatusChecking.instance.pathForDocumentsFile("Resource/EnemyIcon");
        else if (type.Equals("DB"))
            main_path = pathForDocumentsFile("Resource/DB");

        if (!Directory.Exists(main_path))
            Directory.CreateDirectory(main_path);

        for (int i = 0; i < data.Count; i++)
        {
            string cur_path = string.Format("{0}/{1}", main_path, data[i].name);

            FileInfo fileInfo = new FileInfo(cur_path);
            if (!fileInfo.Exists)
                continue;
            else
                File.Delete(cur_path);
        }
    }
    IEnumerator FileCheckIllust(List<FILEInfo> data, string type, Image bar, Text value)
    {
        string main_path = pathForDocumentsFile("Resource/OperatorIllust");

        if (!Directory.Exists(main_path))
            Directory.CreateDirectory(main_path);

        int curCount = 0;
        for (int i = 0; i < data.Count; i++)
        {
            string cur_path = string.Format("{0}/{1}", main_path, data[i].name);

            FileInfo fileInfo = new FileInfo(cur_path);
            if (!fileInfo.Exists)
                data[i].compulsion = "1";
            else if (data[i].compulsion.Equals("1"))
                File.Delete(cur_path);

            if (type.Equals("ALL"))
                File.Delete(cur_path);

            curCount++;
            value.text = string.Format("{0}%", Mathf.RoundToInt((curCount / (float)data.Count) * 100));
            bar.fillAmount = curCount / (float)data.Count;
            yield return new WaitForEndOfFrame();
        }
    }
    public IEnumerator DownLoad(string url, string path, string filename, Image bar, Text value, string type)
    {
        //Debug.Log(url);
        UnityWebRequest request = UnityWebRequest.Get(url);
        //WWW www = new WWW(url);

        bar.fillAmount = 0; 
        if(value != null) value.text = "0%";
        request.SendWebRequest();
        while (!request.isDone)
        {
            if (value != null) value.text = string.Format("{0}{1}", Mathf.RoundToInt(request.downloadProgress * 100), "%");
            bar.fillAmount = request.downloadProgress / 1.0f;
            yield return new WaitForEndOfFrame();
        }
        bar.fillAmount = 1; if (value != null) value.text = "100%";

        if (request.isNetworkError)
            yield break;
        else if (request.isHttpError)
            yield break; 

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        if (type.Equals("PNG") || type.Equals("MP3"))
        {
            //Texture2D save = new Texture2D(www.texture.width, www.texture.height, TextureFormat.ARGB32, false);
            //save.SetPixels(0, 0, www.texture.width, www.texture.height, www.texture.GetPixels());
            //save.Apply();
            //byte[] bytes = save.EncodeToPNG();

            System.IO.File.WriteAllBytes(path + "/" + filename, request.downloadHandler.data);
        }
        else if (type.Equals("ZIP"))
            File.WriteAllBytes(path + "/" + filename + ".zip", request.downloadHandler.data);
        else if (type.Equals("XML"))
            File.WriteAllBytes(path + "/" + filename + ".xml", request.downloadHandler.data);
        else
            System.IO.File.WriteAllBytes(path + "/" + filename, request.downloadHandler.data);

        yield return null;
    }
    public IEnumerator EventFile(string url, string path, string filename, Image bar, RawImage image)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);

        bar.fillAmount = 0; 
        request.SendWebRequest();
        while (!request.isDone)
        {
            bar.fillAmount = request.downloadProgress / 1.0f;
            yield return new WaitForSeconds(.05f);
        }

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        System.IO.File.WriteAllBytes(path + "/" + filename, request.downloadHandler.data);
        bar.fillAmount = 0;

        Texture2D texture = new Texture2D(0, 0);
        texture.LoadImage(request.downloadHandler.data);
        image.texture = texture;
        yield return null;
    }
    // ============================================================================================================================
    public IEnumerator getTextureFormWWW(string url, RawImage target, Image bar, Text value)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        DownloadHandlerTexture texDl = new DownloadHandlerTexture(true);
        request.downloadHandler = texDl;

        bar.fillAmount = 0; value.text = "0%";
        request.SendWebRequest();
        while (!request.isDone)
        {
            value.text = string.Format("{0}{1}", Mathf.RoundToInt(request.downloadProgress * 100), "%");
            bar.fillAmount = request.downloadProgress / 1.0f;
            yield return new WaitForEndOfFrame();
        }
        bar.fillAmount = 1; value.text = "100%";

        if (!(request.isNetworkError || request.isHttpError))
        {
            Destroy(target.texture);
            target.texture = texDl.texture;
        }
    }
    public IEnumerator getAssetBundleFromGitHub(string url, string assetbundleName, Image bar, TextMeshProUGUI value)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        DownloadHandlerTexture texDl = new DownloadHandlerTexture(true);
        request.downloadHandler = texDl;

        bar.fillAmount = 0; value.text = "0%";
        request.SendWebRequest();
        while (!request.isDone)
        {
            value.text = string.Format("{0}{1}", Mathf.RoundToInt(request.downloadProgress * 100), "%");
            bar.fillAmount = request.downloadProgress / 1.0f;
            yield return new WaitForEndOfFrame();
        }
        bar.fillAmount = 1; value.text = "100%";

        if (!(request.isNetworkError || request.isHttpError))
        {
            string folderpath = pathForDocumentsFile("Resource/Spine");
            if (!Directory.Exists(folderpath))
                Directory.CreateDirectory(folderpath);
            string filePath = folderpath + "/" + assetbundleName;
            System.IO.File.WriteAllBytes(filePath, request.downloadHandler.data);
        }
    }
    // OperatorAudio Files ========================================================================================================
    public IEnumerator OperatorAudio(string ver, Image bar_main, Image bar, Text fileName, Text value, Text value_main, GameObject UI)
    {
        bool success = Caching.ClearCache();

        if (!StatusChecking.instance.InternetNetworkStatus())
        {
            Log_Manager.instance.Add_Log("인터넷 연결을 확인해 주세요.");
            yield break;
        }

        string resourcesUrl = "https://raw.githubusercontent.com/PinTrees/Arknight_DB/master/Resources";
        string fileInfoUrl = "https://raw.githubusercontent.com/PinTrees/Arknight_DB/master/Version/Info/OperatorAudioFile.xml";
        string audioPath = pathForDocumentsFile("Resource/OperatorAudio");
        string infoPath = pathForDocumentsFile("Version_Info");
        int fileCount = 0;
        int curCount = 0;

        UI.SetActive(true);
        yield return StartCoroutine(DownLoad(fileInfoUrl, infoPath, "OperatorAudioFile.xml", bar, value, "NULL"));
        yield return new WaitForSeconds(.1f);

        // File Check ========================================================================================================
        fileName.text = "오디오 파일 확인중...";
        yield return new WaitForSeconds(.1f);
        List<FILEInfo> downloadFiles = XML.This.Get_FileInfo("OperatorAudioFile.xml");
        yield return StartCoroutine(FileCheckAudio(downloadFiles, ver, bar_main, value_main));

        // ====================================================================================================================
        // DownLoad Data Refresh ========================================================================================================
        if (ver.Equals("ALL"))
            for (int i = 0; i < downloadFiles.Count; i++)
                downloadFiles[i].compulsion = "1";
        else
        {
            for (int i = 0; i < downloadFiles.Count; i++)
                if (!downloadFiles[i].compulsion.Equals("1"))
                {
                    downloadFiles.RemoveAt(i--);
                }
        }
        fileCount = downloadFiles.Count;

        // DownLoad Start ========================================================================================================
        fileName.text = "오디오 파일 다운로드중...";
        yield return new WaitForSeconds(.1f);
        for (int i = 0; i < downloadFiles.Count; i++)
        {
            string downUrl = string.Format("{0}/OperatorAudio/{1}/{2}", resourcesUrl, downloadFiles[i].folder, downloadFiles[i].fileName);
            string savePatn = string.Format("{0}/{1}", audioPath, downloadFiles[i].folder);     
            StartCoroutine(DownLoad(downUrl, savePatn, downloadFiles[i].fileName, bar, value, "NULL"));
           
            curCount++;
            value_main.text = string.Format("{0}%", Mathf.RoundToInt((curCount / (float)fileCount) * 100));
            bar_main.fillAmount = curCount / (float)fileCount;
            yield return null;
        }

        // ============================================================================================================================
        // DownLoad File Check ========================================================================================================
        fileName.text = "오디오 파일 설치 확인중...";  
        yield return new WaitForSeconds(.1f);
        curCount = 0;
        while (curCount < fileCount)
        {
            curCount = 0;
            for (int i = 0; i < downloadFiles.Count; i++)
            {
                string curPath = string.Format("{0}/{1}/{2}", pathForDocumentsFile("Resource/OperatorAudio"), downloadFiles[i].folder, downloadFiles[i].fileName);
                FileInfo fileInfo = new FileInfo(curPath);
                if (!fileInfo.Exists)
                    continue;

                curCount++;
                value_main.text = string.Format("{0}%", Mathf.RoundToInt((curCount / (float)fileCount) * 100));
                bar_main.fillAmount = curCount / (float)fileCount;

                if (value_main.text.Equals("100%"))
                {
                    fileCount = 0;
                    break;
                }
            }
            yield return new WaitForSeconds(.1f);
        }

        // ===========================================================================================================================
        XML.This.Save_VersionCode("null", ver);
        UI.SetActive(false);
        yield return null;
    }
    IEnumerator FileCheckAudio(List<FILEInfo> data, string type, Image bar, Text value)
    {
        string main_path = pathForDocumentsFile("Resource/OperatorAudio");

        if (!Directory.Exists(main_path))
            Directory.CreateDirectory(main_path);

        int curCount = 0;
        for (int i = 0; i < data.Count; i++)
        {
            string cur_path = string.Format("{0}/{1}", main_path, data[i].folder);

            if (!Directory.Exists(cur_path))
                Directory.CreateDirectory(cur_path);

            string filePath = string.Format("{0}/{1}", cur_path, data[i].fileName);
            FileInfo fileInfo = new FileInfo(filePath);
            if (!fileInfo.Exists)
                data[i].compulsion = "1";
            else
            {
                if (data[i].compulsion.Equals("1"))
                    File.Delete(cur_path);
            }
            curCount++;
            value.text = string.Format("{0}%", Mathf.RoundToInt((curCount / (float)data.Count) * 100));
            bar.fillAmount = curCount / (float)data.Count;
            yield return null;
        }
    }
    // ========================================================================================================
    public IEnumerator DownLoad_DataBase(string ver, Image bar, Text fileName, Text value, GameObject panel)
    {
        if (!StatusChecking.instance.InternetNetworkStatus())
        {
            Log_Manager.instance.Add_Log("인터넷 연결을 확인해 주세요.");
            yield break;
        }

        panel.SetActive(true);

        string fileUrl = string.Format("{0}{1}", "https://raw.githubusercontent.com/PinTrees/Arknight_DB/master/Version/Info/", "DBFile.xml");
        string filePath = pathForDocumentsFile("Version_Info");
        fileName.text = "다운로드 : DB 파일 정보";
        yield return StartCoroutine(DownLoad(fileUrl, filePath, "DBFile.xml", bar, value, "NULL"));
        yield return new WaitForSeconds(.01f);

        fileName.text = "DB 파일 확인중...";
        yield return new WaitForSeconds(.1f);
        List<FILEInfo> downloadFiles = XML.This.Get_FileInfo("DBFile.xml");

        string url = string.Format("https://raw.githubusercontent.com/PinTrees/Arknight_DB/master/DB/{0}", ver);
        string path = pathForDocumentsFile("Resource/DB");

        fileName.text = "DB 파일 다운로드 준비중...";
        yield return new WaitForSeconds(.1f);
        for (int i = 0; i < downloadFiles.Count; i++)
        {
            string curUrl = string.Format("{0}/{1}", url, downloadFiles[i].name);
            StartCoroutine(DownLoad(curUrl, path, downloadFiles[i].name, bar, value, "NULL"));

            value.text = string.Format("{0}%", Mathf.RoundToInt(((i + 1) / (float)downloadFiles.Count) * 100));
            bar.fillAmount = (i + 1) / (float)downloadFiles.Count;
            yield return new WaitForSeconds(.01f);
        }

        string url_1 = string.Format("{0}{1}", "https://raw.githubusercontent.com/PinTrees/Arknight_DB/master/Version/Info/", "verF_operatorAudio.xml");
        string path_1 = pathForDocumentsFile("Version_Info");
        fileName.text = "오퍼레이터 대사 파일 정보 다운로드중";
        yield return StartCoroutine(DownLoad(url_1, path_1, "verF_operatorAudio.xml", bar, value, "NULL"));
        yield return new WaitForSeconds(.01f);
        fileName.text = "오퍼레이터 대사 파일 다운로드중...";
        yield return new WaitForSeconds(.1f);

        string mainUrl = "https://raw.githubusercontent.com/PinTrees/Arknight_DB/master/Resources/OperatorAudio";
        List<FILEInfo> audioDB = XML.This.Get_FileInfo("verF_operatorAudio.xml");

        for (int i = 0; i < audioDB.Count; i++)
        {
            string downUrl = string.Format("{0}/{1}/{2}", mainUrl, audioDB[i].folder, audioDB[i].fileName);
            string curPath = Files.Use.DocumentsPath("Resource/DB/operator/" + audioDB[i].folder);

            if (audioDB[i].compulsion.Equals("1"))
            {
                StartCoroutine(DownLoad(downUrl, curPath, audioDB[i].fileName, bar, value, "NULL"));
                continue;
            }

            if (!Directory.Exists(curPath))
                Directory.CreateDirectory(curPath);

            string file_path = string.Format("{0}/{1}", curPath, audioDB[i].fileName);

            FileInfo fileInfo = new FileInfo(file_path);
            if (fileInfo.Exists)
                if (!audioDB[i].fileName.Equals("ProJeKT_RED_Audio.xml"))
                    continue;

            StartCoroutine(DownLoad(downUrl, curPath, audioDB[i].fileName, bar, value, "NULL"));
            value.text = string.Format("{0}%", Mathf.RoundToInt(((i + 1) / (float)audioDB.Count) * 100));
            bar.fillAmount = ((i + 1) / (float)audioDB.Count);
            yield return new WaitForSeconds(0.01f);
        }

        ClientSetting now = new ClientSetting();
        Load_Setting_Prefs(now);
        if (ver.Equals("NEW"))
            now.DB_ver = StatusChecking.instance.setting.DB_ver;
        else now.DB_ver = ver;
        Save_Setting_Prefs(now);
        StatusChecking.instance.user_setting = now;

        panel.SetActive(false);
        yield return null;
    }
    // ========================================================================================================
    // FileInfo Download ========================================================================================================
    public IEnumerator DownLoad_VersionInfo(Image bar, Text fileName, Text value)
    {
        string url = string.Format("{0}{1}", "https://raw.githubusercontent.com/PinTrees/Arknight_DB/master/Version/Info/", "OperatorIcon.xml");
        string path = pathForDocumentsFile("Version_Info");

        fileName.text = "다운로드 : 오퍼레이터 파일 정보";
        yield return StartCoroutine(DownLoad(url, path, "OperatorIcon", bar, value, "XML"));

        url = string.Format("{0}{1}", "https://raw.githubusercontent.com/PinTrees/Arknight_DB/master/Version/Info/", "EventFile.xml");
        path = pathForDocumentsFile("Version_Info");
        fileName.text = "다운로드 : 이벤트 파일 정보";
        yield return StartCoroutine(DownLoad(url, path, "EventFile", bar, value, "XML"));

        url = string.Format("{0}{1}", "https://raw.githubusercontent.com/PinTrees/Arknight_DB/master/Version/Info/", "EnemyFile.xml");
        path = pathForDocumentsFile("Version_Info");
        fileName.text = "다운로드 : 적 이미지 파일 정보";
        yield return StartCoroutine(DownLoad(url, path, "EnemyFile", bar, value, "XML"));

        yield return new WaitForSeconds(.1f);
    }
    public IEnumerator DownLoad_IlustFileInfo(Image bar, Text fileName, Text value)
    {
        string url = string.Format("{0}{1}", "https://raw.githubusercontent.com/PinTrees/Arknight_DB/master/Version/Info/", "OperatorIllustFile.xml");
        string path = pathForDocumentsFile("Version_Info");
        fileName.text = "다운로드 : 오퍼레이터 일러스트 파일 정보";
        yield return StartCoroutine(DownLoad(url, path, "OperatorIllustFile", bar, value, "XML"));

        yield return null;
    }
    // ========================================================================================================
    public void Load_Setting_Prefs(ClientSetting set)
    {
        //PlayerPrefs.DeleteKey("Setting");
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
        Debug.Log(PlayerPrefs.GetString("Setting"));
        string[] arr = PlayerPrefs.GetString("Setting").Split(',');

        set.DB_ver = arr[0];
        set.onlineDB_use = arr[1];
        set.updateDB = arr[2];
    }
    public void Save_Setting_Prefs(ClientSetting set)
    {
        string saveData = string.Empty;
        saveData += set.DB_ver + ",";
        saveData += set.onlineDB_use + ",";
        saveData += set.updateDB;

        PlayerPrefs.SetString("Setting", saveData);
        PlayerPrefs.Save();
    }
}
