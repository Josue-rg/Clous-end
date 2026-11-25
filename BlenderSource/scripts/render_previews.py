"""
CLOUS-END · render de revision

Abre cada .blend, encuadra el asset y guarda un PNG de revision.
Salida: C:\\Users\\Josue\\Desktop\\CLOUS-END_Modelos3D\\Previews
"""

import bpy
import math
import os
from mathutils import Vector

SRC = r"C:\Users\Josue\Desktop\BlenderSource"
OUT = r"C:\Users\Josue\Desktop\CLOUS-END_Modelos3D\Previews"

FILES = [
    ("Characters/SK_Santa.blend", "01_Santa.png"),
    ("Characters/SK_Reindeer.blend", "02_Renos.png"),
    ("Vehicles/SM_Sleigh.blend", "03_Trineo.png"),
    ("Weapons/SM_Weapons.blend", "04_Armas.png"),
    ("Enemies/SK_Yermos.blend", "05_Enemigos.png"),
    ("Ships/SK_Ships.blend", "06_Naves.png"),
    ("Boss/SK_NicolasCero.blend", "07_Boss.png"),
    ("Props/SM_Props.blend", "08_Props.png"),
    ("Props/SM_Harness_Set.blend", "09_Arnes.png"),
    ("Review/Hero.blend", "10_Conjunto.png"),
]

BASE = r"C:\Users\Josue\Desktop\Clous-end\BlenderSource"


def clean_bounds():
    mn = Vector((1e9, 1e9, 1e9))
    mx = Vector((-1e9, -1e9, -1e9))
    dg = bpy.context.evaluated_depsgraph_get()
    for o in bpy.data.objects:
        if o.type != 'MESH':
            continue
        if any(c.name.startswith("PREVIEW") for c in o.users_collection):
            continue
        ev = o.evaluated_get(dg)
        for corner in ev.bound_box:
            w = ev.matrix_world @ Vector(corner)
            for i in range(3):
                mn[i] = min(mn[i], w[i])
                mx[i] = max(mx[i], w[i])
    return mn, mx


def render_one(rel, out_name):
    path = os.path.join(BASE, rel.replace("/", os.sep))
    if not os.path.exists(path):
        print("FALTA", path)
        return False
    bpy.ops.wm.open_mainfile(filepath=path)
    scene = bpy.context.scene

    mn, mx = clean_bounds()
    if mx.x < -1e8:
        print("sin geometria", rel)
        return False
    center = (mn + mx) * 0.5
    dims = mx - mn
    maxdim = max(dims.x, dims.y, dims.z)

    cam_data = bpy.data.cameras.new("REVIEW_CAM")
    cam_data.lens = 58
    cam = bpy.data.objects.new("REVIEW_CAM", cam_data)
    scene.collection.objects.link(cam)
    az = math.radians(36)
    el = math.radians(15)
    dist = (maxdim * 0.5) / math.tan(math.radians(17.5)) * 1.06
    loc = center + Vector((math.cos(el) * math.sin(az),
                           -math.cos(el) * math.cos(az),
                           math.sin(el))) * dist
    cam.location = loc
    cam.rotation_euler = (center - loc).to_track_quat('-Z', 'Y').to_euler()
    scene.camera = cam

    scene.render.engine = 'BLENDER_EEVEE'
    scene.render.resolution_x = 1400
    scene.render.resolution_y = 1000
    scene.render.film_transparent = False
    try:
        scene.eevee.taa_render_samples = 24
    except Exception:
        pass
    scene.render.filepath = os.path.join(OUT, out_name)
    bpy.ops.render.render(write_still=True)
    print("OK", out_name)
    return True


def main():
    os.makedirs(OUT, exist_ok=True)
    done = 0
    for rel, out in FILES:
        try:
            if render_one(rel, out):
                done += 1
        except Exception as exc:
            print("ERROR", rel, repr(exc))
    print("TOTAL", done, "de", len(FILES))


main()
