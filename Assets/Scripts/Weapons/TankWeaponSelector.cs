using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class TankWeaponSelector : MonoBehaviour
{
    #region VARIABLES
    [Header("Weapon Parent")]
    [SerializeField] private Transform WeaponParent;
    [Header("Weapon Type")]
    [SerializeField] private WeaponType Weapon;
    [Header("Available Weapons")]
    [SerializeField] private List<WeaponSO>Weapons;

    [Header("Runtime References")]
    public WeaponSO ActiveWeapon;

    #endregion

    #region UNITY METHODS
    void Start()
    {
        InitWeapon();
    }
    #endregion

    #region METHODS
    private void InitWeapon()
    {
        WeaponSO weapon = Weapons.Find(weapon => weapon.Type == Weapon);
        if (weapon == null)
        {
            Debug.LogError($"No WeaponSO found for WeaponType:{weapon}");
            return;
        }
        ActiveWeapon = weapon;
        weapon.Spawn(WeaponParent, this);
    }
    #endregion
}
