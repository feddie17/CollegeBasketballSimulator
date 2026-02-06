using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static CollegeBasketballSimulator.DataController;
using static CollegeBasketballSimulator.DataModels;

namespace CollegeBasketballSimulator
{
    public class MarchMadnessController
    {

        public static void SimulateMarchMadness2025(int times, int waitTime)
        {
            //Play-In Games
            string playin1 = "[16]SIU Edwardsville";
            string playin2 = "[16]American";
            string playin3 = "[12]Ohio St.";
            string playin4 = "[12]Oklahoma";
            string playin5 = "[11]San Diego St.";
            string playin6 = "[11]Xavier";
            string playin7 = "[16]Quinnipiac";
            string playin8 = "[16]Southern";

            Console.WriteLine("");
            Console.WriteLine("####################");
            Console.WriteLine("Play-In Games");
            Console.WriteLine("####################");
            Console.WriteLine("");
            string pis1 = RunMultiMatchupMarchMadness2025(playin1, playin2, times).Winner;
            System.Threading.Thread.Sleep(waitTime);
            string pis2 = RunMultiMatchupMarchMadness2025(playin3, playin4, times).Winner;
            System.Threading.Thread.Sleep(waitTime);
            string pis3 = RunMultiMatchupMarchMadness2025(playin5, playin6, times).Winner;
            System.Threading.Thread.Sleep(waitTime);
            string pis4 = RunMultiMatchupMarchMadness2025(playin7, playin8, times).Winner;
            System.Threading.Thread.Sleep(waitTime);


            //South Region
            string team1 = "[1]Auburn";
            string team2 = pis1;
            string team3 = "[8]Memphis";
            string team4 = "[9]West Virginia";
            string team5 = "[5]Oregon";
            string team6 = "[12]Liberty";
            string team7 = "[4]Clemson";
            string team8 = "[13]Lipscomb";
            string team9 = "[6]UCLA";
            string team10 = "[11]Drake";
            string team11 = "[3]Kentucky";
            string team12 = "[14]Troy";
            string team13 = "[7]Marquette";
            string team14 = "[10]Baylor";
            string team15 = "[2]Michigan St.";
            string team16 = "[15]Furman";

            //East Region
            string team17 = "[1]Duke";
            string team18 = pis4;
            string team19 = "[8]Connecticut";
            string team20 = "[9]Vanderbilt";
            string team21 = "[5]BYU";
            string team22 = "[12]UC San Diego";
            string team23 = "[4]Wisconsin";
            string team24 = "[13]High Point";
            string team25 = "[6]Michigan";
            string team26 = "[11]VCU";
            string team27 = "[3]Iowa St.";
            string team28 = "[14]Northern Colorado";
            string team29 = "[7]Louisville";
            string team30 = "[10]Georgia";
            string team31 = "[2]Tennessee";
            string team32 = "[15]Central Connecticut";

            //West Region
            string team33 = "[1]Florida";
            string team34 = "[16]Norfolk St.";
            string team35 = "[8]Gonzaga";
            string team36 = "[9]New Mexico";
            string team37 = "[5]Missouri";
            string team38 = "[12]McNeese St.";
            string team39 = "[4]Maryland";
            string team40 = "[13]Yale";
            string team41 = "[6]Illinois";
            string team42 = pis3;
            string team43 = "[3]Texas A";
            string team44 = "[14]Utah Valley";
            string team45 = "[7]Mississippi";
            string team46 = "[10]Utah St.";
            string team47 = "[2]Texas Tech";
            string team48 = "[15]Bryant";

            //Midwest Region
            string team49 = "[1]Houston";
            string team50 = "[16]Nebraska Omaha";
            string team51 = "[8]Mississippi St.";
            string team52 = "[9]Creighton";
            string team53 = "[5]Arizona";
            string team54 = pis2;
            string team55 = "[4]Purdue";
            string team56 = "[13]Akron";
            string team57 = "[6]Saint Mary's";
            string team58 = "[11]Indiana";
            string team59 = "[3]St. John's";
            string team60 = "[14]Towson";
            string team61 = "[7]Kansas";
            string team62 = "[10]Arkansas";
            string team63 = "[2]Alabama";
            string team64 = "[15]Robert Morris";

            //Round 1
            Console.WriteLine("");
            Console.WriteLine("####################");
            Console.WriteLine("First Round");
            Console.WriteLine("####################");
            Console.WriteLine("");
            string r1s1 = RunMultiMatchupMarchMadness2025(team1, team2, times).Winner;
            System.Threading.Thread.Sleep(waitTime);
            string r1s2 = RunMultiMatchupMarchMadness2025(team3, team4, times).Winner;
            System.Threading.Thread.Sleep(waitTime);
            string r1s3 = RunMultiMatchupMarchMadness2025(team5, team6, times).Winner;
            System.Threading.Thread.Sleep(waitTime);
            string r1s4 = RunMultiMatchupMarchMadness2025(team7, team8, times).Winner;
            System.Threading.Thread.Sleep(waitTime);
            string r1s5 = RunMultiMatchupMarchMadness2025(team9, team10, times).Winner;
            System.Threading.Thread.Sleep(waitTime);
            string r1s6 = RunMultiMatchupMarchMadness2025(team11, team12, times).Winner;
            System.Threading.Thread.Sleep(waitTime);
            string r1s7 = RunMultiMatchupMarchMadness2025(team13, team14, times).Winner;
            System.Threading.Thread.Sleep(waitTime);
            string r1s8 = RunMultiMatchupMarchMadness2025(team15, team16, times).Winner;
            System.Threading.Thread.Sleep(waitTime);
            string r1s9 = RunMultiMatchupMarchMadness2025(team17, team18, times).Winner;
            System.Threading.Thread.Sleep(waitTime);
            string r1s10 = RunMultiMatchupMarchMadness2025(team19, team20, times).Winner;
            System.Threading.Thread.Sleep(waitTime);
            string r1s11 = RunMultiMatchupMarchMadness2025(team21, team22, times).Winner;
            System.Threading.Thread.Sleep(waitTime);
            string r1s12 = RunMultiMatchupMarchMadness2025(team23, team24, times).Winner;
            System.Threading.Thread.Sleep(waitTime);
            string r1s13 = RunMultiMatchupMarchMadness2025(team25, team26, times).Winner;
            System.Threading.Thread.Sleep(waitTime);
            string r1s14 = RunMultiMatchupMarchMadness2025(team27, team28, times).Winner;
            System.Threading.Thread.Sleep(waitTime);
            string r1s15 = RunMultiMatchupMarchMadness2025(team29, team30, times).Winner;
            System.Threading.Thread.Sleep(waitTime);
            string r1s16 = RunMultiMatchupMarchMadness2025(team31, team32, times).Winner;
            System.Threading.Thread.Sleep(waitTime);
            string r1s17 = RunMultiMatchupMarchMadness2025(team33, team34, times).Winner;
            System.Threading.Thread.Sleep(waitTime);
            string r1s18 = RunMultiMatchupMarchMadness2025(team35, team36, times).Winner;
            System.Threading.Thread.Sleep(waitTime);
            string r1s19 = RunMultiMatchupMarchMadness2025(team37, team38, times).Winner;
            System.Threading.Thread.Sleep(waitTime);
            string r1s20 = RunMultiMatchupMarchMadness2025(team39, team40, times).Winner;
            System.Threading.Thread.Sleep(waitTime);
            string r1s21 = RunMultiMatchupMarchMadness2025(team41, team42, times).Winner;
            System.Threading.Thread.Sleep(waitTime);
            string r1s22 = RunMultiMatchupMarchMadness2025(team43, team44, times).Winner;
            System.Threading.Thread.Sleep(waitTime);
            string r1s23 = RunMultiMatchupMarchMadness2025(team45, team46, times).Winner;
            System.Threading.Thread.Sleep(waitTime);
            string r1s24 = RunMultiMatchupMarchMadness2025(team47, team48, times).Winner;
            System.Threading.Thread.Sleep(waitTime);
            string r1s25 = RunMultiMatchupMarchMadness2025(team49, team50, times).Winner;
            System.Threading.Thread.Sleep(waitTime);
            string r1s26 = RunMultiMatchupMarchMadness2025(team51, team52, times).Winner;
            System.Threading.Thread.Sleep(waitTime);
            string r1s27 = RunMultiMatchupMarchMadness2025(team53, team54, times).Winner;
            System.Threading.Thread.Sleep(waitTime);
            string r1s28 = RunMultiMatchupMarchMadness2025(team55, team56, times).Winner;
            System.Threading.Thread.Sleep(waitTime);
            string r1s29 = RunMultiMatchupMarchMadness2025(team57, team58, times).Winner;
            System.Threading.Thread.Sleep(waitTime);
            string r1s30 = RunMultiMatchupMarchMadness2025(team59, team60, times).Winner;
            System.Threading.Thread.Sleep(waitTime);
            string r1s31 = RunMultiMatchupMarchMadness2025(team61, team62, times).Winner;
            System.Threading.Thread.Sleep(waitTime);
            string r1s32 = RunMultiMatchupMarchMadness2025(team63, team64, times).Winner;
            System.Threading.Thread.Sleep(waitTime);

            //Round 2
            Console.WriteLine("");
            Console.WriteLine("####################");
            Console.WriteLine("Second Round");
            Console.WriteLine("####################");
            Console.WriteLine("");
            string r2s1 = RunMultiMatchupMarchMadness2025(r1s1, r1s2, times).Winner;
            System.Threading.Thread.Sleep(waitTime);
            string r2s2 = RunMultiMatchupMarchMadness2025(r1s3, r1s4, times).Winner;
            System.Threading.Thread.Sleep(waitTime);
            string r2s3 = RunMultiMatchupMarchMadness2025(r1s5, r1s6, times).Winner;
            System.Threading.Thread.Sleep(waitTime);
            string r2s4 = RunMultiMatchupMarchMadness2025(r1s7, r1s8, times).Winner;
            System.Threading.Thread.Sleep(waitTime);
            string r2s5 = RunMultiMatchupMarchMadness2025(r1s9, r1s10, times).Winner;
            System.Threading.Thread.Sleep(waitTime);
            string r2s6 = RunMultiMatchupMarchMadness2025(r1s11, r1s12, times).Winner;
            System.Threading.Thread.Sleep(waitTime);
            string r2s7 = RunMultiMatchupMarchMadness2025(r1s13, r1s14, times).Winner;
            System.Threading.Thread.Sleep(waitTime);
            string r2s8 = RunMultiMatchupMarchMadness2025(r1s15, r1s16, times).Winner;
            System.Threading.Thread.Sleep(waitTime);
            string r2s9 = RunMultiMatchupMarchMadness2025(r1s17, r1s18, times).Winner;
            System.Threading.Thread.Sleep(waitTime);
            string r2s10 = RunMultiMatchupMarchMadness2025(r1s19, r1s20, times).Winner;
            System.Threading.Thread.Sleep(waitTime);
            string r2s11 = RunMultiMatchupMarchMadness2025(r1s21, r1s22, times).Winner;
            System.Threading.Thread.Sleep(waitTime);
            string r2s12 = RunMultiMatchupMarchMadness2025(r1s23, r1s24, times).Winner;
            System.Threading.Thread.Sleep(waitTime);
            string r2s13 = RunMultiMatchupMarchMadness2025(r1s25, r1s26, times).Winner;
            System.Threading.Thread.Sleep(waitTime);
            string r2s14 = RunMultiMatchupMarchMadness2025(r1s27, r1s28, times).Winner;
            System.Threading.Thread.Sleep(waitTime);
            string r2s15 = RunMultiMatchupMarchMadness2025(r1s29, r1s30, times).Winner;
            System.Threading.Thread.Sleep(waitTime);
            string r2s16 = RunMultiMatchupMarchMadness2025(r1s31, r1s32, times).Winner;
            System.Threading.Thread.Sleep(waitTime);

            //Sweet 16
            Console.WriteLine("");
            Console.WriteLine("####################");
            Console.WriteLine("Sweet 16");
            Console.WriteLine("####################");
            Console.WriteLine("");
            string r3s1 = RunMultiMatchupMarchMadness2025(r2s1, r2s2, times).Winner;
            System.Threading.Thread.Sleep(waitTime);
            string r3s2 = RunMultiMatchupMarchMadness2025(r2s3, r2s4, times).Winner;
            System.Threading.Thread.Sleep(waitTime);
            string r3s3 = RunMultiMatchupMarchMadness2025(r2s5, r2s6, times).Winner;
            System.Threading.Thread.Sleep(waitTime);
            string r3s4 = RunMultiMatchupMarchMadness2025(r2s7, r2s8, times).Winner;
            System.Threading.Thread.Sleep(waitTime);
            string r3s5 = RunMultiMatchupMarchMadness2025(r2s9, r2s10, times).Winner;
            System.Threading.Thread.Sleep(waitTime);
            string r3s6 = RunMultiMatchupMarchMadness2025(r2s11, r2s12, times).Winner;
            System.Threading.Thread.Sleep(waitTime);
            string r3s7 = RunMultiMatchupMarchMadness2025(r2s13, r2s14, times).Winner;
            System.Threading.Thread.Sleep(waitTime);
            string r3s8 = RunMultiMatchupMarchMadness2025(r2s15, r2s16, times).Winner;
            System.Threading.Thread.Sleep(waitTime);

            //Elite 8
            Console.WriteLine("");
            Console.WriteLine("####################");
            Console.WriteLine("Elite 8");
            Console.WriteLine("####################");
            Console.WriteLine("");
            string r4s1 = RunMultiMatchupMarchMadness2025(r3s1, r3s2, times).Winner;
            System.Threading.Thread.Sleep(waitTime);
            string r4s2 = RunMultiMatchupMarchMadness2025(r3s3, r3s4, times).Winner;
            System.Threading.Thread.Sleep(waitTime);
            string r4s3 = RunMultiMatchupMarchMadness2025(r3s5, r3s6, times).Winner;
            System.Threading.Thread.Sleep(waitTime);
            string r4s4 = RunMultiMatchupMarchMadness2025(r3s7, r3s8, times).Winner;
            System.Threading.Thread.Sleep(waitTime);

            //Final 4
            Console.WriteLine("");
            Console.WriteLine("####################");
            Console.WriteLine("Final 4");
            Console.WriteLine("####################");
            Console.WriteLine("");
            string r5s1 = RunMultiMatchupMarchMadness2025(r4s1, r4s2, times).Winner;
            System.Threading.Thread.Sleep(waitTime);
            string r5s2 = RunMultiMatchupMarchMadness2025(r4s3, r4s4, times).Winner;
            System.Threading.Thread.Sleep(waitTime);

            //Championship
            Console.WriteLine("");
            Console.WriteLine("####################");
            Console.WriteLine("Championship");
            Console.WriteLine("####################");
            Console.WriteLine("");
            string r6s1 = RunMultiMatchupMarchMadness2025(r5s1, r5s2, times).Winner;

        }

