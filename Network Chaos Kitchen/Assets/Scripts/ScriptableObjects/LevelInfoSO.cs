
using UnityEngine;


[CreateAssetMenu(fileName = "LevelInfo", menuName = "Chaos Kitchen/Level Info")]
public class LevelInfoSO : ScriptableObject {
    public string LevelName;
    public Level CurrentLevel;
    public Level NextLevel;
    public int PrimaryScore;
    public int IntermediateScore;
    public int SeniorScore;
}



