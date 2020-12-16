﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHook : MonoBehaviour
{
    public GameObject[] damageCollider;

    public void openDamageColliders() {
        for (int i = 0; i < damageCollider.Length; i++) {
            damageCollider[i].SetActive(true);
        }
    }
    public void CloseDamageColliders() {
        for (int i = 0; i < damageCollider.Length; i++) {
            damageCollider[i].SetActive(false);
        }
    }
}
