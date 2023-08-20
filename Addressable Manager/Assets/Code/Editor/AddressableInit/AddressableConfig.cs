using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;

//存在添加资源进组时候不能自动移动出Resources文件夹中
//不能手动更新UI界面
// 构建Addressable 资源管理
public static class AddressableConfig
{
    static string AddressableAssetsDataFilePath = "Assets/AddressableAssetsData";
    static string ConFigDatasFilePath
    {
        get
        {
            return "Assets/Code/ConFigDatas/" +
#if UNITY_STANDALONE
    "Standalone/";
#elif UNITY_MOBILE
    "Mobile/";
#elif UNITY_VR
    "VR/";
#endif
        }
    }

    static AddressableAssetSettings Settings
    {
        get { return AddressableAssetSettingsDefaultObject.Settings; }
    }

    [MenuItem("Test/Refreash")]
    public static void Refreash()
    {
        RevertConfigToDefault();
        ConfigGroups();
    }

    //清除所有
    public static void RevertConfigToDefault()
    {
        if (Directory.Exists(AddressableAssetsDataFilePath))
            AssetDatabase.DeleteAsset(AddressableAssetsDataFilePath);
        AssetDatabase.Refresh();
        if (!AddressableAssetSettingsDefaultObject.SettingsExists)
        {
            AddressableAssetSettingsDefaultObject.GetSettings(true);
        }
        //Addressables.InitializeAsync();
        AssetDatabase.Refresh();
    }

    public static void ConfigGroups()
    {
        var assetsPath = Directory.GetFiles(ConFigDatasFilePath, "*.asset");
        foreach (var asset in assetsPath)
        {
            var m_group = AssetDatabase.LoadAssetAtPath<AddressableGroup>(asset);
            if (m_group != null)
            {
               var group= CreateGroup<SchemaType>(m_group.name);
                foreach(var item in m_group.assets)
                {
                    var guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(item._assetReference));
                    var lable = item._label.ToString();
                    if(!String.IsNullOrEmpty(guid))
                    {
                        AddAssetEntry(group, guid, lable);
                    }
                }
            }
        }
        //Settings.CreateGroup();
        AssetDatabase.Refresh();
    }

    // 创建分组
    static AddressableAssetGroup CreateGroup<T>(string groupName, bool noSchema=true)
    {
        AddressableAssetGroup group = Settings.FindGroup(groupName);
        if (group == null)
        {
            if (noSchema)
            {
                group = Settings.CreateGroup(groupName, false, false, false, null);
            }
            else
            {
                group = Settings.CreateGroup(groupName, false, false, false, null, new Type[]{
                    typeof(BundledAssetGroupSchema),
                    typeof(ContentUpdateGroupSchema)
                });
            }
        }
            
        Settings.AddLabel(groupName, false);
        return group;
    }

    // 给某分组添加资源
    static AddressableAssetEntry AddAssetEntry(AddressableAssetGroup group, string guid, string lable)
    {
        AddressableAssetEntry entry = group.entries.FirstOrDefault(e => e.guid == guid);
        if (entry == null)
        {
            entry = Settings.CreateOrMoveEntry(guid, group, false, false);
        }
        entry.SetLabel(group.Name, true, false, false);
        entry.SetAddress(GetSampleAddress(AssetDatabase.GUIDToAssetPath(guid)));
        return entry;
    }

    static void RefreashUI()
    {

    }

    static string GetSampleAddress(string path)
    {
        var newAddress = path;
        if(path.Contains("Resource"))
        {
            var startIndex = path.LastIndexOf("Resource");
            newAddress = path.Substring(startIndex);
        }
        return newAddress;
    }

    static void SetGroupSchema(AddressableAssetGroup group)
    {
        var groupSchema = group.GetSchema<BundledAssetGroupSchema>();
        groupSchema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackTogetherByLabel;
        groupSchema.Compression = BundledAssetGroupSchema.BundleCompressionMode.LZ4;
        //var fieldInfo = groupSchema.GetType().GetField("m_AssetBundleProviderType", BindingFlags.Instance | BindingFlags.NonPublic);
        //fieldInfo?.SetValue(groupSchema, new SerializedType { Value = typeof(LuaBundleProvider) });

        //var fieldInfo2 = groupSchema.GetType().GetField("m_BundledAssetProviderType", BindingFlags.Instance | BindingFlags.NonPublic);
        //fieldInfo2?.SetValue(groupSchema, new SerializedType { Value = typeof(LuaBundleProvider) });
    }

    private static void AddSettingsLabels(AddressableAssetSettings setting, List<string> list)
    {
        foreach (var label in list)
        {
            // 添加
            setting.AddLabel(label, true);
        }
    }
}

