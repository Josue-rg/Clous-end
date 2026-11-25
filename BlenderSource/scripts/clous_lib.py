"""
CLOUS-END · libreria de modelado compartida para Blender.

Reglas de proyecto:
- 1 unidad Blender = 1 metro.
- Eje Z arriba. Los personajes miran hacia -Y.
- Nombres SM_ / SK_ / M_ / T_ segun Docs/11.
- Solo geometria y materiales. Rigs y animaciones van en otra fase.

Este modulo se ejecuta dentro de Blender via MCP:
    exec(open(r"...\\clous_lib.py").read())
"""

import bpy
import bmesh
import math
import os
from mathutils import Vector, Matrix, Euler

TAU = math.tau
D2R = math.radians

# ---------------------------------------------------------------------------
# Escena
# ---------------------------------------------------------------------------

def reset_scene():
    """Borra todo y deja una escena limpia en metros."""
    try:
        if bpy.context.object and bpy.context.object.mode != 'OBJECT':
            bpy.ops.object.mode_set(mode='OBJECT')
    except Exception:
        pass

    for obj in list(bpy.data.objects):
        bpy.data.objects.remove(obj, do_unlink=True)

    for coll in list(bpy.data.collections):
        bpy.data.collections.remove(coll)
    for block in (bpy.data.meshes, bpy.data.materials, bpy.data.curves,
                  bpy.data.lights, bpy.data.cameras, bpy.data.images,
                  bpy.data.textures):
        for item in list(block):
            if item.users == 0:
                try:
                    block.remove(item)
                except Exception:
                    pass

    scene = bpy.context.scene
    scene.unit_settings.system = 'METRIC'
    scene.unit_settings.scale_length = 1.0
    return scene


def collection(name, parent=None):
    coll = bpy.data.collections.get(name)
    if coll is None:
        coll = bpy.data.collections.new(name)
        (parent or bpy.context.scene.collection).children.link(coll)
    return coll


def link(obj, coll):
    for c in list(obj.users_collection):
        c.objects.unlink(obj)
    coll.objects.link(obj)
    return obj


def activate(obj):
    try:
        active = bpy.context.view_layer.objects.active
    except Exception:
        active = None
    if active and active.mode != 'OBJECT':
        bpy.ops.object.mode_set(mode='OBJECT')
    bpy.ops.object.select_all(action='DESELECT')
    obj.select_set(True)
    bpy.context.view_layer.objects.active = obj
    return obj


# ---------------------------------------------------------------------------
# Materiales
# ---------------------------------------------------------------------------

def _set_input(node, name, value):
    if name in node.inputs:
        try:
            node.inputs[name].default_value = value
        except Exception as exc:
            print(f"[mat] no pude poner {name}: {exc}")


