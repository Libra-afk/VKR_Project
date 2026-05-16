using UnityEngine;
using KnightAdventure.Utils;
using System;
using System.Collections.Generic;

public class EnemyAi : MonoBehaviour
{
    [SerializeField] private State _startingState = State.Roaming;

    [Header("Roaming")]
    [SerializeField] private float _roamingDistanceMax = 7f;
    [SerializeField] private float _roamingDistanceMin = 3f;
    [SerializeField] private float _roamingTimerMax = 2f;

    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 2f;
    [SerializeField] private float _stopDistance = 0.1f;

    [SerializeField] private bool _isChasingEnemy = false;
    [SerializeField] private float _chasingDistance = 4f;
    [SerializeField] private float _chasingSpeedMultiplier = 2f;

    [SerializeField] private bool _isAttackingEnemy = false;
    [SerializeField] private float _attackingDistance = 2f;
    [SerializeField] private float _attackRate = 2f;
    private float _nextAttackTime = 0f;

    private Rigidbody2D _rb;
    private State _currentState;

    private float _roamingTimer;
    private Vector2 _roamPosition;
    private Vector2 _startingPosition;
    private Vector2 _moveDirection;

    private float _roamingSpeed;
    private float _chasingSpeed;

    private float _nextCheckDirectionTime = 0f;
    private float _checkDirectionDuration = 0.1f;
    private Vector3 _lastPosition;

    public EventHandler OnEnemyAttack;

    private List<Rect> _roomBounds;

    public bool IsRunning
    {
        get
        {
            return _moveDirection.magnitude > 0.1f;
        }
    }

    private enum State
    {
        Idle,
        Roaming,
        Chasing,
        Attacking,
        Death
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _currentState = _startingState;

        _roamingSpeed = _moveSpeed;
        _chasingSpeed = _moveSpeed * _chasingSpeedMultiplier;
    }

    private void Start()
    {
        _startingPosition = _rb.position;
        _roamingTimer = _roamingTimerMax;
        _roamPosition = GetRoamingPosition();
    }

    private void Update()
    {
        StateHandler();
        MovementDirectionHandler();
    }

    public void SetRoomBounds(List<Rect> roomBounds)
    {
        _roomBounds = roomBounds;
    }

    public void SetDeathState()
    {
        _moveDirection = Vector2.zero;
        _currentState = State.Death;
    }

    private void StateHandler()
    {
        switch (_currentState)
        {
            case State.Roaming:
                HandleRoaming();
                CheckCurrentState();
                break;

            case State.Chasing:
                ChasingTarget();
                CheckCurrentState();
                break;

            case State.Attacking:
                AttackingTarget();
                CheckCurrentState();
                break;

            case State.Death:
                _moveDirection = Vector2.zero;
                break;

            default:
            case State.Idle:
                _moveDirection = Vector2.zero;
                break;
        }
    }

    private void ChasingTarget()
    {
        if (Player.Instance == null)
        {
            _moveDirection = Vector2.zero;
            return;
        }
        Vector2 playerPosition = Player.Instance.transform.position;
        _moveDirection = (playerPosition - _rb.position).normalized;
        //ChangeFacingDirection(_rb.position, playerPosition);
    }

    public float GetRoamingAnimationSpeed()
    {
        return _moveSpeed / _roamingSpeed;
    }

    private void CheckCurrentState()
    {
        if (Player.Instance == null)
            return;

        float distanceToPlayer = Vector3.Distance(transform.position, Player.Instance.transform.position);
        State newState = State.Roaming;

        if (_isChasingEnemy)
        {
            if (distanceToPlayer <= _chasingDistance)
            {
                if (Player.Instance.IsAlive())
                {
                    newState = State.Chasing;
                }
                else
                {
                    newState = State.Roaming;
                }
            }
        }

        if (_isAttackingEnemy)
        {
            if (distanceToPlayer <= _attackingDistance)
            {
                if (Player.Instance.IsAlive())
                {
                    newState = State.Attacking;
                }
                else
                {
                    newState = State.Roaming;
                }
            }
        }

        if (newState != _currentState)
        {
            if (newState == State.Chasing)
            {
                _moveSpeed = _chasingSpeed;
            }
            else if (newState == State.Roaming)
            {
                _roamingTimer = 0f;
                _moveSpeed = _roamingSpeed;
            }
            else if (newState == State.Attacking)
            {
                _moveDirection = Vector2.zero;
            }

            _currentState = newState;
        }
    }

    private void AttackingTarget()
    {
        if (Time.time > _nextAttackTime)
        {
            OnEnemyAttack?.Invoke(this, EventArgs.Empty);

            _nextAttackTime = Time.time + _attackRate;
        }

    }


    private void MovementDirectionHandler()
    {
        if (Time.time > _nextCheckDirectionTime)
        {
            if (_currentState == State.Attacking && Player.Instance != null)
            {
                ChangeFacingDirection(transform.position, Player.Instance.transform.position);
            }
            else if (_currentState == State.Roaming && IsRunning)
            {
                ChangeFacingDirection(transform.position, _roamPosition);
            }
            else if (_currentState == State.Chasing && Player.Instance != null)
            {
                ChangeFacingDirection(transform.position, Player.Instance.transform.position);
            }

            _lastPosition = transform.position;
            _nextCheckDirectionTime = Time.time + _checkDirectionDuration;
        }
    }



    private void FixedUpdate()
    {
        _rb.MovePosition(_rb.position + _moveDirection * _moveSpeed * Time.fixedDeltaTime);
    }

    private void HandleRoaming()
    {
        _roamingTimer -= Time.deltaTime;

        float distance = Vector2.Distance(_rb.position, _roamPosition);

        if (distance <= _stopDistance)
        {
            _moveDirection = Vector2.zero;

            if (_roamingTimer <= 0f)
            {
                _roamPosition = GetRoamingPosition();
                _roamingTimer = _roamingTimerMax;
            }

            return;
        }

        _moveDirection = (_roamPosition - _rb.position).normalized;
    }

    private Vector2 GetRoamingPosition()
    {
        if (_roomBounds != null && _roomBounds.Count > 0)
        {
            Rect rect = _roomBounds[UnityEngine.Random.Range(0, _roomBounds.Count)];

            return new Vector2(
                UnityEngine.Random.Range(rect.xMin + 1f, rect.xMax - 1f),
                UnityEngine.Random.Range(rect.yMin + 1f, rect.yMax - 1f)
            );
        }

        return _startingPosition + Utils.GetRandomDir() * UnityEngine.Random.Range(_roamingDistanceMin, _roamingDistanceMax);
    }

    private void ChangeFacingDirection(Vector3 sourcePosition, Vector3 targetPosition)
    {
        if (sourcePosition.x > targetPosition.x)
        {
            transform.rotation = Quaternion.Euler(0, -180, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}

