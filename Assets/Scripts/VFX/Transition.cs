using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace VFX
{
    public class Transition : MonoBehaviour
    {
        [SerializeField] private Animator transition;

        public IEnumerator StartTransition()
        {
            transition.Play("Battle_Enter_Start", 0);

            yield return null;
            yield return new WaitUntil(() => transition.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
        }

        public IEnumerator EndTransition()
        {
            transition.Play("Battle_Enter_End", 0);

            yield return null;
            yield return new WaitUntil(() =>transition.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
        }
    }
}
