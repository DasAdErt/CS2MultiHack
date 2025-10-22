using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CS2MultiHack
{
    public class Entity
    {
        public List<Vector3> bones { get; set; }
        public List<Vector2> bones2D { get; set; }
        public Vector3 position { get; set; }
        public Vector3 viewOffset { get; set; }
        public Vector3[] bone { get; set; }
        public Vector2[] bone2D { get; set; }
        public Vector2 position2D { get; set; }
        public Vector2 viewPosition2D { get; set; }
        public int team { get; set; }
        public bool spotted { get; set; }
        public bool scopped { get; set; }
        public string name { get; set; }
        public short currentWeaponIndex { get; set; }
        public string currentWeaponName { get; set; }
        public int health { get; set; }
        public float distance { get; set; }
        public float[] pixelDistance { get; set; }
        public IntPtr pawnAddress { get; set; }
        public IntPtr controllerAddress { get; set; }
        public uint lifeState { get; set; }
    }

    public static class Weapon
    {
        public static bool IsSniper(short currentWeaponIndex)
        {
            return currentWeaponIndex == (short)WeaponIds.Awp || currentWeaponIndex == (short)WeaponIds.Scar20 || currentWeaponIndex == (short)WeaponIds.G3sg1 || currentWeaponIndex == (short)WeaponIds.Ssg08;
        }

        public static bool IsRifle(short id)
        {
            return id == (short)WeaponIds.Ak47
                || id == (short)WeaponIds.Aug
                || id == (short)WeaponIds.Famas
                || id == (short)WeaponIds.Galilar
                || id == (short)WeaponIds.M4a1
                || id == (short)WeaponIds.M4a1_silencer
                || id == (short)WeaponIds.Sg556;
        }

        public static bool IsSMG(short id)
        {
            return id == (short)WeaponIds.Mac10
                || id == (short)WeaponIds.Mp7
                || id == (short)WeaponIds.Mp9
                || id == (short)WeaponIds.Bizon
                || id == (short)WeaponIds.P90
                || id == (short)WeaponIds.Ump;
        }

        public static bool IsAutoSniper(short id)
        {
            return id == (short)WeaponIds.G3sg1
                || id == (short)WeaponIds.Scar20;
        }

        public static bool IsSSG08(short id) => id == (short)WeaponIds.Ssg08;

        public static bool IsAWP(short id) => id == (short)WeaponIds.Awp;

        public static bool IsPistol(short id)
        {
            return id == (short)WeaponIds.Deagle
                || id == (short)WeaponIds.Elite
                || id == (short)WeaponIds.Fiveseven
                || id == (short)WeaponIds.Glock
                || id == (short)WeaponIds.Usp_silencer
                || id == (short)WeaponIds.Cz75a
                || id == (short)WeaponIds.Revolver
                || id == (short)WeaponIds.P250
                || id == (short)WeaponIds.Tec9
                || id == (short)WeaponIds.Hkp2000;
        }

        public static bool IsKnife(short id)
        {
            return id == (short)WeaponIds.Knife
                || id == (short)WeaponIds.Knife_t
                || id == (short)WeaponIds.Bayonet
                || id == (short)WeaponIds.Knife_flip
                || id == (short)WeaponIds.Knife_gut
                || id == (short)WeaponIds.Knife_karambit
                || id == (short)WeaponIds.Knife_m9_bayonet
                || id == (short)WeaponIds.Knife_tactical
                || id == (short)WeaponIds.Knife_falchion
                || id == (short)WeaponIds.Knife_survival_bowie
                || id == (short)WeaponIds.Knife_butterfly
                || id == (short)WeaponIds.Knife_push
                || id == (short)WeaponIds.Knife_kukri;
        }

        public static bool IsGrenade(short id) {
            return id == (short)GrenadeIds.Flashbang
                || id == (short)GrenadeIds.HEgrenade
                || id == (short)GrenadeIds.Smoke
                || id == (short)GrenadeIds.Molotov
                || id == (short)GrenadeIds.Decoy
                || id == (short)GrenadeIds.Incgrenade;
        }
    }

    public enum GrenadeIds
    {
        Flashbang = 43,
        HEgrenade = 44,
        Smoke = 45,
        Molotov = 46,
        Decoy = 47,
        Incgrenade = 48
    }

    public enum WeaponIds 
    {
        Deagle = 1,
        Elite = 2,
        Fiveseven = 3,
        Glock = 4,
        Ak47 = 7,
        Aug = 8,
        Awp = 9,
        Famas = 10,
        G3sg1 = 11,
        Galilar = 13,
        M249 = 14,
        M4a1 = 16,
        Mac10 = 17,
        P90 = 19,
        Ump = 24,
        Xm1014 = 25,
        Bizon = 26,
        Mag7 = 27,
        Negev = 28,
        Sawedoff = 29,
        Tec9 = 30,
        Taser = 31,
        Hkp2000 = 32,
        Mp7 = 33,
        Mp9 = 34,
        Nova = 35,
        P250 = 36,
        Scar20 = 38,
        Sg556 = 39,
        Ssg08 = 40,
        Knife = 42,
        Flashbang = 43,
        Hegrenade = 44,
        Smokegrenade = 45,
        Molotov = 46,
        Decoy = 47,
        Incgrenade = 48,
        C4 = 49,
        Knife_t = 59,
        M4a1_silencer = 60,
        Usp_silencer = 61,
        Cz75a = 63,
        Revolver = 64,
        Bayonet = 500,
        Knife_flip = 505,
        Knife_gut = 506,
        Knife_karambit = 507,
        Knife_m9_bayonet = 508,
        Knife_tactical = 509,
        Knife_falchion = 512,
        Knife_survival_bowie = 514,
        Knife_butterfly = 515,
        Knife_push = 516,
        Knife_kukri = 526
    }

    public enum BoneIds
    {
        Waist = 0, //0
        Neck = 5, //1
        Head = 6, //2
        ShoulderLeft = 8, //3
        ForeLeft = 9, //4
        HandLeft = 11, //5
        ShoulderRight = 13, //6
        ForeRight = 14, //7
        HandRight = 16, //8
        KneeLeft = 23, //9
        FeetLeft = 24, //10
        KneeRight = 26, //11
        FeetRight = 27, //12
    }
}
