using UnityEngine;
using UnityEngine.UI;

public class AiAgent : MonoBehaviour
{
    // SerializeField = Makes var available to Unity
    [SerializeField] private GameObject _player;
    [SerializeField] private Transform[] _waypoints;
    [SerializeField] private int _waypointIndex = 0;

    [SerializeField] private float _playerInRange = 3.5f;
    [SerializeField] private float _speed = 8f;
    [SerializeField] private float _hit = 0.5f;
 
    [SerializeField] private float _maxAiHealth = 100f;
    [SerializeField] private float _lowAiHealth = 50f;
    [SerializeField] private float _aiHealthHit = 10f;
    [SerializeField] private float _aiHealth;

    private float _prevHealth;
    private float _minSize, _maxSize, _x;
    private float _shrinkPercent;

    private bool isHealing = false;
    private bool isInCombat = false;
    private bool isWaypointHit = false;

    private PlayerHealth _playerHealth;

    [SerializeField] private Text aiHealthText;

    // Start is called before the first frame update
    void Start()
    {
        // init
        _playerHealth = GetComponent<PlayerHealth>();

        // Dimensions
        _maxSize = transform.localScale.x;
        _minSize = 0.5f * _maxSize;

        // Full Health
        ResetHealth();

        // Display
        updateUI();
    }

    // Update is called once per frame
    void Update()
    { }

    public void ResetHealth()
    {
        // Full Health
        _aiHealth = _maxAiHealth;
        SetHealth(_maxAiHealth);
    }

    public void Replay()
    {
        // Reset Health for both AI and Player
        ResetHealth();
        _playerHealth.ResetHealth();
    }

    public void updateUI()
    {
        // Update AI Health Display
        aiHealthText.text = $"{_aiHealth}";
    }

    public bool IsPlayerInRange()
    {
        // Check if Player within range of AI
        bool isInRange = false;

        if (Vector2.Distance(transform.position, _player.transform.position) < _playerInRange)
        {
            isInRange = true;
        }

        if (isInCombat && !isInRange)
        {
            InCombatMode(false);
        }

        return isInRange;
    }

    public void InCombatMode(bool inCombatMode)
    {
        // AI ready to Fight?
        isInCombat = inCombatMode;
    }

    private void SetHealth(float inHealthAdj)
    {
        // Set new health, adjusting to ensure it is in health range
        _prevHealth = _aiHealth;
        _aiHealth += inHealthAdj;
        if (_aiHealth > _maxAiHealth)
        {
            _aiHealth = _maxAiHealth;
        }
        else
        if (_aiHealth < 0f)
        {
            _aiHealth = 0f;
        }

        // if AI Health less than Low health Threshold
        if (_aiHealth < _lowAiHealth)
        {
            isHealing = true;
        }
        else
        {
            isHealing = false;
        }

        if (_prevHealth > 0)
        {   // Shrink / Grow according to health
            _shrinkPercent = _aiHealth / _prevHealth;
            _x = transform.localScale.x * _shrinkPercent;
            if ((_shrinkPercent < 1f && _x >= _minSize) || (_shrinkPercent > 1f && _x <= _maxSize))
            {
                transform.localScale *= _shrinkPercent;
            }
            else if (_shrinkPercent < 1f && _x < _minSize)
            {   // Shrink to min size but no more
                _shrinkPercent = _minSize / transform.localScale.x;
                transform.localScale *= _shrinkPercent;
            }
            else if (_shrinkPercent > 1f && _x > _maxSize)
            {   // Grow to max size but no more
                _shrinkPercent = _maxSize / transform.localScale.x;
                transform.localScale *= _shrinkPercent;
            }
        }

        // Show health status
        updateUI();

        if (IsDead())
        {   // Tell AI it is dead
            Dead();
        }
    }