def material(name, base=(0.5, 0.5, 0.5, 1.0), rough=0.5, metal=0.0,
             spec=0.5, emission=None, emission_strength=0.0, alpha=1.0,
             sss=0.0, sheen=0.0, coat=0.0, noise=None):
    """Material Principled PBR. `noise` = dict(scale, detail, bump, color2)."""
    existing = bpy.data.materials.get(name)
    if existing:
        return existing

    m = bpy.data.materials.new(name)
    m.use_nodes = True
    nodes = m.node_tree.nodes
    links = m.node_tree.links
    nodes.clear()

    out = nodes.new('ShaderNodeOutputMaterial')
    out.location = (500, 0)
    bsdf = nodes.new('ShaderNodeBsdfPrincipled')
    bsdf.location = (180, 0)
    links.new(bsdf.outputs['BSDF'], out.inputs['Surface'])

    _set_input(bsdf, 'Base Color', base)
    _set_input(bsdf, 'Roughness', rough)
    _set_input(bsdf, 'Metallic', metal)
    _set_input(bsdf, 'IOR', 1.45)
    _set_input(bsdf, 'Specular IOR Level', spec)
    _set_input(bsdf, 'Alpha', alpha)
    _set_input(bsdf, 'Subsurface Weight', sss)
    _set_input(bsdf, 'Sheen Weight', sheen)
    _set_input(bsdf, 'Coat Weight', coat)
    if emission is not None:
        _set_input(bsdf, 'Emission Color', emission)
        _set_input(bsdf, 'Emission Strength', emission_strength)
    if alpha < 1.0:
        try:
            m.surface_render_method = 'BLENDED'
        except Exception:
            pass
        try:
            m.blend_method = 'BLEND'
        except Exception:
            pass
        try:
            m.show_transparent_back = False
        except Exception:
            pass

    if noise:
        tex = nodes.new('ShaderNodeTexCoord')
        tex.location = (-900, 0)
        mapping = nodes.new('ShaderNodeMapping')
        mapping.location = (-720, 0)
        try:
            mapping.inputs['Scale'].default_value = noise.get('mapping', (1, 1, 1))
        except Exception:
            pass
        links.new(tex.outputs['Object'], mapping.inputs['Vector'])

        nz = nodes.new('ShaderNodeTexNoise')
        nz.location = (-520, 0)
        nz.inputs['Scale'].default_value = noise.get('scale', 12.0)
        nz.inputs['Detail'].default_value = noise.get('detail', 6.0)
        nz.inputs['Roughness'].default_value = noise.get('roughness', 0.55)
        links.new(mapping.outputs['Vector'], nz.inputs['Vector'])

        ramp = nodes.new('ShaderNodeValToRGB')
        ramp.location = (-300, 120)
        c1 = noise.get('color1', base)
        c2 = noise.get('color2', base)
        ramp.color_ramp.elements[0].color = c1
        ramp.color_ramp.elements[1].color = c2
        links.new(nz.outputs['Fac'], ramp.inputs['Fac'])
        links.new(ramp.outputs['Color'], bsdf.inputs['Base Color'])

        bump_strength = noise.get('bump', 0.0)
        if bump_strength > 0.0:
            bump = nodes.new('ShaderNodeBump')
            bump.location = (-120, -260)
            bump.inputs['Strength'].default_value = bump_strength
            bump.inputs['Distance'].default_value = noise.get('distance', 0.02)
            links.new(nz.outputs['Fac'], bump.inputs['Height'])
            links.new(bump.outputs['Normal'], bsdf.inputs['Normal'])
    return m


