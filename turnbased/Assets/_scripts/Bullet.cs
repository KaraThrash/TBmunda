using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifeTime;
    public Vector3 holdvel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (lifeTime != -1)
        { lifeTime -= Time.deltaTime;
            if (lifeTime <= 0) { Destroy(this.gameObject); }
        }
        GetComponent<Rigidbody>().velocity = holdvel;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Rigidbody>() != null)
        {
            lifeTime = 0.1f;
           // collision.gameObject.GetComponent<Rigidbody>().AddForce((collision.transform.position - transform.position).normalized * 2550 * Time.deltaTime,ForceMode.Impulse);
            collision.gameObject.GetComponent<Rigidbody>().velocity = ((collision.transform.position - transform.position).normalized + Vector3.up) * 100.0f * Time.deltaTime;
        }
        if (collision.transform.tag == "destructable") { Destroy(collision.gameObject); }
    }
}
