# 09 · Arquitectura técnica

> **Alcance final v1.0:** Login → una única escena `Game` → resultados/créditos. Sin capítulos, sin niveles, sin XP, sin monedas y sin mejoras. Todos los assets 3D vienen de Blender. Ver [11 · Alcance final](11_Alcance_Final_Juego_Terminado.md).

## Base del proyecto

| Campo | Valor |
|---|---|
| Unity | 6.6.0f1 |
| Render pipeline | URP 17.6 |
| Input | Input System 1.20 (solo nuevo sistema) |
| UI runtime | uGUI + TextMeshPro (login) |
| UI editor | UI Toolkit (herramientas de editor) |
| Lenguaje | C# |
| Target | PC, 1080p / 60 FPS |

### Reglas de oro

Las reglas del proyecto están en `AGENTS.md`. Resumen operativo:

1. Nada de crear escena/UI en `Awake`, `Start` o `Update`.
2. La automatización vive en Editor Scripts con `[MenuItem]`, y el resultado se guarda como escena o prefab editable.
3. Los valores de gameplay van en `[SerializeField]` o `ScriptableObject`.
4. Un script = una responsabilidad.
5. La lógica de dominio no depende de la UI ni de la cámara.

---

## Flujo de escenas

```
Login
  ↓
Game (una sola escena)
  └─ Misión única: Distrito Aurora
       ├─ Acto 1: afueras
       ├─ Acto 2: centro
       ├─ Acto 3: financiero + checkpoint
       └─ Acto 4: Refugio Aurora + boss
  ↓
Results / Credits
```

### Escenas actuales

| Escena | Estado | Contenido |
|---|---|---|
| `Assets/Scenes/Login.unity` | Implementada | Lobby con diorama 3D y botón JUGAR. |
| `Assets/Scenes/SampleScene.unity` | Plantilla URP | Se conserva como referencia; no es gameplay. |
| `Game` | Hecho (blockout) | Misión única, mapa Distrito Aurora, checkpoint, meta y game over. |
| `VAT_Preview` | Herramienta hecha | Revisión de todos los assets VAT en filas; se genera desde el menú. |
| `Results` | En la demo | Panel de resultados dentro de `Game`; final y créditos pendientes. |

`Login` está en Build Settings como escena 0 y `Game` como escena 1. `SampleScene` se mantiene
como escena secundaria. **No habrá escenas por capítulo.** La demo web usa exactamente esas dos
escenas (ver [13 · Demo web](13_Web_Demo_GitHub_Pages.md)).

---

## Estructura de carpetas

```
BlenderSource/             .blend maestros (fuera de Assets; no los importa Unity)
Assets/
  Art/
    Fonts/                 Fuentes y licencias OFL
    Login/                 Shaders, materiales, texturas, mallas del login
    Materials/VAT/         Materiales M_<Asset>_<Clip> y VAT_<Asset> (clip sets)
    Models/                FBX de Blender: personajes, enemigos, naves, armas, mapa
      VAT/                 mallas de juego + texturas de animación (ver doc 14)
        Generated/         texturas VAT RGBAHalf generadas (las usan los materiales)
  Config/
    LoginConfig.asset
    Weapons/               W_Carbon, W_Nogal, W_Estrella, W_Cascabel
    Enemies/               E_Sabueso … E_Obsidian
  Editor/
    Login/                 Builders y menús del login
    Map/                   Blockout, escena Game y HUD base
    VAT/                   Importador, materiales, prefabs y escena de prueba VAT
    Game/                  GameplayBuilder y GameHudBuilder
  Prefabs/
    Characters/            PF_Santa, PF_Sleigh, PF_Reindeer_*
    Enemies/               PF_Sabueso … PF_NicolasCero, PF_Ship_*
    Weapons/               PF_SM_Weapon_*
    Props/                 pickups, cascabeles, meteoritos, arnés…
  Scenes/                  Login, Game, VAT_Preview
  Scripts/
    Login/                 Lógica de la pantalla de acceso
    Rendering/             VATPlayer, VATClipSet, FloatPhase (animación en GPU)
    Game/
      Input/               PlayerInputReader (única lectura de entrada)
      Player/              VehicleMotor, CharacterAim, WeaponController, SleighHealth, ReindeerSquad
      Camera/              ThirdPersonCamera
      Combat/              EnemyAgent, EnemyDefinition, BossController, GiftBomb, VATTint, daño
      World/               MissionManager, ZoneStreamer, EnemyDirector, Pickups, BossGate
      UI/                  GameHud, PauseMenu, PointerReticle, ZoneTitleHud
      FX/                  TracerBeam, ImpactFlash, SnowFollower
  Shaders/
    VAT/                   VAT_Unlit.shader, FloatWobble.shader
  Settings/                URP assets, volume profiles globales
  TextMesh Pro/            Recursos esenciales de TMP
Docs/                      Documentación de diseño
AGENTS.md                  Reglas de trabajo
```

