using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidOriginal : MonoBehaviour
{
    List<GameObject> boids = new List<GameObject>();
    Rigidbody rb;

    void Start()
    {
        foreach(var b in GameObject.FindGameObjectsWithTag("Player"))
        {
            boids.Add(b); // add all
            if (b.name == transform.name) boids.Remove(b);//remove myself!
        }
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        AgentBehaviour();

        for(int i = 0; i < boids.Count; i++)
        {
            float dist = Vector3.Distance(transform.position, boids[i].transform.position);
            if(dist < 3 && dist > 1.2f)
            {
                //vision is (my forward compared to forward direction(from me to other boid)
               float vision = Vector3.Dot(transform.forward, boids[i].transform.position- transform.position);
               if(vision > 0.6f)
                {
                    Alignment(boids[i].transform.forward);
                    Cohesion( boids[i].transform.position);
                    LerpColour(Color.red, 10);
                }
            } else if(dist< 1.2f)
            {
                Seperate( boids[i].transform.position);
            }
        }
        LerpColour(Color.blue, 20);
    }

    void Alignment(Vector3 otherForward)
    {
        transform.forward -= (transform.forward - otherForward) / 200;
    }

    void Seperate( Vector3 boidPos) 
    {
        rb.AddForce((transform.position - boidPos) * 10);
    }

    void Cohesion(Vector3 boidPos)
    {
        rb.AddForce(( boidPos - transform.position) / 20);
    }

    void AgentBehaviour(float speed = 20, float boundary = 5, float zCorrection = 10, float yCorrection = 3)
    {
        GoForward(speed);
        Boundary(boundary);
        ZCorrection(zCorrection);
        XCorrection(yCorrection);
    }

    void GoForward(float speed)
    {
        rb.AddForce(transform.forward * speed);
    }

    void ZCorrection(float amount)
    {
        float dotZ = Vector3.Dot(-transform.right, Vector3.up) * amount;
        rb.AddTorque(transform.forward * dotZ);
    }

    void XCorrection(float amount)
    {
        float dotX = Vector3.Dot(transform.forward, Vector3.up) * amount;
        rb.AddTorque(transform.right * dotX);
    }

    void Boundary(float dist)
    {
        float bounadry = Vector3.Distance(Vector3.zero, transform.position);

        if (bounadry > dist)
        {
            Vector3 directionToCenter = Vector3.Normalize(transform.position - Vector3.zero);//face towards middle
            float dotForwardtoCenter = Vector3.Dot(directionToCenter, -transform.right);
            rb.AddTorque(0, dotForwardtoCenter * 1, 0);
        }

        // control up and down tilt 
        if (transform.position.y < 1) rb.AddTorque( transform.right *- 0.2f );
        if (transform.position.y > 3) rb.AddTorque(transform.right * 0.2f);
    }

    void LerpColour(Color setColour, float steps)
    {
        GetComponent<Renderer>().material.color -= (GetComponent<Renderer>().material.color - setColour) / steps;
    }
}
