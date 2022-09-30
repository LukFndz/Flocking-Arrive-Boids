using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : IState
{
    private StateMachine _sm;
    private Cazador _hunter;

    public PatrolState(Cazador c, StateMachine sm)
    {
        _sm = sm;
        _hunter = c;
    }

    public void ManualUpdate()
    {
        Advance();
    }

    public void Advance()
    {
        Collider[] colls = Physics.OverlapSphere(_hunter.transform.position, _hunter.ViewRadius, 1 << 6);

        if (colls.Length > 0)
        {
            _hunter.Target = colls[0].transform.parent.root.gameObject.GetComponent<Boid>();
            _sm.ChangeState("ChaseState");
        }
        else
        {
            Vector3 pointDistance = _hunter.wayPoints[_hunter.CurrentWayPoint].transform.position - _hunter.transform.position;

            if (pointDistance.magnitude < _hunter.stoppingDistance)
            {
                _hunter.CurrentWayPoint++;
                if (_hunter.CurrentWayPoint > _hunter.wayPoints.Length - 1)
                    _hunter.CurrentWayPoint = 0;
            }

            Seek();
        }

        _hunter.transform.position += _hunter.GetVelocity() * Time.deltaTime;
        _hunter.transform.forward = _hunter.GetVelocity();
    }

    private void Seek()
    {
        Vector3 desired;
        desired = _hunter.wayPoints[_hunter.CurrentWayPoint].transform.position - _hunter.transform.position;
        desired.Normalize();
        desired *= _hunter.MaxSpeed;

        Vector3 steering = desired - _hunter.GetVelocity();
        steering = Vector3.ClampMagnitude(steering, _hunter.MaxForce);

        _hunter.SetVelocity(Vector3.ClampMagnitude(_hunter.GetVelocity() + steering, _hunter.MaxSpeed));
    }
}
