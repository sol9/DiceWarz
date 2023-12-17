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

    public int count => size.x * size.y;

    public cell this[int x, int y]
    {
        get
        {
            var i = x + y * size.x;
            return valid && cells.valid(i) ? cells[i] : default;
        }
    }

    public bool valid => parent && cells.valid() && cells.Count == count;

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
        if (Selection.activeGameObject == gameObject)
            Handles.Label(transform.position, height.ToString());
    }
#endif

    public static floor makeFloor(area parent, grid2d size, int height)
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
            var child = cell.makeCell(floor, coord.xyh(height), parent.blockPrefab);
            child.transform.localPosition = (offset + coord.xyh()) * parent.unitSize;
            return child;
        }).ToList();

        floor.transform.parent = parent.gameObject.transform;
        floor.transform.localPosition = new Vector3{ y = (height + 0.5f) * parent.unitSize.h };
        return floor;
    }
}
