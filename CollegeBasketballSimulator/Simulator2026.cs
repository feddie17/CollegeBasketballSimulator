using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using System.Xml.XPath;
using static CollegeBasketballSimulator.DataModels;

namespace CollegeBasketballSimulator
{
    //all 2026 code will be contained within this file.
    class Simulator2026
    {

        //this method will scrape the data from the csv file stored on the Bart Torvik site.
        public static List<CollegeModel> ScrapeData2026()
        {
            try
            {
                List<string> allLines = File.ReadAllLines("C:\\Users\\bfeddersen\\Desktop\\root\\TorvikData\\trank_data.csv").Skip(1).ToList();
                List<CollegeModel> results = new List<CollegeModel>();

                foreach(string line in allLines)
                {
                    try
                    {
                        string[] columns = line.Split(',');
                        CollegeModel curr = new CollegeModel();
                        curr.Rank = 0;
                        curr.CustomRankAdjuster = 0;
                        curr.Name = columns[0].Trim();
                        curr.Name = curr.Name.Replace("\"", "");
                        curr.Conference = "";
                        curr.GamesPlayed = 0;
                        curr.Wins = 0;
                        curr.Losses = 0;
                        curr.ADJOE = Convert.ToDecimal(columns[1]);
                        curr.ADJDE = Convert.ToDecimal(columns[2]);
                        curr.BARTHAG = Convert.ToDecimal(columns[3]);
                        curr.EFG_O = Convert.ToDecimal(columns[7]);
                        curr.EFG_D = Convert.ToDecimal(columns[8]);
                        curr.TOR_O = Convert.ToDecimal(columns[11]);
                        curr.TOR_D = Convert.ToDecimal(columns[12]);
                        curr.ORB = Convert.ToDecimal(columns[13]);
                        curr.DRB = Convert.ToDecimal(columns[14]);
                        curr.FTR_O = Convert.ToDecimal(columns[9]);
                        curr.FTR_D = Convert.ToDecimal(columns[10]);
                        curr.PT2_O = Convert.ToDecimal(columns[16]);
                        curr.PT2_D = Convert.ToDecimal(columns[17]);
                        curr.PT3_O = Convert.ToDecimal(columns[18]);
                        curr.PT3_D = Convert.ToDecimal(columns[19]);
                        curr.ADJ_T = Convert.ToDecimal(columns[15]);
                        curr.WAB = Convert.ToDecimal(columns[34]);
                        curr.FTP = Convert.ToDecimal(columns[35]);
                        if(curr.FTP == 0)
                        {
                            //default ft% if 0
                            curr.FTP = 70;
                        }
                        results.Add(curr);
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine("---Error Logging System: Failed to scrape 2026 data row.---");
                        Console.WriteLine(ex.Message);
                    }
                }
                //we have the data, assign the starting rank
                results = results.OrderByDescending(x => x.BARTHAG).ToList();
                int rank = 1;
                foreach(CollegeModel r in results)
                {
                    r.Rank = rank;
                    //set the initial custom adjustment.  this way, teams who are ranked will start off getting the benefit of the doubt in the rankings
                    r.CustomRankAdjuster = (decimal)(36.6 - (r.Rank / 10)); //bigger number for lower rank
                    rank++;
                }
                return results;
            }
            catch (Exception ex)
            {
                Console.WriteLine("---Error Logging System: Failed to scrape 2026 data.---");
                Console.WriteLine(ex.Message);
                return new List<CollegeModel>();
            }
        }

