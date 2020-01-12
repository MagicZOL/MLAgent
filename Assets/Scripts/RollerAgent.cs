using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class RollerAgent : Agent
{
    public Transform target;
    public float speed;

    Rigidbody rBody;

    private void Start()
    {
        rBody = GetComponent<Rigidbody>();
    }

    public override void AgentReset()
    {
        //Roller Agent 위치 초기화, 현재 y는 0.5의 좌표를 가짐
        if(this.transform.position.y < 0)
        {
            this.rBody.angularVelocity = Vector3.zero;
            this.rBody.velocity = Vector3.zero;
            this.transform.position = new Vector3(0, 3.5f, 0);
        }

        //Target 위치 초기화
        //0~4 사이의 값을 만들기 위해 *8 -4 라는 식이 탄생함
        target.position = new Vector3(Random.value * 8 - 4, 0.5f, Random.value * 8 - 4);
    }

    public override void CollectObservations()
    {
        //Target과 Agent위치 수집
        AddVectorObs(target.position); //가로안에 수집하려는 값 넣어줌
        AddVectorObs(this.transform.position);

        //Agent의 Velocity 수집
        AddVectorObs(rBody.velocity.x);
        AddVectorObs(rBody.velocity.z);
    }

    public override void AgentAction(float[] vectorAction) //vectorAction은 강화학습을 위한 랜덤한 값
    {
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = vectorAction[0];
        controlSignal.z = vectorAction[1];
        rBody.AddForce(controlSignal * speed);

        float distanceToTarget = Vector3.Distance(this.transform.position, target.position);

        if(distanceToTarget < 1.42f)
        {
            //점수주기
            SetReward(1.0f);

            //AgentReset를 호출시키는 함수
            Done();
        }

        if (this.transform.position.y < 0)
        {
            //SetReward(-1.0f);
            Done();
        }
    }

    public override float[] Heuristic()
    {
        var action = new float[2];
        action[0] = Input.GetAxis("Horizontal");
        action[1] = Input.GetAxis("Vertical");
        return action;
    }
}
