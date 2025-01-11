using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Xanthos.Interfaces;

namespace Xanthos.Evo
{
    public class RaelisDragonEgg : BaseEvoEgg
    {
        private static readonly int[] DragonHues = new int[] { 1157, 1175, 1172, 1170, 2703, 2473, 2643, 1156, 2704, 2734, 2669, 2621, 2859, 2716, 2791, 2927, 2974, 1161, 2717, 2652, 2821, 2818, 2730, 2670, 2678, 2630, 2641, 2644, 2592, 2543, 2526, 2338, 2339, 1793, 1980, 1983 };

        public override IEvoCreature GetEvoCreature()
        {
            RaelisDragon dragon = new RaelisDragon("a dragon hatchling");
            dragon.Body = 52; // Set the body type to 52 for visibility
            dragon.Hue = DragonHues[Utility.Random(DragonHues.Length)]; // Set a random hue from the predefined list
            return dragon;
        }

        [Constructable]
        public RaelisDragonEgg() : base()
        {
            Name = "a dragon egg";
            ItemID = 0x47E6; // Changed to the new item ID for the egg animation
            HatchDuration = 0.01; // 15 minutes
        }

        public RaelisDragonEgg(Serial serial) : base(serial)
        {
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
        }
    }
}