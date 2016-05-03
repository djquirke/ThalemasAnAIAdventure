using UnityEngine;
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

public class Person 
{
	private bool alive;
	private PersonType type;

	public Person()
	{
		alive = true;
		type = PersonType.LABOURER;
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
}
