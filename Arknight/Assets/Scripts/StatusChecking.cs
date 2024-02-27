using UnityEngine;

using System.IO;
using Setting;

public class StatusChecking : MonoBehaviour
{
    public static StatusChecking instance;
    public DataBaseSetting setting;
    public ClientSetting user_setting;
    public Version version;
    public void Awake()
    {
        Application.targetFrameRate = 60;
        instance = this;
    }
    public void Initialized()
    {
        user_setting = new ClientSetting();
        XML.This.Get_UserSetting(user_setting);
    }
    public bool InternetNetworkStatus()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            return false;
        }
        else if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
        {
            return true;
        }
        else
        {
            return true;
        }
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
    public bool FileCheck()
    {
        bool ret = true;

        string path = pathForDocumentsFile("Operator_Info.xml");
        FileInfo fileInfo = new FileInfo(path);
        if (!fileInfo.Exists)
            ret = false;
        path = pathForDocumentsFile("Operator_DefaultInfo.xml");
        fileInfo = new FileInfo(path);
        if (!fileInfo.Exists)
            ret = false;
        path = pathForDocumentsFile("Operator_Elite1Info.xml");
        fileInfo = new FileInfo(path);
        if (!fileInfo.Exists)
            ret = false;
        path = pathForDocumentsFile("Operator_Elite2Info.xml");
        fileInfo = new FileInfo(path);
        if (!fileInfo.Exists)
            ret = false;
        path = pathForDocumentsFile("Operator_SkillInfo.xml");
        fileInfo = new FileInfo(path);
        if (!fileInfo.Exists)
            ret = false;
        return ret;
    }
}
