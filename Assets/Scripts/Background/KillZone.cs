using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillZone : MonoBehaviour
{
    public Transform firstSpawn;
    public Transform secondSpawn;

    public CameraTrigger ct;

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (!ct.isInSecondRoom)
            {
                other.gameObject.transform.position = firstSpawn.position;
                other.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            }
            else
            {
                other.gameObject.transform.position = secondSpawn.position;
                other.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            }
        }
    }

}
