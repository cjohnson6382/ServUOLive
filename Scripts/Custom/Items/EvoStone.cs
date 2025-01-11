using System;
using Server;
using Server.Items;
using Server.Targeting;
using Server.Mobiles;
using Scripts.Custom.Gumps;
using Xanthos.Evo;

namespace Scripts.Custom.Items
{
    public class EvoStone : Item
    {
        private BaseEvo m_BoundTo;

        [Constructable]
        public EvoStone() : base() // Changed to your specified item ID
        {
            Name = "Evo Stone";
            ItemID = 0x023E; // changed to try and generate the correct image
            Weight = 1.0;
            Hue = 1153; // Keeping the specified hue
            LootType = LootType.Blessed; // Make the item blessed
        }

        public EvoStone(Serial serial) : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            // Console.WriteLine($"EvoStone OnDoubleClick - ItemID: {this.ItemID}, Hue: {this.Hue}"); // Debug logging

            if (m_BoundTo == null)
            {
                from.SendMessage("Select a pet to bind this Evo Stone to.");
                from.Target = new BindTarget(this);
            }
            else
            {
                if (m_BoundTo.Deleted)
                {
                    from.SendMessage("The pet this stone was bound to no longer exists.");
                    m_BoundTo = null;
                    Name = "Evo Stone"; // Reset the name if the pet no longer exists
                }
                else
                {
                    from.SendGump(new RaelisDragonGump((RaelisDragon)m_BoundTo)); // Assuming the Gump is for RaelisDragon; adjust if needed for other BaseEvo types
                }
            }
        }

        private class BindTarget : Target
        {
            private EvoStone m_Stone;

            public BindTarget(EvoStone stone) : base(10, false, TargetFlags.None)
            {
                m_Stone = stone;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is BaseEvo pet && pet.Controlled && pet.ControlMaster == from)
                {
                    m_Stone.m_BoundTo = pet;
                    m_Stone.Name = $"Evo Stone of {pet.Name}"; // Change name to reflect bound pet
                    from.SendMessage($"The Evo Stone has been bound to {pet.Name}.");
                    // Console.WriteLine($"EvoStone Bound - ItemID: {m_Stone.ItemID}, Hue: {m_Stone.Hue}"); // Debug logging
                }
                else
                {
                    from.SendMessage("You must target a pet you own.");
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
            writer.Write(m_BoundTo);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            m_BoundTo = (BaseEvo)reader.ReadMobile();

            // Ensure the item is blessed and update name after deserialization
            LootType = LootType.Blessed;
            // Console.WriteLine($"EvoStone Deserialized - ItemID: {this.ItemID}, Hue: {this.Hue}"); // Debug logging
            if (m_BoundTo != null && !m_BoundTo.Deleted)
            {
                Name = $"Evo Stone of {m_BoundTo.Name}";
            }
            else
            {
                Name = "Evo Stone";
            }
        }
    }
}