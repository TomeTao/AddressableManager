#if !SPACE_DLL

using UnityEngine;

namespace UnityEditor.AddressableManager.Builder
{
    public class BuildAddressablesInsertBuilder
    {
        /// <summary>
        /// Run a clean build before export.
        /// </summary>
        public static bool PreAddressableBuild()
        {
            Debug.Log("BuildAddressablesProcessor.PreExport start");
            bool result = AddressableCustomizeBuilder.BuildAddressables();
            Debug.Log("BuildAddressablesProcessor.PreExport done");
            return result;
        }

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            BuildPlayerWindow.RegisterBuildPlayerHandler(BuildPlayerHandler);
        }

        private static void BuildPlayerHandler(BuildPlayerOptions options)
        {
            if (EditorUtility.DisplayDialog("Build with Addressables",
                "Do you want to build a clean addressables before export?",
                "Build with Addressables", "Skip"))
            {
                var result= PreAddressableBuild();
                if(result)
                {
                    BuildPlayerWindow.DefaultBuildMethods.BuildPlayer(options);
                }
            }
            else
                BuildPlayerWindow.DefaultBuildMethods.BuildPlayer(options);
        }
    }
}
#endif