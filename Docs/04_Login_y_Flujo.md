# 04 · Login y flujo de entrada

> **Actualización v1.1 (lobby sin credenciales):** el acceso ya no pide nombre ni contraseña.
> Es un lobby con un único botón **JUGAR** que carga la escena `Game`. La especificación de
> perfiles que sigue se conserva como archivo de ideas; la implementación actual usa
> `LoginFormController` como lobby y `LoginConfig` solo para la escena de destino y los tiempos.

## Lobby actual (implementado)

- Panel con **DISTRITO AURORA**, **LA CIUDAD TE ESPERA**, **JUGAR** y **SALIR**.
- Sin nombre de jugador, sin contraseña y sin perfiles.
- **JUGAR** hace un fundido y carga `Game`; **SALIR** cierra el juego (en el editor detiene el Play).
- El diorama 3D, la cámara con deriva y las animaciones de entrada se conservan.

## Objetivo

La primera pantalla del juego debe vender el tono en menos de cinco segundos: **Navidad, invasión y cine**. No es un formulario de banco. Es la puerta al último refugio.

La pantalla tiene **exactamente dos botones**: `ENTRAR AL TRINEO` y `SALIR`. Todo lo demás es información, ambientación o estado.

![Vista de la pantalla de login](Images/login_preview.png)

---

## Composición

Referencia: 1920 × 1080. Canvas uGUI con `CanvasScaler` y escalado por resolución.

```
┌────────────────────────────────────────────────────────────────────┐
│  ● CLOUS-END            OPERACIÓN 24·12            AURORA-9 · 78.4 │
│                                                                    │
│   LA ÚLTIMA NAVIDAD                                                │
│                                                                    │
│   C L O U S - E N D                                                │
│   Cuatro renos. Un trineo.                                         │
│   Una ciudad que aún respira.                                      │
│                                                                    │
│   ┌──────────────────────────────┐                                 │
│   │ ACCESO AL REFUGIO            │                                 │
│   │ Perfil local · sin conexión  │                                 │
│   │                              │                                 │
│   │ NOMBRE DE JUGADOR            │                                 │
│   │ [ Escribe tu nombre        ] │                                 │
│   │                              │                                 │
│   │ CONTRASEÑA                   │                                 │
│   │ [ ••••••••                 ] │                                 │
│   │                              │                                 │
│   │ [   ENTRAR AL TRINEO    ›  ] │                                 │
│   │ [          SALIR           ] │                                 │
│   └──────────────────────────────┘                                 │
│                                                                    │
│   v0.1 · Prototipo           4 RENOS EN LÍNEA  ● ● ● ●             │
│   Pulsa ENTER para acceder                                         │
└────────────────────────────────────────────────────────────────────┘
```

El fondo 3D ocupa toda la pantalla. La columna izquierda se apoya en un degradado oscuro para garantizar legibilidad. La escena visible a la derecha: pueblo nevado, árboles, luna, aurora y una sombra alargada en el cielo.

---

## Jerarquía visual

1. **Título `CLOUS-END`.** 96 px, mayúsculas, tracking amplio, blanco hueso. Es lo primero que se lee.
2. **Botón primario.** Rojo Santa, texto crema, brillo cálido. Único elemento rojo saturado de la pantalla.
3. **Campos.** Glass oscuro, borde fino, foco en hielo.
4. **Tagline y microcopy.** Legibles, nunca compitiendo con el título.
5. **Ambientación.** Versión, frecuencia, cuatro puntos de reno.

---

## Tokens de diseño

| Token | Valor | Uso |
|---|---|---|
| `--ink` | `#05080F` | Fondo, sombras |
| `--ink-soft` | `#0B1220` | Panel |
| `--snow` | `#F4F7FA` | Texto principal |
| `--ice` | `#A8C4D9` | Texto secundario |
| `--ice-bright` | `#D9ECFA` | Foco, bordes activos |
| `--red` | `#C8202F` | Botón primario |
| `--red-hot` | `#E63946` | Hover / glow |
| `--amber` | `#F2B25C` | Luces del pueblo, detalles |
| `--aurora` | `#5FE3C8` | Acento frío, estado «señal» |
| `--danger` | `#FF6B6B` | Error |
| `--glass` | `rgba(7, 13, 23, 0.74)` | Panel |
| `--line` | `rgba(217, 236, 250, 0.16)` | Bordes |
| `--radius` | `18 px` | Panel |
| `--radius-sm` | `10 px` | Campos y botones |

Tipografía:
- **Display:** Bebas Neue (título y marca). Licencia OFL incluida en `Assets/Art/Fonts/Source`.
- **Texto:** Lato Regular / Bold (cuerpo, etiquetas, botones). Licencia OFL incluida.

