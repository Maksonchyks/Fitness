using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Workout.Domain.Enums
{
    public enum ExerciseType
    {
        // -- 1 ГРУДИ
        BenchPress = 101,
        InclineDumbbellPress = 102,
        ChestFly = 103,
        DumbbellPress = 104,

        // - 2xx: СПИНА
        DeadLift = 201,
        SeatedCableRow = 202,
        PullUps = 203,
        LatPulldown = 204,
        BentOverRow = 205,

        // - 3xx: НОГИ
        BackSquat = 301,
        RomanianDeadlift = 302,
        LegExtension = 303,
        LegPress = 304,
        HackSquat = 305,
        LegCurl = 306,
        GoodMorning = 307,

        // - 4xx: ДЕЛЬТИ 
        OverheadPress = 401,      
        LateralRaise = 402,       
        FacePull = 403,
        ShoulderPress = 404,

        // - 5xx: БІЦЕПС 
        BarbellCurl = 501,
        HammerCurl = 502,
        ConcentrationCurl = 503,

        // - 6xx: ТРІЦЕПС
        TricepsPushdown = 601,
        CloseGripBenchPress = 602,
        SkullCrusher = 603,

        // - 7 ПРЕС
        Plank = 701,                
        HangingLegRaise = 702,      
        AbWheelRollout = 703,       

        // --- 8 ІКРИ 
        StandingCalfRaise = 801,  
        SeatedCalfRaise = 802,    
        DonkeyCalfRaise = 803,    

        // 9 ПЕРЕДПЛІЧЧЯ
        FarmersWalk = 901,
        WristCurl = 902,
        ReverseBarbellCurl = 903
    }
}
