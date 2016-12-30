using UnityEngine;
using System.Collections;

public class GamePool
{
	public GameObject[] pools;
	public int curSize;
	public int maxSize;
	
	public GamePool(int size)
	{
		curSize = 0;
		maxSize = size;
		pools = new GameObject[maxSize];
	}
	
	public void Push(GameObject obj)
	{
		if(maxSize <= 0 || pools == null)
			return;
			
		if(curSize>=maxSize)
			return;
			
		pools[curSize] = obj;
		//pools[curSize].name = pools[curSize].name+ "-" + curSize;
		curSize++;
		//PrintPool();
	}

	public GameObject Pop()
	{
		if(isEmpty())
			return null;
		GameObject obj = pools[0];
		for(int i = 0; i < curSize-1; i++)
		{
			pools[i] = pools[i+1];
		}
		pools[--curSize] = null;
		//PrintPool();
		return obj;
	}
	public bool isFull()
	{
		if(curSize>=maxSize)
			return true;
		return false;
	}	
	public bool isEmpty()
	{
		return (curSize <= 0 || pools == null);
	}
	
	public void inactiveAllPoolObj()
	{
        for (int i = 0; i < pools.Length;i++ )
        {
            if (pools[i] != null)
                pools[i].SetActive(false);
        }
	}

	public void destroyAllPoolObj()
	{
        for (int i = 0; i < pools.Length; i++)
        {
			if(pools[i]!=null)
				MonoBehaviour.Destroy(pools[i]);
		}
	}

	public int GetNumberOfFreeObj()
	{
		int count = 0;
		for (int i = 0; i < pools.Length; i++)
		{
			if(pools[i]!=null)
			{
				if(!pools[i].activeInHierarchy)
				{
					count++;
				}
			}
		}
		return count;
	}
	
	//public void PrintPool()
	//{
	////	Utils.Log("*** PRINT POOL ***");
	//	if(isEmpty())
	//		Utils.Log("Pool empty!");
	//	else{
 //           for (int i = 0; i < pools.Length; i++)
	//		{
	//			if(pools[i]!=null)
	//				Utils.Log("-- " + pools[i].name);
	//		}
	//	}
	//}
}

public class PoolManager {

	private GamePool[] pools;
	
	public PoolManager(int poolLength)
	{
		initPools(poolLength);
	}
	
	public int getLength()
	{
		if(pools == null)
			return -1;
		return pools.Length;
	}
	
	public void initPools(int poolLength)
	{
		pools = new GamePool[poolLength];
	}
	
	public void initPoolAtIndex(int idx, int lengthAtIdx)
	{
		pools[idx] = new GamePool(lengthAtIdx);
	}
	
	public void checkPoolAtIndex(int idx, int lengthAtIdx)
	{
		if(pools[idx] == null)
			initPoolAtIndex(idx, lengthAtIdx);
	}
	
	public void pushToPool(GameObject obj, int idx)
	{
		if(idx < 0 || idx >= pools.Length)
			return;
		
		pools[idx].Push(obj);
	}
	
	public void pushToPools(GameObject obj, int idx, int lengthAtIdx)
	{
		if(idx < 0 || idx >= pools.Length)
			return;
		checkPoolAtIndex(idx, lengthAtIdx);
		pools[idx].Push(obj);
	}
	
	public GameObject popFromPool(int idx)
	{
		if(idx < 0 || idx >= pools.Length || pools[idx] == null)
			return null;
		return pools[idx].Pop();
	}
	
	public bool isPoolFull(int idx)
	{
		if(idx < 0 || idx >= pools.Length || pools[idx] == null)
			return false;
		return pools[idx].isFull();
	}

    public bool isEmpty(int idx)
    {
        if (idx < 0 || idx >= pools.Length || pools[idx] == null)
            return true;
        return pools[idx].isEmpty();
    }
	
	public void InactiveAllObj(int idx)
	{
		if(idx < 0 || idx >= pools.Length || pools[idx] == null)
			return;
		pools[idx].inactiveAllPoolObj();
	}

	public void DestroyAllObj(int idx)
	{
		if(idx < 0 || idx >= pools.Length || pools[idx] == null)
			return;
		pools[idx].destroyAllPoolObj();
	}

	public int GetNumberOfFreeObj(int idx)
	{
		if(idx < 0 || idx >= pools.Length || pools[idx] == null)
			return 0;
		return pools[idx].GetNumberOfFreeObj();
	}
}
