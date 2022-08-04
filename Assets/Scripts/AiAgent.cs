using UnityEngine;

public class AiAgent : MonoBehaviour
{
    // SerializeField = Makes var available to Unity
    [SerializeField] private GameObject _player;
    //[SerializeField] private Transform _waypoint;
    [SerializeField] private Transform[] _waypoints;
    [SerializeField] private int _waypointIndex = 0;

    [SerializeField] private float _speed;


    // Start is called before the first frame update
    void Start()
    {
        //     _waypoints = new Transform[4];
    }

    // Update is called once per frame
    void Update()
    { }

    public bool IsPlayerInRange()
    {
        if (Vector2.Distance(transform.position, _player.transform.position) < 5f)
        {
            return true;
        }     
        else
        {
            return false;
        }
            
    }

    public void ChasePlayer()
    {
        _speed *= 0.5f;
        MoveToPoint(_player.transform.position);
        _speed *= 2f;
    }


    public void Patrol()
    {
        Vector2 _pos;
        if (_waypointIndex < _waypoints.Length)
        {
            _pos = _waypoints[_waypointIndex].position;
            MoveToPoint(_pos);
            if (Vector2.Distance(transform.position, _pos) < 0.75f)
            {
                _waypointIndex++;
            }
        }
        else
        {
            _pos = _player.transform.position;
            MoveToPoint(_pos);
            if (Vector2.Distance(transform.position, _pos) < 0.75f)
            {
                _waypointIndex = 0;
            }
        }
    }
    //void MoveToObject(Transform obj)
    //{
    //    Vector2 _pos;
    //    _pos = obj.position;
    //    MoveToPoint(_pos);

    //}

    void MoveToPoint(Vector2 point)
    {
        Vector2 directionToPoint = point - (Vector2)this.transform.position;

        //float xAxis = Input.GetAxis("Horizontal");
        //float yAxis = Input.GetAxis("Vertical");
        //Vector2 move = new Vector2(xAxis, yAxis);
        if (directionToPoint.magnitude > 0.5f)
        {
            directionToPoint.Normalize();
            directionToPoint *= _speed * Time.deltaTime;
            transform.position += (Vector3)directionToPoint;

        }
    }
}
