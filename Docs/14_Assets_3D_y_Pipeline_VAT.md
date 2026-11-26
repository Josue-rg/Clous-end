# 14 · Assets 3D y pipeline VAT

> **Estado:** todos los modelos 3D de la v1 están hechos en Blender, animados y
> **listos para importarse a Unity**. Este documento explica en simple qué se
> hizo y exactamente qué hay que hacer para que funcione en el motor.
>
> Documentos relacionados: [06 Personajes](06_Personajes_Vehiculos.md) ·
> [07 Monstruos](07_Monstruos_y_Objetos.md) · [09 Arquitectura](09_Arquitectura_Tecnica.md) ·
> [11 Alcance](11_Alcance_Final_Juego_Terminado.md)

---

## 1. Qué se hizo, en una frase

Se modelaron, texturizaron con materiales PBR y **animaron** todos los assets 3D
del juego (Santa, trineo, 4 renos, 4 armas, enemigos, naves, boss, props), y se
exportaron a un formato que Unity usa **sin esqueletos**: la animación va
horneada en una imagen y un shader mueve la malla en la GPU.

**El paquete se importa con una herramienta de editor.** `Assets/Editor/VAT/`
copia los FBX, EXR y JSON a `Assets/`, fuerza los ajustes de importación, crea un
material por clip y arma los prefabs con `VATPlayer`, LODs y collider. El paso a
paso está en la sección 5. La primera importación real está pendiente de
ejecutar el menú en Unity; las herramientas ya están hechas y compiladas.

---

## 2. Qué hay exactamente

### Personajes y vehículos

| Asset | Triángulos | Clips horneados |
|---|---|---|
| **Santa** | 46.000 | Idle 30f · Draw 34f · Shoot 14f · Reload 26f |
| **Reno** (Rodolfo, Donner, Vixen, Cometa) | 22.000–24.000 c/u | Gallop 24f · Death 38f · Hurt 16f |
| **Trineo** | 33.000 | Flight 36f · Impact 18f |

### Enemigos, naves y boss

| Asset | Triángulos | Clips |
|---|---|---|
| Sabueso | 1.400 | Run 20f · Death 24f |
| Garfio | 2.450 | Hover 36f · Death 28f |
| Espora | 2.544 | Pulse 24f · Death 18f |
| Robot Manto | 1.464 | Idle 36f · Attack 28f |
| Robot Torre | 1.564 | Idle 36f · Attack 28f |
| Nicolás Cero | 9.192 | Idle 48f · Attack 40f |
| Nave Caza | 1.028 | Hover 40f · Attack 30f |
| Nave Recolector | 3.206 | Hover 40f · Attack 30f |

### Armas y objetos (sin animación)

Carbón, Nogal, Estrella, Cascabel · caja de armas · munición · kit de casco ·
escudo de reno · collar · cascabeles · tres meteoritos · set de arnés y riendas.

### Cada asset trae además

- **LOD1** (~45 % de triángulos) y **LOD2** (~18 %).
- **`_COL`**: malla convexa de colisión (entre 39 y 81 triángulos).
- Un **JSON** con frames, FPS, tamaño de textura y bounds. Es la fuente de verdad
  para configurar Unity.

---

## 3. Cómo funciona la animación (VAT explicado simple)

**VAT = Vertex Animation Texture.** La idea, sin tecnicismos:

1. En Blender se anima con huesos, como siempre.
2. Para cada frame, se apunta **dónde está cada vértice** de la malla.
3. Esas posiciones se guardan en una imagen: el **rojo** es la X, el **verde** la
   Y y el **azul** la Z de cada vértice.
4. En Unity **no hay esqueleto**. Un shader lee la imagen, busca la fila de su
   frame y coloca cada vértice ahí. Lo hace la GPU.

Por eso es tan barato en WebGL: no hay `Animator`, no hay
`SkinnedMeshRenderer` y no hay cálculo de piel en CPU. Con GPU instancing, 18
enemigos iguales pueden ser un solo draw call.

**Limitación honesta:** un clip = una textura. Para cambiar de animación se
cambia la textura del material (`VATPlayer.SetClip`). No hay mezcla entre dos
clips (para eso haría falta un shader con dos texturas y un factor `_Blend`).

**Atlas UV y clips de distinta duración.** El índice de vértice (UV1) se hornea
una sola vez con el clip más largo del asset, pero cada clip tiene su propia
textura (`alto = filas × frames`). Por eso el material lleva `_UVFrames` = el
mayor número de frames del asset: el shader reescala la fila base a la altura de
la textura de ese clip. Sin ese ajuste, las piezas que ocupan las filas altas
(patas, astas, arnés) caen sobre el cuerpo y el modelo parece un muñeco.

La alternativa sin texturas está también incluida: **`FloatWobble.shader`**, que
anima con senos en el vertex shader. Sirve para aliens que solo flotan y no gasta
memoria de animación.

