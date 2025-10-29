using SharpDX;
using SharpDX.DirectInput;
using SharpDX.Mathematics.Interop;
using Swed64;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;

namespace CS2MultiHack
{
    internal class Program
    {
        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(int vKey);


        [DllImport("user32.dll")]
        static extern void mouse_event(uint dwFlags, uint dwData, uint dx, uint dy, uint dwExtraInfo);

        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);


        [DllImport("user32.dll")]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        static extern bool SetWindowDisplayAffinity(IntPtr hWnd, uint dwAffinity);

        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int X, int Y);


        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);
        private const uint INPUT_MOUSE = 0;
        private const uint MOUSEEVENTF_MOVE = 0x0001;

        //[DllImport("kernel32.dll")]
        //static extern IntPtr GetConsoleWindow();

        //[DllImport("user32.dll")]
        //static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        //https://learn.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes
        const int KeyE = 0x45;
        const int SPACE_BAR = 0x20;

        const int LBUTTON = 0x01;
        const int RBUTTON = 0x02;
        const int MiddleMOUSE = 0x04;
        const int MOUSE4 = 0x05; //	X1 mouse button
        const int MOUSE5 = 0x06; //	X2 mouse button

        private const int MOUSEEVENTF_LEFTDOWN = 0x0002;
         public const int KEYEVENTF_EXTENDEDKEY = 0x0001;
        public const int KEYEVENTF_KEYUP = 0x0002;
        public const uint VK_SPACE = 0x20; // Виртуальный код клавиши Space

        private float originalFov = 0.0f;
        private bool wasScoped = false;

        private Thread? triggerBotThread;
        private volatile bool triggerBotRunning = false;
        private ManualResetEvent stopTriggerBot = new ManualResetEvent(false);opTriggerBot = new ManualResetEvent(false);setEvent(false);

        private Vector2 oldPunch = Vector2.Zero;

        //const int SW_HIDE = 0;
        //const int SW_SHOW = 5;

        static void Main(string[] args)
        {
            Swed swed;
            Program program = new Program();

            // check if CS2 is running
            try
            {
                swed = new Swed("cs2");
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error: {e.Message}");
                Debug.WriteLine("CS2 process is not running!");
                return;
            }

            IntPtr client_dll = swed.GetModuleBase("client.dll");

            CancellationTokenSource cancelTokenSourceRCS = new CancellationTokenSource();
            CancellationToken tokenRCS = cancelTokenSourceRCS.Token;

            List<Entity> entities = new List<Entity>();
            Entity localPlayer = new Entity();


            IntPtr forceJumpAddress = client_dll + Offsets.jump;
            IntPtr forceAttackAddress = client_dll + Offsets.attack;

            bool isAiming = false;
                      const uint STANDING = 65665;
            const uint CROUCHING = 65667;

            Menu menu = new Menu();

            Thread menuThread = new Thread(() => menu.Start().Wait());
            menuThread.Start();rt(menu.Start().Wait));
            menuThread.Start();

            Vector2 screenSize = menu.screenSize;

            while (true)
            {
                entities.Clear();

                IntPtr localPlayerPawn = swed.ReadPointer(client_dll, Offsets.dwLocalPlayerPawn);
                IntPtr entityList = swed.ReadPointer(client_dll, Offsets.dwEntityList);
                IntPtr listEntryNew = swed.ReadPointer(entityList, 0x10);

                localPlayer.team = swed.ReadInt(localPlayerPawn, Offsets.m_iTeamNum);
                localPlayer.pawnAddress = localPlayerPawn;
                localPlayer.position = swed.ReadVec(localPlayerPawn, Offsets.m_vOldOrigin);
                localPlayer.viewOff                    IntPtr currentController = swed.ReadPointer(listEntryNew, i * 0x78);
                    if (currentController == IntPtr.Zero) continue;

                    int pawnHandle = swed.ReadInt(currentController, Offsets.m_hPlayerPawn);
                    if (pawnHandle == 0) continue;

                    IntPtr listEntry2 = swed.ReadPointer(entityList, 0x8 * ((pawnHandle & 0x7FFF) >> 9) + 0x10);
                    if (listEntry2 == IntPtr.Zero) continue;

                    IntPtr currentPawn = swed.ReadPointer(listEntry2, 0x78 * (pawnHandle & 0x1FF));
                    if (currentPawn == IntPtr.Zero || currentPawn == localPlayer.pawnAddress) continue;

                    IntPtr currentWeapon = swed.ReadPointer(currentPawn, Offsets.m_pClippingWeapon);
                    if (currentWeapon == IntPtr.Zero) continue;

                    short weaponDefinitionIndex = swed.ReadShort(currentWeapon, Offsets.m_AttributeManager + Offsets.m_Item + Offsets.m_iItemDefinitionIndex);
                    if (weaponDefinitionIndex == -1) continue;

                    uint lifeSt                    int teamNum = swed.ReadInt(currentPawn, Offsets.m_iTeamNum);
                    if (teamNum == localPlayer.team && !menu.aimOnTeam) continue;

                    float[] viewMatrix = swed.ReadMatrix(client_dll + Offsets.dwViewMatrix);

                    IntPtr sceneNode = swed.ReadPointer(currentPawn, Offsets.m_pGameSceneNode);
                    if (sceneNode == IntPtr.Zero) continue;
                    
                    IntPtr boneMatrix = swed.ReadPointer(sceneNode, Offsets.m_modelState + 0x80);
                    if (boneMatrix == IntPtr.Zero) continue;

                    IntPtr currentWeaponLocal = swed.ReadPointer(localPlayerPawn, Offsets.m_pClippingWeapon);
                    if (currentWeaponLocal == IntPtr.Zero) continue;

                    short weaponDefinitionIndexLocal = swed.ReadShort(currentWeaponLocal, Offsets.m_AttributeManager + Offsets.m_Item + Offsets.m_iItemDefinitionIndex);
                    if (weaponDefinitionIndexLocal == -1) continue;sets.m_Item + Offsets.m_iItemDefinitionIndex);
                    if (weaponDefinitionIndex == -1) continue;

                    localPlayer.currentWeaponIndex = weaponDefinitionIndexLocal;
                    localPlayer.currentWeaponName = Enum.GetName(typeof(WeaponIds), weaponDefinitionIndexLocal);
                    localPlayer.scopped = swed.ReadBool(localPlayerPawn, Offsets.m_bIsScoped);

                    Entity entity = new Entity();
                    entity.team = teamNum;
                    entity.health = swed.ReadInt(currentPawn, Offsets.m_iHealth);
                    localPlayer.health = swed.ReadInt(localPlayerPawn, Offsets.m_iHealth);
                    entity.name = swed.ReadString(currentController, Offsets.m_iszPlayerName, 16).Split("\0")[0];
                    entity.currentWeaponIndex = weaponDefinitionIndex;

                    entity.currentWeaponName = Enum.GetName(typeof(WeaponIds), weaponDefinitionIndex);

                    entity.spotted = swed.ReadBool(currentPawn, Offsets.m_entitySpottedState + Offsets.m_bSpotted);

                    entity.position = swed.ReadVec(currentPawn, Offsets.m_vOldOrigin);

                    entity.bone = new Vector3[7];
                    entity.bone2D = new Vector2[7];
                    entity.pixelDistance = new float[7];

                    for (int index = 0; index <= 6; index++)
                    {
                        entity.bone[index] = swed.ReadVec(boneMatrix, index * 32); //6 = bone id, 32 = step between bone coordinates /*menu.boneId*/
                        entity.bone2D[index] = Calculate.WorldToScreen(viewMatrix, entity.bone[index], screenSize);
                        entity.pixelDistance[index] = Vector2.Distance(entity.bone2D[index], new Vector2(screenSize.X / 2, screenSize.Y / 2));
                    }

                    entity.viewOffset = swed.ReadVec(currentPawn, Offsets.m_vecViewOffset);
                    entity.position2D = Calculate.WorldToScreen(viewMatrix, entity.position, menu.screenSize);
                    entity.viewPosition2D = Calculate.WorldToScreen(viewMatrix, Vector3.Add(entity.position, entity.viewOffset), menu.screenSize);
                    entity.distance = Vector3.Distance(entity.position, localPlayer.position);
                    entity.bones = Calculate.ReadBones(boneMatrix, swed);
                    entity.bones2D = Calculate.ReadBones2D(entity.bones, viewMatrix, menu.screenSize);

                    entity.controllerAddress = currentController;
                    entity.pawnAddress = currentPawn;
                    entity.lifeState = lifeState;

                    entities.Add(entity);
                }
                menu.UpdateSpectatorList(swed, client_dll, entityList, localPlayer.pawnAddress);
                menu.UpdateLocalPlayer(localPlayer);
                menu.UpdateEntities(entities);

                int team = swed.ReadInt(localPlayerPawn, Offsets.m_iTeamNum);
                uint fFlag = swed.ReadUInt(localPlayerPawn, Offsets.m_fFlags);
                int entIndex = swed.ReadInt(localPlayerPawn, Offsets.m_iIDEntIndex);
                float flashDuration = swed.ReadFloat(localPlayerPawn, Offsets.m_flFlashBangTime);

                if (menu.enableAimbot)
                {
                    if (menu.aimbot)
                    {
                        //entities = entities.Order                        entities = entities
                                    .Where(o => o.pixelDistance != null && !o.pixelDistance.Any(float.IsNaN))
                                    .OrderBy(o => o.pixelDistance.Min())
                                    .ToList();

                        if (entities.Count > 0)
                        {
                            if (GetAsyncKeyState(LBUTTON) < 0 || GetAsyncKeyState(MOUSE5) < 0 || menu.autoAim || menu.autoAimWithSniper && Weapon.IsSniper(localPlayer.currentWeaponIndex) && localPlayer.scopped)
                            {
                                Vector3 originalViewAngles = swed.ReadVec(client_dll, Offsets.dwViewAngles);
                                Vector3 playerView = Vector3.Add(localPlayer.position, localPlayer.viewOffset);

                                if (localPlayer.scopped)
                                {
                                    if (!program.wasScoped)
                                    {
                                        program.originalFov = menu.fov;
                                        menu.fov = program.originalFov * 2;
                                        program.wasScoped = true;
                                    }
                                }
                                else
                                {
                                    if (program.wasScoped)
                                    {
                                        menu.fov = program.originalFov;
                                        program.wasScoped = false;
                                    }
                                }                       menu.fov = originalFov;
                                        wasScoped = false;
                                    }
                                }

                                if (menu.aimOnlyVisible && !entities[0].spotted) 
                                {
                                    Thread.Sleep(3);
                                    continue;
                                }

                                /*if (entities[0].pixelDistance[0] < menu.fov || entities[0].pixelDistance[1] < menu.fov || entities[0].pixelDistance[2] < menu.fov || entities[0].pixelDistance[3] < menu.fov || entities[0].pixelDistance[4] < menu.fov || entities[0].pixelDistance[5] < menu.fov || entities[0].pixelDistance[6] < menu.fov)
                                {
                                    // Calculate RCS (recoil control)
                                    IntPtr localPlyer = swed.ReadPointer(client_dll, Offsets.dwLocalPlayerPawn);
                                    Vector3 punchAngle = Vector3.Zero;
                                    Vector3 aimPunch = swed.ReadVec(localPlyer, Offsets.m_aimPunchAngle);

                                    // RCS standalone
                                    if (menu.rcs && aimPunch.X != 0 && aimPunch.Y != 0)
                                    {
                                        punchAngle.X = aimPunch.X * 2 * 0.01f * menu.rcsIntensity;
                                        punchAngle.Y = aimPunch.Y * 2 * 0.01f * menu.rcsIntensity;
                                    }

                                    Vector2 aimAngles = new Vector2();
                                    Vector3 targetAngles = new Vector3();
                                    Vector3 angle = new Vector3();

                                    if (menu.boneAimCombo == 4)
                                    {
                                        menu.UpdateBoneIdBasedOnWeapon();
                                    }

                                    if (menu.boneAimCombo != 3)
                                    {
                                        aimAngles = Calculate.CalculateAngles(playerView, entities[0].bone[menu.boneId]);
                                        targetAngles = new Vector3(aimAngles.Y, aimAngles.X, 0);
                                        angle = targetAngles - originalViewAngles - punchAngle;
                                    }
                                    else
                                    {
                                        int closestBoneIndex = 0;
                                        float minDistance = float.MaxValue;

                                        for (int i = 0; i < entities[0].pixelDistance.Length; i++)
                                        {
                                            if (entities[0].pixelDistance[i] < minDistance)
                                            {
                                                minDistance = entities[0].pixelDistance[i];
                                                closestBoneIndex = i;
                                            }
                                        }

                                        aimAngles = Calculate.CalculateAngles(playerView, entities[0].bone[closestBoneIndex]);
                                        targetAngles = new Vector3(aimAngles.Y, aimAngles.X, 0);
                                        angle = targetAngles - originalViewAngles - punchAngle;
                                    }

                                    if (angle.Y > 180) angle.Y -= 360;
                                    if (angle.Y < -180) angle.Y += 360;

                                    float smoothValue = menu.smoothFactor;
                                    if (smoothValue > 1.0f)
                                        angle /= smoothValue;

                                    if (!menu.showWindow && !Weapon.IsGrenade(localPlayer.currentWeaponIndex) && localPlayer.currentWeaponIndex != (short)WeaponIds.C4 && !Weapon.IsKnife(localPlayer.currentWeaponIndex) && localPlayer.health > 0)
                                        program.AimWithMouseMovement(angle, menu.sensitivity);
                                }*/

                                // --- Чтение данных игрока ---
                                IntPtr localPlyer = swed.ReadPointer(client_dll, Offsets.dwLocalPlayerPawn);
                                int shotsFired = swed.ReadInt(localPlyer, Offsets.m_iShotsFired);
                                Vector3 targetAngles = Vector3.Zero; // Изначально цели нет

                                // --- Переменные для углов ---
                                Vector3 aimAngle = Vector3.Zero;
                                Vector2 rcsAngle = Vector2.Zero;

                                if (entities[0].pixelDistance[0] < menu.fov || entities[0].pixelDistance[1] < menu.fov || entities[0].pixelDistance[2] < menu.fov || entities[0].pixelDistance[3] < menu.fov || entities[0].pixelDistance[4] < menu.fov || entities[0].pixelDistance[5] < menu.fov || entities[0].pixelDistance[6] < menu.fov)
                                {
                                    // Выбор ближайшей кости
                                    int closestBoneIndex = 0;
                                    float minDistance = float.MaxValue;
                                    for (int i = 0; i < entities[0].pixelDistance.Length; i++)
                                    {
                                        if (entities[0].pixelDistance[i] < minDistance)
                                        {
                                            minDistance = entities[0].pixelDistance[i];
                                            closestBoneIndex = i;
                                        }
                                    }

                                    // Рассчитываем "чистый" угол до цели, без учета отдачи
                                    Vector2 calculatedAngles = Calculate.CalculateAngles(playerView, entities[0].bone[closestBoneIndex]);
                                    targetAngles = new Vector3(calculatedAngles.Y, calculatedAngles.X, 0);
                                }
                                else
                                {
                                    // Если цели нет, аимбот не должен ничего делать, но RCS может работать
                                    targetAngles = originalViewAngles; // Целимся туда, куда и так смотрим
                                }

                                // --- 3. ПРИМЕНЕНИЕ RCS (если включен) ---
                                if (menu.rcs && shotsFired > 0)
                                {
                                    // Читаем отдачу
                                    Vector3 aimPunch = swed.ReadVec(localPlyer, Offsets.m_aimPunchAngle);

                                    float rcsMultiplier = menu.rcsIntensity / 50.0f;

                                    // ВЫЧИТАЕМ отдачу ПРЯМО ИЗ ЦЕЛЕВОГО УГЛА
                                    // Множитель 2.0f - стандартный для CS2
                                    targetAngles.X -= aimPunch.X * rcsMultiplier;
                                    targetAngles.Y -= aimPunch.Y * rcsMultiplier;
                                }

                                // --- 4. Расчет финального движения и наведение ---
                                // Вычисляем разницу между тем, куда мы хотим смотреть, и куда смотрим сейчас
                                Vector3 finalAngle = targetAngles - originalViewAngles;

                                // Проверяем, есть ли смысл двигать мышь
                                if (finalAngle.LengthSquared() > 0.0001f) // LengthSquared() быстрее, чем Length()
                                {
                                    // Нормализация и плавность
                                    if (finalAngle.Y > 180) finalAngle.Y -= 360;
                                    if (finalAngle.Y < -180) finalAngle.Y += 360;

                                    if (menu.smoothFactor > 1.0f)
                                        finalAngle /= menu.smoothFactor;

                                    // Движение мыши
                                    if (!menu.showWindow && !Weapon.IsGrenade(localPlayer.currentWeaponIndex) && localPlayer.currentWeaponIndex != (short)WeaponIds.C4 && !Weapon.IsKnife(localPlayer.currentWeaponIndex) && localPlayer.health > 0)
                                        program.AimWithMouseMovement(finalAngle, menu.sensitivity);
                                }
                            }
                        }
                    }

                    if (menu.triggerbot)
                        program.StartTriggerBot(swed, entIndex, team, entityList, menu);
                    else
                        program.StopTriggerBot();
                }

                if (menu.enableVisuals)
                {
                    if (menu.antiFlash)
                    {
                        if (flashDuration > 0)
                            swed.WriteFloat(localPlayerPawn, Offsets.m_flFlashBangTime, 0);
                    }
                }

                if (menu.enableMisc)
                {
                    if (menu.bunnyHop)
                    {
                        if (GetAsyncKeyState(MOUSE4) < 0)
                        {
                            if (fFlag == STANDING || fFlag == CR                    IntPtr OverlayHwnd = FindWindow(null, "overlay");

                    if (menu.obsBypass)
                        SetWindowDisplayAffinity(OverlayHwnd, (uint)WDA.WDA_EXCLUDEFROMCAPTURE); //Bypass on
                    else
                        SetWindowDisplayAffinity(OverlayHwnd, (uint)WDA.WDA_NONE); //Bypass off

                    Thread.Sleep(3);
                }
            }
        }

        public void StartTriggerBot(Swed swed, int entIndex, int team, IntPtr entityList, Menu menu)
        {
            if (triggerBotRunning) return;                   triggerBotThread = new Thread(() => {
                while (triggerBotRunning && !stopTriggerBot.WaitOne(1))
                {
                    if (menu.triggerbot)
                    {
                        TriggerBot(swed, team, entityList, menu);
                    }
                }
            });et();

            triggerBotThread = new Thread(() => {
                while (triggerBotRunning && !stopTriggerBot.WaitOne(1))
                {
                    if (menu.triggerbot)
                    {
                        program.TriggerBot(swed, team, entityList, menu);
                    }
                    //Thread.Sleep(1);
                }
            });

            triggerBotThread.IsBackground = true;
            triggerBotThread.Start();
        }

        public void StopTriggerBot()
        {
            triggerBotRunning = false;
            stopTriggerBot.Set();
            if (triggerBotThread != null && triggerBotThread.IsAlive)
            {
                triggerBotThread.Join(100);
            }
        }

        private void TriggerBot(Swed swed, int team, IntPtr entityList, Menu menu)
        {
            IntPtr client_dll = swed.GetModuleBase("client.dll");
            IntPtr localPlayerPawn = swed.ReadPointer(client_dll, Offsets.dwLocalPlayerPawn);
            int entIndex = swed.ReadInt(localPlayerPawn, Offsets.m_iIDEntIndex);
            if (entIndex != -1)
            {
                IntPtr listEntry = swed.ReadPointer(entityList, 0x8 * ((entIndex & 0x7FFF) >> 9) + 0x10);
                IntPtr currentPawn = swed.ReadPointer(listEntry, 0x78 * (entIndex & 0x1FF));

                int entityTeam = swed.ReadInt(currentPawn, Offsets.m_iTeamNum);

                if (!menu.shootInTeamTriggerBot)
                {
                    if (team != entityTeam)
                    {
                        if (GetAsyncKeyState(MOUSE4) < 0 || GetAsyncKeyState(MOUSE5) < 0)
                        {
                            if (menu.triggerDelay > 0)
                            {
                                Thread.Sleep(menu.triggerDelay);
                                mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                                Thread.Sleep(35);
                                mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                            }
                        }
                    }
                }
                else
                {
                    if (GetAsyncKeyState(MOUSE4) < 0)
                    {
                        if (menu.triggerDelay > 0)
                        {
                            Thread.Sleep(menu.triggerDelay);
                            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                            Thread.Sleep(35);
                            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                        }
                    }
                }
            }
        }

        private void AimWithMouseMovement(Vector3 angleChange, float sensitivity)
        {
            // Увеличенный коэффициент для более заметных движений
            float factor = 1.0f / sensitivity * 50.0f;

            // Рассчитываем перемещение мыши
            int dx = (int)Math.Round(-angleChange.Y * factor);
            int dy = (int)Math.Round(angleChange.X * factor);

            // Проверяем, есть ли движение вообще
            if (dx == 0 && dy == 0) return;

            // Подготавливаем структуру INPUT
            INPUT[] input = new INPUT[1];
            input[0].type = INPUT_MOUSE;
            input[0].U.mi.dx = dx;
            input[0].U.mi.dy = dy;
            input[0].U.mi.dwFlags = MOUSEEVENTF_MOVE;
            input[0].U.mi.mouseData = 0;
            input[0].U.mi.time = 0;
            input[0].U.mi.dwExtraInfo = IntPtr.Zero;

            // Отправляем событие движения мыши
            SendInput(1, input, Marshal.SizeOf(typeof(INPUT)));
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct InputUnion
        {
            [FieldOffset(0)]
            public MOUSEINPUT mi;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct INPUT
        {
            public uint type;
            public InputUnion U;
        }

        public enum WDA
        {
            WDA_NONE = 0x00000000,
            WDA_MONITOR = 0x00000001,
            WDA_EXCLUDEFROMCAPTURE = 0x00000011,
        }
    }
}
