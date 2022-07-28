using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiAgent : MonoBehaviour
{
    // SerializeField = Makes var available to Unity
    [SerializeField] private GameObject player;
    [SerializeField] float speed;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector2 directionToPlayer = player.transform.position - this.transform.position;

        //float xAxis = Input.GetAxis("Horizontal");
        //float yAxis = Input.GetAxis("Vertical");
        //Vector2 move = new Vector2(xAxis, yAxis);
        if (directionToPlayer.magnitude > 0.01f)
        {
            directionToPlayer.Normalize();
            directionToPlayer *= speed * Time.deltaTime;

            transform.position += (Vector3) directionToPlayer;

        }
    }
}
