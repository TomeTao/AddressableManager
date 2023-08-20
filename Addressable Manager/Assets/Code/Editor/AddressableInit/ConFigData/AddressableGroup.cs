using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Addressables/CreatGroup")] 
public class AddressableGroup : ScriptableObject 
{
    [SerializeField] public List<AddressableAssetReference> assets;
}
