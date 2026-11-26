# 13 · Demo web y publicación

> **Estado:** demo WebGL publicada con el build actual: combate completo, modelos de
> Blender, texturas VAT generadas, barras de vida y 11 oleadas. El sitio se sirve
> desde `main`; el build pesa 63 MB.

---

## Enlaces

| Qué | Dónde |
|---|---|
| Jugar en el navegador | https://josue-rg.github.io/Clous-end/ |
| Repositorio del proyecto y la demo | https://github.com/Josue-rg/Clous-end |

El proyecto y la demo viven en el mismo repositorio público. GitHub Pages publica desde la
rama `main` (raíz), donde están `index.html`, `WebGL/` y `TemplateData/`. `main` es la única
rama: no hay rama `gh-pages` ni repositorio aparte.

---

## Qué incluye la demo

- **Lobby sin credenciales:** **JUGAR** carga la misión y **SALIR** cierra el juego.
- **Escena `Game`:** Distrito Aurora blockout, 4 zonas y 1,5 km.
- **Rig real de Blender:** trineo, Santa y los 4 renos animados con VAT.
- **4 armas:** Carbón (calor), Nogal (escopeta), Estrella (perfora) y Cascabel
  (cadena); cambio con 1–4 y recarga con R.
- **5 enemigos + naves:** Sabueso, Garfio, Espora, Manto, Torre y naves Caza y
  Recolector, en 11 oleadas y 55 enemigos precolocados.
- **Barras de vida** sobre los enemigos.
- **Renos = vidas:** el Garfio puede agarrar uno; rescátalo en 5 s. Escudo de reno.
- **Casco de 100**, kits, checkpoint del puente y game over.
- **Nicolás Cero** en 3 fases, con renos de obsidiana y barrera de arena.
- **HUD de combate**, pausa con Esc, resultados, tiempo, renos, casco y boss.
- **10 pickups** de munición, casco, escudo y las armas de historia.

---

## Cómo se construye

1. Unity **6.6 (6000.6.0f1)**, plataforma **WebGL**.
2. Escenas del build, en orden: `Assets/Scenes/Login.unity`, `Assets/Scenes/Game.unity`.
3. Player Settings → Publishing:
   - **Compression Format:** Gzip.
   - **Decompression Fallback:** activado (GitHub Pages no añade cabeceras de compresión).
   - **Data Caching:** activado.
4. Tamaño de lienzo por defecto: 1600 × 900.
5. El build genera `index.html`, `Build/`, `TemplateData/` y las mallas comprimidas
   `.unityweb`.
6. En el repo, la carpeta `Build/` se guarda como `WebGL/` (el `.gitignore` de Unity
   ignora `Build/`) y `index.html` apunta a `WebGL`. Se añade `.nojekyll` en la raíz.
7. Todo se sube a `main`. GitHub Pages publica desde `main` / (root) y el loader de Unity
   descomprime los `.unityweb` en el navegador.

No hay backend ni guardado remoto. La demo es un sitio estático.

### Script de build

`Assets/Editor/Game/WebBuild.cs` (solo herramienta de editor) configura Gzip,
decompression fallback, data caching y 1600×900, y compila `Login` + `Game` en
`Builds/WebGL`. Se puede ejecutar desde `-executeMethod ClousEnd.EditorTools.WebBuild.Build`
en batch.

---

## Cómo actualizar la demo

1. Reconstruir las escenas si han cambiado (`Clous-End > Login` y `Clous-End > Mapa`).
2. Cambiar el target a **WebGL** y ejecutar `Clous-End > Build > WebGL para GitHub Pages`
   (genera la carpeta `Builds/WebGL`, ignorada por git).
3. Copiar a la raíz del repo: `index.html`, `TemplateData/` y `.nojekyll`; renombrar la
   carpeta `Build` del build a `WebGL` (y dejar `index.html` apuntando a `WebGL`).
4. Commit y `push` a `main`.
5. Esperar a que Pages reconstruya (1–2 minutos) y recargar con **Ctrl + F5** para saltar la
   caché del navegador.

---

## Requisitos y limitaciones

- Navegador de escritorio con **WebGL 2**; teclado y ratón.
- El build actual pesa **63 MB**; la primera carga descarga las texturas VAT.
- Si aparece un 404 justo después de publicar, esperar a que Pages termine y recargar.
- El proyecto completo sigue siendo un juego de PC; la demo web es una compilación del mismo
  contenido.
- No hay controles táctiles, pantalla de carga con logo, botón de pantalla completa ni presets
  de calidad.
- La demo no incluye audio.

---

## Historial de la demo

| Fecha | Cambio |
|---|---|
| 26/11/2026 | **Demo de combate:** 4 armas, 5 enemigos, oleadas, rescate de renos, pickups, Nicolás Cero, HUD, pausa y resultados. |
| 26/11/2026 | Lobby con JUGAR, vuelo, checkpoint, cronómetro, resultados, game over y botón LOBBY. |
| 25/11/2026 | Primera demo: mapa Distrito Aurora, cámara, mira, disparo y HUD. |
