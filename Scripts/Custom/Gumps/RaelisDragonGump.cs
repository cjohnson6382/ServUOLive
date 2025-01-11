using System;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Xanthos.Evo;

namespace Scripts.Custom.Gumps
{
    public class RaelisDragonGump : Gump
    {
        private readonly RaelisDragon _dragon;

        public RaelisDragonGump(RaelisDragon dragon) : base(50, 50)
        {
            _dragon = dragon ?? throw new ArgumentNullException(nameof(dragon));
            CompileGump();
        }

        private void CompileGump()
        {
            // Define the gump layout
            AddBackground(0, 0, 250, 150, 0x2454);

            // Get the evolution spec for the dragon
            BaseEvoSpec spec = _dragon.GetEvoSpec();
            if (spec == null || _dragon.Stage > spec.Stages.Length)
            {
                // Console.WriteLine($"Error: Invalid Spec or Stage for Dragon {_dragon.Name}");
                AddLabel(20, 20, 38, "Error: Dragon data not found");
                return;
            }

            BaseEvoStage currentStage = spec.Stages[_dragon.Stage - 1];

            // Dragon's name
            AddLabel(20, 20, 0, _dragon.Name);

            // Stage label
            AddLabel(20, 50, 0, $"Stage {_dragon.Stage}");

            // Evolution progress within current stage
            int currentEP = _dragon.Ep;
            int nextThreshold = currentStage.NextEpThreshold;
            int stageProgress = Math.Max(0, Math.Min(currentEP, nextThreshold));

            // Debug logging to verify EP values
            // Console.WriteLine($"Debug: Current EP = {currentEP}, Next Threshold = {nextThreshold}");

            // Progress bar for current stage
            AddProgressBar(20, 70, 210, 20, stageProgress, nextThreshold);

            // Add label for EP count, ensure it's using the correct values
            AddLabel(20, 95, 0, $"{currentEP}/{nextThreshold} EP");
        }

        private void AddProgressBar(int x, int y, int width, int height, int current, int max)
        {
            int progress = (int)((double)current / max * width);

            // Log values for debugging
            // Console.WriteLine($"Progress calculation: current={current}, max={max}, progress={progress}, width={width}");

            // Draw entire bar background first
            AddBackground(x, y, width, height, 0x242C); // Empty bar background

            // Then draw the progress part on top
            if (progress > 0)
            {
                AddBackground(x, y, progress, height, 0x2486); // Filled bar progress
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            // No interactive elements in this gump, so no action required
        }
    }
}