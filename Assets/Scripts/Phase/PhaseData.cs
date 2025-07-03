using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Phase_Data", menuName = "My Scriptable Objects/PhaseData")]
public class PhaseData : ScriptableObject
{
    public string phaseName;
    public List<Enemy> enemies;
    public int[] spawnTime;
}
