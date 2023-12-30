using System;
using JetBrains.Annotations;
using MyBox;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Characters.UI
{
    [Serializable]
    public class CharacterStatus
    {
        [SerializeField] [CanBeNull] private TextMeshProUGUI characterName;
        [SerializeField] [CanBeNull] private TextMeshProUGUI characterLevel;
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

        public TextMeshProUGUI Name => characterName;
        public TextMeshProUGUI Level => characterLevel;
        [CanBeNull] public FillableBar HealthBar => hasHealthBar ? healthBar : null;
        [CanBeNull] public FillableBar ExpBar => hasExpBar ? expBar : null;
        [CanBeNull] public CharacterStats Stats => hasStats ? stats : null;
    }

    [Serializable]
    public class CharacterStats
    {
        [SerializeField] private TextMeshProUGUI maxHealth;
        [SerializeField] private TextMeshProUGUI attackText;
        [SerializeField] private TextMeshProUGUI defenceText;
        [SerializeField] private TextMeshProUGUI spAtkText;
        [SerializeField] private TextMeshProUGUI spDefText;
        [SerializeField] private TextMeshProUGUI speedText;

        public TextMeshProUGUI MaxHealth => maxHealth;
        public TextMeshProUGUI AttackText => attackText;
        public TextMeshProUGUI DefenceText => defenceText;
        public TextMeshProUGUI SpAtkText => spAtkText;
        public TextMeshProUGUI SpDefText => spDefText;
        public TextMeshProUGUI SpeedText => speedText;
    }
}