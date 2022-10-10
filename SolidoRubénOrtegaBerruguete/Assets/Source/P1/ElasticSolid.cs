using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Sample code for accessing MeshFilter data.
/// </summary>
public class ElasticSolid : MonoBehaviour 
{
    /// <summary>
    /// Default constructor. Zero all. 
    /// </summary>
    public ElasticSolid()
    {
        this.Paused = true;
        this.TimeStep = 0.01f;
        this.Gravity = new Vector3(0.0f, -9.81f, 0.0f);
        this.IntegrationMethod = Integration.Symplectic;
        this.mass = 1.0f;
        this.stiffness = 50.0f;
        this.damping = 0.0f;

        this.nodes = new List<Node> { };
        this.springs = new List<Spring> { };

        this.vertexes = new List<Vector3> { };
        this.tetrahedrons = new List<Tetrahedron> { };
        this.tetrahedronsList = new List<int> { };
    }

    public enum Integration
    {
        Explicit = 0,
        Symplectic = 1,
    };


    #region InEditorVariables

    public bool Paused;
    public float TimeStep;
    public float mass;
    public float stiffness;
    public float damping;
    public Vector3 Gravity;
    public Integration IntegrationMethod;

    private Mesh mesh;
    private Vector3[] modelVertex;
    private Vector4[] baricentricCoordinates;
    private List<Vector3> vertexes;

    private List<Node> nodes;
    private List<Spring> springs;

    private List<Tetrahedron> tetrahedrons;
    private List<int> tetrahedronsList;
    private int[] tetrahedronsContainsVertex;

    #endregion

    #region OtherVariables

    #endregion

    #region MonoBehaviour

    public void Awake()
    {
        this.mesh = this.GetComponentInChildren<MeshFilter>().mesh;
        this.modelVertex = mesh.vertices;
        this.baricentricCoordinates = new Vector4[modelVertex.Length];
        this.tetrahedronsContainsVertex = new int[modelVertex.Length];

        GetComponent<Parser>().ParseFiles();//Llamamos al metodo para que parsee toda la información
        //Una vez esta todo parseado cogemos los vertices y los tetraedros
        this.vertexes = GetComponent<Parser>().getVertices(); //
        this.tetrahedronsList = GetComponent<Parser>().getTetrahedrons();

        createNodes(this.vertexes);
        createTetrahedrons(this.tetrahedronsList);//los springs se crean ahora cuando se crean los tetraedros
        createBaricentricCoordinates(this.modelVertex, this.tetrahedrons);

        foreach (Tetrahedron tet in tetrahedrons)
        {
            //Se toma como entrada la masa mass del objeto y se calcula la masa de cada tetraedro, y esta masa se reparte a los nodos de dicho tetraedro
            tet.nodes[0].mass += tet.volume * mass/ 4;
            tet.nodes[1].mass += tet.volume * mass / 4;
            tet.nodes[2].mass += tet.volume * mass / 4;
            tet.nodes[3].mass += tet.volume * mass / 4;

            foreach (Spring spring in tet.springs)
            {
                //le damos un 6 del volumen del tetraedro a cada uno de sus muelles para luego hacer bien la rigidez
                spring.volume += tet.volume / 6;
            }
        }
    }

    public void Update()
    {
      
    }

    public void FixedUpdate()
    {
        if (this.Paused) // Si esta parado no se simula nada
            return;

        // Seleccionar el metodo de integración
        switch (this.IntegrationMethod)
        {
            case Integration.Explicit: this.stepExplicit(); break;
            case Integration.Symplectic: this.stepSymplectic(); break;
            default:
                throw new System.Exception("Esto no deberia pasar nunca");
        }
    }

    public void createNodes(List<Vector3> v)
    {
        for (int i = 0; i < v.Count; i++)
        {
            Vector3 pos = transform.TransformPoint(v[i]);

            Node newNode = new Node(pos, 0, this.Gravity, this.damping);
            nodes.Add(newNode);
        }
    }

    public void createTetrahedrons(List<int> t)
    {
        for (int i = 0; i < t.Count - 3; i += 4)
        {
            int vertex1 = t[i];
            int vertex2 = t[i + 1];
            int vertex3 = t[i + 2];
            int vertex4 = t[i + 3];

            //A la hora de crear la arista, si ya existe no la añadimos, evitamos aristas duplicadas
            Spring spring1 = new Spring(nodes[vertex1], nodes[vertex2], this.stiffness, this.damping);
            if (!springs.Contains(spring1)) springs.Add(spring1);
            Spring spring2 = new Spring(nodes[vertex1], nodes[vertex3], this.stiffness, this.damping);
            if (!springs.Contains(spring2)) springs.Add(spring2);
            Spring spring3 = new Spring(nodes[vertex1], nodes[vertex4], this.stiffness, this.damping);
            if (!springs.Contains(spring3)) springs.Add(spring3);
            Spring spring4 = new Spring(nodes[vertex2], nodes[vertex3], this.stiffness, this.damping);
            if (!springs.Contains(spring4)) springs.Add(spring4);
            Spring spring5 = new Spring(nodes[vertex4], nodes[vertex3], this.stiffness, this.damping);
            if (!springs.Contains(spring5)) springs.Add(spring5);
            Spring spring6 = new Spring(nodes[vertex2], nodes[vertex4], this.stiffness, this.damping);
            if (!springs.Contains(spring6)) springs.Add(spring6);

            Spring[] newSprings = { springs[springs.IndexOf(spring1)], springs[springs.IndexOf(spring2)], springs[springs.IndexOf(spring3)], springs[springs.IndexOf(spring4)], springs[springs.IndexOf(spring5)], springs[springs.IndexOf(spring6)] };
            

            this.tetrahedrons.Add(new Tetrahedron(nodes[vertex1], nodes[vertex2], nodes[vertex3], nodes[vertex4], newSprings));
        }
    }

