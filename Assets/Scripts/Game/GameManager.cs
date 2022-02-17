using Ilumisoft.SceneManagement;
using UnityEngine;
//EG REQUIRED
using enableGame;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour
{
    private IScoreSystem _scoreSystem;

    private IHighscoreSystem _highscoreSystem;

    private SceneManager _sceneManager;
    
    private EventManager _eventManager;

    private GameEvent _gameOverEvent;

    private void Awake()
    {
        _eventManager = FindObjectOfType<EventManager>();
        _sceneManager = FindObjectOfType<SceneManager>();

        _scoreSystem = InterfaceUtilities.FindObjectOfType<IScoreSystem>();
        _highscoreSystem = InterfaceUtilities.FindObjectOfType<IHighscoreSystem>();

        egAwake();
    }

    private void Start()
    {
        _gameOverEvent = _eventManager.GetEvent<GameOverEvent>();
        _gameOverEvent.AddListener(OnGameOver);

        _scoreSystem.ResetScore();

        Time.timeScale = 1.0f;
        egBeginSession();
    }

    private void Update()
    {
		egUpdate();
        //_scoreSystem.ModifyScore(5 * Time.deltaTime);
    }

    private void OnGameOver()
    {
		Debug.Log("Game Over");
        //GameOver should only be triggered once
        _gameOverEvent.RemoveListener(OnGameOver);

        //Stop game time
        Time.timeScale = 0.0f;

        //Update highscore
        if (_scoreSystem.Score > _highscoreSystem.Highscore)
        {
            _highscoreSystem.Highscore = _scoreSystem.Score;
        }

        egEndSession();

        //Load game over scene after 1 second
        _sceneManager.LoadSceneDelayed("Game Over", delay: 1.0f);
    }

    ///////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////////////////////////////////////////
	/// BEGIN ENABLEGAMES REQUIRED CODE
    public SkeletonData Skeleton;  		//holds the body data for the avatar
	public NetworkSkeleton netskeleton; //connects avatar to EAG launcher
	public Suki.SukiInput suki = null; //maps avatar body data to game input

	//egFloat,etc. are custom variables that can be attached to parameters in the settings menu and portal
	//They are attached to the parameters in the egAwake function below.
	egFloat Speed=1.0f;		//speed of player
	egFloat Gravity=-1.0f;	//falling cylinder's gravity (-1.0 is unity default)
	egInt GameLength=300; 	//in seconds

	// Use this for initialization
	void egAwake () {
		print ("egAwake");
		// initialize SUKI
		if (Skeleton == null)
			Skeleton = GameObject.Find ("Tracking Avatar").GetComponent<SkeletonData> ();
		if (netskeleton == null)
			netskeleton = GameObject.Find ("Tracking Avatar").GetComponent<NetworkSkeleton> ();
		suki = Suki.SukiInput.Instance;
		suki.Skeleton = Skeleton;
		print ("egAwake:Trying to connect...");

		// connect the client skeleton to the server skeleton (running in the enablegames launcher app)
		string address = PlayerPrefs.GetString(egParameterStrings.LAUNCHER_ADDRESS);
		print ("Address= " + address);
		NetworkClientConnect.Instance.Connect (address);
		print ("egAwake:after connect.");

		// Bind Speed to the variable "STARTING SPEED" from the settings menu
		//NOTE:Binding will be skipped if ParameterHandler not loaded (i.e. running this scene 
		//without first running MainMenu scene)
		//Also, parameters must be added to DefaultParameters.json file (located in StreamingAssets folder).
		VariableHandler.Instance.Register (ParameterStrings.STARTING_SPEED, Speed);
		VariableHandler.Instance.Register (ParameterStrings.GRAVITY, Gravity);
		VariableHandler.Instance.Register (egParameterStrings.GAME_LENGTH, GameLength);
		print ("Speed=" + Speed);
		print ("Gravity=" + Gravity);
		print ("GameLength=" + GameLength);
	}

	// Update is called once per frame
	void egUpdate () {
		// Return to main menu
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			OnGameOver();
		}
	}

	void egBeginSession()
	{
		Tracker.Instance.BeginTracking ();
	}

	void egEndSession()
	{
		Tracker.Instance.Interrupt((int)egEvent.Type.CustomEvent, "GameEnd");
		Tracker.Instance.StopTracking();
		NetworkClientConnect.Instance.Disconnect(); // this will disconnect form the avatar server! remember to disconnect each time you change the time scale or you change scene
	}

	float timeSinceLastLaneMove = 0f;
	/// <summary>
	/// Main game loop. Checks SUKI Input, updates game time, etc.
	/// </summary>
	// private void egGetSukiInput()
	// {
	// 	//print ("egGetSukiInput-------------------");
	// 	timeSinceLastLaneMove += Time.deltaTime;
		/*
			float duration = Time.time - startTime;
			if (duration >= GameLength)  //is game time over?
				showGameOverPanel ();
			timeSinceLastLaneMove += Time.deltaTime;
			*/

		//Get translated game input from SUKI
		// no-op if SUKI is not currently giving us input data

		/*NO LONGER NEED NETSKELETON TO KNOW IF CONNECTED..USES MOVEMENT FROM T-POSE INSTEAD (suki.Updating)
		//print("Game:FixedUpdate:" + suki.Updating);
		if (netskeleton && netskeleton.moving)
		{
			print ("netskel moving:" + suki.Skeleton.Moving);
			suki.Skeleton.moving = true;
			suki.Skeleton.resetMinMax = true;
		}
		*/
		
//		return;
		///
		/// Read the various Suki inputs (depending on what suki file was loaded)
		/// Below contains examples for different types of input, including joint angles, bone positions, etc.
		/// 
		/// 
		// read the placement range input and move the cube
		//elbow angle suki schema profile is set as "placement", but probably should use better name.
		//In X-Z movement, can be used for Z-movement together with "joystick" for X
		
		///
		/// END ENABLEGAMES REQUIRED CODE
		///////////////////////////////////////////////////////////////////////////////
}