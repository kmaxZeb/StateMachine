using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AiAgent))]
public class StateMachine : MonoBehaviour
{
    //The Ai's current state
    public enum State
    {
        Patrol,
        Aware,
        Combat,
        Flee,
        Sleep,
        BerryPicking,
    }
    [SerializeField] private State _state;
    private AiAgent _aiAgent;

    // Start is called before the first frame update
    void Start()
    {
        _aiAgent = GetComponent<AiAgent>();
        NextState();
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}

    // Start is called before the first frame update
    private void NextState()
    {
        switch (_state)
        {
            case State.Patrol:
                StartCoroutine(PatrolState());
                break;
            case State.Combat:
                StartCoroutine(CombatState());
                break;
            //case State.Aware
            default:
                Debug.LogWarning("_state doesn't exist within NextState, stopping statemachine");
                break;
        }
        //StartCoroutine(PatrolState());
    }

    private IEnumerator PatrolState()
    {
        Debug.Log("Patrol : Enter");

        //int _i = 0;
        while (_state == State.Patrol)
        {
            //_i++;
            //Debug.Log("Patrol : Looping " + _i); 
            _aiAgent.Patrol();
            if (_aiAgent.IsPlayerInRange())
            {
                _state = State.Combat;
            }
            yield return null;  // wait a single frame
        }

        Debug.Log("Patrol : Exit");
        NextState();
    }

    private IEnumerator CombatState()
    {
        Debug.Log("Combat : Enter");
        //int _i = 0;
        while (_state == State.Combat)
        {
            //_i++;
            //Debug.Log("Combat : Looping " + _i);
            _aiAgent.ChasePlayer();
            if (!_aiAgent.IsPlayerInRange())
            {
                _state = State.Patrol;
            }
            yield return null;  // wait a single frame
        }
        
        Debug.Log("Combat : Exit");
        NextState();
    }
}