# Paleta del juego (Docs/08)
def palette():
    P = {}

    # Santa
    P['red'] = material("M_Santa_Coat_Red", (0.30, 0.035, 0.030, 1), rough=0.82,
                        sheen=0.35, noise=dict(scale=38, detail=7, bump=0.22,
                                               color1=(0.225, 0.026, 0.022, 1),
                                               color2=(0.345, 0.048, 0.038, 1)))
    P['fur'] = material("M_Fur_Cream", (0.72, 0.68, 0.60, 1), rough=0.88,
                        sheen=0.6, noise=dict(scale=60, detail=8, bump=0.55,
                                              color1=(0.62, 0.58, 0.50, 1),
                                              color2=(0.82, 0.79, 0.72, 1)))
    P['skin'] = material("M_Skin_Aged", (0.52, 0.29, 0.22, 1), rough=0.62, sss=0.18,
                         noise=dict(scale=120, detail=4, bump=0.10,
                                    color1=(0.48, 0.26, 0.20, 1),
                                    color2=(0.55, 0.32, 0.25, 1)))
    P['beard'] = material("M_Hair_White", (0.80, 0.79, 0.75, 1), rough=0.78,
                          sheen=0.5, noise=dict(scale=55, detail=6, bump=0.22,
                                                color1=(0.70, 0.69, 0.66, 1),
                                                color2=(0.88, 0.87, 0.84, 1)))
    P['leather'] = material("M_Leather_Worn", (0.16, 0.09, 0.055, 1), rough=0.55,
                            noise=dict(scale=45, detail=7, bump=0.4,
                                       color1=(0.11, 0.06, 0.035, 1),
                                       color2=(0.22, 0.13, 0.08, 1)))
    P['iron'] = material("M_Iron_Forged", (0.20, 0.19, 0.18, 1), rough=0.52, metal=1.0,
                         noise=dict(scale=35, detail=6, bump=0.35,
                                    color1=(0.14, 0.13, 0.13, 1),
                                    color2=(0.30, 0.29, 0.27, 1)))
    P['brass'] = material("M_Brass", (0.62, 0.44, 0.18, 1), rough=0.32, metal=1.0,
                          noise=dict(scale=40, detail=5, bump=0.2,
                                     color1=(0.46, 0.31, 0.12, 1),
                                     color2=(0.74, 0.56, 0.26, 1)))
    P['wood'] = material("M_Wood_Ash", (0.30, 0.19, 0.10, 1), rough=0.48,
                         noise=dict(scale=18, detail=9, bump=0.45, mapping=(3, 3, 30),
                                    color1=(0.22, 0.13, 0.065, 1),
                                    color2=(0.40, 0.27, 0.15, 1)))
    P['vest'] = material("M_Vest_Thermal", (0.075, 0.095, 0.10, 1), rough=0.75,
                         sheen=0.25, noise=dict(scale=120, detail=5, bump=0.3,
                                                color1=(0.055, 0.070, 0.075, 1),
                                                color2=(0.10, 0.125, 0.13, 1)))
    P['trouser'] = material("M_Trouser_Wool", (0.13, 0.115, 0.10, 1), rough=0.85,
                            sheen=0.2, noise=dict(scale=70, detail=6, bump=0.3,
                                                  color1=(0.10, 0.09, 0.08, 1),
                                                  color2=(0.17, 0.15, 0.13, 1)))
    P['snow'] = material("M_Snow", (0.86, 0.90, 0.96, 1), rough=0.42,
                         noise=dict(scale=140, detail=4, bump=0.3,
                                    color1=(0.78, 0.84, 0.92, 1),
                                    color2=(0.92, 0.95, 1.0, 1)))
    P['glass'] = material("M_Glass_Lantern", (0.62, 0.68, 0.66, 1), rough=0.10,
                          metal=0.0, alpha=0.35, spec=1.0)
    P['warm_light'] = material("M_Emissive_Warm", (1.0, 0.76, 0.40, 1), rough=0.3,
                               emission=(1.0, 0.70, 0.32, 1), emission_strength=6.0)

    # Yermos (obsidiana)
    P['obsidian'] = material("M_Obsidian", (0.018, 0.018, 0.022, 1), rough=0.18,
                             metal=0.35, coat=0.6,
                             noise=dict(scale=9, detail=6, bump=0.25,
                                        color1=(0.008, 0.008, 0.012, 1),
                                        color2=(0.035, 0.035, 0.045, 1)))
    P['yermo_glow'] = material("M_Yermo_Veins", (0.85, 0.93, 1.0, 1), rough=0.25,
                               emission=(0.75, 0.90, 1.0, 1), emission_strength=9.0)
    P['void'] = material("M_Void_Black", (0.004, 0.004, 0.006, 1), rough=0.35)
    return P


# ---------------------------------------------------------------------------
# Primitivas
# ---------------------------------------------------------------------------

def _finish(bm, name, coll=None, smooth=True, merge=1e-5):
    if merge:
        bmesh.ops.remove_doubles(bm, verts=bm.verts, dist=merge)
    bmesh.ops.recalc_face_normals(bm, faces=bm.faces)
    me = bpy.data.meshes.new(name)
    bm.to_mesh(me)
    bm.free()
    if smooth:
        for poly in me.polygons:
            poly.use_smooth = True
    obj = bpy.data.objects.new(name, me)
    coll = coll or collection("ASSET")
    coll.objects.link(obj)
    return obj


def lathe(name, profile, segments=64, angle=TAU, start=0.0, coll=None,
          scale=(1, 1, 1), location=(0, 0, 0), merge=1e-5, smooth=True):
    """Superficie de revolucion alrededor de Z. profile = [(radio, z), ...]"""
    bm = bmesh.new()
    verts = [bm.verts.new((max(r, 0.0) * math.cos(start),
                           max(r, 0.0) * math.sin(start), z)) for r, z in profile]
    edges = []
    for i in range(len(verts) - 1):
        if verts[i].co != verts[i + 1].co:
            edges.append(bm.edges.new((verts[i], verts[i + 1])))
    geom = verts + edges
    bmesh.ops.spin(bm, geom=geom, cent=(0, 0, 0), axis=(0, 0, 1),
                   dvec=(0, 0, 0), angle=angle, steps=segments, use_duplicate=False)
    obj = _finish(bm, name, coll, smooth, merge)
    obj.scale = scale
    obj.location = location
    return obj


