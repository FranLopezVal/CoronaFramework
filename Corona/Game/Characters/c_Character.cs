using System;
using System.Collections;
using UnityEngine;

namespace Corona
{

    /// <summary>
    /// Administra cualquier entidad de tipo 'character'/'personaje' utilizado
    /// en el sistema Corona.
    /// </summary>
    public class c_Character : c_WorldObject
    {

        #region VARIABLES
        [Header("Components")]
        private Rigidbody2D _rb;
        protected Animator _anim;

        [Header("Layer Masks")]
        [SerializeField] private LayerMask _groundLayer;
        [SerializeField] private LayerMask _wallLayer;
        [SerializeField] private LayerMask _cornerCorrectLayer;

        [Header("Movement Variables")]
        [SerializeField] private float _movementAcceleration = 70f;
        [SerializeField] private float _maxMoveSpeed = 12f;
        [SerializeField] private float _groundLinearDrag = 7f;
        protected float _horizontalDirection;
        protected float _verticalDirection;
        private bool _changingDirection => _rb.velocity.x > 0f && _horizontalDirection < 0f || _rb.velocity.x < 0f && _horizontalDirection > 0f;
        private bool _facingRight = true;
        private bool _canMove => !_wallGrab;
        protected bool _running = false;

        protected bool _canInput = true;

        [Header("Jump Variables")]
        [SerializeField] private float _jumpForce = 12f;
        [SerializeField] private float _airLinearDrag = 2.5f;
        [SerializeField] private float _fallMultiplier = 8f;
        [SerializeField] private float _lowJumpFallMultiplier = 5f;
        [SerializeField] private float _downMultiplier = 12f;
        [SerializeField] private int _extraJumps = 1;
        [SerializeField] private float _hangTime = .1f;
        [SerializeField] protected float _jumpBufferLength = .1f;
        private int _extraJumpsValue;
        private float _hangTimeCounter;
        protected float _jumpBufferCounter;
        protected bool _canJump => _jumpBufferCounter > 0f && (_hangTimeCounter > 0f || _extraJumpsValue > 0 || _onWall);
        private bool _isJumping = false;

        [Header("Wall Movement Variables")]
        [SerializeField] private float _wallSlideModifier = 0.5f;
        [SerializeField] private float _wallRunModifier = 0.85f;
        [SerializeField] private float _wallJumpXVelocityHaltDelay = 0.2f;
        private bool _wallGrab => _onWall && !_onGround && Input.GetButton("WallGrab") && !_wallRun;
        private bool _wallSlide => _onWall && !_onGround && !Input.GetButton("WallGrab") && _rb.velocity.y < 0f && !_wallRun;
        private bool _wallRun => _onWall && _verticalDirection > 0f;

        [Header("Dash Variables")]
        [SerializeField] private float _dashSpeed = 15f;
        [SerializeField] private float _dashLength = .3f;
        [SerializeField] protected float _dashBufferLength = .1f;
        protected float _dashBufferCounter;
        private bool _isDashing;
        private bool _hasDashed;
        private bool _canDash => _dashBufferCounter > 0f && !_hasDashed;

        [Header("Ground Collision Variables")]
        [SerializeField] private float _groundRaycastLength;
        [SerializeField] private Vector3 _groundRaycastOffset;
        private bool _onGround;

        [Header("Wall Collision Variables")]
        [SerializeField] private float _wallRaycastLength;
        private bool _onWall;
        private bool _onRightWall;

        [Header("Corner Correction Variables")]
        [SerializeField] private float _topRaycastLength;
        [SerializeField] private Vector3 _edgeRaycastOffset;
        [SerializeField] private Vector3 _innerRaycastOffset;
        private bool _canCornerCorrect;

        private event BuffCallBackUpdate _buffs;
        private List<c_Buff> _buffList;

        private c_StatsContainer _baseStats;
        private c_StatsContainer _addedStats;

        public c_StatsContainer AddedStats => _addedStats;

        public bool CanInput { get { return _canInput; } set { _canInput = value; } }

