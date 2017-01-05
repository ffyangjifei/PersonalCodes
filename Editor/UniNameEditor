using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(UniName), true)]
public class UniNameEditor : PropertyDrawer
{
    private UniName target;

    private GUIContent label;

    private Rect position;

    private SerializedProperty property;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        target = this.attribute as UniName;

        this.property = property;

        this.label = label;

        this.position = position;
        drawInspector();
    }

    private void drawInspector()
    {
        if (target.cnName != string.Empty) label.text = target.cnName;

        GUI.enabled = target.enable;

        switch (property.propertyType)
        {
            case SerializedPropertyType.Generic:
                drawGenericProperty();
                break;
            case SerializedPropertyType.Quaternion:
                drawQuaternionProperty();
                break;
            case SerializedPropertyType.Rect:
                drawRectProperty();
                break;
            case SerializedPropertyType.AnimationCurve:
            case SerializedPropertyType.ArraySize:
            case SerializedPropertyType.Integer:
            case SerializedPropertyType.Boolean:
            case SerializedPropertyType.Bounds:
            case SerializedPropertyType.Character:
            case SerializedPropertyType.Color:
            case SerializedPropertyType.Enum:
            case SerializedPropertyType.Float:
            case SerializedPropertyType.Gradient:
            case SerializedPropertyType.LayerMask:
            case SerializedPropertyType.String:
            case SerializedPropertyType.Vector2:
            case SerializedPropertyType.Vector3:
            case SerializedPropertyType.ObjectReference:
            default:
                drawNormalProperty();
                break;
        }
        GUI.enabled = true;
    }

    private Rect tempRect;
    private float lineHeight = 16;
    private void drawRectProperty()
    {
        tempRect = property.rectValue;
        EditorGUI.indentLevel = property.depth;
        EditorGUI.LabelField(position, label);
        position.y += lineHeight;
        EditorGUI.indentLevel = property.depth + 1;

        float pSpace = position.width * 0.05f;
        position.x = 0;
        position.width *= 0.2f;
        position.height = lineHeight;

        EditorGUI.LabelField(position, "X");
        position.x += pSpace;
        tempV4.x = EditorGUI.FloatField(position, tempRect.x);
        position.x += position.width;
        EditorGUI.LabelField(position, "Y");
        position.x += pSpace;
        tempRect.y = EditorGUI.FloatField(position, tempRect.y);
        position.x += position.width;
        EditorGUI.LabelField(position, "W");
        position.x += pSpace;
        tempRect.width = EditorGUI.FloatField(position, tempRect.width);
        position.x += position.width;
        EditorGUI.LabelField(position, "H");
        position.x += pSpace;
        tempRect.height = EditorGUI.FloatField(position, tempRect.height);

        //tempV4 = EditorGUI.Vector4Field(position, label.text, tempV4);
        property.rectValue = tempRect;
    }

    private Vector4 tempV4;
    private void drawQuaternionProperty()
    {
        tempV4 = new Vector4(property.quaternionValue.x, property.quaternionValue.y, property.quaternionValue.z, property.quaternionValue.w);
        EditorGUI.indentLevel = property.depth;
        EditorGUI.LabelField(position, label);
        position.y += lineHeight;
        EditorGUI.indentLevel = property.depth + 1;

        float pSpace = position.width * 0.05f;
        position.x = 0;
        position.width *= 0.2f;
        position.height = lineHeight;

        EditorGUI.LabelField(position, "X");
        position.x += pSpace;
        tempV4.x = EditorGUI.FloatField(position, tempV4.x);
        position.x += position.width;
        EditorGUI.LabelField(position, "Y");
        position.x += pSpace;
        tempV4.y = EditorGUI.FloatField(position, tempV4.y);
        position.x += position.width;
        EditorGUI.LabelField(position, "Z");
        position.x += pSpace;
        tempV4.z = EditorGUI.FloatField(position, tempV4.z);
        position.x += position.width;
        EditorGUI.LabelField(position, "W");
        position.x += pSpace;
        tempV4.w = EditorGUI.FloatField(position, tempV4.w);

        //tempV4 = EditorGUI.Vector4Field(position, label.text, tempV4);
        property.quaternionValue = new Quaternion(tempV4.x, tempV4.y, tempV4.z, tempV4.w);
    }

    private void drawGenericProperty()
    {
        float tempWid = position.width;
        position.height = base.GetPropertyHeight(property, null);
        position.width = 50;
        EditorGUI.PropertyField(position, property, label);

        position.width = tempWid;

        if (target.showSaveLoadBtn)
        {
            drawSaveBtn();
            drawLoadBtn();
        }

        if (property.isExpanded)
        {
            EditorGUI.indentLevel = property.depth;

            int i = 0;
            int curDep = property.depth;
            //var propEnum = property.GetEnumerator();

            SerializedProperty serializedProperty = property.Copy();
            SerializedProperty endProperty = serializedProperty.GetEndProperty();

            while (serializedProperty.NextVisible(serializedProperty.isExpanded) && !SerializedProperty.EqualContents(serializedProperty, endProperty))
            {
                if (i == 0) position.y += EditorGUI.GetPropertyHeight(serializedProperty);
                i++;
                    
                if (serializedProperty.depth > curDep + 1) continue;

                label.text = serializedProperty.name;

                EditorGUI.indentLevel = serializedProperty.depth;

                if (serializedProperty.propertyType == SerializedPropertyType.Generic && serializedProperty.isArray)
                {
                    drawArrayProperty(serializedProperty);
                    continue;
                }

                if (serializedProperty.propertyType == SerializedPropertyType.Enum)
                {
                    serializedProperty.enumValueIndex = EditorGUI.Popup(position, serializedProperty.name, serializedProperty.enumValueIndex, serializedProperty.enumNames);
                }
                else
                {
                    //Debug.Log(serializedProperty.name + ", " + serializedProperty.propertyType + ", " + serializedProperty.type);
                    EditorGUI.PropertyField(position, serializedProperty, label);
                }
                
                position.y += EditorGUI.GetPropertyHeight(serializedProperty);
            }

            //while (propEnum.MoveNext())
            //{
            //    if (i == 0) position.y += EditorGUI.GetPropertyHeight((propEnum.Current as SerializedProperty));
            //    i++;

            //    if ((propEnum.Current as SerializedProperty).propertyType == SerializedPropertyType.Generic && (propEnum.Current as SerializedProperty).isArray)
            //    {
            //        drawArrayProperty((propEnum.Current as SerializedProperty));
            //        continue;
            //    }

            //    if ((propEnum.Current as SerializedProperty).depth > curDep + 1) continue;

            //    EditorGUI.indentLevel = (propEnum.Current as SerializedProperty).depth;

            //    label.text = (propEnum.Current as SerializedProperty).name;

            //    if ((propEnum.Current as SerializedProperty).isArray && (propEnum.Current as SerializedProperty).isExpanded)
            //    {
            //        (propEnum.Current as SerializedProperty).isExpanded = EditorGUI.Foldout(position, (propEnum.Current as SerializedProperty).isExpanded, label);
            //    }

            //    EditorGUI.PropertyField(position, (propEnum.Current as SerializedProperty), label);
            //    position.y += EditorGUI.GetPropertyHeight((SerializedProperty)propEnum.Current);
            //}
        }
    }

    private void drawArrayProperty(SerializedProperty sp)
    {
        sp.isExpanded = EditorGUI.Foldout(position, sp.isExpanded, label);
        if (sp.isExpanded)
        {
            position.y += base.GetPropertyHeight(sp, null);
            EditorGUI.PropertyField(position, sp.FindPropertyRelative("Array.size"));
            for (int i = 0; i < sp.arraySize; i++)
            {
                position.y += base.GetPropertyHeight(sp, null);
                EditorGUI.PropertyField(position, sp.GetArrayElementAtIndex(i));
            }
            position.y += lineHeight;
        }
        else position.y += EditorGUI.GetPropertyHeight(sp);
    }

    private void drawNormalProperty()
    {
        EditorGUI.PropertyField(position, property, label);
    }

    private void drawLoadBtn()
    {
        tempRect.x += tempRect.width;
        if (GUI.Button(tempRect, "读取属性配置"))
        {
            string path = EditorUtility.OpenFilePanel("导入属性配置", "", "U3DProperty");

            if (path.Length != 0)
            {
                System.Object importData = BinaryDeserializer<System.Object>(path);
                if (importData != null)
                {
                    FieldInfo fieldInfo1 = property.serializedObject.targetObject.GetType().GetField(property.name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

                    this.fieldInfo.SetValue(property.serializedObject.targetObject, importData);
                    Debug.Log("成功导入！");
                }
            }
        }
    }

    private void drawSaveBtn()
    {
        tempRect.x = position.x + position.width * 0.3f;
        tempRect.y = position.y;
        tempRect.width = position.width * 0.3f;
        tempRect.height = lineHeight;

        if (GUI.Button(tempRect, "保存属性配置"))
        {
            string path = EditorUtility.SaveFilePanel("保存属性配置", "", System.DateTime.Now.ToString("yyyy年mm月dd日 HH时mm分ss秒") + "_" + property.name + ".U3DProperty", "U3DProperty");

            if (path.Length != 0)
            {
                string[] separatedPaths = property.propertyPath.Split('.');

                System.Object reflectionTarget = property.serializedObject.targetObject;

                foreach (string path1 in separatedPaths)
                {
                    FieldInfo fieldInfo1 = reflectionTarget.GetType().GetField(path1);
                    reflectionTarget = fieldInfo1.GetValue(reflectionTarget);
                }

                if (BinarySerializer(reflectionTarget, path))
                {
                    Debug.Log("成功导出属性配置到 " + path);
                }
            }
        }
    }

    private bool BinarySerializer(object obj, string path)
    {
        FileStream Stream = new FileStream(path, FileMode.OpenOrCreate);
        BinaryFormatter bin = new BinaryFormatter();
        try
        {
            bin.Serialize(Stream, obj);
            Stream.Close();
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        return true;
    }

    private T BinaryDeserializer<T>(string path)
    {
        try
        {
            FileStream binfile = new FileStream(path, FileMode.Open);
            BinaryFormatter bin = new BinaryFormatter();
            T t = (T)bin.Deserialize(binfile);
            binfile.Close();
            return t;
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (FileNotFoundException)
        { 
            throw; 
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float extraHeight = 2.0f;

        float baseHeight = base.GetPropertyHeight(property, label);

        if (property.propertyType == SerializedPropertyType.Quaternion || property.propertyType == SerializedPropertyType.Rect) baseHeight *= 2f;

        if (!property.isExpanded)
        {
            return baseHeight + extraHeight;
        }

        if (property.propertyType == SerializedPropertyType.Generic)
        {
            if (property.isArray)
            {
                return (property.arraySize + 1) * baseHeight;
            }

            int curDep = property.depth;

            SerializedProperty serializedProperty = property.Copy();
            SerializedProperty endProperty = serializedProperty.GetEndProperty();
            while (serializedProperty.NextVisible(serializedProperty.isExpanded) && !SerializedProperty.EqualContents(serializedProperty, endProperty))
            {
                if (serializedProperty.depth > curDep + 1) continue;

                if (serializedProperty.isArray && serializedProperty.isExpanded)
                {
                    baseHeight += (serializedProperty.arraySize + 2) * lineHeight;
                }
                else
                {
                    baseHeight += EditorGUI.GetPropertyHeight(serializedProperty, null, false);
                }
            }
        }
        return baseHeight;
    }
}
#endif
