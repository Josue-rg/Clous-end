"""
CLOUS-END · Armas (v1)

Carbón · Nogal · Estrella · Cascabel
Objetos de taller con acentos Yermos (obsidiana + vetas de luz).
1 unidad = 1 m. Cañón hacia -Y. Sin rig.
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
cylinder = g["cylinder"]
cone = g["cone"]
ring_torus = g["ring_torus"]
star = g["star"]
loft = g["loft"]
material = g["material"]
palette = g["palette"]
set_material = g["set_material"]
subsurf = g["subsurf"]
bevel = g["bevel"]
solidify = g["solidify"]
apply_all = g["apply_all"]
smooth_join = g["smooth_join"]
studio = g["studio"]
save = g["save"]
report = g["report"]
D2R = g["D2R"]
TAU = g["TAU"]

ROOT = r"C:\Users\Josue\Desktop\Clous-end\BlenderSource"


def make_materials():
    M = {}
    M["wood"] = g["palette"]()['wood']
    M["iron"] = material("M_Gun_Iron", (0.075, 0.075, 0.082, 1), rough=0.42,
                         metal=1.0,
                         noise=dict(scale=45, detail=6, bump=0.22,
                                    color1=(0.045, 0.045, 0.050, 1),
                                    color2=(0.115, 0.115, 0.125, 1)))
    M["brass"] = material("M_Gun_Brass", (0.55, 0.40, 0.16, 1), rough=0.30,
                          metal=1.0,
                          noise=dict(scale=38, detail=5, bump=0.18,
                                     color1=(0.40, 0.28, 0.10, 1),
                                     color2=(0.68, 0.52, 0.24, 1)))
    M["copper"] = material("M_Gun_Copper", (0.52, 0.24, 0.10, 1), rough=0.28,
                           metal=1.0)
    M["walnut"] = material("M_Gun_Walnut", (0.145, 0.075, 0.035, 1), rough=0.35,
                           noise=dict(scale=16, detail=9, bump=0.4,
                                      mapping=(2, 4, 24),
                                      color1=(0.095, 0.048, 0.022, 1),
                                      color2=(0.215, 0.118, 0.058, 1)))
    M["leather"] = material("M_Gun_Leather", (0.115, 0.070, 0.042, 1), rough=0.55)
    M["coal"] = material("M_Coal", (0.022, 0.020, 0.020, 1), rough=0.55,
                         noise=dict(scale=90, detail=6, bump=0.5,
                                    color1=(0.010, 0.010, 0.010, 1),
                                    color2=(0.055, 0.050, 0.048, 1)))
    M["ember"] = material("M_Ember", (1.0, 0.42, 0.10, 1), rough=0.4,
                          emission=(1.0, 0.38, 0.08, 1), emission_strength=9.0)
    M["star"] = material("M_Star_Emitter", (1.0, 0.95, 0.78, 1), rough=0.2,
                         emission=(1.0, 0.90, 0.62, 1), emission_strength=18.0)
    M["glass"] = material("M_Gun_Glass", (0.60, 0.72, 0.70, 1), rough=0.08,
                          alpha=0.30, spec=1.0)
    M["obsidian"] = material("M_Gun_Obsidian", (0.014, 0.014, 0.018, 1), rough=0.16,
                             metal=0.35, coat=0.6,
                             noise=dict(scale=11, detail=6, bump=0.25,
                                        color1=(0.006, 0.006, 0.009, 1),
                                        color2=(0.030, 0.030, 0.038, 1)))
    M["vein"] = material("M_Gun_Vein", (0.80, 0.92, 1.0, 1), rough=0.25,
                         emission=(0.72, 0.88, 1.0, 1), emission_strength=10.0)
    return M


def ring_y(name, y, z=0.0, x=0.0, major=0.05, minor=0.012, mat=None,
           coll=None, rot_extra=0.0):
    obj = ring_torus(name, major=major, minor=minor, location=(x, y, z),
                     coll=coll, rot=(D2R(90), 0, rot_extra),
                     major_seg=20, minor_seg=8)
    return obj


def bar(name, y0, y1, r0, r1, z=0.0, x=0.0, seg=16, coll=None):
    return tube(name, [(x, y0, z), (x, y1, z)], radius=[r0, r1],
                segments=seg, coll=coll)


def shard(name, base, size=1.0, rot=(0, 0, 0), coll=None):
    """Cristal de obsidiana facetado."""
    obj = cone(name, r1=0.030 * size, r2=0.004 * size, depth=0.16 * size,
               location=base, coll=coll, segments=4, smooth=False)
    obj.rotation_euler = rot
    return obj


# ---------------------------------------------------------------------------
# CARBON · fusil automatico de laton con tolva de carbon
# ---------------------------------------------------------------------------

def build_carbon(M, coll):
    parts = []

    def add(o, m):
        set_material(o, m)
        parts.append(o)
        return o

    add(box("SM_Carbon_Receiver", (0.105, 0.34, 0.135),
            location=(0, 0.08, 0.0), bevel=0.016, segments=2, smooth=True), M["brass"])
    add(box("SM_Carbon_ReceiverPlate", (0.112, 0.10, 0.075),
            location=(0, -0.02, 0.03), bevel=0.01, segments=2, smooth=True), M["iron"])
    add(bar("SM_Carbon_Barrel", -0.06, -0.74, 0.030, 0.024, z=0.02), M["iron"])
    add(tube("SM_Carbon_Shroud", [(0, -0.06, 0.02), (0, -0.44, 0.02)],
             radius=[0.052, 0.044], segments=14), M["brass"])
    for i, y in enumerate((-0.14, -0.24, -0.34)):
        add(ring_y("SM_Carbon_Vent_%d" % i, y, z=0.02, major=0.050, minor=0.008), M["iron"])
    add(ring_y("SM_Carbon_Muzzle", -0.73, z=0.02, major=0.030, minor=0.011), M["brass"])

    # tolva de carbon
    hopper = lathe("SM_Carbon_Hopper",
                   [(0.0, 0.0), (0.052, 0.006), (0.086, 0.10), (0.096, 0.16),
                    (0.088, 0.175), (0.0, 0.178)],
                   segments=20, coll=coll, location=(0, 0.02, 0.11))
    add(hopper, M["brass"])
    add(ellipsoid("SM_Carbon_Coal", (0.072, 0.072, 0.030),
                  location=(0, 0.02, 0.262), coll=coll, segments=16, rings=8), M["coal"])
    add(ellipsoid("SM_Carbon_Ember", (0.030, 0.030, 0.016),
                  location=(0, 0.02, 0.268), coll=coll, segments=12, rings=6), M["ember"])
    add(ring_y("SM_Carbon_HopperRim", 0.02, z=0.278, major=0.070, minor=0.010), M["iron"])

    # culata
    stock = loft("SM_Carbon_Stock",
                 [[(-0.048, 0.24, 0.05), (0.048, 0.24, 0.05),
                   (0.048, 0.24, -0.05), (-0.048, 0.24, -0.05)],
                  [(-0.044, 0.40, 0.055), (0.044, 0.40, 0.055),
                   (0.044, 0.40, -0.02), (-0.044, 0.40, -0.02)],
                  [(-0.038, 0.56, 0.075), (0.038, 0.56, 0.075),
                   (0.038, 0.56, -0.005), (-0.038, 0.56, -0.005)],
                  [(-0.030, 0.66, 0.105), (0.030, 0.66, 0.105),
                   (0.030, 0.66, 0.045), (-0.030, 0.66, 0.045)]],
                 coll=coll, close=True, cap_start=True, cap_end=True)
    add(stock, M["wood"])
    add(box("SM_Carbon_Butt", (0.062, 0.028, 0.075),
            location=(0, 0.672, 0.075), bevel=0.012, segments=2, smooth=True), M["brass"])

    # empunadura y gatillo
    add(box("SM_Carbon_Grip", (0.048, 0.075, 0.17),
            location=(0, 0.24, -0.10), rot=(D2R(-16), 0, 0),
            bevel=0.016, segments=3, smooth=True), M["walnut"])
    add(ring_torus("SM_Carbon_Guard", major=0.048, minor=0.008,
                   location=(0, 0.155, -0.075), coll=coll,
                   rot=(D2R(90), 0, 0), major_seg=16, minor_seg=6), M["iron"])
    add(box("SM_Carbon_Trigger", (0.012, 0.030, 0.045),
            location=(0, 0.16, -0.075), rot=(D2R(12), 0, 0)), M["iron"])

    # manometro
    add(cylinder("SM_Carbon_Gauge", radius=0.035, depth=0.016,
                 location=(0.058, 0.10, 0.02), rot=(0, D2R(90), 0),
                 segments=16), M["brass"])
    add(cylinder("SM_Carbon_GaugeFace", radius=0.028, depth=0.004,
                 location=(0.068, 0.10, 0.02), rot=(0, D2R(90), 0),
                 segments=16), M["ember"])

    # acento yermo: cristal en el cañon
    add(shard("SM_Carbon_Shard", (0.055, -0.30, 0.055), size=1.0,
              rot=(D2R(20), D2R(24), 0), coll=coll), M["obsidian"])
    add(tube("SM_Carbon_Vein", [(0.055, -0.30, 0.055), (0.052, -0.31, 0.115),
                                (0.046, -0.30, 0.155)],
             radius=0.005, segments=6), M["vein"])

    # cascabel colgante
    bell = lathe("SM_Carbon_Bell",
                 [(0.0, 0.0), (0.020, 0.003), (0.032, 0.026), (0.038, 0.055),
                  (0.032, 0.075), (0.0, 0.080)],
                 segments=12, coll=coll, location=(0.052, 0.30, -0.06))
    add(bell, M["brass"])
    return parts


# ---------------------------------------------------------------------------
# NOGAL · escopeta de dos cañones
# ---------------------------------------------------------------------------

def build_nogal(M, coll):
    parts = []

    def add(o, m):
        set_material(o, m)
        parts.append(o)
        return o

    for sx, tag in ((-1, "L"), (1, "R")):
        add(bar("SM_Nogal_Barrel_%s" % tag, -0.10, -0.80, 0.036, 0.032,
                z=0.02, x=sx * 0.040), M["iron"])
        add(ring_y("SM_Nogal_Muzzle_%s" % tag, -0.80, z=0.02, x=sx * 0.040,
                   major=0.034, minor=0.010), M["brass"])
        add(shard("SM_Nogal_Shard_%s" % tag, (sx * 0.040, -0.52, 0.058),
                  size=0.7, rot=(D2R(14), D2R(sx * -18), 0), coll=coll), M["obsidian"])

    add(box("SM_Nogal_BarrelRib", (0.098, 0.70, 0.030),
            location=(0, -0.44, 0.008), bevel=0.008, segments=2, smooth=True), M["iron"])
    add(box("SM_Nogal_Forend", (0.112, 0.26, 0.085),
            location=(0, -0.30, -0.045), bevel=0.024, segments=3, smooth=True), M["walnut"])

    add(box("SM_Nogal_Receiver", (0.115, 0.26, 0.115),
            location=(0, 0.02, 0.0), bevel=0.014, segments=2, smooth=True), M["iron"])
    add(cylinder("SM_Nogal_Hinge", radius=0.020, depth=0.13,
                 location=(0, -0.10, -0.02), rot=(0, D2R(90), 0), segments=12), M["brass"])
    add(box("SM_Nogal_TopLever", (0.020, 0.11, 0.016),
            location=(0.030, 0.05, 0.062), rot=(0, 0, D2R(-10)),
            bevel=0.005, smooth=True), M["brass"])

    stock = loft("SM_Nogal_Stock",
                 [[(-0.052, 0.14, 0.048), (0.052, 0.14, 0.048),
                   (0.052, 0.14, -0.052), (-0.052, 0.14, -0.052)],
                  [(-0.048, 0.30, 0.052), (0.048, 0.30, 0.052),
                   (0.048, 0.30, -0.030), (-0.048, 0.30, -0.030)],
                  [(-0.042, 0.46, 0.072), (0.042, 0.46, 0.072),
                   (0.042, 0.46, -0.005), (-0.042, 0.46, -0.005)],
                  [(-0.032, 0.58, 0.098), (0.032, 0.58, 0.098),
                   (0.032, 0.58, 0.038), (-0.032, 0.58, 0.038)]],
                 coll=coll, close=True, cap_start=True, cap_end=True)
    add(stock, M["walnut"])
    add(box("SM_Nogal_Butt", (0.068, 0.022, 0.070),
            location=(0, 0.592, 0.068), bevel=0.012, segments=2, smooth=True), M["brass"])

    add(box("SM_Nogal_Grip", (0.050, 0.070, 0.16),
            location=(0, 0.175, -0.115), rot=(D2R(-18), 0, 0),
            bevel=0.016, segments=3, smooth=True), M["walnut"])
    add(ring_torus("SM_Nogal_Guard", major=0.052, minor=0.008,
                   location=(0, 0.095, -0.085), coll=coll,
                   rot=(D2R(90), 0, 0), major_seg=16, minor_seg=6), M["brass"])
    for sx in (-1, 1):
        add(box("SM_Nogal_Trigger", (0.010, 0.026, 0.040),
                location=(sx * 0.014, 0.105, -0.082), rot=(D2R(10), 0, 0)), M["iron"])

    # cartuchos en la culata
    for i in range(4):
        y = 0.30 + i * 0.055
        add(cylinder("SM_Nogal_Shell_%d" % i, radius=0.016, depth=0.062,
                     location=(0.052, y, 0.020), rot=(D2R(90), 0, 0),
                     segments=10), M["brass"])

    # correa
    strap = tube("SM_Nogal_Sling",
                 [(-0.060, -0.24, -0.080), (-0.075, 0.02, -0.115),
                  (-0.070, 0.34, -0.075), (-0.045, 0.50, 0.010)],
                 radius=0.012, segments=6, coll=coll)
    add(strap, M["leather"])

    # cascabel en el guardamonte
    add(lathe("SM_Nogal_Bell",
              [(0.0, 0.0), (0.016, 0.002), (0.026, 0.020), (0.030, 0.042),
               (0.026, 0.058), (0.0, 0.062)],
              segments=12, coll=coll, location=(0, 0.095, -0.135)), M["brass"])
    return parts


# ---------------------------------------------------------------------------
# ESTRELLA · railgun de bobinas con farol de mira
# ---------------------------------------------------------------------------

def build_estrella(M, coll):
    parts = []

    def add(o, m):
        set_material(o, m)
        parts.append(o)
        return o

    add(box("SM_Estrella_Body", (0.080, 0.42, 0.105),
            location=(0, 0.12, 0.0), bevel=0.014, segments=2, smooth=True), M["brass"])
    add(bar("SM_Estrella_Barrel", -0.06, -1.18, 0.024, 0.018, z=0.02), M["iron"])
    add(tube("SM_Estrella_Rail", [(0, -0.10, 0.055), (0, -1.12, 0.055)],
             radius=[0.014, 0.011], segments=8), M["copper"])

    # bobinas
    for i in range(6):
        y = -0.18 - i * 0.155
        add(ring_y("SM_Estrella_Coil_%d" % i, y, z=0.02, major=0.040,
                   minor=0.013), M["copper"])
        add(tube("SM_Estrella_CoilWire_%d" % i,
                 [(0.030, y, 0.055), (0.048, y + 0.03, 0.075),
                  (0.030, y + 0.06, 0.055)],
                 radius=0.005, segments=5), M["copper"])

    # camara de vacio con nucleo
    add(cylinder("SM_Estrella_Chamber", radius=0.052, depth=0.10,
                 location=(0, -0.02, 0.0), rot=(D2R(90), 0, 0), segments=20), M["glass"])
    add(ellipsoid("SM_Estrella_Core", (0.028, 0.030, 0.028),
                  location=(0, -0.02, 0.0), coll=coll, segments=14, rings=8), M["star"])
    add(ring_torus("SM_Estrella_ChamberRing", major=0.054, minor=0.010,
                   location=(0, 0.03, 0.0), coll=coll, rot=(D2R(90), 0, 0),
                   major_seg=20, minor_seg=6), M["brass"])

    # farol de mira
    add(box("SM_Estrella_SightPost", (0.014, 0.014, 0.055),
            location=(0, -0.06, 0.085), bevel=0.003, smooth=True), M["brass"])
    add(lathe("SM_Estrella_Lantern",
              [(0.0, -0.05), (0.030, -0.048), (0.040, -0.02), (0.042, 0.02),
               (0.030, 0.046), (0.0, 0.048)],
              segments=12, coll=coll, location=(0, -0.06, 0.145)), M["glass"])
    add(ellipsoid("SM_Estrella_Flame", (0.017, 0.017, 0.022),
                  location=(0, -0.06, 0.145), coll=coll, segments=10, rings=6), M["ember"])
    add(ring_torus("SM_Estrella_LanternRim", major=0.040, minor=0.006,
                   location=(0, -0.06, 0.190), coll=coll,
                   major_seg=12, minor_seg=6), M["brass"])

    # emisor en estrella
    add(star("SM_Estrella_Muzzle", outer=0.075, inner=0.030, depth=0.030,
             points=6, coll=coll, location=(0, -1.20, 0.02),
             rot=(D2R(90), 0, 0), phase=D2R(30)), M["star"])
    add(ring_y("SM_Estrella_MuzzleRing", -1.19, z=0.02, major=0.048,
               minor=0.012), M["brass"])

    # culata y empunadura
    stock = loft("SM_Estrella_Stock",
                 [[(-0.042, 0.30, 0.045), (0.042, 0.30, 0.045),
                   (0.042, 0.30, -0.050), (-0.042, 0.30, -0.050)],
                  [(-0.038, 0.46, 0.052), (0.038, 0.46, 0.052),
                   (0.038, 0.46, -0.028), (-0.038, 0.46, -0.028)],
                  [(-0.032, 0.62, 0.080), (0.032, 0.62, 0.080),
                   (0.032, 0.62, 0.020), (-0.032, 0.62, 0.020)]],
                 coll=coll, close=True, cap_start=True, cap_end=True)
    add(stock, M["walnut"])
    add(box("SM_Estrella_Grip", (0.045, 0.065, 0.165),
            location=(0, 0.245, -0.115), rot=(D2R(-20), 0, 0),
            bevel=0.014, segments=3, smooth=True), M["walnut"])
    add(ring_torus("SM_Estrella_Guard", major=0.046, minor=0.007,
                   location=(0, 0.165, -0.085), coll=coll,
                   rot=(D2R(90), 0, 0), major_seg=14, minor_seg=6), M["brass"])

    # cables
    add(tube("SM_Estrella_Cable",
             [(0.045, -0.60, 0.055), (0.075, -0.40, 0.02),
              (0.062, -0.18, -0.02), (0.035, -0.05, -0.03)],
             radius=0.008, segments=6), M["copper"])

    # acento yermo
    add(shard("SM_Estrella_Shard", (-0.048, -0.30, 0.045), size=0.8,
              rot=(D2R(-16), D2R(-26), 0), coll=coll), M["obsidian"])
    add(tube("SM_Estrella_Vein", [(-0.048, -0.30, 0.045), (-0.046, -0.31, 0.095),
                                  (-0.040, -0.30, 0.125)],
             radius=0.004, segments=6), M["vein"])
    return parts


# ---------------------------------------------------------------------------
# CASCABEL · lanzador de cascabeles con tambor
# ---------------------------------------------------------------------------

def build_cascabel(M, coll):
    parts = []

    def add(o, m):
        set_material(o, m)
        parts.append(o)
        return o

    add(box("SM_Cascabel_Receiver", (0.092, 0.26, 0.120),
            location=(0, 0.06, 0.0), bevel=0.018, segments=2, smooth=True), M["brass"])
    add(box("SM_Cascabel_Frame", (0.100, 0.16, 0.100),
            location=(0, -0.10, -0.01), bevel=0.010, segments=2, smooth=True), M["iron"])

    # boca de campana
    bell = lathe("SM_Cascabel_Bell",
                 [(0.0, 0.0), (0.048, 0.004), (0.070, 0.02), (0.096, 0.06),
                  (0.116, 0.10), (0.120, 0.115), (0.108, 0.120)],
                 segments=24, coll=coll, location=(0, -0.18, -0.005))
    bell.rotation_euler = (D2R(90), 0, 0)
    add(bell, M["brass"])
    add(ring_y("SM_Cascabel_BellRim", -0.298, z=-0.005, major=0.112,
               minor=0.014), M["iron"])
    add(cylinder("SM_Cascabel_Throat", radius=0.048, depth=0.10,
                 location=(0, -0.16, -0.005), rot=(D2R(90), 0, 0),
                 segments=16), M["iron"])

    # tambor de cascabeles
    add(cylinder("SM_Cascabel_Drum", radius=0.088, depth=0.075,
                 location=(0, 0.02, 0.015), rot=(0, D2R(90), 0),
                 segments=24), M["brass"])
    add(ring_torus("SM_Cascabel_DrumHoopL", major=0.088, minor=0.010,
                   location=(-0.038, 0.02, 0.015), coll=coll,
                   rot=(0, D2R(90), 0), major_seg=20, minor_seg=6), M["iron"])
    add(ring_torus("SM_Cascabel_DrumHoopR", major=0.088, minor=0.010,
                   location=(0.038, 0.02, 0.015), coll=coll,
                   rot=(0, D2R(90), 0), major_seg=20, minor_seg=6), M["iron"])
    for i in range(6):
        a = D2R(i * 60)
        bell_small = lathe("SM_Cascabel_Bell_%d" % i,
                           [(0.0, 0.0), (0.014, 0.002), (0.022, 0.018),
                            (0.026, 0.040), (0.022, 0.055), (0.0, 0.058)],
                           segments=10, coll=coll)
        bell_small.rotation_euler = (D2R(90), 0, 0)
        bell_small.location = (0.040, 0.02 + math.cos(a) * 0.062,
                               0.015 + math.sin(a) * 0.062)
        add(bell_small, M["brass"])

    # cuna y culata
    add(box("SM_Cascabel_Grip", (0.046, 0.070, 0.16),
            location=(0, 0.24, -0.115), rot=(D2R(-18), 0, 0),
            bevel=0.016, segments=3, smooth=True), M["walnut"])
    add(box("SM_Cascabel_Stock", (0.062, 0.20, 0.09),
            location=(0, 0.34, -0.045), rot=(D2R(-8), 0, 0),
            bevel=0.018, segments=2, smooth=True), M["walnut"])
    add(ring_torus("SM_Cascabel_Guard", major=0.048, minor=0.008,
                   location=(0, 0.165, -0.085), coll=coll,
                   rot=(D2R(90), 0, 0), major_seg=14, minor_seg=6), M["brass"])

    # manivela
    add(cylinder("SM_Cascabel_CrankHub", radius=0.022, depth=0.040,
                 location=(0.070, 0.02, 0.015), rot=(0, D2R(90), 0),
                 segments=12), M["iron"])
    add(tube("SM_Cascabel_CrankArm",
             [(0.092, 0.02, 0.015), (0.092, 0.02, 0.100)],
             radius=0.010, segments=8), M["iron"])
    add(cylinder("SM_Cascabel_CrankGrip", radius=0.016, depth=0.055,
                 location=(0.092, 0.02, 0.130), rot=(0, D2R(90), 0),
                 segments=10), M["walnut"])

    # runa luminosa
    add(star("SM_Cascabel_Rune", outer=0.042, inner=0.017, depth=0.010,
             points=5, coll=coll, location=(0, 0.075, 0.068),
             rot=(D2R(90), 0, 0)), M["star"])
    add(ring_y("SM_Cascabel_RuneRing", 0.075, z=0.068, major=0.052,
               minor=0.008), M["vein"])

    # acento yermo
    add(shard("SM_Cascabel_Shard", (0.058, -0.10, 0.035), size=0.65,
              rot=(D2R(18), D2R(28), 0), coll=coll), M["obsidian"])
    return parts


def assemble(name, builder, M, coll, x, rot_z, tilt):
    parts = builder(M, coll)
    for p in parts:
        apply_all(p)
    joined = smooth_join(parts, name)
    root = bpy.data.objects.new(name, None)
    root.empty_display_type = 'PLAIN_AXES'
    root.empty_display_size = 0.15
    coll.objects.link(root)
    joined.parent = root
    joined.location = (0, 0, 0)
    root.location = (x, 0.0, 0.62)
    root.rotation_euler = (D2R(tilt), 0, D2R(rot_z))
    return root


def build():
    scene = reset_scene()
    M = make_materials()
    coll = collection("SM_Weapons")
    assemble("SM_Weapon_Carbon", build_carbon, M, coll, -2.35, 16, 6)
    assemble("SM_Weapon_Nogal", build_nogal, M, coll, -0.80, 10, 4)
    assemble("SM_Weapon_Estrella", build_estrella, M, coll, 0.85, 14, 5)
    assemble("SM_Weapon_Cascabel", build_cascabel, M, coll, 2.30, 18, 6)

    report([o for o in bpy.data.objects if o.type == 'MESH'])
    studio()
    scene.render.engine = 'BLENDER_EEVEE'
    save(os.path.join(ROOT, "Weapons", "SM_Weapons.blend"))


build()
