using System;
using System.IO;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ProgrammingCanvas : MonoBehaviour
{
	[SerializeField] public TMP_InputField inputField;
	[SerializeField] private Button runButton, exitButton, inspectButton;
	[SerializeField] private TextMeshProUGUI codingMenuInstruction;
	[SerializeField] public int levelNumber;
	private bool showing;
	private Interpreter interpreter;
	private KeyCode exitInspectKey;
	// Start is called before the first frame update
	void Start()
	{
		// populate the textbox
		if (File.Exists("hurdlessave.txt"))
		{
			string[] saveData = Regex.Split(File.ReadAllText("hurdlessave.txt"), "[$]");
			if (int.Parse(saveData[0]) == levelNumber && saveData[1].Length > 0)
			{
				inputField.text = saveData[1];
			}
			else {
				ShowHelp();
			}
		}
		else {
			ShowHelp();
		}
		exitInspectKey = KeyCode.Escape;
		showing = false;
		interpreter = GameObject.Find("Athlete").GetComponent<Interpreter>();
		codingMenuInstruction.text = "Press " + interpreter.cancelKey + " to terminate your program and " + (exitInspectKey == KeyCode.Escape ? "Esc" : exitInspectKey.ToString()) + " to exit level inspection mode";
		runButton.onClick.RemoveAllListeners();
		runButton.onClick.AddListener(RunProgram);
		exitButton.onClick.RemoveAllListeners();
		exitButton.onClick.AddListener(SaveAndExit);
		inspectButton.onClick.RemoveAllListeners();
		inspectButton.onClick.AddListener(ToggleAppearance);
		ToggleAppearance();
	}

	// Update is called once per frame
	void Update()
	{
		if (!(showing || interpreter.running) && Input.GetKeyDown(exitInspectKey)) ToggleAppearance();
	}
	public void ToggleAppearance() {
		showing = !showing;
		transform.GetChild(0).gameObject.SetActive(showing);
	}
	public bool IsShowing() {
		return showing;
	}
	void RunProgram()
	{
		SaveProgram();
		string[] lines = Regex.Split(inputField.text, @"[\n\r]+");
		foreach (string l in lines)
		{
			int commentIndex = l.IndexOf("//");
			string line = commentIndex >= 0 ? l.Substring(0, commentIndex) : l; // Introduce comment handling
			if (line.Length > 0) interpreter.commandQueue.Add(line.Trim().Split(' ', 2));
		}
		Debug.Log(interpreter.commandQueue.Count);
		if (interpreter.commandQueue.Count > 0)
		{
			interpreter.running = true;
			ToggleAppearance();
		}
	}

	// called when advancing to next level, or exiting hurdles
	// completely overwrites the file with new level completed 
	public void SaveProgram() {
		int currentLevel = File.Exists("hurdlessave.txt") ? int.Parse(Regex.Split(File.ReadAllText("hurdlessave.txt"), "[$]")[0]) : 1;
		File.WriteAllText("hurdlessave.txt", currentLevel + "$" + inputField.text);
	}

	void SaveAndExit()
	{
		SaveProgram();
		SceneManager.LoadScene("SelectMinigame");
	}

	private void AddToText(string text) {
		inputField.text += "// " + text + Environment.NewLine;
	}

	private void ShowHelp() {
		AddToText("Commands");
		AddToText("step: Step forward");
		AddToText("jump: Jump and wait until the player touches the ground");
		AddToText("if <condition> ... end: Execute the statements until the \"end\" statement if the given condition is true");
		AddToText("while <condition> ... end: Repeatedly execute the statements until the \"end\" statement while the given condition is true");
		inputField.text += Environment.NewLine;
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
		inputField.text += Environment.NewLine;
		AddToText("You can declare a variable by writing <name of variable> = <value you want it to store>");
		AddToText("A value can be a variable, an expression consisting of the +, -, *, or / operators, or a number like 0.5.");
		AddToText("To get the distance to the next hurdle, use dist(hurdle).");
		AddToText("To get the distance to the finish line, use dist(finish).");
		AddToText("You may delete this text.");
	}
}