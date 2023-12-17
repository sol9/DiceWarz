using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class block : SerializedMonoBehaviour, IDisposable
{
    [ReadOnly, InlineEditor]
    public cell parent;

    public enum state
    {
    }

    public bool isPlaced => parent != null;

    public grid3d coord => isPlaced ? parent.coord : default;

    public void Dispose()
    {
        parent = null;

        if (Application.isPlaying)
            Destroy(gameObject);
        else
            DestroyImmediate(gameObject);
    }
}
