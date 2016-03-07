using UnityEditor;

public class AssetBundlePostprocessor : AssetPostprocessor
{
    /// <summary>AssetBundleの設定変更検知</summary>
    /// <param name="assetPath">変更されたAssetのPath</param>
    /// <param name="previousAssetBundleName">変更前のAssetBundle名</param>
    /// <param name="newAssetBundleName">変更後のAssetBundle名</param>
    public void OnPostprocessAssetbundleNameChanged(
        string assetPath,
        string previousAssetBundleName,
        string newAssetBundleName)
    {
        var importerList = AssetBundleNameManager.ImporterList;
        if (importerList == null) { return; }
        var asset = importerList.Find(_ => _.assetPath == assetPath);
        var importer = AssetImporter.GetAtPath(assetPath);
        if (asset == null)
        {
            importerList.Add(importer);
        }
        else
        {
            if (!string.IsNullOrEmpty(importer.assetBundleName))
            {
                asset = importer;
            }
            else
            {
                importerList.Remove(asset);
            }
        }
        AssetBundleNameManager.GUIUpdate();
    }
}