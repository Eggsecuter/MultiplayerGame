using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager instance;

	public static Dictionary<int, PlayerManager> players = new Dictionary<int, PlayerManager>();

	public GameObject localPlayerPrefab;
	public GameObject playerPrefab;

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

	public void SpawnPlayer(int id, string username, Vector3 position, Quaternion rotation)
	{
		GameObject player;

		if (id == Client.instance.myId)
		{
			player = Instantiate(localPlayerPrefab, position, rotation);
		}
		else
		{
			player = Instantiate(playerPrefab, position, rotation);

			player.GetComponent<PlayerNameTag>().nameTagText.text = username;
		}

		var playerManager = player.GetComponent<PlayerManager>();
		playerManager.Initialize(id, username);

		players.Add(id, playerManager);
	}
}
