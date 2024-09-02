using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryEvent
{
	private int turn; //The turn that the event activates on

	public StoryEvent(int turn)
	{
		this.turn = turn;
	}

	public int getTurn()
	{
		return turn;
	}

}