---

## Copy definitivo

| Elemento | Texto |
|---|---|
| Marca | `CLOUS-END` |
| Eyebrow | `LA ÚLTIMA NAVIDAD` |
| Tagline | `Cuatro renos. Un trineo. Una ciudad que aún respira.` |
| Título del panel | `ACCESO AL REFUGIO` |
| Nota | `Perfil local · sin conexión` |
| Campo 1 | `NOMBRE DE JUGADOR` · placeholder `Escribe tu nombre` |
| Campo 2 | `CONTRASEÑA` · placeholder `Mínimo 4 caracteres` |
| Botón 1 | `ENTRAR AL TRINEO` |
| Botón 2 | `SALIR` |
| Pie | `v0.1 · Prototipo` |
| Estado | `Pulsa ENTER para acceder` |
| Señal | `AURORA-9 · 78.4 MHz` |
| Renos | `4 RENOS EN LÍNEA` |

---

## Estados

### Inicial
- Campos vacíos.
- Botón primario activo pero el formulario no envía si faltan datos.
- Animación de entrada escalonada (título → tagline → panel → botones).

### Foco
- Borde del campo en `--ice-bright`.
- Etiqueta del campo se ilumina.
- Cursor visible y selección de texto con color de acento.

### Error de validación
- Mensaje bajo el campo: `Escribe un nombre de al menos 2 caracteres` o `La contraseña necesita 4 caracteres`.
- Borde rojo, sacudida horizontal de 220 ms.
- El foco vuelve al campo culpable.
- No se usa un popup.

### Credenciales incorrectas
- Mensaje del panel: `No encontramos ese perfil. Revisa el nombre o la contraseña.`
- Tono neutro, sin culpar.
- Enlace textual no interactivo: no hay recuperación de cuenta en el prototipo.

### Cargando
- El botón primario cambia a `ABRIENDO EL REFUGIO…`.
- Spinner circular de 16 px.
- Los campos y el botón secundario se deshabilitan.
- Duración mínima: 900 ms para que la transición se sienta deliberada.

### Éxito
- `ACCESO CONCEDIDO` aparece en verde aurora.
- Fundido a negro de 600 ms.
- Carga de la escena de juego (`Game`), o mensaje de demo si la escena no existe todavía.

### Salir
- `Application.Quit()` en build.
- En el editor, detiene el Play Mode.
- Sin diálogo de confirmación: es una decisión clara.

---

## Animación

| Momento | Movimiento | Duración |
|---|---|---|
| Entrada del título | Opacidad 0→1, desplazamiento 24 px→0 | 700 ms |
| Tagline | Ídem, retraso 120 ms | 600 ms |
| Panel | Ídem, retraso 240 ms | 650 ms |
| Botones | Ídem, retraso 360 ms | 500 ms |
| Hover primario | Elevación 2 px, brillo +12 % | 180 ms |
| Hover secundario | Fondo glass +8 %, borde +20 % | 180 ms |
| Foco de campo | Borde y glow | 150 ms |
| Error | Sacudida X ±8 px, 3 ciclos | 220 ms |
| Éxito | Flash suave + fundido | 600 ms |

Reglas:
- Nada de animaciones que bloqueen la escritura.
- El botón primario responde también a `Enter`.
- `Tab` recorre nombre → contraseña → entrar → salir.
- `Esc` no cierra el juego: para salir hay que pulsar `SALIR` (evita salidas accidentales).

---

## Fondo 3D

La escena de login no es una imagen estática. Es un diorama vivo:

- **Cielo:** degradado nocturno, estrellas, aurora boreal animada y luna baja.
- **Pueblo:** casas de madera con ventanas cálidas, chimeneas humeando, luces de Navidad.
- **Bosque:** pinos nevados, primer plano en el borde derecho.
- **Nieve:** partículas finas, viento lateral suave.
- **Amenaza:** una sombra alargada o una luz morada en el horizonte; nunca un monstruo explícito.
- **Cámara:** deriva lentísima (dolly + parallax) para que la pantalla respire.

El fondo se construye con URP, niebla, bloom y viñeta. La UI es un **Canvas real** (uGUI) con objetos que existen en la Jerarquía: se puede abrir la escena y editar cada texto, botón y color sin dar Play.

---

## Implementación

