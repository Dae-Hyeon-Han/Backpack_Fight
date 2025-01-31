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

    public abstract void EquipWeapon();     // ���� ����
    public abstract void DropWeapon();      // ���� ����
    public abstract void MergeWeapon();     // ���� �������� ���� ����
}
