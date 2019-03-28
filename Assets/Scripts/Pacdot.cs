using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pacdot : MonoBehaviour {

	public int targetPosition;
	public bool isSuperPacdot = false;
	public Vector3 ResPosition = Vector3.zero;
	
	private  void OnTriggerEnter2D(Collider2D col)
	{
		if(col.gameObject.name == "Pacman"){
			if(isSuperPacdot){
               //超级豆豆被吃了，吃豆人变超级吃豆人
			    ResPosition = GameObject.Find("Pacman").GetComponent<Transform>().position;
				GameManager.Instance.OnEatPacdot(gameObject);
				GameManager.Instance.OnEatSuperPacdot();
				Destroy(gameObject);			   
			}
			else{	
				GameManager.Instance.OnEatPacdot(gameObject);
				Destroy(gameObject);
			}				
		}
	}
}
