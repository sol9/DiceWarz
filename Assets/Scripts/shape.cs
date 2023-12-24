using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class shape : SerializedMonoBehaviour
{
	public List<grid3d> members;

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (members.valid())
        {
            Gizmos.color = Color.red;
            Gizmos.matrix = transform.localToWorldMatrix;

            members.ForEach(g => Gizmos.DrawWireCube(g, Vector3.one));
        }
    }
#endif
}
