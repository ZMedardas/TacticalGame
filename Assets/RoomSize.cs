using UnityEngine;
using System.Collections;

public class RoomSize : MonoBehaviour {

	public int height, width;

	public int[] getSize()
	{
		return getSize(0);
	}

	public int[] getSize(int rot)
	{
		int[] ret = new int[2];
		if (rot % 2 == 0)
		{
			ret[0] = height;
			ret[1] = width;
		}
		else
		{
			ret[0] = width;
			ret[1] = height;
		}
		return ret;
	}
}
