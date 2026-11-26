# CLOUS-END · Paquete VAT para Unity

> **En el proyecto CLOUS-END no se importa a mano.** Usa
> `Clous-End > VAT > Importar todo (paquete + prefabs)`. El importador copia
> estos archivos, aplica los ajustes y genera los prefabs. Este README queda como
> referencia del paquete; la guía del proyecto está en `Docs/14`.

Animación por **Vertex Animation Textures**: Blender hornea la pose de cada
vértice en una textura EXR y un shader la aplica en la GPU. En Unity **no hay
huesos, ni SkinRenderer, ni Animator**: la malla es estática y se deforma en el
vertex shader. Pensado para WebGL y para muchos enemigos a la vez.

```
UnityPackage/
  VAT_Unlit.shader     shader principal (lee la textura de posiciones)
  FloatWobble.shader   alternativa matematica (sin texturas) para flotantes
  VATPlayer.cs         reproduce el clip y da fase propia a cada instancia
  FloatPhase.cs        fase aleatoria para FloatWobble
```

Los FBX + EXR + JSON de cada asset están en
`BlenderSource/VAT/<Asset>/`.

---

## 1. Importar el FBX

Arrastra `SK_<Asset>.fbx` a `Assets/Art/Models/VAT/<Asset>/`.

- **Scale Factor:** 1
- **Convert Units:** ON
- **Bake Axis Conversion:** OFF (la rotación de raíz del FBX se conserva en el prefab)
- **Import Cameras / Lights:** OFF
- **Normals:** Import
- **Tangents:** None (no usamos normal maps todavía)
- **Materials:** Import (o None si vas a crear uno propio)

El mesh trae **dos capas UV**: UV0 (atlas, para albedo futuro) y UV1 (índice de
vértice para el VAT). Unity las importa como `uv` y `uv2`. El shader usa `uv2`.

> Si tu malla llegara con una sola capa UV, en el shader cambia `v.uv2` por
> `v.uv` y no uses `_BaseMap`.

---

## 2. Importar la textura EXR

`T_<Asset>_<Clip>_Pos.exr`:

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

Estos ajustes son obligatorios: con filtrado bilineal o compresión se mezclan
véctores de vértices distintos y la malla explota.

---

## 3. Material

Crea un material con **ClousEnd/VAT_Unlit**:

| Propiedad | Valor |
|---|---|
| `_PosTex` | la textura EXR del paso 2 |
| `_Frames` | el campo `frames` del JSON |
| `_UVFrames` | el mayor `frames` de todos los clips del asset (0 si todos coinciden) |
| `_BaseColor` | color del asset |
| `_Ambient` | 0.4–0.6 |
| `_LightDir` | dirección de la luz principal del nivel |
| `_VATAmount` | 1 |
| `_FlipY` | 0 (ver "Problemas") |

Si el FBX tiene varios materiales (submallas), asigna este material a todas y
usa el mismo EXR en cada una. Todas deben tener **GPU Instancing activado**
(el script lo activa solo, pero conviene marcarlo en el asset).

---

## 4. Componente

En el prefab, añade **VATPlayer** (espacio de nombres `ClousEnd.Rendering`):

- `frames` = `frames` del JSON
- `fps` = `fps` del JSON
- `randomPhase` = ON para enemigos (cada uno va desfasado)
- `expandBounds` = ON, con `boundsCenter` y `boundsExtent` del JSON
  (`bounds_min`/`bounds_max`: centro = (min+max)/2, extensión = (max-min)/2 + 0.1).
  Ojo: el JSON está en los ejes de la malla (Blender, Z arriba). El prefab debe
  conservar la rotación de raíz del FBX (90° en X) y los bounds van tal cual, en
  espacio local. El importador del proyecto lo hace solo.

Sin `expandBounds` el modelo parpadea al acercarse al borde de la pantalla,
porque Unity lo descarta con el frustum culling usando los bounds de la malla
en reposo.

---

## 5. Assets incluidos

| Asset | Frames | FPS | Textura | Tamaño |
|---|---|---|---|---|
| Santa | 36 | 30 | 1024×1080 | 8,4 MB |
| Reno (×4) | 24 | 30 | 1024×456 | 1,9 MB c/u |
| Trineo | 36 | 30 | 1024×612 | 4,8 MB |
| Sabueso | 20 | 30 | 1024×40 | 0,3 MB |
| Garfio | 36 | 30 | 1024×~90 | 0,7 MB |
| Espora | 24 | 30 | 1024×~40 | 0,3 MB |
| Robot Manto / Torre | 36 | 30 | 1024×~70 | 0,5 MB |
| Nicolás Cero | 48 | 30 | 1024×240 | 1,9 MB |
| Nave Caza | 40 | 30 | 1024×40 | 0,3 MB |
| Nave Recolector | 40 | 30 | 1024×80 | 0,6 MB |

Los valores exactos están en `vat_<Asset>.json` de cada carpeta.

---

## 6. Problemas típicos

**El modelo aparece tumbado o con la animación al revés**
Pon `_FlipY` = 1. Es el único ajuste que depende de cómo Unity haya volcado el
EXR; con una prueba lo dejas fijo.

**La malla tiembla o se ve "cuadriculada"**
La textura se importó con filtrado Bilinear o con compresión. Vuelve a Point +
None. El EXR es half float: precisión de 0,1 mm.

**La malla aparece negra**
`_BaseColor` está a negro, o el material no tiene `_PosTex` asignado (si es null
el shader usa una textura negra y todos los vértices van al origen).

**Solo se mueve una parte**
El FBX se importó sin la segunda capa UV. Revisa el import settings del mesh.

**Se ve la pose de reposo**
`_VATAmount` = 0, o `VATPlayer` no está en el objeto, o `frames` no coincide
con `_Frames` del material.

---

## 7. Rendimiento en WebGL

- Cada asset animado es **1 draw call por material** (no por hueso). Con GPU
  instancing, N enemigos iguales son 1 draw call por submesh.
- No hay Animator ni SkinnedMeshRenderer: se elimina el coste de CPU de skinning.
- Memoria: las texturas VAT suman ~25 MB. Si necesitas bajarlo:
  - reduce `frames` (a 16–20 para loops lentos),
  - o cambia el horneado a 8 bits en `vat_rig_lib.bake_vat` (la mitad de memoria,
    precisión ~4 mm).

---

## 8. Añadir más clips

Cada asset tiene su `.blend` con el rig y un clip. Para añadir otro (por ejemplo
`Shoot` para Santa):

1. Abre `SK_Santa_vat.blend`.
2. Duplica la acción, renómbrala `AN_Santa_Shoot`.
3. Vuelve a ejecutar `bake_vat` con el nuevo rango de frames y otro nombre de
   EXR (el script `build_vat_assets.py` es la referencia).
4. En Unity, asigna el nuevo EXR y `frames`, y cambia de material/clip con
   `VATPlayer.SetClip(...)`.

Para **fundir dos clips** (idle → shoot) hace falta un shader con dos texturas y
un `_Blend`; no está incluido porque para v1 basta con el corte seco.