        [Header("Battle and play")]
        private float current_healt;
        private float current_stamina;
        private c_Race _race;

        
        public float CurrentHealt => current_healt;
        public float CurrentStamina => current_stamina;

        public float DashStaminaCost = 30;

        public c_Race Race => _race;
        #endregion

        #region UNITY_FUNCTIONS

        protected virtual void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _anim = GetComponentInChildren<Animator>();
            _buffList = new List<c_Buff>();


            _baseStats = new c_StatsContainer(c_StatsContainer.DEFAULT());
            _addedStats = _baseStats;
        }

        protected virtual void Start()
        {
            current_healt = _baseStats[c_Stats.HEALT].Value;
            current_stamina = _baseStats[c_Stats.STAMINA].Value;

        }

        protected virtual void Update()
        {
            ReloadStamina();

            Animation();

            //TODO: No se si va aqui o en Fixed
            //Update 7.0 ++++++++++++++++++++++++++++++++

            _buffs?.Invoke();
            for (int i = 0; i < _buffList.Count; i++)
            {
                if (_buffList[i].Disposed)
                {
                    _buffs -= _buffList[i].UpdateBuff;
                    _buffList.RemoveAt(i);
                    break;
                }
            }
            //+++++++++++++++++++++++++++++++++++++++++++
        }

        protected virtual void FixedUpdate()
        {
            CheckCollisions();
            if (_canDash) StartCoroutine(Dash(_horizontalDirection, _verticalDirection));
            if (!_isDashing)
            {
                if (_canMove) MoveCharacter();
                else _rb.velocity = Vector2.Lerp(_rb.velocity, new Vector2(_horizontalDirection * _maxMoveSpeed, _rb.velocity.y), .5f * Time.deltaTime);

                if (_onGround)
                {
                    ApplyGroundLinearDrag();
                    _extraJumpsValue = _extraJumps;
                    _hangTimeCounter = _hangTime;
                    _hasDashed = false;
                }
                else
                {
                    ApplyAirLinearDrag();
                    FallMultiplier();
                    _hangTimeCounter -= Time.fixedDeltaTime;
                    if (!_onWall || _rb.velocity.y < 0f || _wallRun) _isJumping = false;
                }
                if (_canJump)
                {
                    if (_onWall && !_onGround)
                    {
                        if (!_wallRun && (_onRightWall && _horizontalDirection > 0f || !_onRightWall && _horizontalDirection < 0f))
                        {
                            StartCoroutine(NeutralWallJump());
                        }
                        else
                        {
                            WallJump();
                        }
                        Flip();
                    }
                    else
                    {
                        Jump(Vector2.up);
                    }
                }
                if (!_isJumping)
                {
                    if (_wallSlide) WallSlide();
                    if (_wallGrab) WallGrab();
                    if (_wallRun) WallRun();
                    if (_onWall) StickToWall();
                }
            }
            if (_canCornerCorrect) CornerCorrect(_rb.velocity.y);
        }

        #endregion

        #region F_MOVEMENT

        /// <summary>
        /// Mueve el Personaje utilizado las variables:
        /// <see cref="_horizontalDirection"/> &&
        /// <see cref="_verticalDirection"/> &&
        /// <see cref="_movementAcceleration"/>
        /// </summary>
        protected void MoveCharacter()
        {
            _rb.AddForce(new Vector2(_horizontalDirection, 0f) * _movementAcceleration);

            if (Mathf.Abs(_rb.velocity.x) > (_running ? _maxMoveSpeed * 2 : _maxMoveSpeed))
                _rb.velocity = new Vector2(Mathf.Sign(_rb.velocity.x) * (_running ? _maxMoveSpeed * 2 : _maxMoveSpeed), _rb.velocity.y);

        }

        private void ApplyGroundLinearDrag()
        {
            if (Mathf.Abs(_horizontalDirection) < 0.4f || _changingDirection)
            {
                _rb.drag = _groundLinearDrag;
            }
            else
            {
                _rb.drag = 0f;
            }
        }

        private void ApplyAirLinearDrag()
        {
            _rb.drag = _airLinearDrag;
        }