        public static List<ScheduleGame> Scrape2026Schedule()
        {
            List<string> allLines = File.ReadAllLines("C:\\Users\\bfeddersen\\Desktop\\root\\TorvikData\\2026_super_sked.csv").ToList();
            List<ScheduleGame> results = new List<ScheduleGame>();
            foreach (string line in allLines)
            {
                try
                {
                    string[] columns = line.Split(',');
                    ScheduleGame curr = new ScheduleGame();
                    //assign names
                    curr.AwayTeam = columns[9].Trim();
                    curr.AwayTeam = curr.AwayTeam.Replace("\"", "");
                    curr.HomeTeam = columns[15].Trim();
                    curr.HomeTeam = curr.HomeTeam.Replace("\"", "");
                    //assign date
                    string dateString = columns[1].Trim();
                    DateTime dateDT = Convert.ToDateTime(dateString);
                    curr.GameDate = dateDT;
                    //assign conference game designation
                    curr.ConferenceGame = false;
                    
                    string confColumn = columns[2].Trim();
                    if (confColumn.Contains("vs."))
                    {
                        string conf1 = confColumn.Split("vs.")[0].Trim();
                        string conf2 = confColumn.Split("vs.")[1].Trim();
                        if (conf1 == conf2)
                        {
                            curr.ConferenceGame = true;
                        }
                    }
                    else
                    {
                        string conf1 = confColumn.Split("at")[0].Trim();
                        string conf2 = confColumn.Split("at")[1].Trim();
                        if (conf1 == conf2)
                        {
                            curr.ConferenceGame = true;
                        }
                    }
                     
                    results.Add(curr);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("---Error Logging System: Failed to scrape 2026 schedule row.---");
                    Console.WriteLine(ex.Message);
                }
            }
            results = results.OrderBy(x => x.GameDate).ToList();
            return results;
        }

        public static void SimulateFullSchedule()
        {

            //get all the colleges.
            List<CollegeModel> data = ScrapeData2026();
            //get schedule.
            List<ScheduleGame> schedule = Scrape2026Schedule();
            //get favorite teams.
            List<string> favoriteTeams = new List<string>();
            bool loop = true;
            while (loop)
            {
                string input;
                Console.WriteLine("Enter team to track, or enter 'done': ");
                input = Console.ReadLine();
                if(input == "done")
                {
                    loop = false;
                }
                else
                {
                    favoriteTeams.Add(input);
                }
            }

            //init the first deadline date to the first Sunday of the season.
            DateTime deadlineDate = Convert.ToDateTime("11/9/2025");
            int week = 1;
            //loop through schedule.
            foreach(ScheduleGame s in schedule)
            {
                //if we made it passed the deadline, do a week break.
                if (DateTime.Compare(s.GameDate, deadlineDate) > 0)
                {
                    PrintTop25(data, week, favoriteTeams, deadlineDate);
                    bool loop2 = true;
                    while (loop2)
                    {
                        string input2;
                        Console.WriteLine("Enter 'b12' to see Big 12 standings, 'b10' to see Big 10 standings, or 'go' to continue: ");
                        input2 = Console.ReadLine();
                        if (input2 == "go")
                        {
                            loop2 = false;
                        }
                        else if(input2 == "b12")
                        {
                            PrintBig12Standings(data, favoriteTeams);
                        }
                        else if(input2 == "b10")
                        {
                            PrintBig10Standings(data, favoriteTeams);
                        }
                    }
                    //move the next deadline date to the next Sunday.
                    deadlineDate = deadlineDate.AddDays(7);
                    week++;
                }

                CollegeModel homeTeam = data.Where(x => x.Name == s.HomeTeam).FirstOrDefault();
                CollegeModel awayTeam = data.Where(x => x.Name == s.AwayTeam).FirstOrDefault();
                //only run matchup if we find both teams.
                if(homeTeam != null && awayTeam != null)
                {
                    //simulate game
                    MatchupResult res = DataController.RunMatchup(awayTeam.Name, homeTeam.Name, -15, -15, 0, -5, GameSpeed.Instant, "2026", false);
                    //update data
                    CollegeModel winner = data.Where(x => x.Name == res.Winner).FirstOrDefault();
                    CollegeModel loser = data.Where(x => x.Name == res.Loser).FirstOrDefault();
                    winner.Wins++;
                    loser.Losses++;
                    decimal winnerAdjustment = (decimal)(36.6 - (loser.Rank / 10)); //bigger number for lower rank = more reward for win
                    winnerAdjustment = winnerAdjustment / 10; //only 1/10 of original value
                    winner.CustomRankAdjuster += winnerAdjustment;
                    decimal loserAdjustment = (decimal)(winner.Rank / 10); //smaller number for lower rank = less punishment for loss
                    loserAdjustment = loserAdjustment / 10; //only 1/10 of original value
                    loser.CustomRankAdjuster -= loserAdjustment;
                    if(s.ConferenceGame == true)
                    {
                        winner.ConfWins++;
                        loser.ConfLosses++;
                    }

                    //set overtime string
                    string otString = "";
                    if(res.Overtimes > 0)
                    {
                        if(res.Overtimes > 1)
                        {
                            otString = " " + res.Overtimes.ToString() + "OT";
                        }
                        else
                        {
                            otString = " OT";
                        }
                    }
                    //set favorite string and wait time
                    int waitTime = 10;
                    string favoriteString = "";
                    if(favoriteTeams.Contains(res.Winner) || favoriteTeams.Contains(res.Loser)){
                        favoriteString = " <---------------------------------------";
                        waitTime = 3000;
                    }
                    if(res.Winner == res.HomeTeam)
                    {
                        //home team won
                        Console.WriteLine("(" + res.WinnerScore.ToString() + "-" + res.LoserScore.ToString() + ") " + res.Winner + " vs. " + res.Loser + otString + favoriteString);
                    }
                    else
                    {
                        //away team won
                        Console.WriteLine("(" + res.WinnerScore.ToString() + "-" + res.LoserScore.ToString() + ") " + res.Winner + " @ " + res.Loser + otString + favoriteString);
                    }
                    //pause
                    Thread.Sleep(waitTime);
                    //increment game number.

                }
            }
            Console.WriteLine("-----------------------------------");
            Console.WriteLine("THE SEASON IS OVER. FINAL RANKINGS:");
            Console.WriteLine("-----------------------------------");
            PrintBig10Standings(data, favoriteTeams);
            PrintBig12Standings(data, favoriteTeams);
            PrintTop25(data, week + 1, favoriteTeams, deadlineDate);
            
        }

