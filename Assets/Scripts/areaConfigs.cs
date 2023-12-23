using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "area/configs", fileName = "configs")]
public class areaConfigs : SerializedScriptableObject
{
    [Title("block")]
    public Vector3 unitSize;
    
    [AssetsOnly]
    public block defaultBlockPrefabs;
}