        /// <summary>
        /// Hace saltar al personaje.
        /// </summary>
        /// <param name="direction"></param>
        protected void Jump(Vector2 direction)
        {
            if (!_onGround && !_onWall)
                _extraJumpsValue--;

            ApplyAirLinearDrag();
            _rb.velocity = new Vector2(_rb.velocity.x, 0f);
            _rb.AddForce(direction * _jumpForce, ForceMode2D.Impulse);
            _hangTimeCounter = 0f;
            _jumpBufferCounter = 0f;
            _isJumping = true;
        }

        protected void WallJump()
        {
            Vector2 jumpDirection = _onRightWall ? Vector2.left : Vector2.right;
            Jump(Vector2.up + jumpDirection);
        }

        IEnumerator NeutralWallJump()
        {
            Vector2 jumpDirection = _onRightWall ? Vector2.left : Vector2.right;
            Jump(Vector2.up + jumpDirection);
            yield return new WaitForSeconds(_wallJumpXVelocityHaltDelay);
            _rb.velocity = new Vector2(0f, _rb.velocity.y);
        }

        protected void FallMultiplier()
        {
            if (_verticalDirection < 0f)
            {
                _rb.gravityScale = _downMultiplier;
            }
            else
            {
                if (_rb.velocity.y < 0)
                {
                    _rb.gravityScale = _fallMultiplier;
                }
                else if (_rb.velocity.y > 0 && !Input.GetButton("Jump"))
                {
                    _rb.gravityScale = _lowJumpFallMultiplier;
                }
                else
                {
                    _rb.gravityScale = 1f;
                }
            }
        }

        protected void WallGrab()
        {
            _rb.gravityScale = 0f;
            _rb.velocity = Vector2.zero;
        }

        protected void WallSlide()
        {
            _rb.velocity = new Vector2(_rb.velocity.x, -_maxMoveSpeed * _wallSlideModifier);
        }

        void WallRun()
        {
            _rb.velocity = new Vector2(_rb.velocity.x, _verticalDirection * _maxMoveSpeed * _wallRunModifier);
        }

        void StickToWall()
        {
            //Push player torwards wall
            if (_onRightWall && _horizontalDirection >= 0f)
            {
                _rb.velocity = new Vector2(1f, _rb.velocity.y);
            }
            else if (!_onRightWall && _horizontalDirection <= 0f)
            {
                _rb.velocity = new Vector2(-1f, _rb.velocity.y);
            }

            //Face correct direction
            if (_onRightWall && !_facingRight)
            {
                Flip();
            }
            else if (!_onRightWall && _facingRight)
            {
                Flip();
            }
        }

        void Flip()
        {
            _facingRight = !_facingRight;
            transform.Rotate(0f, 180f, 0f);
        }

        IEnumerator Dash(float x, float y)
        {

            if (y <= -.2f && _onGround) yield break;
            if (!UseStamina(DashStaminaCost)) yield break;

            float dashStartTime = Time.time;
            _hasDashed = true;
            _isDashing = true;
            _isJumping = false;

            _rb.velocity = Vector2.zero;
            _rb.gravityScale = 0f;
            _rb.drag = 0f;

            Vector2 dir;
            if (x != 0f || y != 0f) dir = new Vector2(x, y);
            else
            {
                dir = _facingRight ? Vector2.right : Vector2.left;
            }

            while (Time.time < dashStartTime + _dashLength)
            {
                _rb.velocity = dir.normalized * _dashSpeed; //TODO: Multiply per Time.deltatime
                yield return null;
            }
            _rb.velocity = Vector3.zero;
            _isDashing = false;
        }

        #endregion

        #region F_ANIMATIONS

