using CBBSimulator.Core.Configuration;
using CBBSimulator.Core.Models;

namespace CBBSimulator.Core.Simulation;

public class PossessionEngine
{
    private readonly SimulationConfig _config;
    private readonly Random _random;

    public PossessionEngine(SimulationConfig config, Random? random = null)
    {
        _config = config;
        _random = random ?? Random.Shared;
    }

    public PossessionResult Run(
        CollegeModel offense,
        CollegeModel defense,
        int scoreDiff,
        int defensesFouls,
        bool isOffensiveRebound,
        bool isHomeTeamOnOffense)
    {
        var result = new PossessionResult();

        // Calculate time used based on tempo
        int tempo = (int)Math.Round(offense.ADJ_T);
        tempo = Math.Max(0, tempo - 60); // Convert to 0-15 range

        int randForTempo = _random.Next(1, tempo + 1);
        int randForTime = _random.Next(0, 3);
        int timeTaken = isOffensiveRebound ? 10 - randForTime - randForTempo : 20 - randForTime - randForTempo;

        if (timeTaken < 1)
            timeTaken = 1;

        result.SecondsUsed = timeTaken;

        // Check for turnover (adjusted by config)
        decimal chanceOfTurnover = (offense.TOR_O + defense.TOR_D) / 3 + _config.AdjusterTurnover;
        int randForTurnover = _random.Next(1, 101);

        if (chanceOfTurnover > randForTurnover)
        {
            result.PointsScored = 0;
            result.OffenseKeepsPossession = false;

            // Determine if turnover was an offensive foul
            int offFoulChance = (int)Math.Round(defense.FTR_O / 2);
            int randForOffFoul = _random.Next(1, 101);
            result.OffensiveFoul = offFoulChance >= randForOffFoul;

            return result;
        }

        // Check for dead ball defensive foul
        int chanceOfDeadballFoul = (int)Math.Round((offense.FTR_O + defense.FTR_D) / 4);
        chanceOfDeadballFoul += _config.AdjusterFoul;
        int randForDeadballFoul = _random.Next(1, 101);

        if (chanceOfDeadballFoul >= randForDeadballFoul)
        {
            result.DefensiveFoul = true;
            result.PointsScored = 0;

            // Check if free throws will be shot (bonus situation)
            if ((defensesFouls + 1) > 6)
            {
                result.OffenseKeepsPossession = false;

                if ((defensesFouls + 1) > 9)
                {
                    // Double bonus: 2 free throws
                    result.PointsScored = EvaluateFreeThrows(offense, 2);
                }
                else
                {
                    // One-and-one
                    int firstFT = _random.Next(1, 101);
                    if (firstFT <= offense.FTP)
                    {
                        // Made first, attempt second
                        result.PointsScored = 1;
                        int secondFT = _random.Next(1, 101);
                        if (secondFT <= offense.FTP)
                        {
                            result.PointsScored = 2;
                        }
                    }
                }
                return result;
            }

            // Foul but no free throws
            result.OffenseKeepsPossession = true;
            return result;
        }

        // Determine shot type (2-pointer or 3-pointer)
        bool is3Pointer = DetermineShotType(scoreDiff);

        // Calculate shooting adjustments
        int adjuster = (int)Math.Round((offense.ADJOE + defense.ADJDE) / 2) - 100;

        if (is3Pointer)
        {
            return EvaluateThreePointShot(offense, defense, adjuster, defensesFouls, isHomeTeamOnOffense);
        }
        else
        {
            return EvaluateTwoPointShot(offense, defense, adjuster, defensesFouls, isHomeTeamOnOffense);
        }
    }

    private bool DetermineShotType(int scoreDiff)
    {
        int randForShotType = _random.Next(1, 101);

        if (scoreDiff > 5)
            return randForShotType <= 30;
        else if (scoreDiff < -5)
            return randForShotType > 40;
        else
            return randForShotType > 35;
    }

