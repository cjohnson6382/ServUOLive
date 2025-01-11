#region References
using System;

using Server.Items;
#endregion

namespace Server.Mobiles
{
    public class WarriorGuard : BaseGuard
    {
        private Timer m_AttackTimer, m_IdleTimer;
        private Mobile m_Focus;

        [Constructable]
        public WarriorGuard() : this(null)
        { }

        public WarriorGuard(Mobile target) : base(target)
        {
            InitStats(100, 100, 100);
            Title = "the guard";

            SpeechHue = Utility.RandomDyedHue();
            Hue = Utility.RandomSkinHue();

            if (Female = Utility.RandomBool())
            {
                Body = 0x191;
                Name = NameList.RandomName("female");

                switch (Utility.Random(2))
                {
                    case 0: AddItem(new LeatherSkirt()); break;
                    case 1: AddItem(new LeatherShorts()); break;
                }

                switch (Utility.Random(5))
                {
                    case 0: AddItem(new FemaleLeatherChest()); break;
                    case 1: AddItem(new FemaleStuddedChest()); break;
                    case 2: AddItem(new LeatherBustierArms()); break;
                    case 3: AddItem(new StuddedBustierArms()); break;
                    case 4: AddItem(new FemalePlateChest()); break;
                }
            }
            else
            {
                Body = 0x190;
                Name = NameList.RandomName("male");

                AddItem(new PlateChest());
                AddItem(new PlateArms());
                AddItem(new PlateLegs());

                switch (Utility.Random(3))
                {
                    case 0: AddItem(new Doublet(Utility.RandomNondyedHue())); break;
                    case 1: AddItem(new Tunic(Utility.RandomNondyedHue())); break;
                    case 2: AddItem(new BodySash(Utility.RandomNondyedHue())); break;
                }
            }
            Utility.AssignRandomHair(this);

            if (Utility.RandomBool())
            {
                Utility.AssignRandomFacialHair(this, HairHue);
            }

            Halberd weapon = new Halberd();
            weapon.Movable = false;
            weapon.Crafter = this;
            weapon.Quality = ItemQuality.Exceptional;
            AddItem(weapon);

            Container pack = new Backpack();
            pack.Movable = false;
            pack.DropItem(new Gold(10, 25));
            AddItem(pack);

            Skills[SkillName.Anatomy].Base = 120.0;
            Skills[SkillName.Tactics].Base = 120.0;
            Skills[SkillName.Swords].Base = 100.0;
            Skills[SkillName.MagicResist].Base = 120.0;
            Skills[SkillName.DetectHidden].Base = 100.0;

            NextCombatTime = Core.TickCount + 500;
            Focus = target;
        }

        public WarriorGuard(Serial serial) : base(serial)
        { }

        [CommandProperty(AccessLevel.GameMaster)]
        public override Mobile Focus
        {
            get { return m_Focus; }
            set
            {
                if (Deleted)
                {
                    return;
                }

                Mobile oldFocus = m_Focus;

                if (oldFocus != value)
                {
                    m_Focus = value;

                    if (value != null)
                    {
                        // Check if the mobile is a criminal or murderer
                        if (value.Criminal || (value.Kills > 0 && value.Kills >= 5)) // Assuming 5 kills to be considered a murderer
                        {
                            AggressiveAction(value);
                            Combatant = value;

                            if (value != null)
                            {
                                Say(500131); // Thou wilt regret thine actions, swine!
                            }

                            if (m_AttackTimer != null)
                            {
                                m_AttackTimer.Stop();
                                m_AttackTimer = null;
                            }

                            if (m_IdleTimer != null)
                            {
                                m_IdleTimer.Stop();
                                m_IdleTimer = null;
                            }

                            m_AttackTimer = new AttackTimer(this);
                            m_AttackTimer.Start();
                            ((AttackTimer)m_AttackTimer).DoOnTick();
                        }
                        else
                        {
                            // If not criminal or murderer, don't engage
                            Say("You are not my concern.");
                            m_Focus = null;
                        }
                    }
                    else
                    {
                        m_IdleTimer = new IdleTimer(this);
                        m_IdleTimer.Start();
                    }
                }
                else if (m_Focus == null && m_IdleTimer == null)
                {
                    m_IdleTimer = new IdleTimer(this);
                    m_IdleTimer.Start();
                }
            }
        }

