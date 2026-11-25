"""
CLOUS-END · Nicolás Cero (boss v1)

El anti-Santa: santo hueco de obsidiana y terciopelo negro.
El pecho vacío es su punto débil. 2,6 m. Sin rig.
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
star = g["star"]
material = g["material"]
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


def build():
    scene = reset_scene()
    coll = collection("SK_NicolasCero")
    parts = []

    obs = material("M_NC_Obsidian", (0.010, 0.010, 0.014, 1), rough=0.12,
                   metal=0.6, coat=0.9,
                   noise=dict(scale=6, detail=8, bump=0.3,
                              color1=(0.004, 0.004, 0.006, 1),
                              color2=(0.028, 0.028, 0.036, 1)))
    velvet = material("M_NC_Velvet", (0.003, 0.003, 0.004, 1), rough=0.96,
                      sheen=0.20,
                      noise=dict(scale=70, detail=6, bump=0.25,
                                 color1=(0.004, 0.004, 0.006, 1),
                                 color2=(0.020, 0.020, 0.026, 1)))
    void = material("M_NC_Void", (0.0, 0.0, 0.0, 1), rough=0.5)
    vein = material("M_NC_Vein", (0.85, 0.94, 1.0, 1), rough=0.2,
                    emission=(0.72, 0.90, 1.0, 1), emission_strength=8.0)
    eye = material("M_NC_Eye", (1.0, 1.0, 1.0, 1), rough=0.1,
                   emission=(0.90, 0.96, 1.0, 1), emission_strength=12.0)
    glass = material("M_NC_Glass", (0.58, 0.70, 0.78, 1), rough=0.06,
                     alpha=0.30, spec=1.0)

    def add(o, m, name=None):
        if name:
            o.name = name
            o.data.name = name
        set_material(o, m)
        parts.append(o)
        return o

    # ============================================================ TUNICA
    robe = lathe("SM_NC_Robe",
                 [(0.0, 0.02), (0.60, 0.02), (0.56, 0.12), (0.50, 0.40),
                  (0.45, 0.74), (0.40, 1.06), (0.36, 1.36), (0.33, 1.58),
                  (0.30, 1.70), (0.0, 1.74)],
                 segments=40, coll=coll, scale=(1.0, 0.84, 1.0))
    # pliegues verticales de tela
    def folds(co):
        a = math.atan2(co.y, co.x) if (co.x or co.y) else 0.0
        r = math.sqrt(co.x * co.x + co.y * co.y)
        k = 0.030 * r / 0.55
        co.x += math.cos(a) * math.sin(a * 9.0) * k
        co.y += math.sin(a) * math.sin(a * 9.0) * k
        return co
    deform(robe, folds)
    deform(robe, lambda co: (co.x + 0.03 * math.sin(co.z * 6 + co.x * 3),
                             co.y + 0.02 * math.cos(co.z * 5), co.z))
    subsurf(robe, 1)
    displace(robe, 0.012, 0.14)
    add(robe, velvet)

    # ============================================================ PECHO HUECO
    torso_sections = []
    for z, hw, dep in ((1.60, 0.30, 0.22), (1.72, 0.36, 0.26),
                       (1.86, 0.40, 0.28), (1.98, 0.38, 0.26),
                       (2.06, 0.30, 0.21)):
        ring = []
        for i in range(16):
            a = TAU * i / 16
            ring.append((hw * math.cos(a), dep * math.sin(a), z))
        torso_sections.append(ring)
    chest = loft("SM_NC_Chest", torso_sections, coll=coll, close=True,
                 cap_start=True, cap_end=True, smooth=False)
    bevel(chest, 0.014, 2, 30)
    add(chest, obs)

    cavity = cylinder("SM_NC_Cavity", radius=0.175, depth=0.06,
                      location=(0, -0.255, 1.86), rot=(D2R(90), 0, 0),
                      segments=16)
    add(cavity, void)
    add(ring_torus("SM_NC_CavityRim", major=0.185, minor=0.022,
                   location=(0, -0.268, 1.86), coll=coll,
                   rot=(D2R(90), 0, 0), major_seg=28, minor_seg=6), vein)
    for i in range(4):
        a = D2R(45 + i * 90)
        spike = cone("SM_NC_CavitySpike_%d" % i, r1=0.028, r2=0.002,
                     depth=0.13, location=(math.cos(a) * 0.11, -0.30,
                                           1.86 + math.sin(a) * 0.11),
                     rot=(D2R(90), 0, math.degrees(a)), segments=4,
                     smooth=False)
        add(spike, vein)
    for i in range(5):
        a = D2R(-60 + i * 30)
        spike = cone("SM_NC_CavitySpike_%d" % i, r1=0.030, r2=0.002,
                     depth=0.16, location=(math.cos(a) * 0.14, -0.30,
                                           1.86 + math.sin(a) * 0.14),
                     rot=(D2R(90), 0, math.degrees(a)), segments=4,
                     smooth=False)
        add(spike, obs)

    # ============================================================ CABEZA
    head = ellipsoid("SM_NC_Head", (0.215, 0.235, 0.275),
                     location=(0, 0.0, 2.28), coll=coll, segments=24, rings=16)
    subsurf(head, 1)
    add(head, obs)

    mask = box("SM_NC_Mask", (0.30, 0.10, 0.34),
               location=(0, -0.20, 2.27), bevel=0.05, segments=3, smooth=True)
    add(mask, obs)

    for sx in (-1, 1):
        slit = box("SM_NC_Slit_%d" % sx, (0.075, 0.020, 0.022),
                   location=(sx * 0.070, -0.247, 2.315), rot=(0, 0, D2R(sx * -8)))
        add(slit, eye)

    # barba de esquirlas de vidrio
    beard = []
    for row, (rz, rr, count) in enumerate(((2.18, 0.19, 7),
                                           (2.10, 0.21, 8),
                                           (2.01, 0.20, 7),
                                           (1.93, 0.17, 5))):
        for i in range(count):
            t = i / (count - 1) if count > 1 else 0.5
            a = D2R(200) + D2R(140) * t
            x = math.cos(a) * rr
            y = -abs(math.sin(a)) * rr * 0.85 - 0.10
            shard = cone("SM_NC_BeardShard", r1=0.030, r2=0.002,
                         depth=0.20, location=(x, y, rz),
                         rot=(D2R(180), 0, 0), segments=4, smooth=False)
            set_material(shard, obs)
            beard.append(shard)
    for b in beard:
        apply_all(b)
    add(smooth_join(beard, "SM_NC_Beard"), obs)

    # corona de esquirlas
    crown = []
    for i in range(7):
        a = TAU * i / 7
        shard = cone("SM_NC_CrownShard", r1=0.045, r2=0.002,
                     depth=0.42 + 0.12 * math.cos(a), 
                     location=(math.cos(a) * 0.20, math.sin(a) * 0.20, 2.48),
                     rot=(D2R(math.cos(a) * 10), D2R(math.sin(a) * 10), 0),
                     segments=4, smooth=False)
        set_material(shard, obs)
        crown.append(shard)
    for c in crown:
        apply_all(c)
    add(smooth_join(crown, "SM_NC_Crown"), obs)
    add(ring_torus("SM_NC_CrownBand", major=0.215, minor=0.014,
                   location=(0, 0, 2.44), coll=coll,
                   major_seg=24, minor_seg=6), vein)

    # ============================================================ HOMBROS
    for sx in (-1, 1):
        add(ellipsoid("SM_NC_Shoulder_%d" % sx, (0.19, 0.19, 0.16),
                      location=(sx * 0.40, -0.02, 2.02), coll=coll,
                      segments=16, rings=10), obs)
        for k in range(3):
            spike = cone("SM_NC_ShoulderSpike_%d_%d" % (sx, k), r1=0.045,
                         r2=0.002, depth=0.30 + k * 0.05,
                         location=(sx * (0.44 + k * 0.06), 0.0,
                                   2.14 + k * 0.03),
                         rot=(0, D2R(sx * (30 + k * 12)), 0), segments=4,
                         smooth=False)
            add(spike, obs)

    # ============================================================ BRAZOS
    arms = []
    for sx in (-1, 1):
        sh = (sx * 0.44, -0.03, 1.98)
        el = (sx * 0.56, -0.16, 1.56)
        wr = (sx * 0.50, -0.30, 1.22)
        arm = tube_smooth("SM_NC_Arm_%d" % sx, [sh, el, wr],
                          [0.115, 0.088, 0.070], [0.115, 0.088, 0.070],
                          sub=4, segments=8, coll=coll)
        arms.append(arm)
        for k in range(4):
            a = D2R(20 + k * 30)
            finger = tube("SM_NC_Finger_%d_%d" % (sx, k),
                          [wr, (wr[0] + sx * 0.04, wr[1] - 0.10,
                                wr[2] - 0.10 - 0.02 * k),
                           (wr[0] + sx * 0.03, wr[1] - 0.16,
                            wr[2] - 0.20 - 0.02 * k)],
                          radius=[0.024, 0.016, 0.004], segments=4,
                          coll=coll)
            arms.append(finger)
    for a in arms:
        apply_all(a)
    add(smooth_join(arms, "SM_NC_Arms"), obs)

    # ============================================================ BACULO
    staff = []
    staff.append(tube_smooth("SM_NC_Staff", [(0.62, -0.28, 0.05),
                                             (0.62, -0.30, 1.10),
                                             (0.62, -0.34, 1.95),
                                             (0.62, -0.28, 2.45)],
                             [0.035, 0.030, 0.026, 0.022],
                             [0.035, 0.030, 0.026, 0.022],
                             sub=4, segments=6, coll=coll))
    staff.append(star("SM_NC_StaffStar", outer=0.20, inner=0.085, depth=0.05,
                      points=7, coll=coll, location=(0.62, -0.28, 2.60),
                      rot=(D2R(90), 0, 0)))
    staff.append(ring_torus("SM_NC_StaffRing", major=0.235, minor=0.016,
                            location=(0.62, -0.28, 2.60), coll=coll,
                            rot=(D2R(90), 0, 0), major_seg=24, minor_seg=6))
    for s in staff:
        apply_all(s)
    add(smooth_join(staff, "SM_NC_Staff"), obs)
    add(ellipsoid("SM_NC_StaffCore", (0.075, 0.03, 0.075),
                  location=(0.62, -0.28, 2.60), coll=coll,
                  segments=12, rings=8), eye)

    # ============================================================ CIERRE
    for p in parts:
        p.data.name = p.name if not p.data.name else p.data.name
    joined = smooth_join(parts, "SM_NicolasCero")

    root = bpy.data.objects.new("SM_NicolasCero", None)
    root.empty_display_type = 'PLAIN_AXES'
    root.empty_display_size = 0.8
    coll.objects.link(root)
    joined.parent = root

    report([joined])
    studio()
    scene.render.engine = 'BLENDER_EEVEE'
    save(os.path.join(ROOT, "Boss", "SK_NicolasCero.blend"))


build()
