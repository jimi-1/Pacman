using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GhostMove : MonoBehaviour{
	
	private Rigidbody2D rb;
	private CircleCollider2D cc;
	private Animator ar;
	private Vector3 startPos;
	private int index;
	private List<Vector3> WayPoints = new List<Vector3>();
	public float speed = 0.2f;
	public GameObject[] wayPointsGos;

	// Use this for initialization
	void Start () {
		startPos = transform.position + new Vector3(0,3,0);
		rb = GetComponent<Rigidbody2D>();
		cc = GetComponent<CircleCollider2D>();
		ar = GetComponent<Animator>();
		//后面为wayPointsGos[usingIndex[Index]],敌人最大层为5依次下降,5-2=3、4-2=2、3-2=1、2-2=0；
		LoadPath(wayPointsGos[GameManager.Instance.usingIndex[GetComponent<SpriteRenderer>().sortingOrder-2]]);
		//LoadPath(wayPointsGos[Random.Range(0,wayPointsGos.Length)]);
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		Vector2 temp = Vector2.MoveTowards(transform.position,WayPoints[index],speed);
		if(transform.position != WayPoints[index]){
			rb.MovePosition(temp);
		}else{
			//index = (index+1)% WayPoints.Count;
			index++;
			if(index >= WayPoints.Count){
				index = 0;
				LoadPath(wayPointsGos[GameManager.Instance.usingIndex[GetComponent<SpriteRenderer>().sortingOrder-2]]);
			}
		}
		Vector2 dir = WayPoints[index] - transform.position;
		ar.SetFloat("DirX",dir.x);
		ar.SetFloat("DirY",dir.y);
		
	}

	private void LoadPath(GameObject go){
		WayPoints.Clear();
		foreach(Transform t in go.transform){
			WayPoints.Add(t.position);
		}
		WayPoints.Insert(0,startPos);
		WayPoints.Add(startPos);
	}
	private void OnTriggerEnter2D(Collider2D col){
		if(col.gameObject.name == "Pacman"){
			if(GameManager.Instance.isSuperPacman){
				//被超级吃豆人吃掉,回到重生点
				transform.position = startPos - new Vector3(0,3,0);
				index = 0;
			}
			else{
			col.gameObject.SetActive(false);
			GameManager.Instance.gamePanel.SetActive(false);
			Instantiate(GameManager.Instance.gameoverPrefab);
			GameManager.Instance.SetGameState(false);
			Invoke("ReStart",3f);
			//Destroy(col.gameObject);
			}
		}
	}

	private void ReStart(){
		SceneManager.LoadScene(0);
	}
}
