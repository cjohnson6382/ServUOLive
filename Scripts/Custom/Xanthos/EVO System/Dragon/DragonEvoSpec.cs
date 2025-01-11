using System;
using Server;

namespace Xanthos.Evo
{
    public sealed class RaelisDragonSpec : BaseEvoSpec
    {
        // Singleton pattern implementation for RaelisDragonSpec
        public static RaelisDragonSpec Instance { get { return Nested.instance; } }
        class Nested { static Nested() { } internal static readonly RaelisDragonSpec instance = new RaelisDragonSpec(); }

        private RaelisDragonSpec()
        {
            m_Tamable = true;
            m_MinTamingToHatch = 99.9;
            m_PercentFemaleChance = 0.02;   // Low chance to limit egg access
            m_GuardianEggOrDeedChance = 1.0;
            m_AlwaysHappy = false;
            m_ProducesYoung = true;
            m_PregnancyTerm = 0.10;          // 10% of a day for pregnancy term
            m_AbsoluteStatValues = false;    // Stats are added, not set
            m_MaxEvoResistance = 100;
            m_MaxTrainingStage = 3;
            m_CanAttackPlayers = false;

            m_RandomHues = new int[] 
            { 
                1157, 1175, 1172, 1170, 2703, 2473, 2643, 1156, 2704, 2734, 2669, 2621, 2859, 2716, 2791, 2927, 2974, 1161, 2717, 2652, 2821, 2818, 2730, 2670, 2678, 2630, 2641, 2644, 2592, 2543, 2526, 2338, 2339, 1793, 1980, 1983 
            };

            m_Skills = new SkillName[] 
            { 
                SkillName.Magery, SkillName.EvalInt, SkillName.Meditation, SkillName.MagicResist, 
                SkillName.Tactics, SkillName.Wrestling, SkillName.Anatomy 
            };
            m_MinSkillValues = new int[] { 50, 50, 50, 15, 19, 19, 19 };
            m_MaxSkillValues = new int[] { 120, 120, 110, 110, 100, 100, 100 };

            // Array of all stages for RaelisDragon evolution
            m_Stages = new BaseEvoStage[] 
            { 
                new RaelisDragonStageOne(), new RaelisDragonStageTwo(),
                new RaelisDragonStageThree(), new RaelisDragonStageFour(),
                new RaelisDragonStageFive(), new RaelisDragonStageSix(),
                new RaelisDragonStageSeven() 
            };
        }
    }

    // Stage definitions for RaelisDragon evolution
    public class RaelisDragonStageOne : BaseEvoStage
    {
        public RaelisDragonStageOne()
        {
            EvolutionMessage = "has evolved";
            NextEpThreshold = 1000; EpMinDivisor = 10; EpMaxDivisor = 5; DustMultiplier = 20;
            BaseSoundID = 0xDB;
            BodyValue = 52; ControlSlots = 2; MinTameSkill = 99.9; VirtualArmor = 30;
            Hue = Evo.Flags.kRandomHueFlag;

            DamagesTypes = new ResistanceType[] { ResistanceType.Physical };
            MinDamages = new int[] { 100 };
            MaxDamages = new int[] { 100 };

            ResistanceTypes = new ResistanceType[] { ResistanceType.Physical };
            MinResistances = new int[] { 15 };
            MaxResistances = new int[] { 15 };

            DamageMin = 11; DamageMax = 17; HitsMin = 200; HitsMax = 250;
            StrMin = 296; StrMax = 325; DexMin = 56; DexMax = 75; IntMin = 76; IntMax = 96;
        }
    }

    public class RaelisDragonStageTwo : BaseEvoStage
    {
        public RaelisDragonStageTwo()
        {
            EvolutionMessage = "has evolved";
            NextEpThreshold = 2000; EpMinDivisor = 20; EpMaxDivisor = 10; DustMultiplier = 20;
            BaseSoundID = 219;
            BodyValue = 89; VirtualArmor = 40;
        
            DamagesTypes = new ResistanceType[] { ResistanceType.Physical, ResistanceType.Fire, ResistanceType.Cold, ResistanceType.Poison, ResistanceType.Energy };
            MinDamages = new int[] { 100, 25, 25, 25, 25 };
            MaxDamages = new int[] { 100, 25, 25, 25, 25 };

            ResistanceTypes = new ResistanceType[] { ResistanceType.Physical, ResistanceType.Fire, ResistanceType.Cold, ResistanceType.Poison, ResistanceType.Energy };
            MinResistances = new int[] { 20, 20, 20, 20, 20 };
            MaxResistances = new int[] { 20, 20, 20, 20, 20 };

            DamageMin = 1; DamageMax = 1; HitsMin = 500; HitsMax = 500;
            StrMin = 200; StrMax = 200; DexMin = 20; DexMax = 20; IntMin = 30; IntMax = 30;
        }
    }

