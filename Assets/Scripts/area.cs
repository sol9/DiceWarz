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

    public Vector3 up = Vector3.up;

    [ReadOnly]
    public List<floor> floors;

    public grid3d unitSize => configs.unitSize;
    public block blockPrefab => configs.defaultBlockPrefabs;

    public bool valid => floors.valid();

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
    public void makeArea(grid3d size)
    {
        Dispose();
        
        floors = Enumerable.Range(0, size.h)
            .Select(h => floor.makeFloor(this, size.xy, h))
            .ToList();
    }

    [Button(SdfIconType.TrashFill)]
    public void clearAll()
    {
        Dispose();
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        // if (Selection.activeGameObject == gameObject)
        //     Vertx.Debugging.D.raw<Vertx.Debugging.Shape.Arrow>(new(transform.position, up));
    }
#endif
}
