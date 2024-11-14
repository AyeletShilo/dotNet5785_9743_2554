using DalApi;
using System.Net.Mail;

namespace DalTest
{
    internal class Program
    {
        static void Main(string[] args)
        {

          private static IConfig? s_dalConfig; //stage 1
        private static ICall? s_dalCall; //stage 1
        private static readonly Random s_rand = new();
        private static void createCall()
        {

            string[] address =
                    { "Hess 20,Haifa,Israel","Oren 19, Haifa,Israel","Moshe Sena 29, Jerusalem, Israel","Barzilai 5, Haifa,Israel","Nordau 43, Netanya,Israel"
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
    }
}
    