Convenciones:
- Un script por responsabilidad; nombres en PascalCase.
- Carpetas por feature, no por tipo, cuando el sistema crezca.
- Los assets generados por herramientas viven en `Art/Login` y se pueden editar.
- Los assets de configuración viven en `Config`.

---

## Login: arquitectura implementada

### Escena
`Assets/Scenes/Login.unity` contiene:

- `Environment`: cámara, luces, cielo, terreno, pueblo, iglesia, árbol, pinos, nieve y volumen de postproceso.
- `UI/LoginCanvas`: Canvas Screen Space Overlay con el panel, los dos botones, los campos y los textos.
- `EventSystem`: EventSystem + InputSystemUIInputModule.

Todo está guardado en la escena y se ve sin dar Play.

### Componentes runtime

| Componente | Responsabilidad |
|---|---|
| `LoginFormController` | Validación, estados, guardado, transición. |
| `LoginIntroAnimator` | Entrada escalonada; no crea objetos. |
| `CinematicCameraDrift` | Deriva suave de cámara. |
| `FieldFocusHighlight` | Subrayado del campo activo. |
| `RectShaker` | Feedback de error. |
| `HoverLift` | Microinteracción de botones. |
| `Spinner` | Indicador de carga. |
| `SceneQuitter` | Salir / detener Play. |

### Datos

`LoginConfig` (ScriptableObject):
- Longitudes mínima y máxima de nombre y contraseña.
- `createProfileIfMissing`.
- `nextSceneName`.
- Duraciones de carga, éxito y fundido.

Se edita en `Assets/Config/LoginConfig.asset` y se referencia desde el controlador.

### Persistencia

`LocalProfileStore` encapsula `PlayerPrefs`:
- Clave derivada del nombre normalizado + SHA-256.
- Hash de contraseña con sal.
- Último usuario.
- No es autenticación de servidor.

### Editor builders

| Script | Responsabilidad |
|---|---|
| `LoginSceneBuilder` | Orquesta la creación y guardado de la escena. |
| `LoginAssetFactory` | Fuentes TMP, sprites, materiales, skybox, volumen, config. |
| `LoginEnvironmentBuilder` | Cámara, luces, cielo, terreno, montañas, luna, nieve. |
| `LoginVillageBuilder` | Casas, iglesia, árbol, pinos, vallas, muñeco de nieve. |
| `LoginUIBuilder` | Canvas, panel, campos, botones, textos y wiring. |
| `LoginBuildMenu` | Menús `Clous-End > Login > ...`. |

Los builders son **idempotentes en assets**: si un material, sprite o config ya existe, se reutiliza para no perder ajustes manuales. La escena sí se reconstruye cuando se invoca el menú.

### Wiring
- Los botones usan `UnityEvent` persistentes visibles en el Inspector.
- Las referencias del controlador son `[SerializeField]` privadas.
- No se usa `FindObjectOfType` ni búsquedas por nombre en runtime.
- Los campos de entrada usan `TMP_InputField` con `contentType` y límites del config.

---

## Pipeline de assets 3D (VAT)

