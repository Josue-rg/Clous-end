"""
CLOUS-END · Yermos (v1)

Sabueso · Garfio · Espora  (enemigos comunes)
Manto y Torre rediseñados como robots de cuatro brazos (élites).
Lenguaje: obsidiana, aristas, vetas de luz blanca, extremidades de más.
1 unidad = 1 m. Sin rig.
"""

import bpy
import math
import os
import random

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
ring_torus = g["ring_torus"]
loft = g["loft"]
star = g["star"]
material = g["material"]
set_material = g["set_material"]
subsurf = g["subsurf"]
bevel = g["bevel"]
solidify = g["solidify"]
apply_all = g["apply_all"]
deform = g["deform"]
smooth_join = g["smooth_join"]
studio = g["studio"]
save = g["save"]
report = g["report"]
D2R = g["D2R"]
TAU = g["TAU"]

ROOT = r"C:\Users\Josue\Desktop\Clous-end\BlenderSource"


def materials():
    M = {}
    M["obs"] = material("M_Yermo_Obsidian", (0.014, 0.014, 0.018, 1), rough=0.15,
                        metal=0.4, coat=0.7,
                        noise=dict(scale=9, detail=7, bump=0.30,
                                   color1=(0.006, 0.006, 0.009, 1),
                                   color2=(0.034, 0.034, 0.042, 1)))
    M["obs_dark"] = material("M_Yermo_Obsidian_Deep", (0.006, 0.006, 0.008, 1),
                             rough=0.28, metal=0.5)
    M["bone"] = material("M_Yermo_Bone", (0.36, 0.38, 0.42, 1), rough=0.42,
                         noise=dict(scale=26, detail=6, bump=0.20,
                                    color1=(0.26, 0.28, 0.32, 1),
                                    color2=(0.46, 0.48, 0.52, 1)))
    M["vein"] = material("M_Yermo_Vein", (0.85, 0.94, 1.0, 1), rough=0.22,
                         emission=(0.75, 0.90, 1.0, 1), emission_strength=5.0)
    M["core"] = material("M_Yermo_Core", (1.0, 1.0, 1.0, 1), rough=0.15,
                         emission=(0.85, 0.94, 1.0, 1), emission_strength=7.0)
    M["warn"] = material("M_Yermo_Warn", (1.0, 0.85, 0.30, 1), rough=0.3,
                         emission=(1.0, 0.72, 0.18, 1), emission_strength=5.5)
    M["cloak"] = material("M_Yermo_Static", (0.030, 0.030, 0.045, 1), rough=0.85,
                          alpha=0.72,
                          noise=dict(scale=60, detail=8, bump=0.5,
                                     color1=(0.020, 0.020, 0.032, 1),
                                     color2=(0.070, 0.070, 0.10, 1)))
    M["glass"] = material("M_Yermo_Sac", (0.62, 0.70, 0.78, 1), rough=0.12,
                          alpha=0.35, spec=1.0)
    return M


def hex_section(y, hw, top, bot, points=6, phase=0.0):
    """Seccion angular (hexagono) en el plano XZ."""
    mid = (top + bot) * 0.5
    hh = (top - bot) * 0.5
    pts = []
    for i in range(points):
        a = TAU * i / points + phase
        pts.append((hw * math.cos(a), y, mid + hh * math.sin(a)))
    return pts


# ---------------------------------------------------------------------------
# SABUESO
# ---------------------------------------------------------------------------

