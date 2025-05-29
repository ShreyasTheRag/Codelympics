using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Interpreter : MonoBehaviour
{
	[SerializeField] private float stepForce;        // Movement speed
	[SerializeField] private float maxSpeed;
	[SerializeField] private float jumpPower;    // Jump force
	[SerializeField] public float reloadDelay;
	private Rigidbody rb;                       // Reference to the Rigidbody
	private bool grounded;
	private ProgrammingCanvas canvasScript;
	private ForceMode fm;
	private int programCounter;
	private bool collidedWithHurdle;
	private List<float> hurdlePositions; // finish line is hurdle n - 1
	private int nearestHurdle;
	private float finishLinePosition;
	private float distanceToNextHurdle;
	private Dictionary<string, float> variables;
	private bool jumping;
	public List<string[]> commandQueue;
	public KeyCode cancelKey;
	public bool running;

	void Start()
	{
		rb = GetComponent<Rigidbody>();
		rb.isKinematic = false;
		grounded = false;
		jumping = false;
		canvasScript = GameObject.Find("Programming Canvas").GetComponent<ProgrammingCanvas>();
		fm = ForceMode.Impulse;
		commandQueue = new List<string[]>();
		programCounter = 0;
		collidedWithHurdle = false;
		hurdlePositions = new List<float>();
		nearestHurdle = 0;
		Transform hurdleListObjectTransform = GameObject.Find("Hurdle List").transform;
		foreach (Transform t in hurdleListObjectTransform) hurdlePositions.Add(t.position.x);
		finishLinePosition = GameObject.Find("Finish Line").transform.position.x;
		variables = new Dictionary<string, float>();
		cancelKey = KeyCode.C;
		running = false;
	}

	void Update()
	{
		if (!canvasScript.IsShowing() && programCounter < commandQueue.Count)
		{
			//Debug.Log($"Program counter: {programCounter} Line: {commandQueue[programCounter][0]} {((commandQueue[programCounter].Length > 1) ? commandQueue[programCounter][1] : string.Empty)}");
			updateDistanceToNextHurdle();
			if (nextCommandIs("step"))
			{
				if (grounded) {
					stepForward();
					++programCounter;
				}
			}
			else if (nextCommandIs("jump"))
			{
				//Debug.Log("jump");
				if (grounded) {
					if (jumping)
					{
						++programCounter;
						jumping = false;
					}
					else
					{
						jumping = true;
						jump();
					}
				}
			}
			else if (nextCommandIs("if") || nextCommandIs("while"))
			{
				bool isIfStatement = nextCommandIs("if");
				string[] args = commandQueue[programCounter][1].Split(' ');
				float[] floatArgs = new float[args.Length];
				foreach (int i in new int[] { 0, 2 })
				{
					floatArgs[i] = getFloat(args[i]);
				}
				bool eq = floatArgs[0] == floatArgs[2]; // equalIgnoreCase(args[1], "==")
				bool ne = floatArgs[0] != floatArgs[2]; // equalIgnoreCase(args[1], "!=")
				bool gt = floatArgs[0] > floatArgs[2];
				bool lt = floatArgs[0] < floatArgs[2];
				bool ge = (gt || eq) && equalIgnoreCase(args[1], ">=");
				bool le = (lt || eq) && equalIgnoreCase(args[1], "<=");
				eq = eq && equalIgnoreCase(args[1], "==");
				ne = ne && equalIgnoreCase(args[1], "!=");
				gt = gt && equalIgnoreCase(args[1], ">");
				lt = lt && equalIgnoreCase(args[1], "<");
				if (eq || ne || gt || lt || ge || le) {
					//Debug.Log("True");
					++programCounter;
				} else {
					//Debug.Log("False");
					int nest = 0;
					for (++programCounter; nest != 0 || !nextCommandIs("end"); ++programCounter)
					{
						if (nextCommandIsControlFlow())
						{
							++nest;
						}
						else if (nextCommandIs("end"))
						{
							--nest;
						}
					}
					++programCounter;
				}
			}
			else if (nextCommandIs("end"))
			{
				int tempPC = programCounter;
				int nest = 0;
				bool isControlFlow = nextCommandIsControlFlow();
				for (--programCounter; nest != 0 || !isControlFlow; --programCounter)
				{
					if (isControlFlow)
					{
						--nest;
					}
					else if (nextCommandIs("end"))
					{
						++nest;
					}
					isControlFlow = nextCommandIsControlFlow();
				}
				programCounter++;
				if (nextCommandIs("if"))
				{
					programCounter = tempPC + 1;
				}
			}
			else {
				if (!variables.ContainsKey(commandQueue[programCounter][0])) {
					variables.Add(commandQueue[programCounter][0], 0.0f);
				}
				string[] args = commandQueue[programCounter][1].Split(' ', 2);
				string[] splitExpr = args[1].Split(' ');
				float intermediate = getFloat(splitExpr[0]);
				if (splitExpr.Length > 1) {
					if (splitExpr[1] == "+")
					{
						intermediate += getFloat(splitExpr[2]);
					}
					else if (splitExpr[1] == "-")
					{
						intermediate -= getFloat(splitExpr[2]);
					}
					else if (splitExpr[1] == "*")
					{
						intermediate *= getFloat(splitExpr[2]);
					}
					else if (splitExpr[1] == "/") {
						intermediate /= getFloat(splitExpr[2]);
					}
				}
				if (args[0] == "=")
				{
					setVariable(commandQueue[programCounter][0], intermediate);
				}
				else if (args[0] == "+=") {
					setVariable(commandQueue[programCounter][0], getVariable(commandQueue[programCounter][0]) + intermediate);
				}
				else if (args[0] == "-=")
				{
					setVariable(commandQueue[programCounter][0], getVariable(commandQueue[programCounter][0]) - intermediate);
				}
				else if (args[0] == "*=")
				{
					setVariable(commandQueue[programCounter][0], getVariable(commandQueue[programCounter][0]) * intermediate);
				}
				else if (args[0] == "/=")
				{
					setVariable(commandQueue[programCounter][0], getVariable(commandQueue[programCounter][0]) / intermediate);
				}
				++programCounter;
			}
		}
		if ((!canvasScript.IsShowing() && running) && programCounter >= commandQueue.Count && DistToFinish() > 0) Invoke("reloadScene", reloadDelay);
	}

	private float getFloat(string arg) {
		try
		{
			return float.Parse(arg);
		}
		catch (FormatException)
		{
			if (equalIgnoreCase(arg, "dist(hurdle)"))
			{
				return distanceToNextHurdle;
			}
			else if (equalIgnoreCase(arg, "dist(finish)"))
			{
				return DistToFinish();
			}
			else
			{
				return getVariable(arg);
			}
		}
	}

	private float getVariable(string name) {
		return variables[name];
	}

	private void setVariable(string name, float value) {
		variables[name] = value;
	}

	public float DistToFinish() {
		return finishLinePosition - transform.position.x;
	}

	private void updateDistanceToNextHurdle() {
		//Debug.Log($"dist(finish) = {DistToFinish()} dist(hurdle) = {distanceToNextHurdle} player position = {transform.position.x} hurdlePositions = (Nearest) {string.Join(',', hurdlePositions.GetRange(nearestHurdle, hurdlePositions.Count - nearestHurdle))} (Furthest) grounded = {grounded}");
		int i = hurdlePositions.Count - 1;
		if (distanceToNextHurdle < 0) ++nearestHurdle;
		try
		{
			distanceToNextHurdle = hurdlePositions[nearestHurdle] - transform.position.x;
		}
		catch (ArgumentOutOfRangeException) {
			distanceToNextHurdle = 0;
		}
		//distanceToNextHurdle = hurdlePositions.Count == 0 ? 0.0f : (hurdlePositions[i - 1].x - transform.position.x);
		if (Input.GetKeyDown(cancelKey)) {
			reloadScene();
		}
	}

	private bool equalIgnoreCase(string a, string b) {
		return string.Compare(a, b, StringComparison.OrdinalIgnoreCase) == 0;
	}

	private bool nextCommandIs(string c) {
		return equalIgnoreCase(commandQueue[programCounter][0], c);
	}

	private bool nextCommandIsControlFlow() {
		return nextCommandIs("if") || nextCommandIs("while");
	}

	public void stepForward() {
		if (!collidedWithHurdle)
		{
			rb.AddForce(Vector3.right * stepForce, fm);
			rb.velocity = new Vector3(Math.Min(rb.velocity.x, maxSpeed), rb.velocity.y, rb.velocity.z);
		}
	}

	public void jump() {
		if (grounded)
		{
			grounded = false;
			rb.constraints &= ~RigidbodyConstraints.FreezePositionY;
			rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.collider.gameObject.name == "Track")
		{
			grounded = true;
			rb.constraints |= RigidbodyConstraints.FreezePositionY;
		}
		if (collision.collider.gameObject.name.Contains("Hurdle")) {
			collidedWithHurdle = true;
			rb.constraints |= RigidbodyConstraints.FreezePositionX;
			Invoke("reloadScene", reloadDelay);
		}
	}
	private void OnCollisionExit(Collision collision) {
		if (collision.collider.gameObject.name == "Track") grounded = false;
	}

	public void reloadScene()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}
}