def tube(name, points, radius=0.02, segments=8, coll=None, caps=True, smooth=True):
    """Tubo por una lista de puntos (polyline). radius puede ser float o lista."""
    bm = bmesh.new()
    rings = []
    n = len(points)
    radii = radius if isinstance(radius, (list, tuple)) else [radius] * n
    for i, p in enumerate(points):
        p = Vector(p)
        if i == 0:
            tangent = Vector(points[1]) - p
        elif i == n - 1:
            tangent = p - Vector(points[i - 1])
        else:
            tangent = Vector(points[i + 1]) - Vector(points[i - 1])
        tangent.normalize()
        up = Vector((0, 0, 1))
        if abs(tangent.dot(up)) > 0.95:
            up = Vector((0, 1, 0))
        side = tangent.cross(up).normalized()
        up2 = side.cross(tangent).normalized()
        ring = []
        r_i = radii[min(i, len(radii) - 1)]
        for s in range(segments):
            a = TAU * s / segments
            off = side * (math.cos(a) * r_i) + up2 * (math.sin(a) * r_i)
            ring.append(bm.verts.new(p + off))
        rings.append(ring)
    bm.verts.ensure_lookup_table()
    for i in range(len(rings) - 1):
        for s in range(segments):
            a = rings[i][s]
            b = rings[i][(s + 1) % segments]
            c = rings[i + 1][(s + 1) % segments]
            d = rings[i + 1][s]
            try:
                bm.faces.new((a, b, c, d))
            except ValueError:
                pass
    if caps:
        try:
            bm.faces.new(list(reversed(rings[0])))
        except ValueError:
            pass
        try:
            bm.faces.new(rings[-1])
        except ValueError:
            pass
    return _finish(bm, name, coll, smooth, 1e-6)


def ellipsoid(name, radius=(1, 1, 1), location=(0, 0, 0), segments=24, rings=12,
              coll=None, rot=(0, 0, 0)):
    bm = bmesh.new()
    bmesh.ops.create_uvsphere(bm, u_segments=segments, v_segments=rings, radius=1.0)
    obj = _finish(bm, name, coll)
    obj.scale = radius
    obj.rotation_euler = rot
    obj.location = location
    return obj


def box(name, size=(1, 1, 1), location=(0, 0, 0), coll=None, rot=(0, 0, 0),
        bevel=0.0, segments=2, smooth=False):
    bm = bmesh.new()
    bmesh.ops.create_cube(bm, size=1.0)
    for v in bm.verts:
        v.co.x *= size[0]
        v.co.y *= size[1]
        v.co.z *= size[2]
    if bevel > 0.0:
        bmesh.ops.bevel(bm, geom=list(bm.verts) + list(bm.edges) + list(bm.faces),
                        offset=bevel, segments=segments, profile=0.5,
                        affect='EDGES', clamp_overlap=True)
    obj = _finish(bm, name, coll, smooth)
    obj.location = location
    obj.rotation_euler = rot
    return obj


def cylinder(name, radius=0.1, depth=1.0, location=(0, 0, 0), coll=None,
             rot=(0, 0, 0), segments=32, caps=True, smooth=True):
    bm = bmesh.new()
    bmesh.ops.create_cone(bm, cap_ends=caps, cap_tris=False, segments=segments,
                          radius1=radius, radius2=radius, depth=depth)
    obj = _finish(bm, name, coll, smooth, 1e-6)
    obj.location = location
    obj.rotation_euler = rot
    return obj


def cone(name, r1=0.1, r2=0.0, depth=1.0, location=(0, 0, 0), coll=None,
         rot=(0, 0, 0), segments=32, smooth=True):
    bm = bmesh.new()
    bmesh.ops.create_cone(bm, cap_ends=True, cap_tris=False, segments=segments,
                          radius1=r1, radius2=r2, depth=depth)
    obj = _finish(bm, name, coll, smooth, 1e-6)
    obj.location = location
    obj.rotation_euler = rot
    return obj


