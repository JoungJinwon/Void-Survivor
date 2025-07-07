using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Phase_Data", menuName = "My Scriptable Objects/PhaseData")]
public class PhaseData : ScriptableObject
{
    // Phase의 시작 시간. 초(s) 단위로 설정한다.
    public float phaseStartTime;

    public string phaseName;

    [SerializeField]
    public Tuple<Enemy, int>[] enemySpawnData;

    public List<Enemy> enemies;
    public int[] spawnTime;
}
