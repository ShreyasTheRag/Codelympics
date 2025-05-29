using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AdvanceToNextLevel : MonoBehaviour
{
	[SerializeField] private GameObject player;
	[SerializeField] private GameObject canvas;
	private Interpreter interpreter;
	private bool running;
	private string toDisplayInNextLevel;
	private int currentLevel;

	// Start is called before the first frame update
	void Start()
	{
		interpreter = player.GetComponent<Interpreter>();
		running = true;
		toDisplayInNextLevel = "";
		// currentLevel = File.Exists("hurdlessave.txt") ? int.Parse(Regex.Split(File.ReadAllText("hurdlessave.txt"), "[$]")[0]) : 1;
		currentLevel = canvas.GetComponent<ProgrammingCanvas>().levelNumber;
		Debug.Log("Now in scene: " + currentLevel);
	}

	// Update is called once per frame
	void Update()
	{
		if (interpreter.DistToFinish() <= 0 && running)
		{
			running = false;
			Debug.Log("You crossed the finish line!");
			PrepareHelp();
			// write into file number of next scene
			++currentLevel;
			Debug.Log("Next level is " + currentLevel);
			File.WriteAllText("hurdlessave.txt", currentLevel + "$" + toDisplayInNextLevel);

			SceneManager.LoadScene("Hurdles " + (currentLevel != 8 ? currentLevel : "Post Game Survey"));
		}
	}


	private void AddToText(string text)
	{
		toDisplayInNextLevel += "// " + text + Environment.NewLine;
	}

	private void PrepareHelp()
	{
		AddToText("Commands");
		AddToText("step: Step forward");
		AddToText("jump: Jump and wait until the player touches the ground");
		AddToText("if <condition> ... end: Execute the statements until the \"end\" statement if the given condition is true");
		AddToText("while <condition> ... end: Repeatedly execute the statements until the \"end\" statement while the given condition is true");
		toDisplayInNextLevel += Environment.NewLine;
		AddToText("Operators");
		AddToText("Addition: +");
		AddToText("Subtraction: -");
		AddToText("Multiplication: *");
		AddToText("Division: /");
		AddToText("Greater than: > (>= if greater than or equal to)");
		AddToText("Less than: < (<= if less than or equal to)");
		AddToText("Equal: ==");
		AddToText("Not equal: !=");
		AddToText("Assignment: =");
		toDisplayInNextLevel += Environment.NewLine;
		AddToText("You can declare a variable by writing <name of variable> = <value you want it to store>");
		AddToText("A value can be a variable, an expression consisting of the +, -, *, or / operators, or a number like 0.5.");
		AddToText("To get the distance to the next hurdle, use dist(hurdle).");
		AddToText("To get the distance to the finish line, use dist(finish).");
		AddToText("You may delete this text.");
	}
}