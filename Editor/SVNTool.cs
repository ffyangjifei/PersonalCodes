using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using UnityEditorInternal;
using System.Diagnostics;
using System.Reflection;
/**************************************************
* 
* 创建者：		yangjifei
* 创建时间：	2018/02/06 17:14
* 描述：		点击更新SVN
* 
**************************************************/
public class SVNTool : Editor
{
    [MenuItem("Assets/SVNTool/更新", false, 100)]
    public static void SVNUpdate()
    {
        string[] paths = getSelectedPaths();
        string args = @"/command:update /path:""" + paths[0] + @""" /closeonend:2";
        StartProcess(args);
    }
    [MenuItem("Assets/SVNTool/更新", true)]
    public static bool SVNUpdateValid()
    {
        string[] paths = getSelectedPaths();
        return paths.Length > 0;
    }
    [MenuItem("Assets/SVNTool/提交", false, 200)]
    public static void SVNSubmmit()
    {
        string[] paths = getSelectedPaths();
        string args = @"/command:commit /path:""" + paths[0] + @""" /closeonend:2";
        StartProcess(args);
    }
    [MenuItem("Assets/SVNTool/提交", true)]
    public static bool SVNSubmmitValid()
    {
        string[] paths = getSelectedPaths();
        return paths.Length > 0;
    }
    [MenuItem("Assets/SVNTool/日志", false, 300)]
    public static void SVNLog()
    {
        string[] paths = getSelectedPaths();
        string args = @"/command:log /path:""" + paths[0] + @""" /closeonend:0";
        StartProcess(args);
    }
    [MenuItem("Assets/SVNTool/日志", true)]
    public static bool SVNLogValid()
    {
        string[] paths = getSelectedPaths();
        return paths.Length > 0;
    }

    [MenuItem("Assets/SVNTool/更新Project %#DOWN", false, 100)]
    public static void SVNUpdateProject()
    {
        string path = Application.dataPath;
        string args = @"/command:update /path:""..\" + path + @""" /closeonend:2";
        StartProcess(args);
    }

    [MenuItem("Assets/SVNTool/上传Assets %#UP", false, 100)]
    public static void SVNSubmitAssets()
    {
        string path = Application.dataPath;
        string args = @"/command:commit /path:""" + path + @""" /closeonend:2";
        StartProcess(args);
    }

    private static string[] getSelectedPaths()
    {
        if (Selection.activeObject != null)
        {
            return new string[] { AssetDatabase.GetAssetPath(Selection.activeObject) };
        }


        Assembly assem = Assembly.GetAssembly(typeof(UnityEditor.Editor));
        var ProjectBrowser = assem.GetType("UnityEditor.ProjectBrowser");
        var GetTreeViewFolderSelection = ProjectBrowser.GetMethod("GetTreeViewFolderSelection", BindingFlags.Static | BindingFlags.NonPublic);
        int[] treeViewFolderSelection = (int[])GetTreeViewFolderSelection.Invoke(null, null);

        var GetFolderPathsFromInstanceIDs = ProjectBrowser.GetMethod("GetFolderPathsFromInstanceIDs", BindingFlags.Static | BindingFlags.NonPublic);
        string[] paths = (string[])GetFolderPathsFromInstanceIDs.Invoke(null, new object[] { treeViewFolderSelection });
        return paths;
    }
    static void StartProcess(string Arguments)
    {
        try
        {
            var process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.RedirectStandardInput = true;
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
            startInfo.UseShellExecute = false;
            startInfo.FileName = @"C:\Program Files\TortoiseSVN\bin\TortoiseProc.exe";
            startInfo.Arguments = Arguments;
            process.EnableRaisingEvents = true;
            process.StartInfo = startInfo;

            process.Start();
        }
        catch (System.Exception ex)
        {
            LtLog.Warning("SVNToolException-->Path:" + Arguments + " \nError:" + ex.ToString());
        }
    }
    void OnInspectorUpdate()
    {
        Repaint();
    }
}