    //Se comprueba que tetraedro contiene cada vertice y se llama a calcular sus coordenadas baricentricas
    public void createBaricentricCoordinates(Vector3[] v, List<Tetrahedron> t)
    {
        for (int i = 0; i < v.Length; i++)
        {
            bool found = false;
            int counter = 0;
            while (!found && counter < t.Count)//se mira en todos los tetraedros mientras que no se encuentre 
            {
                if (t[counter].contains(v[i]))//se llama al metodo contains del tetraedro
                {
                    found = true;
                    this.tetrahedronsContainsVertex[i] = counter;
                    this.baricentricCoordinates[i] = computeBaricentricCoordinates(t[counter], v[i]);
                }
                counter++;
            }
        }
    }

    //Calculo de los pesos o coordenadas baricentricas para un punto p dentro de un tetraedro.
    public Vector4 computeBaricentricCoordinates(Tetrahedron t, Vector3 v)
    {
        //los pesos se calculan diviendo el volumen del tetraedro formado por el punto y los otros 3 nodos, entre el volumen total
        Vector4 result = new Vector4();
        float volume = t.volume;
        result[0] = (Mathf.Abs(Vector3.Dot(t.nodes[1].pos - v, Vector3.Cross(t.nodes[2].pos - v, t.nodes[3].pos - v))) / 6) / volume;
        result[1] = (Mathf.Abs(Vector3.Dot(v - t.nodes[0].pos, Vector3.Cross(t.nodes[2].pos - t.nodes[0].pos, t.nodes[3].pos - t.nodes[0].pos))) / 6) / volume;
        result[2] = (Mathf.Abs(Vector3.Dot(t.nodes[1].pos - t.nodes[0].pos, Vector3.Cross(v - t.nodes[0].pos, t.nodes[3].pos - t.nodes[0].pos))) / 6) / volume;
        result[3] = (Mathf.Abs(Vector3.Dot(t.nodes[1].pos - t.nodes[0].pos, Vector3.Cross(t.nodes[2].pos - t.nodes[0].pos, v - t.nodes[0].pos))) / 6) / volume;
        return result;
    }


    public void OnDrawGizmos()
    {
        foreach (Node n in nodes)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(n.pos, 0.2f);
        }

        foreach (Spring s in springs)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(s.nodeA.pos, s.nodeB.pos);
        }

        try
        {
            foreach (Tetrahedron tetraedro in tetrahedrons)
            {
                Gizmos.color = Color.green;
                foreach (Node node in tetraedro.nodes)
                {
                    Vector3 position = transform.TransformPoint(node.pos);
                    Gizmos.DrawSphere(position, 0.1f);
                }

            }
        }
        catch (NullReferenceException e)
        {
        }
    }

    #endregion
    private void stepExplicit()
    {
        foreach (Node node in nodes)
        {
            node.force = Vector3.zero;
            node.ComputeForces();
        }
        foreach (Spring spring in springs)
        {
            
            spring.ComputeForces();
        }

        foreach (Node node in nodes)
        {
            if (!node.isFixed)
            {
                node.pos += TimeStep * node.vel;
                node.vel += TimeStep / this.mass * node.force;
            }
        }

        foreach (Spring spring in springs)
        {
            spring.UpdateLength();
        }

        for (int i = 0; i < this.modelVertex.Length; i++)
        {
            Vector3 pos = baricentricCoordinates[i][0] * tetrahedrons[tetrahedronsContainsVertex[i]].nodes[0].pos +
                baricentricCoordinates[i][1] * tetrahedrons[tetrahedronsContainsVertex[i]].nodes[1].pos +
                baricentricCoordinates[i][2] * tetrahedrons[tetrahedronsContainsVertex[i]].nodes[2].pos +
                baricentricCoordinates[i][3] * tetrahedrons[tetrahedronsContainsVertex[i]].nodes[3].pos;

            this.modelVertex[i] = transform.InverseTransformPoint(pos);
        }
        this.mesh.vertices = this.modelVertex;
    }

    private void stepSymplectic()
    {
        foreach (Node node in nodes)
        {
            node.force = Vector3.zero;
            node.ComputeForces();
        }
        foreach (Spring spring in springs)
        {
            spring.ComputeForces();
        }

        foreach (Node node in nodes)
        {
            if (!node.isFixed)
            {
                node.vel += TimeStep / this.mass * node.force;
                node.pos += TimeStep * node.vel;
            }
        }

        foreach (Spring spring in springs)
        {
            spring.UpdateLength();
        }

        for (int i = 0; i < this.modelVertex.Length; i++)
        {
            Vector3 pos = baricentricCoordinates[i][0] * tetrahedrons[tetrahedronsContainsVertex[i]].nodes[0].pos +
                baricentricCoordinates[i][1] * tetrahedrons[tetrahedronsContainsVertex[i]].nodes[1].pos +
                baricentricCoordinates[i][2] * tetrahedrons[tetrahedronsContainsVertex[i]].nodes[2].pos +
                baricentricCoordinates[i][3] * tetrahedrons[tetrahedronsContainsVertex[i]].nodes[3].pos;

            this.modelVertex[i] = transform.InverseTransformPoint(pos);
        }
        this.mesh.vertices = this.modelVertex;

    }

    public List<Node> getNodes()
    {
        return this.nodes;
    }

    public List<Spring> getSprings()
    {
        return this.springs;
    }

}
