"""
CLOUS-END · VAT de todos los assets animados

Santa, Trineo, Sabueso, Garfio, Espora, Robots, Boss, Naves.
Cada uno: malla de juego + esqueleto + clip loop + textura VAT + FBX + JSON.

Uso:
    ONLY = ["Santa"]        # o [] para todos
"""

import bpy
import math
import os
import traceback

LIB_DIR = globals().get("__file__")
LIB_DIR = (os.path.dirname(os.path.abspath(LIB_DIR)) if LIB_DIR
           else r"C:\Users\Josue\Desktop\Clous-end\BlenderSource\scripts")
LIB = os.path.join(LIB_DIR, "vat_rig_lib.py")
g = {"__name__": "vat_rig_lib"}
exec(compile(open(LIB, encoding="utf-8").read(), LIB, "exec"), g)

clear_scene = g["clear_scene"]
append_collection = g["append_collection"]
join_meshes = g["join_meshes"]
decimate_to = g["decimate_to"]
unwrap_uv0 = g["unwrap_uv0"]
make_armature = g["make_armature"]
skin_proximity = g["skin_proximity"]
make_clip = g["make_clip"]
add_vat_uv = g["add_vat_uv"]
bake_vat = g["bake_vat"]
vat_bounds = g["vat_bounds"]
export_static_fbx = g["export_static_fbx"]
write_manifest = g["write_manifest"]
save_blend = g["save_blend"]
tri_count = g["tri_count"]

SRC = r"C:\Users\Josue\Desktop\Clous-end\BlenderSource"
OUT = os.path.join(SRC, "VAT")

ONLY = []

D2R = math.radians


def obj_from_collection(path, coll_name):
    coll = append_collection(path, coll_name)
    meshes = [o for o in coll.all_objects if o.type == 'MESH']
    return meshes


def obj_by_name(path, obj_name):
    """Abre el blend, coge el objeto (y sus hijos malla) y lo enlaza a la escena."""
    with bpy.data.libraries.load(path, link=False) as (src, dst):
        names = [n for n in src.objects if n == obj_name or n.startswith(obj_name + ".")]
        dst.objects = names
    scene_coll = bpy.context.scene.collection
    root = None
    meshes = []
    for o in dst.objects:
        if o is None:
            continue
        if not o.users_collection:
            try:
                scene_coll.objects.link(o)
            except RuntimeError:
                pass
        if o.type == 'MESH':
            meshes.append(o)
        else:
            root = o
    if root:
        for m in meshes:
            if m.parent is None:
                m.parent = root
    return meshes


# ===========================================================================
# SANTA
# ===========================================================================

def santa_bones():
    b = [
        dict(name="Root", head=(0, 0, 0), tail=(0, 0, 0.25), deform=False),
        dict(name="Hips", head=(0, 0, 0.92), tail=(0, 0, 1.04), parent="Root"),
        dict(name="Spine", head=(0, 0, 1.04), tail=(0, 0, 1.22), parent="Hips"),
        dict(name="Chest", head=(0, 0, 1.22), tail=(0, 0, 1.40), parent="Spine"),
        dict(name="Neck", head=(0, 0, 1.40), tail=(0, -0.005, 1.53), parent="Chest"),
        dict(name="Head", head=(0, -0.005, 1.53), tail=(0, -0.010, 1.79), parent="Neck"),
        dict(name="Beard", head=(0, -0.09, 1.56), tail=(0, -0.13, 1.38), parent="Head"),
        dict(name="CoatFront", head=(0, 0, 1.00), tail=(0, -0.02, 0.60), parent="Hips"),
        dict(name="CoatBack", head=(0, 0.05, 1.00), tail=(0, 0.10, 0.58), parent="Hips"),
    ]
    for tag, sx in (("L", -1), ("R", 1)):
        b += [
            dict(name="Shoulder." + tag,
                 head=(sx * 0.10, 0, 1.38), tail=(sx * 0.22, -0.01, 1.39),
                 parent="Chest"),
            dict(name="Arm." + tag,
                 head=(sx * 0.22, -0.01, 1.39), tail=(sx * 0.35, -0.07, 1.17),
                 parent="Shoulder." + tag),
            dict(name="Forearm." + tag,
                 head=(sx * 0.35, -0.07, 1.17), tail=(sx * 0.37, -0.15, 0.99),
                 parent="Arm." + tag),
            dict(name="Hand." + tag,
                 head=(sx * 0.37, -0.15, 0.99), tail=(sx * 0.38, -0.17, 0.88),
                 parent="Forearm." + tag),
            dict(name="Thigh." + tag,
                 head=(sx * 0.115, 0, 0.92), tail=(sx * 0.116, -0.01, 0.50),
                 parent="Hips"),
            dict(name="Shin." + tag,
                 head=(sx * 0.116, -0.01, 0.50), tail=(sx * 0.118, -0.02, 0.14),
                 parent="Thigh." + tag),
            dict(name="Foot." + tag,
                 head=(sx * 0.118, -0.02, 0.14), tail=(sx * 0.118, -0.21, 0.04),
                 parent="Shin." + tag),
        ]
    return b


