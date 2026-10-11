using CrazyUniverse.Core.Interfaces;

namespace CrazyUniverse.Core.Models;

public class GuardBot(string robotId, double currentCharge, double currentDurability, double chargeCountToScan = 30) :
    Robot (robotId, currentCharge, currentDurability),
    IChargeable,
    IScan
{
    private bool _lockMode;

    public void Recharge(double count)
    {
        if (!_lockMode)
        {
            Charge += count;
            if (Charge > 100)
            {
                Charge = 100;
                AddLog($"Robot {RobotId} recharged completely");
            }
            else
                AddLog($"Robot {RobotId} recharged to {count}");
        }
        else
            AddLog($"Robot {RobotId} cannot be charged");
    }

    public (string robotId, double charge, double durability) ScanRobot(Robot robot)
    {
        if (Charge > chargeCountToScan)
        {
            Charge -= chargeCountToScan;

            var (robotId, charge, durability) = (
                robot.RobotId,
                robot.Charge,
                robot.Durability);

            AddLog($"Robot {RobotId} succesfully scaned robot {robot.RobotId}");
            return (robotId, charge, durability);
        }

        AddLog($"Robot {RobotId} has not enough charge for scan");
        throw new Exception();
    }

    public override void CrazyAction()
    {
        _lockMode = !_lockMode;
        AddLog(_lockMode
            ? $"Robot {RobotId} went crazy and locked his charging firmware"
            : $"Robot {RobotId} has calmed down and it can be recharged again");
    }
}