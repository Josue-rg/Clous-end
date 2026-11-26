"""
CLOUS-END · VAT (Vertex Animation Textures) · libreria

Pipeline:
  1. unir piezas -> malla de juego
  2. decimar al presupuesto
  3. UV0 (atlas futuro) + UV1 (indice de vertice para VAT)
  4. esqueleto + skin por proximidad (robusto, sin heat weighting)
  5. animacion procedural por curvas
  6. horneado: posicion absoluta por vertice y frame -> EXR
  7. export FBX estatico (sin armature) + manifiesto JSON

Textura VAT:
  ancho  = vertices por fila (1024)
  alto   = filas * frames
  pixel  = (x, y, z) de la posicion en espacio local del objeto
  UV1    = ((v % RW + 0.5)/RW , (v // RW + 0.5)/alto)
  El shader suma frame/alto a la Y.

Se ejecuta dentro de Blender (MCP).
"""

import bpy
import bmesh
import math
import os
import json
import numpy as np
from mathutils import Vector

TAU = math.tau
D2R = math.radians


# ---------------------------------------------------------------------------
# Escena / colecciones
# ---------------------------------------------------------------------------

def clear_scene():
    try:
        if bpy.context.view_layer.objects.active and bpy.context.view_layer.objects.active.mode != 'OBJECT':
            bpy.ops.object.mode_set(mode='OBJECT')
    except Exception:
        pass
    for obj in list(bpy.data.objects):
        bpy.data.objects.remove(obj, do_unlink=True)
    for coll in list(bpy.data.collections):
        bpy.data.collections.remove(coll)
    for block in (bpy.data.meshes, bpy.data.materials, bpy.data.armatures,
                  bpy.data.actions, bpy.data.images, bpy.data.lights,
                  bpy.data.cameras):
        for item in list(block):
            if item.users == 0:
                try:
                    block.remove(item)
                except Exception:
                    pass


def append_collection(path, name):
    with bpy.data.libraries.load(path, link=False) as (src, dst):
        if name not in src.collections:
            raise ValueError("coleccion no encontrada: %s en %s" % (name, path))
        dst.collections = [name]
    coll = dst.collections[0]
    try:
        bpy.context.scene.collection.children.link(coll)
    except Exception:
        pass
    return coll


def collection(name):
    coll = bpy.data.collections.get(name)
    if coll is None:
        coll = bpy.data.collections.new(name)
        bpy.context.scene.collection.children.link(coll)
    return coll


def select(objs, active=None):
    """Selecciona sin operadores (funciona tras abrir un .blend)."""
    for o in bpy.context.view_layer.objects:
        try:
            o.select_set(False)
        except Exception:
            pass
    for o in objs:
        try:
            o.select_set(True)
        except Exception:
            pass
    bpy.context.view_layer.objects.active = active or objs[0]


def apply_all_mods(obj):
    select([obj])
    for m in list(obj.modifiers):
        try:
            bpy.ops.object.modifier_apply(modifier=m.name)
        except Exception as exc:
            print("[mod]", obj.name, m.name, exc)
    return obj


def apply_transform(obj, location=True):
    select([obj])
    bpy.ops.object.transform_apply(location=location, rotation=True, scale=True)
    return obj


def join_meshes(objs, name, origin=(0, 0, 0)):
    objs = [o for o in objs if o and o.type == 'MESH']
    for o in objs:
        apply_all_mods(o)
    select(objs, objs[0])
    bpy.ops.object.join()
    obj = bpy.context.view_layer.objects.active
    obj.name = name
    obj.data.name = name
    # origen a la base
    bpy.context.scene.cursor.location = origin
    bpy.ops.object.origin_set(type='ORIGIN_CURSOR')
    apply_transform(obj, location=True)
    return obj


def tri_count(obj):
    me = obj.data
    return sum(max(0, len(p.vertices) - 2) for p in me.polygons)


def decimate_to(obj, target_tris):
    current = tri_count(obj)
    if current <= target_tris:
        return obj
    ratio = max(0.05, target_tris / float(current))
    select([obj])
    mod = obj.modifiers.new("Decimate", 'DECIMATE')
    mod.ratio = ratio
    bpy.ops.object.modifier_apply(modifier=mod.name)
    # limpiar
    bpy.ops.object.mode_set(mode='EDIT')
    bpy.ops.mesh.select_all(action='SELECT')
    bpy.ops.mesh.delete_loose()
    bpy.ops.mesh.remove_doubles(threshold=0.0005)
    bpy.ops.object.mode_set(mode='OBJECT')
    print("[decimate] %s -> %d tris (objetivo %d)" % (obj.name, tri_count(obj), target_tris))
    return obj


