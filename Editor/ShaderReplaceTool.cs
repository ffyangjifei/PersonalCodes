using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System.Threading;
using System.Runtime.Serialization;
public class ShaderReplaceTool : EditorWindow
{
    public static ShaderReplaceData data;
    [MenuItem("Assets/ShaderReplaceTool")]
    public static void AddWindow()
    {
        EditorWindow.GetWindow(typeof(ShaderReplaceTool));
    }
    void OnGUI()
    {
        if (data == null)
        {
            data = ShaderReplaceDataHandler.Load();
        }
        if (data == null && GUILayout.Button("CreateData"))
        {
            ShaderReplaceDataHandler.Create();
        }
        if (data==null)
        {
            return;
        }
        ShaderReplaceDataHandler.Draw(data);

        if (GUILayout.Button("RefreshInfo"))
        {
            data.MaterialList.Clear();
            data.ShaderList.Clear();
            data.FileList.Clear();
            string[] files = System.IO.Directory.GetFiles(Application.dataPath, "*.prefab", System.IO.SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
			{
                var  filePath = files[i].Substring(files[i].IndexOf("Assets")).Replace("\\","/");
                var  fileInfo = new ShaderReplaceData.FileInfoForReplace(filePath);
                data.FileList.Add(fileInfo);
                data.HandleFileInfo(fileInfo);
	        }//预处理 加载预制体信息
            for (int i = 0; i < data.FileList.Count; i++)
            {
                if (data.FileList[i].Renders.Length == 0)
                {
                    data.FileList.Remove(data.FileList[i]);
                    i--;
                }
            }//移除空
            foreach (var item in data.FileList)
            {
                data.GenerateMaterial(item);
            }//材质列表
            foreach (var item in data.MaterialList.Keys)
            {
                data.GenerateShader(item);
            }//shader列表
            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();
        }
    }

    [MenuItem("Assets/ShaderReplaceLock")]
    public static void ShaderReplaceLock()
    {
        string path = "Assets";
        string[] strs = Selection.assetGUIDs;
        if (strs != null)
        {
            if (data == null)
            {
                data = ShaderReplaceDataHandler.Load();
            }
            if (data == null)
            {
                return;
            }
            for (int i = 0; i < strs.Length; i++)
            {
                path = AssetDatabase.GUIDToAssetPath(strs[i]);
                if (!data.PrefabListLock.Contains(path))
                {
                    data.PrefabListLock.Add(path);
                }
            }
        }
        EditorUtility.SetDirty(data);
        AssetDatabase.SaveAssets();
    }
}
public static class ShaderReplaceDataHandler
{
    public static void Create()
    {
        ShaderReplaceData obj = ScriptableObject.CreateInstance<ShaderReplaceData>();
        AssetDatabase.CreateAsset(obj, "Assets/ShaderReplaceData.asset");
    }

    public static ShaderReplaceData Load()
    {
        return AssetDatabase.LoadAssetAtPath<ShaderReplaceData>("Assets/ShaderReplaceData.asset");
    }