        public static void SimulateMarchMadness2025Aggregate(int times)
        {
            //setup of the agg stats
            List<MarchMadnessAggModel> masterList = new List<MarchMadnessAggModel>();


            //Play-In Games
            string playin1 = "[16]Alabama St.";
            masterList.Add(new MarchMadnessAggModel {TeamName = (playin1), ChampWins = 0, ChampExits = 0, FinalFourExits = 0, EliteEightExits = 0, SweetSixteenExits = 0, SecondRoundExits = 0, FirstRoundExits = 0, PlayinExits = 0 });
            string playin2 = "[16]Saint Francis";
            masterList.Add(new MarchMadnessAggModel { TeamName = (playin2), ChampWins = 0, ChampExits = 0, FinalFourExits = 0, EliteEightExits = 0, SweetSixteenExits = 0, SecondRoundExits = 0, FirstRoundExits = 0, PlayinExits = 0 });
            string playin3 = "[11]Texas";
            masterList.Add(new MarchMadnessAggModel { TeamName = (playin3), ChampWins = 0, ChampExits = 0, FinalFourExits = 0, EliteEightExits = 0, SweetSixteenExits = 0, SecondRoundExits = 0, FirstRoundExits = 0, PlayinExits = 0 });
            string playin4 = "[11]Xavier";
            masterList.Add(new MarchMadnessAggModel { TeamName = (playin4), ChampWins = 0, ChampExits = 0, FinalFourExits = 0, EliteEightExits = 0, SweetSixteenExits = 0, SecondRoundExits = 0, FirstRoundExits = 0, PlayinExits = 0 });
            string playin5 = "[11]San Diego St.";
            masterList.Add(new MarchMadnessAggModel { TeamName = (playin5), ChampWins = 0, ChampExits = 0, FinalFourExits = 0, EliteEightExits = 0, SweetSixteenExits = 0, SecondRoundExits = 0, FirstRoundExits = 0, PlayinExits = 0 });
            string playin6 = "[11]North Carolina";
            masterList.Add(new MarchMadnessAggModel { TeamName = (playin6), ChampWins = 0, ChampExits = 0, FinalFourExits = 0, EliteEightExits = 0, SweetSixteenExits = 0, SecondRoundExits = 0, FirstRoundExits = 0, PlayinExits = 0 });
            string playin7 = "[16]American";
            masterList.Add(new MarchMadnessAggModel { TeamName = (playin7), ChampWins = 0, ChampExits = 0, FinalFourExits = 0, EliteEightExits = 0, SweetSixteenExits = 0, SecondRoundExits = 0, FirstRoundExits = 0, PlayinExits = 0 });
            string playin8 = "[16]Mount St. Mary's";
            masterList.Add(new MarchMadnessAggModel { TeamName = (playin8), ChampWins = 0, ChampExits = 0, FinalFourExits = 0, EliteEightExits = 0, SweetSixteenExits = 0, SecondRoundExits = 0, FirstRoundExits = 0, PlayinExits = 0 });


            


            //South Region
            string team1 = "[1]Auburn";
            masterList.Add(new MarchMadnessAggModel { TeamName = (team1), ChampWins = 0, ChampExits = 0, FinalFourExits = 0, EliteEightExits = 0, SweetSixteenExits = 0, SecondRoundExits = 0, FirstRoundExits = 0, PlayinExits = 0 });
            string team3 = "[8]Louisville";
            masterList.Add(new MarchMadnessAggModel { TeamName = (team3), ChampWins = 0, ChampExits = 0, FinalFourExits = 0, EliteEightExits = 0, SweetSixteenExits = 0, SecondRoundExits = 0, FirstRoundExits = 0, PlayinExits = 0 });
            string team4 = "[9]Creighton";
            masterList.Add(new MarchMadnessAggModel { TeamName = (team4), ChampWins = 0, ChampExits = 0, FinalFourExits = 0, EliteEightExits = 0, SweetSixteenExits = 0, SecondRoundExits = 0, FirstRoundExits = 0, PlayinExits = 0 });
            string team5 = "[5]Michigan";
            masterList.Add(new MarchMadnessAggModel { TeamName = (team5), ChampWins = 0, ChampExits = 0, FinalFourExits = 0, EliteEightExits = 0, SweetSixteenExits = 0, SecondRoundExits = 0, FirstRoundExits = 0, PlayinExits = 0 });
            string team6 = "[12]UC San Diego";
            masterList.Add(new MarchMadnessAggModel { TeamName = (team6), ChampWins = 0, ChampExits = 0, FinalFourExits = 0, EliteEightExits = 0, SweetSixteenExits = 0, SecondRoundExits = 0, FirstRoundExits = 0, PlayinExits = 0 });
            string team7 = "[4]Texas A";
            masterList.Add(new MarchMadnessAggModel { TeamName = (team7), ChampWins = 0, ChampExits = 0, FinalFourExits = 0, EliteEightExits = 0, SweetSixteenExits = 0, SecondRoundExits = 0, FirstRoundExits = 0, PlayinExits = 0 });
            string team8 = "[13]Yale";
            masterList.Add(new MarchMadnessAggModel { TeamName = (team8), ChampWins = 0, ChampExits = 0, FinalFourExits = 0, EliteEightExits = 0, SweetSixteenExits = 0, SecondRoundExits = 0, FirstRoundExits = 0, PlayinExits = 0 });
            string team9 = "[6]Mississippi";
            masterList.Add(new MarchMadnessAggModel { TeamName = (team9), ChampWins = 0, ChampExits = 0, FinalFourExits = 0, EliteEightExits = 0, SweetSixteenExits = 0, SecondRoundExits = 0, FirstRoundExits = 0, PlayinExits = 0 });
            string team11 = "[3]Iowa St.";
            masterList.Add(new MarchMadnessAggModel { TeamName = (team11), ChampWins = 0, ChampExits = 0, FinalFourExits = 0, EliteEightExits = 0, SweetSixteenExits = 0, SecondRoundExits = 0, FirstRoundExits = 0, PlayinExits = 0 });
            string team12 = "[14]Lipscomb";
            masterList.Add(new MarchMadnessAggModel { TeamName = (team12), ChampWins = 0, ChampExits = 0, FinalFourExits = 0, EliteEightExits = 0, SweetSixteenExits = 0, SecondRoundExits = 0, FirstRoundExits = 0, PlayinExits = 0 });
            string team13 = "[7]Marquette";
            masterList.Add(new MarchMadnessAggModel { TeamName = (team13), ChampWins = 0, ChampExits = 0, FinalFourExits = 0, EliteEightExits = 0, SweetSixteenExits = 0, SecondRoundExits = 0, FirstRoundExits = 0, PlayinExits = 0 });
            string team14 = "[10]New Mexico";
            masterList.Add(new MarchMadnessAggModel { TeamName = (team14), ChampWins = 0, ChampExits = 0, FinalFourExits = 0, EliteEightExits = 0, SweetSixteenExits = 0, SecondRoundExits = 0, FirstRoundExits = 0, PlayinExits = 0 });
            string team15 = "[2]Michigan St.";
            masterList.Add(new MarchMadnessAggModel { TeamName = (team15), ChampWins = 0, ChampExits = 0, FinalFourExits = 0, EliteEightExits = 0, SweetSixteenExits = 0, SecondRoundExits = 0, FirstRoundExits = 0, PlayinExits = 0 });
            string team16 = "[15]Bryant";
            masterList.Add(new MarchMadnessAggModel { TeamName = (team16), ChampWins = 0, ChampExits = 0, FinalFourExits = 0, EliteEightExits = 0, SweetSixteenExits = 0, SecondRoundExits = 0, FirstRoundExits = 0, PlayinExits = 0 });

            //East Region
            string team17 = "[1]Duke";
            masterList.Add(new MarchMadnessAggModel { TeamName = (team17), ChampWins = 0, ChampExits = 0, FinalFourExits = 0, EliteEightExits = 0, SweetSixteenExits = 0, SecondRoundExits = 0, FirstRoundExits = 0, PlayinExits = 0 });
            string team19 = "[8]Mississippi St.";
            masterList.Add(new MarchMadnessAggModel { TeamName = (team19), ChampWins = 0, ChampExits = 0, FinalFourExits = 0, EliteEightExits = 0, SweetSixteenExits = 0, SecondRoundExits = 0, FirstRoundExits = 0, PlayinExits = 0 });
            string team20 = "[9]Baylor";
            masterList.Add(new MarchMadnessAggModel { TeamName = (team20), ChampWins = 0, ChampExits = 0, FinalFourExits = 0, EliteEightExits = 0, SweetSixteenExits = 0, SecondRoundExits = 0, FirstRoundExits = 0, PlayinExits = 0 });
            string team21 = "[5]Oregon";
            masterList.Add(new MarchMadnessAggModel { TeamName = (team21), ChampWins = 0, ChampExits = 0, FinalFourExits = 0, EliteEightExits = 0, SweetSixteenExits = 0, SecondRoundExits = 0, FirstRoundExits = 0, PlayinExits = 0 });
            string team22 = "[12]Liberty";
            masterList.Add(new MarchMadnessAggModel { TeamName = (team22), ChampWins = 0, ChampExits = 0, FinalFourExits = 0, EliteEightExits = 0, SweetSixteenExits = 0, SecondRoundExits = 0, FirstRoundExits = 0, PlayinExits = 0 });
            string team23 = "[4]Arizona";
            masterList.Add(new MarchMadnessAggModel { TeamName = (team23), ChampWins = 0, ChampExits = 0, FinalFourExits = 0, EliteEightExits = 0, SweetSixteenExits = 0, SecondRoundExits = 0, FirstRoundExits = 0, PlayinExits = 0 });
            string team24 = "[13]Akron";
            masterList.Add(new MarchMadnessAggModel { TeamName = (team24), ChampWins = 0, ChampExits = 0, FinalFourExits = 0, EliteEightExits = 0, SweetSixteenExits = 0, SecondRoundExits = 0, FirstRoundExits = 0, PlayinExits = 0 });
            string team25 = "[6]BYU";
            masterList.Add(new MarchMadnessAggModel { TeamName = (team25), ChampWins = 0, ChampExits = 0, FinalFourExits = 0, EliteEightExits = 0, SweetSixteenExits = 0, SecondRoundExits = 0, FirstRoundExits = 0, PlayinExits = 0 });
            string team26 = "[11]VCU";
            masterList.Add(new MarchMadnessAggModel { TeamName = (team26), ChampWins = 0, ChampExits = 0, FinalFourExits = 0, EliteEightExits = 0, SweetSixteenExits = 0, SecondRoundExits = 0, FirstRoundExits = 0, PlayinExits = 0 });
            string team27 = "[3]Wisconsin";
            masterList.Add(new MarchMadnessAggModel { TeamName = (team27), ChampWins = 0, ChampExits = 0, FinalFourExits = 0, EliteEightExits = 0, SweetSixteenExits = 0, SecondRoundExits = 0, FirstRoundExits = 0, PlayinExits = 0 });
            string team28 = "[14]Montana";
            masterList.Add(new MarchMadnessAggModel { TeamName = (team28), ChampWins = 0, ChampExits = 0, FinalFourExits = 0, EliteEightExits = 0, SweetSixteenExits = 0, SecondRoundExits = 0, FirstRoundExits = 0, PlayinExits = 0 });
            string team29 = "[7]Saint Mary's";
            masterList.Add(new MarchMadnessAggModel { TeamName = (team29), ChampWins = 0, ChampExits = 0, FinalFourExits = 0, EliteEightExits = 0, SweetSixteenExits = 0, SecondRoundExits = 0, FirstRoundExits = 0, PlayinExits = 0 });
            string team30 = "[10]Vanderbilt";
            masterList.Add(new MarchMadnessAggModel { TeamName = (team30), ChampWins = 0, ChampExits = 0, FinalFourExits = 0, EliteEightExits = 0, SweetSixteenExits = 0, SecondRoundExits = 0, FirstRoundExits = 0, PlayinExits = 0 });
            string team31 = "[2]Alabama";
            masterList.Add(new MarchMadnessAggModel { TeamName = (team31), ChampWins = 0, ChampExits = 0, FinalFourExits = 0, EliteEightExits = 0, SweetSixteenExits = 0, SecondRoundExits = 0, FirstRoundExits = 0, PlayinExits = 0 });
            string team32 = "[15]Robert Morris";
            masterList.Add(new MarchMadnessAggModel { TeamName = (team32), ChampWins = 0, ChampExits = 0, FinalFourExits = 0, EliteEightExits = 0, SweetSixteenExits = 0, SecondRoundExits = 0, FirstRoundExits = 0, PlayinExits = 0 });

            //West Region
            string team33 = "[1]Florida";
            masterList.Add(new MarchMadnessAggModel { TeamName = (team33), ChampWins = 0, ChampExits = 0, FinalFourExits = 0, EliteEightExits = 0, SweetSixteenExits = 0, SecondRoundExits = 0, FirstRoundExits = 0, PlayinExits = 0 });
            string team34 = "[16]Norfolk St.";
            masterList.Add(new MarchMadnessAggModel { TeamName = (team34), ChampWins = 0, ChampExits = 0, FinalFourExits = 0, EliteEightExits = 0, SweetSixteenExits = 0, SecondRoundExits = 0, FirstRoundExits = 0, PlayinExits = 0 });
            string team35 = "[8]Connecticut";
            masterList.Add(new MarchMadnessAggModel { TeamName = (team35), ChampWins = 0, ChampExits = 0, FinalFourExits = 0, EliteEightExits = 0, SweetSixteenExits = 0, SecondRoundExits = 0, FirstRoundExits = 0, PlayinExits = 0 });
            string team36 = "[9]Oklahoma";
            masterList.Add(new MarchMadnessAggModel { TeamName = (team36), ChampWins = 0, ChampExits = 0, FinalFourExits = 0, EliteEightExits = 0, SweetSixteenExits = 0, SecondRoundExits = 0, FirstRoundExits = 0, PlayinExits = 0 });
            string team37 = "[5]Memphis";
            masterList.Add(new MarchMadnessAggModel { TeamName = (team37), ChampWins = 0, ChampExits = 0, FinalFourExits = 0, EliteEightExits = 0, SweetSixteenExits = 0, SecondRoundExits = 0, FirstRoundExits = 0, PlayinExits = 0 });
            string team38 = "[12]Colorado St.";
            masterList.Add(new MarchMadnessAggModel { TeamName = (team38), ChampWins = 0, ChampExits = 0, FinalFourExits = 0, EliteEightExits = 0, SweetSixteenExits = 0, SecondRoundExits = 0, FirstRoundExits = 0, PlayinExits = 0 });
            string team39 = "[4]Maryland";
            masterList.Add(new MarchMadnessAggModel { TeamName = (team39), ChampWins = 0, ChampExits = 0, FinalFourExits = 0, EliteEightExits = 0, SweetSixteenExits = 0, SecondRoundExits = 0, FirstRoundExits = 0, PlayinExits = 0 });
            string team40 = "[13]Grand Canyon";
            masterList.Add(new MarchMadnessAggModel { TeamName = (team40), ChampWins = 0, ChampExits = 0, FinalFourExits = 0, EliteEightExits = 0, SweetSixteenExits = 0, SecondRoundExits = 0, FirstRoundExits = 0, PlayinExits = 0 });
            string team41 = "[6]Missouri";
            masterList.Add(new MarchMadnessAggModel { TeamName = (team41), ChampWins = 0, ChampExits = 0, FinalFourExits = 0, EliteEightExits = 0, SweetSixteenExits = 0, SecondRoundExits = 0, FirstRoundExits = 0, PlayinExits = 0 });
            string team42 = "[11]Drake";
            masterList.Add(new MarchMadnessAggModel { TeamName = (team42), ChampWins = 0, ChampExits = 0, FinalFourExits = 0, EliteEightExits = 0, SweetSixteenExits = 0, SecondRoundExits = 0, FirstRoundExits = 0, PlayinExits = 0 });
            string team43 = "[3]Texas Tech";
            masterList.Add(new MarchMadnessAggModel { TeamName = (team43), ChampWins = 0, ChampExits = 0, FinalFourExits = 0, EliteEightExits = 0, SweetSixteenExits = 0, SecondRoundExits = 0, FirstRoundExits = 0, PlayinExits = 0 });
            string team44 = "[14]UNC Wilmington";
            masterList.Add(new MarchMadnessAggModel { TeamName = (team44), ChampWins = 0, ChampExits = 0, FinalFourExits = 0, EliteEightExits = 0, SweetSixteenExits = 0, SecondRoundExits = 0, FirstRoundExits = 0, PlayinExits = 0 });
            string team45 = "[7]Kansas";
            masterList.Add(new MarchMadnessAggModel { TeamName = (team45), ChampWins = 0, ChampExits = 0, FinalFourExits = 0, EliteEightExits = 0, SweetSixteenExits = 0, SecondRoundExits = 0, FirstRoundExits = 0, PlayinExits = 0 });
            string team46 = "[10]Arkansas";
            masterList.Add(new MarchMadnessAggModel { TeamName = (team46), ChampWins = 0, ChampExits = 0, FinalFourExits = 0, EliteEightExits = 0, SweetSixteenExits = 0, SecondRoundExits = 0, FirstRoundExits = 0, PlayinExits = 0 });
            string team47 = "[2]St. John's";
            masterList.Add(new MarchMadnessAggModel { TeamName = (team47), ChampWins = 0, ChampExits = 0, FinalFourExits = 0, EliteEightExits = 0, SweetSixteenExits = 0, SecondRoundExits = 0, FirstRoundExits = 0, PlayinExits = 0 });
            string team48 = "[15]Nebraska Omaha";
            masterList.Add(new MarchMadnessAggModel { TeamName = (team48), ChampWins = 0, ChampExits = 0, FinalFourExits = 0, EliteEightExits = 0, SweetSixteenExits = 0, SecondRoundExits = 0, FirstRoundExits = 0, PlayinExits = 0 });

            //Midwest Region
            string team49 = "[1]Houston";
            masterList.Add(new MarchMadnessAggModel { TeamName = (team49), ChampWins = 0, ChampExits = 0, FinalFourExits = 0, EliteEightExits = 0, SweetSixteenExits = 0, SecondRoundExits = 0, FirstRoundExits = 0, PlayinExits = 0 });
            string team50 = "[16]SIU Edwardsville";
            masterList.Add(new MarchMadnessAggModel { TeamName = (team50), ChampWins = 0, ChampExits = 0, FinalFourExits = 0, EliteEightExits = 0, SweetSixteenExits = 0, SecondRoundExits = 0, FirstRoundExits = 0, PlayinExits = 0 });
            string team51 = "[8]Gonzaga";
            masterList.Add(new MarchMadnessAggModel { TeamName = (team51), ChampWins = 0, ChampExits = 0, FinalFourExits = 0, EliteEightExits = 0, SweetSixteenExits = 0, SecondRoundExits = 0, FirstRoundExits = 0, PlayinExits = 0 });
            string team52 = "[9]Georgia";
            masterList.Add(new MarchMadnessAggModel { TeamName = (team52), ChampWins = 0, ChampExits = 0, FinalFourExits = 0, EliteEightExits = 0, SweetSixteenExits = 0, SecondRoundExits = 0, FirstRoundExits = 0, PlayinExits = 0 });
            string team53 = "[5]Clemson";
            masterList.Add(new MarchMadnessAggModel { TeamName = (team53), ChampWins = 0, ChampExits = 0, FinalFourExits = 0, EliteEightExits = 0, SweetSixteenExits = 0, SecondRoundExits = 0, FirstRoundExits = 0, PlayinExits = 0 });
            string team54 = "[12]McNeese St.";
            masterList.Add(new MarchMadnessAggModel { TeamName = (team54), ChampWins = 0, ChampExits = 0, FinalFourExits = 0, EliteEightExits = 0, SweetSixteenExits = 0, SecondRoundExits = 0, FirstRoundExits = 0, PlayinExits = 0 });
            string team55 = "[4]Purdue";
            masterList.Add(new MarchMadnessAggModel { TeamName = (team55), ChampWins = 0, ChampExits = 0, FinalFourExits = 0, EliteEightExits = 0, SweetSixteenExits = 0, SecondRoundExits = 0, FirstRoundExits = 0, PlayinExits = 0 });
            string team56 = "[13]High Point";
            masterList.Add(new MarchMadnessAggModel { TeamName = (team56), ChampWins = 0, ChampExits = 0, FinalFourExits = 0, EliteEightExits = 0, SweetSixteenExits = 0, SecondRoundExits = 0, FirstRoundExits = 0, PlayinExits = 0 });
            string team57 = "[6]Illinois";
            masterList.Add(new MarchMadnessAggModel { TeamName = (team57), ChampWins = 0, ChampExits = 0, FinalFourExits = 0, EliteEightExits = 0, SweetSixteenExits = 0, SecondRoundExits = 0, FirstRoundExits = 0, PlayinExits = 0 });
            string team59 = "[3]Kentucky";
            masterList.Add(new MarchMadnessAggModel { TeamName = (team59), ChampWins = 0, ChampExits = 0, FinalFourExits = 0, EliteEightExits = 0, SweetSixteenExits = 0, SecondRoundExits = 0, FirstRoundExits = 0, PlayinExits = 0 });
            string team60 = "[14]Troy";
            masterList.Add(new MarchMadnessAggModel { TeamName = (team60), ChampWins = 0, ChampExits = 0, FinalFourExits = 0, EliteEightExits = 0, SweetSixteenExits = 0, SecondRoundExits = 0, FirstRoundExits = 0, PlayinExits = 0 });
            string team61 = "[7]UCLA";
            masterList.Add(new MarchMadnessAggModel { TeamName = (team61), ChampWins = 0, ChampExits = 0, FinalFourExits = 0, EliteEightExits = 0, SweetSixteenExits = 0, SecondRoundExits = 0, FirstRoundExits = 0, PlayinExits = 0 });
            string team62 = "[10]Utah St.";
            masterList.Add(new MarchMadnessAggModel { TeamName = (team62), ChampWins = 0, ChampExits = 0, FinalFourExits = 0, EliteEightExits = 0, SweetSixteenExits = 0, SecondRoundExits = 0, FirstRoundExits = 0, PlayinExits = 0 });
            string team63 = "[2]Tennessee";
            masterList.Add(new MarchMadnessAggModel { TeamName = (team63), ChampWins = 0, ChampExits = 0, FinalFourExits = 0, EliteEightExits = 0, SweetSixteenExits = 0, SecondRoundExits = 0, FirstRoundExits = 0, PlayinExits = 0 });
            string team64 = "[15]Wofford";
            masterList.Add(new MarchMadnessAggModel { TeamName = (team64), ChampWins = 0, ChampExits = 0, FinalFourExits = 0, EliteEightExits = 0, SweetSixteenExits = 0, SecondRoundExits = 0, FirstRoundExits = 0, PlayinExits = 0 });

            int count = 0;
            //we got everything initialized in the list, now start the loop
            while (count < times)
            {
                count++;

                //inside the loop
                //auburn's game
                string pis1 = RunMultiMatchupMarchMadness2025Code(playin1, playin2).Winner;
                if(count == 1) { PrintLoadProgress(1); }
                //illinois's
                string pis2 = RunMultiMatchupMarchMadness2025Code(playin3, playin4).Winner;
                if (count == 1) { PrintLoadProgress(2); }
                //old miss's game
                string pis3 = RunMultiMatchupMarchMadness2025Code(playin5, playin6).Winner;
                if (count == 1) { PrintLoadProgress(3); }
                //duke's game
                string pis4 = RunMultiMatchupMarchMadness2025Code(playin7, playin8).Winner;
                if (count == 1) { PrintLoadProgress(4); }

                string team2 = pis1;
                string team18 = pis4;
                string team10 = pis3;
                string team58 = pis2;

                //Round 1
                string r1s1 = RunMultiMatchupMarchMadness2025Code(team1, team2).Winner;
                if (count == 1) { PrintLoadProgress(5); }
                string r1s2 = RunMultiMatchupMarchMadness2025Code(team3, team4).Winner;
                if (count == 1) { PrintLoadProgress(6); }
                string r1s3 = RunMultiMatchupMarchMadness2025Code(team5, team6).Winner;
                if (count == 1) { PrintLoadProgress(7); }
                string r1s4 = RunMultiMatchupMarchMadness2025Code(team7, team8).Winner;
                if (count == 1) { PrintLoadProgress(8); }
                string r1s5 = RunMultiMatchupMarchMadness2025Code(team9, team10).Winner;
                if (count == 1) { PrintLoadProgress(9); }
                string r1s6 = RunMultiMatchupMarchMadness2025Code(team11, team12).Winner;
                if (count == 1) { PrintLoadProgress(10); }
                string r1s7 = RunMultiMatchupMarchMadness2025Code(team13, team14).Winner;
                if (count == 1) { PrintLoadProgress(11); }
                string r1s8 = RunMultiMatchupMarchMadness2025Code(team15, team16).Winner;
                if (count == 1) { PrintLoadProgress(12); }
                string r1s9 = RunMultiMatchupMarchMadness2025Code(team17, team18).Winner;
                if (count == 1) { PrintLoadProgress(13); }
                string r1s10 = RunMultiMatchupMarchMadness2025Code(team19, team20).Winner;
                if (count == 1) { PrintLoadProgress(14); }
                string r1s11 = RunMultiMatchupMarchMadness2025Code(team21, team22).Winner;
                if (count == 1) { PrintLoadProgress(15); }
                string r1s12 = RunMultiMatchupMarchMadness2025Code(team23, team24).Winner;
                if (count == 1) { PrintLoadProgress(16); }
                string r1s13 = RunMultiMatchupMarchMadness2025Code(team25, team26).Winner;
                if (count == 1) { PrintLoadProgress(17); }
                string r1s14 = RunMultiMatchupMarchMadness2025Code(team27, team28).Winner;
                if (count == 1) { PrintLoadProgress(18); }
                string r1s15 = RunMultiMatchupMarchMadness2025Code(team29, team30).Winner;
                if (count == 1) { PrintLoadProgress(19); }
                string r1s16 = RunMultiMatchupMarchMadness2025Code(team31, team32).Winner;
                if (count == 1) { PrintLoadProgress(20); }
                string r1s17 = RunMultiMatchupMarchMadness2025Code(team33, team34).Winner;
                if (count == 1) { PrintLoadProgress(21); }
                string r1s18 = RunMultiMatchupMarchMadness2025Code(team35, team36).Winner;
                if (count == 1) { PrintLoadProgress(22); }
                string r1s19 = RunMultiMatchupMarchMadness2025Code(team37, team38).Winner;
                if (count == 1) { PrintLoadProgress(23); }
                string r1s20 = RunMultiMatchupMarchMadness2025Code(team39, team40).Winner;
                if (count == 1) { PrintLoadProgress(24); }
                string r1s21 = RunMultiMatchupMarchMadness2025Code(team41, team42).Winner;
                if (count == 1) { PrintLoadProgress(25); }
                string r1s22 = RunMultiMatchupMarchMadness2025Code(team43, team44).Winner;
                if (count == 1) { PrintLoadProgress(26); }
                string r1s23 = RunMultiMatchupMarchMadness2025Code(team45, team46).Winner;
                if (count == 1) { PrintLoadProgress(27); }
                string r1s24 = RunMultiMatchupMarchMadness2025Code(team47, team48).Winner;
                if (count == 1) { PrintLoadProgress(28); }
                string r1s25 = RunMultiMatchupMarchMadness2025Code(team49, team50).Winner;
                if (count == 1) { PrintLoadProgress(29); }
                string r1s26 = RunMultiMatchupMarchMadness2025Code(team51, team52).Winner;
                if (count == 1) { PrintLoadProgress(30); }
                string r1s27 = RunMultiMatchupMarchMadness2025Code(team53, team54).Winner;
                if (count == 1) { PrintLoadProgress(31); }
                string r1s28 = RunMultiMatchupMarchMadness2025Code(team55, team56).Winner;
                if (count == 1) { PrintLoadProgress(32); }
                string r1s29 = RunMultiMatchupMarchMadness2025Code(team57, team58).Winner;
                if (count == 1) { PrintLoadProgress(33); }
                string r1s30 = RunMultiMatchupMarchMadness2025Code(team59, team60).Winner;
                if (count == 1) { PrintLoadProgress(34); }
                string r1s31 = RunMultiMatchupMarchMadness2025Code(team61, team62).Winner;
                if (count == 1) { PrintLoadProgress(35); }
                string r1s32 = RunMultiMatchupMarchMadness2025Code(team63, team64).Winner;
                if (count == 1) { PrintLoadProgress(36); }

                //Round 2
                string r2s1 = RunMultiMatchupMarchMadness2025Code(r1s1, r1s2).Winner;
                string r2s2 = RunMultiMatchupMarchMadness2025Code(r1s3, r1s4).Winner;
                string r2s3 = RunMultiMatchupMarchMadness2025Code(r1s5, r1s6).Winner;
                string r2s4 = RunMultiMatchupMarchMadness2025Code(r1s7, r1s8).Winner;
                string r2s5 = RunMultiMatchupMarchMadness2025Code(r1s9, r1s10).Winner;
                string r2s6 = RunMultiMatchupMarchMadness2025Code(r1s11, r1s12).Winner;
                string r2s7 = RunMultiMatchupMarchMadness2025Code(r1s13, r1s14).Winner;
                string r2s8 = RunMultiMatchupMarchMadness2025Code(r1s15, r1s16).Winner;
                string r2s9 = RunMultiMatchupMarchMadness2025Code(r1s17, r1s18).Winner;
                string r2s10 = RunMultiMatchupMarchMadness2025Code(r1s19, r1s20).Winner;
                string r2s11 = RunMultiMatchupMarchMadness2025Code(r1s21, r1s22).Winner;
                string r2s12 = RunMultiMatchupMarchMadness2025Code(r1s23, r1s24).Winner;
                string r2s13 = RunMultiMatchupMarchMadness2025Code(r1s25, r1s26).Winner;
                string r2s14 = RunMultiMatchupMarchMadness2025Code(r1s27, r1s28).Winner;
                string r2s15 = RunMultiMatchupMarchMadness2025Code(r1s29, r1s30).Winner;
                string r2s16 = RunMultiMatchupMarchMadness2025Code(r1s31, r1s32).Winner;

                //Sweet 16
                MatchupResult mr1 = RunMultiMatchupMarchMadness2025Code(r2s1, r2s2);
                string r3s1 = mr1.Winner;
                masterList.Where(x => x.TeamName == mr1.Loser).FirstOrDefault().SweetSixteenExits += 1;
                MatchupResult mr2 = RunMultiMatchupMarchMadness2025Code(r2s3, r2s4);
                string r3s2 = mr2.Winner;
                masterList.Where(x => x.TeamName == mr2.Loser).FirstOrDefault().SweetSixteenExits += 1;
                MatchupResult mr3 = RunMultiMatchupMarchMadness2025Code(r2s5, r2s6);
                string r3s3 = mr3.Winner;
                masterList.Where(x => x.TeamName == mr3.Loser).FirstOrDefault().SweetSixteenExits += 1;
                MatchupResult mr4 = RunMultiMatchupMarchMadness2025Code(r2s7, r2s8);
                string r3s4 = mr4.Winner;
                masterList.Where(x => x.TeamName == mr4.Loser).FirstOrDefault().SweetSixteenExits += 1;
                MatchupResult mr5 = RunMultiMatchupMarchMadness2025Code(r2s9, r2s10);
                string r3s5 = mr5.Winner;
                masterList.Where(x => x.TeamName == mr5.Loser).FirstOrDefault().SweetSixteenExits += 1;
                MatchupResult mr6 = RunMultiMatchupMarchMadness2025Code(r2s11, r2s12);
                string r3s6 = mr6.Winner;
                masterList.Where(x => x.TeamName == mr6.Loser).FirstOrDefault().SweetSixteenExits += 1;
                MatchupResult mr7 = RunMultiMatchupMarchMadness2025Code(r2s13, r2s14);
                string r3s7 = mr7.Winner;
                masterList.Where(x => x.TeamName == mr7.Loser).FirstOrDefault().SweetSixteenExits += 1;
                MatchupResult mr8 = RunMultiMatchupMarchMadness2025Code(r2s15, r2s16);
                string r3s8 = mr8.Winner;
                masterList.Where(x => x.TeamName == mr8.Loser).FirstOrDefault().SweetSixteenExits += 1;

                //Elite 8
                MatchupResult mr9 = RunMultiMatchupMarchMadness2025Code(r3s1, r3s2);
                string r4s1 = mr9.Winner;
                masterList.Where(x => x.TeamName == mr9.Loser).FirstOrDefault().EliteEightExits += 1;
                MatchupResult mr10 = RunMultiMatchupMarchMadness2025Code(r3s3, r3s4);
                string r4s2 = mr10.Winner;
                masterList.Where(x => x.TeamName == mr10.Loser).FirstOrDefault().EliteEightExits += 1;
                MatchupResult mr11 = RunMultiMatchupMarchMadness2025Code(r3s5, r3s6);
                string r4s3 = mr11.Winner;
                masterList.Where(x => x.TeamName == mr11.Loser).FirstOrDefault().EliteEightExits += 1;
                MatchupResult mr12 = RunMultiMatchupMarchMadness2025Code(r3s7, r3s8);
                string r4s4 = mr12.Winner;
                masterList.Where(x => x.TeamName == mr12.Loser).FirstOrDefault().EliteEightExits += 1;

                //Final 4
                MatchupResult mr13 = RunMultiMatchupMarchMadness2025Code(r4s1, r4s2);
                string r5s1 = mr13.Winner;
                masterList.Where(x => x.TeamName == mr13.Loser).FirstOrDefault().FinalFourExits += 1;
                MatchupResult mr14 = RunMultiMatchupMarchMadness2025Code(r4s3, r4s4);
                string r5s2 = mr14.Winner;
                masterList.Where(x => x.TeamName == mr14.Loser).FirstOrDefault().FinalFourExits += 1;

                //Championship
                MatchupResult m15 = RunMultiMatchupMarchMadness2025Code(r5s1, r5s2);
                string r6s1 = m15.Winner;
                masterList.Where(x => x.TeamName == m15.Loser).FirstOrDefault().ChampExits += 1;
                masterList.Where(x => x.TeamName == m15.Winner).FirstOrDefault().ChampWins += 1;

                List<MarchMadnessAggModel> champList = masterList.Where(x => x.ChampWins > 0).OrderByDescending(x => x.ChampWins).ToList();
                List<MarchMadnessAggModel> champGameList = masterList.Where(x => x.getChampExits() > 0).OrderByDescending(x => x.getChampExits()).ToList();
                List<MarchMadnessAggModel> finalFourList = masterList.Where(x => x.getFinalFours() > 0).OrderByDescending(x => x.getFinalFours()).ToList();
                List<MarchMadnessAggModel> eliteEightList = masterList.Where(x => x.getEliteEights() > 0).OrderByDescending(x => x.getEliteEights()).ToList();
                List<MarchMadnessAggModel> sweetSixteenList = masterList.Where(x => x.getSweetSixteens() > 0).OrderByDescending(x => x.getSweetSixteens()).ToList();

                Console.WriteLine("Results after " + count.ToString() + " sims: ");
                Console.WriteLine("########## Sweet 16s ##########");
                foreach(MarchMadnessAggModel m in sweetSixteenList)
                {
                    Console.WriteLine("(" + m.getSweetSixteens().ToString() + ") " + m.TeamName);
                }
                Console.WriteLine("");
                Console.WriteLine("########## Elite 8s ##########");
                foreach (MarchMadnessAggModel m in eliteEightList)
                {
                    Console.WriteLine("(" + m.getEliteEights().ToString() + ") " + m.TeamName);
                }
                Console.WriteLine("");
                Console.WriteLine("########## Final 4s ##########");
                foreach (MarchMadnessAggModel m in finalFourList)
                {
                    Console.WriteLine("(" + m.getFinalFours().ToString() + ") " + m.TeamName);
                }
                Console.WriteLine("");
                Console.WriteLine("########## Championship Games ##########");
                foreach (MarchMadnessAggModel m in champGameList)
                {
                    Console.WriteLine("(" + m.getChampExits().ToString() + ") " + m.TeamName);
                }
                Console.WriteLine("");
                Console.WriteLine("########## Champions ##########");
                foreach (MarchMadnessAggModel m in champList)
                {
                    Console.WriteLine("(" + m.ChampWins.ToString() + ") " + m.TeamName);
                }
                Console.WriteLine("");

            }
        }