def unwrap_uv0(obj, angle=66.0, island_margin=0.005):
    select([obj])
    bpy.ops.object.mode_set(mode='EDIT')
    bpy.ops.mesh.select_all(action='SELECT')
    try:
        bpy.ops.uv.smart_project(angle_limit=D2R(angle),
                                 island_margin=island_margin,
                                 correct_aspect=True)
    except Exception as exc:
        print("[uv] smart_project fallo:", exc)
        bpy.ops.uv.unwrap(method='ANGLE_BASED', margin=0.005)
    bpy.ops.object.mode_set(mode='OBJECT')
    return obj


# ---------------------------------------------------------------------------
# Esqueleto
# ---------------------------------------------------------------------------

def make_armature(name, bones, coll=None):
    """bones: [dict(name, head, tail, parent=None, deform=True)]"""
    data = bpy.data.armatures.new(name)
    arm = bpy.data.objects.new(name, data)
    (coll or bpy.context.scene.collection).objects.link(arm)
    arm.show_in_front = True

    bpy.ops.object.select_all(action='DESELECT')
    arm.select_set(True)
    bpy.context.view_layer.objects.active = arm
    bpy.ops.object.mode_set(mode='EDIT')
    made = {}
    for spec in bones:
        eb = data.edit_bones.new(spec["name"])
        eb.head = Vector(spec["head"])
        eb.tail = Vector(spec["tail"])
        eb.roll = spec.get("roll", 0.0)
        eb.use_deform = spec.get("deform", True)
        if spec.get("parent"):
            eb.parent = made[spec["parent"]]
            eb.use_connect = False
        made[spec["name"]] = eb
    bpy.ops.object.mode_set(mode='OBJECT')
    for pb in arm.pose.bones:
        pb.rotation_mode = 'XYZ'
    return arm


def _seg_distance(pts, a, b):
    seg = b - a
    ll = float(seg.dot(seg))
    if ll < 1e-9:
        return np.linalg.norm(pts - a, axis=1)
    t = np.clip(((pts - a) @ seg) / ll, 0.0, 1.0)
    proj = a + t[:, None] * seg
    return np.linalg.norm(pts - proj, axis=1)


def skin_proximity(obj, arm, power=5.0, max_inf=3, min_weight=0.02):
    """Pesos por distancia al hueso. Robusto con mallas de varias piezas."""
    arm_data = arm.data
    deform = [b for b in arm_data.bones if b.use_deform]
    if not deform:
        raise ValueError("sin huesos deformables")

    me = obj.data
    nv = len(me.vertices)
    co = np.empty(nv * 3, dtype=np.float64)
    me.vertices.foreach_get("co", co)
    pts = co.reshape(nv, 3)

    dists = np.zeros((nv, len(deform)), dtype=np.float64)
    for i, bone in enumerate(deform):
        a = np.array(bone.head_local, dtype=np.float64)
        b = np.array(bone.tail_local, dtype=np.float64)
        dists[:, i] = _seg_distance(pts, a, b)

    w = 1.0 / np.power(np.maximum(dists, 1e-4), power)
    if max_inf < len(deform):
        idx = np.argpartition(-w, max_inf, axis=1)[:, :max_inf]
        mask = np.zeros_like(w, dtype=bool)
        np.put_along_axis(mask, idx, True, axis=1)
        w = np.where(mask, w, 0.0)
    w /= np.maximum(w.sum(axis=1, keepdims=True), 1e-12)
    w[w < min_weight] = 0.0
    w /= np.maximum(w.sum(axis=1, keepdims=True), 1e-12)

    for vg in list(obj.vertex_groups):
        obj.vertex_groups.remove(vg)
    groups = [obj.vertex_groups.new(name=b.name) for b in deform]

    for vi in range(nv):
        row = w[vi]
        nz = np.nonzero(row)[0]
        for gi in nz:
            groups[gi].add([vi], float(row[gi]), 'REPLACE')

    obj.parent = arm
    obj.matrix_parent_inverse = arm.matrix_world.inverted()
    mod = obj.modifiers.new("Armature", 'ARMATURE')
    mod.object = arm
    mod.use_vertex_groups = True
    print("[skin] %s: %d verts, %d huesos" % (obj.name, nv, len(deform)))
    return obj


