#region AuthorHeader
//
//	EvoSystem version 2.1, by Xanthos
//
#endregion AuthorHeader

using System;
using Server;
using Server.Mobiles;
using Xanthos.Interfaces;

namespace Xanthos.Evo
{
    public abstract class BaseEvoMount : BaseEvo
    {
        private bool m_IsRiding;
        private int m_MountSpeedBonus;

        [Constructable]
        protected BaseEvoMount(string name, AIType ai, double dActiveSpeed, double dPassiveSpeed = 0.4) 
            : base(name, ai, dActiveSpeed, dPassiveSpeed)
        {
            ControlSlots = 3; // Example: Mounts might take up more control slots
            m_MountSpeedBonus = 10; // Example speed bonus when mounted
            m_IsRiding = false;
        }

        // Serialization constructor - without [Constructable]
        protected BaseEvoMount(Serial serial) : base(serial)
        {
            // No initialization here; everything should be handled in Deserialize
        }

        public virtual bool IsRiding
        {
            get => m_IsRiding;
            set => m_IsRiding = value;
        }

        public virtual int MountSpeedBonus
        {
            get => m_MountSpeedBonus;
            set => m_MountSpeedBonus = value;
        }

        // Example method to handle mounting
        public virtual void OnMount(Mobile rider)
        {
            if (!m_IsRiding)
            {
                m_IsRiding = true;
                // Apply speed bonus or other effects when mounted
            }
        }

        // Example method to handle dismounting
        public virtual void OnDismount()
        {
            if (m_IsRiding)
            {
                m_IsRiding = false;
                // Remove speed bonus or other effects when dismounted
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1); // Version

            writer.Write(m_IsRiding);
            writer.Write(m_MountSpeedBonus);

            // Debug logging
            Console.WriteLine($"Serialized {this.GetType().Name} with serial {Serial.Value}. IsRiding: {m_IsRiding}, MountSpeedBonus: {m_MountSpeedBonus}");
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    m_IsRiding = reader.ReadBool();
                    m_MountSpeedBonus = reader.ReadInt();
                    break;
                case 0:
                    // For backward compatibility, set defaults
                    m_IsRiding = false;
                    m_MountSpeedBonus = 10;
                    break;
            }

            // Debug logging
            Console.WriteLine($"Deserialized {this.GetType().Name} with serial {Serial.Value}. IsRiding: {m_IsRiding}, MountSpeedBonus: {m_MountSpeedBonus}");
        }
    }
}