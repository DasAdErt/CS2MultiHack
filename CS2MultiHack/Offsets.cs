using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2MultiHack
{
    public static class Offsets
    {
        //offsets //https://github.com/a2x/cs2-dumper/blob/dc0603ae29d8f7008810cf584c9a35975b5c6c7f/output/cs
        public static int dwLocalPlayerPawn = 0x1BEC440; //https://github.com/a2x/cs2-dumper/blob/dc0603ae29d8f7008810cf584c9a35975b5c6c7f/output/cs#L15
        public static int dwEntityList = 0x1D0FE08; //https://github.com/a2x/cs2-dumper/blob/dc0603ae29d8f7008810cf584c9a35975b5c6c7f/output/cs#L8
        public static int dwViewMatrix = 0x1E2D030;
        public static int dwViewAngles = 0x1E37BE0;

        //buttons
        public static int jump = 0x1BE5FE0; //https://github.com/a2x/cs2-dumper/blob/dc0603ae29d8f7008810cf584c9a35975b5c6c7f/output/buttons.cs#L12
        public static int attack = 0x1BE5AD0; //https://github.com/a2x/cs2-dumper/blob/dc0603ae29d8f7008810cf584c9a35975b5c6c7f/output/buttons.cs#L7

        //client dll
        public static int m_iIDEntIndex = 0x3EDC; //https://github.com/a2x/cs2-dumper/blob/dc0603ae29d8f7008810cf584c9a35975b5c6c7f/output/client_dll.cs#L2148
        public static int m_iTeamNum = 0x3EB; //https://github.com/a2x/cs2-dumper/blob/dc0603ae29d8f7008810cf584c9a35975b5c6c7f/output/client_dll.cs#L5844
        public static int m_iszPlayerName = 0x6E8;
        public static int m_iHealth = 0x34C;
        public static int m_iItemDefinitionIndex = 0x1BA;

        public static int m_Item = 0x50;

        public static int m_fFlags = 0x3F8; //https://github.com/a2x/cs2-dumper/blob/dc0603ae29d8f7008810cf584c9a35975b5c6c7f/output/client_dll.cs#L5847
        public static int m_flFlashBangTime = 0x160C; //https://github.com/a2x/cs2-dumper/blob/dc0603ae29d8f7008810cf584c9a35975b5c6c7f/output/client_dll.cs#L2130

        public static int m_hPlayerPawn = 0x8FC;

        public static int m_lifeState = 0x350;

        public static int m_vOldOrigin = 0x15B0;

        public static int m_vecViewOffset = 0xD98;

        public static int m_modelState = 0x190;

        public static int m_entitySpottedState = 0x2710;

        public static int m_bSpotted = 0x8;

        public static int m_pClippingWeapon = 0x3DF0;
        public static int m_pGameSceneNode = 0x330;

        public static int m_AttributeManager = 0x13A0;

        public static int m_aimPunchAngle = 0x16F4;
        public static int m_iShotsFired = 0x273C;

        public static int m_bIsScoped = 0x2728;

        public static int m_pObserverServices = 0x1418;
        public static int m_hObserverTarget = 0x44;
        public static int m_hObserverPawn = 0x900;
        public static int m_hPawn = 0x6B4;
    }
}
