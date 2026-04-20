using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem.Processors;
using HashTable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;
using System.Text;
using System.Xml.Linq;

public class PlayerController : MonoBehaviourPunCallbacks
{
    private Transform _transform;

	[SerializeField] private Transform visual;//to seperate the play and text for flip movements

	public PhotonView _pv;

    private Rigidbody2D _rb;

    public float speed;

    public float jumpPower;

    public float dartPower;

    //public int hp;

	GameSceneManager _gm; 



	[SerializeField] 
	private AudioSource hitSound;



	//lane boundaries 
	public float leftLimit;
	public float rightLimit;



	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        _transform = this.transform;
        _pv = this.gameObject.GetComponent<PhotonView>();
        _rb = this.gameObject.GetComponent<Rigidbody2D>();
        _gm = GameObject.Find("GameSceneManager").GetComponent<GameSceneManager>();



	}

    // Update is called once per frame
    void Update()
    {
        if (_pv.IsMine)
        {
            Control();

        }
    }

    void Control()
    {
		//left and right
        if (Input.GetKey(KeyCode.LeftArrow)|| Input.GetKey(KeyCode.A))
        {
            _transform.position += Vector3.left * speed * Time.deltaTime;

			visual.localScale = new Vector3(-1, 1, 1);//face left
			_pv.RPC("RPC_Flip", RpcTarget.Others, -1);//sync


		}
		if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
		{
			_transform.position += Vector3.right * speed * Time.deltaTime;

			visual.localScale = new Vector3(1, 1, 1);//face right
			_pv.RPC("RPC_Flip", RpcTarget.Others, 1);//sync
		}

		//limit - lane boundaries 
		_transform.position = new Vector3(
			Mathf.Clamp(_transform.position.x, leftLimit, rightLimit),
			_transform.position.y,
			_transform.position.z
			);

		//attack key
		if (Input.GetKeyDown(KeyCode.W))
		{
			//shoot straight up
			Vector3 offset = new Vector3(0, 2f, 0);

			GameObject DartObj = PhotonNetwork.Instantiate("PhotonDart", visual.position + offset, Quaternion.identity);

			Rigidbody2D brb = DartObj.GetComponent<Rigidbody2D>();

			brb.linearVelocity = new Vector2(0, dartPower);
		}

	}

	private void OnCollisionEnter2D(Collision2D other)
	{
		if (_pv != null && _pv.IsMine)
        {
            if (other.gameObject.CompareTag("Dart"))
            {
                PhotonView DartPV = other.gameObject.GetComponent<PhotonView>();

                if(DartPV != null)
                {
                    if (!DartPV.IsMine)
                    {

						hitSound.Play();

						string attackerName = DartPV.Owner.NickName;
						string myName = _pv.Owner.NickName;

						string attackerColored = $"<color=#FF4A4A>{attackerName}</color>";   // red
						string victimColored = $"<color=#4AA8FF>{myName}</color>";         // blue

						string msg = $"{attackerColored} hit {victimColored}";
						_gm.CallRpcSendMessageToAll(msg);

                    }
                }
            }
        }
	}


	[PunRPC]
	public void RPC_Flip(int dir)
	{
		visual.localScale = new Vector3(dir, 1, 1);
	}



}
