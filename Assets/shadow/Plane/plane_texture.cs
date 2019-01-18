using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class plane_texture : MonoBehaviour {

    private Renderer render;
	// Use this for initialization
	void Start () {
        render = GetComponent<Renderer>();
        StartCoroutine(Tex_change());
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private IEnumerator Tex_change()
    {
        var tex = render.material.color;
        //Color[] color = tex.
        //color.r = 0.0f;  // RGBのR(赤)値
        //color.g = 0.0f; // RGBのG(緑)値
        //color.b = 0.0f; // RGBのB(青)値
        //color.a = 1.0f;// RGBのアルファ値(透明度の値)
        //color = new Color(0.0f, 0.0f, 0.0f);    //色変換

        //render.material.color = color; // 変更した色情報に変更

        yield return null;
    }
}