def ring_torus(name, major=0.2, minor=0.05, location=(0, 0, 0), coll=None,
               rot=(0, 0, 0), major_seg=48, minor_seg=16):
    bm = bmesh.new()
    for i in range(major_seg):
        a = TAU * i / major_seg
        for j in range(minor_seg):
            b = TAU * j / minor_seg
            r = major + minor * math.cos(b)
            bm.verts.new((r * math.cos(a), r * math.sin(a), minor * math.sin(b)))
    bm.verts.ensure_lookup_table()
    for i in range(major_seg):
        for j in range(minor_seg):
            a0 = i * minor_seg + j
            a1 = i * minor_seg + (j + 1) % minor_seg
            b0 = ((i + 1) % major_seg) * minor_seg + j
            b1 = ((i + 1) % major_seg) * minor_seg + (j + 1) % minor_seg
            try:
                bm.faces.new((bm.verts[a0], bm.verts[a1], bm.verts[b1], bm.verts[b0]))
            except ValueError:
                pass
    obj = _finish(bm, name, coll, True, 1e-6)
    obj.location = location
    obj.rotation_euler = rot
    return obj


# ---------------------------------------------------------------------------
# Superficies y barridos
# ---------------------------------------------------------------------------

def loft(name, sections, coll=None, close=False, cap_start=False, cap_end=False,
         smooth=True, merge=1e-5):
    """Une secciones (listas de puntos del mismo tamano) formando una superficie."""
    bm = bmesh.new()
    rings = []
    for sec in sections:
        rings.append([bm.verts.new(p) for p in sec])
    n = len(rings[0])
    rng = n if close else n - 1
    for i in range(len(rings) - 1):
        for s in range(rng):
            s2 = (s + 1) % n
            try:
                bm.faces.new((rings[i][s], rings[i][s2],
                              rings[i + 1][s2], rings[i + 1][s]))
            except ValueError:
                pass
    if cap_start:
        try:
            bm.faces.new(list(reversed(rings[0])))
        except ValueError:
            pass
    if cap_end:
        try:
            bm.faces.new(rings[-1])
        except ValueError:
            pass
    return _finish(bm, name, coll, smooth, merge)


def sweep_rect(name, path, width=0.06, height=0.10, coll=None, caps=True,
               smooth=True, side_axis=(1, 0, 0)):
    """Barre un rectangulo (ancho x alto) por una polilinea.
    side_axis fija el eje del ancho para mantener el marco estable en curvas."""
    bm = bmesh.new()
    rings = []
    n = len(path)
    for i, p in enumerate(path):
        p = Vector(p)
        if i == 0:
            tangent = Vector(path[1]) - p
        elif i == n - 1:
            tangent = p - Vector(path[i - 1])
        else:
            tangent = Vector(path[i + 1]) - Vector(path[i - 1])
        tangent.normalize()
        side = Vector(side_axis).normalized()
        up2 = tangent.cross(side).normalized()
        hw = width * 0.5
        hh = height * 0.5
        corners = [(-hw, -hh), (hw, -hh), (hw, hh), (-hw, hh)]
        rings.append([bm.verts.new(p + side * cx + up2 * cy) for cx, cy in corners])
    bm.verts.ensure_lookup_table()
    for i in range(len(rings) - 1):
        for s in range(4):
            s2 = (s + 1) % 4
            try:
                bm.faces.new((rings[i][s], rings[i][s2],
                              rings[i + 1][s2], rings[i + 1][s]))
            except ValueError:
                pass
    if caps:
        try:
            bm.faces.new(list(reversed(rings[0])))
        except ValueError:
            pass
        try:
            bm.faces.new(rings[-1])
        except ValueError:
            pass
    return _finish(bm, name, coll, smooth, 1e-6)


def star(name, outer=0.3, inner=0.13, depth=0.06, points=8, coll=None,
         location=(0, 0, 0), rot=(0, 0, 0), phase=0.0):
    """Estrella extruida en el plano XY."""
    bm = bmesh.new()
    ring = []
    for i in range(points * 2):
        a = TAU * i / (points * 2) + phase
        r = outer if i % 2 == 0 else inner
        ring.append((math.cos(a) * r, math.sin(a) * r))
    front = [bm.verts.new((x, y, depth * 0.5)) for x, y in ring]
    back = [bm.verts.new((x, y, -depth * 0.5)) for x, y in ring]
    bm.verts.ensure_lookup_table()
    for i in range(len(ring)):
        j = (i + 1) % len(ring)
        try:
            bm.faces.new((front[i], front[j], back[j], back[i]))
        except ValueError:
            pass
    try:
        bm.faces.new(front)
    except ValueError:
        pass
    try:
        bm.faces.new(list(reversed(back)))
    except ValueError:
        pass
    obj = _finish(bm, name, coll, False, 1e-6)
    obj.location = location
    obj.rotation_euler = rot
    return obj


