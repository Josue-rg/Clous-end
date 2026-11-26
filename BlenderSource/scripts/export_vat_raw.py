"""
Exporta las texturas VAT (EXR de posiciones) a un volcado crudo float32
para que Unity pueda generar texturas RGBAHalf sin pasar por su importador
de EXR, que en esta versión corrompe los valores.

Salida:
  BlenderSource/VATRaw/<Asset>/<Textura>.rgba32f
  cabecera: int32 width, int32 height (little endian)
  datos:    RGBA float32, bottom-up (mismo orden que Blender)

Uso:
  blender --background --python BlenderSource/scripts/export_vat_raw.py
"""

import glob
import os

import bpy
import numpy as np

ROOT = os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))
SRC = os.path.join(ROOT, "BlenderSource", "VAT")
DST = os.path.join(ROOT, "BlenderSource", "VATRaw")


def export(path):
    asset = os.path.basename(os.path.dirname(path))
    name = os.path.splitext(os.path.basename(path))[0]
    img = bpy.data.images.load(path)
    w, h = img.size
    arr = np.empty(w * h * 4, dtype=np.float32)
    img.pixels.foreach_get(arr)

    out = os.path.join(DST, asset, name + ".rgba32f")
    os.makedirs(os.path.dirname(out), exist_ok=True)
    header = np.array([w, h], dtype=np.int32)
    with open(out, "wb") as f:
        header.tofile(f)
        arr.tofile(f)

    print("[raw] %s %dx%d" % (out, w, h))
    bpy.data.images.remove(img)


def main():
    paths = sorted(glob.glob(os.path.join(SRC, "*", "T_*_Pos.exr")))
    print("[raw] texturas encontradas:", len(paths))
    for path in paths:
        export(path)
    print("[raw] listo")


main()
