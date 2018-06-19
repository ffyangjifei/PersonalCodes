using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(UILabel_Formated))]
public class UILabel_FormatedInspector : UILabelInspector
{

    override protected bool DrawProperties()
    {
        base.DrawProperties();

        UILabel_Formated mLabel = mWidget as UILabel_Formated;

        //GUI.skin.textArea.wordWrap = true;
        string text = string.IsNullOrEmpty(mLabel.TextFormat) ? "{0}" : mLabel.TextFormat;
        text = EditorGUILayout.TextField("Format",mLabel.TextFormat, GUI.skin.textField);
        if (!text.Equals(mLabel.TextFormat)) { RegisterUndo(); mLabel.TextFormat = text; }

        return true;
    }
}
