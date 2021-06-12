using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerEventOnListOfTasks : MonoBehaviour
{
    [SerializeField] List<Task> tasksToBeDone = new List<Task>();
    [SerializeField] UnityEvent changesAfterCompleted = new UnityEvent();


    public void TaskFinished(Task _task)
    {
        if (tasksToBeDone.Contains(_task))
        {
            tasksToBeDone.Remove(_task);
            checkForCompletion();
        }
    }

    private void checkForCompletion()
    {
        if(tasksToBeDone.Count == 0)
        {
            changesAfterCompleted.Invoke();
        }
    }
}
