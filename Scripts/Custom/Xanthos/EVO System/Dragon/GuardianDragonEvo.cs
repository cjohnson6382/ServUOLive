using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Xanthos.Interfaces;

namespace Xanthos.Evo
{
    [CorpseName("a guardian dragon corpse")]
    public class GuardianDragon : RaelisDragon, IEvoGuardian
    {
        public override bool AddPointsOnDamage { get { return false; } }
        public override bool AddPointsOnMelee { get { return false; } }

        [Constructable]
        public GuardianDragon() : base("A Guardian Dragon")
        {
            Init(); // Call Init directly since it's not overridden anymore
        }

        public GuardianDragon(Serial serial) : base(serial)
        {
        }

        protected void Init()
        {
            base.Init(); // Create and fully evolve the creature

            // Set the body type to 826
            Body = 826;

            // Buff it up
            SetStr(Str * 5); // Increased strength multiplier for ultra-strong
            SetDex(Dex * 5);
            SetStam(Stam * 5);
            SetInt((int)(Int * 3)); // Increased intelligence multiplier
            SetMana((int)(Mana * 3));
            SetHits(Hits * 20); // Increased hit points multiplier

            BaseEvoSpec spec = GetEvoSpec();

            if (null != spec && null != spec.Skills)
            {
                for (int i = 0; i < spec.Skills.Length; i++)
                {
                    // Increase skill values even more for ultra-strong
                    SetSkill(spec.Skills[i], (double)(spec.MaxSkillValues[i]) * 1.25, (double)(spec.MaxSkillValues[i]) * 2.00);
                }
            }

            this.Tamable = false; // Not appropriate as a pet
            Title = "";
        }

        protected void PackSpecialItem() { }
        
        public override void GenerateLoot()
        {
            BaseEvoSpec spec = GetEvoSpec();

            if (null != spec && spec.GuardianEggOrDeedChance > Utility.RandomDouble())
            {
                BaseEvoEgg egg = GetEvoEgg();

                if (null != egg)
                    PackItem(egg);
            }
            AddLoot(LootPack.UltraRich, 4);
            AddLoot(LootPack.FilthyRich);
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
            // Add this line to ensure the body is set correctly upon deserialization
            Body = 826;
        }
    }
}