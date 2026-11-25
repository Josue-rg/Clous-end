"""
CLOUS-END · SM_Sleigh  (v2)

Trineo de madera y hierro, reliquia familiar reforzada. 6,5 x 2,2 m.
1 unidad = 1 m · mira hacia -Y. Sin rig.
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
loft = g["loft"]
sweep_rect = g["sweep_rect"]
star = g["star"]
ring_torus = g["ring_torus"]
cone = g["cone"]
material = g["material"]
palette = g["palette"]
set_material = g["set_material"]
subsurf = g["subsurf"]
bevel = g["bevel"]
solidify = g["solidify"]
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
U_SEG = 21

# (y, medio ancho, z del borde, profundidad)
HULL = [
    (-2.06, 0.24, 1.30, 0.14),
    (-1.88, 0.47, 1.34, 0.33),
    (-1.58, 0.73, 1.29, 0.49),
    (-1.12, 0.89, 1.19, 0.57),
    (-0.62, 0.99, 1.14, 0.61),
    (-0.10, 1.03, 1.10, 0.62),
    (0.40, 1.04, 1.09, 0.62),
    (0.95, 1.02, 1.09, 0.61),
    (1.50, 0.98, 1.11, 0.58),
    (2.05, 0.91, 1.15, 0.53),
    (2.55, 0.80, 1.21, 0.45),
    (2.88, 0.65, 1.26, 0.33),
    (3.03, 0.45, 1.29, 0.19),
]


def hull_at(y):
    for i in range(len(HULL) - 1):
        y0, w0, t0, d0 = HULL[i]
        y1, w1, t1, d1 = HULL[i + 1]
        if y0 <= y <= y1:
            t = (y - y0) / (y1 - y0)
            return (w0 + (w1 - w0) * t, t0 + (t1 - t0) * t, d0 + (d1 - d0) * t)
    if y < HULL[0][0]:
        return HULL[0][1:]
    return HULL[-1][1:]


def usection(y, expand=0.0):
    hw, ztop, depth = hull_at(y)
    hw += expand
    ztop += expand * 0.35
    pts = []
    for i in range(U_SEG):
        a = math.pi * (1.0 - i / (U_SEG - 1))
        x = hw * math.cos(a)
        s = max(0.0, math.sin(a))
        z = ztop - depth * (s ** 0.62)
        pts.append((x, y, z))
    return pts


def build():
    scene = reset_scene()
    P = palette()
    coll = collection("SM_Sleigh")
    parts = []

    wood = P['wood']
    iron = P['iron']
    brass = P['brass']
    leather = P['leather']
    canvas = material("M_Sleigh_Canvas", (0.26, 0.175, 0.115, 1), rough=0.92,
                      noise=dict(scale=42, detail=7, bump=0.30,
                                 color1=(0.19, 0.125, 0.082, 1),
                                 color2=(0.34, 0.24, 0.16, 1)))
    paint = material("M_Sleigh_Paint_Green", (0.042, 0.080, 0.060, 1), rough=0.55,
                     noise=dict(scale=28, detail=6, bump=0.22,
                                color1=(0.028, 0.055, 0.042, 1),
                                color2=(0.070, 0.110, 0.080, 1)))
    ember = P['warm_light']
    star_mat = material("M_StarHeart", (1.0, 0.93, 0.72, 1), rough=0.2,
                        emission=(1.0, 0.86, 0.55, 1), emission_strength=16.0)

    def add(obj, mat, name=None):
        if name:
            obj.name = name
            obj.data.name = name
        set_material(obj, mat)
        parts.append(obj)
        return obj

    # ================================================================ CASCO
    sections = [usection(row[0]) for row in HULL]
    hull = loft("SM_Sleigh_Hull", sections, coll=coll, smooth=True)
    solidify(hull, 0.042, 0)
    subsurf(hull, 1)
    displace(hull, 0.004, 0.16)
    add(hull, wood)

    # ================================================================ COSTADOS
    side_panels = []
    for sx in (-1, 1):
        pts_top, pts_bot = [], []
        for y in (-1.60, -1.10, -0.60, -0.10, 0.40, 0.95, 1.50, 2.02, 2.48):
            hw, ztop, depth = hull_at(y)
            pts_top.append((sx * (hw + 0.024), y, ztop - 0.085))
            pts_bot.append((sx * (hw + 0.024), y, ztop - depth * 0.62))
        panel = loft("SM_Sleigh_Panel", [pts_bot, pts_top], coll=coll,
                     smooth=True)
        solidify(panel, 0.014, 0)
        subsurf(panel, 1)
        side_panels.append(panel)
    for p in side_panels:
        apply_all(p)
    panels = smooth_join(side_panels, "SM_Sleigh_Panels")
    add(panels, paint)

    # ================================================================ ZUNCHOS
    band_parts = []
    for y in (-1.30, -0.35, 0.75, 1.75, 2.55):
        sec = [usection(y - 0.055, expand=0.010),
               usection(y + 0.055, expand=0.010)]
        band = loft("SM_Sleigh_Band", sec, coll=coll, smooth=True)
        solidify(band, 0.024, 0)
        band_parts.append(band)
    for b in band_parts:
        apply_all(b)
    bands = smooth_join(band_parts, "SM_Sleigh_Bands")
    add(bands, iron)

    rivets = []
    for y in (-1.30, -0.35, 0.75, 1.75, 2.55):
        hw, ztop, depth = hull_at(y)
        for t in (0.16, 0.30, 0.44, 0.58, 0.72, 0.86):
            for sx in (-1, 1):
                x = sx * hw * math.cos(math.pi * t)
                z = ztop - depth * (math.sin(math.pi * t) ** 0.62)
                rivets.append(ellipsoid("SM_Sleigh_Rivet", (0.014, 0.014, 0.014),
                                        location=(x * 1.02, y + 0.062, z),
                                        coll=coll, segments=10, rings=6))
    for rv in rivets:
        apply_all(rv)
    rivet_obj = smooth_join(rivets, "SM_Sleigh_Rivets")
    add(rivet_obj, brass)

    # ================================================================ COSTILLAS
    rib_parts = []
    for y in (-1.45, -0.55, 0.60, 1.60, 2.30):
        sec = [usection(y - 0.030, expand=-0.030),
               usection(y + 0.030, expand=-0.030)]
        rib = loft("SM_Sleigh_Rib", sec, coll=coll, smooth=True)
        rib_parts.append(rib)
    for r in rib_parts:
        apply_all(r)
    ribs = smooth_join(rib_parts, "SM_Sleigh_Ribs")
    add(ribs, wood)

    # ================================================================ SUELO
    floor = box("SM_Sleigh_Deck", (1.62, 3.60, 0.045),
                location=(0, 0.62, 0.545), coll=coll, bevel=0.012,
                segments=2, smooth=True)
    add(floor, wood)
    for i in range(5):
        plank = box("SM_Sleigh_Plank_%d" % i, (1.58, 0.028, 0.014),
                    location=(0, -1.10 + i * 0.85, 0.572), coll=coll,
                    bevel=0.005, segments=2, smooth=True)
        add(plank, iron)

    # ================================================================ PATINES
    runner_path = [
        (3.02, 0.14), (2.60, 0.10), (2.00, 0.085), (1.20, 0.075),
        (0.20, 0.070), (-0.80, 0.072), (-1.60, 0.082), (-2.15, 0.108),
        (-2.48, 0.170), (-2.70, 0.300), (-2.82, 0.480), (-2.80, 0.660),
        (-2.66, 0.790), (-2.42, 0.850), (-2.12, 0.850),
    ]
    runner_objs = []
    for sx in (-1, 1):
        path = [(sx * 0.862, y, z) for y, z in runner_path]
        r = sweep_rect("SM_Sleigh_Runner", path, width=0.058, height=0.165,
                       coll=coll)
        subsurf(r, 1)
        runner_objs.append(r)
    for r in runner_objs:
        apply_all(r)
    runners = smooth_join(runner_objs, "SM_Sleigh_Runners")
    add(runners, iron)

    # ================================================================ SUSPENSION
    struts = []
    for y in (-1.45, -0.10, 1.25, 2.20):
        hw, ztop, depth = hull_at(y)
        z_bot = ztop - depth + 0.02
        for sx in (-1, 1):
            post = box("SM_Sleigh_Strut", (0.072, 0.072, z_bot - 0.17),
                       location=(sx * 0.862, y, (z_bot + 0.17) * 0.5),
                       coll=coll, bevel=0.012, segments=2, smooth=True)
            struts.append(post)
            leaf = box("SM_Sleigh_Leaf", (0.048, 0.54, 0.032),
                       location=(sx * 0.862, y + 0.10, 0.225),
                       coll=coll, bevel=0.010, segments=2, smooth=True)
            struts.append(leaf)
            brace = box("SM_Sleigh_Brace", (0.042, 0.26, 0.042),
                        location=(sx * 0.862, y + 0.16, (z_bot + 0.34) * 0.5),
                        coll=coll, rot=(D2R(34), 0, 0))
            struts.append(brace)
    for s in struts:
        apply_all(s)
    strut_join = smooth_join(struts, "SM_Sleigh_Struts")
    add(strut_join, iron)

    # ================================================================ ASIENTO
    seat = box("SM_Sleigh_Seat", (1.70, 0.88, 0.11),
               location=(0, 0.62, 0.945), coll=coll, bevel=0.026, segments=3,
               smooth=True)
    add(seat, wood)
    cushion = box("SM_Sleigh_Cushion", (1.62, 0.80, 0.090),
                  location=(0, 0.62, 1.020), coll=coll, bevel=0.042,
                  segments=4, smooth=True)
    add(cushion, leather)

    back = box("SM_Sleigh_Backrest", (1.76, 0.16, 1.00),
               location=(0, 1.16, 1.020), coll=coll, rot=(D2R(-7), 0, 0),
               bevel=0.030, segments=3, smooth=True)
    add(back, wood)
    back_pad = box("SM_Sleigh_BackPad", (1.58, 0.10, 0.44),
                   location=(0, 1.062, 1.300), coll=coll, rot=(D2R(-7), 0, 0),
                   bevel=0.050, segments=4, smooth=True)
    add(back_pad, canvas)

    for sx in (-1, 1):
        post = box("SM_Sleigh_BackPost", (0.095, 0.20, 1.02),
                   location=(sx * 0.795, 1.16, 1.020), coll=coll,
                   rot=(D2R(-7), 0, 0), bevel=0.020, segments=2, smooth=True)
        add(post, wood)
        knob = ellipsoid("SM_Sleigh_BackKnob", (0.058, 0.058, 0.058),
                         location=(sx * 0.795, 1.095, 1.560), coll=coll,
                         segments=16, rings=8)
        subsurf(knob, 1)
        add(knob, brass)

    # ================================================================ CORAZON
    heart_y = -1.905
    heart_z = 1.430
    core = star("SM_Sleigh_StarHeart", outer=0.225, inner=0.098, depth=0.085,
                points=8, coll=coll, location=(0, heart_y, heart_z),
                rot=(D2R(90), 0, 0), phase=D2R(22.5))
    add(core, star_mat)

    housing = lathe("SM_Sleigh_StarHousing",
                    [(0.0, -0.05), (0.215, -0.048), (0.250, -0.02),
                     (0.252, 0.02), (0.215, 0.048), (0.0, 0.05)],
                    segments=32, coll=coll, location=(0, heart_y + 0.075,
                                                      heart_z))
    housing.rotation_euler = (D2R(-90), 0, 0)
    add(housing, iron)
    ring_cage = ring_torus("SM_Sleigh_StarRing", major=0.255, minor=0.024,
                           location=(0, heart_y + 0.010, heart_z),
                           coll=coll, rot=(D2R(90), 0, 0),
                           major_seg=32, minor_seg=10)
    add(ring_cage, brass)
    for i in range(4):
        a = D2R(45 + i * 90)
        spoke = box("SM_Sleigh_StarSpoke", (0.024, 0.026, 0.270),
                    location=(0, heart_y + 0.008, heart_z), coll=coll,
                    rot=(0, 0, a))
        add(spoke, brass)

    # ================================================================ BOLSA
    random.seed(3)
    sack = ellipsoid("SM_Sleigh_Sack", (0.61, 0.70, 0.44),
                     location=(0, 2.02, 1.09), coll=coll, segments=28, rings=16)
    deform(sack, lambda co: (co.x + 0.05 * math.sin(co.z * 9),
                             co.y + 0.04 * math.cos(co.x * 7),
                             co.z - 0.06 * max(0.0, co.z)))
    subsurf(sack, 1)
    displace(sack, 0.020, 0.10)
    add(sack, canvas)

    neck = lathe("SM_Sleigh_SackNeck",
                 [(0.0, 0.0), (0.19, 0.006), (0.225, 0.055),
                  (0.205, 0.105), (0.0, 0.118)],
                 segments=24, coll=coll, location=(0, 2.02, 1.44))
    subsurf(neck, 1)
    add(neck, canvas)
    tie = ring_torus("SM_Sleigh_SackTie", major=0.208, minor=0.026,
                     location=(0, 2.02, 1.505), coll=coll,
                     major_seg=28, minor_seg=8)
    add(tie, leather)

    for i, (px, py, pz, r) in enumerate(((-0.34, 1.74, 1.20, -18),
                                         (0.30, 2.26, 1.02, 22))):
        patch = box("SM_Sleigh_SackPatch_%d" % i, (0.19, 0.006, 0.16),
                    location=(px, py, pz), coll=coll, rot=(0, 0, D2R(r)),
                    bevel=0.004, smooth=True)
        add(patch, material("M_Sleigh_Patch", (0.18, 0.09, 0.07, 1),
                            rough=0.95))

    # ================================================================ FAROL
    lantern_y = -1.82
    lantern_post = box("SM_Sleigh_LanternPost", (0.072, 0.072, 0.40),
                       location=(0, lantern_y, 1.52), coll=coll,
                       bevel=0.014, segments=2, smooth=True)
    add(lantern_post, iron)
    lantern_body = lathe("SM_Sleigh_Lantern",
                         [(0.0, -0.105), (0.072, -0.102), (0.098, -0.06),
                          (0.106, 0.0), (0.098, 0.06), (0.072, 0.102),
                          (0.0, 0.105)],
                         segments=8, coll=coll, location=(0, lantern_y, 1.80))
    add(lantern_body, P['glass'])
    for i in range(4):
        a = D2R(i * 90 + 45)
        bar = box("SM_Sleigh_LanternBar", (0.010, 0.010, 0.185),
                  location=(math.cos(a) * 0.079, lantern_y + math.sin(a) * 0.079,
                            1.80), coll=coll)
        add(bar, iron)
    lantern_rim = ring_torus("SM_Sleigh_LanternRim", major=0.105, minor=0.011,
                             location=(0, lantern_y, 1.895), coll=coll,
                             major_seg=8, minor_seg=6)
    add(lantern_rim, brass)
    lantern_top = cone("SM_Sleigh_LanternTop", r1=0.118, r2=0.018, depth=0.10,
                       location=(0, lantern_y, 1.955), coll=coll,
                       segments=8, smooth=False)
    add(lantern_top, iron)
    flame = ellipsoid("SM_Sleigh_LanternFlame", (0.042, 0.042, 0.060),
                      location=(0, lantern_y, 1.795), coll=coll,
                      segments=12, rings=8)
    add(flame, ember)

    # ================================================================ CASCABELES
    for i, sx in enumerate((-0.42, -0.14, 0.14, 0.42)):
        bell = lathe("SM_Sleigh_Bell_%d" % i,
                     [(0.0, 0.0), (0.030, 0.004), (0.050, 0.042),
                      (0.060, 0.086), (0.050, 0.122), (0.0, 0.130)],
                     segments=16, coll=coll, location=(sx, -1.70, 1.300))
        add(bell, brass)
        hanger = tube("SM_Sleigh_BellHanger_%d" % i,
                      [(sx, -1.70, 1.440), (sx, -1.70, 1.380)],
                      radius=0.007, segments=6, coll=coll)
        add(hanger, iron)

    # ================================================================ BARANDILLA
    rail_pts = []
    for y in (-1.66, -1.10, -0.55, 0.00, 0.48):
        hw, ztop, depth = hull_at(y)
        rail_pts.append((0.965 * hw, y, ztop + 0.010))
    for sx in (-1, 1):
        pts = [(sx * x, y, z) for x, y, z in rail_pts]
        rail = tube("SM_Sleigh_Rail", pts, radius=0.030, segments=8, coll=coll)
        subsurf(rail, 1)
        add(rail, brass)
        for i, p in enumerate(pts):
            if i % 2 == 0:
                support = box("SM_Sleigh_RailPost", (0.032, 0.032, 0.16),
                              location=(p[0], p[1], p[2] - 0.085), coll=coll,
                              bevel=0.008, segments=2, smooth=True)
                add(support, iron)

    # ================================================================ ANILLAS
    for idx, (y, z) in enumerate(((-1.60, 1.32), (0.10, 1.14), (2.30, 1.26))):
        for sx in (-1, 1):
            hw, ztop, depth = hull_at(y)
            ring = ring_torus("SM_Sleigh_Eye_%d_%s" % (idx, "R" if sx > 0 else "L"),
                              major=0.046, minor=0.013,
                              location=(sx * (hw + 0.022), y, z),
                              coll=coll, rot=(0, D2R(90), 0),
                              major_seg=16, minor_seg=6)
            add(ring, iron)

    # ================================================================ ARMAS
    for sx in (-1, 1):
        mount = box("SM_Sleigh_WeaponMount", (0.055, 0.22, 0.10),
                    location=(sx * 1.035, -0.30, 1.150), coll=coll,
                    bevel=0.012, segments=2, smooth=True)
        add(mount, brass)
        hook = tube("SM_Sleigh_WeaponHook",
                    [(sx * 1.065, -0.30, 1.21), (sx * 1.065, -0.30, 1.30),
                     (sx * 1.000, -0.30, 1.345)],
                    radius=0.014, segments=6, coll=coll)
        add(hook, brass)

    # ================================================================ LAMPARA TRASERA
    lamp_ring = ring_torus("SM_Sleigh_RearRing", major=0.085, minor=0.018,
                           location=(0, 1.255, 1.300), coll=coll,
                           rot=(D2R(90), 0, 0), major_seg=24, minor_seg=8)
    add(lamp_ring, brass)
    lamp_core = ellipsoid("SM_Sleigh_RearGlow", (0.062, 0.030, 0.062),
                          location=(0, 1.262, 1.300), coll=coll,
                          segments=16, rings=10)
    add(lamp_core, ember)
    for i in range(3):
        a = D2R(30 + i * 120)
        bar = box("SM_Sleigh_RearBar_%d" % i, (0.014, 0.014, 0.165),
                  location=(0, 1.272, 1.300), coll=coll, rot=(0, 0, a))
        add(bar, brass)

    # ================================================================ ARNES
    for i, sx in enumerate((-0.62, -0.22, 0.22, 0.62)):
        hook = tube("SM_Sleigh_HarnessHook_%d" % i,
                    [(sx, -1.94, 0.98), (sx, -2.02, 0.90), (sx, -2.02, 0.80)],
                    radius=0.018, segments=8, coll=coll)
        add(hook, iron)
        plate = box("SM_Sleigh_HarnessPlate_%d" % i, (0.10, 0.06, 0.10),
                    location=(sx, -1.88, 1.02), coll=coll, bevel=0.014,
                    segments=2, smooth=True)
        add(plate, brass)

    # ================================================================ NIEVE
    for i, (x, y, r, z) in enumerate(((-0.58, 0.70, 0.15, 1.070),
                                       (0.60, 0.66, 0.13, 1.068),
                                       (0.12, 2.44, 0.16, 1.250),
                                       (-0.66, 1.86, 0.11, 1.180))):
        snow = ellipsoid("SM_Sleigh_Snow_%d" % i, (r, r * 0.8, 0.032),
                         location=(x, y, z), coll=coll, segments=16, rings=8)
        subsurf(snow, 1)
        displace(snow, 0.008, 0.03)
        add(snow, P['snow'])

    # ================================================================ CIERRE
    root = bpy.data.objects.new("SM_Sleigh", None)
    root.empty_display_type = 'PLAIN_AXES'
    root.empty_display_size = 0.6
    coll.objects.link(root)
    for p in parts:
        p.data.name = p.name
        p.parent = root

    print("PARTES:", len(parts))
    report([p for p in parts if p.type == 'MESH'])

    studio()
    scene.render.engine = 'BLENDER_EEVEE'
    save(os.path.join(ROOT, "Vehicles", "SM_Sleigh.blend"))
    return root


build()
