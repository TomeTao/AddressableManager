#if !SPACE_DLL

using System;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace UnityEditor.AddressableManager.Builder
{
    public class AddressableCustomizeBuilder
    {
        private static string build_script
            = "Assets/AddressableAssetsData/DataBuilders/BuildScriptPackedMode.asset";
        private static string settings_asset
            = "Assets/AddressableAssetsData/AddressableAssetSettings.asset";
        private static string profile_name = "Default";
        private static AddressableAssetSettings settings;
       
        public static bool BuildAddressables(string settings= "",string profileName="",string dataBuilder="", bool clearCatch = true)
        {
            SetAddressableAssetSetting(String.IsNullOrEmpty(settings) ? settings_asset: settings);
            SetActiveProfile(String.IsNullOrEmpty(profileName) ? profile_name : profileName);
            IDataBuilder builderScript
                = AssetDatabase.LoadAssetAtPath<ScriptableObject>(String.IsNullOrEmpty(dataBuilder) ? build_script : dataBuilder) as IDataBuilder;
            if (builderScript == null)
            {
                Debug.LogError(build_script + " couldn't be found or isn't a build script.");
                return false;
            }
            SetActiveBuilderScript(builderScript);

            return BuildAddressableContent(clearCatch);
        }

        public static void BuildAddressablesAndPlayer(string settings = "", string profileName = "", string dataBuilder = "", bool clearCatch = true)
        {
            bool contentBuildSucceeded = BuildAddressables(settings, profileName, dataBuilder, clearCatch);

            if (contentBuildSucceeded)
            {
                var options = new BuildPlayerOptions();
                BuildPlayerOptions playerSettings
                    = BuildPlayerWindow.DefaultBuildMethods.GetBuildPlayerOptions(options);

                BuildPipeline.BuildPlayer(playerSettings);
            }
        }

        private static bool BuildAddressableContent(bool clearCatch=true)
        {
            if (clearCatch)
            {
                CustomDataBuilder.ClearScriptBuildPipelineCacheData();
                CustomDataBuilder.ClearCachedData(settings.ActivePlayModeDataBuilder);
            }
           
            AddressableAssetSettings
                .BuildPlayerContent(out AddressablesPlayerBuildResult result);
            bool success = string.IsNullOrEmpty(result.Error);
            if (!success)
            {
                Debug.LogError("Addressables build error encountered: " + result.Error);
            }
            return success;
        }

        private static void SetAddressableAssetSetting(string settingsAsset)
        {
            // This step is optional, you can also use the default settings:
            //settings = AddressableAssetSettingsDefaultObject.Settings;

            settings = AssetDatabase.LoadAssetAtPath<ScriptableObject>(settingsAsset)
                    as AddressableAssetSettings;
            if (settings == null)
            {
                Debug.LogError($"{settingsAsset} couldn't be found or isn't " +
                              $"a settings object.");
            }
            settings = AddressableAssetSettingsDefaultObject.Settings;
        }

        private static void SetActiveProfile(string profile)
        {
            string profileId = settings.profileSettings.GetProfileId(profile);
            if (String.IsNullOrEmpty(profileId))
                Debug.LogWarning($"Couldn't find a profile named, {profile}, " +
                                 $"using current profile instead.");
            else
                settings.activeProfileId = profileId;
        }

        private static void SetActiveBuilderScript(IDataBuilder builder)
        {
            CustomDataBuilder.SetCustomDataBuilder(builder);
        }
    }
}

#endif
