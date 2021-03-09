using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueManager : MonoBehaviour
{
    List<CarBehaviour> queue = new List<CarBehaviour>();

    public CarBehaviour Last()
    {
        CarBehaviour go = null;

        if (queue.Count > 0)
        {
            go= queue[queue.Count - 1];
        }
        return go;
    }

    public CarBehaviour Next()
    {
        CarBehaviour go = null;

        if (queue.Count >=2)
        {
            go=queue[queue.Count - 2];
        }
        
        return go;
    }
    public CarBehaviour First()
    {
        CarBehaviour go = null;

        if (queue.Count > 0)
        {
            go = queue[0];
        }
        return go;
    }
    public void Add(CarBehaviour CarBehaviour)
    {
        queue.Add(CarBehaviour);
    }
    public CarBehaviour PopFirst()
    {
        CarBehaviour go = null;
        if (queue.Count > 0)
        {
            go = queue[0];
            queue.RemoveAt(0);
        }
        return go;
    }
    public int Count()
    {   
        return queue.Count;
    }
  
}
