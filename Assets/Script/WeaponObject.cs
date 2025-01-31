using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponObject
{
    [Header ("---weapon informaiton---")]
    int row;
    int column;
    int[] cellInfo;
    Sprite weaponIcon;

    public abstract void EquipWeapon();     // 무기 장착
    public abstract void DropWeapon();      // 무기 해제
    public abstract void MergeWeapon();     // 무기 병합으로 인한 업글
}
