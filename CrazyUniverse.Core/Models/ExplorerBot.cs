using CrazyUniverse.Core.Interfaces;

namespace CrazyUniverse.Core.Models;

public class ExplorerBot(string robotId, double currentCharge, double currentDurability, double chargeCountToScan = 30) :
    Robot(robotId, currentCharge, currentDurability),
    IChargeable,
    IScan
{
    private bool _randomMode;

    public void Recharge(double count)
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

    public (string robotId, double charge, double durability) ScanRobot(Robot robot)
    {
        if (Charge > chargeCountToScan)
        {
            Charge -= chargeCountToScan;

            string robotId;
            double charge;
            double durability;

            if (_randomMode)
            {
                (robotId, charge, durability) = (
                    GenerateRandomString(),
                    new Random().NextDouble() * 100,
                    new Random().NextDouble() * 100);
                AddLog($"Robot {RobotId} ??? scaned robot {robot.RobotId}");
            }
            else
            {
                (robotId, charge, durability) = (
                    robot.RobotId,
                    robot.Charge + (new Random().NextDouble() * 2 - 1) * 2,
                    robot.Durability + (new Random().NextDouble() * 2 - 1) * 2);
                AddLog($"Robot {RobotId} succesfully scaned robot {robot.RobotId} with slight errors");
            }

            return (robotId, charge, durability);
        }

        AddLog($"Robot {RobotId} has not enough charge for scan");
        throw new Exception();
    }

    public override void CrazyAction()
    {
        _randomMode = !_randomMode;
        AddLog(_randomMode
            ? $"Robot {RobotId} went crazy and returns random scanning result"
            : $"Robot {RobotId} has calmed down and is now returning normal scan results again");
    }

    private static string GenerateRandomString()
    {
        var random = Random.Shared;

        var length = random.Next(3, 13);
        var chars = new char[length];

        for (var i = 0; i < length; i++)
        {
            chars[i] = (char)random.Next(33, 127);
        }

        return new string(chars);
    }
}