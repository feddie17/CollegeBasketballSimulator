using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static CollegeBasketballSimulator.DataModels;

namespace CollegeBasketballSimulator
{
    public enum GameSpeed
    {
        Instant = 0,
        Fast = 1,
        Medium = 2,
        Slow = 3
    }
    public class DataController
    {
        public static int adjuster2PT { get; set; }
        public static int adjuster3PT { get; set; }
        public static int adjusterTurnover { get; set; }
        public static int adjusterFoul { get; set; }
        public static int adjusterHomeTeam { get; set; }
        public static List<CollegeModel> GlobalCollegeData { get; set; }
        public static List<CollegeModel> NewGlobalCollegeData { get; set; }


        public static void RunProgram()
        {
            Console.WriteLine("Loading Program Data...");
            //initialize the global college data
            GlobalCollegeData = ScrapeLive2025Data().Result;
            NewGlobalCollegeData = Simulator2026.ScrapeData2026();
            Console.WriteLine("--------------------------------------------------------------------------------------------------------------");
            Console.WriteLine("-College basketball simulator for 2023-2024 season.");
            Console.WriteLine("-Work in progress, some features are not yet implemented: ");
            Console.WriteLine("-Overtime: not yet implemented, the first team entered in the matchup is current just given the win if the game ends in a tie.");
            Console.WriteLine("-Offensive rebounding on free throws: currently the defense always gets the rebound off a missed free throw.");
            Console.WriteLine("-Free throw percentages: currently only Big 12 and march madness teams have free throw percentages in the .csv file.  All other teams use a default value.");
            Console.WriteLine("-End game situations: teams do not handle end game situations effeciently (fouling if losing, shooting the correct shot based on the score, etc.). ");
            Console.WriteLine("the only current team strategy is a higher chance of shooting a 3 pointer vs a 2 pointer if the team is losing. ");
            Console.WriteLine("in the future I'm thinking about adding a coach/gameplan that can be linked to the team, so each posession the game status can be passed to the coach and the desired game plan can be returned. ");
            Console.WriteLine("");
            Console.WriteLine("-Before running a simulation, be sure to set the correct path to the cbb24.csv file relative to your machine.  That path can be found in DataController.cs. ");
            Console.WriteLine("-The team name you enter in a individual game simulation must be an exact match of the name in the .csv file.  Open the .csv to see the full list of names. ");
            Console.WriteLine("for quick reference, a list of every march madness team name can be found in DataController.cs. ");
            Console.WriteLine("-V1.2 New features: ");
            Console.WriteLine("Live 2025 data scraped from the web in real time.");
            Console.WriteLine("Multi-matchup mode: pick two teams and have them play each other multiple times, and average the results.");
            Console.WriteLine("Big 12 simulation mode: multi-matchup every game this Big 12 season (through week 3)");
            Console.WriteLine("-V1.3 New Features: ");
            Console.WriteLine("In-depth statistics for Big 12 Simulations. ");
            Console.WriteLine("Adjuster added to boost home team statistics. ");
            Console.WriteLine("--------------------------------------------------------------------------------------------------------------");
            Console.WriteLine("");

            

            while (true)
            {
                int adjuster2pt = -12;
                int adjuster3pt = -12;
                int adjusterTurnover = 0;
                int adjusterFoul = -5;
                bool loop1 = true;
                int mode = 0;
                while (loop1)
                {
                    string input1;
                    Console.WriteLine("Enter:");
                    //Console.WriteLine("'g' to simulate an individual game");
                    //Console.WriteLine("'b' to simulate the march madness bracket");
                    Console.WriteLine("'n' to simulate a game with live 2026 data");
                    Console.WriteLine("'m' to simulate multi matchup between 2 teams (2026)");
                    //Console.WriteLine("'s' to simulate the Big 12 Season so far (2025)");
                    //Console.WriteLine("'stats' to calculate prediction statistics");
                    //Console.WriteLine("'mm' to simulate the current 2025 March Madness bracket");
                    //Console.WriteLine("'mmm' to simulate multiple March Madness brackets.");
                    Console.WriteLine("'ns' to simulate new Big 12 Season (2026)");
                    Console.WriteLine("'es' to simulate the entire men's college basketball schedule (2026)");
                    Console.WriteLine("'q' to Quit");
                    input1 = Console.ReadLine();
                    if(input1 == "g")
                    {
                        loop1 = false;
                        mode = 1;
                    }
                    else if(input1 == "b")
                    {
                        loop1 = false;
                        mode = 0;
                    }
                    else if(input1 == "n")
                    {
                        loop1 = false;
                        mode = 2;
                        
                    }
                    else if(input1 == "m")
                    {
                        loop1 = false;
                        mode = 3;
                    }
                    else if(input1 == "s")
                    {
                        loop1 = false;
                        mode = 4;
                    }
                    else if(input1 == "stats")
                    {
                        loop1 = false;
                        mode = 5;
                    }
                    else if(input1 == "mm")
                    {
                        loop1 = false;
                        mode = 6;
                    }
                    else if(input1 == "mmm")
                    {
                        loop1 = false;
                        mode = 7;
                    }
                    else if (input1 == "ns")
                    {
                        loop1 = false;
                        mode = 8;
                    }
                    else if(input1 == "es")
                    {
                        loop1 = false;
                        mode = 9;
                    }
                    else if (input1 == "q")
                    {
                        Environment.Exit(0);
                    }
                    else
                    {
                        Console.WriteLine("Invalid input, please try again. ");
                    }
                }

                if (mode == 0)
                {
                    //tournament
                    Console.WriteLine("");
                    Console.WriteLine("--------------------------------------------------------------------------------------------------------------");
                    Console.WriteLine("March Madness Bracket Simulator");
                    Console.WriteLine("This will simulate the entire bracket, one round at a time.");
                    Console.WriteLine("Correct picks will have an X by their name in the parentheses.");
                    Console.WriteLine("The score of each round will be displayed at the end of the round, ");
                    Console.WriteLine("and the final score of the bracket will be displayed at the end.");
                    Console.WriteLine("--------------------------------------------------------------------------------------------------------------");
                    Console.WriteLine("");
                    bool loop2 = true;
                    while (loop2)
                    {
                        string input2;
                        Console.WriteLine("Enter 'go' to start the simulation: ");
                        input2 = Console.ReadLine();
                        if (input2 == "go")
                        {
                            loop2 = false;
                        }
                        else
                        {
                            Console.WriteLine("Invalid input, please try again. ");
                        }
                    }
                    DataController.SimulateMarchMadness(adjuster2pt, adjuster3pt, adjusterTurnover, adjusterFoul);
                }
                else if (mode == 1)
                {
                    //individual game
                    Console.WriteLine("");
                    Console.WriteLine("--------------------------------------------------------------------------------------------------------------");
                    Console.WriteLine("Individual Game Simulator");
                    Console.WriteLine("A timestamp will be displayed for each action of the game.");
                    Console.WriteLine("The timestamp consists of the time left in the half, the game score, ");
                    Console.WriteLine("the fouls for each team (in parentheses), and a description of the action.");
                    Console.WriteLine("--------------------------------------------------------------------------------------------------------------");
                    Console.WriteLine("");
                    string team1;
                    string team2;
                    Console.WriteLine("Enter Away Team Name: ");
                    team1 = Console.ReadLine();
                    Console.WriteLine("Enter Home Team Name: ");
                    team2 = Console.ReadLine();
                    bool loop3 = true;
                    GameSpeed simSpeed = GameSpeed.Instant;
                    while (loop3)
                    {
                        string input3;
                        Console.WriteLine("Enter simulation speed: 's' for slow, 'm' for medium, 'f' for fast: ");
                        input3 = Console.ReadLine();
                        if (input3 == "s")
                        {
                            loop3 = false;
                            simSpeed = GameSpeed.Slow;
                        }
                        else if (input3 == "m")
                        {
                            loop3 = false;
                            simSpeed = GameSpeed.Medium;
                        }
                        else if (input3 == "f")
                        {
                            loop3 = false;
                            simSpeed = GameSpeed.Fast;
                        }
                        else
                        {
                            Console.WriteLine("Invalid input, please try again. ");
                        }
                    }
                    RunMatchup(team1, team2, adjuster2pt, adjuster3pt, adjusterTurnover, adjusterFoul, simSpeed, "2024", false);
                }
                else if (mode == 2)
                {
                    //2026 matchup mode
                    Console.WriteLine("");
                    Console.WriteLine("--------------------------------------------------------------------------------------------------------------");
                    Console.WriteLine("2026 Mode");
                    Console.WriteLine("--------------------------------------------------------------------------------------------------------------");
                    Console.WriteLine("");
                    string team1;
                    string team2;
                    Console.WriteLine("Enter Away Team Name: ");
                    team1 = Console.ReadLine();
                    Console.WriteLine("Enter Home Team Name: ");
                    team2 = Console.ReadLine();
                    bool loop3 = true;
                    GameSpeed simSpeed = GameSpeed.Instant;
                    while (loop3)
                    {
                        string input3;
                        Console.WriteLine("Enter simulation speed: 's' for slow, 'm' for medium, 'f' for fast: ");
                        input3 = Console.ReadLine();
                        if (input3 == "s")
                        {
                            loop3 = false;
                            simSpeed = GameSpeed.Slow;
                        }
                        else if (input3 == "m")
                        {
                            loop3 = false;
                            simSpeed = GameSpeed.Medium;
                        }
                        else if (input3 == "f")
                        {
                            loop3 = false;
                            simSpeed = GameSpeed.Fast;
                        }
                        else
                        {
                            Console.WriteLine("Invalid input, please try again. ");
                        }
                    }
                    bool loop4 = true;
                    bool neutralGame = false;
                    while (loop4)
                    {
                        string input4;
                        Console.WriteLine("Neutral game?: ");
                        input4 = Console.ReadLine();
                        if (input4 == "y")
                        {
                            loop4 = false;
                            neutralGame = true;
                        }
                        else if (input4 == "n")
                        {
                            loop4 = false;
                            neutralGame = false;
                        }
                        else
                        {
                            Console.WriteLine("Invalid input, please try again. ");
                        }
                    }
                    RunMatchup(team1, team2, adjuster2pt, adjuster3pt, adjusterTurnover, adjusterFoul, simSpeed, "2026", neutralGame);
                }
                else if (mode == 3)
                {
                    //2026 multi matchup mode
                    Console.WriteLine("");
                    Console.WriteLine("--------------------------------------------------------------------------------------------------------------");
                    Console.WriteLine("2025 Multi Matchup Mode");
                    Console.WriteLine("--------------------------------------------------------------------------------------------------------------");
                    Console.WriteLine("");
                    string team1;
                    string team2;
                    Console.WriteLine("Enter Away Team Name: ");
                    team1 = Console.ReadLine();
                    Console.WriteLine("Enter Home Team Name: ");
                    team2 = Console.ReadLine();
                    bool loop3 = true;
                    int times = 0;
                    while (loop3)
                    {
                        string input3;
                        Console.WriteLine("Enter number of times to run matchup: ");
                        input3 = Console.ReadLine();
                        try
                        {
                            times = Convert.ToInt32(input3);
                            loop3 = false;
                        }
                        catch
                        {

                        }
                    }
                    RunMultiMatchup(team1, team2, times);
                }
                else if (mode == 4)
                {
                    //2025 big 12 season mode
                    Console.WriteLine("");
                    Console.WriteLine("--------------------------------------------------------------------------------------------------------------");
                    Console.WriteLine("2025 Big 12 Season Mode");
                    Console.WriteLine("--------------------------------------------------------------------------------------------------------------");
                    Console.WriteLine("");
                    bool loop3 = true;
                    int times = 0;
                    while (loop3)
                    {
                        string input3;
                        Console.WriteLine("Enter number of times to run each matchup: ");
                        input3 = Console.ReadLine();
                        try
                        {
                            times = Convert.ToInt32(input3);
                            loop3 = false;
                        }
                        catch
                        {

                        }
                    }
                    RunBig12Season(times, adjuster2pt, adjuster3pt, adjusterTurnover, adjusterFoul);
                }
                else if (mode == 5)
                {
                    //stats calculation mode
                    Console.WriteLine("");
                    Console.WriteLine("--------------------------------------------------------------------------------------------------------------");
                    Console.WriteLine("Stats Calculation Mode");
                    Console.WriteLine("--------------------------------------------------------------------------------------------------------------");
                    Console.WriteLine("");

                    int r1;
                    int r2;
                    int r3;
                    int r4;
                    int r5;
                    int r6;
                    int r7;
                    int r8;
                    int r9;
                    int r10;
                    int r11;
                    int r12;
                    int r13;
                    int r14;
                    int r15;
                    int r16;
                    int p1;
                    int p2;
                    int p3;
                    int p4;
                    int p5;
                    int p6;
                    int p7;
                    int p8;
                    int p9;
                    int p10;
                    int p11;
                    int p12;
                    int p13;
                    int p14;
                    int p15;
                    int p16;

                    Console.WriteLine("Prediction Score 1?:");
                    p1 = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Prediction Score 2?:");
                    p2 = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Prediction Score 3?:");
                    p3 = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Prediction Score 4?:");
                    p4 = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Prediction Score 5?:");
                    p5 = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Prediction Score 6?:");
                    p6 = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Prediction Score 7?:");
                    p7 = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Prediction Score 8?:");
                    p8 = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Prediction Score 9?:");
                    p9 = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Prediction Score 10?:");
                    p10 = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Prediction Score 11?:");
                    p11 = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Prediction Score 12?:");
                    p12 = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Prediction Score 13?:");
                    p13 = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Prediction Score 14?:");
                    p14 = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Prediction Score 15?:");
                    p15 = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Prediction Score 16?:");
                    p16 = Convert.ToInt32(Console.ReadLine());

                    Console.WriteLine("Real Score 1?:");
                    r1 = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Real Score 2?:");
                    r2 = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Real Score 3?:");
                    r3 = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Real Score 4?:");
                    r4 = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Real Score 5?:");
                    r5 = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Real Score 6?:");
                    r6 = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Real Score 7?:");
                    r7 = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Real Score 8?:");
                    r8 = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Real Score 9?:");
                    r9 = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Real Score 10?:");
                    r10 = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Real Score 11?:");
                    r11 = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Real Score 12?:");
                    r12 = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Real Score 13?:");
                    r13 = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Real Score 14?:");
                    r14 = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Real Score 15?:");
                    r15 = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Real Score 16?:");
                    r16 = Convert.ToInt32(Console.ReadLine());

                    int t1 = p1 - r1;
                    int t2 = p2 - r2;
                    int t3 = p3 - r3;
                    int t4 = p4 - r4;
                    int t5 = p5 - r5;
                    int t6 = p6 - r6;
                    int t7 = p7 - r7;
                    int t8 = p8 - r8;
                    int t9 = p9 - r9;
                    int t10 = p10 - r10;
                    int t11 = p11 - r11;
                    int t12 = p12 - r12;
                    int t13 = p13 - r13;
                    int t14 = p14 - r14;
                    int t15 = p15 - r15;
                    int t16 = p16 - r16;

                    int plusMinusNet = t1 + t2 + t3 + t4 + t5 + t6 + t7 + t8 + t9 + t10 + t11 + t12 + t13 + t14 + t15 + t16;
                    int plusMinusTotal = Math.Abs(t1) + Math.Abs(t2) + Math.Abs(t3) + Math.Abs(t4) + Math.Abs(t5) + Math.Abs(t6) + Math.Abs(t7) + Math.Abs(t8) + Math.Abs(t9) + Math.Abs(t10) + Math.Abs(t11) + Math.Abs(t12) + Math.Abs(t13) + Math.Abs(t14) + Math.Abs(t15) + Math.Abs(t16);
                    string plusMinusNetAvg = Math.Round(Decimal.Divide(plusMinusNet, 16), 2).ToString();
                    string plusMinusTotalAvg = Math.Round(Decimal.Divide(plusMinusTotal, 16), 2).ToString();

                    int m1 = Math.Abs((p1 - p2) - (r1 - r2));
                    int m2 = Math.Abs((p3 - p4) - (r3 - r4));
                    int m3 = Math.Abs((p5 - p6) - (r5 - r6));
                    int m4 = Math.Abs((p7 - p7) - (r7 - r8));
                    int m5 = Math.Abs((p9 - p10) - (r9 - r10));
                    int m6 = Math.Abs((p11 - p12) - (r11 - r12));
                    int m7 = Math.Abs((p13 - p14) - (r13 - r14));
                    int m8 = Math.Abs((p15 - p16) - (r15 - r16));

                    int margOfVic = m1 + m2 + m3 + m4 + m5 + m6 + m7 + m8;
                    string margOfVicAvg = Math.Round(Decimal.Divide(margOfVic, 8), 2).ToString();

                    Console.WriteLine("-------------------------------------------------");
                    Console.WriteLine("Statistic Results: ");
                    Console.WriteLine("Margin of Victory Average: " + margOfVicAvg);
                    Console.WriteLine("Net +/- Average: " + plusMinusNetAvg);
                    Console.WriteLine("Total +/- Average: " + plusMinusTotalAvg);
                    Console.WriteLine("-------------------------------------------------");
                }
                else if (mode == 6)
                {
                    //2025 March Madness mode
                    int times = 1000;
                    int waitTime = 0;
                    string input3;
                    string input4;
                    Console.WriteLine("Enter number of times to run each matchup of the tournament: ");
                    input3 = Console.ReadLine();
                    Console.WriteLine("Enter wait time between games: ");
                    input4 = Console.ReadLine();
                    try
                    {
                        times = Convert.ToInt32(input3);
                        waitTime = Convert.ToInt32(input4);
                    }
                    catch
                    {

                    }
                    MarchMadnessController.SimulateMarchMadness2025(times, waitTime);
                }
                else if(mode == 7)
                {
                    //march madness multi mode
                    string input1;
                    Console.WriteLine("Enter how many times you would like to run the tournament: ");
                    input1 = Console.ReadLine();
                    int input1Int = Convert.ToInt32(input1);
                  
                    MarchMadnessController.SimulateMarchMadness2025Aggregate(input1Int);
                }
                else if(mode == 8)
                {
                    Simulator2026.SimulateBig12Season();
                }
                else if(mode == 9)
                {
                    Simulator2026.SimulateFullSchedule();
                }
            }
            
            
        }

