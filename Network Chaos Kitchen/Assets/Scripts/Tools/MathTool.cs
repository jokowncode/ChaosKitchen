
using UnityEngine;

public static class MathTool {
    public static bool DotTest(Vector3 pos1, Vector3 pos2, Vector3 dir, float threshold = 0.75f) {
        return Vector3.Dot((pos2 - pos1).normalized, dir.normalized) > threshold;
    }
    
    public static void SetIndicatorUIPosition(this Transform uiTrans, Vector3 worldPos, bool revise = true) {
        Camera main = NetworkGameManager.Instance.MainCamera;
        if (!main) return;
        if (revise) {
            worldPos.x += worldPos.x * 0.25f / 1.5f;
        }
        uiTrans.position = main.WorldToScreenPoint(worldPos);
    }
}
