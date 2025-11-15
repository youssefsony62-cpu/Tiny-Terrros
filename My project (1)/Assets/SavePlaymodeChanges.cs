using UnityEngine;
using UnityEditor;

public class SavePlaymodeChanges : MonoBehaviour
{
    [MenuItem("Tools/Save Selected Transform")]
    public static void SaveSelectedTransform()
    {
        if (Selection.activeTransform != null)
        {
            Transform selected = Selection.activeTransform;
            Undo.RecordObject(selected, "Save Transform");
            EditorUtility.SetDirty(selected);

            // Save position, rotation, scale
            PrefabUtility.RecordPrefabInstancePropertyModifications(selected);

            Debug.Log("✅ Saved transform changes for: " + selected.name);
        }
        else
        {
            Debug.LogWarning("⚠ No object selected to save!");
        }
    }
}
