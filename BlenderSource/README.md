# BlenderSource · CLOUS-END

Origen de todos los assets 3D. **Fuera de `Assets/`** para que Unity no importe
los `.blend`.

1 unidad = 1 metro · Z arriba · los personajes miran hacia **-Y**.

---

## Dos carpetas, dos propósitos

```
Characters/ Vehicles/ Weapons/ Enemies/ Ships/ Boss/ Props/
    Modelos de revisión, alta densidad (LOD0 "de artista"). No se exportan.

VAT/
    Paquete de juego: malla optimizada + textura de animación + FBX.
    Esto es lo que se importa a Unity.

UnityPackage/
    Shaders y scripts C# que reproducen las animaciones en GPU.
```

Los modelos de revisión son los que se ven en `Previews/01..10`. Los de `VAT/`
son la versión que corre en el juego (misma silueta, materiales simplificados y
animación horneada).

---

## Pipeline de animación: VAT (Vertex Animation Textures)

En vez de exportar huesos, Blender hornea la posición de cada vértice en cada
frame dentro de una textura:

```
ancho  = vértices por fila (1024)
alto   = filas × frames
píxel  = (x, y, z) de la posición local
UV1    = coordenada de ese vértice en el frame 0
```

El shader suma `frame / frames` a la V y desplaza el vértice en la GPU.
Resultado en Unity: **sin Animator, sin SkinnedMeshRenderer, sin skinning en
CPU**, y con GPU instancing N enemigos son 1 draw call.

| Archivo | Qué es |
|---|---|
| `VAT/<Asset>/SK_<Asset>.fbx` | malla estática con UV0 (atlas) + UV1 (VAT) |
| `VAT/<Asset>/T_<Asset>_<Clip>_Pos.exr` | textura de posiciones (EXR 16 bits + ZIP) |
| `VAT/<Asset>/vat_<Asset>.json` | frames, fps, tamaño de textura, bounds |
| `VAT/<Asset>/SK_<Asset>_vat.blend` | rig + clip para re-hornear |
| `VAT/Static/<Prop>/` | props sin animación (solo FBX + JSON) |

### Assets animados

| Asset | Clip | Frames | FPS | Textura |
|---|---|---|---|---|
| Santa | Idle | 36 | 30 | 1024×1080 |
| Reno ×4 | Gallop | 24 | 30 | 1024×456 |
| Trineo | Flight | 36 | 30 | 1024×612 |
| Sabueso | Run | 20 | 30 | 1024×40 |
| Garfio | Hover | 36 | 30 | 1024×~90 |
| Espora | Pulse | 24 | 30 | 1024×~40 |
| Robot Manto / Torre | Idle | 36 | 30 | 1024×~70 |
| Nicolás Cero | Idle | 48 | 30 | 1024×240 |
| Nave Caza / Recolector | Hover | 40 | 30 | 1024×40 / 80 |

### Props estáticos exportados

Armas (Carbón, Nogal, Estrella, Cascabel), caja de armas, munición, kit de casco,
escudo de reno, collar, cascabeles, 3 meteoritos y el set de arnés.

---

## Cómo se regenera todo

Librería común: `scripts/clous_lib.py` (modelado y materiales) y
`scripts/vat_rig_lib.py` (esqueleto, skin por proximidad, clips, horneado VAT,
export FBX).

| Script | Genera |
|---|---|
| `build_santa.py` | `Characters/SK_Santa.blend` |
| `build_reindeer.py` | `Characters/SK_Reindeer.blend` (4 renos) |
| `build_sleigh.py` | `Vehicles/SM_Sleigh.blend` |
| `build_weapons.py` | `Weapons/SM_Weapons.blend` |
| `build_yermos.py` | `Enemies/SK_Yermos.blend` |
| `build_ships.py` | `Ships/SK_Ships.blend` |
| `build_boss.py` | `Boss/SK_NicolasCero.blend` |
| `build_props.py` | `Props/SM_Props.blend` |
| `build_harness.py` | `Props/SM_Harness_Set.blend` |
| `build_vat_reindeer.py` | `VAT/Reindeer_*/` |
| `build_vat_assets.py` | `VAT/<Asset>/` (resto) |
| `build_static_exports.py` | `VAT/Static/` |
| `render_previews.py` | hojas de los modelos |
| `render_vat_previews.py` | hojas de las animaciones |

Se ejecutan dentro de Blender:

```python
import bpy
P = r"...\scripts\build_vat_assets.py"
g = {"__name__": "build_vat_assets"}
exec(compile(open(P, encoding="utf-8").read(), P, "exec"), g)
```

---

## Verificación hecha

- Decodificación VAT contra la malla animada: error máximo **0,5 mm**.
- Reimportación del FBX: FBX 7.4, 2 capas UV (`UVMap` + `VAT`), 13 materiales,
  dimensiones correctas (0,89 × 0,55 × 1,93 m para Santa).
- Los 4 renos comparten construcción y rig; cambian proporciones, astas,
  materiales y arnés.

---

## Pendiente

- [ ] Rig y animaciones secundarias: disparo, recarga, impacto, mirar atrás,
      victoria/derrota (Santa); salto, herido, enganchado, muerte (renos).
- [ ] UVs definitivas + horneado de Albedo/Normal/ORM. Ahora los materiales son
      procedurales: el FBX exporta color plano por material.
- [ ] LOD1/LOD2 y mallas `_COL`.
- [ ] Importar el paquete a Unity y probar con la cámara y la luz reales.
- [ ] Renos de obsidiana del boss (variante de material del reno base).
