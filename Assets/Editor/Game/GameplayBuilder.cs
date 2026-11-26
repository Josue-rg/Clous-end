using System.Collections.Generic;
using ClousEnd.Game;
using ClousEnd.Rendering;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace ClousEnd.EditorTools
{
    internal sealed class GameplayResult
    {
        public SleighHealth Health;
        public ReindeerSquad Squad;
        public WeaponController Weapons;
        public EnemyDirector Director;
        public BossController Boss;
        public GameHud Hud;
        public PauseMenu Pause;
        public PickupCollector Collector;
    }

    /// <summary>
    /// Monta el gameplay completo sobre el mapa: armas y vida del jugador, los
    /// cuatro renos, las oleadas de enemigos, los pickups, el jefe con sus fases
    /// y el HUD. Todo son objetos y prefabs colocados en el editor.
    /// </summary>
    internal static class GameplayBuilder
    {
        private const string WeaponFolder = "Assets/Config/Weapons";
        private const string EnemyFolder = "Assets/Config/Enemies";
        private const float BossModelScale = 2f;

        private sealed class SpawnSpec
        {
            public string prefab;
            public EnemyDefinition definition;
            public float x;
            public float y;
            public float zAhead;
        }

        public static GameplayResult Build(
            Transform root,
            MapPlayerResult player,
            MissionManager mission,
            MapAssets assets)
        {
            var gameplayRoot = new GameObject("Gameplay").transform;
            gameplayRoot.SetParent(root, false);

            var weaponDefinitions = BuildWeaponDefinitions();
            player.Weapons.Configure(weaponDefinitions);

            var health = player.Player.AddComponent<SleighHealth>();
            var squad = BuildSquad(player);
            var hud = player.Ui != null ? player.Ui.Hud : null;
            var pause = player.Ui != null ? player.Ui.Pause : null;

            var director = BuildEnemies(gameplayRoot, player, health, squad);
            var boss = BuildBoss(gameplayRoot, player, health, squad, hud, mission, assets);
            var collector = BuildPickups(gameplayRoot, player, health, squad, player.Weapons, hud);

            if (hud != null)
            {
                SerializedField.Set(hud, "player", player.Player.transform);
                SerializedField.Set(hud, "health", health);
                SerializedField.Set(hud, "squad", squad);
                SerializedField.Set(hud, "weapons", player.Weapons);
                SerializedField.Set(hud, "boss", boss);
                hud.SetBeats(new[]
                {
                    new GameHud.ObjectiveBeat { z = 0f, text = "APRENDE A VOLAR Y DISPARAR" },
                    new GameHud.ObjectiveBeat { z = 340f, text = "CRUZA EL CENTRO HISTÓRICO" },
                    new GameHud.ObjectiveBeat { z = 820f, text = "ATRAVIESA EL DISTRITO FINANCIERO" },
                    new GameHud.ObjectiveBeat { z = 1180f, text = "LLEGA AL REFUGIO AURORA" }
                });
            }

            SerializedField.Set(mission, "health", health);
            SerializedField.Set(mission, "squad", squad);
            SerializedField.Set(mission, "boss", boss);
            SerializedField.Set(mission, "director", director);
            SerializedField.Set(mission, "hud", hud);
            SerializedField.Set(mission, "pause", pause);

            if (pause != null)
                SerializedField.Set(pause, "mission", mission);

            return new GameplayResult
            {
                Health = health,
                Squad = squad,
                Weapons = player.Weapons,
                Director = director,
                Boss = boss,
                Hud = hud,
                Pause = pause,
                Collector = collector
            };
        }

        // ------------------------------------------------------------------
        // Datos
        // ------------------------------------------------------------------

        private static WeaponDefinition[] BuildWeaponDefinitions()
        {
            VATPaths.EnsureFolder(WeaponFolder);

            var carbon = EnsureWeapon(
                "W_Carbon", "CARBÓN", DamageType.Ballistic,
                6f, 0.125f, 360f, true, 1, 1.2f,
                0, 0f, true, 0.09f, 0.55f);

            var nogal = EnsureWeapon(
                "W_Nogal", "NOGAL", DamageType.Shotgun,
                11f, 0.7f, 150f, false, 6, 6.5f,
                8, 1.25f, false, 0f, 0f);

            var estrella = EnsureWeapon(
                "W_Estrella", "ESTRELLA", DamageType.Piercing,
                80f, 1.5f, 420f, false, 1, 0f,
                3, 1.9f, false, 0f, 0f);
            estrella.pierces = true;
            estrella.maxPierceTargets = 5;
            EditorUtility.SetDirty(estrella);

            var cascabel = EnsureWeapon(
                "W_Cascabel", "CASCABEL", DamageType.Electric,
                22f, 0.6f, 240f, false, 1, 0f,
                6, 1.6f, false, 0f, 0f);
            cascabel.chainTargets = 4;
            cascabel.chainRadius = 18f;
            EditorUtility.SetDirty(cascabel);

            return new[] { carbon, nogal, estrella, cascabel };
        }

        private static WeaponDefinition EnsureWeapon(
            string fileName,
            string displayName,
            DamageType damageType,
            float damage,
            float cooldown,
            float range,
            bool automatic,
            int pellets,
            float spread,
            int magazine,
            float reload,
            bool usesHeat,
            float heatPerShot,
            float cooling)
        {
            var path = WeaponFolder + "/" + fileName + ".asset";
            var weapon = AssetDatabase.LoadAssetAtPath<WeaponDefinition>(path);

            if (weapon == null)
            {
                weapon = ScriptableObject.CreateInstance<WeaponDefinition>();
                AssetDatabase.CreateAsset(weapon, path);
            }

            weapon.id = fileName;
            weapon.displayName = displayName;
            weapon.damageType = damageType;
            weapon.damage = damage;
            weapon.cooldown = cooldown;
            weapon.range = range;
            weapon.automatic = automatic;
            weapon.pellets = pellets;
            weapon.spreadDegrees = spread;
            weapon.magazine = magazine;
            weapon.reloadTime = reload;
            weapon.usesHeat = usesHeat;
            weapon.heatPerShot = heatPerShot;
            weapon.heatCooling = cooling;
            weapon.overheatLock = 1.2f;
            weapon.pierces = false;
            weapon.chainTargets = 0;
            weapon.chainRadius = 18f;
            EditorUtility.SetDirty(weapon);
            return weapon;
        }

        private static EnemyDefinition EnsureEnemy(
            string fileName,
            string displayName,
            EnemyKind kind,
            float maxHealth,
            float moveSpeed,
            float damage,
            float attackRange,
            float cooldown,
            float telegraph,
            float chargeSpeed,
            float chargeDuration,
            float hitRadius,
            float grabDuration,
            float modelScale)
        {
            VATPaths.EnsureFolder(EnemyFolder);

            var path = EnemyFolder + "/" + fileName + ".asset";
            var enemy = AssetDatabase.LoadAssetAtPath<EnemyDefinition>(path);

            if (enemy == null)
            {
                enemy = ScriptableObject.CreateInstance<EnemyDefinition>();
                AssetDatabase.CreateAsset(enemy, path);
            }

            enemy.kind = kind;
            enemy.displayName = displayName;
            enemy.maxHealth = maxHealth;
            enemy.moveSpeed = moveSpeed;
            enemy.damage = damage;
            enemy.attackRange = attackRange;
            enemy.attackCooldown = cooldown;
            enemy.telegraphTime = telegraph;
            enemy.chargeSpeed = chargeSpeed;
            enemy.chargeDuration = chargeDuration;
            enemy.hitRadius = hitRadius;
            enemy.grabDuration = grabDuration;
            enemy.modelScale = Mathf.Max(0.1f, modelScale);
            EditorUtility.SetDirty(enemy);
            return enemy;
        }

        private static EnemyDefinition[] BuildEnemyDefinitions()
        {
            return new[]
            {
                EnsureEnemy("E_Sabueso", "SABUESO", EnemyKind.Sabueso,
                    30f, 26f, 10f, 8f, 2.8f, 0.45f, 40f, 0.8f, 5f, 0f, 2.4f),
                EnsureEnemy("E_Garfio", "GARFIO", EnemyKind.Garfio,
                    40f, 20f, 0f, 55f, 7f, 0.9f, 30f, 0.5f, 4f, 5f, 2f),
                EnsureEnemy("E_Espora", "ESPORA", EnemyKind.Espora,
                    18f, 22f, 16f, 7f, 4f, 0.4f, 34f, 1f, 5f, 0f, 2.4f),
                EnsureEnemy("E_Manto", "MANTO", EnemyKind.Manto,
                    40f, 16f, 12f, 75f, 4.5f, 1.2f, 0f, 0f, 4f, 0f, 2f),
                EnsureEnemy("E_Torre", "TORRE", EnemyKind.Torre,
                    80f, 14f, 14f, 9f, 5.5f, 0.6f, 26f, 0.9f, 6f, 0f, 2f),
                EnsureEnemy("E_ShipCaza", "NAVE CAZA", EnemyKind.Ship,
                    50f, 20f, 12f, 90f, 4f, 1f, 0f, 0f, 4f, 0f, 2.2f),
                EnsureEnemy("E_ShipRecolector", "NAVE RECOLECTOR", EnemyKind.Ship,
                    90f, 16f, 14f, 95f, 4.5f, 1.1f, 0f, 0f, 4f, 0f, 2f),
                EnsureEnemy("E_Obsidian", "RENO DE OBSIDIANA", EnemyKind.Obsidian,
                    65f, 26f, 18f, 10f, 4f, 0.5f, 42f, 0.8f, 6f, 0f, 1.8f)
            };
        }

        // ------------------------------------------------------------------
        // Jugador y renos
        // ------------------------------------------------------------------

        private static ReindeerSquad BuildSquad(MapPlayerResult player)
        {
            var squad = player.Player.AddComponent<ReindeerSquad>();
            var displayNames = new[] { "RODOLFO", "DONNER", "VIXEN", "COMETA" };
            var prefabNames = new[] { "Reindeer_Rodolfo", "Reindeer_Donner", "Reindeer_Vixen", "Reindeer_Cometa" };
            var visuals = new GameObject[4];
            var playbacks = new VATPlayer[4];

            var squadRoot = player.Player.transform.Find("ReindeerSquad");

            if (squadRoot == null)
            {
                Debug.LogWarning("[Clous-End Gameplay] No hay ReindeerSquad en el jugador. " +
                                 "Importa el paquete VAT y reconstruye la escena.");
                squadRoot = new GameObject("ReindeerSquad").transform;
                squadRoot.SetParent(player.Player.transform, false);
            }

            for (var i = 0; i < prefabNames.Length; i++)
            {
                var visual = squadRoot.Find(prefabNames[i]);

                if (visual == null)
                    continue;

                visuals[i] = visual.gameObject;
                playbacks[i] = visual.GetComponent<VATPlayer>();
            }

            squad.Configure(displayNames, visuals, playbacks);
            return squad;
        }

        // ------------------------------------------------------------------
        // Enemigos
        // ------------------------------------------------------------------

        private static EnemyDirector BuildEnemies(
            Transform parent,
            MapPlayerResult player,
            SleighHealth health,
            ReindeerSquad squad)
        {
            var definitions = BuildEnemyDefinitions();
            var sabueso = definitions[0];
            var garfio = definitions[1];
            var espora = definitions[2];
            var manto = definitions[3];
            var torre = definitions[4];
            var shipCaza = definitions[5];
            var shipRecolector = definitions[6];

            var directorGo = new GameObject("EnemyDirector");
            directorGo.transform.SetParent(parent, false);

            var director = directorGo.AddComponent<EnemyDirector>();
            SerializedField.Set(director, "player", player.Player.transform);

            var waves = new List<EnemyWave>();

            waves.Add(BuildWave(parent, player.Player.transform, health, squad, "PRIMERA PRESIÓN", 150f,
                Spawn("Sabueso", sabueso, -8f, 3f, 4f),
                Spawn("Sabueso", sabueso, 8f, 4f, 6f),
                Spawn("Espora", espora, 0f, 3f, 5f)));

            waves.Add(BuildWave(parent, player.Player.transform, health, squad, "LA JAURÍA", 320f,
                Spawn("Sabueso", sabueso, -10f, 2f, 4f),
                Spawn("Sabueso", sabueso, 10f, 3f, 5f),
                Spawn("Sabueso", sabueso, 0f, 5f, 8f),
                Spawn("Espora", espora, 6f, 2f, 4f)));

            waves.Add(BuildWave(parent, player.Player.transform, health, squad, "EL PRIMER GARFIO", 500f,
                Spawn("Garfio", garfio, -12f, 6f, 14f),
                Spawn("Sabueso", sabueso, 8f, 3f, 4f),
                Spawn("Espora", espora, -4f, 2f, 5f)));

            waves.Add(BuildWave(parent, player.Player.transform, health, squad, "FRANCOTIRADORES", 700f,
                Spawn("Robot_Manto", manto, -14f, 8f, 28f),
                Spawn("Robot_Manto", manto, 14f, 8f, 30f),
                Spawn("Sabueso", sabueso, 0f, 3f, 5f)));

            waves.Add(BuildWave(parent, player.Player.transform, health, squad, "ENJAMBRE", 860f,
                Spawn("Espora", espora, -6f, 2f, 4f),
                Spawn("Espora", espora, 0f, 4f, 6f),
                Spawn("Espora", espora, 6f, 2f, 4f),
                Spawn("Sabueso", sabueso, 10f, 3f, 5f),
                Spawn("Garfio", garfio, -12f, 5f, 12f)));

            waves.Add(BuildWave(parent, player.Player.transform, health, squad, "TORRE DE GUARDIA", 960f,
                Spawn("Robot_Torre", torre, -8f, 3f, 6f),
                Spawn("Sabueso", sabueso, 8f, 3f, 4f),
                Spawn("Sabueso", sabueso, -14f, 4f, 8f)));

            waves.Add(BuildWave(parent, player.Player.transform, health, squad, "NAVES", 1100f,
                Spawn("Ship_Caza", shipCaza, -14f, 8f, 30f),
                Spawn("Robot_Manto", manto, 12f, 7f, 26f),
                Spawn("Espora", espora, 0f, 2f, 5f)));

            waves.Add(BuildWave(parent, player.Player.transform, health, squad, "EL ASEDIO", 1220f,
                Spawn("Robot_Torre", torre, -10f, 3f, 7f),
                Spawn("Robot_Torre", torre, 10f, 3f, 7f),
                Spawn("Garfio", garfio, -12f, 6f, 14f),
                Spawn("Espora", espora, 4f, 2f, 5f)));

            waves.Add(BuildWave(parent, player.Player.transform, health, squad, "LA ÚLTIMA ESCUELA", 1300f,
                Spawn("Ship_Recolector", shipRecolector, 12f, 9f, 32f),
                Spawn("Ship_Caza", shipCaza, -14f, 8f, 30f),
                Spawn("Robot_Manto", manto, 0f, 7f, 28f)));

            director.SetWaves(waves.ToArray());
            return director;
        }

        private static SpawnSpec Spawn(
            string prefab,
            EnemyDefinition definition,
            float x,
            float y,
            float zAhead)
        {
            return new SpawnSpec
            {
                prefab = prefab,
                definition = definition,
                x = x,
                y = y,
                zAhead = zAhead
            };
        }

        private static EnemyWave BuildWave(
            Transform parent,
            Transform player,
            SleighHealth health,
            ReindeerSquad squad,
            string name,
            float triggerZ,
            params SpawnSpec[] spawns)
        {
            var waveGo = new GameObject("Wave_" + name.Replace(' ', '_'));
            waveGo.transform.SetParent(parent, false);
            waveGo.transform.localPosition = Vector3.zero;

            var agents = new List<EnemyAgent>();

            for (var i = 0; i < spawns.Length; i++)
            {
                var spec = spawns[i];
                var prefab = VATCatalog.LoadPrefab(spec.prefab);

                if (prefab == null)
                {
                    Debug.LogWarning("[Clous-End Gameplay] Falta el prefab " + spec.prefab + " para " + name + ".");
                    continue;
                }

                var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, waveGo.transform);
                instance.name = spec.prefab + "_" + i;
                instance.transform.position = new Vector3(spec.x, 9f + spec.y, triggerZ + spec.zAhead);
                instance.transform.localScale = Vector3.one * Mathf.Max(0.1f, spec.definition.modelScale);

                MakeEnemyInstance(instance);

                var agent = instance.AddComponent<EnemyAgent>();
                SerializedField.Set(agent, "definition", spec.definition);
                SerializedField.Set(agent, "target", player);
                SerializedField.Set(agent, "hull", health);
                SerializedField.Set(agent, "squad", squad);
                SerializedField.Set(agent, "playback", instance.GetComponent<VATPlayer>());
                SerializedField.Set(agent, "anchorOffset", new Vector3(spec.x, spec.y, spec.zAhead));

                if (UsesBeam(spec.definition.kind))
                    SerializedField.Set(agent, "beam", CreateBeam(instance.transform, true));

                instance.SetActive(false);
                agents.Add(agent);
            }

            return new EnemyWave
            {
                name = name,
                triggerZ = triggerZ,
                agents = agents.ToArray()
            };
        }

        private static bool UsesBeam(EnemyKind kind)
        {
            return kind == EnemyKind.Manto ||
                   kind == EnemyKind.Ship ||
                   kind == EnemyKind.Garfio;
        }

        private static void MakeEnemyInstance(GameObject instance)
        {
            var colliders = instance.GetComponentsInChildren<Collider>(true);

            for (var i = 0; i < colliders.Length; i++)
                colliders[i].isTrigger = true;

            MapObjects.SetLayerRecursive(instance, 0);
        }

        private static LineRenderer CreateBeam(Transform parent, bool narrow)
        {
            var go = new GameObject("Beam", typeof(LineRenderer));
            go.transform.SetParent(parent, false);

            var line = go.GetComponent<LineRenderer>();
            line.useWorldSpace = true;
            line.positionCount = 2;
            line.startWidth = narrow ? 0.12f : 0.18f;
            line.endWidth = narrow ? 0.12f : 0.18f;
            line.numCapVertices = 2;
            line.alignment = LineAlignment.View;
            line.textureMode = LineTextureMode.Stretch;
            line.shadowCastingMode = ShadowCastingMode.Off;
            line.receiveShadows = false;
            line.sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Game/Materials/M_Map_Tracer.mat");
            line.enabled = false;
            return line;
        }

        // ------------------------------------------------------------------
        // Jefe
        // ------------------------------------------------------------------

        private static BossController BuildBoss(
            Transform parent,
            MapPlayerResult player,
            SleighHealth health,
            ReindeerSquad squad,
            GameHud hud,
            MissionManager mission,
            MapAssets assets)
        {
            var definitions = BuildEnemyDefinitions();
            var obsidianDefinition = definitions[7];

            var arenaGo = new GameObject("BossArena");
            arenaGo.transform.SetParent(parent, false);
            arenaGo.transform.position = new Vector3(0f, 10f, 1420f);

            var boss = arenaGo.AddComponent<BossController>();

            var bossPrefab = VATCatalog.LoadPrefab("NicolasCero");
            GameObject model = null;

            if (bossPrefab != null)
            {
                model = (GameObject)PrefabUtility.InstantiatePrefab(bossPrefab, arenaGo.transform);
                model.name = "NicolasCero";
                model.transform.localPosition = Vector3.zero;
                model.transform.localScale = Vector3.one * BossModelScale;

                var colliders = model.GetComponentsInChildren<Collider>(true);

                for (var i = 0; i < colliders.Length; i++)
                    colliders[i].isTrigger = true;
            }
            else
            {
                Debug.LogWarning("[Clous-End Gameplay] Falta el prefab de Nicolás Cero.");
            }

            var beam = CreateBeam(arenaGo.transform, false);

            var obsidians = BuildObsidians(parent, player, health, squad, obsidianDefinition);
            var bombs = BuildBombPool(parent, player, health);

            var gateRoot = new GameObject("BossGate");
            gateRoot.transform.SetParent(parent, false);
            gateRoot.transform.position = new Vector3(0f, 0f, 1440f);

            var blocker = gateRoot.AddComponent<BoxCollider>();
            blocker.center = new Vector3(0f, 60f, 0f);
            blocker.size = new Vector3(260f, 120f, 2f);

            var bars = new GameObject("Bars");
            bars.transform.SetParent(gateRoot.transform, false);

            for (var i = 0; i < 13; i++)
            {
                MapObjects.Box(
                    bars.transform,
                    "Bar" + i,
                    new Vector3(-120f + i * 20f, 60f, 0f),
                    new Vector3(1.2f, 120f, 1.2f),
                    assets.AlienVein,
                    false,
                    false,
                    false);
            }

            MapObjects.Box(bars.transform, "BarsTop", new Vector3(0f, 118f, 0f), new Vector3(260f, 2f, 2f),
                assets.AlienVein, false, false, false);
            MapObjects.Box(bars.transform, "BarsBottom", new Vector3(0f, 2f, 0f), new Vector3(260f, 2f, 2f),
                assets.AlienVein, false, false, false);

            var gate = gateRoot.AddComponent<BossGate>();
            SerializedField.Set(gate, "visuals", bars);
            SerializedField.Set(gate, "blocker", blocker);

            SerializedField.Set(boss, "target", player.Player.transform);
            SerializedField.Set(boss, "hull", health);
            SerializedField.Set(boss, "mission", mission);
            SerializedField.Set(boss, "hud", hud);
            SerializedField.Set(boss, "model", model);
            SerializedField.Set(boss, "modelScale", BossModelScale);
            SerializedField.Set(boss, "playback", model != null ? model.GetComponent<VATPlayer>() : null);
            SerializedField.SetArray(boss, "obsidians", obsidians.ToArray());
            SerializedField.SetArray(boss, "bombs", bombs.ToArray());
            SerializedField.Set(boss, "gate", gate);
            SerializedField.Set(boss, "beam", beam);

            return boss;
        }

        private static List<EnemyAgent> BuildObsidians(
            Transform parent,
            MapPlayerResult player,
            SleighHealth health,
            ReindeerSquad squad,
            EnemyDefinition definition)
        {
            var agents = new List<EnemyAgent>();
            var prefabs = new[] { "Reindeer_Vixen", "Reindeer_Cometa" };
            var offsets = new[] { -16f, 16f };

            for (var i = 0; i < prefabs.Length; i++)
            {
                var prefab = VATCatalog.LoadPrefab(prefabs[i]);

                if (prefab == null)
                {
                    Debug.LogWarning("[Clous-End Gameplay] Falta " + prefabs[i] + " para los renos de obsidiana.");
                    continue;
                }

                var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
                instance.name = "Obsidian_" + prefabs[i];
                instance.transform.position = new Vector3(offsets[i], 11f, 1400f);
                instance.transform.localScale = Vector3.one * Mathf.Max(0.1f, definition.modelScale);

                var capsule = instance.AddComponent<CapsuleCollider>();
                capsule.direction = 2;
                capsule.center = new Vector3(0f, 0f, 1.1f);
                capsule.height = 2.6f;
                capsule.radius = 0.7f;
                capsule.isTrigger = true;

                MapObjects.SetLayerRecursive(instance, 0);

                var lod0 = instance.transform.Find("LOD0");
                var renderer = lod0 != null ? lod0.GetComponent<Renderer>() : instance.GetComponentInChildren<Renderer>(true);
                var tint = instance.AddComponent<VATTint>();
                tint.Configure(renderer, new Color(0.035f, 0.03f, 0.05f), new Color(0.28f, 0.05f, 0.48f));

                var agent = instance.AddComponent<EnemyAgent>();
                SerializedField.Set(agent, "definition", definition);
                SerializedField.Set(agent, "target", player.Player.transform);
                SerializedField.Set(agent, "hull", health);
                SerializedField.Set(agent, "squad", squad);
                SerializedField.Set(agent, "playback", instance.GetComponent<VATPlayer>());
                SerializedField.Set(agent, "anchorOffset", new Vector3(offsets[i] * 0.5f, 3f, 5f));

                instance.SetActive(false);
                agents.Add(agent);
            }

            return agents;
        }

        private static List<GiftBomb> BuildBombPool(
            Transform parent,
            MapPlayerResult player,
            SleighHealth health)
        {
            var bombs = new List<GiftBomb>();
            var prefab = VATCatalog.LoadPrefab("SM_GiftBox_Weapon");

            if (prefab == null)
            {
                Debug.LogWarning("[Clous-End Gameplay] Falta SM_GiftBox_Weapon para las bombas del jefe.");
                return bombs;
            }

            var pool = new GameObject("BombPool").transform;
            pool.SetParent(parent, false);

            for (var i = 0; i < 8; i++)
            {
                var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, pool);
                instance.name = "Bomb_" + i;
                instance.transform.position = new Vector3(0f, 10f, 1420f);

                var colliders = instance.GetComponentsInChildren<Collider>(true);

                for (var j = 0; j < colliders.Length; j++)
                    colliders[j].isTrigger = true;

                var bomb = instance.AddComponent<GiftBomb>();
                SerializedField.Set(bomb, "target", player.Player.transform);
                SerializedField.Set(bomb, "hull", health);

                instance.SetActive(false);
                bombs.Add(bomb);
            }

            return bombs;
        }

        // ------------------------------------------------------------------
        // Pickups
        // ------------------------------------------------------------------

        private static PickupCollector BuildPickups(
            Transform parent,
            MapPlayerResult player,
            SleighHealth health,
            ReindeerSquad squad,
            WeaponController weapons,
            GameHud hud)
        {
            var root = new GameObject("Pickups").transform;
            root.SetParent(parent, false);

            var pickups = new List<Pickup>
            {
                AddPickup(root, "SM_Pickup_Ammo", PickupType.Ammo, 25f, 4f, 250f),
                AddPickup(root, "SM_Pickup_HelmetKit", PickupType.HelmetKit, 25f, -5f, 380f),
                AddPickup(root, "SM_Pickup_ReindeerShield", PickupType.ReindeerShield, 1f, 5f, 520f),
                AddPickup(root, "SM_Pickup_Ammo", PickupType.Ammo, 25f, -4f, 640f),
                AddPickup(root, "SM_Pickup_HelmetKit", PickupType.HelmetKit, 25f, 5f, 760f),
                AddPickup(root, "SM_GiftBox_Weapon", PickupType.WeaponEstrella, 1f, -5f, 900f),
                AddPickup(root, "SM_Pickup_Ammo", PickupType.Ammo, 25f, 4f, 1050f),
                AddPickup(root, "SM_GiftBox_Weapon", PickupType.WeaponCascabel, 1f, 0f, 1160f),
                AddPickup(root, "SM_Pickup_HelmetKit", PickupType.HelmetKit, 25f, -5f, 1260f),
                AddPickup(root, "SM_Pickup_ReindeerShield", PickupType.ReindeerShield, 1f, 5f, 1320f)
            };

            pickups.RemoveAll(pickup => pickup == null);

            var collector = player.Player.AddComponent<PickupCollector>();
            SerializedField.Set(collector, "health", health);
            SerializedField.Set(collector, "squad", squad);
            SerializedField.Set(collector, "weapons", weapons);
            SerializedField.Set(collector, "hud", hud);
            SerializedField.SetArray(collector, "pickups", pickups.ToArray());

            return collector;
        }

        private static Pickup AddPickup(
            Transform parent,
            string prefabName,
            PickupType type,
            float amount,
            float x,
            float z)
        {
            var prefab = VATCatalog.LoadPrefab(prefabName);

            if (prefab == null)
            {
                Debug.LogWarning("[Clous-End Gameplay] Falta el pickup " + prefabName + ".");
                return null;
            }

            var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
            instance.name = "Pickup_" + type + "_" + Mathf.RoundToInt(z);
            instance.transform.position = new Vector3(x, 11f, z);

            var colliders = instance.GetComponentsInChildren<Collider>(true);

            for (var i = 0; i < colliders.Length; i++)
                colliders[i].isTrigger = true;

            MapObjects.SetLayerRecursive(instance, 2);

            var pickup = instance.AddComponent<Pickup>();
            pickup.Configure(type, amount);
            instance.AddComponent<PickupBob>();
            return pickup;
        }
    }
}
