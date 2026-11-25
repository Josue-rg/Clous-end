"""
CLOUS-END · Naves Yermas (v1)

Nave Caza (15k) y Nave Recolector (25k).
Obsidiana, aristas, vetas de luz, sin reflejos calidos.
1 unidad = 1 m. Morro hacia -Y. Sin rig.
"""

import bpy
import math
import os

LIB_DIR = globals().get("__file__")
LIB_DIR = (os.path.dirname(os.path.abspath(LIB_DIR)) if LIB_DIR
           else r"C:\Users\Josue\Desktop\Clous-end\BlenderSource\scripts")
LIB = os.path.join(LIB_DIR, "clous_lib.py")
g = {"__name__": "clous_lib"}
exec(compile(open(LIB, encoding="utf-8").read(), LIB, "exec"), g)

reset_scene = g["reset_scene"]
collection = g["collection"]
lathe = g["lathe"]
tube = g["tube"]
tube_smooth = g["tube_smooth"]
ellipsoid = g["ellipsoid"]
box = g["box"]
cone = g["cone"]
cylinder = g["cylinder"]
loft = g["loft"]
ring_torus = g["ring_torus"]
material = g["material"]
set_material = g["set_material"]
subsurf = g["subsurf"]
bevel = g["bevel"]
apply_all = g["apply_all"]
smooth_join = g["smooth_join"]
studio = g["studio"]
save = g["save"]
report = g["report"]
D2R = g["D2R"]
TAU = g["TAU"]

ROOT = r"C:\Users\Josue\Desktop\Clous-end\BlenderSource"


def mats():
    M = {}
    M["obs"] = material("M_Ship_Obsidian", (0.012, 0.012, 0.016, 1), rough=0.13,
                        metal=0.55, coat=0.8,
                        noise=dict(scale=7, detail=7, bump=0.25,
                                   color1=(0.005, 0.005, 0.008, 1),
                                   color2=(0.030, 0.030, 0.040, 1)))
    M["plate"] = material("M_Ship_Plate", (0.045, 0.046, 0.055, 1), rough=0.35,
                          metal=0.7)
    M["vein"] = material("M_Ship_Vein", (0.80, 0.92, 1.0, 1), rough=0.2,
                         emission=(0.70, 0.88, 1.0, 1), emission_strength=7.0)
    M["core"] = material("M_Ship_Core", (1.0, 1.0, 1.0, 1), rough=0.15,
                         emission=(0.85, 0.94, 1.0, 1), emission_strength=9.0)
    M["glass"] = material("M_Ship_Glass", (0.55, 0.68, 0.75, 1), rough=0.08,
                          alpha=0.32, spec=1.0)
    return M


def wedge(name, coll, sections, smooth=False):
    obj = loft(name, sections, coll=coll, close=True, cap_start=True,
               cap_end=True, smooth=smooth)
    bevel(obj, 0.01, 2, 25)
    return obj


# ---------------------------------------------------------------------------
# CAZA
# ---------------------------------------------------------------------------

