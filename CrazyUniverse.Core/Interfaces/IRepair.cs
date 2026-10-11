using CrazyUniverse.Core.Models;

namespace CrazyUniverse.Core.Interfaces;

public interface IRepair
{
    public bool TryRepairRobot(Robot robot, double repairPoints);
}