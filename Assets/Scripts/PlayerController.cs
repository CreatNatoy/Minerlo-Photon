using Photon.Pun;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour, IPunObservable
{
    [SerializeField] private float _speed;
    [Space] 
    [SerializeField] private PhotonView _photonView;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Sprite _otherPlayerSprite;
    [SerializeField] private Sprite _deadPlayerSprite;
    [SerializeField] private Transform _ladder;
    [SerializeField] private TextMeshProUGUI _nicknameText;

    public bool IsDead { private set; get; }

    public Vector2Int Direction;
    public Vector2Int GamePosition;
    public PhotonView PhotonView => _photonView;
    public int Score = 0;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
            stream.SendNext(Direction);
        else
            Direction = (Vector2Int)stream.ReceiveNext();
    }

    private void Start()
    {
        GamePosition = new Vector2Int((int)transform.position.x, (int)transform.position.y);
        
        FindObjectOfType<MapController>().AddPlayer(this);

        _nicknameText.SetText(_photonView.Owner.NickName);
        if (!_photonView.IsMine) {
            _spriteRenderer.sprite = _otherPlayerSprite;
            _nicknameText.color = Color.red;
        }
    }

    private void Update()
    {
        if (_photonView.IsMine && !IsDead)
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

    public void SetLadderLength(int length) {
        for (int i = 0; i < _ladder.childCount; i++) {
            _ladder.GetChild(i).gameObject.SetActive(i < length);
        }

        while (_ladder.childCount < length) {
            Transform lastTile = _ladder.GetChild(_ladder.childCount - 1);
            Instantiate(lastTile, lastTile.position + Vector3.down, Quaternion.identity, _ladder);
        }
    }

    public void Kill() {
        IsDead = true;
        _spriteRenderer.sprite = _deadPlayerSprite;
        SetLadderLength(0);
    }
}