using UnityEngine;

public class BallController : MonoBehaviour
{
	private Vector2 currentSwipe, swipePosLastFrame, swipePosCurrentFrame;
	private Vector3 nextCollisionPosition, movementVector;
	private Color randomLightColour;

	private AudioSource audioSource;
	private Rigidbody ballRigidBody;

	public ParticleSystem fireworksParticle;
	public AudioClip moveSound;

	public int minSwipeRecognition;
	public bool canPlaySound;
	public bool isMoving;
	public float speed;

	private void Start()
	{
		// Only take pretty light colors
		audioSource = GetComponent<AudioSource>();
		ballRigidBody = GetComponent<Rigidbody>();
		randomLightColour = Random.ColorHSV(0.5f, 1.0f);
		GetComponent<MeshRenderer>().material.color = randomLightColour;
	}

	private void FixedUpdate()
	{
		// Set the balls speed when it should travel
		if (isMoving)
		{
			ballRigidBody.velocity = movementVector * speed;
		}

		// Paint the ground
		foreach (var collider in Physics.OverlapSphere(transform.position - (Vector3.up / 2), 0.05f))
		{
			var piece = collider.gameObject.GetComponent<GroundPieceController>();

			if (piece && !piece.isColoured)
			{
				piece.SetColour(randomLightColour);
			}
		}

		// Check if we have reached our destination
		if (Vector3.Distance(transform.position, nextCollisionPosition) <= 1.0f)
		{
			nextCollisionPosition = Vector3.zero;
			movementVector = Vector3.zero;
			canPlaySound = true;
			isMoving = false;
		}

		if (!isMoving)
		{
			// Swipe mechanism
			if (Input.GetMouseButton(0))
			{
				// Where is the mouse now?
				swipePosCurrentFrame = new(Input.mousePosition.x, Input.mousePosition.y);

				if (swipePosLastFrame != Vector2.zero)
				{

					// Calculate the swipe direction
					currentSwipe = swipePosCurrentFrame - swipePosLastFrame;

					// Minimum amount of swipe recognition
					if (currentSwipe.sqrMagnitude < minSwipeRecognition)
					{
						return;
					}

					// Normalize it to only get the direction not the distance (would fake the balls speed)
					currentSwipe.Normalize();

					// Up/Down swipe
					if (-0.5f < currentSwipe.x && currentSwipe.x < 0.5f)
					{
						SetDestination(currentSwipe.y > 0 ? Vector3.forward : Vector3.back);
					}

					// Left/Right swipe
					if (-0.5f < currentSwipe.y && currentSwipe.y < 0.5f)
					{
						SetDestination(currentSwipe.x > 0 ? Vector3.right : Vector3.left);
					}
				}

				swipePosLastFrame = swipePosCurrentFrame;
			}

			if (Input.GetMouseButtonUp(0))
			{
				swipePosLastFrame = Vector2.zero;
				currentSwipe = Vector2.zero;
			}
		}

		if (canPlaySound)
		{
			audioSource.PlayOneShot(moveSound);
			canPlaySound = false;
		}
	}

	private void SetDestination(Vector3 direction)
	{
		movementVector = direction;

		// Check which object we will collide with
		if (Physics.Raycast(transform.position, direction, out var hit, 100.0f))
		{
			nextCollisionPosition = hit.point;
		}

		canPlaySound = false;
		isMoving = true;
	}

	public void Freeze()
	{
		fireworksParticle.Play();
		audioSource.PlayOneShot(moveSound);
		ballRigidBody.angularVelocity = Vector3.zero;
		ballRigidBody.velocity = Vector3.zero;
		isMoving = false;
		speed = 0.0f;
	}
}
