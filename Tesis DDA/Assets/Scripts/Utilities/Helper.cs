using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helper : MonoBehaviour
{
    [Range(-1, 1)]
    public float vertical;
    [Range(-1, 1)]
    public float horizontal;

    public string[] oh_attacks;
    public string[] th_attacks;

    public bool playAnim;
    public bool twoHanded;
    public bool enableRooMotion;
    public bool usetItem;
    public bool interacting;
    public bool lockOn;

    Animator anim;

    private void Start() {
        anim = GetComponent<Animator>();
    }

    private void Update() {
        enableRooMotion = !anim.GetBool("canMove");
        anim.applyRootMotion = enableRooMotion;

        interacting = anim.GetBool("interacting");
        if (lockOn==false) {
            horizontal = 0;
            vertical = Mathf.Clamp01(vertical);
        }
        anim.SetBool("lockOn",lockOn);
        if (enableRooMotion) {
            return;
        }
        if (usetItem == true) {
            anim.Play("use_item");
            usetItem = false;
        }
        if (interacting) {
            playAnim = false;
            vertical = Mathf.Clamp(vertical,0,0.5f);
        }
        anim.SetBool("two_handed",twoHanded);
        if (playAnim) {
            string targetAnim;
            if (!twoHanded) {
                int r = Random.Range(0, oh_attacks.Length);
                targetAnim = oh_attacks[r];
                if (vertical > 0.5f) {
                    targetAnim = "oh_attack_3";
                }
            }
            else {
                int r = Random.Range(0, th_attacks.Length);
                targetAnim = th_attacks[r];
                if (vertical > 0.5f) {
                    targetAnim = "oh_attack_3"; 
                }
            }
            vertical = 0;
            anim.CrossFade(targetAnim, 0.2f);
            playAnim = false;
        }
        anim.SetFloat("vertical",vertical);
        anim.SetFloat("horizontal",horizontal);
    }
}
