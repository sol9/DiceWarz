using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

[Serializable]
[InlineProperty(LabelWidth = 13)]
public struct grid2d
{
    [HorizontalGroup]
    public int x, y;

    public grid2d(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public grid3d xyh(int h = 0) => new grid3d(x, y, h);

    public static bool operator ==(grid2d a, grid2d b) => a.x == b.x && a.y == b.y;
    public static bool operator !=(grid2d a, grid2d b) => !(a == b);

    public static implicit operator grid3d(grid2d g) => g.xyh(0);

    public static implicit operator Vector2(grid2d g) => new(g.x, g.y);

    public override string ToString() => $"({x}, {y})";
}

[Serializable]
[InlineProperty(LabelWidth = 13)]
public struct grid3d
{
    [HorizontalGroup]
    public int x, y, h;

    public grid3d(int x, int y, int h)
    {
        this.x = x;
        this.y = y;
        this.h = h;
    }

    public grid2d xy => new grid2d(x, y);

    public static bool operator ==(grid3d a, grid3d b) => a.x == b.x && a.y == b.y && a.h == b.h;
    public static bool operator !=(grid3d a, grid3d b) => !(a == b);

    public static grid3d operator *(grid3d a, grid3d b) => new(a.x * b.x, a.y * b.y, a.h * b.h);

    public static Vector3 operator *(Vector3 v, grid3d g) => new(v.x * g.x, v.y * g.h, v.z * g.y);
    public static Vector3 operator *(grid3d g, Vector3 v) => v * g;

    public static implicit operator Vector3(grid3d g) => new(g.x, g.h, g.y);

    public override string ToString() => $"({x}, {y}, {h})";
}

[Serializable]
public class cell : SerializedMonoBehaviour
{
    [ReadOnly]
    public floor parent;
    
    [ReadOnly]
    public grid3d coord;

    [ReadOnly]
    public block placed;

    public grid2d xy => coord.xy;
    public grid3d xyh => coord;

    public bool valid => parent && parent.valid;

    public areaConfigs configs => valid ? parent.configs : default;

    public bool hasBlock => placed;

    public void Dispose()
    {
        if (hasBlock)
        {
            placed.Dispose();
            placed = null;

            if (Application.isPlaying)
                Destroy(gameObject);
            else
                DestroyImmediate(gameObject);
        }
    }

    public bool pushBlock(block block)
    {
        if (hasBlock)
            return false;

        if (!block)
            return false;

        block.onPushed(this);
        placed = block;
        return true;
    }

    public block popBlock()
    {
        if (!hasBlock)
            return null;

        var pop = placed;
        placed = null;

        pop.onPopped();
        return pop;
    }

    [HorizontalGroup("func", Title = "placed Block")]
    [DisableInPlayMode]
    [EnableIf("@valid && !hasBlock")]
    [Button(SdfIconType.PlusSquareFill, "push")]
    public void createBlock()
    {
        if (!valid || hasBlock)
            return;

        placed = Instantiate(configs.defaultBlockPrefabs, transform);
        placed.parent = this;
    }

    [HorizontalGroup("func")]
    [DisableInPlayMode]
    [EnableIf("hasBlock")]
    [Button(SdfIconType.XSquareFill, "pop")]
    public void deleteBlock()
    {
        if (!hasBlock)
            return;

        placed.Dispose();
        placed = null;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (valid && !hasBlock)
        {
            Gizmos.color = new Color(1, 1, 1, 0.05f);
            Gizmos.DrawWireCube(transform.position, configs.unitSize);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (valid && Selection.activeGameObject == gameObject)
        {
            Handles.Label(transform.position, coord.ToString());

            if (!hasBlock)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(transform.position, configs.unitSize);
            }
        }
    }
#endif

    public static cell makeCell(floor parent, grid3d coord, bool makeBlock)
    {
        if (!parent)
            return default;

        var cell = new GameObject(coord.xy.ToString()).AddComponent<cell>();
        cell.parent = parent;
        cell.coord = coord;

        if (makeBlock)
            cell.createBlock();
        
        cell.transform.parent = parent.transform;
        return cell;
    }
}