        public static MatchupResult RunMatchup(string team1, string team2, int a2pt, int a3pt, int aturn, int afoul, GameSpeed simSpeed, string year, bool neutralGame)
        {
            //set the adjusters
            adjuster2PT = a2pt;
            adjuster3PT = a3pt;
            adjusterTurnover = aturn;
            adjusterFoul = afoul;
            adjusterHomeTeam = 3;
            //no home team advantage if neutral game
            if(neutralGame == true)
            {
                adjusterHomeTeam = 0;
            }

            int waitTime = 0;
            if(simSpeed == GameSpeed.Fast)
            {
                waitTime = 1;
            }
            else if(simSpeed == GameSpeed.Medium)
            {
                waitTime = 500;
            }
            else if(simSpeed == GameSpeed.Slow)
            {
                waitTime = 2000;
            }

            int team1Score = 0;
            int team2Score = 0;
            int team1Fouls = 0;
            int team2Fouls = 0;
            int firstHalfTimeLeft = 1200;
            int secondHalfTimeLeft = 1200;
            bool team1HasBall = true;
            List<CollegeModel> data = new List<CollegeModel>();
            if(year == "2025")
            {
                data = GlobalCollegeData;
            }
            else if(year == "2026")
            {
                data = NewGlobalCollegeData;
            }
            else
            {
                data = GetCollegeModelData();
            }
            
            CollegeModel team1Model = data.Where(x => x.Name == team1).FirstOrDefault();
            CollegeModel team2Model = data.Where(x => x.Name == team2).FirstOrDefault();
            if (team1Model == null || team2Model == null)
            {
                Console.WriteLine("Invalid team name: " + team1 + " or " + team2);
                return new MatchupResult();
            }
            
            UpdateFreeThrow(team1Model);
            UpdateFreeThrow(team2Model);

            //print pregame stats
            if(waitTime > 0)
            {
                int team1Adjuster = (int)Math.Round((team1Model.ADJOE + team2Model.ADJDE) / 2);
                team1Adjuster -= 100;
                int team2Adjuster = (int)Math.Round((team2Model.ADJOE + team1Model.ADJDE) / 2);
                team2Adjuster -= 100;
                int team1_3pt = (int)Math.Round((team1Model.PT3_O + team2Model.PT3_D) / 2);
                team1_3pt += team1Adjuster;
                team1_3pt += adjuster3PT;
                int team1_2pt = (int)Math.Round((team1Model.PT2_O + team2Model.PT2_D) / 2);
                team1_2pt += team1Adjuster;
                team1_2pt += adjuster2PT;
                int team2_3pt = (int)Math.Round((team2Model.PT3_O + team1Model.PT3_D) / 2);
                team2_3pt += team2Adjuster;
                team2_3pt += adjuster3PT;
                team2_3pt += adjusterHomeTeam;
                int team2_2pt = (int)Math.Round((team2Model.PT2_O + team1Model.PT2_D) / 2);
                team2_2pt += team2Adjuster;
                team2_2pt += adjuster2PT;
                team2_2pt += adjusterHomeTeam;
                int team1_defReb = (int)Math.Round(team1Model.DRB);
                team1_defReb += team1Adjuster;
                int team1_offReb = (int)Math.Round(team1Model.ORB);
                team1_offReb += team1Adjuster;
                int team2_defReb = (int)Math.Round(team2Model.DRB);
                team2_defReb += team2Adjuster;
                int team2_offReb = (int)Math.Round(team2Model.ORB);
                team2_offReb += team2Adjuster;
                int team1_turn = (int)Math.Round((team1Model.TOR_O + team2Model.TOR_D) / 2);
                team1_turn += adjusterTurnover;
                int team2_turn = (int)Math.Round((team2Model.TOR_O + team1Model.TOR_D) / 2);
                team2_turn += adjusterTurnover;

                int team1_ftp = (int)Math.Round(team1Model.FTP);
                int team2_ftp = (int)Math.Round(team2Model.FTP);

                string team1String = team1 + ":";
                string team2String = team2 + ":";
                int team1Padding = 20 - team1.Length;
                for (int i = 0; i < team1Padding; i++)
                {
                    team1String += " ";
                }
                int team2padding = 20 - team2.Length;
                for (int i = 0; i < team2padding; i++)
                {
                    team2String += " ";
                }

                team1String += "2pt: " + team1_2pt + "%, 3pt: " + team1_3pt + "%, FT: " + team1_ftp + "%, Def Reb: " + team1_defReb + "%, Off Reb: " + team1_offReb + "%, Turnovers: " + team1_turn + "%";
                team2String += "2pt: " + team2_2pt + "%, 3pt: " + team2_3pt + "%, FT: " + team2_ftp + "%, Def Reb: " + team2_defReb + "%, Off Reb: " + team2_offReb + "%, Turnovers: " + team2_turn + "%";
                Console.WriteLine("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
                Console.WriteLine("Pregame Stats: ");
                Console.WriteLine("(These are the team stats for this game.  Stats are automatically adjusted per game for each team based on the relative strength of the other team.  The home team also gets a stat boost.)");
                Console.WriteLine(team1String);
                Console.WriteLine(team2String);
                Console.WriteLine("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
            }
            
            if(waitTime > 0)
            {
                System.Threading.Thread.Sleep(waitTime * 10);
            }
            




            //run first half
            while (firstHalfTimeLeft > 0)
            {
                bool shortPosession = false;
                if (team1HasBall == true)
                {
                    PosessionResult res = RunPossession(team1Model, team2Model, team1Score, team2Score, firstHalfTimeLeft, team1HasBall, shortPosession, team2Fouls, team1Fouls, waitTime);
                    team1Score += res.PointsScored;
                    firstHalfTimeLeft -= res.SecondsUsed;
                    if (res.OffenseKeepsPosession == false)
                    {
                        team1HasBall = !team1HasBall;
                        shortPosession = false;
                    }
                    else
                    {
                        shortPosession = true;
                    }

                    if (res.OffensiveFoul == true)
                    {
                        team1Fouls++;
                    }
                    if (res.DefensiveFoul == true)
                    {
                        team2Fouls++;
                    }
                }
                else
                {
                    PosessionResult res = RunPossession(team2Model, team1Model, team2Score, team1Score, firstHalfTimeLeft, team1HasBall, shortPosession, team1Fouls, team2Fouls, waitTime);
                    team2Score += res.PointsScored;
                    firstHalfTimeLeft -= res.SecondsUsed;
                    if (res.OffenseKeepsPosession == false)
                    {
                        team1HasBall = !team1HasBall;
                        shortPosession = false;
                    }
                    else
                    {
                        shortPosession = true;
                    }

                    if (res.OffensiveFoul == true)
                    {
                        team2Fouls++;
                    }
                    if (res.DefensiveFoul == true)
                    {
                        team1Fouls++;
                    }
                }
                //wait a bit
                if(waitTime > 0)
                {
                    System.Threading.Thread.Sleep(waitTime);
                }
                

            }

            //print halftime score
            if(waitTime > 0)
            {
                Console.WriteLine("#########################################");
                Console.WriteLine("HALF TIME SCORE: " + team1 + " " + team1Score.ToString() + "-" + team2Score.ToString() + " " + team2);
                Console.WriteLine("#########################################");
                if(waitTime > 0)
                {
                    System.Threading.Thread.Sleep(waitTime * 5);
                }
                
            }
            

            //reset fouls
            team1Fouls = 0;
            team2Fouls = 0;

            //run second half
            while (secondHalfTimeLeft > 0)
            {
                bool shortPosession = false;
                if (team1HasBall == true)
                {
                    PosessionResult res = RunPossession(team1Model, team2Model, team1Score, team2Score, secondHalfTimeLeft, team1HasBall, shortPosession, team2Fouls, team1Fouls, waitTime);
                    team1Score += res.PointsScored;
                    secondHalfTimeLeft -= res.SecondsUsed;
                    if (res.OffenseKeepsPosession == false)
                    {
                        team1HasBall = !team1HasBall;
                        shortPosession = false;
                    }
                    else
                    {
                        shortPosession = true;
                    }

                    if (res.OffensiveFoul == true)
                    {
                        team1Fouls++;
                    }
                    if (res.DefensiveFoul == true)
                    {
                        team2Fouls++;
                    }
                }
                else
                {
                    PosessionResult res = RunPossession(team2Model, team1Model, team2Score, team1Score, secondHalfTimeLeft, team1HasBall, shortPosession, team1Fouls, team2Fouls, waitTime);
                    team2Score += res.PointsScored;
                    secondHalfTimeLeft -= res.SecondsUsed;
                    if (res.OffenseKeepsPosession == false)
                    {
                        team1HasBall = !team1HasBall;
                        shortPosession = false;
                    }
                    else
                    {
                        shortPosession = true;
                    }

                    if (res.OffensiveFoul == true)
                    {
                        team2Fouls++;
                    }
                    if (res.DefensiveFoul == true)
                    {
                        team1Fouls++;
                    }

                }
                //wait a bit
                if(waitTime > 0)
                {
                    System.Threading.Thread.Sleep(waitTime);
                }
                

            }

            int overtimeCounter = 0;
            //check for overtime
            if(team1Score == team2Score)
            {
                //overtime
                while(team1Score == team2Score)
                {
                    int overtimeTimeLeft = 300;
                    overtimeCounter++;
                    if(overtimeCounter == 1)
                    {
                        if (waitTime > 0)
                        {
                            Console.WriteLine("#########################################");
                            Console.WriteLine("Final After Regulation: " + team1 + " " + team1Score.ToString() + "-" + team2Score.ToString() + " " + team2);
                            Console.WriteLine("#########################################");
                            if(waitTime > 0)
                            {
                                System.Threading.Thread.Sleep(waitTime * 5);
                            }
                            
                        }
                    }
                    else
                    {
                        if (waitTime > 0)
                        {
                            Console.WriteLine("#########################################");
                            Console.WriteLine("Final After Overtime " + (overtimeCounter - 1).ToString() + ": " + team1 + " " + team1Score.ToString() + "-" + team2Score.ToString() + " " + team2);
                            Console.WriteLine("#########################################");
                            if(waitTime > 0)
                            {
                                System.Threading.Thread.Sleep(waitTime * 5);
                            }
                            
                        }
                    }
                    while (overtimeTimeLeft > 0)
                    {
                        bool shortPosession = false;
                        if (team1HasBall == true)
                        {
                            PosessionResult res = RunPossession(team1Model, team2Model, team1Score, team2Score, overtimeTimeLeft, team1HasBall, shortPosession, team2Fouls, team1Fouls, waitTime);
                            team1Score += res.PointsScored;
                            overtimeTimeLeft -= res.SecondsUsed;
                            if (res.OffenseKeepsPosession == false)
                            {
                                team1HasBall = !team1HasBall;
                                shortPosession = false;
                            }
                            else
                            {
                                shortPosession = true;
                            }

                            if (res.OffensiveFoul == true)
                            {
                                team1Fouls++;
                            }
                            if (res.DefensiveFoul == true)
                            {
                                team2Fouls++;
                            }
                        }
                        else
                        {
                            PosessionResult res = RunPossession(team2Model, team1Model, team2Score, team1Score, overtimeTimeLeft, team1HasBall, shortPosession, team1Fouls, team2Fouls, waitTime);
                            team2Score += res.PointsScored;
                            overtimeTimeLeft -= res.SecondsUsed;
                            if (res.OffenseKeepsPosession == false)
                            {
                                team1HasBall = !team1HasBall;
                                shortPosession = false;
                            }
                            else
                            {
                                shortPosession = true;
                            }

                            if (res.OffensiveFoul == true)
                            {
                                team2Fouls++;
                            }
                            if (res.DefensiveFoul == true)
                            {
                                team1Fouls++;
                            }

                        }
                        //wait a bit
                        if(waitTime > 0)
                        {
                            System.Threading.Thread.Sleep(waitTime);
                        }
                    }
                }
            }


            //print final score
            if(waitTime > 0)
            {
                string overtimeString = "";
                if(overtimeCounter > 0)
                {
                    if(overtimeCounter == 1)
                    {
                        overtimeString = "(OT) ";
                    }
                    else
                    {
                        overtimeString = "(" + overtimeCounter.ToString() + "OT)";
                    }
                }
                Console.WriteLine("#########################################");
                Console.WriteLine("FINAL SCORE: " + overtimeString + team1 + " " + team1Score.ToString() + "-" + team2Score.ToString() + " " + team2);
                Console.WriteLine("#########################################");
                if(waitTime > 0)
                {
                    System.Threading.Thread.Sleep(waitTime * 5);
                }  
            }

            MatchupResult finalRes = new MatchupResult();
            finalRes.AwayTeam = team1;
            finalRes.AwayTeamScore = team1Score;
            finalRes.HomeTeam = team2;
            finalRes.HomeTeamScore = team2Score;
            if(team1Score > team2Score)
            {
                finalRes.Winner = team1;
                finalRes.WinnerScore = team1Score;
                finalRes.Loser = team2;
                finalRes.LoserScore = team2Score;
            }
            else
            {
                finalRes.Winner = team2;
                finalRes.WinnerScore = team2Score;
                finalRes.Loser = team1;
                finalRes.LoserScore = team1Score;
            }
            finalRes.Overtimes = overtimeCounter;
            return finalRes;
            
        }

        public static PosessionResult RunPossession(CollegeModel team1, CollegeModel team2, int team1Score, int team2Score, int secsLeft, bool team1HasBall, bool shortPosession, int defensiveFouls, int offensiveFouls, int waitTime)
        {
            //first init the result model
            PosessionResult res = new PosessionResult();
            //init the random
            Random r = new Random();

            //------------------------------------------------------------ determine how much time has gone off the clock --------------------------------------------------------------------
            //team 1 is on offense, so get their tempo as integer, going to be between 60 and 75
            int tempo = (int)Math.Round(team1.ADJ_T);
            //convert number to 0-15
            tempo = tempo - 60;
            if(tempo < 0)
            {
                tempo = 0;
            }

            //shortest possession should be like 5 seconds, so random should start at 20
            int randForTempo = r.Next(1, tempo + 1);
            int randForTime = r.Next(0, 3);
            int timeTaken = 0;
            if (shortPosession == false)
            {
                timeTaken = 20 - randForTime - randForTempo;
            }
            else
            {
                timeTaken = 10 - randForTime - randForTempo;
                if(timeTaken < 1)
                {
                    timeTaken = 1;
                }

            }



            res.SecondsUsed = timeTaken;
            string timestamp = GetTimeStamp(secsLeft - timeTaken);

            //------------------------------------------------------------- determine if a turnover happened ---------------------------------------------------------------------------------
            //chance of turnover = average of offense turnover % - average of defense turnover %, divided by 3
            decimal chanceOfTurnover = ((team1.TOR_O + team2.TOR_D) / 3);
            decimal randForTurnover = r.Next(1, 101);
            if (chanceOfTurnover > randForTurnover)
            {
                //a turnover happened.
                res.PointsScored = 0;
                res.OffenseKeepsPosession = false;

                //determine if the turnover was an offensive foul
                //determine the odds by half of the defensive team's offensive freethrow attempt rate
                int offFoulChance = (int)Math.Round(team2.FTR_O / 2);
                int randForOffFoul = r.Next(1, 101);
                if (offFoulChance >= randForOffFoul)
                {
                    res.OffensiveFoul = true;
                    if(waitTime > 0)
                    {
                        Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score, team2.Name, team2Score, team1HasBall, offensiveFouls + 1, defensiveFouls) + "---" + team1.Name + " committed an offensive foul.");
                    }
                    
                }
                else
                {
                    res.OffensiveFoul = false;
                    if(waitTime > 0)
                    {
                        Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls) + "---" + team1.Name + " turned the ball over.");
                    }
                    
                }



