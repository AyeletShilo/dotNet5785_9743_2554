
namespace DalTest;

using DalApi;
using DO;
using Microsoft.VisualBasic;
using System.Xml.Linq;

public static class Initialization
{
    private static IAssignment? s_Assignment; //stage 1
    private static ICall? s_dalCall; //stage 1
    private static IVolunteer? s_dalVolunteer; //stage 1
    private static IConfig? s_dalConfig; //stage 1

    private static readonly Random s_rand = new();

    private static void createAssignment()
    {

    }
    private static void createCall()
    {

        string[] address =
                { "Hess 20,Haifa,Israel","Oren 19, Haifa,Israel","Moshe Sne 29, Jerusalem, Israel","Barzilai 5, Haifa,Israel","Nordau 43, Netanya,Israel"
                    ,"Habanaei 27, Jerusalem, Israel","Ben Yehuda 112, Netanya, Israel","Rav Kook 22, Dimona, Israel","Hashomer 1, Tiberias, Israel","Rotem 15, Lod, Israel" };

        for (int i = 0; i < 10; i++)
        {

            string location = address[i];

            int rType = s_rand.Next(0, 3);
            Type type;
            switch (rType)
            {
                case 0:
                    //
                    break;
                case 1:
                    //
                    break;
                //
                case 2:
                    //
                    break;

                default:
                    //
                    break;
            }

            DateTime openTime = new DateTime(s_dalConfig.Clock; //stage 1
            string? description =
            DateTime ? maxTime =





              s_dalCall!.Create(new());
        }
    }

    private static void createVolunteer()
    {
        string emailExt1 = @"@gmail.com", emailExt2 = @"@walla.co.il", emailExt3 = @"@g.jct.ac.il", sityCun = @"Jerusalem, Israel";
        string[] fullName =
            {"Amit Nakesh", "Nir Kuda", "Beni Mus", "Beti Bam","Avi Ron", "Gila Zahav", "Ram Kol", "Chani Chaim","Poli Din","Maya Gido","Ori Gami", "Dina Barzily" };
        string[] Emails =
        {"amit234{0}", "nirkush{1}", "cbh{0}", "bamb{0}" ,"54avi{1}","gila43{0}" , "rkol{2}", "chch{0}" , "poli{0}", "mgido{2}", "ogami{1}", "barzilay{2}", emailExt1,emailExt2,emailExt3};
        string[] Address =
        {$@"Hanarkis 3,{sityCun}", $@"Yafo 210,{sityCun}", $@"Brazil 101,{sityCun}", $@"Nechama 32,{sityCun}", $@"Bayit Va-Gan 46,{sityCun}", $@"Hapisga 90,{sityCun}", $@"Bar-Lev 119,{sityCun}", $@"Najara 19,{sityCun}",$@"Agron 5,{sityCun}", $@"HIlel 67,{sityCun}",$@"Iben-Ezra 27,{sityCun}", $@"Rivka 118,{sityCun}",$@"Daniel 41,{sityCun}", "Ha-Sigalit 6,{sityCun}",$@"Dagan 5,{sityCun}"};
        int i = 0;
        foreach (var name in fullName)
        {
            int id;
            do
                id = calculId();
            //id = s_rand.Next(20000000, 40000000));
            while (s_dalVolunteer!.Read(id) != null);
            string emails = Emails[i];
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
            string phoneNumber = String.Join(phonPre, s_rand.Next(1000000, 10000000));
            double? latitude = null;
            double? longitude = null;
            toSwitch = s_rand.Next(0, 2);
            Role job = (toSwitch % 2 == 0) ? Role.Manager : Role.Donater;
            string? volAddress = Address[i++];
            toSwitch = s_rand.Next(0, 2);
            bool active = (toSwitch % 2 == 0) ? true : false;
            double? maxDistance = null;
            toSwitch = s_rand.Next(1, 4);
            switch (toSwitch)
            {
                case 1:
                    Range typeDis = Range.Walking;
                    break;
                case 2:
                    Range typeDis = Range.Car;
                    break;
                case 3:
                    Range typeDis = Range.Air;
                    break;
            }
            s_dalVolunteer!.Create(new(id, name, phoneNumber,emails,job,active, distance, volAddress,latitude,longitude,maxDistance));
        }


        //Range Distance, //extra
        //string? password, //extra

    }

    private int calculId()
    {
        int randId = s_rand.Next(20000000, 40000000), tmpId = randId, digits = 0, sumDigits = 0, tmpSum = 0;
        for (int i = 0; i < 9; i++)
        {
            digits = tmpId % 10 * ((i % 2 == 0) ? 2 : 1);
            sumDigits += (digits > 9) ? digits % 10 + digits / 10 : digits;
            tmpId /= 10;
        }
        return randId * 10 + 10 - (sumDigits % 10);
    }
}


