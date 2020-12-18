using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SquareBulletBehaviour : MonoBehaviour
{

    public float speed;
    public Vector3 direction;
    public float range;
   // public float radius;
    public bool debug;
    public bool isColliding;
    public Vector3 collisionNormal;
    public float penetration;

    public BulletManager bulletManager;

    //New Stuff
    //[Header("Cube Attributes")]
    //public Vector3 size;
    //public Vector3 max;
    //public Vector3 min;
    //public List<Contact> contacts;
    //private MeshFilter meshFilter;
    //public Bounds bounds;



    // Start is called before the first frame update
    void Start()
    {
        isColliding = false;

       // radius = Mathf.Max(transform.localScale.x, transform.localScale.y, transform.localScale.z) * 0.5f;
        bulletManager = FindObjectOfType<BulletManager>();

        //New stuff
        //meshFilter = GetComponent<MeshFilter>();

        //bounds = meshFilter.mesh.bounds;
        //size = bounds.size;
    }

    // Update is called once per frame
    void Update()
    {
        //max = Vector3.Scale(bounds.max, transform.localScale) + transform.position;
        //min = Vector3.Scale(bounds.min, transform.localScale) + transform.position;
        _Move();
        _CheckBounds();

    }

    private void _Move()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    private void _CheckBounds()
    {
        if (Vector3.Distance(transform.position, Vector3.zero) > range)
        {
            bulletManager.ReturnBullet(this.gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        if (debug)
        {
            Gizmos.color = Color.magenta;

           // Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}
