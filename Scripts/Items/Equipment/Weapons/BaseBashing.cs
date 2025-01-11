using System;
using Server.Mobiles;

namespace Server.Items
{
    public abstract class BaseBashing : BaseMeleeWeapon
    {
        public BaseBashing(int itemID)
            : base(itemID)
        {
        }

        public BaseBashing(Serial serial)
            : base(serial)
        {
        }

        public override int DefHitSound
        {
            get
            {
                return 0x233;
            }
        }

        public override int DefMissSound
        {
            get
            {
                return 0x239;
            }
        }

        public override SkillName DefSkill
        {
            get
            {
                return SkillName.Macing;
            }
        }

        public override WeaponType DefType
        {
            get
            {
                return WeaponType.Bashing;
            }
        }

        public override WeaponAnimation DefAnimation
        {
            get
            {
                return WeaponAnimation.Bash1H;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        public override void OnHit(Mobile attacker, IDamageable defender, double damageBonus)
        {
            base.OnHit(attacker, defender, damageBonus);

            if (defender is Mobile mobileDefender)
            {
                mobileDefender.Stam -= Utility.Random(3, 3); // 3-5 points of stamina loss

                // Check if the defender is not a player (for PvM only)
                if (!Core.AOS && (attacker.Player || attacker.Body.IsHuman) && this.Layer == Layer.TwoHanded && (attacker.Skills[SkillName.Anatomy].Value / 400.0) >= Utility.RandomDouble() && !(mobileDefender is PlayerMobile))
                {
                    double damage = base.GetBaseDamage(attacker);
                    damage *= 1.5;
                    AOS.Damage(mobileDefender, attacker, (int)damage - (int)base.GetBaseDamage(attacker), 0, 0, 0, 0, 0, 0, 100); // Apply the extra damage

                    attacker.SendMessage("You deliver a crushing blow!"); // Consider localizing this message
                    attacker.PlaySound(0x11C);
                }
            }
        }

        // Remove the GetBaseDamage override since it's now handled in OnHit
    }
}