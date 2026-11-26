"""
CLOUS-END · VAT del reno (4 variantes, multi-clip)

Gallop · Death · Hurt
Salida: BlenderSource/VAT/Reindeer_<Nombre>/
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
purge_actions = g["purge_actions"]
make_clip = g["make_clip"]
make_clip_pose = g["make_clip_pose"]
add_vat_uv = g["add_vat_uv"]
bake_vat = g["bake_vat"]
vat_bounds = g["vat_bounds"]
export_static_fbx = g["export_static_fbx"]
write_manifest = g["write_manifest"]
save_blend = g["save_blend"]
tri_count = g["tri_count"]

SRC = r"C:\Users\Josue\Desktop\Clous-end\BlenderSource"
OUT = os.path.join(SRC, "VAT")
REINDEER_BLEND = os.path.join(SRC, "Characters", "SK_Reindeer.blend")

VARIANTS = [
    dict(key="Rodolfo", scale=1.00, width=1.00, leg=0.00, target=24000),
    dict(key="Donner", scale=1.07, width=1.12, leg=0.02, target=24000),
    dict(key="Vixen", scale=0.93, width=0.93, leg=-0.02, target=22000),
    dict(key="Cometa", scale=1.00, width=0.96, leg=0.070, target=22000),
]

FPS = 30


def bones_for(cfg):
    sc = cfg["scale"]
    wid = cfg["width"] * sc
    le = cfg["leg"]

    def bz(z):
        return z * sc + le

    def lz(z):
        t = max(0.0, min(1.0, (z - 0.055) / (1.040 - 0.055)))
        return z * sc + le * t

    def P(x, y, z, leg=False):
        return (x * wid, y * sc, lz(z) if leg else bz(z))

    bones = [
        dict(name="Root", head=(0, 0.20, 0.00), tail=(0, 0.20, 0.30), deform=False),
        dict(name="Spine", head=P(0, 0.62, 1.08), tail=P(0, -0.45, 1.19), parent="Root"),
        dict(name="Chest", head=P(0, -0.45, 1.19), tail=P(0, -0.84, 1.21), parent="Spine"),
        dict(name="Neck", head=P(0, -0.84, 1.21), tail=P(0, -1.16, 1.53), parent="Chest"),
        dict(name="Head", head=P(0, -1.16, 1.53), tail=P(0, -1.62, 1.46), parent="Neck"),
        dict(name="Tail", head=P(0, 0.64, 1.12), tail=P(0, 0.96, 1.13), parent="Spine"),
    ]
    for tag, sx in (("FL", -1), ("FR", 1)):
        bones += [
            dict(name=tag + "_Upper", head=P(sx * 0.155, -0.545, 1.040, True),
                 tail=P(sx * 0.166, -0.470, 0.780, True), parent="Chest"),
            dict(name=tag + "_Lower", head=P(sx * 0.166, -0.470, 0.780, True),
                 tail=P(sx * 0.170, -0.530, 0.330, True), parent=tag + "_Upper"),
            dict(name=tag + "_Hoof", head=P(sx * 0.170, -0.530, 0.330, True),
                 tail=P(sx * 0.163, -0.570, 0.050, True), parent=tag + "_Lower"),
        ]
    for tag, sx in (("HL", -1), ("HR", 1)):
        bones += [
            dict(name=tag + "_Upper", head=P(sx * 0.168, 0.585, 1.055, True),
                 tail=P(sx * 0.196, 0.690, 0.800, True), parent="Spine"),
            dict(name=tag + "_Lower", head=P(sx * 0.196, 0.690, 0.800, True),
                 tail=P(sx * 0.185, 0.510, 0.320, True), parent=tag + "_Upper"),
            dict(name=tag + "_Hoof", head=P(sx * 0.185, 0.510, 0.320, True),
                 tail=P(sx * 0.178, 0.515, 0.050, True), parent=tag + "_Lower"),
        ]
    return bones


def clip_gallop(frames=24):
    tr = []
    for tag, ph, k, amp in (("FL", 0.00, 1.00, 1.00), ("FR", 0.50, 1.00, 1.00),
                            ("HL", 0.24, 1.00, 1.05), ("HR", 0.74, 1.00, 1.05)):
        tr += [(tag + "_Upper", 0, 34 * amp, ph, k, 0),
               (tag + "_Lower", 0, -42 * amp, ph + 0.10, k, 0),
               (tag + "_Hoof", 0, 26 * amp, ph + 0.16, k, 0)]
    tr += [("Spine", 0, 3.0, 0.15, 1.0, 0), ("Chest", 0, 2.0, 0.20, 1.0, 0),
           ("Neck", 0, 4.0, 0.22, 1.0, 2.0), ("Head", 0, 5.0, 0.26, 1.0, -2.0),
           ("Tail", 0, 9.0, 0.10, 1.0, 0)]
    return dict(kind="sine", name="Gallop", frames=frames, tracks=tr,
                loc=[("Root", 1, 0.045, 0.12, 2.0, 0.0),
                     ("Spine", 2, 0.012, 0.30, 1.0, 0.0)])


def clip_death(frames=38):
    """Se desploma de lado. Nunca explota: cae y el arnes queda colgando."""
    poses = [
        (1, {}, {}),
        (5, {"Neck": (18, 0, 0), "Head": (14, 0, 0),
             "FL_Upper": (16, 0, 0), "FR_Upper": (14, 0, 0),
             "FL_Lower": (-18, 0, 0), "FR_Lower": (-16, 0, 0)},
         {"Root": (0, -0.04, 0)}),
        (11, {"Neck": (26, 0, 0), "Head": (22, 0, 0), "Spine": (6, 0, 0),
              "FL_Upper": (40, 0, 0), "FR_Upper": (36, 0, 0),
              "FL_Lower": (-34, 0, 0), "FR_Lower": (-30, 0, 0),
              "HL_Upper": (-14, 0, 0), "HR_Upper": (-12, 0, 0)},
         {"Root": (0, -0.20, 0)}),
        (18, {"Root": (8, 0, 45), "Neck": (20, 0, 0), "Head": (26, 0, 0),
              "FL_Upper": (30, 0, 0), "FR_Upper": (22, 0, 0),
              "HL_Upper": (-22, 0, 0), "HR_Upper": (-26, 0, 0)},
         {"Root": (-0.10, -0.38, 0)}),
        (26, {"Root": (10, 0, 72), "Neck": (10, 0, 0), "Head": (30, 0, 0),
              "FL_Upper": (16, 0, 0), "FR_Upper": (10, 0, 0),
              "FL_Lower": (-10, 0, 0), "FR_Lower": (-6, 0, 0),
              "HL_Upper": (-16, 0, 0), "HR_Upper": (-20, 0, 0),
              "Tail": (10, 0, 0)},
         {"Root": (-0.16, -0.10, 0)}),
        (32, {"Root": (10, 0, 74), "Head": (32, 0, 0)},
         {"Root": (-0.18, -0.08, 0)}),
        (38, {"Root": (10, 0, 74), "Head": (30, 0, 0)},
         {"Root": (-0.18, -0.08, 0)}),
    ]
    return dict(kind="pose", name="Death", frames=frames, poses=poses)


def clip_hurt(frames=16):
    poses = [
        (1, {}, {}),
        (4, {"Spine": (-6, 0, -5), "Chest": (-4, 0, -4), "Neck": (-14, 0, 0),
             "Head": (-10, 0, 6),
             "HL_Upper": (-14, 0, 0), "HR_Upper": (10, 0, 0)},
         {"Root": (0.03, -0.05, 0)}),
        (9, {"Spine": (3, 0, 3), "Neck": (6, 0, 0), "Head": (4, 0, -4),
             "HL_Upper": (8, 0, 0), "HR_Upper": (-8, 0, 0)},
         {"Root": (-0.02, -0.02, 0)}),
        (16, {}, {}),
    ]
    return dict(kind="pose", name="Hurt", frames=frames, poses=poses)


CLIPS = [clip_gallop(), clip_death(), clip_hurt()]


def build_variant(cfg):
    key = cfg["key"]
    purge_actions()
    print("\n=== %s ===" % key)
    coll_src = append_collection(REINDEER_BLEND, "SK_Reindeer_%s" % key)
    meshes = [o for o in coll_src.all_objects if o.type == 'MESH']

    obj = join_meshes(meshes, "SK_Reindeer_%s_Game" % key, origin=(0, 0, 0))
    decimate_to(obj, cfg["target"])
    unwrap_uv0(obj)

    arm = make_armature("RIG_Reindeer_%s" % key, bones_for(cfg),
                        coll=bpy.context.scene.collection)
    skin_proximity(obj, arm, power=5.0, max_inf=3)

    max_frames = max(c["frames"] for c in CLIPS)
    rw, rows, height = add_vat_uv(obj, rw=1024, frames=max_frames)

    out_dir = os.path.join(OUT, "Reindeer_%s" % key)
    os.makedirs(out_dir, exist_ok=True)
    clips_meta = []

    for clip in CLIPS:
        frames = clip["frames"]
        if clip["kind"] == "sine":
            make_clip(arm, "AN_%s_%s" % (key, clip["name"]), frames,
                      clip["tracks"], clip.get("loc"), fps=FPS)
        else:
            make_clip_pose(arm, "AN_%s_%s" % (key, clip["name"]), frames,
                           clip["poses"], fps=FPS)

        exr = os.path.join(out_dir, "T_Reindeer_%s_%s_Pos.exr" % (key, clip["name"]))
        bake_vat(obj, exr, frames, rw=rw, start_frame=1, rows=rows)
        mn, mx = vat_bounds(obj, frames, start_frame=1)
        clips_meta.append({
            "name": clip["name"], "frames": frames, "fps": FPS,
            "loop": clip["kind"] == "sine",
            "texture": os.path.basename(exr),
            "texture_width": rw, "texture_height": rows * frames,
            "rows": rows,
            "bounds_min": [round(v, 4) for v in mn],
            "bounds_max": [round(v, 4) for v in mx],
        })

    fbx = os.path.join(out_dir, "SK_Reindeer_%s.fbx" % key)
    export_static_fbx(obj, fbx)

    write_manifest(os.path.join(out_dir, "vat_%s.json" % key), {
        "asset": "SK_Reindeer_%s" % key,
        "mesh": os.path.basename(fbx),
        "triangles": tri_count(obj),
        "vertices": len(obj.data.vertices),
        "clips": clips_meta,
    })
    save_blend(os.path.join(out_dir, "SK_Reindeer_%s_vat.blend" % key))
    return True


def main():
    results = []
    for cfg in VARIANTS:
        clear_scene()
        try:
            build_variant(cfg)
            results.append((cfg["key"], "OK"))
        except Exception as exc:
            print("ERROR", cfg["key"], repr(exc))
            traceback.print_exc()
            results.append((cfg["key"], "ERROR: %s" % exc))
    print("\nRESULTADOS:")
    for k, r in results:
        print("  ", k, r)


main()
