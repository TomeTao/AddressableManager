using UnityEditor;
using UnityEditor.AddressableManager.Builder;
using UnityEngine;

public static class BuilderInEditor
{

    [MenuItem("Test/Build Addressable bundle")]
    public static void PreAddressableBuild()
    {
        bool result = AddressableCustomizeBuilder.BuildAddressables();
        Debug.Log(result);
    }
}
