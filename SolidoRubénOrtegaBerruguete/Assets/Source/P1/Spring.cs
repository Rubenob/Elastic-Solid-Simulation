using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Spring 
{

    public Node nodeA, nodeB;

    public float Length0;
    public float Length;
    private float damp;

    public float stiffness;
    public float volume;

    public Spring(Node nodeA, Node nodeB, float stiffness, float damping)
    {
        this.nodeA = nodeA;
        this.nodeB = nodeB;
        this.stiffness = stiffness;
        this.damp = damping;
        this.volume = 0;

        UpdateLength();
        Length0 = Length;
    }

    public void UpdateLength()
    {
        Length = (nodeA.pos - nodeB.pos).magnitude;
    }

    public void ComputeForces()
    {
        Vector3 u = nodeA.pos - nodeB.pos;
        u.Normalize();
        //Vector3 force = -stiffness * (Length - Length0) * u;
        //Densidad de rigidez:
        Vector3 force = -volume / (float)Math.Pow(Length0, 2) * stiffness * (Length - Length0) * u;
        force += -(damp * Vector3.Dot(u, nodeA.vel - nodeB.vel)) * u;//amortiguamiento de deformación
        nodeA.force += force;
        nodeB.force -= force;
    }

    /*public bool Equals(Spring s)
    {
        if ((this.nodeA == s.nodeA) && (this.nodeB == s.nodeB))
            return true;
        else
            return false;
    }*/

}
