# CLOUS-END — Documentación

**Versión actual:** demo jugable con el mapa de Distrito Aurora, combate completo y modelos 3D de Blender.
**Motor:** Unity 6.6.0f1 · URP 17.6 · Input System 1.20
**Género:** shooter cinemático de defensa sobre trineo (on-rails con movimiento lateral)
**Plataforma:** PC (teclado y ratón) · WebGL 2
**Demo web:** https://josue-rg.github.io/Clous-end/

---

## Qué es este proyecto

CLOUS-END es la última Nochebuena. Santa Claus sale del taller con cuatro renos, una bolsa de regalos que ya no contiene juguetes y una ciudad invadida por una flota alienígena. El jugador no controla a un soldado: controla el trineo, protege a los cuatro renos que le quedan y escolta a los supervivientes hasta el Refugio Aurora.

La gracia del proyecto no es la cantidad de contenido, sino la **intensidad y la calidad de un viaje corto**: modelos de Blender, animación, puesta en escena y un contraste constante entre el calor de la Navidad y el frío de la invasión.

---

## Documentos

| # | Documento | Contenido |
|---|---|---|
| 01 | [Visión general](01_Vision_General.md) | Pitch, pilares, público y alcance. |
| 02 | [Historia y mundo](02_Historia_y_Mundo.md) | Sinopsis, actos, personajes, facción y glosario. |
| 03 | [Game Design Document](03_Game_Design_Document.md) | Bucle de juego, vidas, armas, combate, dificultad y resultados. |
| 04 | [Login y flujo](04_Login_y_Flujo.md) | Especificación de la pantalla de acceso y del lobby. |
| 05 | [Mapas y ciudades](05_Mapas_y_Ciudades.md) | Archivo de ideas; la versión actual usa Distrito Aurora. |
| 06 | [Personajes y vehículos](06_Personajes_Vehiculos.md) | Santa, renos, trineo y las 4 armas. |
| 07 | [Monstruos y objetos](07_Monstruos_y_Objetos.md) | Los 5 enemigos, el jefe y los pickups de la versión. |
| 08 | [Dirección de arte y audio](08_Direccion_de_Arte_Audio.md) | Realismo, iluminación, animación, VFX, sonido y UI. |
| 09 | [Arquitectura técnica](09_Arquitectura_Tecnica.md) | Escenas, carpetas, sistemas, datos, rendimiento y build. |
| 10 | [Producción](10_Produccion_Roadmap.md) | Herramientas, pipeline de trabajo y fases completadas. |
| 11 | [**Alcance de la versión**](11_Alcance_Final_Juego_Terminado.md) | Documento rector: contenido, mapa, personajes, enemigos y fuera de alcance. |
| 12 | [Mapa de la escena Game](12_Mapa_Blockout_Game.md) | Layout de Distrito Aurora, construcción, rendimiento y colisiones. |
| 13 | [Demo web y publicación](13_Web_Demo_GitHub_Pages.md) | Enlace, build WebGL y publicación en GitHub Pages. |
| 14 | [**Assets 3D y pipeline VAT**](14_Assets_3D_y_Pipeline_VAT.md) | Modelos, animación en GPU e importación a Unity. |
| 15 | [**Combate, misión y jefe**](15_Combate_Mision_y_Jefe.md) | Armas, enemigos, oleadas, renos, pickups, Nicolás Cero, HUD y pausa. |

---

## Cómo leer la documentación

- **01–08** recogen el diseño, la historia y la dirección de arte.
- **09–15** describen la implementación real: escenas, sistemas, assets, combate y publicación.
- **11** es el documento rector del alcance: si otro documento contradice el contenido de la versión, prevalece el 11.

---

## Regla de oro

> Si una decisión no mejora la sensación de **volar, defender o perder a un reno**, no entra en el juego.

La versión se protege con una lista cerrada: **1 mapa, 1 misión, 1 jefe, 4 renos, 4 armas, 5 enemigos, 0 niveles**. Todo lo demás queda fuera de esta entrega.

---

## Contenido de la versión

- **Login/Lobby** con **JUGAR** y **SALIR**, sin credenciales.
- **Game**: Distrito Aurora, 4 zonas y 1,5 km de recorrido.
- **Jugador**: trineo, Santa, 4 renos y las 4 armas en la mano.
- **Combate**: 5 enemigos, 2 naves, 11 oleadas, 55 enemigos, barras de vida, pickups, casco, checkpoint y rescate de renos.
- **Jefe**: Nicolás Cero en 3 fases y 2 renos de obsidiana.
- **Interfaz**: HUD, pausa, resultados, game over y reinicio.
- **Web**: build WebGL publicado en GitHub Pages.

---

## Ficha técnica

| Campo | Valor |
|---|---|
| Escenas | `Login`, `Game` |
| Escena de gameplay | `Assets/Scenes/Game.unity` |
| Datos de armas | `Assets/Config/Weapons` |
| Datos de enemigos | `Assets/Config/Enemies` |
| Modelos 3D | `Assets/Art/Models/VAT` y `Assets/Prefabs` |
| Texturas VAT | `Assets/Art/Models/VAT/Generated` |
| Build web | `index.html`, `WebGL/`, `TemplateData/` en la raíz |
| Rama de trabajo | `main` |

**Git:** una única rama, `main`. Todos los cambios se suben directamente a `main` (ver `AGENTS.md`).

---

## Notas de la versión

- El mapa es un **blockout jugable** de Distrito Aurora; los modelos de personajes, enemigos, armas y props son de Blender.
- Los materiales usan **color plano por material**; las texturas PBR no están horneadas.
- La versión actual **no incluye audio**.
- La demo web pesa **63 MB** y requiere **WebGL 2**.
