using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellBar : MonoBehaviour
{
    public static SpellBar main;
    void Awake() {
        main = this;
    }

    [SerializeField]
    private SpellButton fireBall;
    [SerializeField]
    private SpellButton magicMissile;
    [SerializeField]
    private SpellButton wall;

    public void SpellWasCast(SpellBaseConfig spell) {
        if (spell == fireBall.Spell) {
            fireBall.Cooldown();
        }
        else if (spell == magicMissile.Spell) {
            magicMissile.Cooldown();
        }
        else if (spell == wall.Spell) {
            wall.Cooldown();
        }
    }
}
