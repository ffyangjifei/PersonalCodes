using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Xml;
using System.IO;
using System.Xml.Serialization;
using System.Text;
#if UNITY_5 || UNITY_5_2
public class AssetDataUtil : AssetPostprocessor
{
    private static GameObject[] lastHierarchyList = null;

    private static GameObject[] newHierarchyLisy = null;


    public AssetDataUtil()
    {
        PrefabLightUtil.init();

        //EditorApplication.hierarchyWindowChanged = null;
        //EditorApplication.hierarchyWindowChanged += onHierarchyWindowChanged;
    }

    //public static void onHierarchyWindowChanged()
    //{
    //    GameObject[] tempList = GameObject.FindObjectsOfType<GameObject>();

    //    if (lastHierarchyList == null) lastHierarchyList = tempList;

    //    int newObjCount = tempList.Length - lastHierarchyList.Length;

    //    if (newObjCount > 0)
    //    {
    //        if (DragAndDrop.objectReferences != null)
    //        {
    //            foreach (Object item in DragAndDrop.objectReferences)
    //            {
    //                if ((item as GameObject).isStatic)
    //                {
    //                    setLightInfoForNewObject(item as GameObject);
    //                }
    //            }
    //        }
    //        Debug.Log("新增" + newObjCount + "个对象");
    //    }
    //    else if (newObjCount < 0)
    //    {
    //        Debug.Log("删除" + newObjCount * -1 + "个对象");
    //    }

    //    lastHierarchyList = tempList;
    //}

    //private static void setLightInfoForNewObject(GameObject obj)
    //{
    //    PrefabLightData data = PrefabLightUtil.getLightDataByName(obj.name);
    //    if (data == null)
    //    {
    //        if (obj.name.IndexOf(" (") != -1)
    //        {

    //            data = PrefabLightUtil.getLightDataByName(obj.name.Substring(0, obj.name.IndexOf(" (")));
    //        }
    //    }
    //}

    public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        PrefabLightData data = null;

        if (movedFromAssetPaths.Length == 1)
        {
            int typeIndex = movedFromAssetPaths[0].LastIndexOf(".");
            int startIndex = movedFromAssetPaths[0].LastIndexOf("/");

            if (typeIndex != -1)
            {
                string prefabName = movedFromAssetPaths[0].Substring(startIndex + 1, typeIndex - (startIndex + 1));
                string assetType = movedFromAssetPaths[0].Substring(typeIndex);
                if (assetType == ".prefab")
                {
                    data = PrefabLightUtil.getLightDataByName(prefabName);

                    PrefabLightUtil.deleteItemByName(prefabName);
                }
            }
        }

        if (movedAssets.Length == 1)
        {
            int typeIndex = movedAssets[0].LastIndexOf(".");
            int startIndex = movedAssets[0].LastIndexOf("/");

            if (typeIndex != -1)
            {
                string prefabName = movedAssets[0].Substring(startIndex + 1, typeIndex - (startIndex + 1));
                string assetType = movedAssets[0].Substring(typeIndex);
                if (assetType == ".prefab")
                {
                    if (data != null)
                    {
                        data.prefabName = prefabName;
                        PrefabLightUtil.addNewItem(data.clone());
                        data = null;
                        return;
                    }
                }
            }
        }

        for (int i = 0; i < importedAssets.Length; i++)
        {
            //Debug.Log("importedAssets: " + importedAssets[i]);

            int typeIndex = importedAssets[i].LastIndexOf(".");

            if (typeIndex != -1)
            {
                string assetType = importedAssets[i].Substring(typeIndex);
                if (assetType == ".prefab")
                {
                    if (Selection.activeGameObject != null && Selection.activeGameObject.isStatic && EditorWindow.focusedWindow.title != "Project")
                    {
                        PrefabLightUtil.updatePrefab(Selection.activeGameObject);
                    }
                    else
                    {
                        if (DragAndDrop.objectReferences != null)
                        {
                            foreach (Object item in DragAndDrop.objectReferences)
                            {
                                if ((item as GameObject).isStatic)
                                {
                                    PrefabLightUtil.updatePrefab(item as GameObject);
                                }
                            }
                        }
                    }
                }
            }
        }

        for (int i = 0; i < deletedAssets.Length; i++)
        {
            checkDelete(deletedAssets[i]);
        }

        //EditorApplication.ExecuteMenuItem("Window/Hierarchy");
        //EditorWindow.focusedWindow.SendEvent(EditorGUIUtility.CommandEvent("Duplicate"));
    }

    private static void checkDelete(string path)
    {
        int typeIndex = path.LastIndexOf(".");
        int startIndex = path.LastIndexOf("/");

        if (typeIndex != -1)
        {
            string prefabName = path.Substring(startIndex + 1, typeIndex - (startIndex + 1));
            string assetType = path.Substring(typeIndex);
            if (assetType == ".prefab")
            {
                PrefabLightUtil.deleteItemByName(prefabName);
            }
        }
    }
}