Los modelos 3D y sus animaciones están terminados en Blender y exportados listos
para Unity. El detalle completo, con la tabla de assets y el paso a paso de
importación, está en [14 · Assets 3D y pipeline VAT](14_Assets_3D_y_Pipeline_VAT.md).

### Qué entra al proyecto

| Archivo | Función |
|---|---|
| `SK_<Asset>.fbx` | malla estática, 2 capas UV, materiales |
| `T_<Asset>_<Clip>_Pos.exr` | textura de posiciones por frame (VAT) |
| `vat_<Asset>.json` | frames, fps, tamaño, bounds |
| `SK_<Asset>_LOD1/2.fbx` | niveles de detalle |
| `SK_<Asset>_COL.fbx` | colisión convexa |
| `VAT_Unlit.shader` | desplaza los vértices en GPU |
| `VATPlayer.cs` | avanza el frame por instancia |

### Texturas de posiciones en Unity

El importador EXR de Unity 6.6 transforma los valores de posición (los convierte
en valores tipo color y deforma la malla). Por eso el pipeline no usa el EXR
directamente: `BlenderSource/scripts/export_vat_raw.py` vuelca los píxeles a
float32 en `BlenderSource/VATRaw/` (ignorado por git) y `VATTextureBuilder` crea
assets `RGBAHalf` en `Assets/Art/Models/VAT/Generated`. Los materiales apuntan a
esas texturas generadas y `_FlipY` debe quedarse en **0**.

### Herramientas de importación (editor)

| Archivo | Responsabilidad |
|---|---|
| `Assets/Editor/VAT/VATPackageImporter.cs` | Copia el paquete desde `BlenderSource/VAT` y fuerza la reimportación. |
| `VATAssetPostprocessor.cs` | Aplica los ajustes obligatorios de las texturas EXR y de los FBX. |
| `VATMaterialFactory.cs` | Crea un material por clip y los materiales de los props. |
| `VATTextureBuilder.cs` | Genera texturas `RGBAHalf` nativas desde el volcado float de Blender (evita el importador EXR de Unity). |
| `VATPropBuilder.cs` | Coloca arnés, cascabeles, collar y meteoritos. |
| `VATPrefabBuilder.cs` | Genera los prefabs con `VATPlayer`, LODGroup, collider y bounds. |
| `VATPreviewSceneBuilder.cs` | Genera `Assets/Scenes/VAT_Preview.unity` con todos los assets. |
| `VATRigBuilder.cs` · `VATGameIntegration.cs` | Sustituyen los placeholders del jugador por trineo, Santa y renos. |
| `VATBuildMenu.cs` | Menú `Clous-End > VAT` (importar, prefabs, escena, integrar). |

### Cómo encaja en la arquitectura

- **No hay `Animator` ni `SkinnedMeshRenderer`.** El coste de animación pasa de
  CPU a GPU, que es lo que permite la demo web.
- `VATPlayer` no crea nada: solo escribe `_Frame` en un
  `MaterialPropertyBlock`. Cumple la regla de "nada de crear objetos en runtime".
- Los clips son materiales; cambiar de animación es cambiar la textura
  (`VATPlayer.SetClip`). `VATClipSet` agrupa los clips de cada asset y
  `EnemyAgent`/`BossController` reproducen `Death`/`Hurt`/`Attack`.
- `FloatWobble.shader` + `FloatPhase.cs` son la alternativa sin texturas para
  aliens que solo flotan.
- Los LODs son estáticos: el VAT va en LOD0. Si se quiere animación a distancia,
  hay que hornear una textura por LOD.

---

## Sistemas de gameplay implementados

### Jugador
| Sistema | Responsabilidad |
|---|---|
| `VehicleMotor` | Avance, lateral, vuelo vertical y teleport del trineo. |
| `ThirdPersonCamera` | Cámara orbital, zoom y colisión. |
| `CharacterAim` | Orienta a Santa y el pivote del arma hacia la retícula. |
| `WeaponController` | 4 armas, calor, munición, recarga, perforación y cadena. |
| `WeaponVisualSwitcher` | Muestra solo el modelo del arma equipada. |
| `SleighHealth` | Casco (100), daño, reparación y muerte. |
| `ReindeerSquad` | 4 renos, agarre del Garfio, rescate, pérdida y escudo. |
| `PickupCollector` | Recoge munición, kits, escudos y armas por cercanía. |

