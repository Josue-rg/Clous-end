"""
CLOUS-END · LOD1 / LOD2 / _COL

Para cada asset animado abre su .blend, genera dos niveles de detalle y una
malla de colision convexa. Exporta FBX separados.
Salida: VAT/<Asset>/LOD/
"""

import bpy
import bmesh
import os
import math
import numpy as np
import shutil
import tempfile

LIB_DIR = globals().get("__file__")
LIB_DIR = (os.path.dirname(os.path.abspath(LIB_DIR)) if LIB_DIR
           else r"C:\Users\Josue\Desktop\Clous-end\BlenderSource\scripts")
LIB = os.path.join(LIB_DIR, "vat_rig_lib.py")
g = {"__name__": "vat_rig_lib"}
exec(compile(open(LIB, encoding="utf-8").read(), LIB, "exec"), g)

export_static_fbx = g["export_static_fbx"]
write_manifest = g["write_manifest"]
tri_count = g["tri_count"]

SRC = r"C:\Users\Josue\Desktop\Clous-end\BlenderSource"
OUT = os.path.join(SRC, "VAT")

ASSETS = ["Ship_Recolector"]


def find_mesh():
    for o in bpy.data.objects:
        if o.type == 'MESH' and o.name.endswith("_Game"):
            return o
    for o in bpy.data.objects:
        if o.type == 'MESH':
            return o
    return None


def duplicate_mesh(src, name, ratio):
    dup = src.copy()
    dup.data = src.data.copy()
    dup.name = name
    dup.data.name = name
    for c in src.users_collection:
        c.objects.link(dup)
    for m in list(dup.modifiers):
        dup.modifiers.remove(m)
    dup.parent = None
    dup.matrix_world = src.matrix_world.copy()

    if ratio < 1.0:
        mod = dup.modifiers.new("Dec", 'DECIMATE')
        mod.ratio = ratio
        dg = bpy.context.evaluated_depsgraph_get()
        ev = dup.evaluated_get(dg)
        new_me = bpy.data.meshes.new_from_object(ev)
        new_me.name = name
        old_me = dup.data
        dup.data = new_me
        dup.modifiers.clear()
        if old_me.users == 0:
            bpy.data.meshes.remove(old_me)
    return dup


def convex_collision(src, name):
    dup = duplicate_mesh(src, name, 0.04)
    bm = bmesh.new()
    bm.from_mesh(dup.data)
    res = bmesh.ops.convex_hull(bm, input=list(bm.verts), use_existing_faces=False)
    # borrar geometria interior
    interior = [e for e in res["geom_interior"] if isinstance(e, bmesh.types.BMVert)]
    if interior:
        bmesh.ops.delete(bm, geom=interior, context='VERTS')
    bm.to_mesh(dup.data)
    bm.free()
    for m in list(dup.modifiers):
        dup.modifiers.remove(m)
    return dup


def load_mesh(path, asset):
    """Limpia la escena y anade la malla del asset (sin open_mainfile)."""
    for obj in list(bpy.data.objects):
        bpy.data.objects.remove(obj, do_unlink=True)
    for coll in list(bpy.data.collections):
        bpy.data.collections.remove(coll)
    target = "SK_%s_Game" % asset
    load_path = path
    if os.path.abspath(path) == os.path.abspath(bpy.data.filepath):
        load_path = os.path.join(tempfile.gettempdir(), "lod_src_%s.blend" % asset)
        shutil.copy2(path, load_path)
    with bpy.data.libraries.load(load_path, link=False) as (src, dst):
        names = [n for n in src.objects if n == target]
        dst.objects = names
    mesh = None
    for o in dst.objects:
        if o is None:
            continue
        if not o.users_collection:
            bpy.context.scene.collection.objects.link(o)
        if o.type == 'MESH':
            mesh = o
    return mesh


def process(asset):
    path = os.path.join(OUT, asset, "SK_%s_vat.blend" % asset)
    if not os.path.exists(path):
        print("FALTA", path)
        return False
    src = load_mesh(path, asset)
    if src is None:
        print("sin malla", asset)
        return False

    out_dir = os.path.join(OUT, asset, "LOD")
    os.makedirs(out_dir, exist_ok=True)
    base_tris = tri_count(src)
    levels = []

    for tag, ratio in (("LOD1", 0.45), ("LOD2", 0.18)):
        dup = duplicate_mesh(src, "SK_%s_%s" % (asset, tag), ratio)
        fbx = os.path.join(out_dir, "SK_%s_%s.fbx" % (asset, tag))
        export_static_fbx(dup, fbx)
        levels.append(dict(level=tag, mesh=os.path.basename(fbx),
                           triangles=tri_count(dup)))
        bpy.data.objects.remove(dup, do_unlink=True)

    col = convex_collision(src, "SK_%s_COL" % asset)
    col_fbx = os.path.join(out_dir, "SK_%s_COL.fbx" % asset)
    export_static_fbx(col, col_fbx)
    col_tris = tri_count(col)
    bpy.data.objects.remove(col, do_unlink=True)

    write_manifest(os.path.join(out_dir, "lod_%s.json" % asset), {
        "asset": asset,
        "lod0_triangles": base_tris,
        "levels": levels,
        "collision": {"mesh": os.path.basename(col_fbx), "triangles": col_tris},
    })
    print("OK", asset, base_tris, "->", [l["triangles"] for l in levels],
          "col", col_tris)
    return True


def main():
    done = 0
    for a in ASSETS:
        try:
            if process(a):
                done += 1
        except Exception as exc:
            print("ERROR", a, repr(exc))
    print("TOTAL", done, "de", len(ASSETS))


main()