def tube_ellipse(name, path, rx, rz, segments=16, coll=None, caps=True,
                 smooth=True):
    """Tubo de seccion eliptica (rx ancho, rz alto) por una polilinea."""
    bm = bmesh.new()
    rings = []
    n = len(path)
    rxs = rx if isinstance(rx, (list, tuple)) else [rx] * n
    rzs = rz if isinstance(rz, (list, tuple)) else [rz] * n
    for i, p in enumerate(path):
        p = Vector(p)
        if i == 0:
            tangent = Vector(path[1]) - p
        elif i == n - 1:
            tangent = p - Vector(path[i - 1])
        else:
            tangent = Vector(path[i + 1]) - Vector(path[i - 1])
        tangent.normalize()
        up = Vector((0, 0, 1))
        if abs(tangent.dot(up)) > 0.97:
            up = Vector((0, 1, 0))
        side = tangent.cross(up).normalized()
        up2 = side.cross(tangent).normalized()
        ring = []
        for s in range(segments):
            a = TAU * s / segments
            off = (side * (math.cos(a) * rxs[min(i, len(rxs) - 1)]) +
                   up2 * (math.sin(a) * rzs[min(i, len(rzs) - 1)]))
            ring.append(bm.verts.new(p + off))
        rings.append(ring)
    bm.verts.ensure_lookup_table()
    for i in range(len(rings) - 1):
        for s in range(segments):
            s2 = (s + 1) % segments
            try:
                bm.faces.new((rings[i][s], rings[i][s2],
                              rings[i + 1][s2], rings[i + 1][s]))
            except ValueError:
                pass
    if caps:
        try:
            bm.faces.new(list(reversed(rings[0])))
        except ValueError:
            pass
        try:
            bm.faces.new(rings[-1])
        except ValueError:
            pass
    return _finish(bm, name, coll, smooth, 1e-6)


def _catmull(p0, p1, p2, p3, t):
    t2 = t * t
    t3 = t2 * t
    return 0.5 * ((2.0 * p1) + (-p0 + p2) * t +
                  (2.0 * p0 - 5.0 * p1 + 4.0 * p2 - p3) * t2 +
                  (-p0 + 3.0 * p1 - 3.0 * p2 + p3) * t3)


def densify(points, values, sub=4):
    """Suaviza una polilinea con Catmull-Rom e interpola sus valores."""
    pts = [Vector(p) for p in points]
    vals = [v if isinstance(v, (int, float)) else Vector(v) for v in values]
    n = len(pts)
    out_p, out_v = [], []
    for i in range(n - 1):
        p0 = pts[max(i - 1, 0)]
        p1 = pts[i]
        p2 = pts[i + 1]
        p3 = pts[min(i + 2, n - 1)]
        for s in range(sub):
            t = s / sub
            out_p.append(_catmull(p0, p1, p2, p3, t))
            a, b = vals[i], vals[i + 1]
            out_v.append(a + (b - a) * t if isinstance(a, float) else
                         (a + (b - a) * t))
    out_p.append(pts[-1])
    out_v.append(vals[-1])
    return out_p, out_v


def tube_smooth(name, points, rx, rz, sub=4, segments=16, coll=None,
                caps=True, smooth=True):
    """Tubo eliptico suavizado (Catmull-Rom) por una polilinea de control."""
    n = len(points)
    rxs = list(rx) if isinstance(rx, (list, tuple)) else [rx] * n
    rzs = list(rz) if isinstance(rz, (list, tuple)) else [rz] * n
    p1, r1 = densify(points, rxs, sub)
    _, r2 = densify(points, rzs, sub)
    return tube_ellipse(name, p1, r1, r2, segments=segments, coll=coll,
                        caps=caps, smooth=smooth)


