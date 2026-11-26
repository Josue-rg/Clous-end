# 10 · Producción

> Documento de proceso: cómo se construye, se prueba y se publica CLOUS-END.

---

## Herramientas

| Herramienta | Uso |
|---|---|
| **Unity 6.6.0f1** | Escenas, gameplay, UI, build WebGL |
| **Blender** | Modelos, UVs, rigs, animaciones, LODs y colisiones |
| **Pipeline VAT** | Horneado de animación a texturas y reproducción en GPU |
| **Git + GitHub** | Rama única `main` y publicación de la demo |
| **GitHub Pages** | Sitio estático servido desde la raíz de `main` |

---

## Pipeline de trabajo

```
BlenderSource/
  modelos y animaciones
        ↓
  export_vat_raw.py
        ↓
  Assets/Art/Models/VAT/Generated  (texturas float)
        ↓
  Clous-End > VAT > Importar todo
        ↓
  Assets/Prefabs + Assets/Art/Materials/VAT
        ↓
  Clous-End > Mapa > Construir escena Game
        ↓
  Assets/Scenes/Game.unity
        ↓
  WebBuild / Builds/WebGL
        ↓
  index.html + WebGL/ + TemplateData/ en main
        ↓
  GitHub Pages
```

---

## Fases completadas

| Fase | Resultado |
|---|---|
| **Documentación y alcance** | Visión, historia, GDD, arte, arquitectura y alcance cerrado. |
| **Login / Lobby** | Escena `Login` con JUGAR, SALIR y diorama 3D. |
| **Blockout del mapa** | Distrito Aurora, 4 zonas y 1,5 km con vuelo, cámara y HUD. |
| **Assets 3D** | Santa, trineo, 4 renos, 4 armas, 5 enemigos, 2 naves, jefe y props en Blender. |
| **Animación VAT** | Rigs, clips, texturas de posiciones, LODs y colisiones. |
| **Importación a Unity** | 28 prefabs, 375 materiales, texturas float y prefabs con LOD/collider. |
| **Combate** | 4 armas, 5 enemigos, 11 oleadas, 55 enemigos, jefe, pickups, barras de vida, HUD y pausa. |
| **Misión** | Checkpoint, resultados, game over y reinicio con cámara/posición al frente. |
| **Publicación web** | Build WebGL de 63 MB en GitHub Pages. |

---

## Construcción de la escena

La escena `Game` se genera desde el editor:

| Acción | Menú |
|---|---|
| Importar modelos y prefabs | `Clous-End > VAT > Importar todo` |
| Generar texturas VAT | `Clous-End > VAT > 6 · Generar texturas VAT (float)` |
| Reconstruir la escena | `Clous-End > Mapa > Construir escena Game (blockout)` |
| Abrir la escena | `Clous-End > Mapa > Abrir escena Game` |
| Probar | `Clous-End > Mapa > Entrar en Play Mode` |

La escena y los prefabs quedan guardados y se editan con el Inspector, la Jerarquía y el Scene View. El runtime no crea escena ni UI.

---

## Build web

1. Cambiar el target a **WebGL**.
2. Ejecutar `Clous-End > Build > WebGL para GitHub Pages` (genera `Builds/WebGL`).
3. Copiar `index.html`, `TemplateData/` y la carpeta `Build` renombrada a `WebGL/` en la raíz del repo.
4. Añadir `.nojekyll` si no existe.
5. Commit y `push` a `main`.
6. Esperar a que GitHub Pages reconstruya y recargar con **Ctrl + F5**.

El proyecto usa **Gzip**, **decompression fallback** y **data caching**. El build actual pesa **63 MB**.

---

## Publicación

- Rama única: **`main`**.
- GitHub Pages sirve desde la raíz de `main`.
- La demo está en **https://josue-rg.github.io/Clous-end/**.
- No hay backend ni guardado remoto: el sitio es estático.

---

## Estado de la entrega

La versión actual incluye login, mapa de Distrito Aurora, modelos de Blender, combate completo, jefe, HUD, pausa, resultados y build web. El mapa es un blockout jugable; los personajes, enemigos, armas y props son modelos 3D de Blender. La versión no incluye audio.
