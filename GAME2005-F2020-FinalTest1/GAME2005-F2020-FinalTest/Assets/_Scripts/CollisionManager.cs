using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CollisionManager : MonoBehaviour
{
    public CubeBehaviour[] cubes;
    public BulletBehaviour[] spheres;

    private static Vector3[] faces;

    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        cubes = FindObjectsOfType<CubeBehaviour>();

        faces = new Vector3[]
        {
            Vector3.left, Vector3.right,
            Vector3.down, Vector3.up,
            Vector3.back , Vector3.forward
        };
        spheres = FindObjectsOfType<BulletBehaviour>();

        // check each AABB with every other AABB in the scene
        for (int i = 0; i < cubes.Length; i++)
        {
            for (int j = 0; j < cubes.Length; j++)
            {
                if (i != j)
                {
                    CheckAABBs(cubes[i], cubes[j]);
                }
            }
        }

        // Check each sphere against each AABB in the scene
        foreach (var sphere in spheres)
        {
            foreach (var cube in cubes)
            {
                if (cube.name != "Player")
                {
                    CheckSphereAABB(sphere, cube);
                }
                
            }
        }


    }

    public static void CheckSphereAABB(BulletBehaviour s, CubeBehaviour b)
    {
        // get box closest point to sphere center by clamping
        var x = Mathf.Max(b.min.x, Mathf.Min(s.transform.position.x, b.max.x));
        var y = Mathf.Max(b.min.y, Mathf.Min(s.transform.position.y, b.max.y));
        var z = Mathf.Max(b.min.z, Mathf.Min(s.transform.position.z, b.max.z));

        var distance = Math.Sqrt((x - s.transform.position.x) * (x - s.transform.position.x) +
                                 (y - s.transform.position.y) * (y - s.transform.position.y) +
                                 (z - s.transform.position.z) * (z - s.transform.position.z));

        if ((distance < s.radius) && (!s.isColliding))
        {
            // determine the distances between the contact extents
            float[] distances = {
                (b.max.x - s.transform.position.x),
                (s.transform.position.x - b.min.x),
                (b.max.y - s.transform.position.y),
                (s.transform.position.y - b.min.y),
                (b.max.z - s.transform.position.z),
                (s.transform.position.z - b.min.z)
            };

            float penetration = float.MaxValue;
            Vector3 face = Vector3.zero;

            // check each face to see if it is the one that connected
            for (int i = 0; i < 6; i++)
            {
                if (distances[i] < penetration)
                {
                    // determine the penetration distance
                    penetration = distances[i];
                    face = faces[i];
                }
            }

            s.penetration = penetration;
            s.collisionNormal = face;
            //s.isColliding = true;

            
            Reflect(s);
        }

    }
    
    // This helper function reflects the bullet when it hits an AABB face
    private static void Reflect(BulletBehaviour s)
    {
        if ((s.collisionNormal == Vector3.forward) || (s.collisionNormal == Vector3.back))
        {
            s.direction = new Vector3(s.direction.x, s.direction.y, -s.direction.z);
        }
        else if ((s.collisionNormal == Vector3.right) || (s.collisionNormal == Vector3.left))
        {
            s.direction = new Vector3(-s.direction.x, s.direction.y, s.direction.z);
        }
        else if ((s.collisionNormal == Vector3.up) || (s.collisionNormal == Vector3.down))
        {
            s.direction = new Vector3(s.direction.x, -s.direction.y, s.direction.z);
        }
    }

    public static void CheckAABBs(CubeBehaviour a, CubeBehaviour b)
    {
        Contact contactB = new Contact(b);
        Contact contactA = new Contact(a);

        RigidBody3D aRb = a.GetComponentInParent(typeof(RigidBody3D)) as RigidBody3D;
        RigidBody3D bRb = b.GetComponentInParent(typeof(RigidBody3D)) as RigidBody3D;

        if ((a.min.x <= b.max.x && a.max.x >= b.min.x) &&
            (a.min.y <= b.max.y && a.max.y >= b.min.y) &&
            (a.min.z <= b.max.z && a.max.z >= b.min.z))
        {
            // determine the distances between the contact extents
            float[] distances = {
                (b.max.x - a.min.x),
                (a.max.x - b.min.x),
                (b.max.y - a.min.y),
                (a.max.y - b.min.y),
                (b.max.z - a.min.z),
                (a.max.z - b.min.z)
            };

            

                float penetration = float.MaxValue;
            Vector3 face = Vector3.zero;

            // check each face to see if it is the one that connected
            for (int i = 0; i < 6; i++)
            {
                if (distances[i] < penetration)
                {
                    // determine the penetration distance
                    penetration = distances[i];
                    face = faces[i];



                }
            }
            
            // set the contact properties
            contactB.face = face;
            contactB.penetration = penetration;


            // check if contact does not exist
            if (!a.contacts.Contains(contactB))
            {
                // remove any contact that matches the name but not other parameters
                for (int i = a.contacts.Count - 1; i > -1; i--)
                {
                    if (a.contacts[i].cube.name.Equals(contactB.cube.name))
                    {
                        a.contacts.RemoveAt(i);
                    }
                }

                if (a.name == "SquareBullet(Clone)")
                {
                    Debug.Log("SQUAREBULLET HIT");
                    SquareBulletBehaviour sbBehaviour = a.GetComponentInParent<SquareBulletBehaviour>();

                    if ((contactB.face == Vector3.forward) || (contactB.face == Vector3.back))
                    {
                        Debug.Log("BOUNCING BACK");
                        sbBehaviour.direction = new Vector3(sbBehaviour.direction.x, sbBehaviour.direction.y, -sbBehaviour.direction.z);
                    }
                    else if ((contactB.face == Vector3.right) || (contactB.face == Vector3.left))
                    {
                        sbBehaviour.direction = new Vector3(-sbBehaviour.direction.x, sbBehaviour.direction.y, sbBehaviour.direction.z);
                    }
                    else if ((contactB.face == Vector3.up) || (contactB.face == Vector3.down))
                    {
                        sbBehaviour.direction = new Vector3(sbBehaviour.direction.x, -sbBehaviour.direction.y, sbBehaviour.direction.z);
                    }
                    return;
                }
                else
                {

                    if (contactB.face == Vector3.down)
                    {
                        a.gameObject.GetComponent<RigidBody3D>().Stop();
                        a.isGrounded = true;
                    }
                    else if (contactB.face != Vector3.up)
                    {
                        //DIDN'T WORK QUITE RIGHT. HAPPENED EVERY FRAME RESULTING IN EXTRAORDINARY VELOCITY OF IMPULSE
                        // Relative velocity = Vr
                        // Collision Normal = n
                        // Coefficient of Restitution = e
                        // magnitude of the impulse = j
                        //Debug.Log("--------NEW COLLISION---------" + a + b);
                        //Vector3 relativeVelocity = bRb.velocity - aRb.velocity;
                        //Debug.Log("Relative Velocity: " + relativeVelocity);
                        //float relativeNormal = Vector3.Dot(relativeVelocity, contactB.face);
                        //Debug.Log("Relative Normal: " + relativeNormal);
                        //float coefficientOfRestitution = Mathf.Min(aRb.bounciness, bRb.bounciness);
                        //Debug.Log("coefficientOfRestitution: " + coefficientOfRestitution);
                        //float magnitudeOfVelocityAfterCollision = (-coefficientOfRestitution) * relativeNormal;
                        //Debug.Log("magnitudeOfVelocityAfterCollision: " + magnitudeOfVelocityAfterCollision);
                        //float magnitudeOfImpulse = (-(1 + coefficientOfRestitution) * (relativeNormal)) / ((1 / aRb.mass) + (1 / bRb.mass));
                        //Debug.Log("magnitudeOfImpulse: " + magnitudeOfImpulse);
                        //aRb.velocity = aRb.velocity - ((magnitudeOfImpulse / aRb.mass) * contactA.face);
                        //Debug.Log("A velocity: " + aRb.velocity);
                        //bRb.velocity = bRb.velocity - ((magnitudeOfImpulse / bRb.mass) * contactB.face);
                        //Debug.Log("B Velocity: " + bRb.velocity);
                        //Debug.Log("--------END OF COLLISION---------");

                        // THIS WORKS
                        if (b.gameObject.GetComponent<RigidBody3D>().bodyType == BodyType.DYNAMIC)
                        {
                            bRb.transform.position += contactB.face * penetration;
                            return;
                        }
                    }
                }

                // add the new contact
                a.contacts.Add(contactB);
                a.isColliding = true;
                
            }
        }

            else
        {

            if (a.contacts.Exists(x => x.cube.gameObject.name == b.gameObject.name))
            {
                a.contacts.Remove(a.contacts.Find(x => x.cube.gameObject.name.Equals(b.gameObject.name)));
                a.isColliding = false;

                if (a.gameObject.GetComponent<RigidBody3D>().bodyType == BodyType.DYNAMIC)
                {
                    a.gameObject.GetComponent<RigidBody3D>().isFalling = true;
                    a.isGrounded = false;
                }
            }
        }
    }
    }