# ---------------------------------------------------------------------------
# Modificadores
# ---------------------------------------------------------------------------

def subsurf(obj, levels=2, render=2):
    m = obj.modifiers.new("Subsurf", 'SUBSURF')
    m.levels = levels
    m.render_levels = render
    return m


def bevel(obj, width=0.01, segments=2, angle=50.0, harden=True):
    m = obj.modifiers.new("Bevel", 'BEVEL')
    m.width = width
    m.segments = segments
    m.limit_method = 'ANGLE'
    m.angle_limit = D2R(angle)
    m.harden_normals = harden
    return m


def solidify(obj, thickness=0.02, offset=-1.0):
    m = obj.modifiers.new("Solidify", 'SOLIDIFY')
    m.thickness = thickness
    m.offset = offset
    m.use_rim = True
    return m


def displace(obj, strength=0.01, size=0.2, noise_scale=1.0):
    tex = bpy.data.textures.get(f"T_Displace_{obj.name}")
    if tex is None:
        tex = bpy.data.textures.new(f"T_Displace_{obj.name}", type='CLOUDS')
        tex.noise_scale = size
        tex.noise_depth = 3
    m = obj.modifiers.new("Displace", 'DISPLACE')
    m.texture = tex
    m.strength = strength
    m.mid_level = 0.5
    m.texture_coords = 'LOCAL'
    return m


def mirror(obj, axis='X'):
    m = obj.modifiers.new("Mirror", 'MIRROR')
    m.use_axis = (axis == 'X', axis == 'Y', axis == 'Z')
    m.use_clip = True
    return m


def apply_all(obj):
    activate(obj)
    for m in list(obj.modifiers):
        try:
            bpy.ops.object.modifier_apply(modifier=m.name)
        except Exception as exc:
            print(f"[apply] fallo {obj.name}/{m.name}: {exc}")
    return obj


def apply_transform(obj, location=True):
    activate(obj)
    bpy.ops.object.transform_apply(location=location, rotation=True, scale=True)
    return obj


def mirror_mesh(obj):
    """Duplica la malla espejada en X (espacio de mundo). Devuelve el mismo objeto."""
    apply_transform(obj, location=True)
    bm = bmesh.new()
    bm.from_mesh(obj.data)
    geom = list(bm.verts) + list(bm.edges) + list(bm.faces)
    res = bmesh.ops.duplicate(bm, geom=geom)
    dups = res["geom"]
    verts = [e for e in dups if isinstance(e, bmesh.types.BMVert)]
    faces = [e for e in dups if isinstance(e, bmesh.types.BMFace)]
    bmesh.ops.scale(bm, vec=(-1.0, 1.0, 1.0), verts=verts)
    bmesh.ops.reverse_faces(bm, faces=faces)
    bm.normal_update()
    bm.to_mesh(obj.data)
    bm.free()
    obj.data.update()
    return obj


def set_material(obj, mat):
    obj.data.materials.clear()
    obj.data.materials.append(mat)
    return obj


def set_materials(obj, mats):
    obj.data.materials.clear()
    for m in mats:
        obj.data.materials.append(m)
    return obj


def assign_by_angle(obj, mat_a, mat_b, axis=(0, 0, 1), threshold=0.5):
    """Asigna mat_a a las caras cuyo normal apunta hacia axis, mat_b al resto."""
    obj.data.materials.clear()
    obj.data.materials.append(mat_a)
    obj.data.materials.append(mat_b)
    n = Vector(axis).normalized()
    for poly in obj.data.polygons:
        poly.material_index = 0 if poly.normal.dot(n) > threshold else 1
    return obj


# ---------------------------------------------------------------------------
# Deformacion
# ---------------------------------------------------------------------------

def deform(obj, fn):
    """Aplica fn(Vector) -> Vector a cada vertice en coordenadas locales."""
    me = obj.data
    for v in me.vertices:
        v.co = fn(v.co)
    me.update()
    return obj