| Archivo | Función |
|---|---|
| `Assets/Scenes/Login.unity` | Escena completa: diorama 3D, Canvas, lobby, botones y textos. |
| `Assets/Scripts/Login/LoginConfig.cs` | ScriptableObject con la escena siguiente y los tiempos. |
| `Assets/Scripts/Login/LoginFormController.cs` | Lobby: botón JUGAR y transición a `Game`. |
| `Assets/Scripts/Login/LocalProfileStore.cs` | Sin uso en el lobby v1.1; se conserva como archivo de ideas. |
| `Assets/Scripts/Login/LoginIntroAnimator.cs` | Entrada escalonada de los bloques. |
| `Assets/Scripts/Login/CinematicCameraDrift.cs` | Deriva de cámara del diorama. |
| `Assets/Scripts/Login/FieldFocusHighlight.cs` | Archivo: subrayado de campos del login antiguo. |
| `Assets/Scripts/Login/RectShaker.cs` | Archivo: sacudida de error del login antiguo. |
| `Assets/Scripts/Login/HoverLift.cs` | Elevación de botones al pasar el cursor. |
| `Assets/Scripts/Login/Spinner.cs` | Giro del indicador de carga. |
| `Assets/Scripts/Login/SceneQuitter.cs` | Salir en build / detener Play en editor. |
| `Assets/Editor/Login/LoginSceneBuilder.cs` | Constructor de la escena desde un `[MenuItem]`. |
| `Assets/Editor/Login/LoginUIBuilder.cs` | Crea el Canvas, el lobby y los botones. |
| `Assets/Editor/Login/LoginEnvironmentBuilder.cs` | Crea cámara, cielo, luces, terreno y nieve. |
| `Assets/Editor/Login/LoginVillageBuilder.cs` | Crea pueblo, iglesia, árbol, pinos y muñeco de nieve. |
| `Assets/Editor/Login/LoginAssetFactory.cs` | Genera fuentes, sprites, materiales y postproceso. |
| `Assets/Config/LoginConfig.asset` | Valores editables del login. |
| `Assets/Art/Login/` | Shaders, materiales, texturas y mallas del diorama. |

### Cómo se edita

1. Abrir `Assets/Scenes/Login.unity`.
2. Seleccionar cualquier objeto de la Jerarquía (Canvas, panel, botón, casa, árbol).
3. Ajustar posición, tamaño, color, texto o referencias en el Inspector.
4. Guardar la escena.

Para regenerar la escena desde cero:

- Menú **Clous-End > Login > Construir escena de Login**.
- Sobrescribe `Login.unity`; la herramienta avisa antes.
- Los assets de `Assets/Art/Login` y `Assets/Config` **no** se sobrescriben si ya existen, para respetar ajustes manuales.

### Reglas de implementación
- La UI **no se crea en `Awake` ni en `Start`**. Todo está en la escena.
- Los valores de validación, tiempos, textos y escena siguiente viven en `LoginConfig` o en campos `[SerializeField]`.
- El controlador no conoce la geometría de la UI: solo referencias serializadas.
- La lógica de perfil está separada de la vista (`LocalProfileStore`).
- El editor script es la única automatización; el resultado es un asset persistente y editable.

### Persistencia local
- `PlayerPrefs`: nombre visible, hash SHA-256 con sal, fecha de creación y último usuario.
- La contraseña **no** es seguridad real. Es un perfil local para separar partidas.
- Si el nombre no existe, se crea el perfil en el primer acceso.
- La capa `LocalProfileStore` puede sustituirse por un backend sin tocar la UI ni el controlador.

### Flujo de estados
`Inicial → Validación → Error | Cargando → Éxito → (carga de escena | modo demo)`

- Si `nextSceneName` no está en Build Settings, se muestra el mensaje de demo y se devuelve el formulario a su estado normal.
- La escena de juego se cargará cuando exista `Game` y esté añadida a Build Settings.

### Accesibilidad del login
- Navegación completa por teclado.
- Etiquetas asociadas a los campos.
- Contraste mínimo AA en textos y botones.
- El error no depende solo del color: incluye texto.
- Sin *flashing*: la aurora y el glow no superan 3 Hz.

---

## Criterios de aceptación

- [x] La pantalla se ve correctamente a 1920×1080.
- [x] La pantalla se ve correctamente a 1280×720.
- [ ] La pantalla se ve correctamente a 2560×1440 (pendiente de prueba).
- [x] Solo hay dos botones interactivos.
- [x] `Enter` envía el formulario; `Tab` navega; el foco es visible.
- [x] Los errores se muestran en línea, sin popups.
- [x] El perfil se guarda y se recupera entre sesiones.
- [x] El fondo 3D se mueve y no distrae de la lectura.
- [x] El primer segundo ya transmite «Navidad + invasión».
- [x] En editor, `SALIR` detiene el Play Mode; en build cierra el juego.
