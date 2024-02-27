using UnityEngine;
using UnityEditor;

public class AssetBundleBuilder : MonoBehaviour
{
    [MenuItem("Bundles/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        BuildPipeline.BuildAssetBundles("Assets/AssetBundles", BuildAssetBundleOptions.None, BuildTarget.Android);
    }
}
