
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ScoreboardTurns : UdonSharpBehaviour
{
    int[] array = new int[] {1,2,3,4,5,};
    int[] emptyArray = new int[] {0,0,0,0,0,};
    
    void Start()
    {
    int maxElement = array[0];
    for (int index = 1; index < array.Length; index++)
    {
        if (array[index] > maxElement)
            maxElement = array[index];
    }
    Debug.Log(maxElement);
        //Debug.Log(array.Max());
        /*int upper = array.GetUpperBound(0);
        int lower = array.GetLowerBound(0);
        Debug.Log($"Array from {lower} to {upper}.");*/
    Debug.Log($"{emptyArray[0]}, {emptyArray[1]}, {emptyArray[2]}, {emptyArray[3]}, {emptyArray[4]}");
    array.CopyTo(emptyArray, 0);
    Debug.Log($"{emptyArray[0]}, {emptyArray[1]}, {emptyArray[2]}, {emptyArray[3]}, {emptyArray[4]}");
    }
}
