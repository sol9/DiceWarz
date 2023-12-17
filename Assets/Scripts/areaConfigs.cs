using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "area/configs", fileName = "configs")]
public class areaConfigs : SerializedScriptableObject
{
    [Title("block")]
    public grid3d unitSize;
    
    [AssetsOnly]
    public block defaultBlockPrefabs;
}
