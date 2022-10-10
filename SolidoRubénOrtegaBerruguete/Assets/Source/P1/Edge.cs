using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge : IComparable<Edge> //Define un método de comparación generalizado específico del tipo que implementa un tipo o una clase de valor con el fin de ordenar sus instancias.
{
    public int vertexA;
    public int vertexB;
    public int vertexC;

    public Edge(int vertexA, int vertexB, int vertexC)
    {
        if (vertexA < vertexB)
        {
            this.vertexA = vertexA;
            this.vertexB = vertexB;
            this.vertexC = vertexC;
        }
        else
        {
            this.vertexA = vertexB;
            this.vertexB = vertexA;
            this.vertexC = vertexC;
        }
    }

    public int CompareTo(Edge e) //Metodo necesario del interfaz IComparable para que sepa como comparar 
    {
        if (e == null)
        {
            return 1;
        }
        else if (this.vertexA == e.vertexA)
        {
            return this.vertexB.CompareTo(e.vertexB);
        }
        else
        {
            return this.vertexA.CompareTo(e.vertexA);
        }
    }

    public bool Compare(Edge e)
    {
        if (this.vertexA == e.vertexA && this.vertexB == e.vertexB)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
