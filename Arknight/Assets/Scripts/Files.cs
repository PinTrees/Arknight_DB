using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Spine.Unity;
using UnityEngine.Networking;

public class Files : MonoBehaviour
{
    public static Files Use;

    bool platformAndroid = false;
    // Start is called before the first frame update
    void Awake()
    {
        Use = this;

        if (Application.platform == RuntimePlatform.Android)
            platformAndroid = true;
    }
    public string DocumentsPath(string filename)
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
    public IEnumerator GetAudioFile(string path, string filename, AudioSource data)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        string paths = string.Format("{0}{1}{2}", path, "/", filename);
        //Debug.Log(paths);
        FileInfo fileInfo = new FileInfo(paths);

        if (fileInfo.Exists)
        {
            WWW www = new WWW(@"file://" + paths);
            while(!www.isDone)
            {
                yield return new WaitForSeconds(.01f);
            }
            data.clip = www.GetAudioClip(false, false);
        }

        yield return null;
    }
    public Texture2D GetPNG(string path, string filename)
    {
        // 에셋 번들을 저장할 경로의 폴더가 존재하지 않는다면 생성시킨다.
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        string paths = string.Format("{0}{1}{2}", path, "/", filename);
        //Debug.Log(paths);
        FileInfo fileInfo = new FileInfo(paths);

        if (fileInfo.Exists)
        {
            //Debug.Log(path);
            //path = string.Format("{0}{1}", "file://", path);
            //WWW www = new WWW(path);
            //yield return www;
            //set[i].defualt_ilust = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));
            //yield return set[i].defualt_ilust;
            byte[] byteTexture = System.IO.File.ReadAllBytes(paths);
            Texture2D texture = new Texture2D(0, 0);
            texture.LoadImage(byteTexture);
            return texture;
        }
        else
        {
            return null;
        }
    }
    public IEnumerator GetTextAsset(string path, string filename, TextAsset get)
    {
        WWW www = new WWW(@"file://" + path + "/" + filename);
        yield return www;

        AssetBundle bundle = www.assetBundle;
        Debug.Log(www.text);
        TextAsset target = bundle.LoadAsset<TextAsset>(filename);

        if (get == null)
        {
            get = target;
            Debug.Log(get);
        }
    }
    public void SaveFile(string path, string filename, byte[] data)
    {
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        //Texture2D save = new Texture2D(www.texture.width, www.texture.height, TextureFormat.ARGB32, false);
        //save.SetPixels(0, 0, www.texture.width, www.texture.height, www.texture.GetPixels());
        //save.Apply();
        //byte[] bytes = save.EncodeToPNG();

        System.IO.File.WriteAllBytes(path + "/" + filename, data);
    }
    public IEnumerator RefreshFileName(string path, string oldName, string newName)
    {
        FileInfo file = new FileInfo(path + "/" + oldName);
        file.MoveTo(path + "/" + newName);
        //file.Delete();

        yield return null;
    }
    public IEnumerator DeleteFolder(string path, GameObject UI)
    {
        if (!Directory.Exists(path))
        {
            if (UI != null)
            {
                UI.SetActive(false);
                yield return new WaitForEndOfFrame();
            }
            yield break;
        }
        string[] allFiles = Directory.GetFiles(path);
        for (int i = 0; i < allFiles.Length; i++)
        {
            File.Delete(allFiles[i]);
        }

        if (UI != null)
        {
            UI.SetActive(false);
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForEndOfFrame();
        Directory.Delete(path);
    }
    public IEnumerator DeleteFolders(string path, GameObject UI)
    {
        if (!Directory.Exists(path))
        {
            if (UI != null)
            {
                UI.SetActive(false);
                yield return new WaitForEndOfFrame();
            }
            yield break;
        }

        DirectoryInfo di = new DirectoryInfo(path);
        foreach(var item in di.GetDirectories())
        {
            string filePath = string.Format("{0}/{1}", path, item.Name);
            yield return DeleteFolder(filePath, UI);
        }

        Directory.Delete(path);

        if (UI != null)
        {
            UI.SetActive(false);
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForEndOfFrame();
    }
    public Texture2D ScreenShot()
    {
        int width = Screen.width;
        int height = Screen.height;

        int status = 0, navigation = 0;
        string[] ch = XML.This.GetUIStatusBarXML();

        if (platformAndroid)
        {
            if (ch[0].Equals("1")) status = ApplicationChrome.getStatusbarHeight();
            if (ch[1].Equals("1")) navigation = ApplicationChrome.getNavigationBarHeight();
        }

        //StartCoroutine( LogU.Use.SetLog(status.ToString() + ", " + navigation.ToString()));
        Texture2D tex = new Texture2D(width, height - (navigation + status), TextureFormat.RGB24, true);
        tex.ReadPixels(new Rect(0, navigation, width, height), 0, 0);
        tex.Apply();

        return tex;
    }
    public IEnumerator getSpineFile(string path, string name, SkeletonAnimation get)
    {
        get.gameObject.SetActive(false);
        
        AssetBundle bundle = AssetBundle.LoadFromFile(path);
        //AssetBundleRequest abr = bundle.LoadAssetAsync<TextAsset>(name + ".skel");
        //yield return abr;
        //TextAsset skeletonJSON = (TextAsset)abr.asset;

        TextAsset skeletonJSON = bundle.LoadAsset(name + ".skel") as TextAsset;
        TextAsset atlasFile = bundle.LoadAsset(name + ".atlas") as TextAsset;
        Material atlasMaterial = bundle.LoadAsset(name + "_Material") as Material;
        atlasMaterial.shader = Shader.Find(atlasMaterial.shader.name);

        Material[] materialElement = { atlasMaterial }; 

        //Debug.Log(skeletonJSON);
        //Debug.Log(atlasFile);
        AtlasAsset runtimeAtlasAsset = AtlasAsset.CreateRuntimeInstance(atlasFile, materialElement, true); // seems to fail
        SkeletonDataAsset runtimeSkeletonDataAsset = SkeletonDataAsset.CreateRuntimeInstance(skeletonJSON, runtimeAtlasAsset, true);  // create a non-readable SkeletonDataAsset

        Destroy(get.gameObject.GetComponent<MeshFilter>().mesh);
        get.skeletonDataAsset = runtimeSkeletonDataAsset;
        get.AnimationName = string.Empty;
        get.Initialize(true);
        get.AnimationState.ClearTracks();

        Spine.ExposedList<Spine.Animation> animationList = runtimeSkeletonDataAsset.GetSkeletonData(true).Animations;

        get.AnimationState.SetAnimation(0, animationList.Items[0].Name, true);
        get.gameObject.SetActive(true);

        bundle.Unload(false);
        yield return null;
    }
    public bool FileCheck(string path, string filename)
    {
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        string paths = string.Format("{0}{1}{2}", path, "/", filename);
        //Debug.Log(paths);
        FileInfo fileInfo = new FileInfo(paths);

        return fileInfo.Exists;
    }
}
