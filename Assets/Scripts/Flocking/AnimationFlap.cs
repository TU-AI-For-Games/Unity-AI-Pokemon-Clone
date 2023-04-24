using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationFlap : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Animator animator = GetComponent<Animator>();
        var state = animator.GetCurrentAnimatorStateInfo(0);
        float Offset = Random.Range(0f, 1.0f);
        animator.Play(state.fullPathHash, 0, Offset);
    }
}
