using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;


public class Dart : MonoBehaviour
{
	private Rigidbody2D _rb;

	private float timer;

	private PhotonView _pv;
	public bool IsMine => _pv.IsMine;


	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		timer = 2f;
		_pv = GetComponent<PhotonView>();
		_rb = GetComponent<Rigidbody2D>();

	}

	
	// Update is called once per frame
	void Update()
	{
		timer -= Time.deltaTime;

		if (timer <= 0f)
		{
			if (_pv != null && _pv.IsMine)
			{
				PhotonNetwork.Destroy(gameObject);
			}
		}
	}
}