def taper_z(obj, z0, z1, s0=1.0, s1=1.0, axes=('X', 'Y')):
    def f(co):
        t = (co.z - z0) / (z1 - z0) if z1 != z0 else 0.0
        t = max(0.0, min(1.0, t))
        s = s0 + (s1 - s0) * t
        if 'X' in axes:
            co.x *= s
        if 'Y' in axes:
            co.y *= s
        return co
    return deform(obj, f)


def bend_x(obj, amount=0.0, z_pivot=0.0):
    def f(co):
        co.x += amount * (co.z - z_pivot) ** 2
        return co
    return deform(obj, f)


def squash(obj, sx=1.0, sy=1.0, sz=1.0):
    return deform(obj, lambda co: Vector((co.x * sx, co.y * sy, co.z * sz)))


def smooth_join(objects, name=None):
    """Une objetos y devuelve uno solo (aplica modificadores antes)."""
    objects = [o for o in objects if o is not None]
    for o in objects:
        apply_all(o)
    activate(objects[0])
    for o in objects[1:]:
        o.select_set(True)
    bpy.ops.object.join()
    obj = bpy.context.object
    if name:
        obj.name = name
        obj.data.name = name
    return obj


def duplicate(obj, name, offset=(0, 0, 0)):
    new = obj.copy()
    new.data = obj.data.copy()
    new.name = name
    new.data.name = name
    for c in obj.users_collection:
        c.objects.link(new)
    new.location = Vector(obj.location) + Vector(offset)
    return new


def set_origin_to_base(obj):
    activate(obj)
    bpy.context.scene.cursor.location = (0, 0, 0)
    bpy.ops.object.origin_set(type='ORIGIN_CURSOR')
    return obj


# ---------------------------------------------------------------------------
# Estudio de previsualizacion
# ---------------------------------------------------------------------------

def studio(bg=(0.055, 0.06, 0.075, 1), strength=0.55, floor=True):
    """Luces de estudio + world para revisar el modelo. Coleccion PREVIEW."""
    coll = collection("PREVIEW")
    scene = bpy.context.scene

    world = bpy.data.worlds.get("W_Preview") or bpy.data.worlds.new("W_Preview")
    scene.world = world
    world.use_nodes = True
    bgn = world.node_tree.nodes.get('Background')
    if bgn:
        bgn.inputs['Color'].default_value = bg
        bgn.inputs['Strength'].default_value = strength

    def area(name, loc, rot, energy, size, color=(1, 1, 1)):
        data = bpy.data.lights.new(name, 'AREA')
        data.energy = energy
        data.size = size
        data.color = color
        obj = bpy.data.objects.new(name, data)
        coll.objects.link(obj)
        obj.location = loc
        obj.rotation_euler = rot
        return obj

    area("L_Key", (3.2, -4.2, 4.4), (D2R(52), 0, D2R(38)), 700, 3.0, (1.0, 0.93, 0.84))
    area("L_Fill", (-4.0, -2.2, 2.2), (D2R(72), 0, D2R(-62)), 180, 4.0, (0.62, 0.74, 1.0))
    area("L_Rim", (-1.6, 4.4, 3.4), (D2R(115), 0, D2R(200)), 500, 2.4, (0.75, 0.88, 1.0))

    if floor:
        f = box("PREVIEW_Floor", (14, 14, 0.02), location=(0, 0, -0.01), coll=coll)
        m = material("M_Preview_Floor", (0.035, 0.038, 0.045, 1), rough=0.42)
        set_material(f, m)
        f.hide_select = True

    return coll


def save(path):
    path = os.path.abspath(path)
    os.makedirs(os.path.dirname(path), exist_ok=True)
    bpy.ops.wm.save_as_mainfile(filepath=path)
    print(f"[save] {path}")
    return path


def report(objects=None):
    total = 0
    lines = []
    objs = objects or list(bpy.context.scene.objects)
    for o in objs:
        if o.type != 'MESH':
            continue
        dg = bpy.context.evaluated_depsgraph_get()
        ev = o.evaluated_get(dg)
        me = ev.to_mesh()
        tris = sum(max(0, len(p.vertices) - 2) for p in me.polygons)
        total += tris
        lines.append(f"  {o.name:28s} {tris:7d} tris")
        ev.to_mesh_clear()
    print("\n".join(lines))
    print(f"  {'TOTAL':28s} {total:7d} tris")
    return total
