/*====DIRECTED GRAPH SYSTEM
 * 
 *----Why not a Node Tree?
 * Because we want to allow for cyclical connections.
 * 
 *----TERMS
 * Node/Vertex
 * Edge/Link
 * Weight/Distance/Cost
 * Adjacenct/Neighboring - A node connected by a directed edge to another node. Adjacency is only reciprocal in Undirected Graphs.
 * Weighted - Specified distances/costs/weights between graphs. Unweighted graphs tend to be Undirected as well 
 * 
 *----Mathematical representation of a Graph:
 * V = {V1, V2, V3, V4}
 * E = {(V1, V2, 3), (V2, V4, 7), (V1, V3, 5), (V3, V1, 5)}
 * 
 *----Adjacency Matrix:
 * Row = Origin Vertex; Column = Destination Vertex; Value = Distance
 * 
 *	  A B C D				A →1 B →3 D
 *	A   1 5  	  ____		↓	 ↓
 *	B     1 3	  ____		5    ↓
 *	C        				C 1←←
 *	D        
 * 
 *----Common Graph Functions:
 * CreateGraph(): 
 *		Could also initialize graph using constructor, but a method creates cleaner code. This method basically creates the graph structure using information from a text file or database, and creates an adjacency matrix (usually the matrix is 2D array, and the list is an array with a linked list for each index).
 * AddEdge(node u, node v, int edge): adds edge between vertices
 * RemoveEdge(node u, node v): removes edge between vertices
 * Adjacent(node u, node v): tests whether edge exists between U and V
 * Neighbors(node u): lists any nodes adjacent to U
 *		Could be good for severing. Cut off arm? Also remove all adjacent bodyparts, ad finitum
 * GetNodeValue(node u): returns the value or data associated with a specified node
 * SetNodeValue(data, node): sets a value to a specified node
 * Display(): displays graph as adjacency matrix, in some graphical interfaces both the graph and table are drawn
 * 
 *----Graph Traversal: Breadth-First Search
 *
 *https://www.cs.usfca.edu/~galles/visualization/BFS.html
 *
 * Start at source node, explore neighbors, then their neighbors, etc.
    1  procedure BFS(Graph G, vertex v)
	2      let Q be a queue
	3      Q.enqueue(v)
	4      label v as discovered
	5      while Q is not empty
	6         v ← Q.dequeue()
	7         display(v) //display the vertex
	8         for all edges from v to w in G.adjacentEdges(v) do
	9             if w is not labeled as discovered
	10                Q.enqueue(w)
	11                label w as discovered
 * 
 *----Dijkstra's Shortest Path Algorithm
	1. Maintain a list of unvisited vertices.

	2. Choose a vertex (source) and assign maximum possible cost (i.e., infinity) to every other vertex.

	3. Cost of source remains 0.

	4. In every step of the algorithm it tries to minimize the cost for each vertex. 

		Minimization of cost is a multi-step process:

		a) For each unvisited neighbor (V2, V3, V4) of the current vertex (V1) calculate the new cost from the vertex (V1).

		b) E.g. the new cost of V2 is the minimum of the two ( (existing cost of V2) or (cost of V1 + cost of edge from V1 to V2) )

	5. When all neighbors of current node are considered, mark current node as visited and remove from unvisited list.

	6. Select a vertex from the list of unvisited nodes (which has the smallest cost) and repeat step 4.

	7. At the end there will be no possibilities to improve it further and then the algorithm ends
 * 
 * Pseudo-Code:
 * 
	 1  function Dijkstra(Graph, source):
	 2      create arrays dist[], prev[]
	 3      create PriorityQueue pq
	 4
	 5      for each vertex v in Graph:             // Initialization
	 6          dist[v] ← INFINITY                  // Unknown distance from source to v
	 7          prev[v] ← 0                         // Previous node in optimal path from source
	 8      
	 9      dist[source] ← 0                        // Distance from source to source
	10      enqueue source                          // source node
	11      enqueue u to pq                         // All nodes in the adjacency matrix (unvisited nodes) 
	12      while pq is not empty:
	13          u ← vertex in pq with min dist[u]    // Source node will be selected first
	14          remove u from pq 
	15          
	16          for each neighbor v of u:           // where v is still in Q.
	17              alt ← dist[u] + length(u, v)
	18              if alt < dist[v]:               // A shorter path to v has been found
	19                  dist[v] ← alt 
	20                  prev[v] ← u 
	21                  pq.enqueue(u, dist[v])
	22      return dist[], prev[]
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 */

using Microsoft.Xna.Framework;
using System.Text;

namespace PureHatred.Entities
{
	public class BodyPart : Item
	{
		public int HpMax;
		public int HpCurrent;
		public int HungerComplex;
		public int HungerSimple;

		public int Size;
		public int Capacity;
		public int Trunks;
		public int Branches;
		public int Dexterity;
		public int Striking;
		public int Stability;

		public StringBuilder TierPrefix = new StringBuilder("");
		public int AnatomyTier = 0;

		public BodyPart(Color foreground, Color background, string name, int glyph, int hungerComplex, int hungerSimple, BodyPart parent = null, int hpMax = 10, int hpCurrent = 10) : base(foreground, background, name, glyph)
		{
			Name = name;

			HpCurrent = hpCurrent;
			HpMax = hpMax;
		}
	}
}
