#region AuthorHeader
//
//	EvoSystem version 2.1, by Xanthos
//
#endregion AuthorHeader

using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Gumps;
using Server.Network;
using Server.Targeting;
using Xanthos.Interfaces;
using Xanthos.ShrinkSystem;

namespace Xanthos.Evo
{
    [CorpseName("an evolution creature corpse")]
    public abstract class BaseEvo : BaseCreature, IEvoCreature
    {
        private static double kOverLimitLossChance = 0.02; // Chance that loyalty will be lost if over followers limit

        protected int m_Ep;
        protected int m_Stage;
        protected bool m_Pregnant;
        protected bool m_HasEgg;
        protected DateTime m_DeliveryDate;
        protected ShrinkItem m_ShrinkItem;

        protected int m_FinalStage;
        protected int m_MaxTrainingStage;
        protected int m_EpMinDivisor = 10; // Default values, adjust if needed
        protected int m_EpMaxDivisor = 20; // Default values, adjust if needed
        protected int m_DustMultiplier;
        protected int m_NextEpThreshold;
        protected TimeSpan m_InitialTerm;
        protected bool m_CanAttackPlayers;
        protected bool m_ProducesYoung;
        protected bool m_AlwaysHappy;
        protected DateTime m_NextHappyTime;

        protected string m_Breed;
        protected BaseAI m_ForcedAI;
        protected PregnancyTimer m_PregnancyTimer;

        // Abstract methods for subclasses
        public abstract BaseEvoSpec GetEvoSpec();
        public abstract BaseEvoEgg GetEvoEgg();
        public abstract Type GetEvoDustType();
        public abstract bool AddPointsOnDamage { get; }
        public abstract bool AddPointsOnMelee { get; }

        // Constructors
        [Constructable]
        protected BaseEvo(string name, AIType ai, double dActiveSpeed, double dPassiveSpeed = 0.4) 
            : base(ai, FightMode.Closest, 10, 1, dActiveSpeed, dPassiveSpeed)
        {
            Name = name;
            Init();
            InitAI();
        }

        // Serialization constructor - without [Constructable]
        protected BaseEvo(Serial serial) : base(serial)
        {
            // No initialization here; everything should be handled in Deserialize
        }

        protected virtual void Init()
        {
            if (m_Stage == 0 && m_Ep == 0) // Only initialize for new creatures where both stage and EP are 0
            {
                m_Stage = 1; // Default to stage 1 for new creatures post-hatch
                m_Ep = 0; // Initial experience points
                m_NextEpThreshold = GetEvoSpec()?.Stages?[0].NextEpThreshold ?? 0; // Set initial threshold for evolution

                // Set initial stats and properties
                SetStr(10, 20); // Example for hatchling
                SetDex(10, 20);
                SetInt(10, 20);
                SetHits(20, 30); // Example hit points for hatchling
                Body = 52; // Example body ID for visibility, adjust as needed
                Hue = 0;   // Default hue, change if custom required

                // Ensure divisors are set to prevent division by zero
                m_EpMinDivisor = Math.Max(10, m_EpMinDivisor);  
                m_EpMaxDivisor = Math.Max(m_EpMinDivisor + 10, m_EpMaxDivisor);

                LoadSpecValues(); // Load stage-specific values for the hatchling stage
            }
        }

        protected virtual void InitAI()
        {
            // Placeholder for AI initialization logic - ensure this doesn't interfere with deserialization
        }

        // IEvoCreature interface members
        public virtual TimeSpan RemainingTerm => 
            m_DeliveryDate == DateTime.MinValue ? m_InitialTerm : m_DeliveryDate - DateTime.UtcNow;

        public virtual string Breed
        {
            get => m_Breed ?? (m_Breed = Xanthos.Utilities.Misc.GetFriendlyClassName(GetType().Name));
        }

        public virtual bool Pregnant
        {
            get => m_Pregnant;
            set
            {
                m_Pregnant = value;
                if (value)
                {
                    m_PregnancyTimer = new PregnancyTimer(this);
                    m_DeliveryDate = DateTime.UtcNow + m_PregnancyTimer.Delay;
                }
                else
                {
                    m_PregnancyTimer?.Stop();
                    m_PregnancyTimer = null;
                    m_DeliveryDate = DateTime.MinValue;
                }
            }
        }

        public virtual bool HasEgg
        {
            get => m_HasEgg;
            set
            {
                m_HasEgg = value;
                if (value) Pregnant = false;
            }
        }

        public virtual bool CanHue => true;

        public virtual int Ep
        {
            get => m_Ep;
            set => m_Ep = value;
        }

        public virtual int Stage => m_Stage;

        public virtual void OnShrink(IShrinkItem shrinkItem)
        {
            m_ShrinkItem = (ShrinkItem)shrinkItem;
        }

