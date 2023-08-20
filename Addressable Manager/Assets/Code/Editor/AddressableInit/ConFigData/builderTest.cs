using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

public class PostProcessor : AssetPostprocessor
{
    const string STREAMINGASSETS_RAWDATA = "StreamingAssets_RawData";
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        foreach (string asset in importedAssets)
        {
            if (asset.IndexOf(STREAMINGASSETS_RAWDATA) < 0)
            {
                continue;
            }
            string name = asset.Remove(0, asset.IndexOf(STREAMINGASSETS_RAWDATA) + STREAMINGASSETS_RAWDATA.Length + 1);
            if (name.IndexOf("/") >= 0)
            {
                string groupname = name.Substring(0, name.IndexOf("/"));
                string assetname = name.Remove(0, name.IndexOf("/") + 1);
                var group = settings.FindGroup(groupname);
                if (group == null)
                {
                    var groupTemplate = settings.GetGroupTemplateObject(0) as AddressableAssetGroupTemplate;

                    // Group entry
                    AddressableAssetGroup newGroup = settings.CreateGroup(groupname, false, false, true, null, groupTemplate.GetTypes());
                    groupTemplate.ApplyToAddressableAssetGroup(newGroup);

                    AssetDatabase.SaveAssets();

                    group = settings.FindGroup(groupname);
                    if (group == null)
                    {
                        Debug.LogError($"创建群组失败. {groupname}");
                        continue;
                    }
                }
                string label = Path.GetDirectoryName(assetname).Replace('\\', '/');
                // Addressable entry
                var guid = AssetDatabase.AssetPathToGUID(asset);
                settings.CreateOrMoveEntry(guid, group);
                AssetDatabase.SaveAssets();
                var entry = group.GetAssetEntry(guid);
                // Simplify addressable name
                entry.SetAddress(Path.GetFileNameWithoutExtension(assetname));
                // Set Label
                if (string.IsNullOrEmpty(label) == false)
                {
                    entry.SetLabel(label, true, true);
                }
                AssetDatabase.SaveAssets();
            }
        }
    }
}