using UnityEngine;
using System.Collections;

[System.Flags]
public enum PersonType 
{
	LABOURER    = 1,
	TEACHER     = 2, 
	LUMBERJACK  = 4, 
	MINER       = 8, 
	BLACKSMITH  = 16,
	CARPENTER   = 32,
	TRADER      = 64, 
	RIFLEMAN    = 128,
};

public enum ToolType
{
    NONE,
	AXE,
	CART, 
	RIFFLE,
}

public class Person : Entity, IStorage
{
	private bool alive;
	private PersonType personType;
	private ToolType tool;
	private Storage storage;
	private bool has_cart;
	private int money;

	
	public Storage Store
	{
		get { return storage; }
	}

	public Person()
	{
		alive = true;
		personType = PersonType.LABOURER;
		tool = ToolType.NONE;
		storage = new Storage (1, 1);
		entityType = e_EntityType.PERSON;
	}

	public PersonType Type 
	{ 
		get { return personType; }
		set { personType = value; }
	}

	public bool Alive 
	{ 
		get { return alive;} 
		private set { alive = value;}
	}

	public ToolType Tool 
	{
		get { return tool;}
		set { tool = value;}
	}

	public Storage PersonStore 
	{
		get { return storage;}
	}

	public bool PersonHasCart 
	{
		get { return has_cart;}
		set 
		{ 
			has_cart = value;
			UpdateStorageSize();
		}
	}

	public int Money 
	{
		get { return money;}
	}
		
	public void Attack(Person other)
	{
		if ( personType == PersonType.RIFLEMAN ) 
		{
		  	if( other.Type != PersonType.RIFLEMAN ){
				//100% chance of killing other person
				other.Alive = false;
				//TODO: remove dead person from scene
			}
			else
			{
				//70% chance of killing, 30% chance of getting killed
				int number = Random.Range (1, 10);
				if(number >= 1 && number <= 7)
				{
					other.Alive = false;
				}
				else
				{
					alive = false;
				}
			}
		}
	}

	public void UpdateMoney(int amount)
	{
		money += amount;
	}

	private void UpdateStorageSize()
	{
		storage = has_cart ? new Storage(5, 5) : new Storage(1, 1);
	}

	//TODO: add gametick functionality here
	public override void GameTick ()
	{
		base.GameTick ();
	}

	//2 people are equal if they are the same type
	public override bool Equals (object obj)
	{
		Person other = obj as Person;
		return other.Type == this.Type;
	}
}