### Enemigos y jefe
| Sistema | Responsabilidad |
|---|---|
| `EnemyDefinition` | Vida, daño, velocidad, rango, telegraph y tipo. |
| `EnemyAgent` | IA de Sabueso, Garfio, Espora, Manto, Torre, naves y obsidiana. |
| `EnemyDirector` | Activa/desactiva 9 oleadas precolocadas por la Z del jugador. |
| `BossController` | Nicolás Cero: escudo, fases, bombas y voleas. |
| `GiftBomb` | Regalo-bomba con homing y explosión; se puede disparar. |
| `BossGate` | Barrera de energía de la arena. |
| `VATTint` | Tinte de los renos de obsidiana sin duplicar materiales. |

### Misión y UI
| Sistema | Responsabilidad |
|---|---|
| `MissionManager` | Cronómetro, checkpoint, casco, renos, boss, fin y game over. |
| `EnemyDirector` | Reinicio de oleadas en el checkpoint. |
| `GameHud` | Casco, renos, arma, calor/munición, jefe, objetivo y rescate. |
| `PauseMenu` | Pausa con Escape; reanudar, reiniciar y lobby. |

### Datos (ScriptableObjects) implementados
- `WeaponDefinition`: daño, cadencia, munición, calor, perforación y cadena.
- `EnemyDefinition`: vida, ataque, telegraph, velocidad y tipo.
- Los assets viven en `Assets/Config/Weapons` y `Assets/Config/Enemies`.

### Datos planificados (no bloquean v1)
- `ReindeerDefinition`: pasivas fijas por reno.
- `WaveDefinition`: presupuesto de amenaza en vez de oleadas fijas.
- `MissionDefinition`: actos y música.
- `BossDefinition`: fases y puntos débiles.

**No hay `UpgradeDefinition`** porque no hay mejoras en v1.

Ningún valor de gameplay debe estar en el código. Las definiciones se crean como assets en `Assets/Config`.

---

## Input

- `Assets/InputSystem_Actions.inputactions` contiene los mapas `Player` y `UI`.
- La UI del login usa `InputSystemUIInputModule` con las acciones automáticas.
- Para gameplay se añadirá un `InputReader` que expone eventos (`OnMove`, `OnFire`, `OnDash`, `OnSwitchWeapon`).
- No se permite leer `Input.GetKey` directamente en scripts de gameplay.
- El remapeo se hará sobre el asset de acciones, no sobre el código.

---

## UI

- **Login:** uGUI + TMP, Canvas real en la escena. Editable en el Inspector.
- **HUD de gameplay:** uGUI por ahora; evaluar UI Toolkit si el HUD se vuelve muy complejo.
- **Editor tools:** UI Toolkit / IMGUI según la herramienta.
- Los estilos del login se basan en colores y fuentes compartidas; si crece el menú, se extraerán a un `ScriptableObject` de tema.

---

## Guardado

### Perfil local (implementado)
`PlayerPrefs` con claves derivadas y hash. Sirve para separar partidas en el mismo equipo.

### Sin guardado de progresión (decisión v1)
- No se guarda capítulo, XP, monedas, mejoras, inventario ni armas desbloqueadas.
- La misión se juega de principio a fin; el checkpoint del puente vive en memoria y se pierde al cerrar.
- Solo se guardan el perfil y los ajustes (volumen, sensibilidad, aim assist, subtítulos).
- Si en el futuro se añade guardado de misión, se hará con un `SaveData` versionado y escritura atómica; no ahora.

---

## Preparado para multijugador

Aunque el MVP es un jugador, la arquitectura evita bloquear el futuro:

