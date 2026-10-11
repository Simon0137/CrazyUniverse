using CrazyUniverse.Core.Models;

namespace CrazyUniverse.Core;

public static class RobotFactory
{
    public static CleanerBot AddCleanerBot(string id) => new(id, 100.00D, 70.00D);
    public static ExplorerBot AddExplorerBot(string id) => new ExplorerBot(id, 60.00D, 90.00D, 15.00D);
    public static GuardBot AddGuardBot(string id) => new(id, 80.00D, 80.00D, 35.00D);
    public static RepairBot AddRepairBot(string id) => new(id, 90.00D, 100.00D, 0.40D);
}