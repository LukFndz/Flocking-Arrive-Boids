using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : IState
{
    private StateMachine _sm;
    private Cazador _hunter;

    public ChaseState(Cazador c, StateMachine sm)
    {
        _sm = sm;
        _hunter = c;
    }

    public void ManualUpdate()
    {
        if (_hunter.Target == null)
        {
            _sm.ChangeState("PatrolState");
            return;
        }

        Advance();
        CheckStamina();
    }

    public void Advance()
    {
        Vector3 boidDistance = _hunter.Target.transform.position - _hunter.transform.position;

        if (boidDistance.magnitude <= _hunter.ViewRadius)
            Seek();
        else
            _hunter.Target = null;

        _hunter.transform.position += _hunter.GetVelocity() * Time.deltaTime;
        _hunter.transform.forward = _hunter.GetVelocity();
    }

    private void Seek()
    {
        Vector3 desired;
        desired = _hunter.Target.transform.position - _hunter.transform.position;
        desired.Normalize();
        desired *= _hunter.MaxSpeed;

        Vector3 steering = desired - _hunter.GetVelocity();
        steering = Vector3.ClampMagnitude(steering, _hunter.MaxForce);

        _hunter.SetVelocity(Vector3.ClampMagnitude(_hunter.GetVelocity() + steering, _hunter.MaxSpeed));
    }

    public void Rest()
    {
        _hunter.IsResting = true;
        _hunter.CurrentStamina =_hunter.Stamina;
        _sm.ChangeState("IdleState");
    }

    public void CheckStamina()
    {
        _hunter.CurrentStamina = _hunter.CurrentStamina - Time.deltaTime;

        if (_hunter.CurrentStamina <= 0 && !_hunter.IsResting)
            Rest();
    }
}
