using UnityEngine;
using System.Collections;
/**************************************************
* 
* 创建者：		yangjifei
* 创建时间：	2017/10/23 13:48
* 描述：		继承下改 修改文本格式 省的每次都写个文本格式什么的 烦..
* 
**************************************************/
public class UILabel_Formated : UILabel
{
    public string TextFormat = "{0}";
    string usingText = "";
    public override string text
    {
        get
        {
            if (string.IsNullOrEmpty(usingText))
            {
                return base.mText;
            }
            return usingText;
        }
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                if (!string.IsNullOrEmpty(base.mText))
                {
                    base.mText = string.Empty;
                }
                base.hasChanged = true;
            }
            else if (base.mText != value)
            {
                usingText = value;

                if (!string.IsNullOrEmpty(value))
                {
                    try
                    {
                        base.mText = string.Format(TextFormat, value.Split('|'));
                    }
                    catch (System.Exception ex)
                    {
                        base.mText = value;
                    }
                }
                else
                {
                    base.mText = value;
                }

                base.hasChanged = true;
                if (base.mFont != null)
                {
                    base.mFont.Request(ref base.mText);
                }
                if (base.shrinkToFit)
                {
                    base.MakePixelPerfect();
                }
            }
        }
    }
}