def build_sabueso(M, coll, ox, oy):
    parts = []

    def add(o, m):
        set_material(o, m)
        parts.append(o)
        return o

    # cuerpo drone
    sections = [
        hex_section(-0.52, 0.075, 0.72, 0.60, 6),
        hex_section(-0.40, 0.155, 0.80, 0.54, 6),
        hex_section(-0.20, 0.185, 0.83, 0.52, 6),
        hex_section(0.02, 0.180, 0.82, 0.53, 6),
        hex_section(0.24, 0.150, 0.79, 0.56, 6),
        hex_section(0.42, 0.095, 0.75, 0.60, 6),
        hex_section(0.50, 0.045, 0.72, 0.63, 6),
    ]
    body = loft("SM_Sabueso_Body", sections, coll=coll, close=True,
                cap_start=True, cap_end=True, smooth=False)
    bevel(body, 0.008, 2, 30)
    add(body, M["obs"])

    # placas del lomo
    for i, y in enumerate((-0.30, -0.08, 0.14, 0.34)):
        h = 0.16 - abs(y) * 0.10
        add(box("SM_Sabueso_Spine_%d" % i, (0.045, 0.10, h),
                location=(0, y, 0.84 + h * 0.3), rot=(0, 0, D2R(45)),
                bevel=0.004, smooth=False), M["bone"])

    # nucleo luminoso
    add(ellipsoid("SM_Sabueso_Core", (0.075, 0.075, 0.075),
                  location=(0, -0.06, 0.68), coll=coll, segments=12, rings=8),
        M["core"])
    add(ring_torus("SM_Sabueso_CoreRing", major=0.105, minor=0.010,
                   location=(0, -0.06, 0.68), coll=coll, rot=(D2R(90), 0, 0),
                   major_seg=16, minor_seg=6), M["bone"])

    # cabeza partida en dos mandibulas
    for sx in (-1, 1):
        jaw = cone("SM_Sabueso_Jaw_%d" % sx, r1=0.075, r2=0.012, depth=0.30,
                   location=(sx * 0.055, -0.66, 0.70),
                   rot=(D2R(90), 0, D2R(sx * -14)), segments=4, smooth=False)
        add(jaw, M["obs"])
        tip = cone("SM_Sabueso_Fang_%d" % sx, r1=0.026, r2=0.002, depth=0.14,
                   location=(sx * 0.085, -0.80, 0.66),
                   rot=(D2R(104), 0, D2R(sx * -18)), segments=4, smooth=False)
        add(tip, M["bone"])

    add(ellipsoid("SM_Sabueso_HeadCore", (0.05, 0.06, 0.05),
                  location=(0, -0.60, 0.72), coll=coll, segments=10, rings=8),
        M["warn"])

    # patas: dos segmentos angulares cada una
    legs = []
    for sx in (-1, 1):
        for sy, ya in ((-1, -0.30), (1, 0.24)):
            hip = (sx * 0.15, ya, 0.62)
            knee = (sx * 0.30, ya + sy * 0.10, 0.44)
            foot = (sx * 0.34, ya + sy * 0.02, 0.06)
            leg = tube("SM_Sabueso_Leg", [hip, knee, foot],
                       radius=[0.045, 0.032, 0.014], segments=4, coll=coll)
            legs.append(leg)
    for lg in legs:
        apply_all(lg)
    joined = smooth_join(legs, "SM_Sabueso_Legs")
    add(joined, M["obs"])
    return parts


# ---------------------------------------------------------------------------
# GARFIO
# ---------------------------------------------------------------------------