def build_caza(M, coll, x):
    parts = []

    def add(o, m):
        set_material(o, m)
        parts.append(o)
        return o

    # fuselaje: prisma alargado de morro afilado
    sections = [
        [(-0.35, -3.2, 0.05), (0.35, -3.2, 0.05),
         (0.0, -3.2, 0.55)],
        [(-0.75, -2.2, -0.10), (0.75, -2.2, -0.10),
         (0.0, -2.2, 0.90)],
        [(-1.05, -0.8, -0.20), (1.05, -0.8, -0.20),
         (0.0, -0.8, 1.10)],
        [(-1.10, 0.6, -0.18), (1.10, 0.6, -0.18),
         (0.0, 0.6, 1.05)],
        [(-0.90, 1.8, -0.10), (0.90, 1.8, -0.10),
         (0.0, 1.8, 0.85)],
        [(-0.55, 2.8, 0.02), (0.55, 2.8, 0.02),
         (0.0, 2.8, 0.55)],
    ]
    add(wedge("SM_Caza_Hull", coll, sections), M["obs"])

    # alas en flecha invertida
    for sx in (-1, 1):
        wing = loft("SM_Caza_Wing",
                    [[(sx * 0.9, -1.0, 0.30), (sx * 1.0, -0.6, 0.05),
                      (sx * 1.0, 0.9, 0.05), (sx * 0.9, 1.2, 0.30)],
                     [(sx * 2.3, -1.8, 0.42), (sx * 2.6, -1.4, 0.16),
                      (sx * 2.5, 0.2, 0.16), (sx * 2.2, 0.5, 0.42)],
                     [(sx * 3.1, -2.3, 0.46), (sx * 3.4, -1.9, 0.24),
                      (sx * 3.3, -0.7, 0.24), (sx * 3.0, -0.4, 0.46)]],
                    coll=coll, close=True, cap_start=True, cap_end=True)
        add(wing, M["obs"])
        edge = box("SM_Caza_WingEdge", (0.06, 2.0, 0.05),
                   location=(sx * 2.55, -1.05, 0.42), rot=(0, D2R(sx * 4), 0),
                   bevel=0.01, smooth=False)
        add(edge, M["vein"])

    # nucleo
    add(ellipsoid("SM_Caza_Core", (0.42, 0.75, 0.30),
                  location=(0, 0.4, 0.72), coll=coll, segments=16, rings=10),
        M["core"])
    add(box("SM_Caza_CoreGlass", (0.72, 1.05, 0.10),
            location=(0, 0.4, 1.02), bevel=0.02, smooth=False), M["glass"])

    # espinas dorsales
    for i, y in enumerate((-1.6, -0.6, 0.4, 1.4)):
        h = 0.35 - abs(y) * 0.06
        sp = cone("SM_Caza_Spine_%d" % i, r1=0.06, r2=0.005, depth=h,
                  location=(0, y, 1.02 + h * 0.4),
                  rot=(D2R(-14), 0, 0), segments=4, smooth=False)
        add(sp, M["plate"])

    # motores
    for sx in (-1, 1):
        add(cylinder("SM_Caza_Engine_%d" % sx, radius=0.30, depth=1.1,
                     location=(sx * 0.55, 2.4, 0.35),
                     rot=(D2R(90), 0, 0), segments=10), M["plate"])
        add(cylinder("SM_Caza_Thrust_%d" % sx, radius=0.24, depth=0.08,
                     location=(sx * 0.55, 2.98, 0.35),
                     rot=(D2R(90), 0, 0), segments=10), M["core"])

    # morro luminoso
    add(cone("SM_Caza_Beak", r1=0.22, r2=0.02, depth=0.9,
             location=(0, -3.5, 0.22), rot=(D2R(-90), 0, 0), segments=4,
             smooth=False), M["plate"])
    add(tube("SM_Caza_Maw", [(0, -3.1, 0.30), (0, -3.9, 0.22)],
             radius=[0.10, 0.02], segments=6), M["vein"])
    return parts


# ---------------------------------------------------------------------------
# RECOLECTOR
# ---------------------------------------------------------------------------

