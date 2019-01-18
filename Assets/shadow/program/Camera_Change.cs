using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Change : MonoBehaviour {

    //2Dカメラ
	[SerializeField] private GameObject Camera2;
    //3Dカメラ
    [SerializeField] private GameObject Camera3;
	
	// Update is called once per frame
	 public void OnClick()
    {
            Camera2.SetActive(!Camera2.activeSelf);
            Camera3.SetActive(!Camera3.activeSelf);
            
	}
}