        public override bool OnBeforeDeath()
        {
            if (m_Focus != null && m_Focus.Alive)
            {
                new AvengeTimer(m_Focus).Start(); // If a guard dies, three more guards will spawn
            }

            return base.OnBeforeDeath();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version

            writer.Write(m_Focus);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    {
                        m_Focus = reader.ReadMobile();

                        if (m_Focus != null)
                        {
                            m_AttackTimer = new AttackTimer(this);
                            m_AttackTimer.Start();
                        }
                        else
                        {
                            m_IdleTimer = new IdleTimer(this);
                            m_IdleTimer.Start();
                        }

                        break;
                    }
            }
        }

        public override void OnAfterDelete()
        {
            if (m_AttackTimer != null)
            {
                m_AttackTimer.Stop();
                m_AttackTimer = null;
            }

            if (m_IdleTimer != null)
            {
                m_IdleTimer.Stop();
                m_IdleTimer = null;
            }

            base.OnAfterDelete();
        }

        private class AvengeTimer : Timer
        {
            private readonly Mobile m_Focus;

            public AvengeTimer(Mobile focus) : base(TimeSpan.FromSeconds(2.5), TimeSpan.FromSeconds(1.0), 3)
            {
                m_Focus = focus;
            }

            protected override void OnTick()
            {
                Spawn(m_Focus, m_Focus, 1, true);
            }
        }

        private class AttackTimer : Timer
        {
            private readonly WarriorGuard m_Owner;

            public AttackTimer(WarriorGuard owner) : base(TimeSpan.FromSeconds(.25), TimeSpan.FromSeconds(.20))
            {
                m_Owner = owner;
            }

            public void DoOnTick()
            {
                OnTick();
            }

            protected override void OnTick()
            {
                if (m_Owner.Deleted)
                {
                    Stop();
                    return;
                }

                m_Owner.Criminal = false;
                m_Owner.Kills = 0;
                m_Owner.Stam = m_Owner.StamMax;

                Mobile target = m_Owner.Focus;

                if (target != null && (target.Deleted || !target.Alive || !m_Owner.CanBeHarmful(target) || !(target.Criminal || (target.Kills > 0 && target.Kills >= 5))))
                {
                    m_Owner.Focus = null;
                    Stop();
                    return;
                }
                else if (m_Owner.Weapon is Fists)
                {
                    m_Owner.Kill();
                    Stop();
                    return;
                }

                if (target != null && m_Owner.Combatant != target)
                {
                    m_Owner.Combatant = target;
                }

                if (target == null)
                {
                    Stop();
                }
                else
                {
                    // Use movement from BaseGuard
                    if (!m_Owner.InRange(target, m_Owner.RangeFight))
                    {
                        if (!m_Owner.MoveTo(target, true, 1))
                        {
                            m_Owner.OnFailedMove();
                        }
                    }
                    else
                    {
                       // m_Owner.DoHarmful(target);
                       // m_Owner.DoSwing(target); // Use the swing method from BaseGuard
                    }
                }
            }
        }

        private class IdleTimer : Timer
        {
            private readonly WarriorGuard m_Owner;
            private int m_Stage;

            public IdleTimer(WarriorGuard owner) : base(TimeSpan.FromSeconds(2.0), TimeSpan.FromSeconds(2.5))
            {
                m_Owner = owner;
            }

            protected override void OnTick()
            {
                if (m_Owner.Deleted)
                {
                    Stop();
                    return;
                }

                if ((m_Stage++ % 4) == 0 || !m_Owner.Move(m_Owner.Direction))
                {
                    m_Owner.Direction = (Direction)Utility.Random(8);
                }

                if (m_Stage > 16)
                {
                    Effects.SendLocationParticles(
                        EffectItem.Create(m_Owner.Location, m_Owner.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 2023);
                    m_Owner.PlaySound(0x1FE);

                    m_Owner.Delete();
                }
            }
        }
    }
}