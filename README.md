# Elastic-Solid-Simulation

[![Gameplay Video]()](https://www.youtube.com/watch?v=jtHhDTB4e0Q)

En la escena se encuentra un modelo de gelatina, el cual tiene el script “ElasticSolid” y el script
“Parser”, un cubo que tiene asociado el script “Fixer”. En el script del cubo podemos arrastrar el
objeto que queremos que fije los vértices, en este caso sería el modelo que funciona como
sólido. El cual comprueba que nodos están dentro de él y los fija.

Para ejecutar la simulación en el script “Parser” que tiene el modelo hay que indicarle cuales son
los documentos de los cuales tiene que coger la información de los tetraedros y los nodos para
pasárselo a ElasticSolid. Por otra parte, en ElasticSolid declaramos los valores de TimeStep a
0.01, la masa a 1, el siffness a 10000 y un amortiguamiento de 0., el cual se ha añadido a través
de la fórmula de la amortiguación en el cálculo de la fuerza de los muelles.

Para el primer requisito lo que se ha realizado el script “ElasticSolid” que es muy parecido al
script “MassSpring” de la anterior práctico. Adicionalmente añadimos la función OnDrawGizmos
con el que pintamos gizmos en unos “nodes” y “springs” que hemos declarado “a fuego” en el
script “ElasticSolid” y que posteriormente se utilizara para pintar todos los nodos y los muelles
de nuestro modelo.

En el segundo requisito se ha utilizado la herramienta proporcionada por el profesor llamada
“tetgen”, con el que creamos un .txt a partir de un documento .ply, el cual se ha creado haciendo
un recubrimiento lowPoly sobre el modelo de la gelatina original, que nos proporciona las
posiciones de los vértices de un modelo 3D, que es el que usaremos en esta práctica. A
continuación, creamos un script llamado “Parser” que nos permite leer los .txt que hemos
conseguido usando “tetgen”. Después de haber leído estos documentos creamos “nodos” y
“springs” mediante los datos que nos han proporcionado estas posiciones, consiguiendo así
nuestro modelo 3D “lowpoly” creado mediante gizmos.

Para el tercer requisito se mete le modelo en Unity y se obtiene su mesh. Se crea un nuevo script
llamado “Tetrahedroms” con el que se crean tetraedros que contendrán los vértices de la malla
de nuestro modelo 3D y calculamos las coordenadas baricentricas de estos vértices para saber
dentro de que tetraedro se encuentra cada uno de los vértices de nuestra malla y saber así cómo
van a afectar los vértices de la malla “lowpoly” sobre nuestro modelo normal. A partir de ahora
se actualiza la malla de nuestro modelo normal para que parezca que tiene físicas, aunque
realmente todos los cálculos los hacemos sobre el “recubrimiento lowpoly”.

En el cuarto requisito se han acabado con las aristas duplicadas comparando cada vez que
creamos una arista con las que ya tenemos para no incluirla. También se ha aplicado la densidad
de masa, para la cual se calcula la masa de cada tetraedro y se reparte entre los 4 nodos
calculándola con el volumen del tetraedro y una densidad de masa. Por último, se ha añadido
una densidad de rigidez cambiando la formula del cálculo de fuerza elástica de los muelles.

![image](https://user-images.githubusercontent.com/69243718/194851460-d3c8094c-6d9f-4bca-b4fd-1a61866e8c68.png)

Para ello una vez se calcula el volumen del tetraedro también hace falta dividirlo en 6 y pasárselo
a cada uno de sus muelles. 