        public static MatchupResult RunMultiMatchupMarchMadness2025(string team1, string team2, int times)
        {
            int team1Wins = 0;
            int team2Wins = 0;
            int team1TotalScore = 0;
            int team2TotalScore = 0;

            //chop off ranking
            string team1NoRank = team1.Split(']')[1];
            string team2NoRank = team2.Split(']')[1];

            for (int i = 1; i <= times; i++)
            {
                DataModels.MatchupResult res = DataController.RunMatchup(team1NoRank, team2NoRank, -15, -15, 0, -5, GameSpeed.Instant, "2026", true);
                if (res.Winner == team1NoRank)
                {
                    //team 1 wins this sim
                    team1Wins++;
                    team1TotalScore += res.WinnerScore;
                    team2TotalScore += res.LoserScore;
                }
                else
                {
                    //team 2 wins this sim
                    team2Wins++;
                    team2TotalScore += res.WinnerScore;
                    team1TotalScore += res.LoserScore;
                }
            }

            int team1FinalScore = team1TotalScore / times;
            int team2FinalScore = team2TotalScore / times;
            MatchupResult result = new MatchupResult();
            int winnerPctChance = 0;
            if(team1FinalScore > team2FinalScore)
            {
                //team 1 wins the matchup
                result.Winner = team1;
                result.WinnerScore = team1FinalScore;
                result.Loser = team2;
                result.LoserScore = team2FinalScore;
                result.Overtimes = 0;
                winnerPctChance = (team1Wins * 100) / times;
            }
            else if(team2FinalScore > team1FinalScore)
            {
                //team 2 wins the matchup
                result.Winner = team2;
                result.WinnerScore = team2FinalScore;
                result.Loser = team1;
                result.LoserScore = team1FinalScore;
                result.Overtimes = 0;
                winnerPctChance = (team2Wins * 100) / times;
            }
            else
            {
                //the team average scores are tied, do an individual matchup to determine the winner.
                DataModels.MatchupResult tiebreakerRes = DataController.RunMatchup(team1NoRank, team2NoRank, -15, -15, 0, -5, GameSpeed.Instant, "2026", true);
                if(tiebreakerRes.Winner == team1NoRank)
                {
                    //team 1 wins via tiebreak
                    result.Winner = team1;
                    result.WinnerScore = team1FinalScore + 1;
                    result.Loser = team2;
                    result.LoserScore = team2FinalScore;
                    result.Overtimes = 1;
                    winnerPctChance = (team1Wins * 100) / times;
                }
                else
                {
                    //team 2 wins via tiebreak
                    result.Winner = team2;
                    result.WinnerScore = team2FinalScore + 1;
                    result.Loser = team1;
                    result.LoserScore = team1FinalScore;
                    result.Overtimes = 1;
                    winnerPctChance = (team2Wins * 100) / times;
                }
            }

            string otString = "";
            if(result.Overtimes > 0)
            {
                otString = " (OT)";
            }
            Console.WriteLine("(" + winnerPctChance.ToString() + "% confident) " + result.Winner + " " + result.WinnerScore.ToString() + "-" + result.LoserScore.ToString() + " " + result.Loser + otString);
            return result;
        }

