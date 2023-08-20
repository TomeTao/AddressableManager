using System;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

[Serializable]
public class AddressableAssetReference 
{
    [FormerlySerializedAs("AssetReference")][SerializeField] public Object _assetReference;
    [FormerlySerializedAs("Label")][SerializeField] public AssetReferenceLabels _label;
}

[Serializable]
public enum AssetReferenceLabels
{
    prefab,
    texture,
    material,
    shader,
    audio,
}
