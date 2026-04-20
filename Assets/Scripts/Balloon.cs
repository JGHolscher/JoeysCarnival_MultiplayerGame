using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;


public class Balloon : MonoBehaviour
{
	private PhotonView pv;
	public int lane;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		pv = GetComponent<PhotonView>();
	}

	// Update is called once per frame
	void Update()
	{

	}

	//dart hit balloon
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!PhotonNetwork.IsMasterClient) return;

		if (collision.CompareTag("Dart"))
		{
			//Dart dart = collision.GetComponent<Dart>();
			PhotonView dartPV = collision.GetComponent<PhotonView>();

			if (dartPV == null) return;


			Player shooter = dartPV.Owner;

			GameSceneManager gsm = FindAnyObjectByType<GameSceneManager>();

			//PhotonView dartPV = collision.GetComponent<PhotonView>();

			//track score

			//gsm.AddScore(dartPV.Owner, 10);
			gsm.AddScore(shooter, 10);

			gsm.BalloonPopped(lane);

			// Destroy the dart and balloon
			//PhotonNetwork.Destroy(collision.gameObject);
			if (dartPV.IsMine)
			{
				PhotonNetwork.Destroy(collision.gameObject);
			}

			//PhotonNetwork.Destroy(this.gameObject);
			if (pv != null && pv.IsMine)
			{
				PhotonNetwork.Destroy(gameObject);
			}
		}
	}
}