def build_garfio(M, coll):
    parts = []

    def add(o, m):
        set_material(o, m)
        parts.append(o)
        return o

    # cuerpo flotante (obelisco invertido)
    sections = [
        hex_section(-0.30, 0.10, 1.62, 1.40, 5),
        hex_section(-0.18, 0.24, 1.72, 1.18, 5),
        hex_section(0.00, 0.30, 1.76, 1.06, 5),
        hex_section(0.18, 0.26, 1.70, 1.16, 5),
        hex_section(0.30, 0.12, 1.62, 1.36, 5),
    ]
    body = loft("SM_Garfio_Body", sections, coll=coll, close=True,
                cap_start=True, cap_end=True, smooth=False)
    bevel(body, 0.006, 2, 25)
    add(body, M["obs"])

    add(ellipsoid("SM_Garfio_Core", (0.105, 0.105, 0.115),
                  location=(0, 0, 1.42), coll=coll, segments=14, rings=10),
        M["core"])
    add(ring_torus("SM_Garfio_CoreRing", major=0.155, minor=0.014,
                   location=(0, 0, 1.42), coll=coll, rot=(D2R(90), 0, 0),
                   major_seg=20, minor_seg=6), M["bone"])
    add(ring_torus("SM_Garfio_CoreRing2", major=0.155, minor=0.010,
                   location=(0, 0, 1.42), coll=coll, rot=(0, D2R(90), 0),
                   major_seg=20, minor_seg=6), M["bone"])

    # aletas
    for sx in (-1, 1):
        fin = box("SM_Garfio_Fin_%d" % sx, (0.055, 0.16, 0.30),
                  location=(sx * 0.30, 0, 1.50), rot=(0, D2R(sx * 22), 0),
                  bevel=0.004, smooth=False)
        add(fin, M["bone"])

    # brazo-cable
    cable = tube_smooth("SM_Garfio_Cable",
                        [(0.0, -0.20, 1.60), (0.10, -0.62, 1.44),
                         (-0.16, -1.05, 1.20), (0.08, -1.34, 0.92),
                         (0.02, -1.42, 0.66)],
                        [0.045, 0.038, 0.032, 0.026, 0.024],
                        [0.045, 0.038, 0.032, 0.026, 0.024],
                        sub=4, segments=8, coll=coll)
    add(cable, M["obs_dark"])

    # gancho de tres puntas
    for i in range(3):
        a = D2R(i * 120)
        tip = (math.cos(a) * 0.075, -1.46 + math.sin(a) * 0.02, 0.46)
        hook = tube("SM_Garfio_Hook_%d" % i,
                    [(0.02, -1.42, 0.64),
                     (math.cos(a) * 0.06, -1.46, 0.58),
                     (math.cos(a) * 0.11, -1.44, 0.50),
                     (math.cos(a) * 0.09, -1.40, 0.43)],
                    radius=[0.022, 0.018, 0.014, 0.004], segments=6,
                    coll=coll)
        add(hook, M["bone"])
    add(cylinder("SM_Garfio_HookBase", radius=0.055, depth=0.07,
                 location=(0, -1.42, 0.66), segments=8), M["obs"])

    # anillos flotantes
    for i in range(3):
        r = 0.34 + i * 0.06
        ring = ring_torus("SM_Garfio_Ring_%d" % i, major=r, minor=0.008,
                          location=(0, 0, 1.42), coll=coll,
                          rot=(D2R(30 + i * 40), D2R(i * 35), D2R(i * 20)),
                          major_seg=24, minor_seg=5)
        add(ring, M["vein"])
    return parts


# ---------------------------------------------------------------------------
# ESPORA
# ---------------------------------------------------------------------------

def build_espora(M, coll, x):
    parts = []
    sac = ellipsoid("SM_Espora_Sac", (0.34, 0.34, 0.38),
                    location=(0, 0, 0.62), coll=coll, segments=20, rings=14)
    deform(sac, lambda co: (co.x + 0.03 * math.sin(co.z * 9),
                            co.y + 0.03 * math.cos(co.x * 8),
                            co.z + 0.02 * math.sin(co.y * 10)))
    subsurf(sac, 1)
    set_material(sac, M["glass"])
    parts.append(sac)

    core = ellipsoid("SM_Espora_Core", (0.13, 0.13, 0.15),
                     location=(0, 0, 0.62), coll=coll, segments=16, rings=10)
    set_material(core, M["core"])
    parts.append(core)

    for i in range(8):
        a = TAU * i / 8
        spike = cone("SM_Espora_Spike_%d" % i, r1=0.020, r2=0.002, depth=0.16,
                     location=(math.cos(a) * 0.30, math.sin(a) * 0.30, 0.62),
                     rot=(0, D2R(90), a), segments=4, smooth=False)
        set_material(spike, M["bone"])
        parts.append(spike)

    for p in parts:
        apply_all(p)
    joined = smooth_join(parts, "SM_Espora")
    root = bpy.data.objects.new("SM_Espora", None)
    coll.objects.link(root)
    joined.parent = root
    root.location = (x, 0, 0)
    return root


