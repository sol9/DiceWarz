using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Freya;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class floor : SerializedMonoBehaviour, IDisposable
{
    [ReadOnly]
    public area parent;
    
    [ReadOnly]
    public grid2d size;
    
    [ReadOnly]
    public int height;

    [ReadOnly]
    public List<cell> cells;

    public bool valid => parent && parent.valid
        && parent.size.xy == size
        && cells.valid() && cells.Count == count;

    public areaConfigs configs => valid ? parent.configs : default;

    public int count => size.x * size.y;

    public cell this[int x, int y]
    {
        get
        {
            var i = x + y * size.x;
            return valid && cells.valid(i) ? cells[i] : default;
        }
    }

    public void Dispose()
    {
        if (cells.valid())
        {
            cells.ForEach(g => g.Dispose());
            cells.Clear();

            if (Application.isPlaying)
                Destroy(gameObject);
            else
                DestroyImmediate(gameObject);
        }
    }


#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (valid && Selection.activeGameObject == gameObject)
        {
            Handles.Label(transform.position, height.ToString());

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, size.xyh(1) * configs.unitSize);
        }
    }
#endif

    public static floor makeFloor(area parent, grid2d size, int height, bool makeBlock)
    {
        if (!parent)
            return default;

        var count = size.x * size.y;
        if (count <= 0)
            return default;
        
        var floor = new GameObject(height.ToString()).AddComponent<floor>();
        floor.parent = parent;
        floor.size = size;
        floor.height = height;

        var offset = new Vector3((size.x - 1) * -0.5f, 0, (size.y - 1) * -0.5f);
        floor.cells = Enumerable.Range(0, count).Select(i =>
        {
            var coord = new grid2d(i % size.x, i / size.x);
            var child = cell.makeCell(floor, coord.xyh(height), makeBlock);
            child.transform.localPosition = (offset + coord.xyh()).scale(parent.unitSize);
            return child;
        }).ToList();

        floor.transform.parent = parent.gameObject.transform;
        floor.transform.localPosition = new Vector3{ y = (height + 0.5f) * parent.unitSize.y };
        return floor;
    }
}
