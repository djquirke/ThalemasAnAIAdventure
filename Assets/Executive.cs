using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

interface IAction
{
    Tile Position
    {
        get;
    }

    //Agent will call Action every turn once they're on Position
    //Action will return true when the agent can return to idling
    bool Action(Person P);
}

class PatrolAction : IAction
{
    public Tile Position
    {
        get;
        private set;
    }

    public Tile PatrolStart
    {
        get;
        private set;
    }

    public Tile PatrolEnd
    {
        get;
        private set;
    }

    public PatrolAction(Tile Start, Tile End)
    {
        Position = Start;

        PatrolStart = Start;
        PatrolEnd = End;
    }

    public bool Action(Person P)
    {
        if(Position.Equals(PatrolStart))
        {
            Position = PatrolEnd;
        }
        else
        {
            Position = PatrolStart;
        }

        //Executive must explicitly tell this agent to stop patrolling by setting its action to null or something else
        return false;
    }
}

class MiningAction : IAction
{
    public Tile Position
    {
        get;
        private set;
    }

    public Building Mine
    {
        get;
        private set;
    }

    public int AmountToMine
    {
        get;
        private set;
    }

    private int TicksSinceStart = 0;

    public MiningAction(Building Mine, int AmountToMine)
    {
        this.Position = GameManager.tiles[Mine.Position];
        this.Mine = Mine;
        this.AmountToMine = AmountToMine;
    }

    public bool Action(Person P)
    {
        bool Done = false;

        if(Position.Entities.Find(X => Object.ReferenceEquals(P, X)) != null)
        {
            ++TicksSinceStart;
            if(TicksSinceStart >= Mine.BuildingProduction.TimeToProduce)
            {
                TicksSinceStart = 0;
               if(Mine.Produce())
               {
                   --AmountToMine;
                   if(AmountToMine <= 0)
                   {
                       Done = true;
                   }
               }
            }
        }

        return Done;
    }
}

struct Action
{
	public string name;
	public Vector2 position;
	public bool running;

	public Action(string name, Vector2 pos)
	{
		this.name = name;
		position = pos;
		running = false;
	}
}


public class Executive : MonoBehaviour {

	//Store a queue of actions
	Queue<Action> actions;
	Action current;
	Dictionary<Vector2, Tile> world_state;

    Queue<Action> QueuedActions = new Queue<Action>();

	public void Initialise()
	{
		actions = new Queue<Action>();
        
		world_state = GameManager.WorldState;

        

		//TODO: replace with asking task planner to generate plan to build a barracks
		//move to stone
		//gather stone
		//move to forest
		//gather wood
		//educate carpenter
		//build barracks
		//train riflemen
		//move to opposing teacher
		//attack opposing teacher
	}

	public void GameTick()
	{
		if(actions.Count == 0 && !current.running)
		{
			//generate new plan
		}
		if(!current.running)
		{
			Action next = (Action)actions.Peek();
			//TODO: check with task planner that action is still valid
			//if not, generate steps to complete current task and add to new queue
			//then copy the actions queue into the new queue
			//if it is still valid, perform the action necessary
			current = (Action)actions.Dequeue();
			PerformAction();
		}
		else
		{
			//increment actions tick count
		}

	}

	private void PerformAction()
	{
		world_state = GameManager.WorldState;
		//TODO: This entire switch statement can be replaced by a couple of lines
		//if scriptable objects were used for the actions


		switch (current.name.ToLower()) {
		case "mine":
			//decrement resource from mine
			//increment player resource or cart
			break;
		case "build":
			//spawn building site
			break;
		case "make":
			//start making process
			//set blacksmith to busy
			break;
		case "train": //include educate too
			//start education process
			//set relevant people to busy
			//assign end result
			break;
		case "family":
			//set people to busy
			//assign timeframe
			//assign end result
			break;
		case "store":
			//decrement resource from source
			//increment storage
			break;
		case "smelt":
			//start smelting process
			//set labourer to busy
			break;
		case "saw":
			//start saw process
			//set labourer to busy
			break;
		case "quarry":
			//start quarry process
			//set labourer to busy
			break;
		case "cut tree":
			//start foresting process
			//set lumberjack to busy
			break;
		case "trade":
			//increment opposing player with goods
			//increment yourself with goods
			//decrement yourself of goods traded to opponent
			//decrement opponent of goods traded to yourself
			break;
		case "move":
			//ask motion planner to start moving
			break;
		default:
			break;
		}
	}
}
