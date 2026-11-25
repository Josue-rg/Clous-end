"""
CLOUS-END · SK_Santa  (v2)

Modelo semirrealista cinematografico. 1 unidad = 1 m. Mira hacia -Y.
Presupuesto objetivo: 60-80k tris (Docs/06).
Sin rig ni animaciones: fase "solo modelos".
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
material = g["material"]
palette = g["palette"]
set_material = g["set_material"]
subsurf = g["subsurf"]
bevel = g["bevel"]
solidify = g["solidify"]
displace = g["displace"]
apply_all = g["apply_all"]
apply_transform = g["apply_transform"]
mirror_mesh = g["mirror_mesh"]
deform = g["deform"]
smooth_join = g["smooth_join"]
studio = g["studio"]
save = g["save"]
report = g["report"]
D2R = g["D2R"]
TAU = g["TAU"]

ROOT = r"C:\Users\Josue\Desktop\Clous-end\BlenderSource"

S_BIG = (32, 16)
S_MED = (24, 12)
S_SML = (16, 8)
S_TINY = (12, 6)

# ---------------------------------------------------------------------------
# Medidas maestras
# ---------------------------------------------------------------------------

COAT_HEM = 0.575
COAT_TOP = 1.500
Z_HEAD = 1.668
Z_EYES = 1.690
Z_CHIN = 1.556

COAT_PROFILE = [
    (0.300, 0.575),
    (0.316, 0.620),
    (0.324, 0.700),
    (0.322, 0.800),
    (0.318, 0.880),
    (0.320, 0.960),
    (0.314, 1.040),
    (0.302, 1.120),
    (0.286, 1.200),
    (0.268, 1.280),
    (0.252, 1.350),
    (0.240, 1.400),
    (0.216, 1.442),
    (0.160, 1.472),
    (0.140, 1.498),
]

Y_SQUASH = 0.84
GAP = D2R(44)
START = 1.5 * math.pi + GAP * 0.5


def coat_r(z):
    prof = COAT_PROFILE
    for i in range(len(prof) - 1):
        (r0, z0), (r1, z1) = prof[i], prof[i + 1]
        if z0 <= z <= z1:
            t = (z - z0) / (z1 - z0) if z1 > z0 else 0.0
            return r0 + (r1 - r0) * t
    return prof[0][0] if z < prof[0][1] else prof[-1][0]


def build():
    scene = reset_scene()
    P = palette()
    coll = collection("SK_Santa")
    parts = []

    def add(obj, mat, name=None):
        if name:
            obj.name = name
            obj.data.name = name
        set_material(obj, mat)
        parts.append(obj)
        return obj

    # =============================================================== TORSO
    torso_prof = [(0.0, COAT_HEM + 0.02)]
    for r, z in COAT_PROFILE:
        torso_prof.append((max(r - 0.042, 0.0), z))
    torso_prof.append((0.0, COAT_TOP - 0.012))
    torso = lathe("SK_Santa_Torso", torso_prof, segments=40, coll=coll,
                  scale=(1, Y_SQUASH, 1))
    subsurf(torso, 1)
    add(torso, P['vest'])

    chest = ellipsoid("SK_Santa_Chest", (0.222, 0.180, 0.165),
                      location=(0, -0.012, 1.330), coll=coll, segments=S_MED[0], rings=S_MED[1])
    subsurf(chest, 1)
    add(chest, P['vest'])

    belly = ellipsoid("SK_Santa_Belly", (0.243, 0.210, 0.205),
                      location=(0, -0.022, 1.030), coll=coll, segments=S_MED[0], rings=S_MED[1])
    subsurf(belly, 1)
    add(belly, P['vest'])

    # cremallera corta del chaleco
    zip_line = box("SK_Santa_VestZip", (0.010, 0.008, 0.135),
                   location=(0, -0.252, 1.345), coll=coll, bevel=0.003)
    add(zip_line, P['iron'])

    # =============================================================== ABRIGO
    coat_prof = [(0.0, COAT_HEM)] + COAT_PROFILE + [(0.0, COAT_TOP)]
    coat = lathe("SK_Santa_Coat", coat_prof, segments=48,
                 angle=TAU - GAP, start=START, coll=coll,
                 scale=(1, Y_SQUASH, 1))
    solidify(coat, 0.022, 0)
    subsurf(coat, 1)
    displace(coat, 0.005, 0.11)
    add(coat, P['red'])

    tail = lathe("SK_Santa_CoatTail",
                 [(0.0, 0.500), (0.296, 0.520), (0.318, 0.560), (0.322, 0.600),
                  (0.0, 0.610)],
                 segments=40, angle=D2R(130), start=D2R(115), coll=coll,
                 scale=(1, Y_SQUASH, 1))
    solidify(tail, 0.020, 0)
    subsurf(tail, 1)
    add(tail, P['red'])

    # vivo de piel en la abertura delantera (dos cintas curvas)
    for side, a in (("R", START), ("L", START + (TAU - GAP))):
        pts = []
        for r, z in COAT_PROFILE:
            pts.append((r * math.cos(a) * 1.005,
                        r * math.sin(a) * Y_SQUASH * 1.005, z))
        piping = tube("SK_Santa_CoatTrim_%s" % side, pts,
                      radius=0.019, segments=8, coll=coll)
        subsurf(piping, 1)
        add(piping, P['fur'])

    # cuello de piel
    collar = lathe("SK_Santa_Collar",
                   [(0.0, 1.428), (0.150, 1.430), (0.176, 1.450),
                    (0.174, 1.476), (0.146, 1.496), (0.0, 1.500)],
                   segments=40, coll=coll, scale=(1, 0.95, 1))
    subsurf(collar, 1)
    displace(collar, 0.013, 0.05)
    add(collar, P['fur'])

    # bajo del abrigo con piel
    hem_fur = lathe("SK_Santa_HemFur",
                    [(0.0, 0.546), (0.296, 0.548), (0.314, 0.566),
                     (0.316, 0.590), (0.300, 0.606), (0.0, 0.608)],
                    segments=40, coll=coll, scale=(1, Y_SQUASH, 1))
    subsurf(hem_fur, 1)
    displace(hem_fur, 0.014, 0.045)
    add(hem_fur, P['fur'])

    # =============================================================== CINTURON
    belt_prof = [(0.0, 0.938)]
    for z in (0.938, 0.960, 0.990, 1.020):
        belt_prof.append((coat_r(z) - 0.032, z))
    belt_prof.append((0.0, 1.020))
    belt = lathe("SK_Santa_Belt", belt_prof, segments=40, coll=coll,
                 scale=(1, Y_SQUASH, 1))
    solidify(belt, 0.016, 0)
    subsurf(belt, 1)
    add(belt, P['leather'])

    y_front = -coat_r(0.980) * Y_SQUASH + 0.028
    buckle = box("SK_Santa_Buckle", (0.072, 0.018, 0.078),
                 location=(0, y_front, 0.980), coll=coll,
                 bevel=0.012, segments=3, smooth=True)
    add(buckle, P['brass'])
    buckle_in = box("SK_Santa_BuckleInner", (0.040, 0.022, 0.044),
                    location=(0, y_front - 0.002, 0.980), coll=coll,
                    bevel=0.006, segments=2, smooth=True)
    add(buckle_in, P['leather'])

    pouch = box("SK_Santa_Pouch", (0.072, 0.056, 0.086),
                location=(0.238, -0.150, 0.900), coll=coll,
                rot=(0, 0, D2R(-24)), bevel=0.016, segments=3, smooth=True)
    subsurf(pouch, 1)
    add(pouch, P['leather'])
    flap = box("SK_Santa_PouchFlap", (0.078, 0.060, 0.028),
               location=(0.240, -0.152, 0.936), coll=coll,
               rot=(D2R(5), 0, D2R(-24)), bevel=0.010, segments=2, smooth=True)
    add(flap, P['leather'])

    # =============================================================== CABEZA
    head = ellipsoid("SK_Santa_Head", (0.113, 0.121, 0.129),
                     location=(0, -0.004, Z_HEAD), coll=coll, segments=S_BIG[0], rings=S_BIG[1])
    subsurf(head, 1)
    add(head, P['skin'])

    skull_back = ellipsoid("SK_Santa_SkullBack", (0.116, 0.110, 0.118),
                           location=(0, 0.020, Z_HEAD + 0.006), coll=coll,
                           segments=S_MED[0], rings=S_MED[1])
    subsurf(skull_back, 1)
    add(skull_back, P['beard'])

    jaw = ellipsoid("SK_Santa_Jaw", (0.090, 0.086, 0.074),
                    location=(0, -0.028, Z_CHIN + 0.034), coll=coll,
                    segments=S_MED[0], rings=S_MED[1])
    subsurf(jaw, 1)
    add(jaw, P['skin'])

    nose = ellipsoid("SK_Santa_Nose", (0.027, 0.032, 0.026),
                     location=(0, -0.122, 1.643), coll=coll, segments=S_MED[0], rings=S_MED[1])
    subsurf(nose, 1)
    add(nose, P['skin'])
    nose_tip = ellipsoid("SK_Santa_NoseTip", (0.020, 0.019, 0.017),
                         location=(0, -0.137, 1.631), coll=coll, segments=S_SML[0], rings=S_SML[1])
    subsurf(nose_tip, 1)
    add(nose_tip, P['skin'])

    for sx in (-1, 1):
        cheek = ellipsoid("SK_Santa_Cheek_%s" % ("L" if sx < 0 else "R"),
                          (0.036, 0.026, 0.027),
                          location=(sx * 0.058, -0.093, 1.644), coll=coll,
                          segments=S_SML[0], rings=S_SML[1])
        subsurf(cheek, 1)
        add(cheek, P['skin'])
        ear = ellipsoid("SK_Santa_Ear_%s" % ("L" if sx < 0 else "R"),
                        (0.016, 0.030, 0.038),
                        location=(sx * 0.117, -0.004, 1.652), coll=coll,
                        rot=(0, 0, D2R(sx * -8)), segments=S_SML[0], rings=S_SML[1])
        subsurf(ear, 1)
        add(ear, P['skin'])

    eye_mat = material("M_Santa_Eye", (0.018, 0.022, 0.028, 1), rough=0.07)
    for sx in (-1, 1):
        eye = ellipsoid("SK_Santa_Eye_%s" % ("L" if sx < 0 else "R"),
                        (0.017, 0.012, 0.014),
                        location=(sx * 0.046, -0.121, Z_EYES), coll=coll,
                        segments=S_SML[0], rings=S_SML[1])
        subsurf(eye, 1)
        add(eye, eye_mat)
        glint = ellipsoid("SK_Santa_Glint_%s" % ("L" if sx < 0 else "R"),
                          (0.0028, 0.0022, 0.0028),
                          location=(sx * 0.049, -0.132, Z_EYES + 0.005),
                          coll=coll, segments=S_TINY[0], rings=S_TINY[1])
        add(glint, material("M_Glint", (1, 1, 1, 1), rough=0.1,
                            emission=(1, 1, 1, 1), emission_strength=1.5))

        brow = ellipsoid("SK_Santa_Brow_%s" % ("L" if sx < 0 else "R"),
                         (0.044, 0.014, 0.011),
                         location=(sx * 0.048, -0.118, 1.707), coll=coll,
                         rot=(0, D2R(sx * -14), D2R(sx * 8)), segments=S_SML[0], rings=S_SML[1])
        subsurf(brow, 1)
        add(brow, P['beard'])

    # =============================================================== BARBA
    beard_base = ellipsoid("SK_Santa_BeardBase", (0.122, 0.104, 0.132),
                           location=(0, -0.036, 1.482), coll=coll,
                           segments=S_MED[0], rings=S_MED[1])
    subsurf(beard_base, 1)
    add(beard_base, P['beard'])

    random.seed(11)
    locks = []
    rows = [
        (1.545, 0.112, 0.098, 11, 0.030, 0.060),
        (1.495, 0.120, 0.104, 11, 0.033, 0.070),
        (1.445, 0.122, 0.100, 10, 0.032, 0.072),
        (1.398, 0.114, 0.092, 9, 0.030, 0.066),
        (1.360, 0.098, 0.080, 7, 0.027, 0.056),
    ]
    for rz, rx, ry, count, lr, lz in rows:
        for i in range(count):
            t = i / (count - 1) if count > 1 else 0.5
            a = D2R(205) + D2R(130) * t
            px = math.cos(a) * rx
            py = -abs(math.sin(a)) * ry - 0.006
            jx = random.uniform(-0.004, 0.004)
            jz = random.uniform(-0.005, 0.005)
            lock = ellipsoid("SK_Santa_Lock", (lr, lr * 0.85, lz),
                             location=(px + jx, py, rz + jz), coll=coll,
                             rot=(D2R(-8), 0, D2R(-math.degrees(a) * 0.06)),
                             segments=S_TINY[0], rings=S_TINY[1])
            set_material(lock, P['beard'])
            locks.append(lock)
    for lock in locks:
        apply_all(lock)
    beard_locks = smooth_join(locks, "SK_Santa_BeardLocks")
    parts.append(beard_locks)

    for sx in (-1, 1):
        pts = [(sx * 0.004, -0.140, 1.606),
               (sx * 0.036, -0.142, 1.596),
               (sx * 0.062, -0.132, 1.577),
               (sx * 0.078, -0.114, 1.551),
               (sx * 0.080, -0.102, 1.530)]
        mus = tube("SK_Santa_Mustache_%s" % ("L" if sx < 0 else "R"), pts,
                   radius=[0.023, 0.023, 0.019, 0.014, 0.007],
                   segments=10, coll=coll)
        subsurf(mus, 1)
        add(mus, P['beard'])

        side = ellipsoid("SK_Santa_Sideburn_%s" % ("L" if sx < 0 else "R"),
                         (0.030, 0.046, 0.072),
                         location=(sx * 0.100, -0.010, 1.612), coll=coll,
                         rot=(0, D2R(sx * 6), 0), segments=S_SML[0], rings=S_SML[1])
        subsurf(side, 1)
        add(side, P['beard'])

    # =============================================================== GORRO
    hz = 1.748
    hat = lathe("SK_Santa_Hat",
                [(0.000, hz), (0.120, hz + 0.004), (0.123, hz + 0.026),
                 (0.114, hz + 0.058), (0.098, hz + 0.096), (0.076, hz + 0.138),
                 (0.050, hz + 0.176), (0.026, hz + 0.204), (0.000, hz + 0.216)],
                segments=40, coll=coll, scale=(1, 0.95, 1))

    def droop(co):
        t = max(0.0, co.z - (hz + 0.050))
        co.y -= 4.2 * t * t
        co.z -= 1.60 * t * t
        return co

    deform(hat, droop)
    solidify(hat, 0.016, -1)
    subsurf(hat, 1)
    add(hat, P['red'])

    brim = lathe("SK_Santa_HatBrim",
                 [(0.0, hz - 0.026), (0.120, hz - 0.024), (0.134, hz + 0.002),
                  (0.130, hz + 0.026), (0.112, hz + 0.036), (0.0, hz + 0.036)],
                 segments=48, coll=coll, scale=(1, 0.97, 1))
    subsurf(brim, 1)
    displace(brim, 0.012, 0.04)
    add(brim, P['fur'])

    pom_tip = (0.0, -0.114, hz + 0.166)
    for i in range(7):
        a = TAU * i / 7 + random.uniform(-0.3, 0.3)
        r = random.uniform(0.010, 0.024)
        p1 = (pom_tip[0] + math.cos(a) * 0.012,
              pom_tip[1] + math.sin(a) * 0.012,
              pom_tip[2] - 0.004)
        p2 = (pom_tip[0] + math.cos(a) * r,
              pom_tip[1] + math.sin(a) * r - 0.008,
              pom_tip[2] - random.uniform(0.028, 0.052))
        tuft = tube("SK_Santa_Pom_%d" % i, [p1, p2],
                    radius=[0.011, 0.0025], segments=6, coll=coll)
        subsurf(tuft, 1)
        add(tuft, P['beard'])

    # =============================================================== BRAZOS
    arm_parts = []

    shoulder = (0.262, -0.020, 1.408)
    elbow = (0.352, -0.072, 1.175)
    wrist = (0.372, -0.152, 0.990)

    sleeve = tube("SK_Santa_Sleeve",
                  [shoulder, (0.304, -0.040, 1.292), elbow,
                   (0.360, -0.112, 1.078), wrist],
                  radius=[0.096, 0.089, 0.081, 0.071, 0.061],
                  segments=14, coll=coll)
    subsurf(sleeve, 1)
    displace(sleeve, 0.004, 0.09)
    set_material(sleeve, P['red'])
    arm_parts.append(sleeve)

    cap = ellipsoid("SK_Santa_ShoulderCap", (0.100, 0.100, 0.094),
                    location=(shoulder[0], shoulder[1], shoulder[2] + 0.004),
                    coll=coll, segments=S_MED[0], rings=S_MED[1])
    subsurf(cap, 1)
    set_material(cap, P['red'])
    arm_parts.append(cap)

    cuff = lathe("SK_Santa_Cuff",
                 [(0.0, -0.026), (0.066, -0.024), (0.073, 0.0),
                  (0.066, 0.024), (0.0, 0.026)],
                 segments=24, coll=coll)
    apply_transform(cuff, location=False)
    cuff.location = wrist
    cuff.rotation_euler = (D2R(40), 0, D2R(-9))
    subsurf(cuff, 1)
    displace(cuff, 0.008, 0.03)
    set_material(cuff, P['fur'])
    arm_parts.append(cuff)

    palm = ellipsoid("SK_Santa_Palm", (0.042, 0.031, 0.062),
                     location=(0.378, -0.164, 0.934), coll=coll,
                     rot=(D2R(10), 0, 0), segments=S_SML[0], rings=S_SML[1])
    subsurf(palm, 1)
    set_material(palm, P['leather'])
    arm_parts.append(palm)

    fingers = []
    for i in range(4):
        fx = 0.378 + (i - 1.5) * 0.019
        base = (fx, -0.176, 0.894)
        mid = (fx + (i - 1.5) * 0.002, -0.189, 0.858)
        tip = (fx + (i - 1.5) * 0.004, -0.184, 0.828)
        f = tube("SK_Santa_Finger_%d" % i, [base, mid, tip],
                 radius=[0.0115, 0.0105, 0.0080], segments=6, coll=coll)
        subsurf(f, 1)
        set_material(f, P['leather'])
        fingers.append(f)
    thumb = tube("SK_Santa_Thumb",
                 [(0.344, -0.166, 0.944), (0.334, -0.190, 0.921),
                  (0.337, -0.198, 0.900)],
                 radius=[0.014, 0.012, 0.009], segments=6, coll=coll)
    subsurf(thumb, 1)
    set_material(thumb, P['leather'])
    fingers.append(thumb)
    for f in fingers:
        apply_all(f)
    hand = smooth_join(fingers, "SK_Santa_Fingers")
    arm_parts.append(hand)

    for obj in arm_parts:
        apply_all(obj)
        mirror_mesh(obj)
        parts.append(obj)

    # =============================================================== PIERNAS
    leg_parts = []
    hip = (0.115, 0.0, 0.960)
    knee = (0.116, -0.012, 0.500)
    ankle = (0.118, -0.018, 0.150)

    leg = tube("SK_Santa_Leg",
               [hip, (0.116, -0.006, 0.740), knee,
                (0.118, -0.014, 0.330), ankle],
               radius=[0.113, 0.104, 0.092, 0.086, 0.079],
               segments=14, coll=coll)
    subsurf(leg, 1)
    set_material(leg, P['trouser'])
    leg_parts.append(leg)

    shaft = lathe("SK_Santa_BootShaft",
                  [(0.0, 0.098), (0.081, 0.100), (0.086, 0.180),
                   (0.089, 0.260), (0.087, 0.300), (0.0, 0.305)],
                  segments=24, coll=coll)
    shaft.location = (ankle[0], ankle[1], 0)
    subsurf(shaft, 1)
    set_material(shaft, P['leather'])
    leg_parts.append(shaft)

    foot = ellipsoid("SK_Santa_Foot", (0.087, 0.156, 0.075),
                     location=(ankle[0], -0.074, 0.086), coll=coll,
                     segments=S_SML[0], rings=S_SML[1])
    subsurf(foot, 1)
    set_material(foot, P['leather'])
    leg_parts.append(foot)

    toe = ellipsoid("SK_Santa_Toe", (0.077, 0.062, 0.061),
                    location=(ankle[0], -0.194, 0.079), coll=coll,
                    segments=S_SML[0], rings=S_SML[1])
    subsurf(toe, 1)
    set_material(toe, P['leather'])
    leg_parts.append(toe)

    sole = box("SK_Santa_Sole", (0.174, 0.328, 0.034),
               location=(ankle[0], -0.080, 0.019), coll=coll,
               bevel=0.012, segments=2, smooth=True)
    set_material(sole, P['iron'])
    leg_parts.append(sole)

    spikes = []
    for i in range(4):
        for j in (-1, 1):
            spikes.append(g["cone"]("SK_Santa_Spike", r1=0.015, r2=0.002,
                                    depth=0.028,
                                    location=(ankle[0] + j * 0.054,
                                              -0.018 - i * 0.068, 0.000),
                                    coll=coll, rot=(D2R(180), 0, 0),
                                    segments=8))
    for sp in spikes:
        apply_all(sp)
    crampons = smooth_join(spikes, "SK_Santa_Crampons")
    set_material(crampons, P['iron'])
    leg_parts.append(crampons)

    for zz in (0.182, 0.252):
        strap = lathe("SK_Santa_Strap",
                      [(0.0, -0.012), (0.089, -0.012), (0.091, 0.0),
                       (0.089, 0.012), (0.0, 0.012)],
                      segments=24, coll=coll)
        strap.location = (ankle[0], ankle[1], zz)
        subsurf(strap, 1)
        set_material(strap, P['iron'])
        leg_parts.append(strap)

    for obj in leg_parts:
        apply_all(obj)
        mirror_mesh(obj)
        parts.append(obj)

    boot_fur = lathe("SK_Santa_BootFur",
                     [(0.0, 0.290), (0.093, 0.292), (0.099, 0.306),
                      (0.093, 0.320), (0.0, 0.322)],
                     segments=24, coll=coll)
    boot_fur.location = (ankle[0], ankle[1], 0)
    subsurf(boot_fur, 1)
    displace(boot_fur, 0.007, 0.028)
    apply_all(boot_fur)
    mirror_mesh(boot_fur)
    add(boot_fur, P['fur'])

    # =============================================================== DETALLES
    for i, zz in enumerate((1.190, 1.290, 1.385)):
        btn = ellipsoid("SK_Santa_Toggle_%d" % i, (0.015, 0.010, 0.015),
                        location=(0.078, -0.248 - 0.006 * i, zz), coll=coll,
                        segments=S_TINY[0], rings=S_TINY[1])
        subsurf(btn, 1)
        add(btn, P['wood'])

    patch = box("SK_Santa_Patch", (0.072, 0.006, 0.066),
                location=(-0.180, -0.238, 0.740), coll=coll,
                rot=(0, 0, D2R(-12)), bevel=0.004, smooth=True)
    add(patch, material("M_Santa_Patch", (0.16, 0.055, 0.045, 1), rough=0.9))
    stitch = box("SK_Santa_PatchStitch", (0.074, 0.004, 0.009),
                 location=(-0.180, -0.241, 0.772), coll=coll,
                 rot=(0, 0, D2R(-12)))
    add(stitch, P['brass'])

    # =============================================================== CIERRE
    root = bpy.data.objects.new("SK_Santa", None)
    root.empty_display_type = 'PLAIN_AXES'
    root.empty_display_size = 0.35
    coll.objects.link(root)
    for p in parts:
        p.data.name = p.name
        p.parent = root

    print("PARTES:", len(parts))
    report([p for p in parts if p.type == 'MESH'])

    studio()
    scene.render.engine = 'BLENDER_EEVEE'
    scene.render.resolution_x = 1000
    scene.render.resolution_y = 1200
    save(os.path.join(ROOT, "Characters", "SK_Santa.blend"))
    return root


build()
