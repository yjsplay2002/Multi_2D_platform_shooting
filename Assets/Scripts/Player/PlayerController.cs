using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private LayerMask _groundLayer;
        [SerializeField] private float _speed = 4f;
        [SerializeField] private float _jumpPower = 300f;
        
        private Rigidbody2D _rigidbody2D;
        private Animator _animator;
        private SpriteRenderer _spriteRenderer;
        
        bool isGround;
        Vector3 curPos;
        private static readonly int VelocityX = Animator.StringToHash("velocityX");
        private static readonly int VelocityY = Animator.StringToHash("velocityY");
        private static readonly int Grounded = Animator.StringToHash("grounded");

        void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }


        void Update()
        {
            // Movement
            float axis = Input.GetAxisRaw("Horizontal");
            _rigidbody2D.velocity = new Vector2(_speed * axis, _rigidbody2D.velocity.y);

            _animator.SetFloat(VelocityX, Mathf.Abs(axis) );
            SetFlip(axis);
            _animator.SetFloat(VelocityY, _rigidbody2D.velocity.y);
            
            // Jump and ground check
            isGround = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(0, -0.5f), 0.07f, _groundLayer);
            _animator.SetBool(Grounded, isGround);

            if (Input.GetKeyDown(KeyCode.UpArrow) && isGround)
                Jump();
            
            
        }

        private void SetFlip(float axis)
        {
            if (axis != 0)
                _spriteRenderer.flipX = axis < 0;
        }

        private void Jump()
        {
            _rigidbody2D.velocity = Vector2.zero;
            _rigidbody2D.AddForce(Vector2.up * _jumpPower);
        }
    }
}