# ---------------------------------------------------------------------------
# Animacion procedural
# ---------------------------------------------------------------------------

def clear_animation(arm):
    arm.animation_data_clear()
    for act in list(bpy.data.actions):
        if act.users == 0:
            bpy.data.actions.remove(act)


def purge_actions():
    """Elimina TODAS las acciones (evita AN_X.001, .002 entre rebuilds)."""
    try:
        for obj in bpy.data.objects:
            if obj.animation_data:
                obj.animation_data_clear()
    except Exception:
        pass
    for act in list(bpy.data.actions):
        try:
            bpy.data.actions.remove(act)
        except Exception:
            pass


def iter_fcurves(action):
    """Blender 4.4+ guarda las curvas en layers/strips/channelbags."""
    try:
        curves = list(action.fcurves)
        if curves:
            return curves
    except Exception:
        pass
    out = []
    for layer in getattr(action, "layers", []):
        for strip in layer.strips:
            for cb in getattr(strip, "channelbags", []):
                out.extend(list(cb.fcurves))
    return out


def make_clip(arm, name, frames, tracks, loc_tracks=None, fps=30):
    """
    tracks:     list of (bone, axis(0=x,1=y,2=z), amp_deg, phase, freq, offset_deg)
    loc_tracks: list of (bone, axis, amp_m, phase, freq, offset_m)
    Genera un loop: frame 1 == frame frames+1
    """
    clear_animation(arm)
    arm.animation_data_create()
    action = bpy.data.actions.new(name)
    try:
        arm.animation_data.action = action
    except Exception:
        arm.animation_data.action = action
    if hasattr(arm.animation_data, "action_slot"):
        for slot in action.slots:
            arm.animation_data.action_slot = slot
            break
    scene = bpy.context.scene
    scene.render.fps = fps

    for f in range(1, frames + 2):
        t = (f - 1) / float(frames)
        acc = {}
        for bone, axis, amp, phase, freq, off in tracks:
            key = (bone, axis)
            ang = off + amp * math.sin(TAU * (t * freq + phase))
            acc[key] = acc.get(key, 0.0) + ang
        for (bone, axis), ang in acc.items():
            pb = arm.pose.bones.get(bone)
            if pb is None:
                continue
            pb.rotation_mode = 'XYZ'
            e = list(pb.rotation_euler)
            e[axis] = D2R(ang)
            pb.rotation_euler = e
            pb.keyframe_insert("rotation_euler", frame=f, group=bone)

        if loc_tracks:
            lacc = {}
            for bone, axis, amp, phase, freq, off in loc_tracks:
                key = (bone, axis)
                val = off + amp * math.sin(TAU * (t * freq + phase))
                lacc[key] = lacc.get(key, 0.0) + val
            for (bone, axis), val in lacc.items():
                pb = arm.pose.bones.get(bone)
                if pb is None:
                    continue
                loc = list(pb.location)
                loc[axis] = val
                pb.location = loc
                pb.keyframe_insert("location", frame=f, group=bone)

    # interpolacion suave
    for fc in iter_fcurves(action):
        for kp in fc.keyframe_points:
            kp.interpolation = 'BEZIER'
            kp.handle_left_type = 'AUTO_CLAMPED'
            kp.handle_right_type = 'AUTO_CLAMPED'
    action.use_fake_user = True
    print("[clip] %s %d frames" % (name, frames))
    return action


