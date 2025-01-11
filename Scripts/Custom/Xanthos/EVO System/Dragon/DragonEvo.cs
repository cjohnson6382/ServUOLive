using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Xanthos.Interfaces;
using Scripts.Custom.Gumps;

namespace Xanthos.Evo
{
    [CorpseName("a dragon corpse")]
    public class RaelisDragon : BaseEvo, IEvoCreature
    {
        public override BaseEvoSpec GetEvoSpec()
        {
            return RaelisDragonSpec.Instance;
        }

        public override BaseEvoEgg GetEvoEgg()
        {
            return new RaelisDragonEgg();
        }

        public override bool AddPointsOnDamage { get { return true; } }
        public override bool AddPointsOnMelee { get { return false; } }
        public override Type GetEvoDustType() { return typeof(RaelisDragonDust); }

        public bool HasBreath { get { return true; } }

        // Track last EP to avoid unnecessary gump updates
        private int m_LastEp;

        // Constructor for new instances
        public RaelisDragon(string name) : base(name, AIType.AI_Mage, 0.01)
        {
            LoadSpecValues();
            m_LastEp = Ep; // Initialize with current EP
        }

        // Constructor for deserialization
        public RaelisDragon(Serial serial) : base(serial)
        {
        }

        // Implementing IEvoCreature members
        public override TimeSpan RemainingTerm => base.RemainingTerm;
        public override string Breed => base.Breed;
        public override bool Pregnant 
        { 
            get => base.Pregnant; 
            set => base.Pregnant = value; 
        }
        public override bool HasEgg 
        { 
            get => base.HasEgg; 
            set => base.HasEgg = value; 
        }
        public override bool CanHue => base.CanHue;
        public override int Ep 
        { 
            get => base.Ep; 
            set 
            { 
                base.Ep = value;
                // Log when EP changes, even if not from damage
                // Console.WriteLine($"RaelisDragon Ep updated to {value}");
            }
        }
        public override int Stage => base.Stage;
        
        public override void OnShrink(IShrinkItem shrinkItem)
        {
            base.OnShrink(shrinkItem);
        }

        protected override void LoadSpecValues()
        {
            base.LoadSpecValues();
            RaelisDragonSpec spec = (RaelisDragonSpec)GetEvoSpec();
            if (spec != null && spec.Stages != null && m_Stage < spec.Stages.Length)
            {
                BaseEvoStage stage = spec.Stages[m_Stage];
                
                // Set skills based on the current stage
                for (int i = 0; i < spec.Skills.Length; i++)
                {
                    SetSkill(spec.Skills[i], spec.MinSkillValues[i], spec.MaxSkillValues[i]);
                }

                // Debug logging to check if skills are being set
                // Console.WriteLine($"Setting skills for RaelisDragon at stage {m_Stage}");
                foreach (var skill in Skills)
                {
                    // Console.WriteLine($"{skill.SkillName}: {skill.Base}");
                }
            }
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            base.OnDamage(amount, from, willKill);
            
            if (!willKill && from != null && from is PlayerMobile && AddPointsOnDamage)
            {
                // Add XP and update gump
                GainExperience(10); // Example XP gain, adjust as needed
            }
        }

        // Method to handle experience gain and gump updating
        public void GainExperience(int amount)
        {
            int previousEp = Ep;
            Ep += amount;
            // Console.WriteLine($"Gained {amount} EP, from {previousEp} to {Ep}");
            UpdateGump();
        }

        // Method to update the gump for all viewers
        private void UpdateGump()
        {
            // Only update if EP has changed to avoid unnecessary processing
            if (Ep != m_LastEp)
            {
                m_LastEp = Ep; // Update last EP for comparison in next update

                // Log how many players are being updated
                int count = 0;
                foreach (NetState state in NetState.Instances)
                {
                    Mobile m = state.Mobile;
                    if (m != null && m.HasGump(typeof(RaelisDragonGump)))
                    {
                        count++;
                        m.CloseGump(typeof(RaelisDragonGump));
                        m.SendGump(new RaelisDragonGump(this));
                    }
                }
                // Console.WriteLine($"Updated gump for {count} players.");
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);			
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            m_LastEp = Ep; // Set last EP on deserialization
        }
    }
}