- **Dominio separado de presentación:** el daño, las vidas y las oleadas no dependen de la cámara ni del Canvas.
- **Datos inmutables:** las definiciones son ScriptableObjects de solo lectura en runtime.
- **Eventos:** los sistemas se comunican por eventos, no por referencias cruzadas.
- **Autoridad:** cuando llegue el multijugador, el servidor ejecutará los sistemas de dominio y los clientes solo presentarán.
- **Sin singletons de conveniencia:** los servicios se inyectan por referencia serializada.
- **Determinismo:** evitar `Random` global en lógica; usar un servicio de azar con semilla.

No se implementará red en el MVP. Solo se evita el diseño que obligaría a reescribirlo todo.

---

## Rendimiento

### Objetivos
- 1080p / 60 FPS en PC medio.
- 16.6 ms de frame budget.
- Login: < 1.5 ms de CPU de UI, < 2 ms de scripts.
- Gameplay: < 8 ms de CPU, < 12 ms de GPU.

### Reglas
- Object pooling para proyectiles, enemigos y VFX.
- Sin `Instantiate`/`Destroy` por frame en combate.
- LODs en personajes y arquitectura.
- Materiales compartidos y GPU instancing donde aplique.
- Sin luces en tiempo real innecesarias.
- Perfiles con el Profiler antes de añadir contenido.

---

## Pruebas

### Login (actual)
- [x] Compila sin errores.
- [x] La escena se construye y se guarda.
- [x] El perfil se crea y se recupera.
- [x] El estado de éxito aparece.
- [x] Error de contraseña incorrecta.
- [x] La pantalla se ve bien a 1920×1080 y 1280×720.
- [ ] Navegación completa con Tab y Enter (probar).
- [ ] 2560×1440 (probar).
- [ ] SALIR en build (probar).

### Gameplay (implementado, pendiente de QA)
- Compilación y construcción de la escena `Game` verificadas en una copia aislada.
- 33 agentes, 10 pickups, 9 oleadas, jefe, HUD y pausa cableados sin referencias nulas.
- Smoke test de PlayMode sin excepciones de los sistemas nuevos (el bucle de batch no
  avanza tiempo de juego, así que el combate completo se prueba en el editor).
- Pendiente: tests de EditMode para daño/vidas/casco, tests de PlayMode del flujo
  completo y prueba manual de la misión de 20–30 minutos.

---

## Build

- Plataforma inicial: Windows x64.
- Development Build con Profiler para pruebas.
- Escenas en Build Settings: `Login`, `Game`, `Results`.
- Compresión de texturas y audio por plataforma.
- No incluir `AgentScripts` ni carpetas de pruebas en el build.

---

## Control de versiones y publicación

- La única rama es `main`. Todos los cambios se commitean y se suben **directamente a `main`**; no se usan ramas de trabajo, `gh-pages` ni repositorios separados.
- La demo WebGL se publica desde `main` (raíz del repo): `index.html`, `WebGL/` y `TemplateData/`. GitHub Pages sirve esa ruta.
- Al cerrar una tarea, el cambio se commitea y se empuja a `main` (ver `AGENTS.md`).

---

## Limitaciones conocidas del login

- El fondo es un diorama low-poly generado con primitivas; no representa el arte final realista.
- No hay audio todavía (viento, campanas, radio).
- La contraseña es local y no segura.
- La escena `Game` existe y es jugable; el login carga `Game`.
- La tipografía display es Bebas Neue (OFL); el cuerpo es Lato (OFL).
- Los assets generados se pueden editar, pero el menú de reconstrucción reemplaza la escena.

---

## Próximos pasos técnicos

1. Revisar `VAT_Preview` y la escena `Game` con los modelos reales (importación y escena ya ejecutadas, doc 14).
2. Probar la misión completa con el arte final y ajustar dificultad, oleadas, daño y tiempos (doc 15).
3. Hornear Albedo/Normal/ORM de los assets para que el detalle llegue al motor.
4. Optimizar las texturas VAT generadas (124 MB de assets; normalizar por clip o BC6H).
5. Añadir audio, intro/final cinematográficos y créditos completos.
6. Integrar el kit de Blender y comprobar escala/rendimiento.
7. Pruebas de resolución y accesibilidad; build de Windows.