def santa_clip(arm, frames):
    tr = [
        ("Spine", 0, 1.6, 0.00, 1.0, 0),
        ("Chest", 0, 1.4, 0.06, 1.0, -1.0),
        ("Neck", 0, 1.2, 0.10, 1.0, 0),
        ("Head", 2, 4.0, 0.08, 1.0, 0),
        ("Head", 0, 1.5, 0.12, 1.0, -1.0),
        ("Beard", 0, 2.5, 0.18, 1.0, 0),
        ("CoatFront", 0, 1.4, 0.22, 1.0, 0),
        ("CoatBack", 0, 1.2, 0.26, 1.0, 0),
    ]
    for tag, ph in (("L", 0.0), ("R", 0.5)):
        tr += [
            ("Arm." + tag, 0, 2.2, ph, 1.0, 0),
            ("Forearm." + tag, 0, 1.6, ph + 0.10, 1.0, 0),
            ("Shoulder." + tag, 0, 1.0, ph + 0.05, 1.0, 0),
        ]
    loc = [("Root", 1, 0.010, 0.15, 1.0, 0.0)]
    make_clip(arm, "AN_Santa_Idle", frames, tr, loc)


# ===========================================================================
# TRINEO
# ===========================================================================

def sleigh_bones():
    return [
        dict(name="Root", head=(0, 0, 0), tail=(0, 0, 0.3), deform=False),
        dict(name="Body", head=(0, 0, 0.55), tail=(0, 0, 1.10), parent="Root"),
        dict(name="SuspFront", head=(0, -1.4, 0.55), tail=(0, -1.4, 0.90),
             parent="Root"),
        dict(name="SuspBack", head=(0, 1.1, 0.55), tail=(0, 1.1, 0.90),
             parent="Root"),
        dict(name="Sack", head=(0, 2.0, 0.90), tail=(0, 2.0, 1.45),
             parent="Body"),
        dict(name="StarHeart", head=(0, -1.85, 1.30), tail=(0, -1.85, 1.55),
             parent="Body"),
    ]


def sleigh_clip(arm, frames):
    tr = [
        ("Body", 0, 1.5, 0.10, 1.0, 0),
        ("Body", 1, 0.8, 0.30, 1.0, 0),
        ("SuspFront", 0, 2.0, 0.05, 2.0, 0),
        ("SuspBack", 0, 1.6, 0.20, 2.0, 0),
        ("Sack", 0, 4.5, 0.35, 1.0, 0),
        ("Sack", 1, 2.5, 0.40, 1.0, 0),
        ("StarHeart", 1, 4.0, 0.0, 1.0, 0),
    ]
    loc = [("Root", 1, 0.030, 0.15, 2.0, 0.0)]
    make_clip(arm, "AN_Sleigh_Flight", frames, tr, loc)


# ===========================================================================
# SABUESO
# ===========================================================================

def sabueso_bones():
    b = [
        dict(name="Root", head=(0, 0, 0), tail=(0, 0, 0.2), deform=False),
        dict(name="Body", head=(0, 0.25, 0.66), tail=(0, -0.40, 0.68), parent="Root"),
        dict(name="Head", head=(0, -0.45, 0.70), tail=(0, -0.78, 0.70), parent="Body"),
        dict(name="JawL", head=(0.05, -0.58, 0.70), tail=(0.09, -0.80, 0.66), parent="Head"),
        dict(name="JawR", head=(-0.05, -0.58, 0.70), tail=(-0.09, -0.80, 0.66), parent="Head"),
    ]
    for tag, sx in (("FL", -1), ("FR", 1)):
        b += [
            dict(name=tag + "_Up", head=(sx * 0.15, -0.30, 0.62),
                 tail=(sx * 0.30, -0.30, 0.44), parent="Body"),
            dict(name=tag + "_Lo", head=(sx * 0.30, -0.30, 0.44),
                 tail=(sx * 0.34, -0.30, 0.06), parent=tag + "_Up"),
        ]
    for tag, sx in (("HL", -1), ("HR", 1)):
        b += [
            dict(name=tag + "_Up", head=(sx * 0.15, 0.24, 0.62),
                 tail=(sx * 0.30, 0.24, 0.44), parent="Body"),
            dict(name=tag + "_Lo", head=(sx * 0.30, 0.24, 0.44),
                 tail=(sx * 0.34, 0.24, 0.06), parent=tag + "_Up"),
        ]
    return b


