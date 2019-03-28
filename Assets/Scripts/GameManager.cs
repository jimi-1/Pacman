using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using LitJson;

public class GameManager : MonoBehaviour {
	private static GameManager _instance;
	public static GameManager Instance{
		get{
			return _instance;
		}
	}
	private List<GameObject> pacdotsGos =new List<GameObject>();
	private int score;
	private int win;

	public GameObject pacman;
	public GameObject blinky;
	public GameObject clyde;
	public GameObject inky;
	public GameObject pinky;
	public GameObject startPanel;
	public GameObject gamePanel;
	public GameObject startCountDownPrefab;
	public GameObject gameoverPrefab;
	public GameObject winrPrefab;
    public GameObject pacdot;
	public AudioClip startClip;
	public  Text sorceText;
   

	public Text messageText;

	public List<int> usingIndex = new List<int>();
	public List<int> rawIndex = new List<int>(){0,1,2,3};

    public bool isSuperPacman = false;

    public GameObject shellPrefab;

    public Vector3 a = Vector3.zero;
 

public Quaternion b = new Quaternion(0,0,0,0);
//public Vector3 a = Vector3.zero;


    //是否是暂停状态
    public bool isPaused = true;
    public GameObject menuGO;
   // public GameObject[] Pacdots;
    

	  //暂停状态
    private void Pause()
    {
        isPaused = true;
        menuGO.SetActive(true);
        Time.timeScale = 0; //时间停止（物体不运动）
        Cursor.visible = true; //使鼠标可见
    }
    //非暂停状态
    private void UnPause()
    {
        isPaused = false;
        menuGO.SetActive(false);
        Time.timeScale = 1;
        Cursor.visible = false;
    }

    public void ContinueGame()
    {
        ShowMessage("");
        UnPause();
    }

    public void OnRestart()//点击“重新开始”时执行此方法
    {
        //Loading Scene0
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        Time.timeScale = 1f;
        ShowMessage("");
    }

	public void QuitGame(){
		Application.Quit();
	}

        //保存游戏
    public void SaveGame()
    {
        SaveByJson();
        //SaveByBin();
    }

    //加载游戏
    public void LoadGame()
    {
        LoadByJson();
        //LoadByBin();  
    }


 
    //创建Save对象并存储当前游戏状态信息
    private Save CreateSaveGO()
    {
        //新建Save对象
        Save save = new Save();

        save.x = GameObject.Find("Pacman").transform.position.x;
        save.y = GameObject.Find("Pacman").transform.position.y;
        //save.Pacdots = pacdotsGos;

        foreach (GameObject t in pacdotsGos)
        {
            save.PacdotsX.Add(t.transform.position.x);
            save.PacdotsY.Add(t.transform.position.y);
        }



        //遍历所有的target
        //如果其中有处于激活状态的怪物，就把该target的位置信息和激活状态的怪物的类型添加到List中
        /*   foreach (GameObject targetGO in targetGOs)
          {
              //Maze.pacdotsGos
              TargetManager targetManager = targetGO.GetComponent<TargetManager>();
              if (targetManager.activeMonster != null)
              {
                  save.livingTargetPositions.Add(targetManager.targetPosition);
                  int type = targetManager.activeMonster.GetComponent<MonsterManager>().monsterType;
                  save.livingMonsterTypes.Add(type);
              }
          }*/
        //把shootNum和score保存在Save对象中
        save.score = score;
        //返回该Save对象
        return save;
    }
	

    private void SaveByBin()
    {
         Debug.Log("Thhd");

        //序列化过程（将Save对象转换为字节流）
        //创建Save对象并保存当前游戏状态
        Save save = CreateSaveGO();
        //创建一个二进制格式化程序
        BinaryFormatter bf = new BinaryFormatter();
        //创建一个文件流
        FileStream fileStream = File.Create(Application.dataPath + "/StreamingFile" + "/byBin.txt");
        //用二进制格式化程序的序列化方法来序列化Save对象,参数：创建的文件流和需要序列化的对象
        bf.Serialize(fileStream, save);
        //关闭流
        fileStream.Close();

        Debug.Log("bsgd");

        //如果文件存在，则显示保存成功
        if (File.Exists(Application.dataPath + "/StreamingFile" + "/byBin.txt"))
        {
         ShowMessage("保存成功");
        }
    }


    private void LoadByBin()
    {
        if(File.Exists(Application.dataPath + "/StreamingFile" + "/byBin.txt"))
        {
            Debug.Log("aewgh");

            //反序列化过程
            //创建一个二进制格式化程序
            BinaryFormatter bf = new BinaryFormatter();
            //打开一个文件流
            FileStream fileStream = File.Open(Application.dataPath + "/StreamingFile" + "/byBin.txt", FileMode.Open);
            //调用格式化程序的反序列化方法，将文件流转换为一个Save对象
            Save save = (Save)bf.Deserialize(fileStream);

            Debug.Log("nfds");
            //关闭文件流
            fileStream.Close();

            SetGame(save);
            ShowMessage("");

        }
        else
        {
            ShowMessage("存档文件不存在");
        }

        
    }