        public static void PrintTop25(List<CollegeModel> data, int week, List<string> favoriteTeams, DateTime deadlineDate)
        {
            List<CollegeModel> tempData = data.OrderByDescending(x => x.CustomRankAdjuster).ThenByDescending(x => x.BARTHAG).Take(25).ToList();
            Console.WriteLine("-----------------------------------------");
            Console.WriteLine("Top 25 Rankings Week " + week.ToString() + " (" + deadlineDate.ToShortDateString() + ")");
            int count = 1;
            foreach(CollegeModel t in tempData)
            {
                string favString = "";
                if (favoriteTeams.Contains(t.Name))
                {
                    favString = " <---------------------------------------";
                }
                Console.WriteLine(count.ToString() + ". " + t.Name + " [" + t.Wins + "-" + t.Losses + "]" + favString);
                count++;
            }
            Console.WriteLine("-----------------------------------------");
        }

        public static void PrintBig12Standings(List<CollegeModel> data, List<string> favoriteTeams)
        {
            List<string> confTeams = new List<string>{ "Houston", "Arizona", "BYU", "Iowa St.", "Kansas", "Texas Tech", "Cincinnati", "Baylor", "Oklahoma St.", "West Virginia", "TCU", "Arizona St.", "Kansas St.", "UCF", "Utah", "Colorado" };
            List<CollegeModel> confData = data.Where(x => confTeams.Contains(x.Name)).ToList();
            confData = confData.OrderByDescending(x => x.ConfWins).ThenBy(x => x.ConfLosses).ThenByDescending(x => x.Wins).ThenBy(x => x.Losses).ToList();
            Console.WriteLine("-----------------------------------------");
            Console.WriteLine("Big 12 Rankings: ");
            int count = 1;
            foreach (CollegeModel t in confData)
            {
                string favString = "";
                if (favoriteTeams.Contains(t.Name))
                {
                    favString = " <---------------------------------------";
                }
                Console.WriteLine(count.ToString() + ". " + t.Name + " [" + t.ConfWins + "-" + t.ConfLosses + "] (" + t.Wins + "-" + t.Losses + ")" + favString);
                count++;
            }
            Console.WriteLine("-----------------------------------------");
        }
        public static void PrintBig10Standings(List<CollegeModel> data, List<string> favoriteTeams)
        {
            List<string> confTeams = new List<string> { "Illinois", "Michigan", "Purdue", "UCLA", "Ohio St.", "Michigan St.", "USC", "Indiana", "Wisconsin", "Iowa", "Oregon", "Nebraska", "Washington", "Maryland", "Northwestern", "Minnesota", "Rutgers", "Penn St." };
            List<CollegeModel> confData = data.Where(x => confTeams.Contains(x.Name)).ToList();
            confData = confData.OrderByDescending(x => x.ConfWins).ThenBy(x => x.ConfLosses).ThenByDescending(x => x.Wins).ThenBy(x => x.Losses).ToList();
            Console.WriteLine("-----------------------------------------");
            Console.WriteLine("Big 10 Rankings: ");
            int count = 1;
            foreach (CollegeModel t in confData)
            {
                string favString = "";
                if (favoriteTeams.Contains(t.Name))
                {
                    favString = " <---------------------------------------";
                }
                Console.WriteLine(count.ToString() + ". " + t.Name + " [" + t.ConfWins + "-" + t.ConfLosses + "] (" + t.Wins + "-" + t.Losses + ")" + favString);
                count++;
            }
            Console.WriteLine("-----------------------------------------");
        }

