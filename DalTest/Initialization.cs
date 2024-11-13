
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
   


        for(int i=0; i<10; i++)
        {
            string[] studentNames =
            { "Dani Levy", "Eli Amar", "Yair Cohen", "Ariela Levin", "Dina Klein", "Hazit 5 Sdarot" };

            Type callType =
            string callAddress
            double latitude
            double longitude
            DateTime openTime
            string? description =
            DateTime? maxTime =





            s_dalCall!.Create(new());
        }
        private static void createVolunteer()
    {

    }

}
