
namespace DalTest;

using DalApi;
using DO;
using Microsoft.VisualBasic;
using System;
using System.Xml.Linq;

public static class Initialization
{
    //private static IAssignment? s_dalAssignment; //stage 1
    //private static ICall? s_dalCall; //stage 1
    //private static IVolunteer? s_dalVolunteer; //stage 1
    //private static IConfig? s_dalConfig; //stage 1

    private static IDal? s_dal;//stage 2

    private static readonly Random s_rand = new();

    private static void createAssignment()
    {
        //List<Call>? existingCall = s_dalCall?.ReadAll(); //stage1
        //List<Volunteer>? existingVol = s_dalVolunteer?.ReadAll(); //stage1
        IEnumerable<Call>? existingCall = s_dal.Call?.ReadAll(); //stage2
        IEnumerable<Volunteer>? existingVol = s_dal.Volunteer?.ReadAll(); //stage2
        int counter = 0;

        foreach (Call item in existingCall)
        {
            int callId = item.Id;
            Random rand = new Random();
            //int randomV = rand.Next(existingVol.Count); //stage1
            //int volunteerId = existingVol[randomV].Id; //stage1

            counter = rand.Next(1, 50);
            int count = 0;
            Volunteer selected = default;

            
            foreach (var vol in existingVol)
            {
                count++;
                if (rand.Next(count) == 0) 
                {
                    selected = vol;
                }
            }
            int volunteerId = selected.Id;

            DateTime start = item.OpenTime;
            DateTime? end = item.MaxTime;
            int time = 1;
            if (end is null)
                end = new DateTime(2026,01,01);
            else
                time = (end - start).Value.Days;
            
            DateTime entry = start.AddDays(rand.Next(time));


            DateTime? stop;
            AssignmentEnum? stopEnum;
            if (counter > 23)
            {
                int timeToStop = (end - entry).Value.Days;
                stop = entry.AddDays(rand.Next(timeToStop));
                if (counter > 41)
                    stopEnum = AssignmentEnum.TakenCare;
                else if (counter > 33)
                    stopEnum = AssignmentEnum.SelfCancel;
                else
                    stopEnum = AssignmentEnum.CancelAdmin;
            }
            else if (counter > 10)
            {
                stop = entry.AddDays(rand.Next(30,100));
                stopEnum = AssignmentEnum.CancelExpired;
            }
            else
            {
                stop = null;
                stopEnum = null;
            }

            Assignment newA = new(0, callId, volunteerId, entry, stop, stopEnum);
            //s_dalAssignment.Create(newA); //stage1
            s_dal.Assignment.Create(newA); //stage2
        }
    }
    private static void createCall()
    {
        string location = "Jerusalem, Israel";
        string[] addresses =
          { $"Jaffa 23, {location}", $"Ben Yehuda 12, {location}", $"Herzl Boulevard 45, {location}", $"Hanevi'im 37, {location}", $"King David 10, {location}", $"Emek Refaim 28, {location}",
            $"Azza 15, {location}", $"King George 32, {location}", $"Rothschild Boulevard 5, {location}", $"Shmuel Hanavi 14, {location}", $"Hillel 8, {location}", $"Agron 18, {location}",
            $"Bezalel 9, {location}", $"Hagra 6, {location}", $"Derech Hevron 50, {location}", $"Derech Beit Lehem 77, {location}", $"Hatzvi 3, {location}", $"Palmach 20, {location}", $"Misgav Ladach 1, {location}",
            $"Carmel 13, {location}", $"Shahal 22, {location}", $"Beit Hadfus 7, {location}", $"Naomi 19, {location}", $"Michael 2, {location}", $"Arlozorov 11, {location}", $"Jabotinsky 24, {location}",
            $"Chopin 5, {location}", $"Haturim 17, {location}", $"Tchernichovsky 14, {location}", $"Mordechai Alkahi 7, {location}", $"David Remez 4, {location}",$"Mendele Mokher Sfarim 10, {location}",
            $"Nissim Behar 12, {location}", $"Ha-Rav Kook 6, {location}", $"Ein Gedi 16, {location}", $"Yishayahu 18, {location}", $"Shaked 13, {location}", $"Geula 10, {location}", $"Hanurit 8, {location}",
            $"Hanarkisim 15, {location}", $"Hapa'amon 2, {location}", $"Golda Meir Boulevard 68, {location}", $"Yad Harutsim 4, {location}", $"Hagan 6, {location}", $"Ma'aleh Hazeytim 19, {location}", $"Habika 21, {location}",
            $"Nof Harim 27, {location}", $"Derech Hagefen 35, {location}"   };
        double[] latitudes =
          { 31.7812, 31.7818, 31.7748, 31.7854, 31.7755, 31.7619, 31.7707, 31.7810, 31.7752,
            31.8032, 31.7802, 31.7767, 31.7770, 31.7696, 31.7516, 31.7513, 31.7671, 31.7715,
            31.7831, 31.7778, 31.7780, 31.7879, 31.7743, 31.7762, 31.7735, 31.7678, 31.7692,
            31.7645, 31.7653, 31.7669, 31.7758, 31.7630, 31.7720, 31.7705, 31.7621, 31.7587,
            31.7734, 31.7813, 31.7857, 31.7775, 31.7811, 31.7840, 31.7777, 31.7749, 31.7904,
            31.7911, 31.7945, 31.7698, 31.7732, 31.7696, 31.7520, 31.7643   };

        double[] longitudes =
        {
            35.2202, 35.2194, 35.2002, 35.2217, 35.2256, 35.2315, 35.2139, 35.2161, 35.2297,
            35.2128, 35.2187, 35.2163, 35.2155, 35.2105, 35.2151, 35.2147, 35.2091, 35.2085,
            35.2173, 35.2186, 35.2113, 35.2094, 35.2078, 35.2146, 35.2165, 35.2189, 35.2122,
            35.2145, 35.2153, 35.2118, 35.2192, 35.2183, 35.2205, 35.2090, 35.2074, 35.2137,
            35.2169, 35.2172, 35.2149, 35.2180, 35.2173, 35.2168, 35.2194, 35.2218, 35.2227,
            35.2211, 35.2142, 35.2159, 35.2108, 35.2124, 35.2102
        };
        int i = 0;
        foreach (var address in addresses)
        {
            TypeOfCall callType = TypeOfCall.shopping;
            string? description = null;
            int rType = s_rand.Next(5);
            switch (rType)
            {
                case 0:
                    callType = TypeOfCall.repairing;
                    description = "Certain repairs in the house - replacing a light bulb, plumbing work and minor renovations.";
                    break;
                case 1:
                    callType = TypeOfCall.cleaning;
                    description = "Floor washing, kitchen cleaning, dust removal, and toilet cleaning.";
                    break;
                case 2:
                    callType = TypeOfCall.technologyHelp;
                    description = "Learning and helping with technology - connecting to social networks, installing the Internet, dealing with viruses, learning how to use it.";
                    break;
                case 3:
                    callType = TypeOfCall.talking;
                    description = "An hour or more of conversation to relieve loneliness";
                    break;
                case 4:
                    callType = TypeOfCall.shopping;
                    description = "Doing weekly shopping at the local supermarket.";
                    break;
                default:
                    callType = TypeOfCall.shopping;
                    break;
            }
            double latitude = latitudes[i];
            double longitude = longitudes[i++];
            DateTime openTime = GenerateOpeningTime();
            //DateTime? MaxTime = null;
            rType=s_rand.Next(0, 2);

            DateTime? maxTime;
            do
                maxTime = (rType % 2 == 0) ? null : GenerateEndingTime();
            while (maxTime < openTime);


           // s_dalCall!.Create(new(rType, callType, address, latitude, longitude, openTime, description, maxTime)); //stage1
            s_dal!.Call.Create(new(rType, callType, address, latitude, longitude, openTime, description, maxTime)); //stage2
        }
    }
    private static void createVolunteer()
    {
        string location = @"Jerusalem, Israel";
        string[] emailsEnds = { @"@gmail.com", @"@walla.co.il", @"@g.jct.ac.il" };
        int i = 0;
        string[] fullNames =
            {"Amit Nakesh", "Nir Kuda", "Beni Mus", "Beti Bam","Avi Ron", "Gila Zahav", "Ram Kol", "Chani Chaim","Poli Din","Maya Gido","Ori Gami", "Dina Barzily", "Moise Simon", "Ran Amitz", "Nurit Lady", "Jonathan Shapira" };
        string[] Emails =
        {"amit234", "nirkush", "cbh", "bamb" ,"54avi","gila43" , "rkol", "chch" , "poli", "mgido", "ogami", "barzilay", "Simoni98","rAmitz", "perach" ,"jShapira"};
        string[] Addresses =
        {$"Ha-Narkis 3,{location}", $"Jaffa 210,{location}", $"Brazil 101,{location}", $"Nechama 32,{location}", $"Bayit Va-Gan 46,{location}", $"Ha-Pisga 90,{location}",
            $"Bar-Lev 119,{location}", $"Najara 19,{location}",$"Agron 5,{location}", $"Hilel 67,{location}",$"Iben-Ezra 27,{location}", $"Rivka 118,{location}",
            $"Daniel 41,{location}", $"Ha-Sigalit 6,{location}", $"Dagan 5,{location}", $"Shmuel Hanavi 20, {location}"};
        double[] latitudes =
        {   31.7771, 31.7814, 31.7859, 31.7790, 31.7798, 31.7742, 31.7725, 31.7633, 31.7808,
            31.7723, 31.7727, 31.7731, 31.7756, 31.7664, 31.7650 ,31.8017 };
        double[] longitudes =
        {   35.2255, 35.2277, 35.2090, 35.2201, 35.2280, 35.2116, 35.2212, 35.2083, 35.2139,
            35.2134, 35.2128, 35.2123, 35.2167, 35.2150, 35.2143 , 35.2171 };

        foreach (var name in fullNames)
        {
            int id;
            do
                id = calculId();
            // id = s_rand.Next(20000000, 40000000));
            //while (s_daVolunteer!.Read(id) != null); //stage1
            while (s_dal!.Volunteer!.Read(id) != null);  //stage2

            int toSwitch = s_rand.Next(3);
            string email = Emails[i]+ emailsEnds[toSwitch];
            toSwitch = s_rand.Next(1, 5);
            string phonPre = "";
            switch (toSwitch)
            {
                case 1:
                    phonPre = "050";
                    break;
                case 2:
                    phonPre = "052";
                    break;
                case 3:
                    phonPre = "053";
                    break;
                case 4:
                    phonPre = "058";
                    break;
            }
            string phoneNumber = phonPre + s_rand.Next(1000000, 10000000);
            double? latitude = latitudes[i];
            double? longitude = longitudes[i];
            toSwitch = s_rand.Next(2);
            Role job = (toSwitch % 2 == 0) ? Role.Manager : Role.Donater;
            string? volAddress = Addresses[i++];
            toSwitch = s_rand.Next(2);
            bool active = (toSwitch % 2 == 0) ? true : false;
            double? maxDistance = null;
            RangeType typeDis;
            toSwitch = s_rand.Next(1, 4);
            switch (toSwitch)
            {
                case 1:
                    typeDis = RangeType.Walking;
                    break;
                case 2:
                    typeDis = RangeType.Car;
                    break;
                case 3:
                    typeDis = RangeType.Air;
                    break;
                default:
                    typeDis = RangeType.Air;
                    break;
            }
            //s_dalVolunteer!.Create(new(id, name, phoneNumber, email, job, active, typeDis, volAddress, latitude, longitude, maxDistance));
            s_dal.Volunteer!.Create(new(id, name, phoneNumber, email, job, active, typeDis, volAddress, latitude, longitude, maxDistance));
        }
        //string? password, //extra

    }
    public static void Do(IDal dal)
    {
        //s_dalAssignment= dalAssignment ?? throw new NullReferenceException("DAL object can not be null!"); //stage1
        //s_dalCall = dalCall ?? throw new NullReferenceException("DAL object can not be null!"); //stage1
        //s_dalConfig = dalConfig ?? throw new NullReferenceException("DAL object can not be null!"); //stage1
        //s_dalVolunteer = dalVolunteer ?? throw new NullReferenceException("DAL object can not be null!"); //stage1
        s_dal = dal ?? throw new NullReferenceException("DAL object can not be null!"); // stage 2


    Console.WriteLine("Reset Configuration values and List values...");
        //s_dalConfig.Reset(); //stage1
        //s_dalAssignment.DeleteAll(); //stage1
        //s_dalCall.DeleteAll(); //stage1
        //s_dalVolunteer.DeleteAll(); //stage1

        s_dal.ResetDB(); //stage2
                                  
        Console.WriteLine("Initializing lists ...");
        createCall();
        createVolunteer();
        createAssignment();
    }
    private static int calculId()
    {
        int randId = s_rand.Next(20000000, 40000000), tmpId = randId, digits = 0, sumDigits = 0;
        for (int i = 0; i < 9; i++)
        {
            digits = tmpId % 10 * ((i % 2 == 0) ? 2 : 1);
            sumDigits += (digits > 9) ? digits % 10 + digits / 10 : digits;
            tmpId /= 10;
        }
        return randId * 10 + 10 - (sumDigits % 10);
    }
    private static DateTime GenerateOpeningTime()
    {
        DateTime currentTime = DateTime.Now;
        double daysAgo = s_rand.Next(0, 30);
        double hoursAgo = s_rand.Next(0, 24);  
        double minutesAgo = s_rand.Next(0, 60);

        DateTime openingTime = currentTime.AddDays(-daysAgo).AddHours(-hoursAgo).AddMinutes(-minutesAgo);

        return openingTime;
    }

    private static DateTime GenerateEndingTime()
    {
        DateTime currentTime = DateTime.Now;
        double daysAgo = s_rand.Next(-15, 30);
        double hoursAgo = s_rand.Next(-23, 24);
        double minutesAgo = s_rand.Next(-59, 60);

        DateTime openingTime = currentTime.AddDays(+daysAgo).AddHours(+hoursAgo).AddMinutes(+minutesAgo).AddSeconds(0);

        return openingTime;
    }

}