    private int EvaluateFreeThrows(CollegeModel shooter, int numShots)
    {
        int madeCount = 0;
        for (int i = 0; i < numShots; i++)
        {
            if (_random.Next(1, 101) <= shooter.FTP)
                madeCount++;
        }
        return madeCount;
    }

    private PossessionResult EvaluateThreePointShot(
        CollegeModel offense,
        CollegeModel defense,
        int adjuster,
        int defensesFouls,
        bool isHomeTeamOnOffense)
    {
        var result = new PossessionResult();

        int percentChance = (int)Math.Round((offense.PT3_O + defense.PT3_D) / 2);
        percentChance += adjuster + _config.Adjuster3PT;

        if (!isHomeTeamOnOffense)
            percentChance += _config.AdjusterHomeTeam;

        int randForShot = _random.Next(1, 101);

        if (randForShot <= percentChance)
        {
            // Made 3-pointer
            result.PointsScored = 3;
            result.OffenseKeepsPossession = false;
            return result;
        }

        // Missed 3-pointer
        int chanceOf3ptFoul = (int)Math.Round((offense.FTR_O + defense.FTR_D) / 8);
        chanceOf3ptFoul += _config.AdjusterFoul;
        int randFor3ptFoul = _random.Next(1, 101);

        if (chanceOf3ptFoul >= randFor3ptFoul)
        {
            // Fouled on 3-pointer (3 free throws)
            result.PointsScored = EvaluateFreeThrows(offense, 3);
            result.OffenseKeepsPossession = false;
            result.DefensiveFoul = true;
            return result;
        }

        // No foul, check for rebounds
        return EvaluateRebounds(offense, defense, adjuster);
    }

    private PossessionResult EvaluateTwoPointShot(
        CollegeModel offense,
        CollegeModel defense,
        int adjuster,
        int defensesFouls,
        bool isHomeTeamOnOffense)
    {
        var result = new PossessionResult();

        int percentChance = (int)Math.Round((offense.PT2_O + defense.PT2_D) / 2);
        percentChance += adjuster + _config.Adjuster2PT;

        if (!isHomeTeamOnOffense)
            percentChance += _config.AdjusterHomeTeam;

        int randForShot = _random.Next(1, 101);

        if (randForShot <= percentChance)
        {
            // Made 2-pointer
            result.PointsScored = 2;
            result.OffenseKeepsPossession = false;
            return result;
        }

        // Missed 2-pointer
        int chanceOf2ptFoul = (int)Math.Round((offense.FTR_O + defense.FTR_D) / 4);
        chanceOf2ptFoul += _config.AdjusterFoul;
        int randFor2ptFoul = _random.Next(1, 101);

        if (chanceOf2ptFoul >= randFor2ptFoul)
        {
            // Fouled on 2-pointer (2 free throws)
            result.PointsScored = EvaluateFreeThrows(offense, 2);
            result.OffenseKeepsPossession = false;
            result.DefensiveFoul = true;
            return result;
        }

        // No foul, check for rebounds
        return EvaluateRebounds(offense, defense, adjuster);
    }

    private PossessionResult EvaluateRebounds(CollegeModel offense, CollegeModel defense, int adjuster)
    {
        var result = new PossessionResult();
        var adjusterDef = (int)Math.Round((defense.ADJOE + offense.ADJDE) / 2) - 100;

        int defReboundChance = (int)Math.Round(defense.DRB) + adjusterDef;
        int randForDefRebound = _random.Next(1, 101);

        if (randForDefRebound <= defReboundChance)
        {
            // Defensive rebound
            result.PointsScored = 0;
            result.OffenseKeepsPossession = false;
            return result;
        }

        // Defensive rebound failed, check for offensive rebound
        int offReboundChance = (int)Math.Round(offense.ORB) + adjuster;
        int randForOffRebound = _random.Next(1, 101);

        if (randForOffRebound <= offReboundChance)
        {
            // Offensive rebound
            result.PointsScored = 0;
            result.OffenseKeepsPossession = true;
            return result;
        }

        // Defensive rebound
        result.PointsScored = 0;
        result.OffenseKeepsPossession = false;
        return result;
    }
}
