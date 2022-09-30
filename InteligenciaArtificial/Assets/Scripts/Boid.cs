using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    private Vector3 _velocity;

    [SerializeField]private float _maxSpeed;
    [Range(0.01f, 1f)]
    [SerializeField] private float _maxForce;
    [SerializeField] private float _viewRadius;
    [Range(0f, 0.5f)]
    [SerializeField] private float _cohesionWeight;
    [Range(0.5f, 1f)]
    [SerializeField] private float _alignWeight;
    [Range(0.5f, 1f)]
    [SerializeField] private float _separationWeight;

    private float _separationRadius;
    private Cazador _hunter;

    public static List<Boid> allBoids = new List<Boid>();
    private void Start()
    {
        _hunter = GameManager.Instance.hunter;

        allBoids.Add(this);

        Vector3 random = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));

        AddForce(random.normalized * _maxSpeed);
    }
    private void Update()
    {
        CheckBounds();
        Advance();
    }
    private void CheckBounds()
    {
        transform.position = GameManager.Instance.SetObjectBoundPosition(transform.position);
    }
    private void Advance()
    {
        Vector3 hunterDistance = _hunter.transform.position - transform.position;

        if (hunterDistance.magnitude <= _viewRadius)
        {
            Evade();
        }
        else
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, _viewRadius, 1 << 7);

            if (colliders.Length > 0)
                Arrive(colliders[0].transform);
            else
                AddForce(GetSeparation() * _separationWeight + GetAlignment() * _alignWeight + GetCohesion() * _cohesionWeight);
        }

        transform.position += _velocity * Time.deltaTime;
        transform.forward = _velocity.normalized;
    }
    private Vector3 GetCohesion()
    {
        Vector3 desired = Vector3.zero;
        int count = 0;

        foreach (var item in allBoids)
        {
            if (item == this) continue;

            if (Vector3.Distance(transform.position, item.transform.position) <= _viewRadius)
            {
                desired += item.transform.position;
                count++;
            }
        }

        if (count == 0) return desired;

        desired /= count;

        desired -= transform.position;

        return CalculateSteering(desired);
    }
    private Vector3 GetSeparation()
    {
        Vector3 desired = Vector3.zero;

        foreach (Boid boid in allBoids)
        {
            if (boid == this) continue;

            Vector3 dist = boid.transform.position - transform.position;
            if (dist.magnitude <= _viewRadius)
            {
                desired += dist;
            }
        }
        if (desired == Vector3.zero) return desired;

        desired *= -1;

        return CalculateSteering(desired);
    }
    private Vector3 GetAlignment()
    {
        Vector3 desired = Vector3.zero;
        int count = 0;
        foreach (var item in allBoids)
        {
            if (item == this) continue;
            if (Vector3.Distance(transform.position, item.transform.position) <= _viewRadius)
            {
                desired += item._velocity;
                count++;
            }
        }
        if (count == 0) return desired;
        desired /= count;

        return CalculateSteering(desired);
    }
    private void Evade()
    {
        Vector3 desired;

        Vector3 pos = _hunter.transform.position + _hunter.Velocity * Time.deltaTime;

        desired = pos - transform.position;
        desired.Normalize();
        desired *= _maxSpeed;
        desired *= -1;

        Vector3 steering = desired - _velocity;
        steering = Vector3.ClampMagnitude(steering, _maxForce);

        _velocity = Vector3.ClampMagnitude(_velocity + steering, _maxSpeed);
    }
    private void Arrive(Transform target)
    {
        Vector3 desired;
        desired = target.transform.position - transform.position;

        desired.Normalize();
        desired *= _maxSpeed;

        Vector3 steering = desired - _velocity;
        steering = Vector3.ClampMagnitude(steering, _maxForce);

        _velocity = Vector3.ClampMagnitude(_velocity + steering, _maxSpeed);
    }
    Vector3 CalculateSteering(Vector3 desired)
    {
        return Vector3.ClampMagnitude((desired.normalized * _maxSpeed) - _velocity, _maxForce);
    }
    private void AddForce(Vector3 force)
    {
        _velocity = Vector3.ClampMagnitude(_velocity + force, _maxSpeed);
    }
    public Vector3 GetVelocity()
    {
        return _velocity;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Hunter"))
        {
            allBoids.Remove(this);
            Destroy(gameObject);
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _viewRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _separationRadius);
    }
}
