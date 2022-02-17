using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float verticalSpeed = 15.0f;
    [SerializeField] private float horizontalSpeed = 10.0f;
    
    private IHealthSystem _healthSystem;
    private IHorizontalInputProvider _horizontalInputProvider;
    private Rigidbody _rigidbody;
    private EventManager _eventManager;
    private GameManager _gameManager;
    
    private void Awake()
    {
        _eventManager = FindObjectOfType<EventManager>();
        
        _rigidbody = GetComponent<Rigidbody>();

        _healthSystem = GetComponent<IHealthSystem>();

        _horizontalInputProvider = new HorizontalInputProvider();

        _gameManager = FindObjectOfType<GameManager>();

        suki = _gameManager.suki;
    }

    private void Start()
    {
        _rigidbody.velocity = new Vector3(0, 0, -verticalSpeed);

        if (_healthSystem != null)
        {
            _healthSystem.OnHealthEmpty.AddListener(Die);
        }
    }

    private void FixedUpdate()
    {
        ProcessInput();
        ProcessSukiInput();
    }

    private void ProcessInput()
    {
        var horizontalInput = _horizontalInputProvider.GetHorizontalInput();
        
        var movement = new Vector3(horizontalInput, 0,0);
        
        _rigidbody.AddForce(movement*horizontalSpeed);
    }


    private Suki.SukiInput suki = null; //maps avatar body data to game input

    private void ProcessSukiInput()
    {

        if (!suki.Updating)
		{
				print("Game:suki not updating.");
			return;
		}


        if (suki.RangeExists("placement")) {
			// we can use a range value as a placement to move left and right
			float range = suki.GetRange("placement");
			// convert 0f to 1f to -1f to 1f
			float xPercent = (range * 2) - 1f;
			//print("placement mode:" + range + ":" + xPercent);
			// add a deadzone of +/- %
			float deadzone = 0.2f;
			if (xPercent > -deadzone && xPercent < deadzone){
				xPercent = 0f;
			}
			// move the object
            ApplyForceFromInput(xPercent);
		}
		//shoulder profile is set as "joystick"
		//In X-Z movement, can be used for X-movement togther with "placement" for Z.
		if (suki.RangeExists("joystick")) {
			// we can use a range value as a placement to move left and right
			float range = suki.GetRange("joystick");
			// convert 0f to 1f to -1f to 1f
			float xPercent = (range * 2) - 1f;
			//print("joysick mode:" + range + ":" + xPercent);
			// move the object
			float deadzone = 0.2f;
			if (xPercent > -deadzone && xPercent < deadzone) {
				xPercent = 0f;
			}

			ApplyForceFromInput(xPercent);
		}

		//moving in discrete steps/lanes
		if (suki.SignalExists("moveLeft") && suki.SignalExists("moveRight")) {
			// we can use a pair of triggers to move left or move right
			bool moveLeft = suki.GetSignal("moveLeft");
			bool moveRight = suki.GetSignal("moveRight");

			// Vector3 pos = PlayerObject.transform.localPosition;

			// only if there is a direction to move, and it's been some time since our last move
			// Instead of changing the speed of the movement here we change the pause between movements
			if ((!moveLeft && !moveRight) || (moveLeft && moveRight) ) // we use speed as a time scaler
			{
				return;
			}
			else if (moveLeft)
			{
				ApplyForceFromInput(-1f);
			}
			else if (moveRight)
			{
				ApplyForceFromInput(-1f);
			}
			// PlayerObject.transform.localPosition = pos; //REPLACE PlayerObject with whatever object or vector you want to be updated
			// timeSinceLastLaneMove = 0f;

		}
		//using foot or hand x-y position to control player position
		//You could also use each independently as Kollect does to control the hand/footprints.
		if (suki.Location2DExists ("leftfoot") || suki.Location2DExists ("rightfoot") || suki.Location2DExists ("lefthand") || suki.Location2DExists ("righthand")) {
			Vector2 fpos;
			if (suki.Location2DExists ("leftfoot"))
				fpos = suki.GetLocation2D ("leftfoot");
			else if (suki.Location2DExists ("rightfoot"))
				fpos = suki.GetLocation2D ("rightfoot");
			else if (suki.Location2DExists ("lefthand"))
				fpos = suki.GetLocation2D ("lefthand");
			else if (suki.Location2DExists ("righthand"))
				fpos = suki.GetLocation2D ("righthand");
			else
				fpos = new Vector2 ();
			// Vector3 pos = PlayerObject.transform.localPosition; //REPLACE PlayerObject with whatever object or vector you want to be updated
			// convert 0f to 1f to -1f to 1f
			float xPercent = (fpos.x * 2) - 1f;
            ApplyForceFromInput(xPercent);


			// float yPercent = (fpos.y * 2) - 1f;
			// float weight = 10f;
			// pos.x = (pos.x * (weight-1) + (xPercent))/weight; // we use speed as position scaler
			// pos.y = (pos.y * (weight-1) + (yPercent))/weight; // we use speed as position scaler
			// //pos.x = pos.x + (fpos.x * Speed/40); // we use speed as position scaler
			//PlayerObject.transform.position = Vector3.Lerp(LeftFoot.transform.position, new Vector3(newX, newY, newZ), 1f);
			// PlayerObject.transform.localPosition = pos;
		}
		// checkRange ();
	}
	// void checkRange() {
	// 	float maxX=4f, maxY= 3f;
	// 	Vector3 pos = PlayerObject.transform.localPosition;  //REPLACE PlayerObject with whatever object or vector you want to be updated
	// 	if (pos.x> maxX)
	// 		pos.x=maxX;
	// 	if (pos.x< -maxX)
	// 		pos.x= -maxX;
	// 	if (pos.y> maxY)
	// 		pos.y=maxY;
	// 	if (pos.y< -maxY)
	// 		pos.y= -maxY;
	// 	PlayerObject.transform.localPosition = pos;

	
    // }

    private void ApplyForceFromInput(float inp) {
        Debug.Log("ApplyForceFromInput: " + inp);
        Vector3 movement = new Vector3(inp, 0, 0);
        _rigidbody.AddForce(movement*horizontalSpeed);
    }
    
    private void Die() {
        //Invoke Game Over
        _eventManager.GetEvent<GameOverEvent>().Invoke();


    }

    
}