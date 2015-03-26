using UnityEngine;
using System.Collections;
using CompleteProject;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviour
{
    private const string typeName = "UniqueGameName";
    public const string gameName = "Dat";

    private bool isRefreshingHostList = false;
    private HostData[] hostList;
	//public GameObject enemy;
    public GameObject playerPrefab;
	public PlayerPoint[] _ArraryPlayer;
	public Slider slider;
	public Text txtScore;
	public Text txtScoreAll;
	private GameObject p;


	//public PlayerHealth playerHealth;       // Reference to the player's heatlh.
	//public GameObject enemy;                // The enemy prefab to be spawned.
	//public float spawnTime = 3f;            // How long between each spawn.
	//public Transform[] spawnPoints ;         // An array of the spawn points this enemy can spawn from.

    void OnGUI()
    {
        if (!Network.isClient && !Network.isServer)
        {
            if (GUI.Button(new Rect(100, 100, 250, 100), "Start Server"))
                StartServer();

            if (GUI.Button(new Rect(100, 250, 250, 100), "Refresh Hosts"))
                RefreshHostList();

            if (hostList != null)
            {
                for (int i = 0; i < hostList.Length; i++)
                {
                    if (GUI.Button(new Rect(400, 100 + (110 * i), 300, 100), hostList[i].gameName))
                        JoinServer(hostList[i]);
                }
            }
        }
    }

	void Start()
	{
		MasterServer.ipAddress = "192.168.20.19";

	}

    private void StartServer()
    {
        Network.InitializeServer(5, 23466, !Network.HavePublicAddress());
        MasterServer.RegisterHost(typeName, gameName);
		EnemyManager[] arrNet; 
		//TODO 
		arrNet = GetComponents<EnemyManager> ();
		Debug.Log(arrNet.Length);
		foreach (EnemyManager item   in arrNet) {
			item.enabled=true;
			Debug.Log("run");
				}

		//enemy.SetActive(true);
		//InvokeRepeating ("Spawn", spawnTime, spawnTime);


    }

    void OnServerInitialized()
    {
        SpawnPlayer();
    }


    void Update()
    {
        if (isRefreshingHostList && MasterServer.PollHostList().Length > 0)
        {
            isRefreshingHostList = false;
            hostList = MasterServer.PollHostList();
        }
		if(p!=null)
		{
			slider.value=p.GetComponent<PlayerHealth>().percentHealth;
			txtScore.text=p.GetComponent<PlayerHealth>().PlayerScore.ToString();
		}
		GameObject[] allplayer = GameObject.FindGameObjectsWithTag ("Player");
		Debug.Log ("Number Player :"+allplayer.Length);
		double temple=0;
		foreach (GameObject item in allplayer) 
		{
			temple += item.GetComponent<PlayerHealth>().PlayerScoreTemple;
		}
		txtScoreAll.text= temple.ToString();

    }

    private void RefreshHostList()
    {

        if (!isRefreshingHostList)
        {
            isRefreshingHostList = true;
            MasterServer.RequestHostList(typeName);
        }

    }


    private void JoinServer(HostData hostData)
    {
        Network.Connect(hostData);
    }

    void OnConnectedToServer()
    {
        SpawnPlayer();
    }

	/*private void Spawn ()
	{

		// If the player has no health left...
		//TODO
		if(playerHealth.currentHealth <= 0f)
		{
			// ... exit the function.
			return;
		}
		
		// Find a random index between zero and one less than the number of spawn points.
		int spawnPointIndex = Random.Range (0, spawnPoints.Length);

		// Create an instance of the enemy prefab at the randomly selected spawn point's position and rotation.
		Network.Instantiate(enemy, spawnPoints[spawnPointIndex].position, spawnPoints[spawnPointIndex].rotation,1) ;
	}*/


    private void SpawnPlayer()
    {
        p = Network.Instantiate(playerPrefab, Vector3.zero, Quaternion.identity, 0) as GameObject;
		
		var camera = GameObject.FindGameObjectWithTag ("MainCamera");
		camera.GetComponent<CameraFollow> ().target = p.transform;
		//Slider healthSlider = GetComponent<Slider> ();
		//healthSlider.value = p.currentHealth;
		//Slider _Slider = transform.gameObject.GetComponent<Slider>();
		//Debug.Log (p.GetComponent<PlayerHealth> ().startingHealth.ToString ());
		//LBPoint.text =p.GetComponent<PlayerHealth>().currentHealth.ToString();


    }
}
