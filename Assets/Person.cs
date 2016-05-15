﻿using UnityEngine;
using System.Collections;

public enum PersonType 
{
	LABOURER,
	TEACHER, 
	LUMBERJACK, 
	MINER, 
	BLACKSMITH,
	CARPENTER,
	TRADER, 
	RIFLEMAN
};

public enum ToolType
{
	AXE,
	CART, 
	RIFFLE,
	NONE
}

public class Person : IStorage
{
	private bool alive;
	private PersonType type;
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
		type = PersonType.LABOURER;
		tool = ToolType.NONE;
		storage = new Storage (1, 1);
	}

	public PersonType Type 
	{ 
		get { return type; }
		set { type = value; }
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
		if ( type == PersonType.RIFLEMAN ) 
		{
		  	if( other.Type != PersonType.RIFLEMAN ){
				//100% chance of killing other person
				other.Alive = false;
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
}
