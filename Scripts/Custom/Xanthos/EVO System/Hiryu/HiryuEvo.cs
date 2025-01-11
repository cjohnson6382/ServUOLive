using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Xanthos.Interfaces;

namespace Xanthos.Evo
{
    [CorpseName("a hiryu corpse")]
    public class EvoHiryu : BaseEvoMount, IEvoCreature
    {
        public override BaseEvoSpec GetEvoSpec() => HiryuEvoSpec.Instance;
        public override BaseEvoEgg GetEvoEgg() => new HiryuEvoEgg();
        public override bool AddPointsOnDamage => true;
        public override bool AddPointsOnMelee => false;
        public override Type GetEvoDustType() => typeof(HiryuEvoDust);

        // Assuming 'HasBreath' isn't in BaseEvoMount, we define it here
        public virtual bool HasBreath => true;

        // Use AIType instead of int for AI behavior
        [Constructable]
        public EvoHiryu(string name) : base(name, AIType.AI_Melee, 0.2)
        {
            // Additional initialization if needed
        }

        // Serialization constructor
        public EvoHiryu(Serial serial) : base(serial)
        {
        }

        // Implementing IEvoCreature members - these are already inherited from BaseEvoMount
        // No need to redeclare unless you're adding specific logic

        public override WeaponAbility GetWeaponAbility() => WeaponAbility.Dismount;

        public override bool SubdueBeforeTame => true; // Must be beaten into submission

        public override int GetAngerSound() => 0x4FF;
        public override int GetIdleSound() => 0x4FE;
        public override int GetAttackSound() => 0x4FD;
        public override int GetHurtSound() => 0x500;
        public override int GetDeathSound() => 0x4FC;

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}