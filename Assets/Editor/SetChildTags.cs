using UnityEngine;
using UnityEditor;

public class SetChildTags
{
    [MenuItem("Tools/Set Children Tags Same As Parent")]
    static void SetTags()
    {
        foreach(GameObject obj in Selection.gameObjects)
        {
            SetChildrenTag(obj, obj.tag);
        }

        Debug.Log("Child tags updated!");
    }

    static void SetChildrenTag(GameObject parent, string tag)
    {
        foreach(Transform child in parent.transform)
        {
            child.gameObject.tag = tag;

            SetChildrenTag(child.gameObject, tag);
        }
    }
}