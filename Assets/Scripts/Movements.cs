using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Movements : MonoBehaviour
{
    private Rigidbody _body;
    [SerializeField] private Transform targetTrans;
    [SerializeField] private CatmullRomSpeedControlled catmull;
    [SerializeField] private List<Rigidbody> agentBodies;

    [SerializeField] private Transform[] pts;
    
    [SerializeField] private float _speed;
    
    Vector3 currentVelocity = Vector3.zero;
    Vector3 dirToTarget = Vector3.zero;
    Vector3 targetVelocity = Vector3.zero;
    Vector3 desiredVelocity = Vector3.zero;

    private float distance = 0.0f;
    
    [SerializeField] private steeringBehavior status = steeringBehavior.Seek;
    private enum steeringBehavior
    {
        Seek,
        Arrival,
        Evade,
        Flee,
        Pursuit
    }
    
    private void Start()
    {
        _body = GetComponent<Rigidbody>();
        _body.useGravity = false;
    }

    private void Update()
    {
        currentVelocity = _body.velocity;
        
        dirToTarget = (targetTrans.position - transform.position).normalized;
        distance = Vector3.Distance(targetTrans.position, transform.position);

        var alignment = computeAlignment();
        var cohesion = computeCohesion();
        var separation = computeSeparation();

        Vector3 computedVel = new Vector3(_body.velocity.x + alignment.x + cohesion.x + separation.x, _body.velocity.z,
            _body.velocity.y + alignment.z + cohesion.z + separation.z);
        
        _body.velocity += Vector3.Normalize(computedVel);
        
        switch (status)
        {
            case steeringBehavior.Seek:
                Seek();
                break;
            
            case steeringBehavior.Arrival:
                Seek();
                Arrival();
                break;
            
            case steeringBehavior.Evade:
                Evade();
                break;
            
            case steeringBehavior.Flee:
                Flee();
                break;
            
            case steeringBehavior.Pursuit:
                Pursuit(futurePos());
                break;
        }

        if (status != steeringBehavior.Evade)
        {
            _body.AddForce(desiredVelocity);
            _body.drag = 0.0f;
        }
    }

    void Seek()
    {
        transform.LookAt(targetTrans.position);
        
        targetVelocity = dirToTarget * _speed;
        desiredVelocity = targetVelocity - currentVelocity;
    }
    
    void Pursuit(Vector3 pos)
    {
        transform.LookAt(pos);
        
        targetVelocity = (pos - transform.position).normalized * _speed;
        desiredVelocity = targetVelocity - currentVelocity;
    }
    
    void Flee()
    {
        transform.LookAt(-targetTrans.position);
        
        targetVelocity = dirToTarget * _speed;
        desiredVelocity = targetVelocity - currentVelocity;
        desiredVelocity = -desiredVelocity;
    }

    private void Arrival()
    {
        if (distance < 5.2f)
        {
            targetVelocity = dirToTarget * distance;
            desiredVelocity = targetVelocity - currentVelocity;
        }
    }

    void Evade()
    {
        transform.LookAt(targetTrans.position);
        
        if (distance < 5.2f)
        {
            _body.drag = 0.0f;
            targetVelocity = -(dirToTarget * _speed);
            desiredVelocity = targetVelocity - currentVelocity;
        }
        else if (distance >= 5)
        {
            desiredVelocity = Vector3.zero;
            _body.angularVelocity = Vector3.zero;
            _body.drag = 5.0f;
        }
        
        _body.AddForce(desiredVelocity);
    }
    
    public Vector3 futurePos()
    {
        float T = distance/3.0f;
        Vector3 futurePos = targetTrans.position + targetTrans.gameObject.GetComponent<Rigidbody>().velocity * T;
        return futurePos;
    }

    Vector3 computeAlignment()
    {
        int neighborCount = 0;
        Vector3 velXZ = Vector3.zero;
        
        foreach (var agent in agentBodies)
        { 
            if (Vector3.Distance(agent.transform.position, transform.position) < 5.0f && agent != _body)
            { 
                velXZ += new Vector3(agent.velocity.x, 0, agent.velocity.z); 
                neighborCount++;
            }
        }

        velXZ.x /= neighborCount;
        velXZ.z /= neighborCount;
        Vector3.Normalize(velXZ);
        return velXZ;
    }

    Vector3 computeCohesion()
    {
        int neighborCount = 0;
        Vector3 velXZ = Vector3.zero;
        
        foreach (var agent in agentBodies)
        {
            if (Vector3.Distance(agent.transform.position, transform.position) < 5.0f && agent != _body)
            { 
                velXZ += new Vector3(agent.transform.position.x, 0, agent.transform.position.z); 
                neighborCount++; 
            }
        }

        velXZ.x /= neighborCount;
        velXZ.z /= neighborCount;
        Vector3 v = new Vector3(velXZ.x - transform.position.x, velXZ.y, velXZ.z - transform.position.z);
        Vector3.Normalize(v);
        return velXZ;
    }

    Vector3 computeSeparation()
    {
        int neighborCount = 0;
        Vector3 velXZ = Vector3.zero;
        
        foreach (var agent in agentBodies)
        {
            if (Vector3.Distance(agent.transform.position, transform.position) < 5.0f && agent != _body)
            { 
                velXZ += new Vector3(agent.transform.position.x - transform.position.x, 0, agent.transform.position.z - transform.position.z); 
                neighborCount++; 
            }
        }

        velXZ.x /= neighborCount;
        velXZ.z /= neighborCount;
        Vector3 v = new Vector3(velXZ.x - transform.position.x, velXZ.y, velXZ.z - transform.position.z);
        Vector3.Normalize(v);
        return velXZ;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Banana")
            StartCoroutine(Banana());
    }

    IEnumerator Banana()
    {
        _body.drag = 15.0f;

        yield return new WaitForSeconds(4.0f);
        
        _body.drag = 0.5f;
    }
}