    public bool IsDead()
    {
        // if AI Health is all gone then it is dead
        if (_aiHealth > 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public bool InHealingMode()
    {
        return isHealing;
    }

    public bool IsWaypointHit()
    {
        return isWaypointHit;
    }

    public void Shot()
    {
        // Double damage
        Hit(2 * _aiHealthHit);
    }

    public void Hit()
    {
        // Default damage
        Hit(_aiHealthHit);
    }

    private void Hit(float inHealthHit)
    {
        // Hit so reduce health
        SetHealth(-inHealthHit);
    }

    void AddHealth()
    {
        // Increase health by default amount
        AddHealth(_aiHealthHit);
    }

    void AddHealth(float inHealthBonus)
    {
        // Increase health by specified amount
        SetHealth(inHealthBonus);
    }

    public bool IsPlayerHit()
    {
        // Check if AI hit player.
        if (Vector2.Distance(transform.position, _player.transform.position) < _hit)
        {
            // Let player know they got hit
            _playerHealth.Hit();
            return true;
        }
        else
        {
            return false;
        }
    }

    public void ChasePlayer()
    {
        if (!_playerHealth.IsDead())
        {
            // Chase at half speed
            _speed *= 0.5f;
            MoveToPoint(_player.transform.position);
            _speed *= 2f;
        }
    }

    public void MoveToWaypoint()
    {
        // Move AI to target waypoint
        Vector2 _pos;
        isWaypointHit = false;
        if (_waypointIndex < _waypoints.Length)
        {   // Move towards target waypoint
            _pos = _waypoints[_waypointIndex].position;
            MoveToPoint(_pos);
            if (Vector2.Distance(transform.position, _pos) < _hit)
            {   // Waypoint reached. Target next waypoint on list
                isWaypointHit = true;
                _waypointIndex++;
                if (_waypointIndex >= _waypoints.Length)
                {   // Last Waypoint reached. Target 1st waypoint on list
                    _waypointIndex = 0;
                }
            }
        }
        else
        {   // Move towards player
            MoveToPoint(_player.transform.position);
            if (IsPlayerHit())
            {
                // Now target 1st waypoint on list
                _waypointIndex = 0;
            }
        }
    }

    public void Patrol()
    {
        MoveToWaypoint();
        if (IsWaypointHit())
        {   // Waypoint Hit, get mini health boost.
            AddHealth(_aiHealthHit * 0.5f);
        }
    }

    public void Flee()
    {
        // Double speed when fleeing
        _speed *= 2f;
        MoveToWaypoint();
        _speed *= 0.5f;
    }

    public void Heal()
    {
        // Half speed when healing
        _speed *= 0.5f;
        MoveToWaypoint();
        _speed *= 2f;
        if (IsWaypointHit())
        {   // Waypoint Hit, get health boost.
            AddHealth();
        }
    }


    //void MoveToObject(Transform obj)
    //{
    //    Vector2 _pos;
    //    _pos = obj.position;
    //    MoveToPoint(_pos);
    //}

    public void Search()
    {
        // Find and store nearest waypoint
        int _closestIndex = 0;
        float _closestDistance = float.MaxValue;

        // loop for every waypoint
        //for (init; condition; iterator)
        //for (int _index = 0; _index < _waypoints.Length; _index++)
        int _index = 0;
        while (_index < _waypoints.Length)
        {
            // distance to X waypoint
            float _currentDistance = Vector2.Distance(_waypoints[_index].position, transform.position);
            // if (distance to X < prev closest waypoint)
            if (_currentDistance < _closestDistance)
            {
                // new waypoint is closest
                _closestDistance = _currentDistance;
                _closestIndex = _index;
            }
            _index++;
        }

        _waypointIndex = _closestIndex;
    }

    void MoveToPoint(Vector2 point)
    {
        Vector2 directionToPoint = point - (Vector2)this.transform.position;

        //float xAxis = Input.GetAxis("Horizontal");
        //float yAxis = Input.GetAxis("Vertical");
        //Vector2 move = new Vector2(xAxis, yAxis);
        if (directionToPoint.magnitude > _hit)
        {
            directionToPoint.Normalize();
            directionToPoint *= _speed * Time.deltaTime;
            transform.position += (Vector3)directionToPoint;

        }
    }

    public void Dead()
    {
        // Placeholder for future code releases
        return;
    }
}
