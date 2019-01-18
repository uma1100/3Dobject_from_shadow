using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour {

    private SpriteRenderer spRen;
    //グローバルでSpriteRendererの変数を定義

	void Awake ()
    {
        spRen = GetComponent<SpriteRenderer>();
        //ゲームが始まったらspRenに画像をコンポーネント
	}
	
	// Update is called once per frame
	void Update ()
    {
        Get();
	}

    void Get()
    {
        var tex = spRen.sprite.texture;
        //tex.GetPixel();
    }
}