        public static void SimulateBig12Season()
        {
            List<ConferenceTeam> data = InitBig12Teams();
            for(var week = 1; week <= 19; week++)
            {
                string[] weekInputs = GetBig12WeekData(week);
                RunConferenceWeek(weekInputs, data);
                PrintLeagueStandings(data, week);
                bool loop = true;
                while (loop)
                {
                    string input;
                    input = Console.ReadLine();
                    if (input == "go")
                    {
                        loop = false;
                    }
                }
            }
        }

        public static List<ConferenceTeam> InitBig12Teams()
        {
            List<ConferenceTeam> results = new List<ConferenceTeam>();
            results.Add(new ConferenceTeam { TeamName = "West Virginia", Wins = 0, Losses = 0 });
            results.Add(new ConferenceTeam { TeamName = "Iowa St.", Wins = 0, Losses = 0 });
            results.Add(new ConferenceTeam { TeamName = "Colorado", Wins = 0, Losses = 0 });
            results.Add(new ConferenceTeam { TeamName = "Houston", Wins = 0, Losses = 0 });
            results.Add(new ConferenceTeam { TeamName = "Arizona St.", Wins = 0, Losses = 0 });
            results.Add(new ConferenceTeam { TeamName = "Cincinnati", Wins = 0, Losses = 0 });
            results.Add(new ConferenceTeam { TeamName = "Kansas", Wins = 0, Losses = 0 });
            results.Add(new ConferenceTeam { TeamName = "UCF", Wins = 0, Losses = 0 });
            results.Add(new ConferenceTeam { TeamName = "Baylor", Wins = 0, Losses = 0 });
            results.Add(new ConferenceTeam { TeamName = "TCU", Wins = 0, Losses = 0 });
            results.Add(new ConferenceTeam { TeamName = "BYU", Wins = 0, Losses = 0 });
            results.Add(new ConferenceTeam { TeamName = "Kansas St.", Wins = 0, Losses = 0 });
            results.Add(new ConferenceTeam { TeamName = "Oklahoma St.", Wins = 0, Losses = 0 });
            results.Add(new ConferenceTeam { TeamName = "Texas Tech", Wins = 0, Losses = 0 });
            results.Add(new ConferenceTeam { TeamName = "Arizona", Wins = 0, Losses = 0 });
            results.Add(new ConferenceTeam { TeamName = "Utah", Wins = 0, Losses = 0 });
            return results;
        }

        public static List<ConferenceTeam> InitBig10Teams()
        {
            List<ConferenceTeam> results = new List<ConferenceTeam>();
            results.Add(new ConferenceTeam { TeamName = "Northwestern", Wins = 0, Losses = 0 });
            results.Add(new ConferenceTeam { TeamName = "Oregon", Wins = 0, Losses = 0 });
            results.Add(new ConferenceTeam { TeamName = "USC", Wins = 0, Losses = 0 });
            results.Add(new ConferenceTeam { TeamName = "Ohio St.", Wins = 0, Losses = 0 });
            results.Add(new ConferenceTeam { TeamName = "UCLA", Wins = 0, Losses = 0 });
            results.Add(new ConferenceTeam { TeamName = "Purdue", Wins = 0, Losses = 0 });
            results.Add(new ConferenceTeam { TeamName = "Penn St.", Wins = 0, Losses = 0 });
            results.Add(new ConferenceTeam { TeamName = "Wisconsin", Wins = 0, Losses = 0 });
            results.Add(new ConferenceTeam { TeamName = "Washington", Wins = 0, Losses = 0 });
            results.Add(new ConferenceTeam { TeamName = "Michigan St.", Wins = 0, Losses = 0 });
            results.Add(new ConferenceTeam { TeamName = "Michigan", Wins = 0, Losses = 0 });
            results.Add(new ConferenceTeam { TeamName = "Maryland", Wins = 0, Losses = 0 });
            results.Add(new ConferenceTeam { TeamName = "Illinois", Wins = 0, Losses = 0 });
            results.Add(new ConferenceTeam { TeamName = "Iowa", Wins = 0, Losses = 0 });
            results.Add(new ConferenceTeam { TeamName = "Minnesota", Wins = 0, Losses = 0 });
            results.Add(new ConferenceTeam { TeamName = "Indiana", Wins = 0, Losses = 0 });
            results.Add(new ConferenceTeam { TeamName = "Nebraska", Wins = 0, Losses = 0 });
            results.Add(new ConferenceTeam { TeamName = "Rutgers", Wins = 0, Losses = 0 });
            return results;
        }

