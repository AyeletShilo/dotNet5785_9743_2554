
namespace DalTest;

using DalApi;
using DO;
using Microsoft.VisualBasic;
using System;
using System.Xml.Linq;

public static partial class Initialization
{
    private static IAssignment? s_Assignment; //stage 1
    private static ICall? s_dalCall; //stage 1
    private static IVolunteer? s_dalVolunteer; //stage 1
    private static IConfig? s_dalConfig; //stage 1

    private static readonly Random s_rand = new();

    private static partial void createAssignment()
    {

    }
    private static void createCall()
    {
        string location = "Jerusalem, Israel";
        string[] addresses =
          { $"Jaffa 23, {location}", $"Ben Yehuda 12, {location}", $"Herzl Boulevard 45, {location}", $"Hanevi'im 37, {location}", $"King David 10, {location}", $"Emek Refaim 28, {location}",
            $"Azza 15, {location}", $"King George 32, {location}", $"Rothschild Boulevard 5, {location}", $"Shmuel Hanavi 14, {location}", $"Hillel 8, {location}", $"Agron 18, {location}",
            $"Bezalel 9, {location}", $"Hagra 6, {location}", $"Derech Hevron 50, {location}", $"Derech Beit Lehem 77, {location}", $"Hatzvi 3, {location}", $"Palmach 20, {location}", $"Misgav Ladach 1, {location}",
            $"Carmel 13, {location}",, $"Shahal 22, {location}", $"Beit Hadfus 7, {location}", $"Naomi 19, {location}", $"Michael 2, {location}", $"Arlozorov 11, {location}", $"Jabotinsky 24, {location}",
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


          
   Type callType,
   string callAddress,
   double latitud = latitudes[i];
            double longitude = longitudes[i];
            DateTime openTime = new DateTime(s_dalConfig.Clock);
            string? Description = null,
   DateTime? MaxTime = null
        }
        //for (int i = 0; i < 10; i++)
        //{

        //    string location = addresses[i];

        //    int rType = s_rand.Next(0, 3);
        //    Type type;
        //    switch (rType)
        //    {
        //        case 0:
        //            //
        //            break;
        //        case 1:
        //            //
        //            break;
        //        //
        //        case 2:
        //            //
        //            break;

        //        default:
        //            //
        //            break;
        //    }

        //    DateTime openTime = new DateTime(s_dalConfig.Clock; //stage 1
        //    string? description =
        //    DateTime ? maxTime =





        s_dalCall!.Create(new());
    }


    private static void createVolunteer()
    {
        string emailExt1 = @"@gmail.com", emailExt2 = @"@walla.co.il", emailExt3 = @"@g.jct.ac.il", location = @"Jerusalem, Israel";
        int i = 0;
        string[] fullNames =
            {"Amit Nakesh", "Nir Kuda", "Beni Mus", "Beti Bam","Avi Ron", "Gila Zahav", "Ram Kol", "Chani Chaim","Poli Din","Maya Gido","Ori Gami", "Dina Barzily" };
        string[] Emails =
        {"amit234{0}", "nirkush{1}", "cbh{0}", "bamb{0}" ,"54avi{1}","gila43{0}" , "rkol{2}", "chch{0}" , "poli{0}", "mgido{2}", "ogami{1}", "barzilay{2}", emailExt1,emailExt2,emailExt3};
        string[] Addresses =
        {$"Ha-Narkis 3,{location}", $"Jaffa 210,{location}", $"Brazil 101,{location}", $"Nechama 32,{location}", $"Bayit Va-Gan 46,{location}", $"Ha-Pisga 90,{location}",
            $"Bar-Lev 119,{location}", $"Najara 19,{location}",$"Agron 5,{location}", $"Hilel 67,{location}",$"Iben-Ezra 27,{location}", $"Rivka 118,{location}",
            $"Daniel 41,{location}", $"Ha-Sigalit 6,{location}", $"Dagan 5,{location}"};
        double[] latitudes =
        {   31.7771, 31.7814, 31.7859, 31.7790, 31.7798, 31.7742, 31.7725, 31.7633, 31.7808,
            31.7723, 31.7727, 31.7731, 31.7756, 31.7664, 31.7650  };
        double[] longitudes =
        {   35.2255, 35.2277, 35.2090, 35.2201, 35.2280, 35.2116, 35.2212, 35.2083, 35.2139,
            35.2134, 35.2128, 35.2123, 35.2167, 35.2150, 35.2143  };

        foreach (var name in fullNames)
        {
            int id;
            do
                //id = calculId();
                id = s_rand.Next(20000000, 40000000));
            while (s_dalVolunteer!.Read(id) != null) ;
            string email = Emails[i];
            int toSwitch = s_rand.Next(1, 5);
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
            toSwitch = s_rand.Next(0, 2);
            Role job = (toSwitch % 2 == 0) ? Role.Manager : Role.Donater;
            string? volAddress = Addresses[i++];
            toSwitch = s_rand.Next(0, 2);
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
            s_dalVolunteer!.Create(new(id, name, phoneNumber, email, job, active, typeDis, volAddress, latitude, longitude, maxDistance));
        }


        //Range Distance, //extra
        //string? password, //extra

    }

    //private int calculId()
    //{
    //    int randId = s_rand.Next(20000000, 40000000), tmpId = randId, digits = 0, sumDigits = 0, tmpSum = 0;
    //    for (int i = 0; i < 9; i++)
    //    {
    //        digits = tmpId % 10 * ((i % 2 == 0) ? 2 : 1);
    //        sumDigits += (digits > 9) ? digits % 10 + digits / 10 : digits;
    //        tmpId /= 10;
    //    }
    //    return randId * 10 + 10 - (sumDigits % 10);
    //}
}


