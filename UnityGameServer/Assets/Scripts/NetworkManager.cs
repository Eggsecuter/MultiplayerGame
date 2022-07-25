using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
	public static NetworkManager instance;

	public GameObject playerPrefab;
	public int port;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else if (instance != this)
		{
			Destroy(this);
		}
	}

	private void Start()
	{
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = 30;

		Server.Instance.Start(50, port);
	}

	private void OnApplicationQuit()
	{
		Server.Instance.Stop();
	}

	public Player InstantiatePlayer()
	{
		return Instantiate(playerPrefab, new Vector3(0f, 0.5f, 0f), Quaternion.identity).GetComponent<Player>();
	}
}
