using UnityEngine;

[CreateAssetMenu(fileName = "CameraSetting", menuName = "CameraSetting", order = 1)]
public class CameraSetting : ScriptableObject
{
    public float OrtoSizeByMapSize = 1f;
    public float PositionModifierByMapSize = 0.5f;
    public float DefaultOrtoSize = 7.3f;
    public Vector3 StartingPosition;
}