def make_clip_pose(arm, name, frames, poses, fps=30):
    """
    Clip de una sola pasada (no loop).
    poses: [(frame, {bone: (rx,ry,rz)}, {bone: (lx,ly,lz)})]  grados / metros
    """
    clear_animation(arm)
    arm.animation_data_create()
    action = bpy.data.actions.new(name)
    arm.animation_data.action = action
    bpy.context.scene.render.fps = fps

    def key(frame, bone, rot=None, loc=None):
        pb = arm.pose.bones.get(bone)
        if pb is None:
            return
        pb.rotation_mode = 'XYZ'
        if rot is not None:
            pb.rotation_euler = (D2R(rot[0]), D2R(rot[1]), D2R(rot[2]))
            pb.keyframe_insert("rotation_euler", frame=frame, group=bone)
        if loc is not None:
            pb.location = loc
            pb.keyframe_insert("location", frame=frame, group=bone)

    # frame 1: todo a cero para no arrastrar poses
    for pb in arm.pose.bones:
        key(1, pb.name, rot=(0, 0, 0), loc=(0, 0, 0))

    for frame, rots, locs in poses:
        for bone, rot in (rots or {}).items():
            key(frame, bone, rot=rot)
        for bone, loc in (locs or {}).items():
            key(frame, bone, loc=loc)

    for fc in iter_fcurves(action):
        for kp in fc.keyframe_points:
            kp.interpolation = 'BEZIER'
            kp.handle_left_type = 'AUTO_CLAMPED'
            kp.handle_right_type = 'AUTO_CLAMPED'
    action.use_fake_user = True
    print("[clip pose] %s %d frames" % (name, frames))
    return action


# ---------------------------------------------------------------------------
# VAT
# ---------------------------------------------------------------------------

def add_vat_uv(obj, rw=1024, frames=24):
    """Crea UV1 con el indice de vertice. Devuelve (rw, rows, height)."""
    nv = len(obj.data.vertices)
    rows = int(math.ceil(nv / float(rw)))
    height = rows * frames
    me = obj.data

    while len(me.uv_layers) < 2:
        me.uv_layers.new(name="UVMap" if len(me.uv_layers) == 0 else "VAT")

    uv = me.uv_layers[1]
    uv.name = "VAT"
    for i in range(len(uv.data)):
        uv.data[i].uv = (0.0, 0.0)

    loop_vert = np.empty(len(me.loops), dtype=np.int32)
    me.loops.foreach_get("vertex_index", loop_vert)

    r = loop_vert // rw
    c = loop_vert % rw
    u = (c + 0.5) / float(rw)
    v = (r + 0.5) / float(height)

    arr = np.empty(len(me.loops) * 2, dtype=np.float32)
    arr[0::2] = u
    arr[1::2] = v
    uv.data.foreach_set("uv", arr)
    me.uv_layers.active_index = 1
    print("[vat uv] %d verts, %d rows, textura %dx%d" % (nv, rows, rw, height))
    return rw, rows, height


def bake_vat(obj, out_path, frames, rw=1024, start_frame=1, rows=None):
    """Hornea posiciones absolutas (espacio local del objeto) a un EXR."""
    scene = bpy.context.scene
    nv = len(obj.data.vertices)
    rows = rows or int(math.ceil(nv / float(rw)))
    height = rows * frames
    width = rw

    buf = np.zeros((height, width, 4), dtype=np.float32)
    buf[..., 3] = 1.0

    scene.frame_set(start_frame)
    base_count = nv

    for fi in range(frames):
        scene.frame_set(start_frame + fi)
        dg = bpy.context.evaluated_depsgraph_get()
        ev = obj.evaluated_get(dg)
        me = ev.to_mesh()
        if len(me.vertices) != base_count:
            ev.to_mesh_clear()
            raise RuntimeError("topologia cambiante: %d != %d" % (len(me.vertices), base_count))
        co = np.empty(nv * 3, dtype=np.float32)
        me.vertices.foreach_get("co", co)
        ev.to_mesh_clear()
        co = co.reshape(nv, 3)
        r = np.arange(nv) // rw
        c = np.arange(nv) % rw
        buf[fi * rows + r, c, :3] = co

    img_name = os.path.splitext(os.path.basename(out_path))[0]
    if img_name in bpy.data.images:
        bpy.data.images.remove(bpy.data.images[img_name])
    img = bpy.data.images.new(img_name, width=width, height=height,
                              float_buffer=True, alpha=True)
    img.pixels.foreach_set(buf.reshape(-1))
    os.makedirs(os.path.dirname(out_path), exist_ok=True)

    # guardar EXR 16 bits + ZIP (mitad de peso, precision 0.03 mm)
    scene = bpy.context.scene
    rs = scene.render.image_settings
    old_fmt, old_depth, old_codec = rs.file_format, rs.color_depth, rs.exr_codec
    old_view, old_look = scene.view_settings.view_transform, scene.view_settings.look
    try:
        rs.file_format = 'OPEN_EXR'
        rs.color_depth = '16'
        rs.exr_codec = 'ZIP'
        scene.view_settings.view_transform = 'Raw'
        scene.view_settings.look = 'None'
        img.save_render(out_path, scene=scene)
    except Exception as exc:
        print("[vat] save_render fallo, guardo 32 bits:", exc)
        img.filepath_raw = out_path
        img.file_format = 'OPEN_EXR'
        img.save()
    finally:
        rs.file_format, rs.color_depth, rs.exr_codec = old_fmt, old_depth, old_codec
        scene.view_settings.view_transform = old_view
        scene.view_settings.look = old_look
    bpy.data.images.remove(img)
    size_mb = os.path.getsize(out_path) / (1024.0 * 1024.0)
    print("[vat] %s  %dx%d  %d frames  %.2f MB" % (
        os.path.basename(out_path), width, height, frames, size_mb))
    return width, height


