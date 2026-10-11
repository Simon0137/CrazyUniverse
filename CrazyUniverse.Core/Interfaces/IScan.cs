using CrazyUniverse.Core.Models;

namespace CrazyUniverse.Core.Interfaces;

public interface IScan
{
    public (string robotId, double charge, double durability) ScanRobot(Robot robot);
}