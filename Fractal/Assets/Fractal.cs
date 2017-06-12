using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fractal : MonoBehaviour {

	//allows changing variables from editor
	public Mesh mesh;
	public Material material;
	public float childScale;
	public float spawnProbability;
	public float maxRotationSpeed;

	public Mesh[] meshes;

	//used to ensure dynamic batching
	//not certain what dynamic batching is, but has to do with mesh optimization
	private Material[,] materials;

	private float rotationSpeed;

	//depth information maintained to prevent infinite recursing
	public int maxDepth;
	private int depth;

	//array created to streamline local position
	private static Vector3[] childDirections = {
		Vector3.up,
		Vector3.right,
		Vector3.left,
		Vector3.forward,
		Vector3.back
	};

	//array created to streamline local rotation
	private static Quaternion[] childOrientations = {
		Quaternion.identity,
		Quaternion.Euler (0f, 0f, -90f),
		Quaternion.Euler (0f, 0f, 90f),
		Quaternion.Euler(90f, 0f, 0f),
		Quaternion.Euler(-90f, 0f, 0f)
	};

	// Use this for initialization
	void Start () {

		//create an array if it's null
		if (materials == null) {
			InitializeMaterials ();
		}

		//set a rotation speed
		rotationSpeed = Random.Range(-maxRotationSpeed, maxRotationSpeed);

		gameObject.AddComponent<MeshFilter> ().mesh = meshes[Random.Range(0, meshes.Length)];
		gameObject.AddComponent<MeshRenderer> ().material = materials[depth, Random.Range(0, 2)];

		//creates a new object if maxDepth isn't reached yet
		if (depth < maxDepth) {
			StartCoroutine (CreateChildren());
		}

	}

	void Update(){
		transform.Rotate (0f, rotationSpeed * Time.deltaTime, 0f);
	}
	

	private IEnumerator CreateChildren(){

		//for loop goes through stored positions and quaternions
		for (int i = 0; i < childDirections.Length; i++) {

			//change chances of each child spawning
			if (Random.value < spawnProbability) {

				//random creates more varied intervals of object creation
				yield return new WaitForSeconds (Random.Range(0.1f, 0.5f));

				//"initialized" with parent object
				new GameObject ("Fractal Child").AddComponent<Fractal> ().Initialize (this, i);
			}
		}
	}


	//passes important information from parent to child
	private void Initialize(Fractal parent, int childIndex){
		meshes = parent.meshes;
		materials = parent.materials;
		maxDepth = parent.maxDepth;
		depth = parent.depth + 1;
		childScale = parent.childScale;
		spawnProbability = parent.spawnProbability;
		maxRotationSpeed = parent.maxRotationSpeed;

		//this line ensures the child appears as a child in the hierarchy
		transform.parent = parent.transform;

		//move / rotate new child object 
		transform.localScale = Vector3.one * childScale;
		transform.localPosition = childDirections[childIndex] * (0.5f + 0.5f * childScale);
		transform.localRotation = childOrientations[childIndex];
	}

	private void InitializeMaterials(){
		materials = new Material[maxDepth + 1, 2];

		for (int i = 0; i <= maxDepth; i++) {

			float t = i / (maxDepth - 1f);
			materials [i, 0] = new Material (material);
			materials[i, 0].color =  Color.Lerp (Color.white, Color.blue, t);

			materials [i, 1] = new Material (material);
			materials[i, 1].color =  Color.Lerp (Color.white, Color.yellow, t);
		}

		materials [maxDepth, 0].color = Color.green;
		materials [maxDepth, 1].color = Color.red;
	}
}
