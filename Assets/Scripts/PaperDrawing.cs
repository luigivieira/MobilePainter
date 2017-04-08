using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Implementa o desenho sobre a folha de papel.
/// </summary>
public class PaperDrawing: MonoBehaviour
{
    public Image cursor;

    public string floatRangeProperty = "Saturation";
    public float cycleTime = 10;
    public Renderer rend;

    // Use this for initialization
    void Start ()
    {
        rend = GetComponent<Renderer>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        ProceduralMaterial substance = rend.sharedMaterial as ProceduralMaterial;
        if(substance)
        {
            float lerp = Mathf.PingPong(Time.time * 2 / cycleTime, 1);
            substance.SetProceduralFloat(floatRangeProperty, lerp);
            substance.RebuildTextures();
        }

        /*Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hitInfo = Physics2D.Raycast(pos, Vector2.zero);

        if(hitInfo)
        {
            cursor.transform.position = pos;
        }*/
    }
}
