#if !SPACE_DLL

using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.Build.Pipeline.Utilities;
using UnityEngine;

namespace UnityEditor.AddressableManager.Builder
{
    internal class CustomDataBuilder
    {
        public static void SetCustomDataBuilder(IDataBuilder builder)
        {
            AddressableAssetSettings settings
                = AddressableAssetSettingsDefaultObject.Settings;

            int index = settings.DataBuilders.IndexOf((ScriptableObject)builder);
            if (index > 0)
                settings.ActivePlayerDataBuilderIndex = index;
            else if (AddressableAssetSettingsDefaultObject.Settings.AddDataBuilder(builder))
                settings.ActivePlayerDataBuilderIndex
                    = AddressableAssetSettingsDefaultObject.Settings.DataBuilders.Count - 1;
            else
                Debug.LogWarning($"{builder} could not be found " +
                                 $"or added to the list of DataBuilders");
        }

        public static void ClearCachedData(IDataBuilder builder)
        {
            builder.ClearCachedData();
        }

        public static void ClearScriptBuildPipelineCacheData(bool prompt=false)
        {
            BuildCache.PurgeCache(prompt);
        }
    }
}
#endif