---

## 4. Dónde está todo

```
BlenderSource/                    fuera de Assets (Unity no lo importa)
  Characters/ Vehicles/ Weapons/ Enemies/ Ships/ Boss/ Props/
                                  modelos de revisión, alta densidad
  VAT/                            PAQUETE DE JUEGO (esto es lo que va a Unity)
    <Asset>/
      SK_<Asset>.fbx              malla + 2 capas UV + materiales
      T_<Asset>_<Clip>_Pos.exr    textura de animación
      vat_<Asset>.json            frames, fps, textura, bounds
      SK_<Asset>_vat.blend        rig + clips (para rehornear)
      LOD/                        LOD1, LOD2, _COL y lod_<Asset>.json
    Static/<Prop>/                props sin animación
  UnityPackage/
    VAT_Unlit.shader              shader principal
    FloatWobble.shader            animación matemática (sin texturas)
    VATPlayer.cs                  reproduce el clip, fase por instancia
    FloatPhase.cs                 desfase aleatorio para FloatWobble
    README_VAT_Unity.md           guía detallada de import
```

Y en Unity, lo que genera el importador:

```
Assets/
  Art/
    Models/VAT/<Asset>/           FBX, EXR, JSON y LOD/ copiados del paquete
      Static/<Prop>/              props sin animación
    Materials/VAT/                M_<Asset>_<Clip>.mat y M_<Asset>_LOD.mat
  Shaders/VAT/                    VAT_Unlit.shader, FloatWobble.shader
  Scripts/Rendering/              VATPlayer.cs, FloatPhase.cs
  Prefabs/
    Characters/                   PF_Santa, PF_Sleigh, PF_Reindeer_*
    Enemies/                      PF_Sabueso … PF_NicolasCero, PF_Ship_*
    Weapons/                      PF_SM_Weapon_*
    Props/                        PF_SM_Pickup_*, PF_SM_Meteor_*, …
  Scenes/VAT_Preview.unity        escena de revisión de todos los assets
```

`VATPlayer` y `FloatPhase` viven en el espacio de nombres `ClousEnd.Rendering`.

Cada prefab animado es un root con `VATPlayer` (y `LODGroup` en enemigos) y un
hijo `LOD0` con la malla y los materiales; `LOD1`/`LOD2` son hijos estáticos. Los
props estáticos son un solo objeto con `MeshRenderer` y `BoxCollider`.

Peso total de las texturas de animación: **26,5 MB** (EXR 16 bits con ZIP).

---

## 5. Cómo hacerlo funcionar en Unity

### 5.1 · Vía rápida (la que usa el proyecto)

En Unity, en la barra de menús:

1. **`Clous-End > VAT > Importar todo (paquete + prefabs)`**
   - Copia `BlenderSource/VAT` a `Assets/Art/Models/VAT`.
   - Copia los shaders y los scripts si todavía no existen.
   - Fuerza los ajustes de textura y de modelo de cada asset.
   - Crea un material por clip y un prefab por asset, con su clip por defecto y
     con LODGroup/collider cuando corresponde.
2. **`Clous-End > VAT > 3 · Crear escena de prueba VAT`**
3. Abre `Assets/Scenes/VAT_Preview.unity` y dale **Play**. Todos los assets
   deben verse en filas y animarse.
4. **`Clous-End > VAT > 4 · Integrar modelos en la escena Game`** cuando la
   revisión esté bien: sustituye los placeholders del jugador (cubo, cilindro y
   arma) por el trineo, Santa y los cuatro renos, sin regenerar el mapa.
   > Ese menú solo cambia el arte del jugador. Para tener el gameplay completo
   > (combate, oleadas, HUD y jefe) hay que reconstruir la escena con
   > **`Clous-End > Mapa > Construir escena Game (blockout)`**, que ya monta el
   > rig y el combate (doc 15).

Los pasos manuales de abajo quedan como referencia de qué hace la herramienta;
no hace falta repetirlos a mano.

> Si la malla aparece en la pose de reposo o con la escala mal, vuelve a ejecutar
> **Importar todo**: el importador fuerza los ajustes y el constructor corrige
> las diferencias de unidades.

### 5.2 · Paso manual de referencia

> Orden recomendado: primero **un** asset (Santa), comprobar que se mueve, y
> después repetir con el resto.

#### Paso 1 · Copiar los archivos
Copia a `Assets/`:

```
Assets/Art/Models/VAT/<Asset>/SK_<Asset>.fbx
Assets/Art/Models/VAT/<Asset>/T_<Asset>_<Clip>_Pos.exr
Assets/Art/Models/VAT/<Asset>/LOD/...
Assets/Shaders/VAT/VAT_Unlit.shader
Assets/Shaders/VAT/FloatWobble.shader
Assets/Scripts/Rendering/VATPlayer.cs
Assets/Scripts/Rendering/FloatPhase.cs
```

