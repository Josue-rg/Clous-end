# 13 · Demo web y publicación

> **Estado:** demo WebGL publicada en GitHub Pages. El flujo es **lobby → JUGAR → mapa →
> meta / game over**. La demo sirve para probar el mapa y el vuelo, no es el juego final.

---

## Enlaces

| Qué | Dónde |
|---|---|
| Jugar en el navegador | https://josue-rg.github.io/Clous-end-web/ |
| Repo del build (público) | https://github.com/Josue-rg/Clous-end-web |
| Código del proyecto (privado) | https://github.com/Josue-rg/Clous-end |

El repositorio del proyecto es **privado** y el plan actual no permite Pages en repos privados.
Por eso el build se publica en un repositorio público aparte (`Clous-end-web`); el código no se
duplica ni se hace público.

---

## Qué incluye la demo

- **Lobby sin credenciales:** un botón **JUGAR** carga el mapa y **SALIR** cierra el juego.
- **Escena `Game`:** Distrito Aurora en blockout, 4 zonas y 1,5 km.
- **Vuelo libre:** avanzar/retroceder, desplazamiento lateral, subir/bajar, cámara en tercera
  persona, mira con ratón y disparo hitscan.
- **Cronómetro** visible arriba a la derecha.
- **Checkpoint** al cruzar el puente (z = 1000), con aviso y reaparición en el carril.
- **Meta** en el Refugio (z = 1450): **MISIÓN COMPLETADA**, tiempo, **RENOS 4 / 4** y botones
  **REINTENTAR** y **LOBBY**.
- **Game over** si el trineo cae por debajo del mapa: **PARTIDA TERMINADA** con **REINTENTAR**
  (vuelve al checkpoint) y **LOBBY**.
- Placeholders cubo/cilindro para el vehículo y el personaje. Sin enemigos, armas finales,
  renos modelados ni boss.

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
   `.unityweb`. Se añade un archivo `.nojekyll` en la raíz.
6. Esa carpeta se publica en la rama `gh-pages` del repo `Clous-end-web`. GitHub Pages la
   sirve como sitio estático; el loader de Unity descomprime los `.unityweb` en el navegador.

No hay backend ni guardado remoto. La demo es un sitio estático.

---

## Cómo actualizar la demo

1. Reconstruir las escenas si han cambiado (`Clous-End > Login` y `Clous-End > Mapa`).
2. Cambiar el target a **WebGL** y generar el build en una carpeta limpia.
3. En esa carpeta:
   - crear `.nojekyll`;
   - `git init` en la rama `gh-pages`;
   - commit y `git push -f` a `https://github.com/Josue-rg/Clous-end-web.git`.
4. Esperar a que Pages reconstruya (1–2 minutos).
5. Recargar con **Ctrl + F5** para saltar la caché del navegador.

---

## Requisitos y limitaciones

- Navegador de escritorio con **WebGL 2**; teclado y ratón (no hay controles táctiles).
- El build pesa ~19 MB; la primera carga tarda unos segundos.
- Si aparece un 404 justo después de publicar, esperar a que Pages termine y recargar.
- El proyecto completo sigue siendo un juego de PC; la demo web es una compilación del mismo
  contenido.
- Pendiente para la web: controles táctiles, pantalla de carga con logo, botón de pantalla
  completa y presets de calidad.

---

## Historial de la demo

| Fecha | Cambio |
|---|---|
| 26/11/2026 | Lobby con JUGAR, vuelo, checkpoint, cronómetro, resultados, game over y botón LOBBY. |
| 25/11/2026 | Primera demo: mapa Distrito Aurora, cámara, mira, disparo y HUD. |
