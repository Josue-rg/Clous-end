"""
CLOUS-END · hojas de contacto de las animaciones VAT

Para cada asset animado abre el .blend, encuadra y renderiza 4 frames,
los une en una tira horizontal y la guarda en Previews/.
"""

import bpy
import os
import math
import numpy as np
from mathutils import Vector

BASE = r"C:\Users\Josue\Desktop\Clous-end\BlenderSource"
OUT = r"C:\Users\Josue\Desktop\CLOUS-END_Modelos3D\Previews"
TMP = r"C:\Users\Josue\AppData\Local\Temp\opencode\vat_frames"

ASSETS = [
    ("Santa", "011_Santa_Idle.png", 4),
    ("Sleigh", "012_Trineo_Vuelo.png", 4),
    ("Sabueso", "013_Sabueso_Carrera.png", 4),
    ("Garfio", "014_Garfio_Hover.png", 4),
    ("Espora", "015_Espora_Pulso.png", 4),
    ("Robot_Manto", "016_Robot_Manto.png", 4),
    ("Robot_Torre", "017_Robot_Torre.png", 4),
    ("NicolasCero", "018_Boss_Idle.png", 4),
    ("Ship_Caza", "019_Nave_Caza.png", 4),
    ("Ship_Recolector", "020_Nave_Recolector.png", 4),
]


def add_studio():
    world = bpy.data.worlds.get("W_Prev") or bpy.data.worlds.new("W_Prev")
    bpy.context.scene.world = world
    world.use_nodes = True
    bg = world.node_tree.nodes.get('Background')
    if bg:
        bg.inputs['Color'].default_value = (0.05, 0.055, 0.07, 1)
        bg.inputs['Strength'].default_value = 0.6

    coll = bpy.data.collections.new("PREVIEW")
    bpy.context.scene.collection.children.link(coll)

    def area(name, loc, rot, energy, size, color):
        d = bpy.data.lights.new(name, 'AREA')
        d.energy = energy
        d.size = size
        d.color = color
        o = bpy.data.objects.new(name, d)
        coll.objects.link(o)
        o.location = loc
        o.rotation_euler = rot
        return o

    area("L_Key", (3.0, -4.0, 4.2), (math.radians(52), 0, math.radians(38)),
         900, 3.0, (1.0, 0.93, 0.84))
    area("L_Fill", (-4.0, -2.2, 2.2), (math.radians(72), 0, math.radians(-62)),
         260, 4.0, (0.62, 0.74, 1.0))
    area("L_Rim", (-1.6, 4.4, 3.4), (math.radians(115), 0, math.radians(200)),
         700, 2.4, (0.75, 0.88, 1.0))

    import bmesh
    bm = bmesh.new()
    s = 12.0
    vs = [bm.verts.new((-s, -s, -0.01)), bm.verts.new((s, -s, -0.01)),
          bm.verts.new((s, s, -0.01)), bm.verts.new((-s, s, -0.01))]
    bm.faces.new(vs)
    me = bpy.data.meshes.new("PREVIEW_Floor")
    bm.to_mesh(me)
    bm.free()
    floor = bpy.data.objects.new("PREVIEW_Floor", me)
    m = bpy.data.materials.new("M_PrevFloor")
    m.use_nodes = True
    b = next(n for n in m.node_tree.nodes if n.type == 'BSDF_PRINCIPLED')
    b.inputs['Base Color'].default_value = (0.035, 0.038, 0.045, 1)
    b.inputs['Roughness'].default_value = 0.45
    floor.data.materials.append(m)
    for c in list(floor.users_collection):
        c.objects.unlink(floor)
    coll.objects.link(floor)


def bounds(obj):
    dg = bpy.context.evaluated_depsgraph_get()
    ev = obj.evaluated_get(dg)
    mn = Vector((1e9, 1e9, 1e9))
    mx = Vector((-1e9, -1e9, -1e9))
    for c in ev.bound_box:
        w = ev.matrix_world @ Vector(c)
        for i in range(3):
            mn[i] = min(mn[i], w[i])
            mx[i] = max(mx[i], w[i])
    return mn, mx


def render_asset(name, out_name, nframes):
    path = os.path.join(BASE, "VAT", name, "SK_%s_vat.blend" % name)
    if not os.path.exists(path):
        print("FALTA", path)
        return False
    bpy.ops.wm.open_mainfile(filepath=path)
    add_studio()

    scene = bpy.context.scene
    obj = next((o for o in bpy.data.objects
                if o.type == 'MESH' and o.name.endswith("_Game")), None)
    if obj is None:
        print("sin mesh en", name)
        return False

    scene.frame_set(1)
    mn, mx = bounds(obj)
    center = (mn + mx) * 0.5
    dims = mx - mn
    maxdim = max(dims.x, dims.y, dims.z)

    cam_data = bpy.data.cameras.new("CAM")
    cam_data.lens = 60
    cam = bpy.data.objects.new("CAM", cam_data)
    scene.collection.objects.link(cam)
    az = math.radians(34)
    el = math.radians(14)
    dist = (maxdim * 0.5) / math.tan(math.radians(16.5)) * 1.05
    loc = center + Vector((math.cos(el) * math.sin(az),
                           -math.cos(el) * math.cos(az),
                           math.sin(el))) * dist
    cam.location = loc
    cam.rotation_euler = (center - loc).to_track_quat('-Z', 'Y').to_euler()
    scene.camera = cam

    scene.render.engine = 'BLENDER_EEVEE'
    scene.render.resolution_x = 640
    scene.render.resolution_y = 640
    scene.render.film_transparent = False
    try:
        scene.eevee.taa_render_samples = 16
    except Exception:
        pass

    total = int(scene.frame_end)
    frames = [1 + int(round(i * 24 / nframes)) for i in range(nframes)]

    tiles = []
    os.makedirs(TMP, exist_ok=True)
    for i, f in enumerate(frames):
        scene.frame_set(f)
        fp = os.path.join(TMP, "%s_%02d.png" % (name, i))
        scene.render.filepath = fp
        bpy.ops.render.render(write_still=True)
        tiles.append(fp)

    # unir horizontalmente
    imgs = []
    for fp in tiles:
        img = bpy.data.images.load(fp)
        w, h = img.size
        arr = np.array(img.pixels[:], dtype=np.float32).reshape(h, w, 4)
        imgs.append(arr)
        bpy.data.images.remove(img)
    strip = np.concatenate(imgs, axis=1)
    hh, ww = strip.shape[0], strip.shape[1]
    out = bpy.data.images.new("strip", width=ww, height=hh, alpha=True)
    out.pixels.foreach_set(strip.reshape(-1))
    out.filepath_raw = os.path.join(OUT, out_name)
    out.file_format = 'PNG'
    out.save()
    bpy.data.images.remove(out)
    print("OK", out_name, ww, hh)
    return True


def main():
    os.makedirs(OUT, exist_ok=True)
    done = 0
    for name, out_name, nf in ASSETS:
        try:
            if render_asset(name, out_name, nf):
                done += 1
        except Exception as exc:
            print("ERROR", name, repr(exc))
    print("TOTAL", done, "de", len(ASSETS))


main()
