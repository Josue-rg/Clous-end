"""
CLOUS-END · SK_Reindeer  (v3)

Reno base + 4 variantes: Rodolfo, Donner, Vixen, Cometa.
Semirrealista. 1 unidad = 1 m. Mira hacia -Y. Sin rig.
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
tube_ellipse = g["tube_ellipse"]
tube_smooth = g["tube_smooth"]
loft = g["loft"]
sweep_rect = g["sweep_rect"]
ellipsoid = g["ellipsoid"]
box = g["box"]
ring_torus = g["ring_torus"]
star = g["star"]
material = g["material"]
palette = g["palette"]
set_material = g["set_material"]
subsurf = g["subsurf"]
bevel = g["bevel"]
solidify = g["solidify"]
displace = g["displace"]
apply_all = g["apply_all"]
smooth_join = g["smooth_join"]
studio = g["studio"]
save = g["save"]
report = g["report"]
D2R = g["D2R"]
TAU = g["TAU"]

ROOT = r"C:\Users\Josue\Desktop\Clous-end\BlenderSource"

BODY = [
    (-0.92, 0.098, 1.060, 0.985),
    (-0.84, 0.185, 1.185, 0.845),
    (-0.68, 0.248, 1.272, 0.760),
    (-0.48, 0.284, 1.318, 0.722),
    (-0.22, 0.300, 1.334, 0.710),
    (0.04, 0.292, 1.320, 0.722),
    (0.32, 0.274, 1.288, 0.750),
    (0.60, 0.248, 1.252, 0.796),
    (0.80, 0.198, 1.212, 0.858),
    (0.94, 0.104, 1.170, 0.932),
]

VARIANTS = [
    dict(key="Rodolfo", offset=(-4.6, 0), scale=1.00, width=1.00, leg=0.00,
         antler=1.00, fur=(0.255, 0.175, 0.118), belly=(0.62, 0.57, 0.48),
         harness="red", bell="brass", nose_glow=True, plates=False,
         ribbons=False, star=False, mane=1.00),
    dict(key="Donner", offset=(-1.55, 0), scale=1.07, width=1.12, leg=0.02,
         antler=1.16, fur=(0.185, 0.135, 0.100), belly=(0.50, 0.46, 0.40),
         harness="leather", bell="iron", nose_glow=False, plates=True,
         ribbons=False, star=False, mane=1.20),
    dict(key="Vixen", offset=(1.55, 0), scale=0.93, width=0.93, leg=-0.02,
         antler=0.80, fur=(0.330, 0.240, 0.165), belly=(0.70, 0.66, 0.57),
         harness="blue", bell="brass", nose_glow=False, plates=False,
         ribbons=True, star=False, mane=0.90),
    dict(key="Cometa", offset=(4.6, 0), scale=1.00, width=0.96, leg=0.070,
         antler=1.02, fur=(0.285, 0.205, 0.145), belly=(0.66, 0.62, 0.53),
         harness="star", bell="brass", nose_glow=False, plates=False,
         ribbons=False, star=True, mane=1.02),
]

NECK = [(0.0, -0.835, 1.190), (0.0, -0.900, 1.288), (0.0, -0.968, 1.380),
        (0.0, -1.035, 1.458), (0.0, -1.100, 1.522), (0.0, -1.158, 1.566)]
NECK_RX = [0.202, 0.186, 0.167, 0.148, 0.132, 0.120]
NECK_RZ = [0.238, 0.218, 0.196, 0.172, 0.152, 0.135]

HEAD = [(0.0, -1.195, 1.575), (0.0, -1.290, 1.570), (0.0, -1.375, 1.552),
        (0.0, -1.450, 1.528), (0.0, -1.520, 1.500), (0.0, -1.575, 1.476),
        (0.0, -1.605, 1.462)]
HEAD_RX = [0.106, 0.101, 0.090, 0.075, 0.062, 0.053, 0.047]
HEAD_RZ = [0.112, 0.106, 0.093, 0.079, 0.067, 0.056, 0.049]

ABASE = (0.0, -1.250, 1.628)
BEAM = [(0.056, 0.000, 0.000), (0.092, 0.034, 0.110), (0.128, 0.094, 0.240),
        (0.158, 0.174, 0.360), (0.178, 0.258, 0.450), (0.186, 0.334, 0.506),
        (0.178, 0.396, 0.536)]
TINES = [
    [(0.088, 0.030, 0.100), (0.104, -0.070, 0.178), (0.114, -0.150, 0.235)],
    [(0.130, 0.092, 0.235), (0.156, 0.000, 0.305), (0.170, -0.070, 0.360)],
    [(0.160, 0.170, 0.350), (0.212, 0.140, 0.440), (0.242, 0.115, 0.505)],
    [(0.180, 0.250, 0.445), (0.236, 0.282, 0.540), (0.262, 0.310, 0.605)],
]


def body_at(y):
    for i in range(len(BODY) - 1):
        y0, w0, t0, b0 = BODY[i]
        y1, w1, t1, b1 = BODY[i + 1]
        if y0 <= y <= y1:
            t = (y - y0) / (y1 - y0)
            return (w0 + (w1 - w0) * t, t0 + (t1 - t0) * t, b0 + (b1 - b0) * t)
    if y < BODY[0][0]:
        return BODY[0][1:]
    return BODY[-1][1:]


def body_section(y, expand=0.0, seg=24):
    hw, top, bot = body_at(y)
    hw += expand
    mid = (top + bot) * 0.5
    hh = (top - bot) * 0.5 + expand
    pts = []
    for i in range(seg):
        a = TAU * i / seg
        pts.append((hw * math.cos(a), y, mid + hh * math.sin(a)))
    return pts


def make_fur_material(name, color):
    return material("M_Fur_%s" % name, tuple(color[:3]) + (1,), rough=0.86,
                    sheen=0.45,
                    noise=dict(scale=46, detail=7, bump=0.32, mapping=(1, 3, 1),
                               color1=tuple(c * 0.70 for c in color[:3]) + (1,),
                               color2=tuple(min(1.0, c * 1.20) for c in color[:3]) + (1,)))


def make_belly_material(name, color):
    return material("M_FurBelly_%s" % name, tuple(color[:3]) + (1,), rough=0.90,
                    sheen=0.55,
                    noise=dict(scale=54, detail=6, bump=0.26,
                               color1=tuple(c * 0.84 for c in color[:3]) + (1,),
                               color2=tuple(min(1.0, c * 1.10) for c in color[:3]) + (1,)))


def build_reindeer(cfg, P):
    key = cfg["key"]
    root_coll = collection("SK_Reindeer_%s" % key)
    parts = []
    ox, oy = cfg["offset"]
    sc = cfg["scale"]
    wid = cfg["width"] * sc
    leg_extra = cfg["leg"]

    fur = make_fur_material(key, cfg["fur"])
    belly = make_belly_material(key, cfg["belly"])
    hoof_mat = material("M_Hoof", (0.050, 0.044, 0.040, 1), rough=0.40)
    eye_mat = material("M_Reindeer_Eye", (0.010, 0.010, 0.012, 1), rough=0.05)
    antler_mat = material("M_Antler", (0.345, 0.288, 0.208, 1), rough=0.60,
                          noise=dict(scale=34, detail=6, bump=0.28,
                                     color1=(0.255, 0.205, 0.140, 1),
                                     color2=(0.445, 0.378, 0.278, 1)))
    nose_mat = material("M_Reindeer_Muzzle", (0.075, 0.062, 0.058, 1), rough=0.55,
                        noise=dict(scale=90, detail=5, bump=0.2,
                                   color1=(0.055, 0.045, 0.042, 1),
                                   color2=(0.105, 0.088, 0.082, 1)))

    harness_mats = {
        "red": material("M_Harness_Red", (0.28, 0.030, 0.028, 1), rough=0.62),
        "leather": material("M_Harness_Leather", (0.15, 0.093, 0.055, 1),
                            rough=0.55),
        "blue": material("M_Harness_Blue", (0.055, 0.105, 0.185, 1), rough=0.65),
        "star": material("M_Harness_Star", (0.185, 0.055, 0.045, 1), rough=0.6),
    }
    hm = harness_mats[cfg["harness"]]

    def bz(z):
        return z * sc + leg_extra

    def lz(z):
        t = max(0.0, min(1.0, (z - 0.055) / (1.040 - 0.055)))
        return z * sc + leg_extra * t

    def add(obj, mat, name=None):
        if name:
            obj.name = name
            obj.data.name = name
        set_material(obj, mat)
        parts.append(obj)
        return obj

    # ================================================================ TRONCO
    sections = []
    for y, hw, top, bot in BODY:
        sec = body_section(y)
        sections.append([(x * wid, y * sc, bz(z)) for x, y, z in sec])
    body = loft("SK_Reindeer_Body", sections, coll=root_coll, close=True,
                cap_start=True, cap_end=True, smooth=True)
    subsurf(body, 1)
    displace(body, 0.004, 0.09)
    body.data.materials.clear()
    body.data.materials.append(fur)
    body.data.materials.append(belly)
    for poly in body.data.polygons:
        poly.material_index = 1 if poly.normal.z < -0.32 else 0
    parts.append(body)

    # hombros y muslos
    for sx in (-1, 1):
        sh = ellipsoid("SK_Reindeer_Shoulder",
                       (0.118 * wid, 0.190 * sc, 0.155 * sc),
                       location=(sx * 0.158 * wid, -0.560 * sc, bz(1.090)),
                       coll=root_coll, segments=18, rings=12)
        subsurf(sh, 1)
        add(sh, fur)
        th = ellipsoid("SK_Reindeer_Thigh",
                       (0.128 * wid, 0.200 * sc, 0.168 * sc),
                       location=(sx * 0.162 * wid, 0.590 * sc, bz(1.070)),
                       coll=root_coll, segments=18, rings=12)
        subsurf(th, 1)
        add(th, fur)

    # pecho peludo
    chest = ellipsoid("SK_Reindeer_Chest", (0.145 * wid, 0.115 * sc, 0.150 * sc),
                      location=(0, -0.880 * sc, bz(0.960)),
                      coll=root_coll, segments=16, rings=10)
    subsurf(chest, 1)
    displace(chest, 0.008, 0.05)
    add(chest, belly)

    # ================================================================ CUELLO
    neck = tube_smooth("SK_Reindeer_Neck",
                        [(x * wid, y * sc, bz(z)) for x, y, z in NECK],
                        rx=[r * wid for r in NECK_RX],
                        rz=[r * sc for r in NECK_RZ],
                        sub=6, segments=20, coll=root_coll)
    subsurf(neck, 1)
    displace(neck, 0.004, 0.08)
    neck.data.materials.clear()
    neck.data.materials.append(fur)
    neck.data.materials.append(belly)
    for poly in neck.data.polygons:
        poly.material_index = 1 if poly.normal.z < -0.25 else 0
    parts.append(neck)

    mane = ellipsoid("SK_Reindeer_Mane",
                     (0.098 * wid * cfg["mane"], 0.180 * sc, 0.125 * sc),
                     location=(0, -0.960 * sc, bz(1.250)),
                     coll=root_coll, segments=20, rings=12)
    subsurf(mane, 1)
    displace(mane, 0.012, 0.05)
    add(mane, belly)

    # ================================================================ CABEZA
    head = tube_smooth("SK_Reindeer_Head",
                        [(x * wid, y * sc, bz(z)) for x, y, z in HEAD],
                        rx=[r * wid for r in HEAD_RX],
                        rz=[r * sc for r in HEAD_RZ],
                        sub=7, segments=20, coll=root_coll)
    subsurf(head, 1)
    displace(head, 0.003, 0.06)
    add(head, fur)

    brow = ellipsoid("SK_Reindeer_Brow", (0.092 * wid, 0.085 * sc, 0.058 * sc),
                     location=(0, -1.318 * sc, bz(1.606)),
                     coll=root_coll, segments=16, rings=10)
    subsurf(brow, 1)
    add(brow, fur)

    muzzle = ellipsoid("SK_Reindeer_Muzzle",
                       (0.048 * wid, 0.042 * sc, 0.036 * sc),
                       location=(0, -1.672 * sc, bz(1.439)),
                       coll=root_coll, segments=16, rings=10)
    subsurf(muzzle, 1)
    add(muzzle, nose_mat)

    for sx in (-1, 1):
        nostril = ellipsoid("SK_Reindeer_Nostril", (0.010, 0.008, 0.007),
                            location=(sx * 0.022 * wid, -1.700 * sc,
                                      bz(1.441)),
                            coll=root_coll, segments=10, rings=6)
        add(nostril, material("M_Nostril", (0.010, 0.009, 0.009, 1), rough=0.3))

        eye = ellipsoid("SK_Reindeer_Eye", (0.019, 0.017, 0.018),
                        location=(sx * 0.085 * wid, -1.392 * sc, bz(1.540)),
                        coll=root_coll, segments=14, rings=8)
        add(eye, eye_mat)
        glint = ellipsoid("SK_Reindeer_Glint", (0.0045, 0.0038, 0.0045),
                          location=(sx * 0.090 * wid, -1.403 * sc,
                                    bz(1.547)),
                          coll=root_coll, segments=8, rings=6)
        add(glint, material("M_Glint", (1, 1, 1, 1), rough=0.1,
                            emission=(1, 1, 1, 1), emission_strength=1.4))

        ear = ellipsoid("SK_Reindeer_Ear", (0.030 * sc, 0.082 * sc, 0.044 * sc),
                        location=(sx * 0.100 * wid, -1.258 * sc, bz(1.632)),
                        coll=root_coll,
                        rot=(D2R(-12), D2R(sx * 28), D2R(sx * 14)),
                        segments=14, rings=10)
        subsurf(ear, 1)
        add(ear, fur)

    # ================================================================ ASTAS
    ast = cfg["antler"]
    antler_parts = []

    def asta(rel):
        pts = []
        for x, y, z in rel:
            wz = ABASE[2] + (ABASE[2] + z - ABASE[2]) * 0.0 + (z * ast)
            pts.append(((ABASE[0] + x) * wid,
                        (ABASE[1] + y) * sc,
                        bz(ABASE[2] + z * ast)))
        return pts

    for sx in (-1, 1):
        beam = [(sx * x, y, z) for x, y, z in BEAM]
        antler_parts.append(tube(
            "SK_Reindeer_Beam_%s" % ("R" if sx > 0 else "L"), asta(beam),
            radius=[0.052, 0.047, 0.040, 0.033, 0.026, 0.018, 0.009],
            segments=8, coll=root_coll))
        for ti, t in enumerate(TINES):
            t2 = [(sx * x, y, z) for x, y, z in t]
            antler_parts.append(tube(
                "SK_Reindeer_Tine_%d_%s" % (ti, "R" if sx > 0 else "L"),
                asta(t2), radius=[0.030, 0.020, 0.008], segments=6,
                coll=root_coll))

    for a in antler_parts:
        subsurf(a, 1)
        set_material(a, antler_mat)
    for a in antler_parts:
        apply_all(a)
    antlers = smooth_join(antler_parts, "SK_Reindeer_Antlers")
    parts.append(antlers)

    # ================================================================ PATAS
    front_pts = [(0.155, -0.545, 1.040), (0.166, -0.470, 0.780),
                 (0.172, -0.510, 0.520), (0.168, -0.545, 0.330),
                 (0.164, -0.565, 0.190), (0.162, -0.578, 0.070)]
    hind_pts = [(0.168, 0.585, 1.055), (0.196, 0.690, 0.800),
                (0.188, 0.505, 0.470), (0.182, 0.522, 0.320),
                (0.178, 0.520, 0.190), (0.176, 0.512, 0.070)]

    leg_objs, hoof_objs = [], []
    for pts, radii in ((front_pts, (0.096, 0.078, 0.056, 0.036, 0.041, 0.031)),
                       (hind_pts, (0.112, 0.092, 0.058, 0.036, 0.041, 0.031))):
        for sx in (-1, 1):
            path = [(sx * x * wid, y * sc, lz(z)) for x, y, z in pts]
            leg = tube_smooth("SK_Reindeer_Leg", path,
                              [r * sc for r in radii],
                              [r * sc for r in radii],
                              sub=3, segments=12, coll=root_coll)
            subsurf(leg, 1)
            leg_objs.append(leg)
            hoof = box("SK_Reindeer_Hoof", (0.080 * sc, 0.128 * sc, 0.078 * sc),
                       location=(sx * pts[-1][0] * wid,
                                 pts[-1][1] * sc - 0.022, lz(0.038)),
                       coll=root_coll, bevel=0.020, segments=2, smooth=True)
            hoof_objs.append(hoof)

    for lg in leg_objs:
        apply_all(lg)
    legs = smooth_join(leg_objs, "SK_Reindeer_Legs")
    add(legs, fur)
    for hf in hoof_objs:
        apply_all(hf)
    hooves = smooth_join(hoof_objs, "SK_Reindeer_Hooves")
    add(hooves, hoof_mat)

    # ================================================================ COLA
    tail = ellipsoid("SK_Reindeer_Tail", (0.038 * sc, 0.054 * sc, 0.078 * sc),
                     location=(0, 0.962 * sc, bz(1.140)), coll=root_coll,
                     segments=12, rings=8)
    subsurf(tail, 1)
    add(tail, belly)

    # ================================================================ ARNES
    hparts = []
    for y in (-0.545, -0.150):
        sec = [body_section(y - 0.042, expand=0.014, seg=24),
               body_section(y + 0.042, expand=0.014, seg=24)]
        sec = [[(x * wid, yy * sc, bz(z)) for x, yy, z in s] for s in sec]
        band = loft("SK_Reindeer_Band", sec, coll=root_coll, close=True,
                    smooth=True)
        solidify(band, 0.020, 0)
        hparts.append(band)

    spine = []
    for y in (-0.56, -0.28, 0.0, 0.28, 0.58, 0.80):
        hw, top, bot = body_at(y)
        spine.append((0.0, y * sc, bz(top) + 0.018))
    hparts.append(sweep_rect("SK_Reindeer_SpineStrap", spine,
                             width=0.058 * sc, height=0.018 * sc,
                             coll=root_coll, side_axis=(1, 0, 0)))

    collar_pts = []
    for i in range(16):
        a = TAU * i / 16
        collar_pts.append((math.cos(a) * 0.176 * wid,
                           -0.930 * sc + math.sin(a) * 0.045,
                           bz(1.300) + math.sin(a) * 0.150))
    hparts.append(loft("SK_Reindeer_Collar",
                       [collar_pts,
                        [(x, y + 0.050, z) for x, y, z in collar_pts]],
                       coll=root_coll, close=True, smooth=True))

    for hp in hparts:
        apply_all(hp)
    harness = smooth_join(hparts, "SK_Reindeer_Harness")
    add(harness, hm)

    bell_mat = P['brass'] if cfg["bell"] == "brass" else P['iron']
    bell = lathe("SK_Reindeer_Bell",
                 [(0.0, 0.0), (0.026, 0.004), (0.044, 0.035),
                  (0.052, 0.072), (0.044, 0.100), (0.0, 0.108)],
                 segments=14, coll=root_coll,
                 location=(0, -0.560 * sc, bz(0.820)))
    add(bell, bell_mat)
    add(ring_torus("SK_Reindeer_BellRing", major=0.030, minor=0.008,
                   location=(0, -0.560 * sc, bz(0.932)),
                   coll=root_coll, major_seg=12, minor_seg=6), bell_mat)

    if cfg["plates"]:
        for sx in (-1, 1):
            plate = box("SK_Reindeer_Plate", (0.035, 0.150, 0.092),
                        location=(sx * 0.300 * wid, -0.300 * sc, bz(1.140)),
                        coll=root_coll, rot=(0, D2R(sx * -8), 0),
                        bevel=0.014, segments=2, smooth=True)
            add(plate, P['iron'])

    if cfg["ribbons"]:
        ribbon_mat = material("M_Ribbon_Blue", (0.10, 0.20, 0.42, 1), rough=0.55)
        for sx in (-1, 1):
            for i, y in enumerate((-0.42, -0.18)):
                ribbon = box("SK_Reindeer_Ribbon", (0.030, 0.056, 0.150),
                             location=(sx * 0.284 * wid, y * sc,
                                       bz(1.010 - i * 0.045)),
                             coll=root_coll, rot=(D2R(6), 0, D2R(sx * 12)),
                             bevel=0.005, smooth=True)
                add(ribbon, ribbon_mat)

    if cfg["star"]:
        add(star("SK_Reindeer_Star", outer=0.070, inner=0.029, depth=0.012,
                 points=5, coll=root_coll,
                 location=(0, -0.070 * sc, bz(1.345)),
                 rot=(D2R(-90), 0, 0)), P['brass'])

    if cfg["nose_glow"]:
        glow = ellipsoid("SK_Reindeer_NoseGlow", (0.036, 0.027, 0.029),
                         location=(0, -1.706 * sc, bz(1.440)),
                         coll=root_coll, segments=16, rings=10)
        add(glow, material("M_Rodolfo_Nose", (0.9, 0.30, 0.20, 1), rough=0.30,
                           emission=(1.0, 0.32, 0.16, 1), emission_strength=7.0))

    # ================================================================ CIERRE
    root = bpy.data.objects.new("SK_Reindeer_%s" % key, None)
    root.empty_display_type = 'PLAIN_AXES'
    root.empty_display_size = 0.5
    root_coll.objects.link(root)
    root.location = (ox, oy, 0)
    for p in parts:
        p.data.name = p.name + "_" + key
        p.parent = root
    return root


def build():
    scene = reset_scene()
    P = palette()
    roots = [build_reindeer(cfg, P) for cfg in VARIANTS]
    print("RENOS:", len(roots))
    report([o for o in bpy.data.objects if o.type == 'MESH'])
    studio()
    scene.render.engine = 'BLENDER_EEVEE'
    save(os.path.join(ROOT, "Characters", "SK_Reindeer.blend"))
    return roots


build()