        public static void PrintLeagueStandings(List<ConferenceTeam> data, int week)
        {
            data = data.OrderByDescending(x => x.Wins).ThenBy(x => x.Losses).ToList();
            Console.WriteLine("Conference Standings Through Week " + week.ToString() + ". Type 'go' to simulate the next week.");
            Console.WriteLine("------------------------------------");
            foreach(ConferenceTeam d in data)
            {
                Console.WriteLine("[" + d.Wins.ToString() + "-" + d.Losses.ToString() + "] " + d.TeamName);
            }
            Console.WriteLine("------------------------------------");
        }

        public static void RunConferenceWeek(string[] inputs, List<ConferenceTeam> data)
        {

            MatchupResult res1 = DataController.RunMatchup(inputs[0], inputs[1], -15, -15, 0, -5, GameSpeed.Instant, "2026", false);
            MatchupResult res2 = DataController.RunMatchup(inputs[2], inputs[3], -15, -15, 0, -5, GameSpeed.Instant, "2026", false);
            MatchupResult res3 = DataController.RunMatchup(inputs[4], inputs[5], -15, -15, 0, -5, GameSpeed.Instant, "2026", false);
            MatchupResult res4 = DataController.RunMatchup(inputs[6], inputs[7], -15, -15, 0, -5, GameSpeed.Instant, "2026", false);
            MatchupResult res5 = DataController.RunMatchup(inputs[8], inputs[9], -15, -15, 0, -5, GameSpeed.Instant, "2026", false);
            MatchupResult res6 = DataController.RunMatchup(inputs[10], inputs[11], -15, -15, 0, -5, GameSpeed.Instant, "2026", false);
            data.Where(x => x.TeamName == res1.Winner).FirstOrDefault().Wins += 1;
            data.Where(x => x.TeamName == res1.Loser).FirstOrDefault().Losses += 1;
            data.Where(x => x.TeamName == res2.Winner).FirstOrDefault().Wins += 1;
            data.Where(x => x.TeamName == res2.Loser).FirstOrDefault().Losses += 1;
            data.Where(x => x.TeamName == res3.Winner).FirstOrDefault().Wins += 1;
            data.Where(x => x.TeamName == res3.Loser).FirstOrDefault().Losses += 1;
            data.Where(x => x.TeamName == res4.Winner).FirstOrDefault().Wins += 1;
            data.Where(x => x.TeamName == res4.Loser).FirstOrDefault().Losses += 1;
            data.Where(x => x.TeamName == res5.Winner).FirstOrDefault().Wins += 1;
            data.Where(x => x.TeamName == res5.Loser).FirstOrDefault().Losses += 1;
            data.Where(x => x.TeamName == res6.Winner).FirstOrDefault().Wins += 1;
            data.Where(x => x.TeamName == res6.Loser).FirstOrDefault().Losses += 1;
            Console.WriteLine(res1.AwayTeam + " (" + res1.AwayTeamScore + "-" + res1.HomeTeamScore + ") " + res1.HomeTeam);
            Thread.Sleep(1000);
            Console.WriteLine(res2.AwayTeam + " (" + res2.AwayTeamScore + "-" + res2.HomeTeamScore + ") " + res2.HomeTeam);
            Thread.Sleep(1000);
            Console.WriteLine(res3.AwayTeam + " (" + res3.AwayTeamScore + "-" + res3.HomeTeamScore + ") " + res3.HomeTeam);
            Thread.Sleep(1000);
            Console.WriteLine(res4.AwayTeam + " (" + res4.AwayTeamScore + "-" + res4.HomeTeamScore + ") " + res4.HomeTeam);
            Thread.Sleep(1000);
            Console.WriteLine(res5.AwayTeam + " (" + res5.AwayTeamScore + "-" + res5.HomeTeamScore + ") " + res5.HomeTeam);
            Thread.Sleep(1000);
            Console.WriteLine(res6.AwayTeam + " (" + res6.AwayTeamScore + "-" + res6.HomeTeamScore + ") " + res6.HomeTeam);
            Thread.Sleep(1000);

            if (inputs.Length > 12)
            {
                MatchupResult res7 = DataController.RunMatchup(inputs[12], inputs[13], -15, -15, 0, -5, GameSpeed.Instant, "2026", false);
                data.Where(x => x.TeamName == res7.Winner).FirstOrDefault().Wins += 1;
                data.Where(x => x.TeamName == res7.Loser).FirstOrDefault().Losses += 1;
                Console.WriteLine(res7.AwayTeam + " (" + res7.AwayTeamScore + "-" + res7.HomeTeamScore + ") " + res7.HomeTeam);
                Thread.Sleep(1000);
            }
            if(inputs.Length > 14)
            {
                MatchupResult res8 = DataController.RunMatchup(inputs[14], inputs[15], -15, -15, 0, -5, GameSpeed.Instant, "2026", false);
                data.Where(x => x.TeamName == res8.Winner).FirstOrDefault().Wins += 1;
                data.Where(x => x.TeamName == res8.Loser).FirstOrDefault().Losses += 1;
                Console.WriteLine(res8.AwayTeam + " (" + res8.AwayTeamScore + "-" + res8.HomeTeamScore + ") " + res8.HomeTeam);
                Thread.Sleep(1000);
            }
            if (inputs.Length > 16)
            {
                MatchupResult res9 = DataController.RunMatchup(inputs[16], inputs[17], -15, -15, 0, -5, GameSpeed.Instant, "2026", false);
                data.Where(x => x.TeamName == res9.Winner).FirstOrDefault().Wins += 1;
                data.Where(x => x.TeamName == res9.Loser).FirstOrDefault().Losses += 1;
                Console.WriteLine(res9.AwayTeam + " (" + res9.AwayTeamScore + "-" + res9.HomeTeamScore + ") " + res9.HomeTeam);
                Thread.Sleep(1000);
            }
            Console.WriteLine("");
        }