#### Paso 2 · Importar el FBX

- Scale Factor **1**, Convert Units **ON**, Bake Axis Conversion **OFF**.
  El FBX de Blender trae la conversión de ejes como una rotación de raíz (90° en
  X). El importador la copia al prefab para que la malla Z-up quede en Y arriba;
  por eso no hay que hornearla.
- Import Cameras y Lights **OFF**.
- Normals **Import**. Tangents **None** (todavía no hay normal maps).
- Materials **Import**.

El FBX trae **dos capas UV**:
- **UV0** (`UVMap`): atlas, para el horneado de albedo futuro.
- **UV1** (`VAT`): el índice de vértice que usa el shader.

Unity las importa como `uv` y `uv2`. El shader lee `uv2`.

#### Paso 3 · Importar la textura EXR

| Ajuste | Valor |
|---|---|
| Texture Type | Default |
| sRGB (Color Texture) | **OFF** |
| Filter Mode | **Point (no filter)** |
| Wrap Mode | **Clamp** |
| Compression | **None** |
| Max Size | 2048 o más |
| Generate Mip Maps | OFF |
| Read/Write | OFF |

Con filtrado bilineal o compresión se mezclan vértices distintos y la malla
"explota". Es el error número uno.

#### Paso 4 · Crear el material

Material nuevo con el shader **ClousEnd/VAT_Unlit**:

| Propiedad | Valor |
|---|---|
| `_PosTex` | el EXR del paso 3 |
| `_Frames` | `frames` del JSON |
| `_UVFrames` | el mayor `frames` de todos los clips del asset (0 si todos coinciden) |
| `_BaseColor` | color del material (mira el FBX) |
| `_VATAmount` | 1 |
| `_FlipY` | 0 (ver problemas) |
| `_Ambient` | 0,5 |
| `_LightDir` | dirección de la luz principal del nivel |

Marca **GPU Instancing** en el material. El importador crea un material por
submesh cuando el FBX trae varios colores, todos con el mismo EXR.

> Un material por clip. Para el Santa harás cuatro: Idle, Draw, Shoot y Reload.

#### Paso 5 · Añadir el componente

En el prefab, añade **VATPlayer**:

- `frames` y `fps`: los del JSON.
- `loop`: ON para Idle, Gallop, Hover, Flight, Run, Pulse. **OFF** para Draw,
  Shoot, Reload, Death, Hurt, Impact, Attack.
- `randomPhase`: ON en enemigos (cada uno va desfasado).
- `expandBounds`: ON, con `boundsCenter` / `boundsExtent` del JSON:
  - centro = (min + max) / 2
  - extensión = (max − min) / 2 + 0,1
  - **Los bounds del JSON están en los ejes de la malla (Blender, Z arriba).**
    El importador aplica al prefab la rotación de raíz del FBX (90° en X) y usa
    los bounds tal cual, en espacio local. No hay que convertirlos a mano.

Si no amplías los bounds, el modelo parpadea en el borde de la pantalla porque
Unity lo descarta con el frustum culling usando la pose de reposo.

#### Paso 6 · Probar

Pon el prefab en la escena, dale Play. Si se mueve, repite con el resto.

---

## 6. Problemas típicos y su solución

| Síntoma | Causa | Solución |
|---|---|---|
| La malla tiembla o se ve "cuadriculada" | Textura con filtro Bilinear o comprimida | Point + Compression None |
| Está todo negro | `_BaseColor` en negro o falta `_PosTex` | Asignar color y textura |
| Se ve la pose de reposo, sin animación | `_VATAmount` = 0, falta VATPlayer o `frames` no coincide | Revisar los tres |
| Se ve tumbado o girado | El FBX entró con los ejes de Blender | `Bake Axis Conversion` ON y reimportar |
| Todos los assets salen del mismo color | El FBX no guardó el color del material | Revisar los materiales en Blender antes de exportar |
| Se mueve solo una parte | El FBX entró con una sola capa UV | Revisar import settings del mesh |
| Las patas, astas o el arnés se hunden en el cuerpo | `_UVFrames` no coincide con el clip más largo del atlas | `_UVFrames` = mayor `frames` del asset; el importador lo pone solo |
| La animación va al revés / tumbada | Volcado del EXR | `_FlipY` = 1 |
| Desaparece al acercarse al borde | Bounds en reposo | `expandBounds` ON |
| Va a saltos entre frames | `frames` del componente distinto al del material | Igualar con el JSON |

---

## 7. Rendimiento esperado