public class PrefabLightUtil
{
    private static string configName = "PrefabLightConfig.xml";
    private static string configPath = "/../Resources/U3D Tools/AssetDataUtils";

    private static string editorPath = "";

    private static List<PrefabLightData> localList = null;

    private static PrefabLightData prefabLightData = null;

    private static Renderer[] renders = null;

    private static bool isInit = false;

    public static void init()
    {
        if (isInit) return;
        isInit = true;

        DirectoryInfo rootDir = new DirectoryInfo(Application.dataPath);
        FileInfo[] files = rootDir.GetFiles("AssetDataUtil.cs", SearchOption.AllDirectories);
        editorPath = Path.GetDirectoryName(files[0].FullName.Replace("\\", "/").Replace(Application.dataPath, "Assets"));
        editorPath = editorPath + configPath;

        checkConfig();

        refreshSceneObjectLight();
    }

    public static void refreshSceneObjectLight()
    {
        if (localList == null) return;

        if (localList.Count > 0)
        {
            GameObject[] tempObjs = GameObject.FindObjectsOfType<GameObject>();
            if (tempObjs.Length == 0) return;

            List<GameObject> objs = new List<GameObject>();
            for (int i = 0; i < tempObjs.Length; i++)
            {
                if (tempObjs[i].isStatic)
                {
                    QuickCopyUtil.copyLightFromConfig(tempObjs[i]);
                }
            }
        }
    }

    public static PrefabLightData getLightDataByName(string name)
    {
        for (int i = 0; i < localList.Count; i++)
        {
            if (localList[i].prefabName == name)
            {
                return localList[i];
            }
        }
        return null;
    }

    public static void deleteItemByName(string name)
    {
        PrefabLightData isHasItem = hasItem(name);
        if (isHasItem != null)
        {
            localList.Remove(isHasItem);
        }
        saveConfig();
    }

    public static void updatePrefab(GameObject obj)
    {
        renders = obj.GetComponentsInChildren<Renderer>(true);

        prefabLightData = new PrefabLightData();
        prefabLightData.prefabName = obj.name;
        prefabLightData.lightmapIndex = new int[renders.Length];
        prefabLightData.lightmapScaleOffset = new Vector4[renders.Length];


        for (int i = 0; i < renders.Length; i++)
        {
            prefabLightData.lightmapIndex[i] = renders[i].lightmapIndex;
            prefabLightData.lightmapScaleOffset[i] = renders[i].lightmapScaleOffset;
        }

        addNewItem(prefabLightData);
    }

    public static void addNewItem(PrefabLightData item)
    {
        if (!modifileItem(item))
        {
            localList.Add(item);
            prefabLightData = null;
        }

        saveConfig();
    }

    public static bool modifileItem(PrefabLightData item)
    {
        for (int i = 0; i < localList.Count; i++)
        {
            if (localList[i].prefabName == item.prefabName)
            {
                localList[i] = item;
                return true;
            }
        }
        return false;
    }

    private static PrefabLightData hasItem(string name)
    {
        for (int i = 0; i < localList.Count; i++)
        {
            if (localList[i].prefabName == name)
            {
                return localList[i];
            }
        }
        return null;
    }

    private static void checkConfig()
    {
        if (File.Exists(editorPath + "/" + configName))
        {
            loadConfig();
        }
        else
        {
            localList = new List<PrefabLightData>();
        }
    }

    private static void loadConfig()
    {
        XmlSerializer xs = new XmlSerializer(typeof(List<PrefabLightData>));

        FileStream fs = new FileStream(editorPath + "/" + configName, FileMode.Open);
        XmlReader reader = new XmlTextReader(fs);
        localList = (List<PrefabLightData>)xs.Deserialize(reader);
        fs.Close();
    }

    private static void saveConfig()
    {
        XmlSerializer x = new XmlSerializer(typeof(List<PrefabLightData>));
        TextWriter writer = new StreamWriter(editorPath + "/" + configName);
        x.Serialize(writer, localList);
        writer.Close();

        AssetDatabase.Refresh();
    }
}

[System.Serializable]
public class PrefabLightData
{
    public string prefabName = "";
    public int[] lightmapIndex = { 1 };
    public Vector4[] lightmapScaleOffset = { Vector4.zero };

    public PrefabLightData clone()
    {
        PrefabLightData data = new PrefabLightData();
        data.prefabName = this.prefabName;
        data.lightmapIndex = this.lightmapIndex;
        data.lightmapScaleOffset = this.lightmapScaleOffset;
        return data;
    }
}
#endif
