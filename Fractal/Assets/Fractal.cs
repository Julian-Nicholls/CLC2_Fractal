using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fractal : MonoBehaviour {

	//allows changing variables from editor
	public Mesh mesh;
	public Material material;
	public float childScale;

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
		gameObject.AddComponent<MeshFilter> ().mesh = mesh;
		gameObject.AddComponent<MeshRenderer> ().material = material;

		GetComponent<MeshRenderer> ().material.color = Color.Lerp (Color.white, Color.blue, (float)depth / maxDepth);

		//creates a new object if maxDepth isn't reached yet
		if (depth < maxDepth) {
			StartCoroutine (CreateChildren());
		}

	}

	private IEnumerator CreateChildren(){

		//for loop goes through stored positions and quaternions
		for (int i = 0; i < childDirections.Length; i++) {

			//random creates more varied intervals of object creation
			yield return new WaitForSeconds (Random.Range(0.1f, 0.5f));

			//"initialized" with parent object
			new GameObject ("Fractal Child").AddComponent<Fractal> ().Initialize (this, i);

		}
	}


	//passes important information from parent to child
	private void Initialize(Fractal parent, int childIndex){
		mesh = parent.mesh;
		material = parent.material;
		maxDepth = parent.maxDepth;
		depth = parent.depth + 1;
		childScale = parent.childScale;

		//this line ensures the child appears as a child in the hierarchy
		transform.parent = parent.transform;

		//move / rotate new child object 
		transform.localScale = Vector3.one * childScale;
		transform.localPosition = childDirections[childIndex] * (0.5f + 0.5f * childScale);
		transform.localRotation = childOrientations[childIndex];
	}
}