    //JSON:存档和读档
    private void SaveByJson()
    {
         Debug.Log("vvkad");

        Save save = CreateSaveGO();
        string filePath = Application.dataPath + "/StreamingFile" + "/byJson.json";
        //利用JsonMapper将save对象转换为Json格式的字符串
        string saveJsonStr = JsonMapper.ToJson(save);
        //将这个字符串写入到文件中
        //创建一个StreamWriter，并将字符串写入文件中
        StreamWriter sw = new StreamWriter(filePath);
        sw.Write(saveJsonStr);
        //关闭StreamWriter
        sw.Close();

        Debug.Log("ryawjkad");

        ShowMessage("保存成功");
    }
	



    private void LoadByJson()
    { 
        string filePath = Application.dataPath + "/StreamingFile" + "/byJson.json";
        if(File.Exists(filePath))
        {
            //创建一个StreamReader，用来读取流
            StreamReader sr = new StreamReader(filePath);
            //将读取到的流赋值给jsonStr
            string jsonStr = sr.ReadToEnd();
            //关闭
            sr.Close();

            //将字符串jsonStr转换为Save对象
            Save save = JsonMapper.ToObject<Save>(jsonStr);
            SetGame(save);
            ShowMessage("");
        }
        else
        {
            ShowMessage("存档文件不存在");
        }
    }



	    private void SetGame(Save save)
    {
        //先将所有的targrt里面的怪物清空，并重置所有的计时
        /*  foreach(GameObject targetGO in targetGOs)
          {
              targetGO.GetComponent<TargetManager>().UpdateMonsters();
          }
          //通过反序列化得到的Save对象中存储的信息，激活指定的怪物
          for(int i = 0; i < save.livingTargetPositions.Count; i++)
          {
              int position = save.livingTargetPositions[i];
              int type = save.livingMonsterTypes[i];
              targetGOs[position].GetComponent<TargetManager>().ActivateMonsterByType(type);
          }*/
        pacdotsGos.Clear();
        foreach (Transform t in GameObject.Find("Maze").transform)
        {
            Destroy(t.gameObject);
        }
        
        for(int i = 0; i < (save.PacdotsX.Count); i++)
        {
            //var tmp = GameObject.Find("Pacdot");
            //var nowPacdot = Instantiate(pacdot, GameObject.Find("Maze").transform);
            var nowPacdot = Instantiate(GameObject.Find("Pacdot"), GameObject.Find("Maze").transform);
            var tmp=nowPacdot.GetComponent<Collider2D>();
            var now=tmp.enabled = true;
            nowPacdot.name = "Pacdot";
            var nowPositon = new Vector2();
            nowPositon.x = (float)save.PacdotsX[i];
            nowPositon.y = (float)save.PacdotsY[i];
            nowPacdot.transform.position = nowPositon;
            nowPacdot.SetActive(true);
            pacdotsGos.Add(nowPacdot);
            win = pacdotsGos.Count;
            // nowPacdot.transform.parent = GameObject.Find("Maze").transform;
            //GameObject.Find("Maze").AddComponent(nowPacdot);

        }
        a =  new Vector3((float)save.x,(float)save.y,0);
        GameObject.Find("Pacman").GetComponent<Pacman>().dest=a;
        GameObject.Find("Pacman").GetComponent<Transform>().position = a;
        //GameObject.Find("Pacman").transform.Translate(22,5,0,Space.World);
        
        //Debug.Log(save.x +"   "+save.y);
        var tmpx=GameObject.Find("Pacman").GetComponent<Transform>().position.x;
        var tmpy = GameObject.Find("Pacman").GetComponent<Transform>().position.y;
        Debug.Log(save.x + "   " + save.y);
        Debug.Log(tmpx + "   " + tmpy);
       
        score = save.score;
        //更新UI显示
        sorceText.text = "Score:" + save.score.ToString();
        //调整为未暂停状态
        UnPause();
    }




	// Use this for initialization
	private void Awake(){
		_instance = this;
		int tempCount = rawIndex.Count;
		for(int i = 0;i <tempCount;i++){
			int tempIndex = Random.Range(0,rawIndex.Count);
			usingIndex.Add(rawIndex[tempIndex]);
			rawIndex.RemoveAt(tempIndex);
		}
		foreach(Transform t in GameObject.Find("Maze").transform){
			pacdotsGos.Add(t.gameObject);
		}
		win = pacdotsGos.Count;
	}
	private void Start(){
		SetGameState(false);
	}

