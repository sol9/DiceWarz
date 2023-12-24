using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class area : SerializedMonoBehaviour, IDisposable
{
    [InlineEditor]
    public areaConfigs configs;

    [ReadOnly]
    public List<floor> floors;

    [InfoBox("invalidated area, need to re-build", InfoMessageType.Error, VisibleIf = "@!valid")]
    public bool valid => configs && count > 0
        && floors.valid() && floors.Count == size.h;

    [ShowInInspector, ReadOnly]
    public grid3d size { get; private set; }
    public int count => size.x * size.y * size.h;

    public Vector3 unitSize => configs.unitSize;
    public block blockPrefab => configs.defaultBlockPrefabs;

    public void Dispose()
    {
        if (floors.valid())
        {
            floors.ForEach(f => f.Dispose());
            floors.Clear();
        }
    }

    [Title("build")]
    [Button(SdfIconType.Grid3x3GapFill)]
    public void makeArea(grid3d size, bool makeBlock)
    {
        Dispose();
        
        floors = Enumerable.Range(0, size.h)
            .Select(h => floor.makeFloor(this, size.xy, h, makeBlock))
            .ToList();

        this.size = size;
    }

    [Button(SdfIconType.TrashFill)]
    public void clearAll()
    {
        Dispose();
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (valid && Selection.activeGameObject == gameObject)
        {
            Handles.Label(transform.position, size.ToString());

            var volume = size * configs.unitSize;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position.changeTo(y: volume.y * 0.5f), volume);
        }
    }
#endif
}