        public static string[] GetBig12WeekData(int week)
        {
            if(week == 1)
            {
                return ["West Virginia", "Iowa St.", "Colorado", "Arizona St.", "Houston", "Cincinnati", "Kansas", "UCF", "Baylor", "TCU", "BYU", "Kansas St.", "Oklahoma St.", "Texas Tech", "Arizona", "Utah"];
            }
            else if(week == 2)
            {
                return ["Cincinnati", "West Virginia", "Texas Tech", "Houston", "UCF", "Oklahoma St.", "TCU", "Kansas", "Arizona St.", "BYU", "Iowa St.", "Baylor", "Utah", "Colorado", "Kansas St.", "Arizona"];
            }
            else if (week == 3)
            {
                return ["Kansas St.", "Arizona St.", "Kansas", "West Virginia", "Arizona", "TCU", "Houston", "Baylor", "Oklahoma St.", "Iowa St.", "BYU", "Utah", "Texas Tech", "Colorado", "Cincinnati", "UCF"];
            }
            else if (week == 4)
            {
                return ["Iowa St.", "Kansas", "Baylor", "Oklahoma St.", "West Virginia", "Houston", "Colorado", "Cincinnati", "TCU", "BYU", "UCF", "Kansas St.", "Utah", "Texas Tech", "Arizona St.", "Arizona"];
            }
            else if (week == 5)
            {
                return ["Baylor", "Kansas", "Iowa St.", "Cincinnati", "Arizona", "UCF", "Colorado", "West Virginia", "Kansas St.", "Oklahoma St.", "BYU", "Texas Tech", "TCU", "Utah", "Arizona St.", "Houston"];
            }
            else if (week == 6)
            {
                return ["Texas Tech", "Baylor", "Utah", "Kansas St.", "UCF", "Iowa St.", "Oklahoma St.", "TCU", "Kansas", "Colorado", "West Virginia", "Arizona St.", "Cincinnati", "Arizona"];
            }
            else if (week == 7)
            {
                return ["Utah", "BYU", "Cincinnati", "Arizona St.", "TCU", "Baylor", "Kansas", "Kansas St.", "Iowa St.", "Oklahoma St.", "Houston", "Texas Tech", "West Virginia", "Arizona", "UCF", "Colorado"];
            }
            else if (week == 8)
            {
                return ["Arizona", "BYU", "Arizona St.", "UCF", "Kansas St.", "West Virginia", "Baylor", "Cincinnati", "Houston", "TCU", "Colorado", "Iowa St."];
            }
            else if (week == 9)
            {
                return ["Arizona", "Arizona St.", "Texas Tech", "UCF", "Baylor", "West Virginia", "BYU", "Kansas", "Cincinnati", "Houston", "Oklahoma St.", "Utah", "Iowa St.", "Kansas St.", "TCU", "Colorado"];
            }
            else if (week == 10)
            {
                return ["Kansas", "Texas Tech", "BYU", "Oklahoma St.", "UCF", "Houston", "Colorado", "Baylor", "Arizona St.", "Utah", "West Virginia", "Cincinnati"];
            }
            else if (week == 11)
            {
                return ["Houston", "BYU", "Kansas St.", "TCU", "Utah", "Kansas", "Baylor", "Iowa St.", "Oklahoma St.", "Arizona", "Arizona St.", "Colorado", "UCF", "Cincinnati", "Texas Tech", "West Virginia"];
            }
            else if (week == 12)
            {
                return ["Arizona", "Kansas", "Oklahoma St.", "Arizona St.", "BYU", "Baylor", "Iowa St.", "TCU", "Houston", "Utah", "Colorado", "Texas Tech", "Cincinnati", "Kansas St."];
            }
            else if (week == 13)
            {
                return ["West Virginia", "UCF", "Colorado", "BYU", "Kansas St.", "Houston", "TCU", "Oklahoma St.", "Kansas", "Iowa St.", "Texas Tech", "Arizona", "Utah", "Cincinnati"];
            }
            else if (week == 14)
            {
                return ["Houston", "Iowa St.", "TCU", "UCF", "Texas Tech", "Arizona St.", "Baylor", "Kansas St.", "Utah", "West Virginia", "Kansas", "Oklahoma St.", "BYU", "Arizona"];
            }
            else if (week == 15)
            {
                return ["Iowa St.", "BYU", "Arizona St.", "Baylor", "Arizona", "Houston", "Cincinnati", "Kansas", "Kansas St.", "Texas Tech", "West Virginia", "TCU", "Oklahoma St.", "Colorado", "UCF", "Utah"];
            }
            else if (week == 16)
            {
                return ["Houston", "Kansas", "UCF", "BYU", "Arizona", "Baylor", "Arizona St.", "TCU", "West Virginia", "Oklahoma St.", "Cincinnati", "Texas Tech", "Iowa St.", "Utah", "Kansas St.", "Colorado"];
            }
            else if (week == 17)
            {
                return ["Baylor", "UCF", "Utah", "Arizona St.", "Oklahoma St.", "Cincinnati", "BYU", "West Virginia", "Colorado", "Houston", "TCU", "Kansas St.", "Texas Tech", "Iowa St.", "Kansas", "Arizona"];
            }
            else if (week == 18)
            {
                return ["Iowa St.", "Arizona", "BYU", "Cincinnati", "Oklahoma St.", "UCF", "Kansas", "Arizona St.", "West Virginia", "Kansas St.", "TCU", "Texas Tech", "Colorado", "Utah", "Baylor", "Houston"];
            }
            else if (week == 19)
            {
                return ["UCF", "West Virginia", "Texas Tech", "BYU", "Utah", "Baylor", "Houston", "Oklahoma St.", "Cincinnati", "TCU", "Kansas St.", "Kansas", "Arizona St.", "Iowa St.", "Arizona", "Colorado"];
            }
            return [""];
        }

        public class ConferenceTeam
        {
            public string TeamName { get; set; }
            public int Wins { get; set; }
            public int Losses { get; set; }
        }

        public class ScheduleGame
        {
            public string AwayTeam { get; set; }
            public string HomeTeam { get; set; }
            public DateTime GameDate { get; set; }
            public bool ConferenceGame { get; set; }
        }

    }
}