def sabueso_clip(arm, frames):
    tr = [("Body", 0, 2.0, 0.1, 1.0, 0), ("Head", 0, 4.0, 0.15, 1.0, 0),
          ("JawL", 1, 6.0, 0.0, 2.0, 0), ("JawR", 1, 6.0, 0.5, 2.0, 0)]
    for tag, ph in (("FL", 0.0), ("FR", 0.5), ("HL", 0.25), ("HR", 0.75)):
        tr += [(tag + "_Up", 0, 30.0, ph, 1.0, 0),
               (tag + "_Lo", 0, -34.0, ph + 0.12, 1.0, 0)]
    loc = [("Root", 1, 0.030, 0.20, 2.0, 0.0)]
    make_clip(arm, "AN_Sabueso_Run", frames, tr, loc)


# ===========================================================================
# GARFIO
# ===========================================================================

def garfio_bones():
    return [
        dict(name="Root", head=(0, 0, 0), tail=(0, 0, 0.3), deform=False),
        dict(name="Body", head=(0, 0, 1.00), tail=(0, 0, 1.70), parent="Root"),
        dict(name="Arm1", head=(0, -0.20, 1.60), tail=(0, -0.70, 1.40), parent="Body"),
        dict(name="Arm2", head=(0, -0.70, 1.40), tail=(0, -1.20, 1.00), parent="Arm1"),
        dict(name="Hook", head=(0, -1.20, 1.00), tail=(0, -1.42, 0.60), parent="Arm2"),
    ]


def garfio_clip(arm, frames):
    tr = [
        ("Body", 0, 3.0, 0.00, 1.0, 0),
        ("Body", 1, 2.0, 0.25, 1.0, 0),
        ("Arm1", 0, 7.0, 0.10, 1.0, 0),
        ("Arm1", 2, 5.0, 0.40, 1.0, 0),
        ("Arm2", 0, 9.0, 0.22, 1.0, 0),
        ("Hook", 0, 12.0, 0.35, 1.0, 0),
        ("Hook", 2, 8.0, 0.55, 1.0, 0),
    ]
    loc = [("Root", 1, 0.045, 0.10, 1.0, 0.0)]
    make_clip(arm, "AN_Garfio_Hover", frames, tr, loc)


# ===========================================================================
# ESPORA
# ===========================================================================

def espora_bones():
    return [
        dict(name="Root", head=(0, 0, 0), tail=(0, 0, 0.25), deform=False),
        dict(name="Body", head=(0, 0, 0.30), tail=(0, 0, 0.95), parent="Root"),
    ]


def espora_clip(arm, frames):
    tr = [("Body", 0, 4.0, 0.0, 1.0, 0), ("Body", 1, 3.0, 0.35, 1.0, 0)]
    loc = [("Root", 1, 0.060, 0.0, 1.0, 0.0),
           ("Body", 1, 0.030, 0.25, 2.0, 0.0)]
    make_clip(arm, "AN_Espora_Pulse", frames, tr, loc)


# ===========================================================================
# ROBOTS
# ===========================================================================

def robot_bones(scale=1.0):
    s = scale
    b = [
        dict(name="Root", head=(0, 0, 0), tail=(0, 0, 0.3), deform=False),
        dict(name="Hips", head=(0, 0, 0.35 * s), tail=(0, 0, 1.05 * s), parent="Root"),
        dict(name="Chest", head=(0, 0, 1.05 * s), tail=(0, 0, 1.62 * s), parent="Hips"),
        dict(name="Head", head=(0, 0, 1.70 * s), tail=(0, 0, 2.02 * s), parent="Chest"),
    ]
    for tag, sx in (("L", -1), ("R", 1)):
        for i, (zz, spread) in enumerate(((1.50, 0.40), (1.22, 0.30))):
            nm = "Arm%s%d" % (tag, i)
            head = (sx * 0.26 * s, 0.0, zz * s)
            mid = (sx * (0.26 + spread * 0.5) * s, -0.10 * s, (zz - 0.30) * s)
            tail = (sx * (0.26 + spread) * s, -0.34 * s, (zz - 0.58) * s)
            b += [
                dict(name=nm, head=head, tail=mid, parent="Chest"),
                dict(name=nm + "b", head=mid, tail=tail, parent=nm),
            ]
    return b


