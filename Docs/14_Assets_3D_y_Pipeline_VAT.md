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

**Nada se ha importado todavía a `Assets/`.** El paquete está preparado y
verificado; la importación es el paso siguiente y está explicada aquí abajo.

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

Peso total de las texturas de animación: **26,5 MB** (EXR 16 bits con ZIP).

---

## 5. Cómo hacerlo funcionar en Unity (paso a paso)

> Orden recomendado: primero **un** asset (Santa), comprobar que se mueve, y
> después repetir con el resto.

### Paso 1 · Copiar los archivos

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

### Paso 2 · Importar el FBX

- Scale Factor **1**, Convert Units **ON**, Bake Axis Conversion **OFF**.
- Import Cameras y Lights **OFF**.
- Normals **Import**. Tangents **None** (todavía no hay normal maps).
- Materials **Import**.

El FBX trae **dos capas UV**:
- **UV0** (`UVMap`): atlas, para el horneado de albedo futuro.
- **UV1** (`VAT`): el índice de vértice que usa el shader.

Unity las importa como `uv` y `uv2`. El shader lee `uv2`.

### Paso 3 · Importar la textura EXR

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

### Paso 4 · Crear el material

Material nuevo con el shader **ClousEnd/VAT_Unlit**:

| Propiedad | Valor |
|---|---|
| `_PosTex` | el EXR del paso 3 |
| `_Frames` | `frames` del JSON |
| `_BaseColor` | color del material (mira el FBX) |
| `_VATAmount` | 1 |
| `_FlipY` | 0 (ver problemas) |
| `_Ambient` | 0,45 |
| `_LightDir` | dirección de la luz principal del nivel |

Marca **GPU Instancing** en el material. Si el FBX trae varias submeshes,
asigna este material a todas y usa el mismo EXR.

> Un material por clip. Para el Santa harás cuatro: Idle, Draw, Shoot y Reload.

### Paso 5 · Añadir el componente

En el prefab, añade **VATPlayer**:

- `frames` y `fps`: los del JSON.
- `loop`: ON para Idle, Gallop, Hover, Flight, Run, Pulse. **OFF** para Draw,
  Shoot, Reload, Death, Hurt, Impact, Attack.
- `randomPhase`: ON en enemigos (cada uno va desfasado).
- `expandBounds`: ON, con `boundsCenter` / `boundsExtent` del JSON:
  - centro = (min + max) / 2
  - extensión = (max − min) / 2 + 0,1

Si no amplías los bounds, el modelo parpadea en el borde de la pantalla porque
Unity lo descarta con el frustum culling usando la pose de reposo.

### Paso 6 · Probar

Pon el prefab en la escena, dale Play. Si se mueve, repite con el resto.

---

## 6. Problemas típicos y su solución

| Síntoma | Causa | Solución |
|---|---|---|
| La malla tiembla o se ve "cuadriculada" | Textura con filtro Bilinear o comprimida | Point + Compression None |
| Está todo negro | `_BaseColor` en negro o falta `_PosTex` | Asignar color y textura |
| Se ve la pose de reposo, sin animación | `_VATAmount` = 0, falta VATPlayer o `frames` no coincide | Revisar los tres |
| Se mueve solo una parte | El FBX entró con una sola capa UV | Revisar import settings del mesh |
| La animación va al revés / tumbada | Volcado del EXR | `_FlipY` = 1 |
| Desaparece al acercarse al borde | Bounds en reposo | `expandBounds` ON |
| Va a saltos entre frames | `frames` del componente distinto al del material | Igualar con el JSON |

---

## 7. Rendimiento esperado

- Cada asset animado: **1 draw call por material**, no por hueso.
- Sin Animator ni SkinnedMeshRenderer: se elimina el skinning de CPU.
- Instancing: N enemigos iguales comparten draw call y cada uno lleva su fase.
- Memoria de texturas: 26,5 MB comprimidos en disco.
- Los LOD1/LOD2 son estáticos (el VAT va en LOD0). A distancia de LOD1 el modelo
  se queda en la pose de reposo: aceptable, pero si molesta se hornea un VAT por
  LOD.

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

---

## 10. Lo que falta (y no bloquea la importación)

1. **Horneado de Albedo/Normal/ORM.** Ahora el FBX lleva un color plano por
   material: la silueta y la animación son finales, pero el detalle procedural de
   Blender no viaja. Es lo que más se nota al importar.
2. **Clips que faltan:** enganchado por el Garfio, salto, victoria/derrota, y las
   animaciones de las armas en mano. El rig y el pipeline ya están; es añadir
   clips y rehornear.
3. **LODs animados** si se quiere VAT también en LOD1/LOD2.
4. **Import real y prueba** con la cámara y la luz del juego.

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
> `BlenderSource/VAT/` se importa a Unity copiando los FBX, los EXR y los
> shaders, creando un material `ClousEnd/VAT_Unlit` por clip y añadiendo
> `VATPlayer` al prefab. No hace falta esqueleto ni Animator. El paso pendiente
> antes de que se vea como en Blender es hornear las texturas de albedo.
