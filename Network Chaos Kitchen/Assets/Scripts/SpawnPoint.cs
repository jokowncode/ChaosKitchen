using UnityEngine;

public class SpawnPoint : MonoBehaviour {

    public Vector3 GetSpawnPoint(int index) {
        if (index >= transform.childCount || index < 0) {
            return Vector3.zero;
        }
        return transform.GetChild(index).position;
    }

}