def robot_clip(arm, frames):
    tr = [("Chest", 0, 1.8, 0.0, 1.0, 0), ("Chest", 2, 1.5, 0.25, 1.0, 0),
          ("Head", 2, 5.0, 0.10, 1.0, 0)]
    for tag, ph in (("L", 0.0), ("R", 0.5)):
        for i in range(2):
            tr += [("Arm%s%d" % (tag, i), 0, 3.0 + i * 2, ph + i * 0.15, 1.0, 0),
                   ("Arm%s%db" % (tag, i), 2, 3.0, ph + 0.2, 1.0, 0)]
    loc = [("Root", 1, 0.012, 0.15, 1.0, 0.0)]
    make_clip(arm, "AN_Robot_Idle", frames, tr, loc)


# ===========================================================================
# BOSS
# ===========================================================================

def boss_bones():
    return [
        dict(name="Root", head=(0, 0, 0), tail=(0, 0, 0.35), deform=False),
        dict(name="Robe", head=(0, 0, 0.35), tail=(0, 0, 1.40), parent="Root"),
        dict(name="Torso", head=(0, 0, 1.40), tail=(0, 0, 2.10), parent="Robe"),
        dict(name="Head", head=(0, 0, 2.10), tail=(0, 0, 2.48), parent="Torso"),
        dict(name="Arm.L", head=(0.42, 0, 1.98), tail=(0.52, -0.24, 1.22),
             parent="Torso"),
        dict(name="Arm.R", head=(-0.42, 0, 1.98), tail=(-0.52, -0.24, 1.22),
             parent="Torso"),
        dict(name="Staff", head=(0.62, -0.28, 0.05), tail=(0.62, -0.28, 2.55),
             parent="Arm.R"),
    ]


def boss_clip(arm, frames):
    tr = [
        ("Robe", 0, 0.8, 0.10, 1.0, 0),
        ("Torso", 0, 1.2, 0.20, 1.0, 0),
        ("Head", 2, 4.0, 0.05, 1.0, 0),
        ("Head", 0, 1.5, 0.30, 1.0, 0),
        ("Arm.L", 0, 2.0, 0.15, 1.0, 0),
        ("Arm.R", 0, 1.5, 0.65, 1.0, 0),
        ("Staff", 1, 2.0, 0.10, 1.0, 0),
    ]
    loc = [("Root", 1, 0.025, 0.0, 1.0, 0.0)]
    make_clip(arm, "AN_NicolasCero_Idle", frames, tr, loc)


# ===========================================================================
# NAVES
# ===========================================================================

def ship_bones(long_axis=6.0):
    return [
        dict(name="Root", head=(0, 0, 0), tail=(0, 0, 0.4), deform=False),
        dict(name="Hull", head=(0, 0, 0.4), tail=(0, 0, 1.6), parent="Root"),
        dict(name="Nose", head=(0, -long_axis * 0.5, 0.5),
             tail=(0, -long_axis, 0.4), parent="Hull"),
        dict(name="Tail", head=(0, long_axis * 0.4, 0.4),
             tail=(0, long_axis * 0.8, 0.4), parent="Hull"),
    ]


def ship_clip(arm, frames):
    tr = [("Hull", 0, 1.2, 0.0, 1.0, 0), ("Hull", 1, 1.6, 0.25, 1.0, 0),
          ("Nose", 0, 1.0, 0.30, 1.0, 0), ("Tail", 0, 1.0, 0.55, 1.0, 0)]
    loc = [("Root", 1, 0.055, 0.0, 1.0, 0.0)]
    make_clip(arm, "AN_Ship_Hover", frames, tr, loc)


# ===========================================================================
# ESPECIFICACIONES
# ===========================================================================

