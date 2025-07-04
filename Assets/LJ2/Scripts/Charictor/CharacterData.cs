using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace LJ2
{
    [CreateAssetMenu(fileName = "CharictorData", menuName = "Charictor/CharictorData")]
    public class CharacterData : ScriptableObject
    {
        public int id;
        public Grade grade;
        public string characterName;
        public GameObject characterModel;

        public Elemental elemental;
        public int maxLevel;
        public int hp;
        public int hpPlus;
        public GameObject bulletPrefab;
        public float attackDamage;
        public float damagePlus;
        public float attackSpeed;
        public float moveSpeed;
        public int defense;


        public int ultCoolDefault;
        public int ultCoolReduce;
        public Skill[] ultLore;
        public GameObject ultVisual;

        public Parry parry;
        public int parryCool;

        public Sprite icon;
        public Sprite image;

        public int upgradeUnitDefault;
        public int upgradeUnitPlus;


    }

    public enum Grade
    {
        SSR,
        R
    }
    public enum Elemental
    {
        물,
        불,
        바람
    }

    public enum Skill
    {
        A,
        B,
        D,
        E,
        H,
        I,
        J,
        K,
        L,
        M,
        N,
        O
    }

    public enum Parry
    {
        방어막,
        무적,
        반사B
    }
}