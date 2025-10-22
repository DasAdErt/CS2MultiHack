using ClickableTransparentOverlay;
using ImGuiNET;
using Swed64;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace CS2MultiHack
{

    public class Menu : Overlay
    {
        public bool enableAimbot = false;
        public bool aimbot = false;
        public bool aimOnTeam = false;
        public bool aimOnlyVisible = false;

        public bool autoAim = false;
        public bool autoAimWithSniper = false;

        public bool drawFov = false;
        public bool rcs = false;
        public float fov = 250f;
        public float smoothFactor = 15f;
        public float rcsIntensity = 100.0f;

        public bool triggerbot = false;
        public bool shootInTeamTriggerBot = false;
        public int triggerDelay = 50;

        public bool enableVisuals = false;
        public bool antiFlash = false;
        public bool name = false;
        public bool glow = false;
        public bool weaponName = false;
        public bool crosshairInSniper = false;
        public bool skeletonESP = false;
        public bool box = false;
        public bool snapLines = false;
        public bool teammates = false;
        public bool onlyVisible = false;
        public bool healthBar = false;

        public Vector4 enemyBoxColor = new Vector4(0, 1, 0, 1);
        public Vector4 enemyBoxColorInvisible = new Vector4(1, 0, 0, 1);

        public Vector4 enemySnapLinesColor = new Vector4(0, 1, 0, 1);
        public Vector4 enemySnapLinesColorInvisible = new Vector4(1, 0, 0, 1);

        public Vector4 enemyBonesColor = new Vector4(0, 1, 0, 1);
        public Vector4 enemyBonesColorInvisible = new Vector4(1, 0, 0, 1);


        public Vector4 teamColor = new Vector4(0, 1, 0, 1);
        public Vector4 teamColorInvisible = new Vector4(0, 0, 1, 1);

        public Vector4 nameColor = new Vector4(1, 1, 1, 1);
        public Vector4 weaponColor = new Vector4(1, 1, 1, 1);

        public Vector4 fovAimbotColor = new Vector4(1, 1, 1, 1);

        public Vector4 hiddenColor = new Vector4(0, 0, 0, 0);

        public int boneId = 0;
        public int boneAimCombo = 2;

        public bool enableMisc = true;
        public bool bunnyHop = false;
        public bool watermark = true;
        public bool spectatorList = false;
        public bool obsBypass = true;

        public float sensitivity = 1.75f; // Чувствительность мыши из игры

        public bool showWindow = true;
        public bool focusedWindow;

        public Vector2 screenSize = new Vector2(1920, 1080);

        private ConcurrentQueue<Entity> entities = new ConcurrentQueue<Entity>();
        private Entity localPlayer = new Entity();
        private readonly object entityLock = new object();

        ImDrawListPtr drawList;
        public List<string> spectators = new List<string>();

        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(int vKey);

        const int INSERT = 0x2D;
        const int HOME = 0x24;

        public Menu()
        {
            VSync = false;
            LoadConfig();
        }

        public void LoadConfig()
        {
            var config = ConfigManager.Instance;

            // Load Aimbot settings
            enableAimbot = config.GetBool("enableAimbot", enableAimbot);
            aimbot = config.GetBool("aimbot", aimbot);
            aimOnTeam = config.GetBool("aimOnTeam", aimOnTeam);
            aimOnlyVisible = config.GetBool("aimOnlyVisible", aimOnlyVisible);
            autoAim = config.GetBool("autoAim", autoAim);
            autoAimWithSniper = config.GetBool("autoAimWithSniper", autoAimWithSniper);
            drawFov = config.GetBool("drawFov", drawFov);
            rcs = config.GetBool("rcs", rcs);
            fov = config.GetFloat("fov", fov);
            smoothFactor = config.GetFloat("smoothFactor", smoothFactor);
            rcsIntensity = config.GetFloat("rcsIntensity", rcsIntensity);

            // Load Triggerbot settings
            triggerbot = config.GetBool("triggerbot", triggerbot);
            shootInTeamTriggerBot = config.GetBool("shootInTeamTriggerBot", shootInTeamTriggerBot);
            triggerDelay = config.GetInt("triggerDelay", triggerDelay);

            // Load Visual settings
            enableVisuals = config.GetBool("enableVisuals", enableVisuals);
            antiFlash = config.GetBool("antiFlash", antiFlash);
            name = config.GetBool("name", name);
            glow = config.GetBool("glow", glow);
            weaponName = config.GetBool("weaponName", weaponName);
            crosshairInSniper = config.GetBool("crosshairInSniper", crosshairInSniper);
            skeletonESP = config.GetBool("skeletonESP", skeletonESP);
            box = config.GetBool("box", box);
            snapLines = config.GetBool("snapLines", snapLines);
            teammates = config.GetBool("teammates", teammates);
            onlyVisible = config.GetBool("onlyVisible", onlyVisible);
            healthBar = config.GetBool("healthBar", healthBar);

            // Load Color settings
            enemyBoxColor = config.GetVector4("enemyBoxColor", enemyBoxColor);
            enemyBoxColorInvisible = config.GetVector4("enemyBoxColorInvisible", enemyBoxColorInvisible);
            enemySnapLinesColor = config.GetVector4("enemySnapLinesColor", enemySnapLinesColor);
            enemySnapLinesColorInvisible = config.GetVector4("enemySnapLinesColorInvisible", enemySnapLinesColorInvisible);
            enemyBonesColor = config.GetVector4("enemyBonesColor", enemyBonesColor);
            enemyBonesColorInvisible = config.GetVector4("enemyBonesColorInvisible", enemyBonesColorInvisible);
            teamColor = config.GetVector4("teamColor", teamColor);
            teamColorInvisible = config.GetVector4("teamColorInvisible", teamColorInvisible);
            nameColor = config.GetVector4("nameColor", nameColor);
            weaponColor = config.GetVector4("weaponColor", weaponColor);
            fovAimbotColor = config.GetVector4("fovAimbotColor", fovAimbotColor);
            hiddenColor = config.GetVector4("hiddenColor", hiddenColor);

            // Load Bone settings
            boneId = config.GetInt("boneId", boneId);
            boneAimCombo = config.GetInt("boneAimCombo", boneAimCombo);

            // Load Misc settings
            enableMisc = config.GetBool("enableMisc", enableMisc);
            bunnyHop = config.GetBool("bunnyHop", bunnyHop);
            watermark = config.GetBool("watermark", watermark);
            spectatorList = config.GetBool("spectatorList", spectatorList);
            obsBypass = config.GetBool("obsBypass", obsBypass);
        }

        public void SaveConfig()
        {
            var config = ConfigManager.Instance;

            // Save Aimbot settings
            config.SetBool("enableAimbot", enableAimbot);
            config.SetBool("aimbot", aimbot);
            config.SetBool("aimOnTeam", aimOnTeam);
            config.SetBool("aimOnlyVisible", aimOnlyVisible);
            config.SetBool("autoAim", autoAim);
            config.SetBool("autoAimWithSniper", autoAimWithSniper);
            config.SetBool("drawFov", drawFov);
            config.SetBool("rcs", rcs);
            config.SetFloat("fov", fov);
            config.SetFloat("smoothFactor", smoothFactor);
            config.SetFloat("rcsIntensity", rcsIntensity);

            // Save Triggerbot settings
            config.SetBool("triggerbot", triggerbot);
            config.SetBool("shootInTeamTriggerBot", shootInTeamTriggerBot);
            config.SetInt("triggerDelay", triggerDelay);

            // Save Visual settings
            config.SetBool("enableVisuals", enableVisuals);
            config.SetBool("antiFlash", antiFlash);
            config.SetBool("name", name);
            config.SetBool("glow", glow);
            config.SetBool("weaponName", weaponName);
            config.SetBool("crosshairInSniper", crosshairInSniper);
            config.SetBool("skeletonESP", skeletonESP);
            config.SetBool("box", box);
            config.SetBool("snapLines", snapLines);
            config.SetBool("teammates", teammates);
            config.SetBool("onlyVisible", onlyVisible);
            config.SetBool("healthBar", healthBar);

            // Save Color settings
            config.SetVector4("enemyBoxColor", enemyBoxColor);
            config.SetVector4("enemyBoxColorInvisible", enemyBoxColorInvisible);
            config.SetVector4("enemySnapLinesColor", enemySnapLinesColor);
            config.SetVector4("enemySnapLinesColorInvisible", enemySnapLinesColorInvisible);
            config.SetVector4("enemyBonesColor", enemyBonesColor);
            config.SetVector4("enemyBonesColorInvisible", enemyBonesColorInvisible);
            config.SetVector4("teamColor", teamColor);
            config.SetVector4("teamColorInvisible", teamColorInvisible);
            config.SetVector4("nameColor", nameColor);
            config.SetVector4("weaponColor", weaponColor);
            config.SetVector4("fovAimbotColor", fovAimbotColor);
            config.SetVector4("hiddenColor", hiddenColor);

            // Save Bone settings
            config.SetInt("boneId", boneId);
            config.SetInt("boneAimCombo", boneAimCombo);

            // Save Misc settings
            config.SetBool("enableMisc", enableMisc);
            config.SetBool("bunnyHop", bunnyHop);
            config.SetBool("watermark", watermark);
            config.SetBool("spectatorList", spectatorList);
            config.SetBool("obsBypass", obsBypass);

            // Write to file
            config.Save();
        }

        protected override void Render()
        {
            DrawMenu();
        }

        protected void DrawMenu()
        {

            if (GetAsyncKeyState(INSERT) < 0 || GetAsyncKeyState(HOME) < 0)
            {
                showWindow = !showWindow;
                Thread.Sleep(200);
            }

            if (showWindow)
            {
                focusedWindow = ImGui.IsWindowFocused();
                ImGui.Begin("ImGui .NET ", ImGuiWindowFlags.AlwaysAutoResize);

                if (ImGui.BeginTabBar("Tabs"))
                {
                    if (ImGui.BeginTabItem("Aimbot"))
                    {
                        ImGui.Checkbox("Enable", ref enableAimbot);

                        if (enableAimbot)
                        {
                            ImGui.BeginChild("Group 1", new Vector2(500, 300));
                            ImGui.Checkbox("Aimbot", ref aimbot);
                            if (aimbot)
                            {
                                //if (ImGui::Combo("AimKey", &MenuConfig::AimBotHotKey, "LBUTTON\0MENU\0RBUTTON\0XBUTTON1\0XBUTTON2\0CAPITAL\0SHIFT\0CONTROL"))// added LBUTTON
                                //{
                                //    AimControl::SetHotKey(MenuConfig::AimBotHotKey);
                                //} //https://github.com/TKazer/CS2_External/blob/b7e3b41687ce6ba798975741d9a9755249d1b65c/CS2_External/Cheats.cpp
                                
                                if(ImGui.Combo("Bone for Aim", ref boneAimCombo, "Head\0Neck\0Waist\0Nearest\0Test by admin\0"))
                                {
                                    switch (boneAimCombo)
                                    {
                                        case 0:
                                            boneId = 6;
                                            break;
                                        case 1:
                                            boneId = 5;
                                            break;
                                        case 2:
                                            boneId = 0;
                                            break;
                                        case 3:
                                            //0,1,2,3,4 - тело
                                            //5 - шея
                                            //6 - голова
                                            //if (Weapon.IsAWP(localPlayer.currentWeaponIndex) || Weapon.IsRifle(localPlayer.currentWeaponIndex) || Weapon.IsSMG(localPlayer.currentWeaponIndex)) boneId = 3; //hitboxes - 0,1,2,3,4 (самые приоритетные части)
                                            //if (Weapon.IsSSG08(localPlayer.currentWeaponIndex) || Weapon.IsPistol(localPlayer.currentWeaponIndex)) boneId = 5; //hitboxes - прицел над противником, то в 6, 5, если ниже головы, то 4,3,2,1,0
                                            //if (Weapon.IsAutoSniper(localPlayer.currentWeaponIndex)) boneId = 4; //hitboxes - 0,1,2,3,4 (самые приоритетные части)
                                            //if (Weapon.IsRifle(localPlayer.currentWeaponIndex)) boneId = 3; //hitboxes - прицел над противником, то в 6, 5, если shots > 1, то 4,3,2,1,0 для ркс
                                            //if (Weapon.IsSMG(localPlayer.currentWeaponIndex)) boneId = 3; //hitboxes - прицел над противником, то в 6, 5, если shots > 1, то 4,3,2,1,0 для ркс
                                            //if (Weapon.IsPistol(localPlayer.currentWeaponIndex)) boneId = 5; //hitboxes - прицел над противником, то в 6, 5, лучше стрелять в 5
                                            //boneId = 3;

                                            //UpdateBoneIdBasedOnWeapon();
                                            break;
                                        case 4:
                                            UpdateBoneIdBasedOnWeapon();
                                            break;
                                    }
                                }

                                ImGui.SliderFloat("Smooth", ref smoothFactor, 1, 50/*00*/);
                                ImGui.SliderFloat("RCS Intensity", ref rcsIntensity, 0, 100, "%.0f%%");
                                ImGui.Checkbox("Draw FOV", ref drawFov);
                                ImGui.SliderFloat("FOV", ref fov, 1, 1080);
                                ImGui.Checkbox("Aim on team", ref aimOnTeam);
                                ImGui.Checkbox("Aim only visible", ref aimOnlyVisible);
                                ImGui.Checkbox("Auto Aim", ref autoAim);
                                ImGui.Checkbox("Auto aim with Sniper", ref autoAimWithSniper);
                                ImGui.Checkbox("RCS", ref rcs);
                            }

                            ImGui.Separator();

                            ImGui.Text("MOUSE5 Auto Aim and Auto Fire (Trigger bot)");
                            ImGui.Text("LBUTTON for Aim");
                            ImGui.Text("MOUSE4 for Trigger");
                            ImGui.EndChild();

                            ImGui.SameLine();

                            ImGui.BeginChild("Group 2", new Vector2(450, 100));
                            ImGui.Checkbox("Trigger Bot", ref triggerbot);
                            if (triggerbot)
                            {
                                ImGui.SliderInt("Delay trigger", ref triggerDelay, 1, 500);
                                ImGui.Checkbox("Shoot in Team", ref shootInTeamTriggerBot);
                            }
                            ImGui.EndChild();
                        }

                        ImGui.EndTabItem();
                    }

                    if (ImGui.BeginTabItem("Visuals"))
                    {
                        ImGui.Checkbox("Enable", ref enableVisuals);

                        if (enableVisuals)
                        {
                            ImGui.BeginChild("Group 1", new Vector2(300, 400));

                            ImGui.Spacing();
                            ImGui.Spacing();
                            ImGui.Spacing();

                            ImGui.Checkbox("Teammates", ref teammates);
                            if (teammates)
                            {
                                ImGui.ColorEdit4("##teamColor", ref teamColor);
                                ImGui.ColorEdit4("##teamColor", ref teamColorInvisible);
                            }

                            ImGui.Spacing();
                            ImGui.Spacing();
                            ImGui.Spacing();

                            ImGui.Checkbox("Only Visible", ref onlyVisible);
                            ImGui.Checkbox("Name", ref name);
                            ImGui.Checkbox("Weapon name", ref weaponName);
                            ImGui.Checkbox("Health Bar", ref healthBar);
                            //if (healthBar)
                            //    ImGui.ColorEdit4("##healthBarColor", ref healthBarColor);

                            ImGui.Checkbox("Box", ref box);
                            if (box)
                            {
                                ImGui.ColorEdit4("##enemyBoxColor", ref enemyBoxColor);
                                ImGui.ColorEdit4("##enemyBoxColorInvisible", ref enemyBoxColorInvisible);
                            }

                            ImGui.Checkbox("Snap Lines", ref snapLines);
                            if (snapLines)
                            {
                                ImGui.ColorEdit4("##enemySnapLinesColor", ref enemySnapLinesColor);
                                ImGui.ColorEdit4("##enemySnapLinesColorInvisible", ref enemySnapLinesColorInvisible);
                            }

                            ImGui.Checkbox("Skeleton ESP", ref skeletonESP);
                            if (skeletonESP)
                            {
                                //ImGui.SliderFloat("Bone Thickness", ref boneThickness, 1, 10);
                                ImGui.ColorEdit4("##enemyBonesColor", ref enemyBonesColor);
                                ImGui.ColorEdit4("##enemyBonesColorInvisible", ref enemyBonesColorInvisible);
                            }

                            ImGui.EndChild();

                            ImGui.SameLine();

                            ImGui.BeginChild("Group 2", new Vector2(300, 200));

                            ImGui.Checkbox("Anti Flash", ref antiFlash);
                            ImGui.Checkbox("Enable crosshair with sniper", ref crosshairInSniper);

                            ImGui.EndChild();
                        }

                        ImGui.EndTabItem();
                    }

                    if (ImGui.BeginTabItem("Misc"))
                    {
                        ImGui.Checkbox("Enable", ref enableMisc);

                        if (enableMisc)
                        {
                            ImGui.Checkbox("Bunny Hop", ref bunnyHop);
                            ImGui.Checkbox("Watermark", ref watermark);
                            ImGui.Checkbox("Spectator List", ref spectatorList);
                            ImGui.Checkbox("OBS Bypass", ref obsBypass);

                            ImGui.Separator();

                            ImGui.Text("MOUSE4 Bunnyhop");
                        }

                        ImGui.EndTabItem();
                    }

                    if (ImGui.BeginTabItem("Config"))
                    {

                        if (ImGui.Button("Save"))
                            SaveConfig();

                        if (ImGui.Button("Load"))
                            LoadConfig();

                        ImGui.EndTabItem();
                    }

                    ImGui.EndTabBar();
                }

                ImGui.End();
            }

            DrawOverlay(screenSize);
            drawList = ImGui.GetWindowDrawList();

            if (enableMisc && spectatorList)
                DrawSpectatorList();

            if (crosshairInSniper && enableVisuals)
                DrawCrosshairInSniper(localPlayer);

            if (watermark && enableMisc)
                DrawWatermark();

            if (drawFov && enableAimbot && aimbot)
                drawList.AddCircle(new Vector2(screenSize.X / 2, screenSize.Y / 2), fov, ImGui.ColorConvertFloat4ToU32(fovAimbotColor));

            foreach (var entity in entities)
            {
                if (EntityOnScreen(entity))
                {
                    if (healthBar == true && enableVisuals == true && entity.health > 0)
                        DrawHealthBar(entity);
                    if (skeletonESP == true && enableVisuals == true && entity.health > 0)
                        DrawBones(entity);
                    if (box == true && enableVisuals == true && entity.health > 0)
                        DrawBox(entity);
                    if (snapLines == true && enableVisuals == true && entity.health > 0)
                        DrawLines(entity);
                    if (name == true && enableVisuals == true && entity.health > 0)
                        DrawName(entity, 20, 20);
                    if (weaponName == true && enableVisuals == true && entity.health > 0)
                        DrawWeaponName(entity, 20);
                }
            }
        }

        public void UpdateSpectatorList(Swed swed, IntPtr client_dll, IntPtr entityList, IntPtr localPlayerPawn)
        {
            // Очистить текущий список наблюдателей
            spectators.Clear();
            //spectators.Add("huy");

            if (localPlayerPawn == IntPtr.Zero) return;

            IntPtr listEntryNew = swed.ReadPointer(entityList, 0x10);
            if (listEntryNew == IntPtr.Zero) return;

            for (int i = 0; i < 64; i++)
            {
                try
                {
                    IntPtr currentController = swed.ReadPointer(listEntryNew, i * 0x78);
                    if (currentController == IntPtr.Zero) continue;

                    // Получаем обработчик пешки
                    int pawnHandle = swed.ReadInt(currentController, Offsets.m_hPlayerPawn);
                    if (pawnHandle == 0) continue;

                    // Получаем пешку игрока
                    IntPtr listEntry2 = swed.ReadPointer(entityList, 0x8 * ((pawnHandle & 0x7FFF) >> 9) + 0x10);
                    if (listEntry2 == IntPtr.Zero) continue;

                    IntPtr currentPawn = swed.ReadPointer(listEntry2, 0x78 * (pawnHandle & 0x1FF));
                    if (currentPawn == IntPtr.Zero) continue;
                    if (currentPawn == localPlayerPawn) continue;

                    // Проверяем, мёртв ли игрок
                    uint lifeState = swed.ReadUInt(currentPawn, Offsets.m_lifeState);
                    if (lifeState == 256) continue; // 256 означает, что игрок жив

                    // Получаем указатель на пешку наблюдателя
                    int observerPawnHandle = swed.ReadInt(currentController, Offsets.m_hObserverPawn);
                    if (observerPawnHandle == 0) continue;

                    IntPtr listEntry3 = swed.ReadPointer(entityList, 0x8 * ((observerPawnHandle & 0x7FFF) >> 9) + 0x10);
                    if (listEntry3 == IntPtr.Zero) continue;

                    IntPtr observerPawn = swed.ReadPointer(listEntry3, 0x78 * (observerPawnHandle & 0x1FF));
                    if (observerPawn == IntPtr.Zero) continue;

                    // Проверяем сервисы наблюдателя
                    IntPtr observerServices = swed.ReadPointer(observerPawn, Offsets.m_pObserverServices);
                    if (observerServices == IntPtr.Zero) continue;

                    // Получаем цель, за которой наблюдает игрок
                    int observerTargetHandle = swed.ReadInt(observerServices, Offsets.m_hObserverTarget);
                    if (observerTargetHandle == 0) continue;

                    IntPtr listEntry4 = swed.ReadPointer(entityList, 0x8 * ((observerTargetHandle & 0x7FFF) >> 9) + 0x10);
                    if (listEntry4 == IntPtr.Zero) continue;

                    IntPtr observerTargetPawn = swed.ReadPointer(listEntry4, 0x78 * (observerTargetHandle & 0x1FF));
                    if (observerTargetPawn == IntPtr.Zero) continue;

                    // Если цель наблюдения - локальный игрок, добавляем наблюдателя в список
                    if (observerTargetPawn == localPlayerPawn)
                    {
                        string spectatorName = swed.ReadString(currentController, Offsets.m_iszPlayerName, 16).Split('\0')[0];
                        if (!string.IsNullOrEmpty(spectatorName) && !spectators.Contains(spectatorName))
                        {
                            spectators.Add(spectatorName);
                        }
                    }
                }
                catch (Exception)
                {
                    // Просто пропускаем ошибки для стабильности
                    continue;
                }
            }
        }

        private void DrawSpectatorList()
        {
            if (spectators.Count == 0) return;

            List<string> spectatorsCopy;
            lock (spectators)
            {
                try
                {
                    spectatorsCopy = new List<string>(spectators.ToArray());
                }
                catch (ArgumentException ex)
                {
                    Debug.WriteLine($"Error creating spectatorsCopy: {ex.Message}");
                    Debug.WriteLine($"spectators.Count: {spectators.Count}");
                    foreach (var spectator in spectators)
                    {
                        Debug.WriteLine($"spectator: {spectator}");
                    }
                    throw;
                }
            }

            float baseX = ImGui.GetIO().DisplaySize.X - 180;
            float baseY = 40;

            Vector4 titleColor = new Vector4(1.0f, 0.8f, 0.0f, 1.0f); // Gold color for title
            Vector4 spectatorColor = new Vector4(1.0f, 1.0f, 1.0f, 1.0f); // White for spectator names

            float width = 160;
            float height = 25 + spectatorsCopy.Count * 20;
            drawList.AddRectFilled(
                new Vector2(baseX - 5, baseY - 5),
                new Vector2(baseX + width, baseY + height),
                ImGui.ColorConvertFloat4ToU32(new Vector4(0.0f, 0.0f, 0.0f, 0.7f))
            );

            drawList.AddText(new Vector2(baseX, baseY), ImGui.ColorConvertFloat4ToU32(titleColor), "Spectators");

            drawList.AddLine(
                new Vector2(baseX, baseY + 20),
                new Vector2(baseX + width - 10, baseY + 20),
                ImGui.ColorConvertFloat4ToU32(new Vector4(0.5f, 0.5f, 0.5f, 1.0f))
            );

            for (int i = 0; i < spectatorsCopy.Count; i++)
            {
                if (!string.IsNullOrEmpty(spectatorsCopy[i]))
                {
                    Vector2 textLocation = new Vector2(baseX + 5, baseY + 25 + (i * 20));
                    drawList.AddText(textLocation, ImGui.ColorConvertFloat4ToU32(spectatorColor), spectatorsCopy[i]);
                }
            }
        }

        // Настройки прицела
        private static readonly float Size = 12.0f;
        private static readonly float Thickness = 1.5f;
        private static readonly Vector4 Color = new Vector4(1.0f, 0.0f, 0.0f, 1.0f); // Красный цвет
        private static readonly float OutlineThickness = 3.0f;
        private static readonly Vector4 OutlineColor = new Vector4(0.0f, 0.0f, 0.0f, 1.0f); // Черный контур

        public void UpdateBoneIdBasedOnWeapon()
        {
            if (Weapon.IsAWP(localPlayer.currentWeaponIndex) || Weapon.IsRifle(localPlayer.currentWeaponIndex) || Weapon.IsSMG(localPlayer.currentWeaponIndex)) boneId = 3; //hitboxes - 0,1,2,3,4 (самые приоритетные части)
            if (Weapon.IsSSG08(localPlayer.currentWeaponIndex) || Weapon.IsPistol(localPlayer.currentWeaponIndex)) boneId = 5; //hitboxes - прицел над противником, то в 6, 5, если ниже головы, то 4,3,2,1,0
            if (Weapon.IsAutoSniper(localPlayer.currentWeaponIndex)) boneId = 4;
        }
        public void DrawCrosshairInSniper(Entity localPlayer)
        {
            if (!Weapon.IsSniper(localPlayer.currentWeaponIndex) || localPlayer.scopped) return;

            var drawList = ImGui.GetBackgroundDrawList();
            var viewport = ImGui.GetMainViewport();
            Vector2 center = viewport.Size * 0.5f;

            // Рисуем контур
            DrawCrosshairPart(drawList, center, Size + 2.0f, OutlineThickness, OutlineColor);

            // Основной прицел
            DrawCrosshairPart(drawList, center, Size, Thickness, Color);

            // Центральная точка
            drawList.AddCircleFilled(center, 2.0f, ImGui.ColorConvertFloat4ToU32(Color));
        }

        private void DrawCrosshairPart(ImDrawListPtr drawList, Vector2 center, float size, float thickness, Vector4 color)
        {
            uint col = ImGui.ColorConvertFloat4ToU32(color);

            // Горизонтальные линии
            drawList.AddLine(
                new Vector2(center.X - size, center.Y),
                new Vector2(center.X + size, center.Y),
                col,
                thickness
            );

            // Вертикальные линии
            drawList.AddLine(
                new Vector2(center.X, center.Y - size),
                new Vector2(center.X, center.Y + size),
                col,
                thickness
            );
        }

        private static readonly Vector4 TextColor = new Vector4(1.0f, 1.0f, 1.0f, 1.0f); // Белый цвет
        private static readonly Vector2 WindowPadding = new Vector2(10.0f, 5.0f); // Отступы окна
        private const float TextPadding = 5.0f; // Отступ текста от краев

        public static void DrawWatermark()
        {
            var io = ImGui.GetIO();
            var viewport = ImGui.GetMainViewport();

            // Устанавливаем позицию окна в правом верхнем углу
            ImGui.SetNextWindowPos(new Vector2(
                viewport.Pos.X + viewport.Size.X - WindowPadding.X,
                viewport.Pos.Y + WindowPadding.Y),
                ImGuiCond.Always,
                new Vector2(1.0f, 0.0f));

            // Настройки стиля окна
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(TextPadding, TextPadding));
            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.0f);
            ImGui.PushStyleColor(ImGuiCol.WindowBg, new Vector4(0.0f, 0.0f, 0.0f, 0.7f)); // Полупрозрачный фон

            // Флаги окна
            ImGuiWindowFlags flags = ImGuiWindowFlags.NoDecoration
                                   | ImGuiWindowFlags.AlwaysAutoResize
                                   | ImGuiWindowFlags.NoFocusOnAppearing
                                   | ImGuiWindowFlags.NoNav
                                   | ImGuiWindowFlags.NoMove;

            if (ImGui.Begin("##Watermark", flags))
            {
                ImGui.PushStyleColor(ImGuiCol.Text, TextColor);

                // Формируем текст водяного знака
                int fps = (int)io.Framerate;
                string watermarkText = $"ImGui .NET Multi Hack | FPS overlay: {fps}";

                ImGui.TextUnformatted(watermarkText);
                ImGui.PopStyleColor();
            }

            ImGui.End();
            ImGui.PopStyleVar(2);
            ImGui.PopStyleColor();
        }

        bool EntityOnScreen(Entity entity)
        {
            Menu menu = new Menu();
            if (entity.position2D.X > 0 && entity.position2D.X < menu.screenSize.X && entity.position2D.Y > 0 && entity.position2D.Y < menu.screenSize.Y)
            {
                return true;
            }
            return false;
        }

        private void DrawName(Entity entity, int xOffset, int yOffset)
        {
            
            Vector2 textLocation = new Vector2(entity.viewPosition2D.X - xOffset, entity.viewPosition2D.Y - yOffset);

            Vector4 nameColorNew = localPlayer.team == entity.team ? teamColor : nameColor;

            if (onlyVisible == true)
                nameColorNew = entity.spotted == true ? nameColorNew : hiddenColor;
            
            if (!teammates)
            {
                if (localPlayer.team != entity.team)
                    drawList.AddText(textLocation, ImGui.ColorConvertFloat4ToU32(nameColorNew)/*ImGui.ColorConvertFloat4ToU32(nameColor)*/, $"{entity.name}");
            } else
                drawList.AddText(textLocation, ImGui.ColorConvertFloat4ToU32(nameColorNew)/*ImGui.ColorConvertFloat4ToU32(nameColor)*/, $"{entity.name}");
        }

        private void DrawWeaponName(Entity entity, int xOffset/*, int yOffset*/)
        {
            
            Vector2 textLocation = new Vector2(entity.viewPosition2D.X - xOffset, entity.position2D.Y);

            Vector4 nameColorNew = localPlayer.team == entity.team ? teamColor : nameColor;

            if (onlyVisible == true)
                nameColorNew = entity.spotted == true ? nameColorNew : hiddenColor;
            if (!teammates)
            {
                if (localPlayer.team != entity.team)
                    drawList.AddText(textLocation, ImGui.ColorConvertFloat4ToU32(nameColorNew)/*ImGui.ColorConvertFloat4ToU32(nameColor)*/, $"{entity.currentWeaponName}");

            } else
                drawList.AddText(textLocation, ImGui.ColorConvertFloat4ToU32(nameColorNew)/*ImGui.ColorConvertFloat4ToU32(nameColor)*/, $"{entity.currentWeaponName}");
        }

        //private void DrawHealthBar(Entity entity)
        //{

        //    float entityHeight = entity.position2D.Y - entity.viewPosition2D.Y;

        //    float boxLeft = entity.viewPosition2D.X - entityHeight / 3;
        //    float boxRight = entity.position2D.X + entityHeight / 3;

        //    float barPercentWidth = 0.05f;
        //    float barPixelWidth = barPercentWidth * (boxRight - boxLeft);

        //    float barHeight = entityHeight * (entity.health / 100f);

        //    Vector2 barTop = new Vector2(boxLeft - barPixelWidth, entity.position2D.Y - barHeight);
        //    Vector2 barBottom = new Vector2(boxLeft, entity.position2D.Y);

        //    Vector4 healthBarColor = new Vector4(0, 1, 0, 1);

        //    if (onlyVisible == true)
        //        healthBarColor = entity.spotted == true ? healthBarColor : hiddenColor;

        //    if (!teammates)
        //    {
        //        if (localPlayer.team != entity.team)
        //            drawList.AddRectFilled(barTop, barBottom, ImGui.ColorConvertFloat4ToU32(healthBarColor));
        //    }
        //    else
        //        drawList.AddRectFilled(barTop, barBottom, ImGui.ColorConvertFloat4ToU32(healthBarColor));
        //}

        private void DrawHealthBar(Entity entity)
        {
            float entityHeight = entity.position2D.Y - entity.viewPosition2D.Y;

            float boxLeft = entity.viewPosition2D.X - entityHeight / 3;
            float boxRight = entity.position2D.X + entityHeight / 3;

            float barPercentWidth = 0.05f;
            float barPixelWidth = barPercentWidth * (boxRight - boxLeft);

            float maxBarHeight = entityHeight;
            float currentBarHeight = entityHeight * (entity.health / 100f);

            Vector2 bgBarTop = new Vector2(boxLeft - barPixelWidth, entity.position2D.Y - maxBarHeight);
            Vector2 bgBarBottom = new Vector2(boxLeft, entity.position2D.Y);

            Vector2 healthBarTop = new Vector2(boxLeft - barPixelWidth, entity.position2D.Y - currentBarHeight);
            Vector2 healthBarBottom = new Vector2(boxLeft, entity.position2D.Y);

            Vector4 healthBarColor = new Vector4(0, 1, 0, 1); // Зелёный
            Vector4 bgColor = new Vector4(0, 0, 0, 1); // Чёрный фон
            Vector4 textColor = new Vector4(1, 1, 1, 1); // Белый текст

            if (onlyVisible && !entity.spotted)
            {
                healthBarColor = hiddenColor;
                bgColor = hiddenColor;
                textColor = hiddenColor;
            }

            drawList.AddRectFilled(bgBarTop, bgBarBottom, ImGui.ColorConvertFloat4ToU32(bgColor));

            if (teammates || localPlayer.team != entity.team)
            {
                drawList.AddRectFilled(healthBarTop, healthBarBottom, ImGui.ColorConvertFloat4ToU32(healthBarColor));
            }

            // Текст слева от бара (вверху)
            string healthText = $"{entity.health}";
            Vector2 textSize = ImGui.CalcTextSize(healthText);
            float textOffset = 5; // Отступ между текстом и баром

            // Позиция текста
            Vector2 textPos = new Vector2(
                bgBarTop.X - textSize.X - textOffset, // X: слева от бара
                bgBarTop.Y // Y: верх бара
            );

            drawList.AddText(textPos, ImGui.ColorConvertFloat4ToU32(textColor), healthText);
        }

        private void DrawBox(Entity entity)
        {
            
            float entityHeight = entity.position2D.Y - entity.viewPosition2D.Y;

            Vector2 rectTop = new Vector2(entity.viewPosition2D.X - entityHeight / 3, entity.viewPosition2D.Y);
            Vector2 rectBottom = new Vector2(entity.position2D.X + entityHeight / 3, entity.position2D.Y);

            Vector4 boxColor = localPlayer.team == entity.team ? teamColor : enemyBoxColor;

            if (onlyVisible == false)
                boxColor = entity.spotted == true ? boxColor : enemyBoxColorInvisible;
            else
                boxColor = entity.spotted == true ? boxColor : hiddenColor;

            if (!teammates)
            {
                if (localPlayer.team != entity.team)
                    drawList.AddRect(rectTop, rectBottom, ImGui.ColorConvertFloat4ToU32(boxColor));
            }
            else
                drawList.AddRect(rectTop, rectBottom, ImGui.ColorConvertFloat4ToU32(boxColor));
        }

        private void DrawLines(Entity entity)
        {
            
            Vector4 lineColor = localPlayer.team == entity.team ? teamColor : enemySnapLinesColor;


            if (onlyVisible == false)
                lineColor = entity.spotted == true ? lineColor : enemySnapLinesColorInvisible;
            else
                lineColor = entity.spotted == true ? lineColor : hiddenColor;

            if (!teammates)
            {
                if (localPlayer.team != entity.team)
                    drawList.AddLine(new Vector2(screenSize.X / 2, screenSize.Y), entity.position2D, ImGui.ColorConvertFloat4ToU32(lineColor));
            }
            else
                drawList.AddLine(new Vector2(screenSize.X / 2, screenSize.Y), entity.position2D, ImGui.ColorConvertFloat4ToU32(lineColor));
        }

        private void DrawBones(Entity entity)
        {
            
            Vector4 boneColorNew = localPlayer.team == entity.team ? teamColor : enemyBonesColor;

            if (onlyVisible == false)
                boneColorNew = entity.spotted == true ? boneColorNew : enemyBonesColorInvisible;
            else
                boneColorNew = entity.spotted == true ? boneColorNew : hiddenColor;

            uint uintColor = ImGui.ColorConvertFloat4ToU32(boneColorNew);

            //public float boneThickness = 4;

            float currentBoneThickness = 4 / entity.distance;


            if (!teammates)
            {
                if (localPlayer.team != entity.team)
                {
                    drawList = ImGui.GetWindowDrawList();

                    drawList.AddLine(entity.bones2D[1], entity.bones2D[2], uintColor, currentBoneThickness);
                    drawList.AddLine(entity.bones2D[1], entity.bones2D[3], uintColor, currentBoneThickness);
                    drawList.AddLine(entity.bones2D[1], entity.bones2D[6], uintColor, currentBoneThickness);
                    drawList.AddLine(entity.bones2D[3], entity.bones2D[4], uintColor, currentBoneThickness);
                    drawList.AddLine(entity.bones2D[6], entity.bones2D[7], uintColor, currentBoneThickness);
                    drawList.AddLine(entity.bones2D[4], entity.bones2D[5], uintColor, currentBoneThickness);
                    drawList.AddLine(entity.bones2D[7], entity.bones2D[8], uintColor, currentBoneThickness);
                    drawList.AddLine(entity.bones2D[1], entity.bones2D[0], uintColor, currentBoneThickness);
                    drawList.AddLine(entity.bones2D[0], entity.bones2D[9], uintColor, currentBoneThickness);
                    drawList.AddLine(entity.bones2D[0], entity.bones2D[11], uintColor, currentBoneThickness);
                    drawList.AddLine(entity.bones2D[9], entity.bones2D[10], uintColor, currentBoneThickness);
                    drawList.AddLine(entity.bones2D[11], entity.bones2D[12], uintColor, currentBoneThickness);
                    drawList.AddCircle(entity.bones2D[2], 3 + currentBoneThickness, uintColor);
                }
            }
            else
            {
                drawList = ImGui.GetWindowDrawList();

                drawList.AddLine(entity.bones2D[1], entity.bones2D[2], uintColor, currentBoneThickness);
                drawList.AddLine(entity.bones2D[1], entity.bones2D[3], uintColor, currentBoneThickness);
                drawList.AddLine(entity.bones2D[1], entity.bones2D[6], uintColor, currentBoneThickness);
                drawList.AddLine(entity.bones2D[3], entity.bones2D[4], uintColor, currentBoneThickness);
                drawList.AddLine(entity.bones2D[6], entity.bones2D[7], uintColor, currentBoneThickness);
                drawList.AddLine(entity.bones2D[4], entity.bones2D[5], uintColor, currentBoneThickness);
                drawList.AddLine(entity.bones2D[7], entity.bones2D[8], uintColor, currentBoneThickness);
                drawList.AddLine(entity.bones2D[1], entity.bones2D[0], uintColor, currentBoneThickness);
                drawList.AddLine(entity.bones2D[0], entity.bones2D[9], uintColor, currentBoneThickness);
                drawList.AddLine(entity.bones2D[0], entity.bones2D[11], uintColor, currentBoneThickness);
                drawList.AddLine(entity.bones2D[9], entity.bones2D[10], uintColor, currentBoneThickness);
                drawList.AddLine(entity.bones2D[11], entity.bones2D[12], uintColor, currentBoneThickness);
                drawList.AddCircle(entity.bones2D[2], 3 + currentBoneThickness, uintColor);
            }
        }

        public void UpdateEntities(IEnumerable<Entity> newEntities)
        {
            entities = new ConcurrentQueue<Entity>(newEntities);
        }

        public void UpdateLocalPlayer(Entity newEntity)
        {
            lock (entityLock)
            {
                localPlayer = newEntity;
            }
        }

        public Entity GetLocalPlayer()
        {
            lock (entityLock)
            {
                return localPlayer;
            }
        }

        void DrawOverlay(Vector2 screenSize)
        {
            ImGui.SetNextWindowSize(screenSize);
            ImGui.SetNextWindowPos(new Vector2(0, 0));
            ImGui.Begin("overlay", ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoBringToFrontOnFocus | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoInputs | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse);
        }
    }
}