        public static MatchupResult RunMultiMatchupMarchMadness2025Code(string team1, string team2)
        {
            //chop off ranking
            string team1NoRank = team1.Split(']')[1];
            string team2NoRank = team2.Split(']')[1];


            DataModels.MatchupResult res = DataController.RunMatchup(team1NoRank, team2NoRank, -10, -10, 0, 0, GameSpeed.Instant, "2025", true);
            MatchupResult result = new MatchupResult();
            if (res.Winner == team1NoRank)
            {
                //team 1 wins this sim
                result.Winner = team1;
                result.Loser = team2;
            }
            else
            {
                //team 2 wins this sim
                result.Winner = team2;
                result.Loser = team1;
            }
            return result;
            
        }

        public static string TrimTeamName(string teamName)
        {
            return teamName.Split(']')[1];
        }
        public static void PrintLoadProgress(int gameNumber)
        {
            string res = "[";
            int whiteSpace = 36 - gameNumber;
            for (int i = 0; i < gameNumber; i++)
            {
                res += "=";
            }
            for (int i = 0; i < whiteSpace; i++)
            {
                res += " ";
            }
            res += "]";
            Console.WriteLine(res);
        }
    }

    public class MarchMadnessAggModel
    {
        public string TeamName { get; set; }
        public int ChampWins { get; set; }
        public int ChampExits { get; set; }
        public int FinalFourExits { get; set; }
        public int EliteEightExits { get; set; }
        public int SweetSixteenExits { get; set; }
        public int SecondRoundExits { get; set; }
        public int FirstRoundExits { get; set; }
        public int PlayinExits { get; set; }

        public int getChampExits()
        {
            return this.ChampWins + this.ChampExits;
        }
        public int getFinalFours()
        {
            return this.ChampWins + this.ChampExits + this.FinalFourExits;
        }
        public int getEliteEights()
        {
            return this.ChampWins + this.ChampExits + this.FinalFourExits + this.EliteEightExits;
        }
        public int getSweetSixteens()
        {
            return this.ChampWins + this.ChampExits + this.FinalFourExits + this.EliteEightExits + this.SweetSixteenExits;
        }
    }
}
