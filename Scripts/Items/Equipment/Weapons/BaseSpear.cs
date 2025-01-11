using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Engines.Harvest;
using Server.Mobiles;

namespace Server.Items
{
    public abstract class BaseSpear : BaseMeleeWeapon, IHarvestTool
    {
        public BaseSpear(int itemID)
            : base(itemID)
        {
        }

        public BaseSpear(Serial serial)
            : base(serial)
        {
        }

        public override int DefHitSound
        {
            get
            {
                return 0x23C;
            }
        }

        public override int DefMissSound
        {
            get
            {
                return 0x238;
            }
        }

        public override SkillName DefSkill
        {
            get
            {
                return SkillName.Fencing;
            }
        }

        public override WeaponType DefType
        {
            get
            {
                return WeaponType.Piercing;
            }
        }

        public override WeaponAnimation DefAnimation
        {
            get
            {
                return WeaponAnimation.Pierce1H;
            }
        }

        public virtual HarvestSystem HarvestSystem
        {
            get
            {
                return Lumberjacking.System;
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (this.HarvestSystem == null)
                return;

            if (this.IsChildOf(from.Backpack) || this.Parent == from)
                this.HarvestSystem.BeginHarvesting(from, this);
            else
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (this.HarvestSystem != null)
                BaseHarvestTool.AddContextMenuEntries(from, this, list, this.HarvestSystem);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)3); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 3:
                    break;
                case 2:
                    {
                        if (version == 2)
                            ShowUsesRemaining = reader.ReadBool();
                        goto case 1;
                    }
                case 1:
                    {
                        if (version == 2)
                            UsesRemaining = reader.ReadInt();
                        goto case 0;
                    }
                case 0:
                    {
                        break;
                    }
            }
        }

        public override void OnHit(Mobile attacker, IDamageable defender, double damageBonus)
        {
            base.OnHit(attacker, defender, damageBonus);

            if (!Core.AOS && defender is Mobile mobileDefender && this.Layer == Layer.TwoHanded && (attacker.Skills[SkillName.Anatomy].Value / 400.0) >= Utility.RandomDouble())
            {
                // Check if the defender is not a player (for PvM only)
                if (!(mobileDefender is PlayerMobile))
                {
                    mobileDefender.SendMessage("You receive a paralyzing blow!"); // Consider localizing this message
                    mobileDefender.Freeze(TimeSpan.FromSeconds(2.0));

                    attacker.SendMessage("You deliver a paralyzing blow!"); // Consider localizing this message
                    attacker.PlaySound(0x11C);
                }
            }

            if (!Core.AOS && defender is Mobile && this.Poison != null && this.PoisonCharges > 0)
            {
                --this.PoisonCharges;

                if (Utility.RandomDouble() >= 0.5) // 50% chance to poison
                    ((Mobile)defender).ApplyPoison(attacker, this.Poison);
            }
        }
    }
}