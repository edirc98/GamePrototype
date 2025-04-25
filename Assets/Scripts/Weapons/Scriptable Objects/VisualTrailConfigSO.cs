using UnityEngine;

[CreateAssetMenu(fileName = "Visual Trail Config", menuName = "Tank Weapons/ Visual Trail Configuration", order = 4)]
public class VisualTrailConfigSO : ScriptableObject
{
    #region VARIABLES

    //Trail Visual
    public Material TrailMaterial;
    public AnimationCurve TrailWidthCurve;
    public float TrailDuration = 0.5f;
    public float TrailMinVertexDistance = 0.1f;
    public Gradient TrailColorGradient;

    //Trail Behavior
    public float TrailMissDistance = 100.0f;
    public float TrailSimSpeed = 100.0f;
    #endregion
}