SPECS = [
    dict(name="Santa", kind="coll",
         src=os.path.join(SRC, "Characters", "SK_Santa.blend"),
         coll="SK_Santa", bones=santa_bones, clip=santa_clip,
         frames=36, target=60000, fps=30, clip_name="Idle"),
    dict(name="Sleigh", kind="coll",
         src=os.path.join(SRC, "Vehicles", "SM_Sleigh.blend"),
         coll="SM_Sleigh", bones=sleigh_bones, clip=sleigh_clip,
         frames=36, target=60000, fps=30, clip_name="Flight"),
    dict(name="Sabueso", kind="obj",
         src=os.path.join(SRC, "Enemies", "SK_Yermos.blend"),
         obj="SM_Sabueso", bones=sabueso_bones, clip=sabueso_clip,
         frames=20, target=20000, fps=30, clip_name="Run"),
    dict(name="Garfio", kind="obj",
         src=os.path.join(SRC, "Enemies", "SK_Yermos.blend"),
         obj="SM_Garfio", bones=garfio_bones, clip=garfio_clip,
         frames=36, target=20000, fps=30, clip_name="Hover"),
    dict(name="Espora", kind="obj",
         src=os.path.join(SRC, "Enemies", "SK_Yermos.blend"),
         obj="SM_Espora", bones=espora_bones, clip=espora_clip,
         frames=24, target=12000, fps=30, clip_name="Pulse"),
    dict(name="Robot_Manto", kind="obj",
         src=os.path.join(SRC, "Enemies", "SK_Yermos.blend"),
         obj="SM_Robot_Manto", bones=lambda: robot_bones(1.0),
         clip=robot_clip, frames=36, target=20000, fps=30, clip_name="Idle"),
    dict(name="Robot_Torre", kind="obj",
         src=os.path.join(SRC, "Enemies", "SK_Yermos.blend"),
         obj="SM_Robot_Torre", bones=lambda: robot_bones(1.12),
         clip=robot_clip, frames=36, target=20000, fps=30, clip_name="Idle"),
    dict(name="NicolasCero", kind="obj",
         src=os.path.join(SRC, "Boss", "SK_NicolasCero.blend"),
         obj="SM_NicolasCero", bones=boss_bones, clip=boss_clip,
         frames=48, target=40000, fps=30, clip_name="Idle"),
    dict(name="Ship_Caza", kind="obj",
         src=os.path.join(SRC, "Ships", "SK_Ships.blend"),
         obj="SM_Ship_Caza", bones=lambda: ship_bones(6.0), clip=ship_clip,
         frames=40, target=30000, fps=30, clip_name="Hover"),
    dict(name="Ship_Recolector", kind="obj",
         src=os.path.join(SRC, "Ships", "SK_Ships.blend"),
         obj="SM_Ship_Recolector", bones=lambda: ship_bones(12.0),
         clip=ship_clip, frames=40, target=40000, fps=30, clip_name="Hover"),
]


def build_one(spec):
    name = spec["name"]
    print("\n=== %s ===" % name)
    if spec["kind"] == "coll":
        meshes = obj_from_collection(spec["src"], spec["coll"])
    else:
        meshes = obj_by_name(spec["src"], spec["obj"])
    print("piezas:", len(meshes))

    obj = join_meshes(meshes, "SK_%s_Game" % name, origin=(0, 0, 0))
    decimate_to(obj, spec["target"])
    unwrap_uv0(obj)

    arm = make_armature("RIG_%s" % name, spec["bones"](),
                        coll=bpy.context.scene.collection)
    skin_proximity(obj, arm, power=5.0, max_inf=3)
    spec["clip"](arm, spec["frames"])

    rw, rows, height = add_vat_uv(obj, rw=1024, frames=spec["frames"])

    out_dir = os.path.join(OUT, name)
    os.makedirs(out_dir, exist_ok=True)
    exr = os.path.join(out_dir, "T_%s_%s_Pos.exr" % (name, spec["clip_name"]))
    bake_vat(obj, exr, spec["frames"], rw=rw, start_frame=1, rows=rows)

    mn, mx = vat_bounds(obj, spec["frames"], start_frame=1)
    fbx = os.path.join(out_dir, "SK_%s.fbx" % name)
    export_static_fbx(obj, fbx)

    write_manifest(os.path.join(out_dir, "vat_%s.json" % name), {
        "asset": "SK_%s" % name,
        "mesh": os.path.basename(fbx),
        "triangles": tri_count(obj),
        "vertices": len(obj.data.vertices),
        "clip": spec["clip_name"],
        "frames": spec["frames"],
        "fps": spec["fps"],
        "loop": True,
        "texture": os.path.basename(exr),
        "texture_width": rw,
        "texture_height": height,
        "rows": rows,
        "bounds_min": [round(v, 4) for v in mn],
        "bounds_max": [round(v, 4) for v in mx],
    })
    save_blend(os.path.join(out_dir, "SK_%s_vat.blend" % name))
    return True


def main():
    results = []
    for spec in SPECS:
        if ONLY and spec["name"] not in ONLY:
            continue
        clear_scene()
        try:
            build_one(spec)
            results.append((spec["name"], "OK"))
        except Exception as exc:
            print("ERROR", spec["name"], repr(exc))
            traceback.print_exc()
            results.append((spec["name"], "ERROR: %s" % exc))
    print("\nRESULTADOS:")
    for k, r in results:
        print("  ", k, r)


main()
