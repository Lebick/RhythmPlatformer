using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Alchemy.Inspector;
using TMPro;

public class GameHUD : MonoBehaviour
{
    [BoxGroup("ÄÞº¸")] public TMP_Text comboText;
    [BoxGroup("ÄÞº¸")] public Animator comboAnim;
    [BoxGroup("ÄÞº¸")] public ParticleSystem comboParticle;

    public void SetCombo(int combo)
    {
        comboText.text = $"{combo}\nCombo";

        comboAnim.SetTrigger("Combo");

        if(!comboParticle.isPlaying)
            comboParticle.Play();
    }

    public void ResetCombo(int combo)
    {
        comboText.text = $"{combo}\nCombo";

        comboAnim.SetTrigger("Hide");

        if (comboParticle.isPlaying)
            comboParticle.Stop();
    }
}
