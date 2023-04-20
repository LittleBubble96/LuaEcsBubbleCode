using UnityEngine;

namespace Bubble.Tools
{
    public static class ExtensionMethods
    {
        public static void ForceCrossFade(this Animator animator , string aniName , float transitionDuration, int layer = 0, float normalizedTime = float.NegativeInfinity)
        {
            animator.Update(0);
            if (animator.GetNextAnimatorStateInfo(layer).fullPathHash == 0)
            {
                animator.CrossFade(aniName, transitionDuration, layer, normalizedTime);
            }
            else
            {
                // animator.Play(animator.GetNextAnimatorStateInfo(layer).fullPathHash, layer);
                animator.Update(0);
                animator.CrossFade(aniName, transitionDuration, layer, normalizedTime);
            }
        }
    }
}