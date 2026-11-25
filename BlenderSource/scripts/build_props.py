"""
CLOUS-END · Props y pickups (v1)

Cajas de regalo/armas, munición, kit de casco, escudo de reno, collar,
meteoritos y cascabeles sueltos.
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
ellipsoid = g["ellipsoid"]
box = g["box"]
cone = g["cone"]
cylinder = g["cylinder"]
ring_torus = g["ring_torus"]
star = g["star"]
material = g["material"]
palette = g["palette"]
set_material = g["set_material"]
subsurf = g["subsurf"]
bevel = g["bevel"]
displace = g["displace"]
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
    M["wood"] = material("M_Crate_Wood", (0.235, 0.150, 0.078, 1), rough=0.62,
                         noise=dict(scale=22, detail=8, bump=0.4, mapping=(3, 20, 3),
                                    color1=(0.165, 0.100, 0.048, 1),
                                    color2=(0.320, 0.215, 0.120, 1)))
    M["wood_dark"] = material("M_Crate_Wood_Dark", (0.135, 0.082, 0.042, 1),
                              rough=0.68)
    M["iron"] = material("M_Prop_Iron", (0.085, 0.080, 0.078, 1), rough=0.5,
                         metal=1.0)
    M["brass"] = material("M_Prop_Brass", (0.56, 0.40, 0.16, 1), rough=0.32,
                          metal=1.0)
    M["ribbon"] = material("M_Ribbon_Red", (0.34, 0.030, 0.028, 1), rough=0.42,
                           sheen=0.5,
                           noise=dict(scale=70, detail=5, bump=0.2,
                                      color1=(0.26, 0.020, 0.020, 1),
                                      color2=(0.42, 0.048, 0.040, 1)))
    M["card"] = material("M_Cardboard", (0.40, 0.285, 0.170, 1), rough=0.92,
                         noise=dict(scale=80, detail=5, bump=0.25,
                                    color1=(0.32, 0.225, 0.130, 1),
                                    color2=(0.48, 0.350, 0.215, 1)))
    M["paper"] = material("M_Label", (0.70, 0.66, 0.57, 1), rough=0.85)
    M["leather"] = material("M_Prop_Leather", (0.145, 0.090, 0.052, 1), rough=0.55)
    M["obsidian"] = material("M_Meteor_Rock", (0.032, 0.030, 0.034, 1), rough=0.58,
                             metal=0.10,
                             noise=dict(scale=8, detail=7, bump=0.5,
                                        color1=(0.012, 0.011, 0.013, 1),
                                        color2=(0.060, 0.056, 0.062, 1)))
    M["vein"] = material("M_Meteor_Vein", (0.85, 0.95, 1.0, 1), rough=0.3,
                         emission=(0.72, 0.90, 1.0, 1), emission_strength=12.0)
    M["glow"] = material("M_Pickup_Glow", (1.0, 0.86, 0.52, 1), rough=0.25,
                         emission=(1.0, 0.80, 0.42, 1), emission_strength=10.0)
    M["shield"] = material("M_Shield_Glow", (0.66, 0.90, 1.0, 1), rough=0.2,
                           emission=(0.55, 0.85, 1.0, 1), emission_strength=9.0)
    M["snow"] = material("M_Prop_Snow", (0.86, 0.90, 0.96, 1), rough=0.42)
    return M


def add_bow(coll, z, size=1.0, mat=None):
    """Lazo de cinta en cruz sobre una caja."""
    parts = []
    for sx in (-1, 1):
        loop = ring_torus("SM_BowLoop", major=0.070 * size, minor=0.016 * size,
                          location=(sx * 0.075 * size, 0, z),
                          coll=coll, rot=(D2R(90), 0, D2R(sx * 26)),
                          major_seg=18, minor_seg=6)
        parts.append(loop)
    knot = ellipsoid("SM_BowKnot", (0.030 * size, 0.030 * size, 0.026 * size),
                     location=(0, 0, z + 0.004 * size), coll=coll,
                     segments=12, rings=8)
    parts.append(knot)
    for i, (sx, sy) in enumerate(((-1, -1), (1, -1), (-1, 1), (1, 1))):
        tail = box("SM_BowTail_%d" % i, (0.022 * size, 0.006 * size, 0.075 * size),
                   location=(sx * 0.030 * size, sy * 0.012 * size,
                             z - 0.045 * size),
                   coll=coll, rot=(0, D2R(sx * 12), D2R(sx * 10)),
                   bevel=0.003, smooth=True)
        parts.append(tail)
    for p in parts:
        apply_all(p)
    bow = smooth_join(parts, "SM_Bow")
    set_material(bow, mat)
    return bow


def build_weapon_crate(M, coll, x):
    parts = []

    def add(o, m):
        set_material(o, m)
        parts.append(o)
        return o

    # caja
    add(box("SM_Crate_Body", (1.10, 0.56, 0.44),
            location=(0, 0, 0.22), bevel=0.014, segments=2, smooth=True), M["wood"])
    for i in range(3):
        add(box("SM_Crate_Slat_%d" % i, (1.13, 0.075, 0.47),
                location=(0, -0.19 + i * 0.19, 0.22), bevel=0.008,
                segments=2, smooth=True), M["wood_dark"])
    for sx in (-1, 1):
        add(box("SM_Crate_Corner_%d" % sx, (0.055, 0.58, 0.47),
                location=(sx * 0.545, 0, 0.22), bevel=0.008, segments=2,
                smooth=True), M["iron"])
    add(box("SM_Crate_Band", (1.15, 0.60, 0.035),
            location=(0, 0, 0.425), bevel=0.006, smooth=True), M["iron"])

    # tapa abierta
    lid = box("SM_Crate_Lid", (1.10, 0.56, 0.050),
              location=(0, 0.40, 0.66), rot=(D2R(-58), 0, 0),
              bevel=0.010, segments=2, smooth=True)
    add(lid, M["wood"])
    add(box("SM_Crate_LidRibbon", (0.145, 0.58, 0.056),
            location=(0, 0.40, 0.66), rot=(D2R(-58), 0, 0),
            bevel=0.004, smooth=True), M["ribbon"])

    # cinta en cruz
    for sy in (0,):
        add(box("SM_Crate_RibbonY", (0.145, 0.60, 0.47),
                location=(0, 0, 0.22), bevel=0.004, smooth=True), M["ribbon"])
    add(box("SM_Crate_RibbonX", (1.15, 0.145, 0.47),
            location=(0, 0, 0.22), bevel=0.004, smooth=True), M["ribbon"])

    # interior iluminado
    add(box("SM_Crate_InnerGlow", (0.95, 0.42, 0.02),
            location=(0, 0, 0.395), bevel=0.01, smooth=True), M["glow"])

    # placa
    add(box("SM_Crate_Plate", (0.20, 0.012, 0.13),
            location=(0, -0.292, 0.24), bevel=0.008, smooth=True), M["brass"])
    add(star("SM_Crate_Star", outer=0.048, inner=0.020, depth=0.012,
             points=5, coll=coll, location=(0, -0.300, 0.24),
             rot=(D2R(90), 0, 0)), M["glow"])

    for p in parts:
        apply_all(p)
    joined = smooth_join(parts, "SM_GiftBox_Weapon")
    root = bpy.data.objects.new("SM_GiftBox_Weapon", None)
    root.empty_display_type = 'PLAIN_AXES'
    root.empty_display_size = 0.3
    coll.objects.link(root)
    joined.parent = root
    root.location = (x, 0, 0)
    return root


def build_ammo_box(M, coll, x):
    parts = []

    def add(o, m):
        set_material(o, m)
        parts.append(o)
        return o

    add(box("SM_Ammo_Body", (0.36, 0.26, 0.20),
            location=(0, 0, 0.10), bevel=0.008, segments=2, smooth=True), M["card"])
    add(box("SM_Ammo_Lid", (0.375, 0.275, 0.035),
            location=(0, 0, 0.215), bevel=0.006, segments=2, smooth=True), M["card"])
    add(box("SM_Ammo_Label", (0.16, 0.005, 0.09),
            location=(0, -0.132, 0.11), bevel=0.003, smooth=True), M["paper"])
    add(box("SM_Ammo_Stripe", (0.376, 0.02, 0.045),
            location=(0, 0, 0.02), bevel=0.002, smooth=True), M["ribbon"])
    for sx in (-1, 1):
        add(cylinder("SM_Ammo_Round_%d" % sx, radius=0.014, depth=0.055,
                     location=(sx * 0.07, -0.05, 0.245),
                     rot=(D2R(90), 0, 0), segments=10), M["brass"])
    for p in parts:
        apply_all(p)
    joined = smooth_join(parts, "SM_Pickup_Ammo")
    root = bpy.data.objects.new("SM_Pickup_Ammo", None)
    coll.objects.link(root)
    joined.parent = root
    root.location = (x, 0, 0.28)
    joined.location = (0, 0, -0.28)
    return root


def build_helmet_kit(M, coll, x):
    parts = []

    def add(o, m):
        set_material(o, m)
        parts.append(o)
        return o

    add(box("SM_Kit_Body", (0.30, 0.24, 0.20),
            location=(0, 0, 0.10), bevel=0.010, segments=2, smooth=True), M["wood"])
    add(box("SM_Kit_Lid", (0.315, 0.255, 0.03),
            location=(0, 0, 0.208), bevel=0.005, segments=2, smooth=True), M["wood_dark"])
    for sx in (-1, 1):
        add(box("SM_Kit_Corner_%d" % sx, (0.022, 0.26, 0.21),
                location=(sx * 0.148, 0, 0.10), bevel=0.004, smooth=True), M["brass"])
    add(ring_torus("SM_Kit_Handle", major=0.075, minor=0.010,
                   location=(0, 0, 0.245), coll=coll, rot=(0, D2R(90), 0),
                   major_seg=18, minor_seg=6), M["iron"])
    for p in parts:
        apply_all(p)
    joined = smooth_join(parts, "SM_Pickup_HelmetKit")
    root = bpy.data.objects.new("SM_Pickup_HelmetKit", None)
    root.empty_display_type = 'PLAIN_AXES'
    root.empty_display_size = 0.2
    coll.objects.link(root)
    joined.parent = root
    bow = add_bow(coll, 0.255, 0.85, M["ribbon"])
    bow.location = (0, 0, 0)
    bow.parent = root
    root.location = (x, 0, 0)
    return root


def build_shield(M, coll, x):
    parts = []
    core = ellipsoid("SM_Shield_Core", (0.14, 0.14, 0.14),
                     location=(0, 0, 0.14), coll=coll, segments=20, rings=12)
    set_material(core, M["shield"])
    parts.append(core)
    for i, rot in enumerate(((0, 0, 0), (D2R(70), 0, D2R(20)),
                             (D2R(-60), D2R(30), 0))):
        r = ring_torus("SM_Shield_Ring_%d" % i, major=0.21, minor=0.012,
                       location=(0, 0, 0.14), coll=coll, rot=rot,
                       major_seg=32, minor_seg=6)
        set_material(r, M["shield"])
        parts.append(r)
    add = ellipsoid("SM_Shield_Snow", (0.06, 0.06, 0.06),
                    location=(0, 0, 0.14), coll=coll, segments=12, rings=8)
    set_material(add, M["glow"])
    parts.append(add)
    for p in parts:
        apply_all(p)
    joined = smooth_join(parts, "SM_Pickup_ReindeerShield")
    root = bpy.data.objects.new("SM_Pickup_ReindeerShield", None)
    coll.objects.link(root)
    joined.parent = root
    root.location = (x, 0, 0)
    return root


def build_collar(M, coll, x):
    parts = []
    band = ring_torus("SM_Collar_Band", major=0.075, minor=0.012,
                      location=(0, 0, 0.075), coll=coll, rot=(D2R(24), 0, 0),
                      major_seg=28, minor_seg=8)
    set_material(band, M["leather"])
    parts.append(band)
    tag = box("SM_Collar_Tag", (0.045, 0.010, 0.060),
              location=(0, -0.078, 0.062), rot=(D2R(12), 0, 0),
              bevel=0.008, segments=2, smooth=True)
    set_material(tag, M["brass"])
    parts.append(tag)
    for p in parts:
        apply_all(p)
    joined = smooth_join(parts, "SM_Collar")
    root = bpy.data.objects.new("SM_Collar", None)
    coll.objects.link(root)
    joined.parent = root
    root.location = (x, 0, 0)
    return root


def build_bells(M, coll, x):
    parts = []
    for i, (bx, by) in enumerate(((0, 0), (0.09, 0.03), (-0.07, 0.05))):
        b = lathe("SM_Bell_%d" % i,
                  [(0.0, 0.0), (0.030, 0.004), (0.050, 0.040),
                   (0.060, 0.082), (0.050, 0.118), (0.0, 0.126)],
                  segments=16, coll=coll,
                  location=(bx, by, 0.02 + i * 0.002),
                  )
        set_material(b, M["brass"])
        parts.append(b)
    for p in parts:
        apply_all(p)
    joined = smooth_join(parts, "SM_Prop_Bells")
    root = bpy.data.objects.new("SM_Prop_Bells", None)
    coll.objects.link(root)
    joined.parent = root
    root.location = (x, 0, 0)
    joined.location = (0, 0, 0)
    return root


def build_meteor(M, coll, x, size=1.0, seed=1):
    random.seed(seed)
    rock = ellipsoid("SM_Meteor_Rock", (0.55 * size, 0.50 * size, 0.46 * size),
                     location=(0, 0, 0.46 * size), coll=coll,
                     segments=16, rings=12)
    deform(rock, lambda co: (co.x + 0.10 * math.sin(co.y * 7 + co.x * 5),
                             co.y + 0.09 * math.cos(co.z * 6 + co.x * 4),
                             co.z + 0.08 * math.sin(co.x * 8 + co.y * 3)))
    subsurf(rock, 1)
    displace(rock, 0.10 * size, 0.16 * size)
    set_material(rock, M["obsidian"])
    parts = [rock]

    veins = []
    for i in range(3):
        a = random.uniform(0, TAU)
        verts = []
        for s in range(6):
            t = s / 5.0
            r = (0.600 * size) * (1.0 + 0.05 * math.sin(t * 7 + i))
            verts.append((math.cos(a + t * 1.3) * r,
                          math.sin(a + t * 1.3) * r,
                          0.10 * size + t * 0.80 * size))
        v = tube("SM_Meteor_Vein_%d" % i, verts, radius=0.022 * size,
                 segments=6, coll=coll)
        set_material(v, M["vein"])
        veins.append(v)

    for p in parts:
        apply_all(p)
    joined = smooth_join(parts, "SM_Meteor_%d" % seed)
    root = bpy.data.objects.new("SM_Meteor_%d" % seed, None)
    coll.objects.link(root)
    joined.parent = root
    root.location = (x, 0, 0)
    return root


def build():
    scene = reset_scene()
    M = materials()
    coll = collection("SM_Props")

    build_weapon_crate(M, coll, -3.2)
    build_ammo_box(M, coll, -1.9)
    build_helmet_kit(M, coll, -1.1, )
    build_shield(M, coll, 0.1)
    build_collar(M, coll, 1.0)
    build_bells(M, coll, 1.7)
    build_meteor(M, coll, 3.0, size=1.0, seed=1)
    build_meteor(M, coll, 4.4, size=0.62, seed=2)
    build_meteor(M, coll, 5.5, size=1.35, seed=3)

    report([o for o in bpy.data.objects if o.type == 'MESH'])
    studio()
    scene.render.engine = 'BLENDER_EEVEE'
    save(os.path.join(ROOT, "Props", "SM_Props.blend"))


build()
