using System;
using UnityEngine;

public class Dialogue : MonoBehaviour
{
    [Serializable]
    public struct Status
    {
        public string nameDestination;
        public int no;
    }

    public Status status;
    public bool isReadingCompleted;
}
