"""
CLOUS-END · SM_Harness_Set (v1)

Balancín, riendas y cuerdas que unen el trineo con la yunta de renos.
1 unidad = 1 m. Sin rig (se riggea con huesos/rieles despues).
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
sweep_rect = g["sweep_rect"]
ellipsoid = g["ellipsoid"]
box = g["box"]
cylinder = g["cylinder"]
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


def build():
    scene = reset_scene()
    coll = collection("SM_Harness_Set")
    parts = []

    wood = material("M_Harness_Wood", (0.28, 0.175, 0.092, 1), rough=0.55,
                    noise=dict(scale=20, detail=8, bump=0.4, mapping=(2, 3, 16),
                               color1=(0.20, 0.120, 0.060, 1),
                               color2=(0.36, 0.235, 0.130, 1)))
    iron = material("M_Harness_Iron", (0.10, 0.095, 0.092, 1), rough=0.48,
                    metal=1.0)
    brass = material("M_Harness_Brass", (0.58, 0.42, 0.17, 1), rough=0.30,
                     metal=1.0)
    ropem = material("M_Harness_Rope", (0.33, 0.26, 0.17, 1), rough=0.90,
                     noise=dict(scale=140, detail=6, bump=0.45,
                                color1=(0.24, 0.185, 0.115, 1),
                                color2=(0.42, 0.335, 0.225, 1)))
    leather = material("M_Harness_Leather2", (0.155, 0.095, 0.055, 1), rough=0.58)

    def add(o, m, name=None):
        if name:
            o.name = name
            o.data.name = name
        set_material(o, m)
        parts.append(o)
        return o

    # ------------------------------------------------------------------ balancin
    bar = sweep_rect("SM_Harness_Yoke",
                     [(-1.15, 0.30, 1.02), (-0.60, 0.26, 1.05),
                      (0.0, 0.24, 1.06), (0.60, 0.26, 1.05),
                      (1.15, 0.30, 1.02)],
                     width=0.085, height=0.11, coll=coll, side_axis=(0, 1, 0))
    subsurf(bar, 1)
    add(bar, wood)
    for sx in (-1, 1):
        add(ellipsoid("SM_Harness_YokeCap", (0.055, 0.055, 0.055),
                      location=(sx * 1.15, 0.30, 1.02), coll=coll,
                      segments=14, rings=8), brass)

    # puntos de enganche
    for sx in (-1, 1):
        for i, yy in enumerate((0.28, 0.40)):
            add(ring_torus("SM_Harness_Ring_%d_%d" % (sx, i),
                           major=0.045, minor=0.012,
                           location=(sx * (0.42 + i * 0.30), yy, 1.00),
                           coll=coll, rot=(0, D2R(90), 0),
                           major_seg=16, minor_seg=6), iron)

    # ------------------------------------------------------------------ riendas
    reins = []
    for sx in (-1, 1):
        for i in range(2):
            x0 = sx * (0.42 + i * 0.30)
            path = [(x0, 0.36, 1.01),
                    (x0 * 1.10, -0.60, 1.02),
                    (x0 * 1.20, -1.70, 1.08),
                    (x0 * 1.25, -2.60, 1.14)]
            reins.append(tube_smooth("SM_Harness_Rein_%d_%d" % (sx, i),
                                     path,
                                     [0.020, 0.019, 0.018, 0.016],
                                     [0.020, 0.019, 0.018, 0.016],
                                     sub=4, segments=6, coll=coll))
    for r in reins:
        apply_all(r)
    add(smooth_join(reins, "SM_Harness_Reins"), ropem)

    # cuerdas de tiro (gruesas)
    ropes = []
    for sx in (-1, 1):
        path = [(sx * 0.30, 0.20, 0.98), (sx * 0.32, -0.70, 0.96),
                (sx * 0.34, -1.60, 0.94), (sx * 0.34, -2.50, 0.92)]
        ropes.append(tube_smooth("SM_Harness_Tow_%d" % sx, path,
                                 [0.034, 0.032, 0.030, 0.028],
                                 [0.034, 0.032, 0.030, 0.028],
                                 sub=4, segments=8, coll=coll))
    for r in ropes:
        apply_all(r)
    add(smooth_join(ropes, "SM_Harness_Tow"), ropem)

    # ------------------------------------------------------------------ bozal
    for sx in (-1, 1):
        add(ring_torus("SM_Harness_Noseband_%d" % sx, major=0.085, minor=0.016,
                       location=(sx * 0.38, -2.30, 1.28), coll=coll,
                       rot=(D2R(70), 0, 0), major_seg=18, minor_seg=6), leather)

    # ------------------------------------------------------------------ cintas
    for sx in (-1, 1):
        strap = box("SM_Harness_Strap_%d" % sx, (0.030, 1.30, 0.055),
                    location=(sx * 0.52, -0.75, 1.06), rot=(D2R(-2), 0, 0),
                    bevel=0.006, smooth=True)
        add(strap, leather)

    for p in parts:
        apply_all(p)

    root = bpy.data.objects.new("SM_Harness_Set", None)
    root.empty_display_type = 'PLAIN_AXES'
    root.empty_display_size = 0.5
    coll.objects.link(root)
    for p in parts:
        p.data.name = p.name
        p.parent = root

    print("PARTES:", len(parts))
    report(parts)
    studio()
    scene.render.engine = 'BLENDER_EEVEE'
    save(os.path.join(ROOT, "Props", "SM_Harness_Set.blend"))


build()