def build_recolector(M, coll, x):
    parts = []

    def add(o, m):
        set_material(o, m)
        parts.append(o)
        return o

    sections = [
        [(-0.6, -6.0, -0.2), (0.6, -6.0, -0.2), (0.0, -6.0, 0.7)],
        [(-1.6, -4.4, -0.6), (1.6, -4.4, -0.6), (0.0, -4.4, 1.5)],
        [(-2.3, -2.2, -0.9), (2.3, -2.2, -0.9), (0.0, -2.2, 2.0)],
        [(-2.5, 0.4, -0.9), (2.5, 0.4, -0.9), (0.0, 0.4, 2.1)],
        [(-2.2, 2.8, -0.7), (2.2, 2.8, -0.7), (0.0, 2.8, 1.8)],
        [(-1.5, 5.0, -0.4), (1.5, 5.0, -0.4), (0.0, 5.0, 1.2)],
        [(-0.7, 6.2, -0.1), (0.7, 6.2, -0.1), (0.0, 6.2, 0.6)],
    ]
    add(wedge("SM_Recolector_Hull", coll, sections), M["obs"])

    # anillos estructurales
    for i, y in enumerate((-4.0, -1.8, 0.6, 2.9, 4.8)):
        hw = 1.5 + (1.0 - abs(y) / 6.2) * 1.0
        top = 1.2 + (1.0 - abs(y) / 6.2) * 0.9
        ring = loft("SM_Recolector_Ring_%d" % i,
                    [[(-hw, y - 0.10, -0.7), (hw, y - 0.10, -0.7),
                      (0.0, y - 0.10, top)],
                     [(-hw, y + 0.10, -0.7), (hw, y + 0.10, -0.7),
                      (0.0, y + 0.10, top)]],
                    coll=coll, close=True, cap_start=True, cap_end=True,
                    smooth=False)
        add(ring, M["plate"])

    # columna luminosa
    add(box("SM_Recolector_Spine", (0.16, 10.0, 0.10),
            location=(0, 0.2, 2.02), bevel=0.02, smooth=False), M["vein"])

    # nucleo
    add(ellipsoid("SM_Recolector_Core", (0.8, 1.1, 0.55),
                  location=(0, 0.4, 1.55), coll=coll, segments=18, rings=12),
        M["core"])
    add(box("SM_Recolector_CoreGlass", (1.5, 2.0, 0.16),
            location=(0, 0.4, 2.12), bevel=0.03, smooth=False), M["glass"])

    # brazos-gancho colgantes
    for sx in (-1, 1):
        for i, y in enumerate((-2.6, 0.0, 2.6)):
            arm = tube_smooth("SM_Recolector_Arm_%d_%d" % (sx, i),
                              [(sx * 1.9, y, 0.6), (sx * 2.4, y - 0.3, -0.4),
                               (sx * 2.1, y - 0.8, -1.5),
                               (sx * 2.3, y - 0.6, -2.4)],
                              [0.16, 0.13, 0.10, 0.07],
                              [0.16, 0.13, 0.10, 0.07],
                              sub=4, segments=8, coll=coll)
            add(arm, M["plate"])
            for k in range(3):
                a = D2R(k * 120)
                hook = tube("SM_Recolector_Hook_%d_%d_%d" % (sx, i, k),
                            [(sx * 2.3, y - 0.6, -2.42),
                             (sx * 2.3 + math.cos(a) * 0.16, y - 0.6 + math.sin(a) * 0.10, -2.62),
                             (sx * 2.3 + math.cos(a) * 0.20, y - 0.6 + math.sin(a) * 0.06, -2.82)],
                            radius=[0.05, 0.03, 0.008], segments=6,
                            coll=coll)
                add(hook, M["vein"])

    # jaulas
    for i, y in enumerate((-3.2, 3.2)):
        cage = loft("SM_Recolector_Cage_%d" % i,
                    [[(-0.7, y - 0.7, 0.6), (0.7, y - 0.7, 0.6),
                      (0.7, y - 0.7, 2.4), (-0.7, y - 0.7, 2.4)],
                     [(-0.5, y + 0.7, 0.8), (0.5, y + 0.7, 0.8),
                      (0.5, y + 0.7, 2.2), (-0.5, y + 0.7, 2.2)]],
                    coll=coll, close=True, cap_start=False, cap_end=False,
                    smooth=False)
        add(cage, M["glass"])
        add(box("SM_Recolector_CageBar_%d" % i, (0.08, 0.08, 1.9),
                location=(0, y, 1.5), bevel=0.01, smooth=False), M["vein"])

    # motores
    for sx in (-1, 1):
        for sy in (-0.3, 0.9):
            add(cylinder("SM_Recolector_Engine",
                         radius=0.55, depth=1.6,
                         location=(sx * 1.9, 5.6 + sy, 0.3),
                         rot=(D2R(90), 0, 0), segments=12), M["plate"])
            add(cylinder("SM_Recolector_Thrust", radius=0.45, depth=0.10,
                         location=(sx * 1.9, 6.42 + sy, 0.3),
                         rot=(D2R(90), 0, 0), segments=12), M["core"])
    return parts


def finish(name, parts, coll, x):
    for p in parts:
        apply_all(p)
    joined = smooth_join(parts, name)
    root = bpy.data.objects.new(name, None)
    root.empty_display_type = 'PLAIN_AXES'
    root.empty_display_size = 1.0
    coll.objects.link(root)
    joined.parent = root
    root.location = (x, 0, 6.0)
    return root


def build():
    scene = reset_scene()
    M = mats()
    coll = collection("SK_Ships")
    finish("SM_Ship_Caza", build_caza(M, coll, -9.0), coll, -9.0)
    finish("SM_Ship_Recolector", build_recolector(M, coll, 9.0), coll, 9.0)

    report([o for o in bpy.data.objects if o.type == 'MESH'])
    studio()
    scene.render.engine = 'BLENDER_EEVEE'
    save(os.path.join(ROOT, "Ships", "SK_Ships.blend"))


build()