- Cada asset animado: **1 draw call por material**, no por hueso.
- Sin Animator ni SkinnedMeshRenderer: se elimina el skinning de CPU.
- Instancing: N enemigos iguales comparten draw call y cada uno lleva su fase.
- Memoria de texturas: 26,5 MB comprimidos en disco.
- Los LOD1/LOD2 son estáticos (el VAT va en LOD0). Los prefabs del jugador
  (Santa, trineo y renos) **no llevan LODGroup**: siempre están en LOD0 para no
  perder el galope a la distancia normal de cámara. Los enemigos, naves y boss
  llevan LODGroup con umbrales 0,05 / 0,015 / 0,004; a esa distancia la pose de
  reposo no se nota. Si molesta, se hornea un VAT por LOD.

---

## 8. Cómo se regenera o se cambia algo

Todo es reproducible con los scripts de `BlenderSource/scripts/`:

| Quiero… | Script |
|---|---|
| Cambiar un modelo | `build_santa.py`, `build_reindeer.py`, `build_sleigh.py`, … |
| Cambiar una animación | editar el clip en `build_vat_assets.py` / `build_vat_reindeer.py` |
| Rehornear un asset | ejecutar el script de VAT correspondiente |
| Rehacer LODs y colisiones | `build_lods_collisions.py` |
| Regenerar las hojas de revisión | `render_previews.py`, `render_vat_previews.py` |

Se ejecutan dentro de Blender (con el servidor MCP activo). El detalle está en
`BlenderSource/README.md`.

---

## 9. Verificación hecha

- **Decodificación de la textura contra la malla animada: error máximo 0,5 mm**
  (precisión de half float). La animación que se ve en Blender es la que va a
  reproducir el shader.
- **FBX reimportado**: versión 7.4, dos capas UV (`UVMap` + `VAT`), materiales y
  escala correctos (Santa 0,89 × 0,55 × 1,93 m).
- **Hoja de contactos por animación** en `Previews/011…020`: la muerte del reno,
  la sacada de arma, el galope, el ataque del boss, etc., revisadas frame a frame.
- **Herramientas compiladas y verificadas en una copia aislada del proyecto**
  (Unity 6.6, modo batch): 146 archivos importados, **28 prefabs creados y 0
  omitidos**. Se renderizaron la escena de prueba y el rig integrado en `Game`
  para revisar escala, orientación, colores y animación.
- **Corregido el atlas UV multi-clip**: el UV1 se hornea con el clip más largo
  del asset y cada textura tiene su propia altura. El shader usa `_UVFrames`
  para reescalar la fila base; sin eso, las patas, las astas y el arnés del reno
  caían dentro del cuerpo.
- **En el proyecto principal queda ejecutar el menú** `Clous-End > VAT >
  Importar todo` una vez; el editor estaba abierto durante la integración y por
  eso la ejecución se hizo sobre una copia.

---

## 10. Lo que falta (y no bloquea la importación)

1. **Horneado de Albedo/Normal/ORM.** Ahora el FBX lleva un color plano por
   material: la silueta y la animación son finales, pero el detalle procedural de
   Blender no viaja. Es lo que más se nota al importar.
2. **Clips que faltan:** enganchado por el Garfio, salto, victoria/derrota, y las
   animaciones de las armas en mano. El rig y el pipeline ya están; es añadir
   clips y rehornear.
3. **LODs animados** si se quiere VAT también en LOD1/LOD2.
4. **Revisión visual y ajuste de offsets**: se hace en `VAT_Preview` y en la
   escena `Game` (altura del trineo y hueco de Santa). El rig del jugador está
   montado con offsets serializados en `VATRigBuilder`, fáciles de retocar.

---

## 11. Decisiones de esta fase

- **Trineo a 4,6 m** (el documento pedía 6,5 m). Frente a un reno de 1,33 m a la
  cruz, 6,5 m se leía como un barco. Se puede volver atrás cambiando
  `SLEIGH_SCALE` y rehorneando.
- **Manto y Torre** son los dos robots de cuatro brazos: se rediseñaron así para
  no inventar enemigos fuera de alcance.
- **Meteoritos** entran como peligro ambiental, no como enemigo.
- **Naves:** Caza y Recolector. La Aguja/Ojo de fondo queda pendiente.
- **3 aliens** = Sabueso, Garfio y Espora.

---

## 12. Resumen para el equipo

> Los modelos y las animaciones están terminados y verificados. El paquete de
> `BlenderSource/VAT/` entra a Unity con **`Clous-End > VAT > Importar todo`**:
> copia FBX, EXR y JSON, aplica los ajustes, crea un material `ClousEnd/VAT_Unlit`
> por clip y arma los prefabs con `VATPlayer`, LODs y collider. No hace falta
> esqueleto ni Animator. Después se revisa en `Assets/Scenes/VAT_Preview.unity`
> y se integra en `Game` desde el menú. El paso pendiente antes de que se vea
> como en Blender es hornear las texturas de albedo.
