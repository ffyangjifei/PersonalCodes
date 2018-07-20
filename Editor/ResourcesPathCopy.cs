using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Text.RegularExpressions;

public class ResourcesPathCopy : Editor
{
    [MenuItem("Assets/CopyPath/CopyFullPath", false, 100)]
    public static void CopyFullPath()
    {
        LtLog.Error(Application.dataPath.Replace("/Assets","")+"/"+AssetDatabase.GetAssetPath(Selection.activeInstanceID));
    }
    [MenuItem("Assets/CopyPath/CopyResourcesPath", false, 100)]
    public static void CopyResourcesPath()
    {
        string s = AssetDatabase.GetAssetPath(Selection.activeInstanceID);
        Regex regex = new Regex(".+Resources/(.+)[.][^.]+");
        EditorGUIUtility.systemCopyBuffer = regex.Match(s).Groups[1].ToString();
    }
}
