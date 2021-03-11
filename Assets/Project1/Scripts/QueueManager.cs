using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueManager : MonoBehaviour
{
    List<CarBehaviour> queue = new List<CarBehaviour>();

    public CarBehaviour Last() => queue.Count > 0 ? queue[queue.Count - 1] : null;
    public CarBehaviour Next()
    {
        CarBehaviour go = null;

        if (queue.Count >=2)
        {
            go=queue[queue.Count - 2];
        }
        
        return go;
    }
    public CarBehaviour First() => queue.Count > 0 ? queue[0] : null;
    public int Add(CarBehaviour CarBehaviour)
    {
        int index = queue.Count;
        queue.Add(CarBehaviour);
        return index;
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
    public CarBehaviour this[int index]
    {
        get => queue[index];
        set => queue[index] = value;
    }

}
