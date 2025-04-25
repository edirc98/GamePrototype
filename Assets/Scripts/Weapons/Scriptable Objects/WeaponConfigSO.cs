using UnityEngine;


[CreateAssetMenu(fileName = "Weapon Config", menuName = "Tank Weapons/ Weapon Configuration", order = 2)]
public class WeaponConfigSO : ScriptableObject
{
    #region VARIABLES
    //Layer for hit mask 
    public LayerMask HitMask;

    //Weapon shoot parameters
    public float FireRate = 0.25f;

    public bool HasSpread = false;
    public Vector3 SpreadAxis = Vector3.zero;
    public Vector3 SpreadAmmount = Vector3.zero;
    #endregion
}