# ---------------------------------------------------------------------------
# ROBOTS DE CUATRO BRAZOS
# ---------------------------------------------------------------------------

def build_robot(M, coll, name, x, cfg):
    parts = []

    def add(o, m):
        set_material(o, m)
        parts.append(o)
        return o

    s = cfg["scale"]
    # torax
    torax = loft("SM_Robot_Torax",
                 [hex_section(0.26 * s, 0.20 * s, 1.52 * s, 1.18 * s, 6),
                  hex_section(0.10 * s, 0.28 * s, 1.62 * s, 1.10 * s, 6),
                  hex_section(-0.10 * s, 0.30 * s, 1.64 * s, 1.06 * s, 6),
                  hex_section(-0.28 * s, 0.22 * s, 1.56 * s, 1.14 * s, 6)],
                 coll=coll, close=True, cap_start=True, cap_end=True,
                 smooth=False)
    bevel(torax, 0.008, 2, 25)
    add(torax, M["obs"])

    # cadera / columna
    add(tube("SM_Robot_Spine", [(0, 0.02, 0.36 * s), (0, 0.02, 1.10 * s)],
             radius=[0.085 * s, 0.075 * s], segments=6), M["obs_dark"])
    add(loft("SM_Robot_Pelvis",
             [hex_section(0.16 * s, 0.11 * s, 0.44 * s, 0.28 * s, 6),
              hex_section(-0.16 * s, 0.12 * s, 0.46 * s, 0.26 * s, 6)],
             coll=coll, close=True, cap_start=True, cap_end=True,
             smooth=False), M["obs"])

    # cabeza: yelmo de una sola ranura
    add(loft("SM_Robot_Head",
             [hex_section(0.14 * s, 0.09 * s, 1.94 * s, 1.78 * s, 6),
              hex_section(0.02 * s, 0.135 * s, 2.02 * s, 1.74 * s, 6),
              hex_section(-0.12 * s, 0.115 * s, 1.98 * s, 1.78 * s, 6)],
             coll=coll, close=True, cap_start=True, cap_end=True,
             smooth=False), M["bone"])
    add(box("SM_Robot_Visor", (0.185 * s, 0.035 * s, 0.028 * s),
            location=(0, -0.115 * s, 1.885 * s), bevel=0.004, smooth=False),
        M["warn"])

    # cuatro brazos largos
    arms = []
    for sx in (-1, 1):
        for level, (zz, spread) in enumerate(((1.50, 0.40), (1.22, 0.30))):
            sh = (sx * 0.26 * s, 0.0, zz * s)
            el = (sx * (0.26 + spread * 0.5) * s, -0.10 * s,
                  (zz - 0.30) * s)
            wr = (sx * (0.26 + spread) * s, -0.34 * s, (zz - 0.58) * s)
            arm = tube_smooth("SM_Robot_Arm_%d_%d" % (sx, level),
                              [sh, el, wr],
                              [0.055 * s, 0.042 * s, 0.030 * s],
                              [0.055 * s, 0.042 * s, 0.030 * s],
                              sub=4, segments=6, coll=coll)
            arms.append(arm)
            # pinza de dos dedos
            for fy in (-1, 1):
                finger = tube("SM_Robot_Finger_%d_%d_%d" % (sx, level, fy),
                              [wr, (wr[0] + sx * 0.02 * s,
                                    wr[1] - 0.09 * s, wr[2] + fy * 0.03 * s)],
                              radius=[0.020 * s, 0.006 * s], segments=4,
                              coll=coll)
                arms.append(finger)
    for a in arms:
        apply_all(a)
    joined = smooth_join(arms, "SM_Robot_Arms")
    add(joined, M["obs"])

    # piernas
    legs = []
    for sx in (-1, 1):
        hip = (sx * 0.11 * s, 0.06 * s, 0.34 * s)
        knee = (sx * 0.16 * s, -0.06 * s, 0.20 * s)
        foot = (sx * 0.17 * s, -0.02 * s, 0.02 * s)
        legs.append(tube("SM_Robot_Leg_%d" % sx, [hip, knee, foot],
                         radius=[0.055 * s, 0.040 * s, 0.030 * s],
                         segments=6, coll=coll))
    for lg in legs:
        apply_all(lg)
    add(smooth_join(legs, "SM_Robot_Legs"), M["obs"])

    # pecho luminoso / escudo
    add(ellipsoid("SM_Robot_Core", (0.075 * s, 0.05 * s, 0.075 * s),
                  location=(0, -0.28 * s, 1.40 * s), coll=coll,
                  segments=12, rings=8), M["core"])

    if cfg.get("cloak"):
        cloak = loft("SM_Robot_Cloak",
                     [hex_section(0.20 * s, 0.30 * s, 1.56 * s, 1.10 * s, 8),
                      hex_section(0.06 * s, 0.40 * s, 1.40 * s, 0.90 * s, 8),
                      hex_section(-0.10 * s, 0.34 * s, 1.20 * s, 0.70 * s, 8)],
                     coll=coll, close=False, cap_start=False, cap_end=False,
                     smooth=True)
        solidify(cloak, 0.02, 0)
        add(cloak, M["cloak"])

    if cfg.get("shield"):
        disc = cylinder("SM_Robot_ShieldDisc", radius=0.34 * s, depth=0.020,
                        location=(0.0, -0.46 * s, 1.34 * s),
                        rot=(D2R(90), 0, 0), segments=12)
        add(disc, M["vein"])
        add(ring_torus("SM_Robot_ShieldRing", major=0.34 * s, minor=0.016,
                       location=(0.0, -0.46 * s, 1.34 * s), coll=coll,
                       rot=(D2R(90), 0, 0), major_seg=24, minor_seg=6),
            M["bone"])

    if cfg.get("rifle"):
        add(tube("SM_Robot_Rifle", [(0.62 * s, -0.30 * s, 1.24 * s),
                                    (0.70 * s, -1.35 * s, 1.16 * s)],
                 radius=[0.030 * s, 0.018 * s], segments=6), M["obs_dark"])
        add(ring_torus("SM_Robot_RifleRing", major=0.032 * s, minor=0.008,
                       location=(0.70 * s, -1.30 * s, 1.162 * s), coll=coll,
                       rot=(D2R(90), 0, 0), major_seg=12, minor_seg=5),
            M["vein"])

    for p in parts:
        apply_all(p)
    joined = smooth_join(parts, name)
    root = bpy.data.objects.new(name, None)
    coll.objects.link(root)
    joined.parent = root
    root.location = (x, 0, 0)
    return root


def finish(name, parts, coll, x, y=0.0):
    for p in parts:
        apply_all(p)
    joined = smooth_join(parts, name)
    root = bpy.data.objects.new(name, None)
    root.empty_display_type = 'PLAIN_AXES'
    root.empty_display_size = 0.4
    coll.objects.link(root)
    joined.parent = root
    root.location = (x, y, 0)
    return root


def build():
    scene = reset_scene()
    M = materials()
    coll = collection("SK_Yermos")

    finish("SM_Sabueso", build_sabueso(M, coll, -4.4, 0), coll, -4.4)
    finish("SM_Garfio", build_garfio(M, coll), coll, -1.6)
    build_espora(M, coll, 1.4)
    build_robot(M, coll, "SM_Robot_Manto", 4.2,
                dict(scale=1.0, cloak=True, rifle=True))
    build_robot(M, coll, "SM_Robot_Torre", 7.4,
                dict(scale=1.12, shield=True))

    report([o for o in bpy.data.objects if o.type == 'MESH'])
    studio()
    scene.render.engine = 'BLENDER_EEVEE'
    save(os.path.join(ROOT, "Enemies", "SK_Yermos.blend"))


build()
