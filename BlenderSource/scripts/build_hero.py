"""
CLOUS-END · escena de revision (no es un asset)

Junta Santa, trineo, arnes, dos renos y un arma para comprobar escala y lectura.
Se ejecuta en Blender con MCP. Guarda BlenderSource/Review/Hero.blend
"""

import bpy
import os

LIB_DIR = globals().get("__file__")
LIB_DIR = (os.path.dirname(os.path.abspath(LIB_DIR)) if LIB_DIR
           else r"C:\Users\Josue\Desktop\Clous-end\BlenderSource\scripts")
LIB = os.path.join(LIB_DIR, "clous_lib.py")
g = {"__name__": "clous_lib"}
exec(compile(open(LIB, encoding="utf-8").read(), LIB, "exec"), g)

RESET = g["reset_scene"]
STUDIO = g["studio"]
SAVE = g["save"]

SRC = r"C:\Users\Josue\Desktop\Clous-end\BlenderSource"

APPEND = [
    (os.path.join(SRC, "Vehicles", "SM_Sleigh.blend"), "SM_Sleigh"),
    (os.path.join(SRC, "Props", "SM_Harness_Set.blend"), "SM_Harness_Set"),
    (os.path.join(SRC, "Characters", "SK_Santa.blend"), "SK_Santa"),
    (os.path.join(SRC, "Characters", "SK_Reindeer.blend"), "SK_Reindeer_Rodolfo"),
    (os.path.join(SRC, "Characters", "SK_Reindeer.blend"), "SK_Reindeer_Cometa"),
]


def append_collection(path, name):
    before = set(bpy.data.objects.keys())
    with bpy.data.libraries.load(path, link=False) as (src, dst):
        if name in src.collections:
            dst.collections = [name]
    if not dst.collections or dst.collections[0] is None:
        print("no encontrado:", path, name)
        return
    coll = dst.collections[0]
    bpy.context.scene.collection.children.link(coll)
    for obj in coll.all_objects:
        if obj.name not in bpy.context.scene.objects and obj.name in bpy.data.objects:
            try:
                bpy.context.scene.collection.objects.link(obj)
            except RuntimeError:
                pass
    return coll


def build():
    bpy.ops.wm.read_homefile(use_empty=True)
    reset = RESET()
    reset.render.engine = 'BLENDER_EEVEE'

    for path, name in APPEND:
        append_collection(path, name)

    # colocar raices
    def move(name, loc, rot=None):
        o = bpy.data.objects.get(name)
        if o:
            o.location = loc
            if rot:
                o.rotation_euler = rot
            return o
        print("falta", name)
        return None

    move("SM_Sleigh", (0, 0, 0))
    move("SM_Harness_Set", (0, -2.30, 0))
    move("SK_Reindeer_Rodolfo", (-0.82, -4.55, 0))
    move("SK_Reindeer_Cometa", (0.82, -4.55, 0))
    move("SK_Santa", (-1.85, 0.95, 0))
    move("SM_Weapon_Carbon", (0.55, 0.55, 1.55),
         (0, 0, 1.05))

    STUDIO(bg=(0.045, 0.05, 0.062, 1), strength=0.5)
    coll = bpy.data.collections.get("PREVIEW")
    if coll:
        coll.objects["L_Key"].data.energy = 1400
        coll.objects["L_Rim"].data.energy = 900

    SAVE(os.path.join(SRC, "Review", "Hero.blend"))


build()
