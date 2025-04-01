
using UnityEngine;

public static class CollisionDetectTool {
    public static bool CollisionDetect(this CapsuleCollider collider, out RaycastHit hit, Vector3 velocity, float distance) {
        return Physics.CapsuleCast(collider.transform.position,
            collider.transform.position + Vector3.up * collider.height, collider.radius, velocity,
            out hit, distance);
    }
}
