using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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
        Heal,
        Dead,
    }
    [SerializeField] private State _state;
    private AiAgent _aiAgent;

    [SerializeField] private Text _aiStatusText;

    // Start is called before the first frame update
    void Start()
    {
        _aiAgent = GetComponent<AiAgent>();
        NextState();
    }

    // Update is called once per frame
    void Update()
    {
        // Allow for Game Replay
        if (_state == State.Dead && !_aiAgent.IsDead())
        {
            // AI lives again. Begin at nearest waypoint.
            _state = State.Flee;
            NextState();
        }
    }

    public void updateUI()
    {
        // Display State
        _aiStatusText.text = $"{_state}";
    }


    private void NextState()
    {
        // Process according to State
        switch (_state)
        {
            case State.Patrol:
                StartCoroutine(PatrolState());
                break;
            case State.Aware:
                StartCoroutine(AwareState());
                break;
            case State.Combat:
                StartCoroutine(CombatState());
                break;
            case State.Flee:
                StartCoroutine(FleeState());
                break;
            case State.Heal:
                StartCoroutine(HealState());
                break;
            case State.Dead:
                StartCoroutine(DeadState());
                break;
            default:
                Debug.LogWarning("_state doesn't exist within NextState, stopping statemachine");
                break;
        }

        // Display
        updateUI();
    }

    private IEnumerator PatrolState()
    {
        // Patrol
        _aiAgent.InCombatMode(false);
        _aiAgent.Search();

        while (_state == State.Patrol)
        {
            _aiAgent.Patrol();
            if (_aiAgent.IsPlayerInRange())
            {
                // On guard, player nearby.
                _state = State.Aware;
            }  
            yield return null;  // wait a single frame
        }
        NextState();
    }

    private IEnumerator AwareState()
    {
        // On guard. Player about.
        while (_state == State.Aware)
        {
            if (_aiAgent.InHealingMode())
            {
                // AI healing so prepare to flee
                _state = State.Flee;
            }
            else
            {
                if (_aiAgent.IsPlayerInRange()) //&& !_aiAgent.IsPlayerHit())
                {
                    // Prepare for battle
                    _state = State.Combat;
                }
                else
                {
                    // Player gone, carry on.
                    _state = State.Patrol;
                }
            }
            yield return null;  // wait a single frame
        }
        NextState();
    }

    private IEnumerator CombatState()
    {
        // Ready to fight?
        while (_state == State.Combat)
        {
            if (_aiAgent.IsPlayerHit() || _aiAgent.InHealingMode())
            {   // Player hit or AI too weak.
                _state = State.Flee;
            }
            else
            {   // Chase Player
                _aiAgent.InCombatMode(true);
                _aiAgent.ChasePlayer();
                if (!_aiAgent.IsPlayerInRange())
                {   // Player out of range, go back to Patrol
                    _state = State.Patrol;
                }
            }
            yield return null;  // wait a single frame
        }
        NextState();
    }

    private IEnumerator FleeState()
    {
        // Flee to nearest waypoint
        _aiAgent.InCombatMode(false);
        _aiAgent.Search();

        while (_state == State.Flee)
        {
            // Flee
            _aiAgent.Flee();

            if (_aiAgent.InHealingMode())
            {   // Enter Healing state
                _state = State.Heal;
            }
            else
            {   if (_aiAgent.IsWaypointHit())
                {   // Made it to waypoint so now get back on Patrol
                    _state = State.Patrol;
                }
            }
            yield return null;  // wait a single frame
        };
        NextState();
    }

    private IEnumerator HealState()
    {
        // Time to heal
        _aiAgent.InCombatMode(false);
        while (_state == State.Heal)
        {
            if (_aiAgent.IsDead())
            {
                // RIP
                _state = State.Dead;
            }
            else
            {
                if (_aiAgent.InHealingMode())
                {
                    // Heal
                    _aiAgent.Heal();
                }
                else
                {
                    // Just Flee
                    _state = State.Flee;
                }
            }
            yield return null;  // wait a single frame
        }
        NextState();
    }

    private IEnumerator DeadState()
    {
        // RIP
        _aiAgent.Dead();

        //_aiAgent.InCombatMode(false);
        //while (_state == State.Dead)
        //{
        //    if (_aiAgent.IsDead())
        //    {
        //        // Dead
        //        _aiAgent.Dead();
        //    }
        //    else
        //    {
        //        // Check wounds
        //        _state = State.Heal;
        //    }
        yield return null;  // wait a single frame
        //}
        //NextState();
    }
}