    public class RaelisDragonStageThree : BaseEvoStage
    {
        public RaelisDragonStageThree()
        {
            EvolutionMessage = "has evolved";
            NextEpThreshold = 3000; EpMinDivisor = 30; EpMaxDivisor = 20; DustMultiplier = 20;
            BaseSoundID = 0x5A;
            BodyValue = 0xCE; VirtualArmor = 50;
        
            DamagesTypes = null;
            MinDamages = null;
            MaxDamages = null;

            ResistanceTypes = new ResistanceType[] { ResistanceType.Physical, ResistanceType.Fire, ResistanceType.Cold, ResistanceType.Poison, ResistanceType.Energy };
            MinResistances = new int[] { 40, 40, 40, 40, 40 };
            MaxResistances = new int[] { 40, 40, 40, 40, 40 };

            DamageMin = 1; DamageMax = 1; HitsMin = 100; HitsMax = 100;
            StrMin = 100; StrMax = 100; DexMin = 10; DexMax = 10; IntMin = 20; IntMax = 20;
        }
    }

    public class RaelisDragonStageFour : BaseEvoStage
    {
        public RaelisDragonStageFour()
        {
            EvolutionMessage = "has evolved";
            NextEpThreshold = 4000; EpMinDivisor = 50; EpMaxDivisor = 40; DustMultiplier = 20;
            BaseSoundID = 362;
            BodyValue = 60; ControlSlots = 3; MinTameSkill = 119.9; VirtualArmor = 60;
        
            DamagesTypes = null;
            MinDamages = null;
            MaxDamages = null;

            ResistanceTypes = new ResistanceType[] { ResistanceType.Physical, ResistanceType.Fire, ResistanceType.Cold, ResistanceType.Poison, ResistanceType.Energy };
            MinResistances = new int[] { 60, 60, 60, 60, 60 };
            MaxResistances = new int[] { 60, 60, 60, 60, 60 };	

            DamageMin = 1; DamageMax = 1; HitsMin = 100; HitsMax = 100;
            StrMin = 100; StrMax = 100; DexMin = 10; DexMax = 10; IntMin = 120; IntMax = 120;
        }
    }

    public class RaelisDragonStageFive : BaseEvoStage
    {
        public RaelisDragonStageFive()
        {
            EvolutionMessage = "has evolved";
            NextEpThreshold = 5000; EpMinDivisor = 160; EpMaxDivisor = 40; DustMultiplier = 20;
            BodyValue = 59; VirtualArmor = 70;
        
            DamagesTypes = new ResistanceType[] { ResistanceType.Physical, ResistanceType.Fire, ResistanceType.Cold, ResistanceType.Poison, ResistanceType.Energy };
            MinDamages = new int[] { 100, 50, 50, 50, 50 };
            MaxDamages = new int[] { 100, 50, 50, 50, 50 };

            ResistanceTypes = new ResistanceType[] { ResistanceType.Physical, ResistanceType.Fire, ResistanceType.Cold, ResistanceType.Poison, ResistanceType.Energy };
            MinResistances = new int[] { 80, 80, 80, 80, 80 };
            MaxResistances = new int[] { 80, 80, 80, 80, 80 };	

            DamageMin = 5; DamageMax = 5; HitsMin = 100; HitsMax = 100;
            StrMin = 100; StrMax = 100; DexMin = 20; DexMax = 20; IntMin = 120; IntMax = 120;
        }
    }

    public class RaelisDragonStageSix : BaseEvoStage
    {
        public RaelisDragonStageSix()
        {
            EvolutionMessage = "has evolved";
            NextEpThreshold = 6000; EpMinDivisor = 540; EpMaxDivisor = 480; DustMultiplier = 20;
            BodyValue = 46; VirtualArmor = 170;
        
            DamagesTypes = null;
            MinDamages = null;
            MaxDamages = null;

            ResistanceTypes = new ResistanceType[] { ResistanceType.Physical, ResistanceType.Fire, ResistanceType.Cold, ResistanceType.Poison, ResistanceType.Energy };
            MinResistances = new int[] { 98, 98, 98, 98, 98 };
            MaxResistances = new int[] { 98, 98, 98, 98, 98 };	

            DamageMin = 5; DamageMax = 5; HitsMin = 100; HitsMax = 100;
            StrMin = 100; StrMax = 100; DexMin = 20; DexMax = 20; IntMin = 120; IntMax = 120;
        }
    }

    public class RaelisDragonStageSeven : BaseEvoStage
    {
        public RaelisDragonStageSeven()
        {
            Title = "The Ancient Dragon";
            EvolutionMessage = "has evolved to its highest form and is now an Ancient Dragon"; 							            NextEpThreshold = 7000; EpMinDivisor = 740; EpMaxDivisor = 660; DustMultiplier = 20;
            BaseSoundID = 362;
            BodyValue = 197; ControlSlots = 4; VirtualArmor = 270;
        
            DamagesTypes = new ResistanceType[] { ResistanceType.Physical, ResistanceType.Fire, ResistanceType.Cold, ResistanceType.Poison, ResistanceType.Energy };
            MinDamages = new int[] { 100, 75, 75, 75, 75 };
            MaxDamages = new int[] { 100, 75, 75, 75, 75 };

            ResistanceTypes = null;
            MinResistances = null;
            MaxResistances = null;	

            DamageMin = 15; DamageMax = 55; HitsMin = 1350; HitsMax = 1800;
            StrMin = 115; StrMax = 150; DexMin = 35; DexMax = 125; IntMin = 115; IntMax = 125;
        }
    }
}