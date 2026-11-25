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
| `Assets/Scenes/Login.unity` | Implementada | Diorama 3D + Canvas de acceso. |
| `Assets/Scenes/SampleScene.unity` | Plantilla URP | Se conserva como referencia; no es gameplay. |
| `Game` | Pendiente | Misión única, mapa Distrito Aurora, boss final. |
| `Results` | Pendiente | Resultados, final y créditos. |

`Login` está en Build Settings como escena 0. `SampleScene` se mantiene como escena secundaria. **No habrá escenas por capítulo.**

---

## Estructura de carpetas

```
BlenderSource/             .blend maestros (fuera de Assets; no los importa Unity)
Assets/
  Art/
    Fonts/                 Fuentes y licencias OFL
    Login/                 Shaders, materiales, texturas, mallas del login
    Models/                FBX de Blender: personajes, enemigos, naves, armas, mapa
  Config/                  ScriptableObjects de configuración
  Editor/
    Login/                 Builders y menús del login
  Scenes/                  Login, Game, Results
  Scripts/
    Login/                 Lógica de la pantalla de acceso
    Core/                  (pendiente) flujo, guardado, eventos
    Gameplay/              (pendiente) trineo, renos, armas, enemigos
    UI/                    (pendiente) HUD y menús de gameplay
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

## Sistemas planificados para gameplay

### Core
| Sistema | Responsabilidad |
|---|---|
| `MissionFlowService` | Intro, actos, checkpoint, game over, final y créditos. |
| `SaveService` | Perfil y ajustes; no guarda progreso de misión. |
| `InputReader` | Traduce Input Actions a eventos de dominio. |
| `GameEventBus` | Eventos desacoplados (reno perdido, casco dañado, boss). |
| `AudioDirector` | Música, mezcla, ducking, radio. |
| `CheckpointService` | Estado del checkpoint del puente en memoria. |

### Gameplay
| Sistema | Responsabilidad |
|---|---|
| `SleighController` | Movimiento lateral, turbo, dash, inclinación. |
| `SleighHealth` | Casco, daño, reparación, caída. |
| `ReindeerSquad` | 4 renos, pasivas, estados, rescate. |
| `WeaponController` | Arma equipada, calor, munición, cambio. |
| `Projectile` / `Damageable` | Impactos y daño. |
| `EnemyDirector` | Spawns por presupuesto de amenaza. |
| `BossController` | Fases, ataques, puntos débiles. |
| `PickupCollector` | Kits de casco, munición, escudos y armas de historia. |
| `MissionDirector` | Actos, oleadas, set pieces y boss. |

### Datos (ScriptableObjects)
- `WeaponDefinition`: daño, cadencia, munición, VFX, sonido.
- `ReindeerDefinition`: nombre, pasiva, modelo, animaciones.
- `EnemyDefinition`: vida, ataque, telegraph, prioridad.
- `WaveDefinition`: composición, presupuesto, duración.
- `MissionDefinition`: actos, checkpoints, música y objetivos.
- `BossDefinition`: fases, ataques y puntos débiles.

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

### Gameplay (pendiente)
- Tests de EditMode para daño, vidas y casco.
- Tests de PlayMode para el flujo de misión y checkpoint.
- Prueba manual de la misión completa de 20–30 minutos.

---

## Build

- Plataforma inicial: Windows x64.
- Development Build con Profiler para pruebas.
- Escenas en Build Settings: `Login`, `Game`, `Results`.
- Compresión de texturas y audio por plataforma.
- No incluir `AgentScripts` ni carpetas de pruebas en el build.

---

## Limitaciones conocidas del login

- El fondo es un diorama low-poly generado con primitivas; no representa el arte final realista.
- No hay audio todavía (viento, campanas, radio).
- La contraseña es local y no segura.
- La escena `Game` no existe; el éxito muestra el modo demo.
- La tipografía display es Bebas Neue (OFL); el cuerpo es Lato (OFL).
- Los assets generados se pueden editar, pero el menú de reconstrucción reemplaza la escena.

---

## Próximos pasos técnicos

1. Crear la escena `Game` con el blockout del único mapa.
2. Implementar `SleighController` con datos serializados.
3. Implementar `ReindeerSquad` y el evento de pérdida.
4. Añadir Sabueso y Carbón.
5. Montar el HUD como prefab editable.
6. Integrar el kit de Blender y comprobar escala/rendimiento.
7. Añadir audio de viento y campanas al login.
8. Pruebas de resolución y accesibilidad.
9. Perfilar el login en build.
