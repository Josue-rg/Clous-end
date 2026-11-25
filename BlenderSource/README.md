# BlenderSource · CLOUS-END

Origen de todos los assets 3D del juego. **Esta carpeta queda fuera de `Assets/`**
para que Unity no importe los `.blend` (ver `Docs/11_Alcance_Final_Juego_Terminado.md`).

---

## Estado

Fase actual: **modelos v1** (sin rig ni animaciones).
Semirrealista cinematográfico · 1 unidad de Blender = 1 metro · Z arriba · los
personajes miran hacia **-Y**.

| Asset | Archivo | Triángulos | Estado |
|---|---|---|---|
| Santa | `Characters/SK_Santa.blend` | ~92 k | Modelo v1 |
| 4 renos (Rodolfo, Donner, Vixen, Cometa) | `Characters/SK_Reindeer.blend` | ~150 k (4) | Modelo v1 |
| Trineo | `Vehicles/SM_Sleigh.blend` | ~33 k | Modelo v1 |
| Armas Carbón, Nogal, Estrella, Cascabel | `Weapons/SM_Weapons.blend` | ~12 k (4) | Modelo v1 |
| Yermos: Sabueso, Garfio, Espora | `Enemies/SK_Yermos.blend` | ~6 k (3) | Modelo v1 |
| Robots Manto y Torre (4 brazos) | `Enemies/SK_Yermos.blend` | ~3 k (2) | Modelo v1 |
| Naves Caza y Recolector | `Ships/SK_Ships.blend` | ~4 k (2) | Modelo v1 |
| Nicolás Cero (boss) | `Boss/SK_NicolasCero.blend` | ~9 k | Modelo v1 |
| Cajas, pickups, meteoritos, cascabeles | `Props/SM_Props.blend` | ~11 k | Modelo v1 |
| Arnés y riendas | `Props/SM_Harness_Set.blend` | ~3 k | Modelo v1 |

Pendiente de esta fase:

- [ ] Rig de Santa (humanoid + huesos de abrigo/barba).
- [ ] Rig del reno base y reutilización en las 4 variantes.
- [ ] Rig de suspensión del trineo.
- [ ] Rig de Nicolás Cero y de los renos de obsidiana.
- [ ] Todas las animaciones de `Docs/06` y `Docs/11`.
- [ ] LOD0/1/2 y mallas `_COL`.
- [ ] UVs definitivas y horneado de texturas PBR (ahora los materiales son
      procedurales de Blender; hay que hornear Albedo/Normal/ORM antes de Unity).
- [ ] Export FBX 7.4 con *Forward -Z, Up Y* y prueba en una escena limpia.
- [ ] Renos de obsidiana (variante de material + astas del reno base).

---

## Cómo se genera todo

Cada asset es un script reproducible. La librería común está en
`scripts/clous_lib.py` (materiales, lathe, loft, tubos suavizados, booleanas de
apariencia, modificadores, estudio de render).

En Blender, con el servidor MCP activo, se ejecuta por ejemplo:

```python
import bpy
P = r"C:\Users\Josue\Desktop\Clous-end\BlenderSource\scripts\build_santa.py"
g = {"__name__": "build_santa"}
exec(compile(open(P, encoding="utf-8").read(), P, "exec"), g)
```

| Script | Genera |
|---|---|
| `build_santa.py` | `Characters/SK_Santa.blend` |
| `build_reindeer.py` | `Characters/SK_Reindeer.blend` (4 variantes) |
| `build_sleigh.py` | `Vehicles/SM_Sleigh.blend` |
| `build_weapons.py` | `Weapons/SM_Weapons.blend` |
| `build_yermos.py` | `Enemies/SK_Yermos.blend` |
| `build_ships.py` | `Ships/SK_Ships.blend` |
| `build_boss.py` | `Boss/SK_NicolasCero.blend` |
| `build_props.py` | `Props/SM_Props.blend` |
| `build_harness.py` | `Props/SM_Harness_Set.blend` |

Cada script borra la escena, construye el asset, imprime el recuento de
triángulos y guarda el `.blend` con un set de luces de estudio en la colección
`PREVIEW` (no exportar esa colección).

---

## Convenciones

- Prefijos: `SM_` malla estática, `SK_` malla con esqueleto, `M_` material,
  `T_` textura, `AN_` animación, `_LOD0/1/2`, `_COL`.
- Un objeto raíz vacío por asset (`SK_Santa`, `SM_Sleigh`, …) con las piezas
  como hijas, para mover el conjunto sin romper nada.
- Los materiales son procedurales y se llaman `M_*`; en la fase de export se
  hornean a texturas.
- Escala real: Santa 1,96 m con gorro; reno 1,33 m a la cruz y 2,2 m con astas;
  trineo 5,9 × 2,16 m; armas 0,9–1,6 m.

## Siguiente paso

Cuando los modelos estén aprobados:

1. Rig + skinning por asset.
2. Animaciones de `Docs/06` (Santa, renos, trineo) y `Docs/11` (enemigos, boss).
3. UVs, horneado PBR y LODs.
4. Export FBX a `Assets/Art/Models/...` y prueba en la escena `Game`.
