using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shot : MonoBehaviour
{
    // Unity cannot see this public var
    public Bullet shotByBullet = null;

    private AiAgent _aiAgent;

    // Start is called before the first frame update
    void Start()
    {
        _aiAgent = GetComponent<AiAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Lookout. Bullet about.
        Bullet bullet = collision.gameObject.GetComponent<Bullet>();
        if (bullet != null)
        {
            // We got hit by a bullet
            if (bullet == shotByBullet) return; // ignore if already hit by this bullet
            _aiAgent.Shot();
            shotByBullet = bullet;
        }
    }
}
