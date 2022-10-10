using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetrahedron 
{
    public List<Node> nodes = new List<Node> ();
    public Spring[] springs;
    public float volume;

    public Tetrahedron(Node node1, Node node2, Node node3, Node node4, Spring[] springs_)
    {
        nodes.Add(node1);
        nodes.Add(node2);
        nodes.Add(node3);
        nodes.Add(node4);
        springs = springs_;
        this.volume = computeVolume();
    }

    public float computeVolume()
    {
        //Dados los 4 nodos del tetraedro, calculamos su volumen.
        float v;
        v = Mathf.Abs(Vector3.Dot(nodes[1].pos - nodes[0].pos, Vector3.Cross(nodes[2].pos - nodes[0].pos, nodes[3].pos - nodes[0].pos))) / 6;
        return v;
    }


    //Comprobación de la posición de un vértice respecto a la orientación de las caras del tetraedro.
    public bool contains(Vector3 v)
    {
        return same(nodes[0], nodes[1], nodes[2], nodes[3], v) && same(nodes[1], nodes[2], nodes[3], nodes[0], v) && same(nodes[2], nodes[3], nodes[0], nodes[1], v) && same(nodes[3], nodes[0], nodes[1], nodes[2], v);
    }

    //Hay que entenderlo bien para la defensa 
    private bool same(Node nodeA, Node nodeB, Node nodeC, Node nodeD, Vector3 v)
    {
        //Producto verctorial: Croos = es un vector perpendicular a los vectores que se multiplican
        //Producto escalar: Dot = devuelve un numero entero 
        Vector3 n = Vector3.Cross(nodeB.pos - nodeA.pos, nodeC.pos - nodeA.pos);
        return (Vector3.Dot(nodeD.pos - nodeA.pos, n) * Vector3.Dot(v - nodeA.pos, n)) >= 0;
    }

}
