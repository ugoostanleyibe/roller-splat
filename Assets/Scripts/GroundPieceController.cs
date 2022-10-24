using UnityEngine;

public class GroundPieceController : MonoBehaviour
{
	public bool isColoured;

	public void SetColour(Color colour)
	{
		isColoured = true;
		GameManager.singleton.CheckCompletion();
		GetComponent<MeshRenderer>().material.color = colour;
	}
}
