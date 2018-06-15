// ----------------------------------------------------------------------------------
//
//
// ----------------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

public class NcDrawFpsRect : MonoBehaviour
{
    //public  bool        centerTop       = true;
    public AnchorPos anchor = AnchorPos.Custom;
    public Rect startNormalRect = new Rect(0, 0, 0.1f, 0.1f);		// The rect the window is initially displayed at.
	private	Rect		startRect		= new Rect( 0, 0, 75, 50 );		// The rect the window is initially displayed at.
	public	bool		updateColor		= true;							// Do you want the color to change if the FPS gets low
	public	bool		allowDrag		= true;							// Do you want to allow the dragging of the FPS window
	public  float		frequency		= 0.5F;							// The update frequency of the fps
	public	int			nbDecimal		= 1;							// How many decimal do you want to display
	 
	private float		accum			= 0f;							// FPS accumulated over the interval
	private int			frames			= 0;							// Frames drawn over the interval
	private Color		color			= Color.white;					// The color of the GUI, depending of the FPS ( R < 10, Y < 30, G >= 30 )
	private string		sFPS			= "";							// The fps formatted into a string.
	private GUIStyle	style;											// The style the text will be displayed at, based en defaultSkin.label.
	 
	void Start()
    {
        startRect = startNormalRect;
        startRect.width *= Screen.width;
        startRect.height *= Screen.height;

		StartCoroutine(FPS());
	}
	 
	void Update()
    {
        startRect.width = startNormalRect.width * Screen.width;
        startRect.height = startNormalRect.height * Screen.height;

		accum += Time.timeScale / Time.deltaTime;
		++frames;
	}

	IEnumerator FPS()
	{
		while (true)
		{
			// Update the FPS
			float fps = accum/frames;
			sFPS = fps.ToString( "f" + Mathf.Clamp( nbDecimal, 0, 10 ) );
	    
			//Update the color
			color = (fps >= 30) ? Color.green : ((fps > 10) ? Color.yellow : Color.red);
	        
			accum = 0.0F;
			frames = 0;
	        
			yield return new WaitForSeconds( frequency );
		}
	}

	void OnGUI()
	{
        //if (Debug.isDebugBuild)
        {
            // Copy the default label skin, change the color and the alignement
            if (style == null)
            {
                style = new GUIStyle(GUI.skin.label);
                style.normal.textColor = Color.white;
                style.alignment = TextAnchor.MiddleCenter;
            }

            GUI.color = updateColor ? color : Color.white;
            //if (anchor != AnchorPos.Custom)
            //{

            //    float x = 
            //    float y =
            //    startRect = new Rect(x, y, width, height);
            //}
            Rect rect = startRect;
            if (anchor != AnchorPos.Custom)
            {
                //rect.x += Screen.width / 2 - rect.width / 2;
                rect.x += ((int)anchor / 10) * (Screen.width - startRect.width) / 2;
                rect.y += ((int)anchor % 10) * (Screen.height - startRect.height) / 2;
            }
            startRect = GUI.Window(0, rect, DoMyWindow, "");
            if (anchor != AnchorPos.Custom)
            {
                startRect.x -= ((int)anchor / 10) * (Screen.width - startRect.width) / 2;
                startRect.y -= ((int)anchor % 10) * (Screen.height - startRect.height) / 2;
            }
            //startRect.x -= Screen.width / 2 - rect.width / 2;
        }
	}

	void DoMyWindow(int windowID)
	{
		GUI.Label(new Rect(0, 0, startRect.width, startRect.height), sFPS + " FPS", style);
		if (allowDrag)
			GUI.DragWindow(new Rect(0, 0, Screen.width, Screen.height));
	}

    public enum AnchorPos
    {
        Custom = -1,
        LeftTop = 0,
        Left = 1,
        LeftBottom = 2,
        CenterTop = 10,
        Center = 11,
        CenterBottom = 12,
        RightTop = 20,
        Right = 21,
        RightBottom = 22,
    }
}
