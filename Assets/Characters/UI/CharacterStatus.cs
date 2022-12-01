using System;
using Characters.Monsters;
using JetBrains.Annotations;
using MyBox;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Characters.UI
{
    [Serializable]
    public class CharacterStatus
    {
        [SerializeField] [CanBeNull] private Text characterName;
        [SerializeField] [CanBeNull] private Text characterLevel;
        [SerializeField] [CanBeNull] private Image statusCondition;
        
        [SerializeField] protected bool hasHealthBar;
        [SerializeField] [ConditionalField(nameof(hasHealthBar))]
        private FillableBar healthBar;

        [SerializeField] protected bool hasExpBar;
        [SerializeField] [ConditionalField(nameof(hasExpBar))]
        private FillableBar expBar;

        [SerializeField] protected bool hasStats;
        [SerializeField] [ConditionalField(nameof(hasStats))]
        private CharacterStats stats;
        
        public Text Name => characterName;
        public Text Level => characterLevel;
        [CanBeNull] public FillableBar HealthBar => hasHealthBar ? healthBar : null;
        [CanBeNull] public FillableBar ExpBar => hasExpBar ? expBar : null;
        [CanBeNull] public CharacterStats Stats => hasStats ? stats : null;
    }

    [Serializable]
    public class CharacterStats
    {
        [SerializeField] private Text maxHealth;
        [SerializeField] private Text attackText;
        [SerializeField] private Text defenceText;
        [SerializeField] private Text spAtkText;
        [SerializeField] private Text spDefText;
        [SerializeField] private Text speedText;

        public Text MaxHealth => maxHealth;
        public Text AttackText => attackText;
        public Text DefenceText => defenceText;
        public Text SpAtkText => spAtkText;
        public Text SpDefText => spDefText;
        public Text SpeedText => speedText;
    }
}