	public void OnStartButton(){
		StartCoroutine(PlayerCountDown());
		AudioSource.PlayClipAtPoint(startClip,new Vector3(0,-5,0));
		startPanel.SetActive(false);
	}
	IEnumerator PlayerCountDown(){
		Object go = Instantiate(startCountDownPrefab);
		yield return new WaitForSeconds(3.3f);
		Destroy(go);
		SetGameState(true);
		gamePanel.SetActive(true);
		GetComponent<AudioSource>().Play();
	}
	public void OnExitButton(){
		Application.Quit();
	}
	// Update is called once per frame
	void Update () {


       

		 //判断是否按下ESC键，按下的话，调出Menu菜单，并将游戏状态更改为暂停状态
        if (Input.GetKeyDown(KeyCode.Escape))
        { 
            Pause();
        }

		if(gameObject.activeInHierarchy){
			//sorceText.text = "Score:" + score.ToString();
		}
		if(win == (score/100) && pacman.GetComponent<Pacman>().enabled != false){
			gamePanel.SetActive(false);
			Instantiate(winrPrefab);
			StopAllCoroutines();
			SetGameState(false);
		}
		if(win == (score/100) ){
			if(Input.anyKeyDown){
				SceneManager.LoadScene(0);
			}
		}
	}

	public void OnEatPacdot(GameObject go){
		score += 100;
        sorceText.text = "Score:" + score.ToString();
		pacdotsGos.Remove(go);
	}

    public void OnEatSuperPacdot(){
	    Invoke("CreateSuperPacdot",10f);
        isSuperPacman = true;
        Respawn();
        pacman.GetComponent<SpriteRenderer>().color = new Color(0.8f,0.8f,0.8f);
        FreezeEnemy();
        StartCoroutine(RecoveryEnemy());
	}

    IEnumerator RecoveryEnemy(){
        yield return new WaitForSeconds(5f);
        pacman.GetComponent<SpriteRenderer>().color = new Color(1.0f,1.0f,1.0f);
        DisFreezeEnemy();
        isSuperPacman = false;
    }

    private void FreezeEnemy(){
        blinky.GetComponent<GhostMove>().enabled = false;
		clyde.GetComponent<GhostMove>().enabled = false;
		inky.GetComponent<GhostMove>().enabled = false;
		pinky.GetComponent<GhostMove>().enabled = false; 
        blinky.GetComponent<SpriteRenderer>().color = new Color(0.7f,0.7f,0.7f);
		clyde.GetComponent<SpriteRenderer>().color = new Color(0.7f,0.7f,0.7f);
		inky.GetComponent<SpriteRenderer>().color = new Color(0.7f,0.7f,0.7f);
		pinky.GetComponent<SpriteRenderer>().color = new Color(0.7f,0.7f,0.7f);                 
    }

    private void DisFreezeEnemy(){
        blinky.GetComponent<GhostMove>().enabled = true;
		clyde.GetComponent<GhostMove>().enabled = true;
		inky.GetComponent<GhostMove>().enabled = true;
		pinky.GetComponent<GhostMove>().enabled = true; 
        blinky.GetComponent<SpriteRenderer>().color = new Color(1.0f,1.0f,1.0f);
		clyde.GetComponent<SpriteRenderer>().color = new Color(1.0f,1.0f,1.0f);
		inky.GetComponent<SpriteRenderer>().color = new Color(1.0f,1.0f,1.0f);
		pinky.GetComponent<SpriteRenderer>().color = new Color(1.0f,1.0f,1.0f);                 
    }


	public void SetGameState(bool state){
		pacman.GetComponent<Pacman>().enabled = state;
		blinky.GetComponent<GhostMove>().enabled = state;
		clyde.GetComponent<GhostMove>().enabled = state;
		inky.GetComponent<GhostMove>().enabled = state;
		pinky.GetComponent<GhostMove>().enabled = state;
        Invoke("CreateSuperPacdot",10f);
	}

	    public void ShowMessage(string str)
    {
        messageText.text = str;
    }

    private void CreateSuperPacdot(){
        int tempIndex = Random.Range(0,pacdotsGos.Count);
        pacdotsGos[tempIndex].transform.localScale = new Vector3(3,3,3);
        pacdotsGos[tempIndex].GetComponent<Pacdot>().isSuperPacdot = true;
    }

        public void Respawn()
    {
        //开启携程RespawnCo
        StartCoroutine("RespawnCo");
        
    }


    /// <summary>
    /// 携程，等待X秒后复活角色
    /// </summary>
    /// <returns></returns>
    public IEnumerator RespawnCo()
    {
        
        //隐藏角色
        pacman.gameObject.SetActive(false);
        //等待X秒后执行
        
        yield return new WaitForSeconds(1.0f);
        //把复活碰撞点的位置传给角色，让角色在复活碰撞点复活
        
     //   pacman.transform.position = GameObject.Find("Pacdot").GetComponent<Pacdot>().ResPosition;
       // pacman.transform.position = pacdot.ResPosition;

        //显示角色
        
        pacman.gameObject.SetActive(true);
    }
     
}