        void Animation()
        {
            if (_anim == null) return;

            if (_isDashing)
            {
                _anim.SetBool("isDashing", true);
                _anim.SetBool("isGrounded", false);
                _anim.SetBool("isFalling", false);
                _anim.SetBool("WallGrab", false);
                _anim.SetBool("isJumping", false);
                _anim.SetFloat("horizontalDirection", 0f);
                _anim.SetFloat("verticalDirection", 0f);
            }
            else
            {
                _anim.SetBool("isDashing", false);

                if ((_horizontalDirection < 0f && _facingRight || _horizontalDirection > 0f && !_facingRight) && !_wallGrab && !_wallSlide)
                {
                    Flip();
                }
                if (_onGround)
                {
                    _anim.SetBool("isGrounded", true);
                    _anim.SetBool("isFalling", false);
                    _anim.SetBool("WallGrab", false);
                    _anim.SetFloat("horizontalDirection", Mathf.Abs(_horizontalDirection));
                }
                else
                {
                    _anim.SetBool("isGrounded", false);
                }
                if (_isJumping)
                {
                    _anim.SetBool("isJumping", true);
                    _anim.SetBool("isFalling", false);
                    _anim.SetBool("WallGrab", false);
                    _anim.SetFloat("verticalDirection", 0f);
                }
                else
                {
                    _anim.SetBool("isJumping", false);

                    if (_wallGrab || _wallSlide)
                    {
                        _anim.SetBool("WallGrab", true);
                        _anim.SetBool("isFalling", false);
                        _anim.SetFloat("verticalDirection", 0f);
                    }
                    else if (_rb.velocity.y < -0.1f)
                    {
                        _anim.SetBool("isFalling", true);
                        _anim.SetBool("WallGrab", false);
                        _anim.SetFloat("verticalDirection", 0f);
                    }
                    if (_wallRun)
                    {
                        _anim.SetBool("isFalling", false);
                        _anim.SetBool("WallGrab", false);
                        _anim.SetFloat("verticalDirection", Mathf.Abs(_verticalDirection));
                    }
                }
            }
        }

        #endregion

        #region F_PHYSICS
        void CornerCorrect(float Yvelocity)
        {
            //Push player to the right
            RaycastHit2D _hit = Physics2D.Raycast(transform.position - _innerRaycastOffset + Vector3.up * _topRaycastLength, Vector3.left, _topRaycastLength, _cornerCorrectLayer);
            if (_hit.collider != null)
            {
                float _newPos = Vector3.Distance(new Vector3(_hit.point.x, transform.position.y, 0f) + Vector3.up * _topRaycastLength,
                    transform.position - _edgeRaycastOffset + Vector3.up * _topRaycastLength);
                transform.position = new Vector3(transform.position.x + _newPos, transform.position.y, transform.position.z);
                _rb.velocity = new Vector2(_rb.velocity.x, Yvelocity);
                return;
            }

            //Push player to the left
            _hit = Physics2D.Raycast(transform.position + _innerRaycastOffset + Vector3.up * _topRaycastLength, Vector3.right, _topRaycastLength, _cornerCorrectLayer);
            if (_hit.collider != null)
            {
                float _newPos = Vector3.Distance(new Vector3(_hit.point.x, transform.position.y, 0f) + Vector3.up * _topRaycastLength,
                    transform.position + _edgeRaycastOffset + Vector3.up * _topRaycastLength);
                transform.position = new Vector3(transform.position.x - _newPos, transform.position.y, transform.position.z);
                _rb.velocity = new Vector2(_rb.velocity.x, Yvelocity);
            }
        }

        private void CheckCollisions()
        {
            //Ground Collisions
            _onGround = Physics2D.Raycast(transform.position + _groundRaycastOffset, Vector2.down, _groundRaycastLength, _groundLayer) ||
                        Physics2D.Raycast(transform.position - _groundRaycastOffset, Vector2.down, _groundRaycastLength, _groundLayer);

            //Corner Collisions
            _canCornerCorrect = Physics2D.Raycast(transform.position + _edgeRaycastOffset, Vector2.up, _topRaycastLength, _cornerCorrectLayer) &&
                                !Physics2D.Raycast(transform.position + _innerRaycastOffset, Vector2.up, _topRaycastLength, _cornerCorrectLayer) ||
                                Physics2D.Raycast(transform.position - _edgeRaycastOffset, Vector2.up, _topRaycastLength, _cornerCorrectLayer) &&
                                !Physics2D.Raycast(transform.position - _innerRaycastOffset, Vector2.up, _topRaycastLength, _cornerCorrectLayer);

            //Wall Collisions
            _onWall = Physics2D.Raycast(transform.position, Vector2.right, _wallRaycastLength, _wallLayer) ||
                        Physics2D.Raycast(transform.position, Vector2.left, _wallRaycastLength, _wallLayer);
            _onRightWall = Physics2D.Raycast(transform.position, Vector2.right, _wallRaycastLength, _wallLayer);
        }
        #endregion