    static Vector2 scroll1;
    static Vector2 scroll2;
    internal static void Draw(ShaderReplaceData data)
    {
        //GUILayout.Label("PrefabPath:" + data.FileList.Count);
        //scroll1 = GUILayout.BeginScrollView(scroll1);
        //foreach (var item in data.FileList)
        //{
        //    GUILayout.BeginHorizontal(GUILayout.Width(50));
        //        GUILayout.Label(item.filePath);
        //        if (GUILayout.Button("Select"))
        //        {
        //            Selection.activeObject = item.ResourceObj;
        //            Selection.selectionChanged();
        //        }
        //    GUILayout.EndHorizontal();
        //}
        //GUILayout.EndScrollView();

        //GUILayout.Box("");

        GUILayout.Label("Shader:" + data.ShaderList.Count);
        scroll2 = GUILayout.BeginScrollView(scroll2);
        foreach (var item in data.ShaderList)
        {
            GUILayout.BeginHorizontal();
            if (!data.ShaderFold.ContainsKey(item.Key))
            {
                data.ShaderFold.Add(item.Key,false);
            }
            data.ShaderFold[item.Key] = EditorGUILayout.Foldout(data.ShaderFold[item.Key], item.Key.name);
            if (GUILayout.Button("Replace", GUILayout.Width(60)))
            {
                if (data.ShaderReplaceConfig[item.Key] != null)
                {
                    foreach (var var_mat in item.Value)
                    {
                        bool needCopy = false;
                        foreach (var var_prefab in data.MaterialList[var_mat])
                        {
                            if (isContain(data, var_prefab))
                            {
                                needCopy = true;
                            }
                        }
                        if (needCopy)
                        {
                            string copPath = AssetDatabase.GetAssetPath(var_mat).Replace(".mat","_unlight.mat");
                            if (AssetDatabase.LoadAssetAtPath(copPath,typeof(Object))==null)
                            {
                                AssetDatabase.CreateAsset(GameObject.Instantiate(var_mat),copPath);
                                AssetDatabase.ImportAsset(copPath, ImportAssetOptions.Default);  
                                AssetDatabase.SaveAssets();
                            }
                            var newMat = AssetDatabase.LoadAssetAtPath<Material>(copPath);
                            foreach (var var_prefab in data.MaterialList[var_mat])
                            {
                                if (isContain(data, var_prefab))
                                {
                                    foreach (var red in var_prefab.ResourceObj.GetComponentsInChildren<Renderer>(true))
                                    {
                                        Material[] backup = red.sharedMaterials;
                                        for (int i = 0; i < backup.Length; i++)
                                        {
                                            if (backup[i]!=null && backup[i].Equals(var_mat))
                                            {
                                                backup[i] = newMat;
                                            }
                                        }
                                        red.sharedMaterials = backup;
                                    } 
                                }
                            }
                        }
                        var_mat.shader = data.ShaderReplaceConfig[item.Key];
                    }
                }
            }
            if (!data.ShaderReplaceConfig.ContainsKey(item.Key))
            {
                data.ShaderReplaceConfig.Add(item.Key, null);
            }
            data.ShaderReplaceConfig[item.Key] = EditorGUILayout.ObjectField(data.ShaderReplaceConfig[item.Key], typeof(Shader), true, GUILayout.Width(120)) as Shader;
            GUILayout.EndHorizontal();
            if (data.ShaderFold[item.Key])
            {
                GUILayout.BeginVertical();
                foreach (var mat in data.ShaderList[item.Key])
                {
                    if (!data.MaterialFold.ContainsKey(mat))
                    {
                        data.MaterialFold.Add(mat, false);
                    }
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(15);
                    data.MaterialFold[mat] = EditorGUILayout.Foldout(data.MaterialFold[mat], mat.name);
                    GUILayout.EndHorizontal();
                    if (data.MaterialFold[mat])
                    {
                            GUILayout.BeginVertical();
                            foreach (var finfo in data.MaterialList[mat])
                            {
                                //if (!data.MaterialFold.ContainsKey(mat))
                                //{
                                //    data.MaterialFold.Add(mat, false);
                                //}
                                GUILayout.BeginHorizontal();
                                GUILayout.Space(30);
                                //data.MaterialFold[mat] = EditorGUILayout.Foldout(data.MaterialFold[mat], mat.name);
                                //if (data.MaterialFold[mat])
                                //{
                                //}
                                GUILayout.Label(finfo.filePath);
                                if (GUILayout.Button("Select",GUILayout.Width(50)))
                                {
                                    Selection.activeObject = finfo.ResourceObj;
                                    Selection.selectionChanged();
                                }
                                bool contain = isContain(data, finfo);
                                bool seCon = GUILayout.Toggle(contain, "锁定") ;
                                if (contain!=seCon)
                                {
                                    if (seCon)
                                    {
                                        data.PrefabListLock.Add(finfo.filePath);
                                    }
                                    else
                                    {
                                        data.PrefabListLock.Remove(finfo.filePath);
                                    }

                                    EditorUtility.SetDirty(data);
                                    AssetDatabase.SaveAssets();
                                }
                                GUILayout.EndHorizontal();
                            }
                            GUILayout.EndVertical();
                    }
                }
                GUILayout.EndVertical();
            }
        }
        GUILayout.EndScrollView();
    }

    private static bool isContain(ShaderReplaceData data, ShaderReplaceData.FileInfoForReplace finfo)
    {
        foreach (var item in data.PrefabListLock)
        {
            if (finfo.filePath.StartsWith(item))
            {
                return true;
            }
        }
        return data.PrefabListLock.Contains(finfo.filePath);
    }
}

[System.Serializable]
public class SerializableSDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    // We save the keys and values in two lists because Unity does understand those.
    [SerializeField]
    public List<TKey> _keys;
    [SerializeField]
    public List<TValue> _values;

    // Before the serialization we fill these lists
    public void OnBeforeSerialize()
    {
        _keys = new List<TKey>();
        foreach (var item in this.Keys)
        {
            _keys.Add(item);
        }
        _values = new List<TValue>();
        foreach (var item in this.Values)
        {
            _values.Add(item);
        }
    }
    // After the serialization we create the dictionary from the two lists
    public void OnAfterDeserialize()
    {
        this.Clear();
        int count = Mathf.Min(_keys.Count, _values.Count);
        for (int i = 0; i < count; ++i)
        {
            this.Add(_keys[i], _values[i]);
        }
    }
}