def vat_bounds(obj, frames, start_frame=1):
    scene = bpy.context.scene
    mn = np.array([1e9, 1e9, 1e9])
    mx = np.array([-1e9, -1e9, -1e9])
    nv = len(obj.data.vertices)
    for fi in range(frames):
        scene.frame_set(start_frame + fi)
        dg = bpy.context.evaluated_depsgraph_get()
        ev = obj.evaluated_get(dg)
        me = ev.to_mesh()
        co = np.empty(nv * 3, dtype=np.float32)
        me.vertices.foreach_get("co", co)
        ev.to_mesh_clear()
        co = co.reshape(nv, 3)
        mn = np.minimum(mn, co.min(axis=0))
        mx = np.maximum(mx, co.max(axis=0))
    return mn.tolist(), mx.tolist()


# ---------------------------------------------------------------------------
# Export
# ---------------------------------------------------------------------------

def export_static_fbx(obj, path):
    """Exporta solo la malla, sin armature ni animacion (copia temporal)."""
    tmp = obj.copy()
    tmp.data = obj.data.copy()
    tmp.name = obj.name + "_EXPORT"
    for c in obj.users_collection:
        c.objects.link(tmp)
    for m in list(tmp.modifiers):
        tmp.modifiers.remove(m)
    tmp.parent = None
    tmp.matrix_world = obj.matrix_world.copy()
    select([tmp])
    os.makedirs(os.path.dirname(path), exist_ok=True)
    bpy.ops.export_scene.fbx(
        filepath=path,
        use_selection=True,
        apply_unit_scale=True,
        apply_scale_options='FBX_SCALE_ALL',
        axis_forward='-Z',
        axis_up='Y',
        object_types={'MESH'},
        use_mesh_modifiers=False,
        mesh_smooth_type='FACE',
        use_tspace=False,
        bake_anim=False,
        add_leaf_bones=False,
        path_mode='AUTO',
    )
    bpy.data.objects.remove(tmp, do_unlink=True)
    print("[fbx]", path)
    return path


def simulate_vat(obj, exr_path, frame, rw, rows):
    """Aplica la textura al mesh (para verificar el horneado). Devuelve un duplicado."""
    img = bpy.data.images.load(exr_path, check_existing=False)
    w, h = img.size
    px = np.array(img.pixels[:], dtype=np.float32).reshape(h, w, 4)
    nv = len(obj.data.vertices)
    block = px[frame * rows:(frame + 1) * rows, :, :3].reshape(-1, 3)[:nv]
    dup = obj.copy()
    dup.data = obj.data.copy()
    dup.name = obj.name + "_VATSIM"
    for c in obj.users_collection:
        c.objects.link(dup)
    for m in list(dup.modifiers):
        dup.modifiers.remove(m)
    for i, v in enumerate(dup.data.vertices):
        v.co = block[i]
    dup.data.update()
    bpy.data.images.remove(img)
    return dup


def write_manifest(path, data):
    os.makedirs(os.path.dirname(path), exist_ok=True)
    with open(path, "w", encoding="utf-8") as fh:
        json.dump(data, fh, indent=2)
    print("[json]", path)


def save_blend(path):
    os.makedirs(os.path.dirname(path), exist_ok=True)
    bpy.ops.wm.save_as_mainfile(filepath=path)
    print("[blend]", path)
