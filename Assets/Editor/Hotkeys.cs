using UnityEditor;
using UnityEngine;
using System.Collections;

public class Hotkeys : MonoBehaviour {
    [MenuItem("Shortcuts/Assets/Create/Folder %#q")]
    static void CreateFolderShortcut() {
        EditorApplication.ExecuteMenuItem("Assets/Create/Folder");
    }

    [MenuItem("Shortcuts/Assets/Create/C# Script %#r")]
    static void CreateCSharpScriptShortcut() {
        EditorApplication.ExecuteMenuItem("Assets/Create/C# Script");
    }

    [MenuItem("Shortcuts/GameObject/Create Centered Empty %#e")]
    static void CreateCenteredEmptyShortcut() {
        GameObject gameObject = new GameObject("GameObject");
        gameObject.transform.position = Vector3.zero;
        gameObject.transform.rotation = Quaternion.identity;
    }
}