        // Damage methods
        public override int Damage(int amount, Mobile defender)
        {
            if (AddPointsOnDamage) AddPoints(defender);
            return base.Damage(amount, defender);
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            if (AddPointsOnMelee) AddPoints(defender);
            base.OnGaveMeleeAttack(defender);
        }

        private void AddPoints(Mobile defender)
        {
            if (defender == null || defender.Deleted) return;

            if (defender is TrainingElemental && m_Stage >= m_MaxTrainingStage && ControlMaster != null)
            {
                Emote("*stops fighting*");
                Combatant = null;
                ControlTarget = ControlMaster;
                ControlOrder = OrderType.Follow;
                ControlMaster.SendMessage("Your pet can no longer gain experience points fighting Training Elementals!");
                return;
            }

            if (defender is BaseCreature bc)
            {
                int minDivisor = Math.Max(1, m_EpMinDivisor);  
                int maxDivisor = Math.Max(1, m_EpMaxDivisor);

                int minPoints = 5 + (bc.HitsMax / minDivisor);
                int maxPoints = 5 + (bc.HitsMax / maxDivisor);

                m_Ep += Utility.RandomMinMax(minPoints, maxPoints);

                // Check for evolution immediately after gaining EP
                CheckForEvolution();
            }
        }

        protected virtual void CheckForEvolution()
        {
            BaseEvoSpec spec = GetEvoSpec();
            if (spec != null && spec.Stages != null && m_Stage < spec.Stages.Length)
            {
                int currentStageIndex = m_Stage - 1; // Stage indexes are 0-based, but m_Stage starts from 1
                if (currentStageIndex >= 0 && currentStageIndex < spec.Stages.Length)
                {
                    BaseEvoStage currentStage = spec.Stages[currentStageIndex];
                    if (m_Ep >= currentStage.NextEpThreshold)
                    {
                        Evolve(false);
                    }
                }
            }
        }

        protected virtual void Evolve(bool hatching)
        {
            BaseEvoSpec spec = GetEvoSpec();
            if (spec != null && spec.Stages != null && m_Stage < spec.Stages.Length)
            {
                m_Stage++;
                LoadSpecValues();
                SetStr(Str + 10, Str + 20);
                SetDex(Dex + 5, Dex + 10);
                SetInt(Int + 5, Int + 10);
                SetHits(HitsMax + 20, HitsMax + 30);

                Console.WriteLine($"{this.Name} has evolved to stage {m_Stage}!");

                if (hatching)
                {
                    // Special logic for hatching if needed
                }
            }
        }

        protected virtual void LoadSpecValues()
        {
            BaseEvoSpec spec = GetEvoSpec();
            if (spec != null && spec.Stages != null && m_Stage - 1 < spec.Stages.Length) // m_Stage is 1-based, array is 0-based
            {
                BaseEvoStage stage = spec.Stages[m_Stage - 1];
                Body = stage.BodyValue;
                // Apply other properties from stage
                if (stage.NextEpThreshold > 0)
                {
                    m_NextEpThreshold = stage.NextEpThreshold;
                }
                // Update any other stage-specific values here
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)2); // Version

            writer.Write(m_ShrinkItem);
            writer.Write(m_Ep);
            writer.Write(m_Stage);
            writer.Write(m_Pregnant);
            writer.Write(m_HasEgg);
            writer.Write(m_DeliveryDate);

            // Debug logging
            // Console.WriteLine($"Serialized {this.GetType().Name} with serial {Serial.Value}. EP: {m_Ep}, Stage: {m_Stage}");
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 2:
                    m_ShrinkItem = (ShrinkItem)reader.ReadItem();
                    goto case 1;
                case 1:
                    // Fall through to case 0 for backwards compatibility
                    goto case 0;
                case 0:
                    m_Ep = reader.ReadInt();
                    m_Stage = reader.ReadInt();
                    m_Pregnant = reader.ReadBool();
                    m_HasEgg = reader.ReadBool();
                    m_DeliveryDate = reader.ReadDateTime();
                    break;
            }

            // Load stage-specific values after deserialization
            LoadSpecValues();

            // Debug logging
            // Console.WriteLine($"Deserialized {this.GetType().Name} with serial {Serial.Value}. EP: {m_Ep}, Stage: {m_Stage}");
        }
    }

    public class PregnancyTimer : Timer
    {
        private IEvoCreature m_Evo;

        public PregnancyTimer(IEvoCreature female) : base(female.RemainingTerm)
        {
            Priority = TimerPriority.OneMinute;
            m_Evo = female;
            Start();
        }

        protected override void OnTick()
        {
            m_Evo.HasEgg = true;
            Stop();
        }
    }
}