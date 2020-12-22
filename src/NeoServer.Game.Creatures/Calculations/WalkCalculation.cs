using NeoServer.Game.Contracts.World;
using NeoServer.Game.Contracts.World.Tiles;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Creatures.Calculations
{
    public class WalkCalculation
    {
        public static long CalculateEventStepTicks(IDynamicTile tile, long lastStep, uint lastStepCost, int speed)
        {
            var firstStep = lastStep == 0;

            long stepTick = GetWalkDelay(tile, lastStep, lastStepCost, speed);

            if (stepTick <= 0)
            {
                long stepDuration = GetStepDuration(tile, speed);
                if (firstStep && stepDuration > 0)
                {
                    stepTick = 1;
                }
                else
                {
                    stepTick = stepDuration * lastStepCost;
                }
            }
            Console.WriteLine(stepTick);
            return stepTick;
        }

        private static int GetWalkDelay(IDynamicTile tile, long lastStep, uint lastStepCost, int speed)
        {
            if (lastStep == 0)
            {
                return 0;
            }

            long timeTicks = DateTime.Now.Millisecond;
            long stepDuration = GetStepDuration(tile, speed) * lastStepCost;

            return (int)(stepDuration - (timeTicks - TimeSpan.FromTicks(lastStep).Milliseconds));
        }

        private static long GetStepDuration(IDynamicTile tile, int stepSpeed)
        {
            int calculatedStepSpeed;
            int groundSpeed;

            double speedA = 857.36;
            double speedB = 261.29;
            double speedC = -4795.01;

            if (stepSpeed > -speedB)
            {
                calculatedStepSpeed = (int)Math.Floor((speedA * Math.Log((stepSpeed / 2) + speedB) + speedC) + 0.5);

                if (calculatedStepSpeed == 0)
                {
                    calculatedStepSpeed = 1;
                }
            }
            else
            {
                calculatedStepSpeed = 1;
            }

            groundSpeed = tile.StepSpeed == 0 ? 150 : tile.StepSpeed;

            double duration = Math.Floor((double)(1000 * groundSpeed / calculatedStepSpeed));

            long stepDuration = (long)Math.Ceiling(duration / 50) * 50;

            //todo
            //const Monster* monster = getMonster();
            //if (monster && monster->isTargetNearby() && !monster->isFleeing() && !monster->getMaster())
            //{
            //    stepDuration *= 2;
            //}

            return stepDuration;

        }
    }
}
