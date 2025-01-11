using System;
using Server.Items;
using Server.Network;

namespace Server.Mobiles
{
    public abstract class BaseGuard : Mobile
    {
        public BaseGuard(Mobile target)
        {
            GuardImmune = true;

            if (target != null)
            {
                Location = target.Location;
                Map = target.Map;

                Effects.SendLocationParticles(EffectItem.Create(Location, Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 5023);
            }

            InitStats(120, 120, 120); // Default stats
            RangePerception = 10; // Default perception range
            RangeFight = 1; // Default fight range
        }

        public BaseGuard(Serial serial) : base(serial)
        {
        }

        public abstract Mobile Focus { get; set; }

        // Added properties for combat and perception range
        public virtual int RangePerception { get; set; }
        public virtual int RangeFight { get; set; }

        public override bool CanBeHarmful(IDamageable target, bool message, bool ignoreOurBlessedness)
        {
            if (target is Mobile && ((Mobile)target).GuardImmune)
            {
                return false;
            }

            return base.CanBeHarmful(target, message, ignoreOurBlessedness);
        }

        public static void Spawn(Mobile caller, Mobile target)
        {
            Spawn(caller, target, 1, false);
        }

        public static void Spawn(Mobile caller, Mobile target, int amount, bool onlyAdditional)
        {
            if (target == null || target.Deleted || target.GuardImmune)
                return;

            IPooledEnumerable eable = target.GetMobilesInRange(15);

            foreach (Mobile m in eable)
            {
                if (m is BaseGuard)
                {
                    BaseGuard g = (BaseGuard)m;

                    if (g.Focus == null) // idling
                    {
                        g.Focus = target;

                        --amount;
                    }
                    else if (g.Focus == target && !onlyAdditional)
                    {
                        --amount;
                    }
                }
            }

            eable.Free();

            while (amount-- > 0)
                caller.Region.MakeGuard(target);
        }

        public override bool OnBeforeDeath()
        {
            Effects.SendLocationParticles(EffectItem.Create(Location, Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 2023);

            PlaySound(0x1FE);

            Delete();

            return false;
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

        // New methods for movement and combat
        protected bool MoveTo(Mobile m, bool run, int range)
        {
            if (!InRange(m, range))
            {
                if (!Move(GetDirectionTo(m))) // Changed to use the single argument version of Move
                {
                    return false;
                }
            }
            return true;
        }

        protected void Run(Direction d)
        {
            if ((Spell != null && Spell.IsCasting) || Paralyzed || Frozen)
            {
                return;
            }

            Direction = d | Direction.Running;

            if (!Move(Direction)) // Changed to use the single argument version of Move
            {
                OnFailedMove();
            }
        }

        protected virtual void OnFailedMove()
        {
            // Handle when movement fails, could involve trying to find a new path or returning to idle behavior
        }

        public virtual void DoSwing(Mobile defender)
        {
            if (Weapon is BaseWeapon weapon)
            {
                weapon.OnSwing(this, defender);
            }
            else
            {
                // If no weapon, do a basic damage calculation
                int damage = Utility.RandomMinMax(5, 15);
                AOS.Damage(defender, this, damage, 100, 0, 0, 0, 0); // 100% physical damage
            }
        }
    }
}