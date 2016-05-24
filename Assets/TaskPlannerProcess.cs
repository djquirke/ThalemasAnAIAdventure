using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Linq;

public class TaskPlannerProcess  
{
	private Process PProcessPlanner;
	public string workingDirectory = "";
	private string plannerFilename = "";
	private string PDDLdomFileName = ""; 
	private string PDDLprbFileName = ""; 
	private string solutionFilename = "";
	public string parsedSolution = "";
	public int actionsFound = 0;
	public List<string> actions;

	public List<string> GetListOfActions {
		get{ return actions;}
	}

	// generic instantiation and initialisation
	private void InstantiatePlanner()
	{
		PProcessPlanner = new Process();
		actions = new List<string> ();

		workingDirectory = Application.dataPath + @"/Planner";
		plannerFilename = workingDirectory + @"/metric-ff.exe";
		PDDLdomFileName = @"planner-domain"; 
		PDDLprbFileName = @"planner-problem"; 
		solutionFilename = workingDirectory + @"/ffSolution.soln";
	}

	private void ProcessStart()
	{
		PProcessPlanner.StartInfo.WorkingDirectory = workingDirectory;
		// metric-ff needs to run using the full explicit path ...
		PProcessPlanner.StartInfo.FileName = plannerFilename;
		PProcessPlanner.StartInfo.Arguments = string.Format("-o {0}.pddl -f {1}.pddl", PDDLdomFileName, PDDLprbFileName);
		PProcessPlanner.StartInfo.CreateNoWindow = true;
		PProcessPlanner.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
		// run the process, and wait until it has closed
		PProcessPlanner.Start();
		PProcessPlanner.WaitForExit();
	}
	
	public void Start () {

		InstantiatePlanner();
		//generate problem file
		ProcessStart();
		if(File.Exists(solutionFilename))
		{
			UnityEngine.Debug.Log ("TASK SOLUTION FOUND");

			// using Linq, extract all the lines containing ':', 
			// ie all the lines specifying the actions in the solution generated
			var result = File.ReadAllLines (solutionFilename).Where(s => s.Contains(":"));
			actionsFound = result.Count();

			// show the actions parsed in the editor window
			for (int i = 0; i < actionsFound; i++) {
				parsedSolution = result.ToList ()[i].ToString ();
				actions.Add(parsedSolution);
				//UnityEngine.Debug.Log (parsedSolution);
			}

			// delete the solution file, so you don't get to read it again next time you generate a new solution
			File.Delete (solutionFilename);
			UnityEngine.Debug.Log ("SOLUTION FILE DELETED");
		}
		else
		{
			UnityEngine.Debug.Log("NO SOLUTION FILE CREATED");
		}
	}

	void Update () {
	
	}
}
