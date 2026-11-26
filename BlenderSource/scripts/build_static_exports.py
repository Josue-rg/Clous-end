"""
CLOUS-END · export estatico (sin animacion)

Armas, props, pickups y arnes: join + UV + FBX + manifiesto.
"""

import bpy
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
export_static_fbx = g["export_static_fbx"]
write_manifest = g["write_manifest"]
tri_count = g["tri_count"]

SRC = r"C:\Users\Josue\Desktop\Clous-end\BlenderSource"
OUT = os.path.join(SRC, "VAT", "Static")

WEAPONS = ["SM_Weapon_Carbon", "SM_Weapon_Nogal", "SM_Weapon_Estrella",
           "SM_Weapon_Cascabel"]
PROPS = ["SM_GiftBox_Weapon", "SM_Pickup_Ammo", "SM_Pickup_HelmetKit",
         "SM_Pickup_ReindeerShield", "SM_Collar", "SM_Prop_Bells",
         "SM_Meteor_1", "SM_Meteor_2", "SM_Meteor_3"]


def load_objects(path, names):
    with bpy.data.libraries.load(path, link=False) as (src, dst):
        picked = [n for n in src.objects if n in names]
        dst.objects = picked
    scene_coll = bpy.context.scene.collection
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
    return meshes


def export_set(src_path, names, coll=None):
    clear_scene()
    if coll:
        coll_obj = append_collection(src_path, coll)
        meshes = [o for o in coll_obj.all_objects if o.type == 'MESH']
        label = coll
    else:
        meshes = load_objects(src_path, names)
        label = names[0]
    if not meshes:
        raise RuntimeError("sin mallas para %s" % label)
    obj = join_meshes(meshes, "SM_%s_Game" % label.replace("SM_", ""),
                      origin=(0, 0, 0))
    unwrap_uv0(obj)
    out_dir = os.path.join(OUT, label)
    os.makedirs(out_dir, exist_ok=True)
    fbx = os.path.join(out_dir, "%s.fbx" % label)
    export_static_fbx(obj, fbx)
    write_manifest(os.path.join(out_dir, "asset_%s.json" % label), {
        "asset": label,
        "mesh": os.path.basename(fbx),
        "triangles": tri_count(obj),
        "vertices": len(obj.data.vertices),
        "animated": False,
    })
    return True


def main():
    results = []
    for name in WEAPONS:
        try:
            export_set(os.path.join(SRC, "Weapons", "SM_Weapons.blend"), [name])
            results.append((name, "OK"))
        except Exception as exc:
            results.append((name, "ERROR %s" % exc))

    for name in PROPS:
        try:
            export_set(os.path.join(SRC, "Props", "SM_Props.blend"), [name])
            results.append((name, "OK"))
        except Exception as exc:
            results.append((name, "ERROR %s" % exc))

    try:
        export_set(os.path.join(SRC, "Props", "SM_Harness_Set.blend"), [],
                   coll="SM_Harness_Set")
        results.append(("SM_Harness_Set", "OK"))
    except Exception as exc:
        results.append(("SM_Harness_Set", "ERROR %s" % exc))

    print("\nRESULTADOS:")
    for k, r in results:
        print("  ", k, r)


main()