                return res;
            }

            //------------------------------------------------------------- no turnover or offensive foul, now what? --------------------------------------------------------------------------------------------------

            //------------------------------------------------------------- see if a dead ball defensive foul occurs --------------------------------------------------------------------------------------------------

            //the chance of a deadball foul is going to be half of the average of the FT attempt rates.
            int chanceOfDeadballFoul = (int)Math.Round((team1.FTR_O + team2.FTR_D) / 4);
            chanceOfDeadballFoul += adjusterFoul;
            int randForDeadballFoul = r.Next(1, 101);
            if (chanceOfDeadballFoul >= randForDeadballFoul)
            {
                //a deadball defensive foul happened.
                res.DefensiveFoul = true;
                //check if freethrows will happen
                if ((defensiveFouls + 1) > 6)
                {
                    //freethrows will happen
                    res.OffenseKeepsPosession = false;
                    //check if it's double bonus
                    int freethrowChance = (int)Math.Round(team1.FTP);
                    if ((defensiveFouls + 1) > 9)
                    {
                        //double bonus, two shots
                        if(waitTime > 0)
                        {
                            Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team1.Name + " was fouled on the floor, two free throws coming.");
                        }
                        res.PointsScored = 0;
                        int randFt1 = r.Next(1, 101);
                        if (freethrowChance >= randFt1)
                        {
                            //made the first one
                            res.PointsScored++;
                            if(waitTime > 0)
                            {
                                Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team1.Name + " made the first free throw.");
                                Thread.Sleep(waitTime);
                            }
                        }
                        else
                        {
                            //missed the first one
                            if(waitTime > 0)
                            {
                                Thread.Sleep(waitTime);
                                Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team1.Name + " missed the first free throw.");
                            }
                        }
                        int randFt2 = r.Next(1, 101);
                        if (freethrowChance >= randFt2)
                        {
                            //made the second one
                            res.PointsScored++;
                            if(waitTime > 0)
                            {
                                Thread.Sleep(waitTime);
                                Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team1.Name + " made the second free throw.");
                            }
                        }
                        else
                        {
                            //missed the second one
                            if(waitTime > 0)
                            {
                                Thread.Sleep(waitTime);
                                Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team1.Name + " missed the second free throw.");
                                Thread.Sleep(waitTime);
                                Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team2.Name + " got the defensive rebound.");
                            }
                            
                        }
                        return res;
                    }
                    else
                    {
                        //one and one
                        if(waitTime > 0)
                        {
                            Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team1.Name + " was fouled on the floor, one and one coming.");
                        }
                        
                        res.PointsScored = 0;
                        int randFt1 = r.Next(1, 101);
                        if (freethrowChance >= randFt1)
                        {
                            //made the first one
                            res.PointsScored++;
                            if(waitTime > 0)
                            {
                                Thread.Sleep(waitTime);
                                Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team1.Name + " made the first free throw.");
                            }
                            
                            int randFt2 = r.Next(1, 101);
                            if (freethrowChance <= randFt2)
                            {
                                //made the second one
                                res.PointsScored++;
                                if(waitTime > 0)
                                {
                                    Thread.Sleep(waitTime);
                                    Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team1.Name + " made the second free throw.");
                                }
                                
                                return res;
                            }
                            else
                            {
                                //missed the second one
                                if(waitTime > 0)
                                {
                                    Thread.Sleep(waitTime);
                                    Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team1.Name + " missed the second free throw.");
                                    Thread.Sleep(waitTime);
                                    Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team2.Name + " got the defensive rebound.");
                                }
                                
                                return res;
                            }

                        }
                        else
                        {
                            //missed the first one
                            if(waitTime > 0)
                            {
                                Thread.Sleep(waitTime);
                                Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team1.Name + " missed the first free throw.");
                                Thread.Sleep(waitTime);
                                Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team2.Name + " got the defensive rebound.");
                            }
                            
                            return res;
                        }
                    }
                }

                //if we got to here, no freethrows happened.
                if(waitTime > 0)
                {
                    Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team1.Name + " was fouled on the floor.");
                }

                res.PointsScored = 0;
                res.OffenseKeepsPosession = true;
                return res;
            }



            //determine if the shot is a 2 or 3
            bool is3Pointer = false;
            //im gonna say if a team is within 5 either way, 35% chance, winning by more than 5, 25% chance, losing by more than 5, 45% chance.

            int randForShotType = r.Next(1, 101);
            if (team1Score - team2Score > 5)
            {
                //winning by more than 5
                if (randForShotType > 30)
                {
                    is3Pointer = false;
                }
                else
                {
                    is3Pointer = true;
                }
            }
            else if (team2Score - team1Score > 5)
            {
                //losing by more than 5
                if (randForShotType > 40)
                {
                    is3Pointer = false;
                }
                else
                {
                    is3Pointer = true;
                }
            }
            else
            {
                //within 5 either way
                if (randForShotType > 35)
                {
                    is3Pointer = false;
                }
                else
                {
                    is3Pointer = true;
                }
            }

            //we now know what kind of shot is being taken, so split based on that
            //first calculate overall adjustment.  This takes into account how good the team is
            int adjuster = (int)Math.Round((team1.ADJOE + team2.ADJDE) / 2);
            int adjusterDef = (int)Math.Round((team2.ADJOE + team1.ADJDE) / 2);
            adjuster = adjuster - 100;
            adjusterDef = adjusterDef - 100;

            if (is3Pointer == true)
            {
                //3 pointer attempted
                int percentChance = (int)Math.Round((team1.PT3_O + team2.PT3_D) / 2);
                percentChance = percentChance + adjuster;
                //give home team a boost
                if(team1HasBall == false)
                {
                    percentChance += adjusterHomeTeam;
                }
                percentChance += adjuster3PT;
                int randForShot = r.Next(1, 101);
                if (randForShot <= percentChance)
                {
                    //he made that shit
                    if(waitTime > 0)
                    {
                        Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + 3, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls) + "---" + team1.Name + " made 3 point basket.");
                    }
                    
                    res.PointsScored = 3;
                    res.OffenseKeepsPosession = false;
                    return res;
                }
                else
                {
                    //he missed that shit
                    //but was he fouled? 
                    //percent of fouled on a 3 point shot will be 1/4 average of FT attempt rates
                    int chanceOf3ptFoul = (int)Math.Round((team1.FTR_O + team2.FTR_D) / 8);
                    chanceOf3ptFoul += adjusterFoul;
                    int randFor3ptFoul = r.Next(1, 101);
                    if (chanceOf3ptFoul >= randFor3ptFoul)
                    {
                        //fouled on a 3
                        if(waitTime > 0)
                        {
                            Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team1.Name + " was fouled on a 3 point basket.");
                        }
                        
                        res.PointsScored = 0;
                        res.OffenseKeepsPosession = false;
                        res.DefensiveFoul = true;
                        int freethrowChance = (int)Math.Round(team1.FTP);
                        int randFt1 = r.Next(1, 101);
                        int randFt2 = r.Next(1, 101);
                        int randFt3 = r.Next(1, 101);
                        if (freethrowChance >= randFt1)
                        {
                            //made the first
                            res.PointsScored++;
                            if(waitTime > 0)
                            {
                                Thread.Sleep(waitTime);
                                Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team1.Name + " made the first free throw.");
                            }
                           
                        }
                        else
                        {
                            //missed the first
                            if(waitTime > 0)
                            {
                                Thread.Sleep(waitTime);
                                Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team1.Name + " missed the first free throw.");
                            }
                           
                        }
                        if (freethrowChance >= randFt2)
                        {
                            //made the second
                            res.PointsScored++;
                            if(waitTime > 0)
                            {
                                Thread.Sleep(waitTime);
                                Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team1.Name + " made the second free throw.");
                            }
                           
                        }
                        else
                        {
                            //missed the second
                            if(waitTime > 0)
                            {
                                Thread.Sleep(waitTime);
                                Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team1.Name + " missed the second free throw.");
                            }
                          
                        }
                        if (freethrowChance >= randFt3)
                        {
                            //made the third
                            res.PointsScored++;
                            if(waitTime > 0)
                            {
                                Thread.Sleep(waitTime);
                                Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team1.Name + " made the third free throw.");
                            }
                           
                        }
                        else
                        {
                            //missed the third
                            if(waitTime > 0)
                            {
                                Thread.Sleep(waitTime);
                                Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team1.Name + " missed the third free throw.");
                                Thread.Sleep(waitTime);
                                Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team2.Name + " got the defensive rebound.");
                            }
                            
                        }
                        return res;
                    }

                    if(waitTime > 0)
                    {
                        Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls) + "---" + team1.Name + " missed 3 point basket.");
                    }
                    
                    //give them a chance to def rebound
                    int defReboundChance = (int)Math.Round(team2.DRB);
                    defReboundChance += adjusterDef;
                    int randForDefRebound = r.Next(1, 101);
                    if (randForDefRebound <= defReboundChance)
                    {
                        //got the defensive rebound
                        if(waitTime > 0)
                        {
                            Thread.Sleep(waitTime);
                            Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls) + "---" + team2.Name + " got the defensive rebound.");
                        }
                       
                        res.PointsScored = 0;
                        res.OffenseKeepsPosession = false;
                        return res;
                    }
                    else
                    {
                        //didnt get the defensive rebound, chance to get the offensive rebound
                        int offReboundChance = (int)Math.Round(team1.ORB);
                        offReboundChance += adjuster;
                        int randForOffRebound = r.Next(1, 101);
                        if (randForOffRebound <= offReboundChance)
                        {
                            //got the offensive rebound
                            if(waitTime > 0)
                            {
                                Thread.Sleep(waitTime);
                                Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls) + "---" + team1.Name + " got the offensive rebound.");
                            }
                           
                            res.PointsScored = 0;
                            res.OffenseKeepsPosession = true;
                            return res;
                        }
                        else
                        {
                            //didn't get the offensive rebound
                            if(waitTime > 0)
                            {
                                Thread.Sleep(waitTime);
                                Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls) + "---" + team2.Name + " got the defensive rebound.");
                            }
                           
                            res.PointsScored = 0;
                            res.OffenseKeepsPosession = false;
                            return res;
                        }
                    }
                }
            }
            else
            {
                //2 pointer attempted
                int percentChance = (int)Math.Round((team1.PT2_O + team2.PT2_D) / 2);
                percentChance = percentChance + adjuster;
                percentChance += adjuster2PT;
                //give home team a boost
                if (team1HasBall == false)
                {
                    percentChance += adjusterHomeTeam;
                }
                int randForShot = r.Next(1, 101);
                if (randForShot <= percentChance)
                {
                    //he made that shit
                    if(waitTime > 0)
                    {
                        Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + 2, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls) + "---" + team1.Name + " made 2 point basket.");
                    }
                   
                    res.PointsScored = 2;
                    res.OffenseKeepsPosession = false;
                    return res;
                }
                else
                {
                    //he missed that shit
                    //but was he fouled?
                    //chance of FT attemp rate average divided by 2
                    int chanceOf2ptFoul = (int)Math.Round((team1.FTR_O + team2.FTR_D) / 4);
                    chanceOf2ptFoul += adjusterFoul;
                    int randFor2ptFoul = r.Next(1, 101);
                    if (chanceOf2ptFoul >= randFor2ptFoul)
                    {
                        //fouled on a 2
                        if(waitTime > 0)
                        {
                            Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team1.Name + " was fouled on a 2 point basket.");
                        }
                        
                        res.PointsScored = 0;
                        res.OffenseKeepsPosession = false;
                        res.DefensiveFoul = true;
                        int freethrowChance = (int)Math.Round(team1.FTP);
                        int randFt1 = r.Next(1, 101);
                        int randFt2 = r.Next(1, 101);
                        if (freethrowChance >= randFt1)
                        {
                            //made the first
                            res.PointsScored++;
                            if(waitTime > 0)
                            {
                                Thread.Sleep(waitTime);
                                Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team1.Name + " made the first free throw.");
                            }
                            
                        }
                        else
                        {
                            //missed the first
                            if(waitTime > 0)
                            {
                                Thread.Sleep(waitTime);
                                Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team1.Name + " missed the first free throw.");
                            }
                           
                        }
                        if (freethrowChance >= randFt2)
                        {
                            //made the second
                            res.PointsScored++;
                            if(waitTime > 0)
                            {
                                Thread.Sleep(waitTime);
                                Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team1.Name + " made the second free throw.");
                            }
                           
                        }
                        else
                        {
                            //missed the second
                            if(waitTime > 0)
                            {
                                Thread.Sleep(waitTime);
                                Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team1.Name + " missed the second free throw.");
                                Thread.Sleep(waitTime);
                                Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team2.Name + " got the defensive rebound.");
                            }
                           
                        }
                        return res;
                    }

                    if(waitTime > 0)
                    {
                        Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls) + "---" + team1.Name + " missed 2 point basket.");
                    }
                   
                    //give them a chance to def rebound
                    int defReboundChance = (int)Math.Round(team2.DRB);
                    defReboundChance += adjusterDef;
                    int randForDefRebound = r.Next(1, 101);
                    if (randForDefRebound <= defReboundChance)
                    {
                        //got the defensive rebound
                        if(waitTime > 0)
                        {
                            Thread.Sleep(waitTime);
                            Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls) + "---" + team2.Name + " got the defensive rebound.");
                        }
                       
                        res.PointsScored = 0;
                        res.OffenseKeepsPosession = false;
                        return res;
                    }
                    else
                    {
                        //didnt get the defensive rebound, chance to get the offensive rebound
                        int offReboundChance = (int)Math.Round(team1.ORB);
                        offReboundChance += adjuster;
                        int randForOffRebound = r.Next(1, 101);
                        if (randForOffRebound >= offReboundChance)
                        {
                            //got the offensive rebound
                            if(waitTime > 0)
                            {
                                Thread.Sleep(waitTime);
                                Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls) + "---" + team1.Name + " got the offensive rebound.");
                            }
                            
                            res.PointsScored = 0;
                            res.OffenseKeepsPosession = true;
                            return res;
                        }
                        else
                        {
                            //didn't get the offensive rebound
                            if(waitTime > 0)
                            {
                                Thread.Sleep(waitTime);
                                Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls) + "---" + team2.Name + " got the defensive rebound.");
                            }
                            
                            res.PointsScored = 0;
                            res.OffenseKeepsPosession = false;
                            return res;
                        }
                    }
                }
            }

        }

        public static string GetTimeStamp(int secondsLeft)
        {
            if (secondsLeft <= 1)
            {
                return "0:01";
            }
            int minutes = secondsLeft / 60;
            int secs = secondsLeft % 60;
            string secSpacer = "";
            if (secs < 10)
            {
                secSpacer = "0";
            }
            return minutes.ToString() + ":" + secSpacer + secs.ToString();
        }

        public static string GetScoreboard(string team1, int team1Score, string team2, int team2Score, bool team1HasBall, int team1Fouls, int team2Fouls)
        {
            if (team1HasBall)
            {
                return "((" + team1Fouls.ToString() + ")" + team1 + " " + team1Score.ToString() + "-" + team2Score.ToString() + " " + team2 + "(" + team2Fouls.ToString() + "))";
            }
            else
            {
                return "((" + team2Fouls.ToString() + ")" + team2 + " " + team2Score.ToString() + "-" + team1Score.ToString() + " " + team1 + "(" + team1Fouls.ToString() + "))";
            }

        }

        public static int SimulateMarchMadness(int a2pt, int a3pt, int aturn, int afoul)
        {
            string team1 = "Connecticut";
            string team2 = "Stetson";
            string team3 = "Florida Atlantic";
            string team4 = "Northwestern";
            string team5 = "San Diego St.";
            string team6 = "UAB";
            string team7 = "Auburn";
            string team8 = "Yale";
            string team9 = "BYU";
            string team10 = "Duquesne";
            string team11 = "Illinois";
            string team12 = "Morehead St.";
            string team13 = "Washington St.";
            string team14 = "Drake";
            string team15 = "Iowa St.";
            string team16 = "South Dakota St.";

            string team17 = "Houston";
            string team18 = "Longwood";
            string team19 = "Nebraska";
            string team20 = "Texas A&M";
            string team21 = "Wisconsin";
            string team22 = "James Madison";
            string team23 = "Duke";
            string team24 = "Vermont";
            string team25 = "Texas Tech";
            string team26 = "North Carolina St.";
            string team27 = "Kentucky";
            string team28 = "Oakland";
            string team29 = "Florida";
            string team30 = "Colorado";
            string team31 = "Marquette";
            string team32 = "Western Kentucky";

            string team33 = "North Carolina";
            string team34 = "Wagner";
            string team35 = "Mississippi St.";
            string team36 = "Michigan St.";
            string team37 = "Saint Mary's";
            string team38 = "Grand Canyon";
            string team39 = "Alabama";
            string team40 = "Charleston";
            string team41 = "Clemson";
            string team42 = "New Mexico";
            string team43 = "Baylor";
            string team44 = "Colgate";
            string team45 = "Dayton";
            string team46 = "Nevada";
            string team47 = "Arizona";
            string team48 = "Long Beach St.";

            string team49 = "Purdue";
            string team50 = "Grambling";
            string team51 = "Utah St.";
            string team52 = "TCU";
            string team53 = "Gonzaga";
            string team54 = "McNeese";
            string team55 = "Kansas";
            string team56 = "Samford";
            string team57 = "South Carolina";
            string team58 = "Oregon";
            string team59 = "Creighton";
            string team60 = "Akron";
            string team61 = "Texas";
            string team62 = "Colorado St.";
            string team63 = "Tennessee";
            string team64 = "Saint Peter's";

            string r1g1 = "Connecticut";
            string r1g2 = "Northwestern";
            string r1g3 = "San Diego St.";
            string r1g4 = "Yale";
            string r1g5 = "Duquesne";
            string r1g6 = "Illinois";
            string r1g7 = "Washington St.";
            string r1g8 = "Iowa St.";
            string r1g9 = "Houston";
            string r1g10 = "Texas A&M";
            string r1g11 = "James Madison";
            string r1g12 = "Duke";
            string r1g13 = "North Carolina St.";
            string r1g14 = "Oakland";
            string r1g15 = "Colorado";
            string r1g16 = "Marquette";
            string r1g17 = "North Carolina";
            string r1g18 = "Michigan St.";
            string r1g19 = "Grand Canyon";
            string r1g20 = "Alabama";
            string r1g21 = "Clemson";
            string r1g22 = "Baylor";
            string r1g23 = "Dayton";
            string r1g24 = "Arizona";
            string r1g25 = "Purdue";
            string r1g26 = "Utah St.";
            string r1g27 = "Gonzaga";
            string r1g28 = "Kansas";
            string r1g29 = "Oregon";
            string r1g30 = "Creighton";
            string r1g31 = "Texas";
            string r1g32 = "Tennessee";

            string r2g1 = "Connecticut";
            string r2g2 = "San Diego St.";
            string r2g3 = "Illinois";
            string r2g4 = "Iowa St.";
            string r2g5 = "Houston";
            string r2g6 = "Duke";
            string r2g7 = "North Carolina St.";
            string r2g8 = "Marquette";
            string r2g9 = "North Carolina";
            string r2g10 = "Alabama";
            string r2g11 = "Clemson";
            string r2g12 = "Arizona";
            string r2g13 = "Purdue";
            string r2g14 = "Gonzaga";
            string r2g15 = "Creighton";
            string r2g16 = "Tennessee";

            string r3g1 = "Connecticut";
            string r3g2 = "Illinois";
            string r3g3 = "Duke";
            string r3g4 = "North Carolina St.";
            string r3g5 = "Alabama";
            string r3g6 = "Clemson";
            string r3g7 = "Purdue";
            string r3g8 = "Tennessee";

            string r4g1 = "Connecticut";
            string r4g2 = "Alabama";
            string r4g3 = "North Carolina St.";
            string r4g4 = "Purdue";

            string r5g1 = "Connecticut";
            string r5g2 = "Purdue";

            string r6g1 = "Connecticut";

            int round1Points = 0;
            int round2Points = 0;
            int round3Points = 0;
            int round4Points = 0;
            int round5Points = 0;
            int round6points = 0;
            int totalPoints = 0;




            string r1s1 = RunMatchup(team1, team2, a2pt, a3pt, aturn, afoul, GameSpeed.Instant, "2024", true).Winner;
            Console.WriteLine(GetGameStampForSimulator(r1s1, r1g1, 1));
            string r1s2 = RunMatchup(team3, team4, a2pt, a3pt, aturn, afoul, GameSpeed.Instant, "2024", true).Winner;
            Console.WriteLine(GetGameStampForSimulator(r1s2, r1g2, 1));
            string r1s3 = RunMatchup(team5, team6, a2pt, a3pt, aturn, afoul, GameSpeed.Instant, "2024", true).Winner;
            Console.WriteLine(GetGameStampForSimulator(r1s3, r1g3, 1));
            string r1s4 = RunMatchup(team7, team8, a2pt, a3pt, aturn, afoul, GameSpeed.Instant, "2024", true).Winner;
            Console.WriteLine(GetGameStampForSimulator(r1s4, r1g4, 1));
            string r1s5 = RunMatchup(team9, team10, a2pt, a3pt, aturn, afoul, GameSpeed.Instant, "2024", true).Winner;
            Console.WriteLine(GetGameStampForSimulator(r1s5, r1g5, 1));
            string r1s6 = RunMatchup(team11, team12, a2pt, a3pt, aturn, afoul, GameSpeed.Instant, "2024", true).Winner;
            Console.WriteLine(GetGameStampForSimulator(r1s6, r1g6, 1));
            string r1s7 = RunMatchup(team13, team14, a2pt, a3pt, aturn, afoul, GameSpeed.Instant, "2024", true).Winner;
            Console.WriteLine(GetGameStampForSimulator(r1s7, r1g7, 1));
            string r1s8 = RunMatchup(team15, team16, a2pt, a3pt, aturn, afoul, GameSpeed.Instant, "2024", true).Winner;
            Console.WriteLine(GetGameStampForSimulator(r1s8, r1g8, 1));
            string r1s9 = RunMatchup(team17, team18, a2pt, a3pt, aturn, afoul, GameSpeed.Instant, "2024", true).Winner;
            Console.WriteLine(GetGameStampForSimulator(r1s9, r1g9, 1));
            string r1s10 = RunMatchup(team19, team20, a2pt, a3pt, aturn, afoul, GameSpeed.Instant, "2024", true).Winner;
            Console.WriteLine(GetGameStampForSimulator(r1s10, r1g10, 1));
            string r1s11 = RunMatchup(team21, team22, a2pt, a3pt, aturn, afoul, GameSpeed.Instant, "2024", true).Winner;
            Console.WriteLine(GetGameStampForSimulator(r1s11, r1g11, 1));
            string r1s12 = RunMatchup(team23, team24, a2pt, a3pt, aturn, afoul, GameSpeed.Instant, "2024", true).Winner;
            Console.WriteLine(GetGameStampForSimulator(r1s12, r1g12, 1));
            string r1s13 = RunMatchup(team25, team26, a2pt, a3pt, aturn, afoul, GameSpeed.Instant, "2024", true).Winner;
            Console.WriteLine(GetGameStampForSimulator(r1s13, r1g13, 1));
            string r1s14 = RunMatchup(team27, team28, a2pt, a3pt, aturn, afoul, GameSpeed.Instant, "2024", true).Winner;
            Console.WriteLine(GetGameStampForSimulator(r1s14, r1g14, 1));
            string r1s15 = RunMatchup(team29, team30, a2pt, a3pt, aturn, afoul, GameSpeed.Instant, "2024", true).Winner;
            Console.WriteLine(GetGameStampForSimulator(r1s15, r1g15, 1));
            string r1s16 = RunMatchup(team31, team32, a2pt, a3pt, aturn, afoul, GameSpeed.Instant, "2024", true).Winner;
            Console.WriteLine(GetGameStampForSimulator(r1s16, r1g16, 1));
            string r1s17 = RunMatchup(team33, team34, a2pt, a3pt, aturn, afoul, GameSpeed.Instant, "2024", true).Winner;
            Console.WriteLine(GetGameStampForSimulator(r1s17, r1g17, 1));
            string r1s18 = RunMatchup(team35, team36, a2pt, a3pt, aturn, afoul, GameSpeed.Instant, "2024", true).Winner;
            Console.WriteLine(GetGameStampForSimulator(r1s18, r1g18, 1));
            string r1s19 = RunMatchup(team37, team38, a2pt, a3pt, aturn, afoul, GameSpeed.Instant, "2024", true).Winner;
            Console.WriteLine(GetGameStampForSimulator(r1s19, r1g19, 1));
            string r1s20 = RunMatchup(team39, team40, a2pt, a3pt, aturn, afoul, GameSpeed.Instant, "2024", true).Winner;
            Console.WriteLine(GetGameStampForSimulator(r1s20, r1g20, 1));
            string r1s21 = RunMatchup(team41, team42, a2pt, a3pt, aturn, afoul, GameSpeed.Instant, "2024", true).Winner;
            Console.WriteLine(GetGameStampForSimulator(r1s21, r1g21, 1));
            string r1s22 = RunMatchup(team43, team44, a2pt, a3pt, aturn, afoul, GameSpeed.Instant, "2024", true).Winner;
            Console.WriteLine(GetGameStampForSimulator(r1s22, r1g22, 1));
            string r1s23 = RunMatchup(team45, team46, a2pt, a3pt, aturn, afoul, GameSpeed.Instant, "2024", true).Winner;
            Console.WriteLine(GetGameStampForSimulator(r1s23, r1g23, 1));
            string r1s24 = RunMatchup(team47, team48, a2pt, a3pt, aturn, afoul, GameSpeed.Instant, "2024", true).Winner;
            Console.WriteLine(GetGameStampForSimulator(r1s24, r1g24, 1));
            string r1s25 = RunMatchup(team49, team50, a2pt, a3pt, aturn, afoul, GameSpeed.Instant, "2024", true).Winner;
            Console.WriteLine(GetGameStampForSimulator(r1s25, r1g25, 1));
            string r1s26 = RunMatchup(team51, team52, a2pt, a3pt, aturn, afoul, GameSpeed.Instant, "2024", true).Winner;
            Console.WriteLine(GetGameStampForSimulator(r1s26, r1g26, 1));
            string r1s27 = RunMatchup(team53, team54, a2pt, a3pt, aturn, afoul, GameSpeed.Instant, "2024", true).Winner;
            Console.WriteLine(GetGameStampForSimulator(r1s27, r1g27, 1));
            string r1s28 = RunMatchup(team55, team56, a2pt, a3pt, aturn, afoul, GameSpeed.Instant, "2024", true).Winner;
            Console.WriteLine(GetGameStampForSimulator(r1s28, r1g28, 1));
            string r1s29 = RunMatchup(team57, team58, a2pt, a3pt, aturn, afoul, GameSpeed.Instant, "2024", true).Winner;
            Console.WriteLine(GetGameStampForSimulator(r1s29, r1g29, 1));
            string r1s30 = RunMatchup(team59, team60, a2pt, a3pt, aturn, afoul, GameSpeed.Instant, "2024", true).Winner;
            Console.WriteLine(GetGameStampForSimulator(r1s30, r1g30, 1));
            string r1s31 = RunMatchup(team61, team62, a2pt, a3pt, aturn, afoul, GameSpeed.Instant, "2024", true).Winner;
            Console.WriteLine(GetGameStampForSimulator(r1s31, r1g31, 1));
            string r1s32 = RunMatchup(team63, team64, a2pt, a3pt, aturn, afoul, GameSpeed.Instant, "2024", true).Winner;
            Console.WriteLine(GetGameStampForSimulator(r1s32, r1g32, 1));

            round1Points += GetGamePointsForSimulator(r1s1, r1g1, 1);
            round1Points += GetGamePointsForSimulator(r1s2, r1g2, 1);
            round1Points += GetGamePointsForSimulator(r1s3, r1g3, 1);
            round1Points += GetGamePointsForSimulator(r1s4, r1g4, 1);
            round1Points += GetGamePointsForSimulator(r1s5, r1g5, 1);
            round1Points += GetGamePointsForSimulator(r1s6, r1g6, 1);
            round1Points += GetGamePointsForSimulator(r1s7, r1g7, 1);
            round1Points += GetGamePointsForSimulator(r1s8, r1g8, 1);
            round1Points += GetGamePointsForSimulator(r1s9, r1g9, 1);
            round1Points += GetGamePointsForSimulator(r1s10, r1g10, 1);
            round1Points += GetGamePointsForSimulator(r1s11, r1g11, 1);
            round1Points += GetGamePointsForSimulator(r1s12, r1g12, 1);
            round1Points += GetGamePointsForSimulator(r1s13, r1g13, 1);
            round1Points += GetGamePointsForSimulator(r1s14, r1g14, 1);
            round1Points += GetGamePointsForSimulator(r1s15, r1g15, 1);
            round1Points += GetGamePointsForSimulator(r1s16, r1g16, 1);
            round1Points += GetGamePointsForSimulator(r1s17, r1g17, 1);
            round1Points += GetGamePointsForSimulator(r1s18, r1g18, 1);
            round1Points += GetGamePointsForSimulator(r1s19, r1g19, 1);
            round1Points += GetGamePointsForSimulator(r1s20, r1g20, 1);
            round1Points += GetGamePointsForSimulator(r1s21, r1g21, 1);
            round1Points += GetGamePointsForSimulator(r1s22, r1g22, 1);
            round1Points += GetGamePointsForSimulator(r1s23, r1g23, 1);
            round1Points += GetGamePointsForSimulator(r1s24, r1g24, 1);
            round1Points += GetGamePointsForSimulator(r1s25, r1g25, 1);
            round1Points += GetGamePointsForSimulator(r1s26, r1g26, 1);
            round1Points += GetGamePointsForSimulator(r1s27, r1g27, 1);
            round1Points += GetGamePointsForSimulator(r1s28, r1g28, 1);
            round1Points += GetGamePointsForSimulator(r1s29, r1g29, 1);
            round1Points += GetGamePointsForSimulator(r1s30, r1g30, 1);
            round1Points += GetGamePointsForSimulator(r1s31, r1g31, 1);
            round1Points += GetGamePointsForSimulator(r1s32, r1g32, 1);

            totalPoints += round1Points;
            Console.WriteLine("");
            Console.WriteLine("##########################################");
            Console.WriteLine("Round 1 points: " + round1Points.ToString());
            Console.WriteLine("##########################################");
            Console.WriteLine("");

            string r2s1 = RunMatchup(r1s1, r1s2, a2pt, a3pt, aturn, afoul, GameSpeed.Instant, "2024", true).Winner;
            Console.WriteLine(GetGameStampForSimulator(r2s1, r2g1, 2));
            string r2s2 = RunMatchup(r1s3, r1s4, a2pt, a3pt, aturn, afoul, GameSpeed.Instant, "2024", true).Winner;
            Console.WriteLine(GetGameStampForSimulator(r2s2, r2g2, 2));
            string r2s3 = RunMatchup(r1s5, r1s6, a2pt, a3pt, aturn, afoul, GameSpeed.Instant, "2024", true).Winner;
            Console.WriteLine(GetGameStampForSimulator(r2s3, r2g3, 2));
            string r2s4 = RunMatchup(r1s7, r1s8, a2pt, a3pt, aturn, afoul, GameSpeed.Instant, "2024", true).Winner;
            Console.WriteLine(GetGameStampForSimulator(r2s4, r2g4, 2));
            string r2s5 = RunMatchup(r1s9, r1s10, a2pt, a3pt, aturn, afoul, GameSpeed.Instant, "2024", true).Winner;
            Console.WriteLine(GetGameStampForSimulator(r2s5, r2g5, 2));
            string r2s6 = RunMatchup(r1s11, r1s12, a2pt, a3pt, aturn, afoul, GameSpeed.Instant, "2024", true).Winner;
            Console.WriteLine(GetGameStampForSimulator(r2s6, r2g6, 2));
            string r2s7 = RunMatchup(r1s13, r1s14, a2pt, a3pt, aturn, afoul, GameSpeed.Instant, "2024", true).Winner;
            Console.WriteLine(GetGameStampForSimulator(r2s7, r2g7, 2));
            string r2s8 = RunMatchup(r1s15, r1s16, a2pt, a3pt, aturn, afoul, GameSpeed.Instant, "2024", true).Winner;
            Console.WriteLine(GetGameStampForSimulator(r2s8, r2g8, 2));
            string r2s9 = RunMatchup(r1s17, r1s18, a2pt, a3pt, aturn, afoul, GameSpeed.Instant, "2024", true).Winner;
            Console.WriteLine(GetGameStampForSimulator(r2s9, r2g9, 2));
            string r2s10 = RunMatchup(r1s19, r1s20, a2pt, a3pt, aturn, afoul, GameSpeed.Instant, "2024", true).Winner;
            Console.WriteLine(GetGameStampForSimulator(r2s10, r2g10, 2));
            string r2s11 = RunMatchup(r1s21, r1s22, a2pt, a3pt, aturn, afoul, GameSpeed.Instant, "2024", true).Winner;
            Console.WriteLine(GetGameStampForSimulator(r2s11, r2g11, 2));
            string r2s12 = RunMatchup(r1s23, r1s24, a2pt, a3pt, aturn, afoul, GameSpeed.Instant, "2024", true).Winner;
            Console.WriteLine(GetGameStampForSimulator(r2s12, r2g12, 2));
            string r2s13 = RunMatchup(r1s25, r1s26, a2pt, a3pt, aturn, afoul, GameSpeed.Instant, "2024", true).Winner;
            Console.WriteLine(GetGameStampForSimulator(r2s13, r2g13, 2));
            string r2s14 = RunMatchup(r1s27, r1s28, a2pt, a3pt, aturn, afoul, GameSpeed.Instant, "2024", true).Winner;
            Console.WriteLine(GetGameStampForSimulator(r2s14, r2g14, 2));
            string r2s15 = RunMatchup(r1s29, r1s30, a2pt, a3pt, aturn, afoul, GameSpeed.Instant, "2024", true).Winner;
            Console.WriteLine(GetGameStampForSimulator(r2s15, r2g15, 2));
            string r2s16 = RunMatchup(r1s31, r1s32, a2pt, a3pt, aturn, afoul, GameSpeed.Instant, "2024", true).Winner;
            Console.WriteLine(GetGameStampForSimulator(r2s16, r2g16, 2));

            round2Points += GetGamePointsForSimulator(r2s1, r2g1, 2);
            round2Points += GetGamePointsForSimulator(r2s2, r2g2, 2);
            round2Points += GetGamePointsForSimulator(r2s3, r2g3, 2);
            round2Points += GetGamePointsForSimulator(r2s4, r2g4, 2);
            round2Points += GetGamePointsForSimulator(r2s5, r2g5, 2);
            round2Points += GetGamePointsForSimulator(r2s6, r2g6, 2);
            round2Points += GetGamePointsForSimulator(r2s7, r2g7, 2);
            round2Points += GetGamePointsForSimulator(r2s8, r2g8, 2);
            round2Points += GetGamePointsForSimulator(r2s9, r2g9, 2);
            round2Points += GetGamePointsForSimulator(r2s10, r2g10, 2);
            round2Points += GetGamePointsForSimulator(r2s11, r2g11, 2);
            round2Points += GetGamePointsForSimulator(r2s12, r2g12, 2);
            round2Points += GetGamePointsForSimulator(r2s13, r2g13, 2);
            round2Points += GetGamePointsForSimulator(r2s14, r2g14, 2);
            round2Points += GetGamePointsForSimulator(r2s15, r2g15, 2);
            round2Points += GetGamePointsForSimulator(r2s16, r2g16, 2);

            totalPoints += round2Points;
            Console.WriteLine("");
            Console.WriteLine("##########################################");
            Console.WriteLine("Round 2 points: " + round2Points.ToString());
            Console.WriteLine("##########################################");
            Console.WriteLine("");

            string r3s1 = RunMatchup(r2s1, r2s2, a2pt, a3pt, aturn, afoul, GameSpeed.Instant, "2024", true).Winner;
            Console.WriteLine(GetGameStampForSimulator(r3s1, r3g1, 3));
            string r3s2 = RunMatchup(r2s3, r2s4, a2pt, a3pt, aturn, afoul, GameSpeed.Instant, "2024", true).Winner;
            Console.WriteLine(GetGameStampForSimulator(r3s2, r3g2, 3));
            string r3s3 = RunMatchup(r2s5, r2s6, a2pt, a3pt, aturn, afoul, GameSpeed.Instant, "2024", true).Winner;
            Console.WriteLine(GetGameStampForSimulator(r3s3, r3g3, 3));
            string r3s4 = RunMatchup(r2s7, r2s8, a2pt, a3pt, aturn, afoul, GameSpeed.Instant, "2024", true).Winner;
            Console.WriteLine(GetGameStampForSimulator(r3s4, r3g4, 3));
            string r3s5 = RunMatchup(r2s9, r2s10, a2pt, a3pt, aturn, afoul, GameSpeed.Instant, "2024", true).Winner;
            Console.WriteLine(GetGameStampForSimulator(r3s5, r3g5, 3));
            string r3s6 = RunMatchup(r2s11, r2s12, a2pt, a3pt, aturn, afoul, GameSpeed.Instant, "2024", true).Winner;
            Console.WriteLine(GetGameStampForSimulator(r3s6, r3g6, 3));
            string r3s7 = RunMatchup(r2s13, r2s14, a2pt, a3pt, aturn, afoul, GameSpeed.Instant, "2024", true).Winner;
            Console.WriteLine(GetGameStampForSimulator(r3s7, r3g7, 3));
            string r3s8 = RunMatchup(r2s15, r2s16, a2pt, a3pt, aturn, afoul, GameSpeed.Instant, "2024", true).Winner;
            Console.WriteLine(GetGameStampForSimulator(r3s8, r3g8, 3));

            round3Points += GetGamePointsForSimulator(r3s1, r3g1, 3);
            round3Points += GetGamePointsForSimulator(r3s2, r3g2, 3);
            round3Points += GetGamePointsForSimulator(r3s3, r3g3, 3);
            round3Points += GetGamePointsForSimulator(r3s4, r3g4, 3);
            round3Points += GetGamePointsForSimulator(r3s5, r3g5, 3);
            round3Points += GetGamePointsForSimulator(r3s6, r3g6, 3);
            round3Points += GetGamePointsForSimulator(r3s7, r3g7, 3);
            round3Points += GetGamePointsForSimulator(r3s8, r3g8, 3);

            totalPoints += round3Points;
            Console.WriteLine("");
            Console.WriteLine("##########################################");
            Console.WriteLine("Sweet 16 points: " + round3Points.ToString());
            Console.WriteLine("##########################################");
            Console.WriteLine("");

            string r4s1 = RunMatchup(r3s1, r3s2, a2pt, a3pt, aturn, afoul, GameSpeed.Instant, "2024", true).Winner;
            Console.WriteLine(GetGameStampForSimulator(r4s1, r4g1, 4));
            string r4s2 = RunMatchup(r3s3, r3s4, a2pt, a3pt, aturn, afoul, GameSpeed.Instant, "2024", true).Winner;
            Console.WriteLine(GetGameStampForSimulator(r4s2, r4g2, 4));
            string r4s3 = RunMatchup(r3s5, r3s6, a2pt, a3pt, aturn, afoul, GameSpeed.Instant, "2024", true).Winner;
            Console.WriteLine(GetGameStampForSimulator(r4s3, r4g3, 4));
            string r4s4 = RunMatchup(r3s7, r3s8, a2pt, a3pt, aturn, afoul, GameSpeed.Instant, "2024", true).Winner;
            Console.WriteLine(GetGameStampForSimulator(r4s4, r4g4, 4));

            round4Points += GetGamePointsForSimulator(r4s1, r4g1, 4);
            round4Points += GetGamePointsForSimulator(r4s2, r4g2, 4);
            round4Points += GetGamePointsForSimulator(r4s3, r4g3, 4);
            round4Points += GetGamePointsForSimulator(r4s4, r4g4, 4);

            totalPoints += round4Points;
            Console.WriteLine("");
            Console.WriteLine("##########################################");
            Console.WriteLine("Elite 8 points: " + round4Points.ToString());
            Console.WriteLine("##########################################");
            Console.WriteLine("");

            string r5s1 = RunMatchup(r4s1, r4s2, a2pt, a3pt, aturn, afoul, GameSpeed.Instant, "2024", true).Winner;
            Console.WriteLine(GetGameStampForSimulator(r5s1, r5g1, 5));
            string r5s2 = RunMatchup(r4s3, r4s4, a2pt, a3pt, aturn, afoul, GameSpeed.Instant, "2024", true).Winner;
            Console.WriteLine(GetGameStampForSimulator(r5s2, r5g2, 5));

            round5Points += GetGamePointsForSimulator(r5s1, r5g1, 5);
            round5Points += GetGamePointsForSimulator(r5s2, r5g2, 5);

            totalPoints += round5Points;
            Console.WriteLine("");
            Console.WriteLine("##########################################");
            Console.WriteLine("Final 4 points: " + round5Points.ToString());
            Console.WriteLine("##########################################");
            Console.WriteLine("");

            string r6s1 = RunMatchup(r5s1, r5s2, a2pt, a3pt, aturn, afoul, GameSpeed.Instant, "2024", true).Winner;
            round6points += GetGamePointsForSimulator(r6s1, r6g1, 6);
            totalPoints += round6points;
            Console.WriteLine("");
            Console.WriteLine("##########################################");
            Console.WriteLine("Champion: " + r6s1);
            Console.WriteLine("Simulation total points: " + totalPoints.ToString());
            Console.WriteLine("##########################################");
            Console.WriteLine("");
            System.Threading.Thread.Sleep(2000);

            return totalPoints;

        }

        public static string GetGameStampForSimulator(string s, string g, int round)
        {
            string popen = "";
            string pclose = "";
            for (var i = 0; i < round; i++)
            {
                popen += "(";
                pclose += ")";
            }
            if (s == g)
            {
                return popen + "X" + pclose + " " + s;
            }
            else
            {
                return popen + " " + pclose + " " + s;
            }
        }
        public static int GetGamePointsForSimulator(string s, string g, int round)
        {
            if (s != g)
            {
                return 0;
            }
            else
            {
                if (round == 1)
                {
                    return 10;
                }
                else if (round == 2)
                {
                    return 20;
                }
                else if (round == 3)
                {
                    return 40;
                }
                else if (round == 4)
                {
                    return 80;
                }
                else if (round == 5)
                {
                    return 160;
                }
                else if (round == 6)
                {
                    return 320;
                }
            }
            return 0;
        }

        //import the college data.
        public static List<CollegeModel> GetCollegeModelData()
        {

            List<CollegeModel> results = File.ReadAllLines("C:\\Users\\bfeddersen\\Desktop\\root\\Work\\cbb24.csv").Skip(1).Select(x => ReadCollegeModelFromCSV(x)).ToList();
            return results;

        }

        public static List<CollegeModel> GetCollegeModelData2025()
        {

            List<CollegeModel> results = File.ReadAllLines("C:\\Users\\bfeddersen\\Desktop\\root\\Work\\cbb25.csv").Skip(1).Select(x => ReadCollegeModelFromCSV(x)).ToList();
            return results;

        }

        //college data helper method.
        public static CollegeModel ReadCollegeModelFromCSV(string csvLine)
        {
            try
            {
                string[] values = csvLine.Split(',');
                CollegeModel model = new CollegeModel();
                model.Rank = Convert.ToInt32(values[0]);
                model.Name = values[1];
                model.Conference = values[2];
                model.GamesPlayed = Convert.ToInt32(values[3]);
                model.Wins = Convert.ToInt32(values[4]);
                model.Losses = model.GamesPlayed - model.Wins;
                model.ADJOE = Convert.ToDecimal(values[5]);
                model.ADJDE = Convert.ToDecimal(values[6]);
                model.BARTHAG = Convert.ToDecimal(values[7]);
                model.EFG_O = Convert.ToDecimal(values[8]);
                model.EFG_D = Convert.ToDecimal(values[9]);
                model.TOR_O = Convert.ToDecimal(values[10]);
                model.TOR_D = Convert.ToDecimal(values[11]);
                model.ORB = Convert.ToDecimal(values[12]);
                model.DRB = Convert.ToDecimal(values[13]);
                model.FTR_O = Convert.ToDecimal(values[14]);
                model.FTR_D = Convert.ToDecimal(values[15]);
                model.PT2_O = Convert.ToDecimal(values[16]);
                model.PT2_D = Convert.ToDecimal(values[17]);
                model.PT3_O = Convert.ToDecimal(values[18]);
                model.PT3_D = Convert.ToDecimal(values[19]);
                model.ADJ_T = Convert.ToDecimal(values[20]);
                model.WAB = Convert.ToDecimal(values[21]);
                try
                {
                    model.FTP = Convert.ToDecimal(values[23]);
                }
                catch
                {
                    //just default FT% to 70 if they don't have one set.
                    model.FTP = 70;
                }

                return model;
            }
            catch
            {
                return new CollegeModel();
            }
            
        }

        public static void RunBig12Season(int times, int a2pt, int a3pt, int aturn, int afoul)
        {
            List<MatchupResult> simulationList = new List<MatchupResult>();
            List<MatchupResult> resultList = new List<MatchupResult>();




            //ACTUAL RESULTS----------------------------------------------------------------
            //week 1
            resultList.Add(CreateMatchupResult("Cincinnati", 67, "Kansas St.", 70));
            resultList.Add(CreateMatchupResult("Houston", 60, "Oklahoma St.", 47));
            resultList.Add(CreateMatchupResult("TCU", 81, "Arizona", 90));
            resultList.Add(CreateMatchupResult("Iowa St.", 79, "Colorado", 69));
            resultList.Add(CreateMatchupResult("UCF", 87, "Texas Tech", 83));
            resultList.Add(CreateMatchupResult("West Virginia", 62, "Kansas", 61));
            resultList.Add(CreateMatchupResult("Utah", 56, "Baylor", 81));
            resultList.Add(CreateMatchupResult("Arizona St.", 56, "BYU", 76));

            //week 2
            resultList.Add(CreateMatchupResult("Oklahoma St.", 50, "West Virginia", 69));
            resultList.Add(CreateMatchupResult("BYU", 55, "Houston", 86));
            resultList.Add(CreateMatchupResult("Baylor", 55, "Iowa St.", 74));
            resultList.Add(CreateMatchupResult("Arizona", 72, "Cincinnati", 67));
            resultList.Add(CreateMatchupResult("Kansas St.", 62, "TCU", 63));
            resultList.Add(CreateMatchupResult("Colorado", 61, "Arizona St.", 81));
            resultList.Add(CreateMatchupResult("Texas Tech", 93, "Utah", 65));
            resultList.Add(CreateMatchupResult("Kansas", 99, "UCF", 48));

            //week 3
            resultList.Add(CreateMatchupResult("TCU", 46, "Houston", 65));
            resultList.Add(CreateMatchupResult("Arizona", 75, "West Virginia", 56));
            resultList.Add(CreateMatchupResult("Utah", 59, "Iowa St.", 82));
            resultList.Add(CreateMatchupResult("Cincinnati", 48, "Baylor", 68));
            resultList.Add(CreateMatchupResult("Kansas St.", 66, "Oklahoma St.", 79));
            resultList.Add(CreateMatchupResult("Texas Tech", 72, "BYU", 67));
            resultList.Add(CreateMatchupResult("Colorado", 74, "UCF", 75));
            resultList.Add(CreateMatchupResult("Arizona St.", 55, "Kansas", 74));

            //week 4
            resultList.Add(CreateMatchupResult("Kansas", 54, "Cincinnati", 40));
            resultList.Add(CreateMatchupResult("BYU", 67, "TCU", 71));
            resultList.Add(CreateMatchupResult("Iowa St.", 85, "Texas Tech", 84));
            resultList.Add(CreateMatchupResult("Houston", 87, "Kansas St.", 57));
            resultList.Add(CreateMatchupResult("Oklahoma St.", 62, "Utah", 83));
            resultList.Add(CreateMatchupResult("Baylor", 72, "Arizona St.", 66));
            resultList.Add(CreateMatchupResult("UCF", 80, "Arizona", 88));
            resultList.Add(CreateMatchupResult("West Virginia", 78, "Colorado", 70));

            //week 5
            resultList.Add(CreateMatchupResult("Oklahoma St.", 69, "BYU", 85));
            resultList.Add(CreateMatchupResult("Texas Tech", 61, "Kansas St.", 57));
            resultList.Add(CreateMatchupResult("UCF", 95, "Arizona St.", 89));
            resultList.Add(CreateMatchupResult("Baylor", 70, "Arizona", 81));
            resultList.Add(CreateMatchupResult("Kansas", 57, "Iowa St.", 74));
            resultList.Add(CreateMatchupResult("Utah", 73, "TCU", 65));
            resultList.Add(CreateMatchupResult("West Virginia", 54, "Houston", 70));
            resultList.Add(CreateMatchupResult("Cincinnati", 68, "Colorado", 62));

            //week 6
            resultList.Add(CreateMatchupResult("Houston", 69, "UCF", 68));
            resultList.Add(CreateMatchupResult("Kansas St.", 74, "Kansas", 84));
            resultList.Add(CreateMatchupResult("Arizona", 54, "Texas Tech", 70));
            resultList.Add(CreateMatchupResult("Arizona St.", 60, "Cincinnati", 67));
            resultList.Add(CreateMatchupResult("Colorado", 73, "Oklahoma St.", 83));
            resultList.Add(CreateMatchupResult("Iowa St.", 57, "West Virginia", 64));
            resultList.Add(CreateMatchupResult("BYU", 72, "Utah", 73));
            resultList.Add(CreateMatchupResult("TCU", 74, "Baylor", 71));

            //week 7
            resultList.Add(CreateMatchupResult("Texas Tech", 81, "Cincinnati", 71));
            resultList.Add(CreateMatchupResult("UCF", 83, "Iowa St.", 108));
            resultList.Add(CreateMatchupResult("Arizona St.", 65, "West Virginia", 57));
            resultList.Add(CreateMatchupResult("Arizona", 92, "Oklahoma St.", 78));
            resultList.Add(CreateMatchupResult("BYU", 83, "Colorado", 67));
            resultList.Add(CreateMatchupResult("Utah", 36, "Houston", 70));
            resultList.Add(CreateMatchupResult("Kansas", 74, "TCU", 61));
            resultList.Add(CreateMatchupResult("Kansas St.", 62, "Baylor", 70));

            //week 8
            resultList.Add(CreateMatchupResult("Iowa St.", 76, "Arizona St.", 61));
            resultList.Add(CreateMatchupResult("Colorado", 63, "Arizona", 78));
            resultList.Add(CreateMatchupResult("TCU", 58, "UCF", 85));
            resultList.Add(CreateMatchupResult("Baylor", 76, "Utah", 61));
            resultList.Add(CreateMatchupResult("West Virginia", 60, "Kansas St.", 73));
            resultList.Add(CreateMatchupResult("Houston", 92, "Kansas", 86));
            resultList.Add(CreateMatchupResult("Cincinnati", 52, "BYU", 80));
            resultList.Add(CreateMatchupResult("Oklahoma St.", 54, "Texas Tech", 64));

            //week 9
            resultList.Add(CreateMatchupResult("Iowa St.", 75, "Arizona", 86));
            resultList.Add(CreateMatchupResult("UCF", 87, "Kansas", 91));
            resultList.Add(CreateMatchupResult("Arizona St.", 70, "Colorado", 68));
            resultList.Add(CreateMatchupResult("Baylor", 89, "BYU", 93));
            resultList.Add(CreateMatchupResult("Cincinnati", 66, "Utah", 69));
            resultList.Add(CreateMatchupResult("Houston", 63, "West Virginia", 49));
            resultList.Add(CreateMatchupResult("TCU", 57, "Texas Tech", 71));
            resultList.Add(CreateMatchupResult("Oklahoma St.", 57, "Kansas St.", 85));

            //PREDICTIONS-------------------------------------------------------------------
            int awayPMNet = 0;
            int homePMNet = 0;
            int awayPMTotal = 0;
            int homePMTotal = 0;
            int margVicDiff = 0;
            int gamesSimulated = 0;
            int gamesCorrect = 0;

            foreach(MatchupResult actualResult in resultList)
            {
                MatchupResult predictionResult = ProcessBig12Matchup(actualResult, times, a2pt, a3pt, aturn, afoul);
                Console.WriteLine(GetBig12PredictorLog(predictionResult, actualResult));
                awayPMNet += (predictionResult.AwayTeamScore - actualResult.AwayTeamScore);
                homePMNet += (predictionResult.HomeTeamScore - actualResult.HomeTeamScore);
                awayPMTotal += Math.Abs(predictionResult.AwayTeamScore - actualResult.AwayTeamScore);
                homePMTotal += Math.Abs(predictionResult.HomeTeamScore - actualResult.HomeTeamScore);
                margVicDiff += Math.Abs((predictionResult.AwayTeamScore - predictionResult.HomeTeamScore) - (actualResult.AwayTeamScore - actualResult.HomeTeamScore));
                gamesSimulated++;
                if(predictionResult.Winner == actualResult.Winner)
                {
                    gamesCorrect++;
                }
            }

            Console.WriteLine("");
            Console.WriteLine("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
            Console.WriteLine("---------------------------Simulation Statistics---------------------------");
            Console.WriteLine("Away Team +/- Net        : " + awayPMNet.ToString());
            Console.WriteLine("Away Team +/- Net Avg.   : " + Math.Round(Decimal.Divide(awayPMNet, gamesSimulated), 2).ToString());
            Console.WriteLine("Home Team +/- Net        : " + homePMNet.ToString());
            Console.WriteLine("Home Team +/- Net Avg.   : " + Math.Round(Decimal.Divide(homePMNet, gamesSimulated), 2).ToString());
            Console.WriteLine("Combined  +/- Net        : " + (awayPMNet + homePMNet).ToString());
            Console.WriteLine("Combined  +/- Net Avg.   : " + Math.Round(Decimal.Divide((awayPMNet + homePMNet), gamesSimulated) / 2, 2).ToString());
            Console.WriteLine("--------------------------------");
            Console.WriteLine("Away Team +/- Total      : " + awayPMTotal.ToString());
            Console.WriteLine("Away Team +/- Total Avg. : " + Math.Round(Decimal.Divide(awayPMTotal, gamesSimulated), 2).ToString());
            Console.WriteLine("Home Team +/- Total      : " + homePMTotal.ToString());
            Console.WriteLine("Home Team +/- Total Avg. : " + Math.Round(Decimal.Divide(homePMTotal, gamesSimulated), 2).ToString());
            Console.WriteLine("Combined  +/- Total      : " + (awayPMTotal + homePMTotal).ToString());
            Console.WriteLine("Combined  +/- Total Avg. : " + Math.Round(Decimal.Divide((awayPMTotal + homePMTotal), gamesSimulated) / 2, 2).ToString());
            Console.WriteLine("--------------------------------");
            Console.WriteLine("Margin of Victory Total  : " + margVicDiff.ToString());
            Console.WriteLine("Margin of Victory Avg.   : " + Math.Round(Decimal.Divide(margVicDiff, gamesSimulated), 2).ToString());
            Console.WriteLine("Prediction Record        : " + gamesCorrect.ToString() + "-" + (gamesSimulated - gamesCorrect).ToString());
            Console.WriteLine("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
            Console.WriteLine("");







        }


        public static string GetBig12PredictorLog(MatchupResult pred, MatchupResult act)
        {
            string result = "";
            if (act.HomeTeamScore == 00 || act.AwayTeamScore == 00)
            {
                result = "(?)";
            }
            else
            {
                if(pred.Winner == act.Winner)
                {
                    result = "(X)";
                }
                else
                {
                    result = "( )";
                }
            }
            result += " " + pred.AwayTeam + " " + pred.AwayTeamScore.ToString() + "-" + pred.HomeTeamScore.ToString() + " " + pred.HomeTeam;
            //display the plus minuses
            //int awayPM = pred.AwayTeamScore - act.AwayTeamScore;
            //int homePM = pred.HomeTeamScore - act.HomeTeamScore;
            //result += " (" + awayPM.ToString() + " , " + homePM.ToString() + ")";
            return result;
        }

        public static MatchupResult ProcessBig12Matchup(MatchupResult r, int times, int a2pt, int a3pt, int aturn, int afoul)
        {
            int aTotal = 0;
            int hTotal = 0;

            int aWins = 0;
            int hWins = 0;

            for (int i = 1; i <= times; i++)
            {
                DataModels.MatchupResult res = DataController.RunMatchup(r.AwayTeam, r.HomeTeam, a2pt, a3pt, aturn, afoul, GameSpeed.Instant, "2025", false);
                aTotal += res.AwayTeamScore;
                hTotal += res.HomeTeamScore;
                if(res.Winner == r.AwayTeam)
                {
                    aWins++;
                }
                else
                {
                    hWins++;
                }
            }
            int awayScore = (aTotal / times);
            int homeScore = (hTotal / times);
            MatchupResult result = new MatchupResult();
            result.AwayTeam = r.AwayTeam;
            result.AwayTeamScore = awayScore;
            result.HomeTeam = r.HomeTeam;
            result.HomeTeamScore = homeScore;
            if(awayScore > homeScore)
            {
                result.Winner = r.AwayTeam;
                result.WinnerScore = awayScore;
                result.Loser = r.HomeTeam;
                result.LoserScore = homeScore;
            }
            else
            {
                result.Winner = r.HomeTeam;
                result.WinnerScore = homeScore;
                result.Loser = r.AwayTeam;
                result.LoserScore = awayScore;
            }
            return result;
        }

        public static void RunMultiMatchup(string team1, string team2, int times)
        {
            int iTotal = 0;
            int uTotal = 0;

            int iWins = 0;
            int uWins = 0;

            for (int i = 1; i <= times; i++)
            {
                DataModels.MatchupResult res = DataController.RunMatchup(team1, team2, -12, -12, 0, -5, GameSpeed.Instant, "2026", false);
                if (res.Winner == team1)
                {
                    Console.WriteLine(i.ToString() + ". (W) " + team1 + " " + res.WinnerScore.ToString() + "-" + res.LoserScore.ToString() + " " + team2);
                    iTotal += res.WinnerScore;
                    uTotal += res.LoserScore;
                    iWins++;
                }
                else
                {
                    Console.WriteLine(i.ToString() + ". (L) " + team1 + " " + res.LoserScore.ToString() + "-" + res.WinnerScore.ToString() + " " + team2);
                    uTotal += res.WinnerScore;
                    iTotal += res.LoserScore;
                    uWins++;
                }
            }
            Console.WriteLine("");
            Console.WriteLine("Totals: ");
            Console.WriteLine(team1 + " win chance: " + ((iWins * 100) / times) + "%" + " (" + iWins + "-" + uWins + " total wins)");
            Console.WriteLine("Average score: " + team1 + " " + (iTotal / times).ToString() + "-" + (uTotal / times).ToString() + " " + team2);
        }

        public static async Task<List<CollegeModel>> ScrapeLive2025Data()
        {
            try
            {
                List<CollegeModel> res = new List<CollegeModel>();
                HtmlWeb web = new HtmlWeb();
                HtmlDocument doc = new HtmlDocument();
                try
                {
                    doc = web.Load("https://barttorvik.com/#");
                }
                catch(Exception ex)
                {
                    Console.WriteLine("EXCEPTION: " + ex.Message);
                }
                
                List<HtmlNode> rows = doc.DocumentNode.Descendants("tr").ToList();
                foreach(HtmlNode row in rows)
                {
                    try
                    {
                        CollegeModel curr = new CollegeModel();
                        List<HtmlNode> cells = row.Descendants("td").ToList();
                        curr.Rank = Convert.ToInt32(cells[0].InnerText);
                        string nameText = cells[1].InnerText;
                        if (nameText.Contains("&"))
                        {
                            nameText = nameText.Substring(0, nameText.IndexOf("&"));
                        }
                        curr.Name = nameText;
                        curr.Conference = cells[2].InnerText;
                        curr.GamesPlayed = Convert.ToInt32(cells[3].InnerText);
                        curr.Wins = 0;
                        curr.Losses = 0;
                        curr.ADJOE = Convert.ToDecimal(cells[5].InnerText);
                        curr.ADJDE = Convert.ToDecimal(cells[6].InnerText);
                        curr.BARTHAG = Convert.ToDecimal(cells[7].InnerText);
                        curr.EFG_O = Convert.ToDecimal(cells[8].InnerText);
                        curr.EFG_D = Convert.ToDecimal(cells[9].InnerText);
                        curr.TOR_O = Convert.ToDecimal(cells[10].InnerText);
                        curr.TOR_D = Convert.ToDecimal(cells[11].InnerText);
                        curr.ORB = Convert.ToDecimal(cells[12].InnerText);
                        curr.DRB = Convert.ToDecimal(cells[13].InnerText);
                        curr.FTR_O = Convert.ToDecimal(cells[14].InnerText);
                        curr.FTR_D = Convert.ToDecimal(cells[15].InnerText);
                        curr.PT2_O = Convert.ToDecimal(cells[16].InnerText);
                        curr.PT2_D = Convert.ToDecimal(cells[17].InnerText);
                        curr.PT3_O = Convert.ToDecimal(cells[18].InnerText);
                        curr.PT3_D = Convert.ToDecimal(cells[19].InnerText);
                        curr.ADJ_T = Convert.ToDecimal(cells[22].InnerText);
                        curr.WAB = Convert.ToDecimal(cells[23].InnerText);

                        //free throws
                        //this will get set later
                        curr.FTP = 0; 
                        res.Add(curr);

                    }
                    catch(Exception ex)
                    {
                        //just skip the team if they have a error.
                    }
                    
                    
                }
                return res;

            }
            catch(Exception ex)
            {
                return new List<CollegeModel>();
            }
        }

        public static MatchupResult CreateMatchupResult(string awayTeam, int awayTeamScore, string homeTeam, int homeTeamScore)
        {
            MatchupResult res = new MatchupResult();
            res.AwayTeam = awayTeam;
            res.AwayTeamScore = awayTeamScore;
            res.HomeTeam = homeTeam;
            res.HomeTeamScore = homeTeamScore;
            if (res.AwayTeamScore > res.HomeTeamScore)
            {
                res.Winner = awayTeam;
                res.WinnerScore = awayTeamScore;
                res.Loser = homeTeam;
                res.LoserScore = homeTeamScore;
            }
            else
            {
                res.Winner = homeTeam;
                res.WinnerScore = homeTeamScore;
                res.Loser = awayTeam;
                res.LoserScore = awayTeamScore;
            }
            res.Overtimes = 0;
            return res;
        }

        public static void UpdateFreeThrow(CollegeModel team)
        {
            //first check and see if team already has FTP
            if(team.FTP != 0)
            {
                //FTP is already set, we outta here
                return;
            }

            string urlTeamName = team.Name.Replace(" ", "+");
            if (urlTeamName.Contains("'"))
            {
                urlTeamName = urlTeamName.Replace("'", "%27");
            }

            if (urlTeamName.Contains("&"))
            {
                urlTeamName = urlTeamName.Replace("&", "%26");
            }

            HtmlWeb w = new HtmlWeb();
            HtmlDocument teamDoc = w.Load("https://barttorvik.com/team.php?team=" + urlTeamName + "&year=2025");
            HtmlNode ftpDiv = teamDoc.GetElementbyId("ft_per");
            string ftpString = ftpDiv.InnerText;
            decimal ftp = Convert.ToDecimal(ftpString);
            team.FTP = ftp;
            if(team.FTP == 0)
            {
                //the FTP didn't set, so default it
                team.FTP = 70;
            }
        }

    }
}