        #region F_MISC

        public bool HasStamina(float staminaCost)
        {
            return !(current_stamina - staminaCost < 0);
        }

        public bool UseStamina(float cost)
        {
            if (!HasStamina(cost)) return false;
            current_stamina -= cost;
            reloadTime = 0;
            return true;
        }

        //+++STAMINA VARIABLES
        float reloadTime = 0f;
        //++++++++++++++++++++++

        public void ReloadStamina()
        {
            if (current_stamina >= AddedStats[c_Stats.STAMINA].Value) return;

            reloadTime += Time.deltaTime;
            if (reloadTime >= 2f)
            {
                reloadTime = 0f;
                current_stamina += AddedStats[c_Stats.STAMINA_RELOAD].Value;
                if (current_stamina > AddedStats[c_Stats.STAMINA].Value) current_stamina = AddedStats[c_Stats.STAMINA].Value;
            }
        }


        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;

            //Ground Check
            Gizmos.DrawLine(transform.position + _groundRaycastOffset, transform.position + _groundRaycastOffset + Vector3.down * _groundRaycastLength);
            Gizmos.DrawLine(transform.position - _groundRaycastOffset, transform.position - _groundRaycastOffset + Vector3.down * _groundRaycastLength);

            //Corner Check
            Gizmos.DrawLine(transform.position + _edgeRaycastOffset, transform.position + _edgeRaycastOffset + Vector3.up * _topRaycastLength);
            Gizmos.DrawLine(transform.position - _edgeRaycastOffset, transform.position - _edgeRaycastOffset + Vector3.up * _topRaycastLength);
            Gizmos.DrawLine(transform.position + _innerRaycastOffset, transform.position + _innerRaycastOffset + Vector3.up * _topRaycastLength);
            Gizmos.DrawLine(transform.position - _innerRaycastOffset, transform.position - _innerRaycastOffset + Vector3.up * _topRaycastLength);

            //Corner Distance Check
            Gizmos.DrawLine(transform.position - _innerRaycastOffset + Vector3.up * _topRaycastLength,
                            transform.position - _innerRaycastOffset + Vector3.up * _topRaycastLength + Vector3.left * _topRaycastLength);
            Gizmos.DrawLine(transform.position + _innerRaycastOffset + Vector3.up * _topRaycastLength,
                            transform.position + _innerRaycastOffset + Vector3.up * _topRaycastLength + Vector3.right * _topRaycastLength);

            //Wall Check
            Gizmos.DrawLine(transform.position, transform.position + Vector3.right * _wallRaycastLength);
            Gizmos.DrawLine(transform.position, transform.position + Vector3.left * _wallRaycastLength);
        }

        protected override void Interact()
        {
            base.Interact();
        }

        //Update 7.0 +++++++++++++++++++++++++++++++++
        public void AddBuff(c_Buff buff)
        {
            if(_buffList.Contains(buff))
            {
                _buffList.Find(o => o.GetId == buff.GetId).AddDuration (buff);
            }else
            _buffList.Add(buff);
            buff.StartBuff(this);
            _buffs += buff.UpdateBuff;
        }

        public void RemoveAllBuffs()
        {
            _buffs = null;
            _buffList.Clear();
        }

        public bool HasBuff<T>() where T : c_Buff
        {
            foreach (var b in _buffList)
            {
                if (b is T) return true;
            }
            return false;
        }
        //++++++++++++++++++++++++++++++++++++++++++++
        #endregion

        #region F_BATTLE

        public void SetDamage() //UNCOMPLETE
        {

        }

        #endregion

    }
}
