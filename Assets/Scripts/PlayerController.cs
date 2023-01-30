using Photon.Pun;
using UnityEngine;

public class PlayerController : MonoBehaviour, IPunObservable
{
    [SerializeField] private float _speed;
    [Space] 
    [SerializeField] private PhotonView _photonView;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    public Vector2Int Direction { get; private set; }
    public Vector2Int GamePosition { get; private set; }

    private void Start()
    {
        GamePosition = new Vector2Int((int)transform.position.x, (int)transform.position.y);
        
        FindObjectOfType<MapController>().AddPlayer(this);
    }

    private void Update()
    {
        if (_photonView.IsMine)
        {
            if (Input.GetKey(KeyCode.LeftArrow)) Direction = Vector2Int.left;
            if (Input.GetKey(KeyCode.RightArrow)) Direction = Vector2Int.right;
            if (Input.GetKey(KeyCode.UpArrow)) Direction = Vector2Int.up;
            if (Input.GetKey(KeyCode.DownArrow)) Direction = Vector2Int.down;
        }
        
        if (Direction == Vector2Int.left)
            _spriteRenderer.flipX = true;
        else if (Direction == Vector2Int.right)
            _spriteRenderer.flipX = false;

        transform.position = Vector3.Lerp(transform.position, (Vector2)GamePosition, Time.deltaTime * 3);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
            stream.SendNext(Direction);
        else
            Direction = (Vector2Int)stream.ReceiveNext();
    }
}