# CLOUS-END — Documentación de diseño

**Estado:** preproducción · documentación v0.6 · alcance final definido · lobby, blockout, demo web, **combate completo jugable** y **modelos 3D de Blender integrados en `Game`** con texturas VAT float
**Motor:** Unity 6.6.0f1 · URP 17.6 · Input System 1.20
**Género:** shooter cinemático de defensa sobre trineo (on-rails / semi-libre)
**Duración objetivo:** 20–30 minutos · **1 mapa** · sin niveles ni progresión
**Plataforma inicial:** PC (teclado + ratón / mando)

---

## Qué es este proyecto

CLOUS-END es la última Nochebuena. Santa Claus sale del taller con cuatro renos, una bolsa de regalos que ya no contiene juguetes y una ciudad invadida por una flota alienígena. El jugador no controla a un soldado: controla el trineo, protege a los cuatro renos que le quedan y escolta a los supervivientes hasta el Refugio Aurora.

La gracia del proyecto no es la cantidad de contenido, sino la **intensidad y la calidad de un viaje corto**: modelos realistas, animación cuidada, mapas con puesta en escena y un contraste constante entre el calor de la Navidad y el frío de la invasión.

---

## Documentos

| # | Documento | Para qué sirve |
|---|---|---|
| 01 | [Visión general](01_Vision_General.md) | Pitch, pilares, público, alcance. |
| 02 | [Historia y mundo](02_Historia_y_Mundo.md) | Sinopsis, actos, personajes, facción, glosario. |
| 03 | [Game Design Document](03_Game_Design_Document.md) | Bucle de juego, vidas, armas, combate, dificultad y resultados. |
| 04 | [Login y flujo](04_Login_y_Flujo.md) | Especificación completa de la pantalla de acceso. |
| 05 | [Mapas y ciudades](05_Mapas_y_Ciudades.md) | Archivo de ideas; el mapa final es uno (Distrito Aurora). |
| 06 | [Personajes y vehículos](06_Personajes_Vehiculos.md) | Santa, renos, trineo y 4 armas; todo el 3D en Blender, sin mejoras. |
| 07 | [Monstruos y objetos](07_Monstruos_y_Objetos.md) | 5 enemigos y 1 boss en v1; el resto es archivo de ideas. |
| 08 | [Dirección de arte y audio](08_Direccion_de_Arte_Audio.md) | Realismo, iluminación, animación, VFX, sonido, UI. |
| 09 | [Arquitectura técnica](09_Arquitectura_Tecnica.md) | Escenas, carpetas, sistemas, datos, rendimiento. |
| 10 | [Producción y roadmap](10_Produccion_Roadmap.md) | Hitos, estimación, riesgos y orden de trabajo. |
| 11 | [Alcance final del juego terminado](11_Alcance_Final_Juego_Terminado.md) | **Documento rector:** 1 mapa, sin niveles, todo el 3D en Blender, qué falta y criterios de aceptación. |
| 12 | [Mapa blockout: escena Game](12_Mapa_Blockout_Game.md) | Qué hay montado, cómo reconstruirlo, controles, rendimiento y dónde sustituir los placeholders. |
| 13 | [Demo web y publicación](13_Web_Demo_GitHub_Pages.md) | Enlace jugable, cómo se genera el build WebGL, cómo se publica en GitHub Pages y sus límites. |
| 14 | [**Assets 3D y pipeline VAT**](14_Assets_3D_y_Pipeline_VAT.md) | Qué modelos hay, cómo se animan sin huesos (VAT) y **cómo importarlos a Unity paso a paso**. |
| 15 | [**Combate, misión y jefe**](15_Combate_Mision_y_Jefe.md) | Armas, enemigos, oleadas, rescate de renos, pickups, Nicolás Cero, HUD y pausa implementados. |

---

## Regla de oro del proyecto

> Si una decisión no mejora la sensación de **volar, defender o perder a un reno**, no entra en el juego terminado.

El alcance se protege con una lista cerrada: **1 mapa, 1 misión, 1 boss, 4 renos, 4 armas, 5 enemigos, 0 niveles**. Todo el 3D final se hace en Blender. Todo lo demás es post-lanzamiento.

**Git:** una única rama, `main`. Todos los cambios se commitean y se suben directamente a `main` (ver `AGENTS.md`).

**Documento rector:** [11 · Alcance final](11_Alcance_Final_Juego_Terminado.md).

**Demo web:** https://josue-rg.github.io/Clous-end-web/

**Siguiente paso:** probar la misión completa en el editor (`Login > JUGAR`) y
ajustar dificultad, cámara y tiempos. Después: audio real, intro/final
cinematográficos y albedo/Normal/ORM de los modelos.

---

## Estado de implementación

- [x] Documentación de visión, historia, gameplay, mapas, personajes, monstruos, arte, técnica, producción y alcance final.
- [x] Lobby jugable con botón JUGAR, SALIR y diorama 3D (sin credenciales).
- [x] Reglas de trabajo en `AGENTS.md` (sin creación en runtime, datos en Inspector/ScriptableObjects).
- [x] Blockout jugable de la escena `Game`: Distrito Aurora, vuelo, cámara, mira, disparo y HUD de pruebas.
- [x] Cronómetro, checkpoint del puente, meta con resultados y game over con reintento.
- [x] Demo web publicada en GitHub Pages (ver doc 13).
- [x] **Assets 3D de la v1 hechos en Blender**: Santa, trineo, 4 renos, 4 armas, 4 enemigos
      con sus variantes, 2 naves, Nicolás Cero, props, arnés y meteoritos.
- [x] **Rigs, clips y horneado VAT** de todos los assets animados (ver doc 14).
- [x] **LOD1, LOD2 y mallas de colisión** de los 14 assets animados.
- [x] **Importación del paquete a Unity**: 28 prefabs, 375 materiales, LODs y colliders en el proyecto principal.
- [x] **Texturas VAT float**: assets `RGBAHalf` generados desde el volcado de Blender (el importador EXR de Unity corrompe las posiciones).
- [x] **Combate completo**: 4 armas, 5 enemigos, 11 oleadas, rescate de renos,
      pickups, Nicolás Cero en 3 fases, HUD, pausa, checkpoint y resultados (doc 15).
- [x] **Rig del jugador integrado**: trineo, Santa, los 4 renos y las 4 armas en la mano; arnés, cascabeles, collar y meteoritos.
- [x] **Escena `Game` reconstruida** con el arte real, el combate y el jefe.
- [ ] Horneado de Albedo/Normal/ORM (el FBX lleva color plano por material).
- [ ] Optimizar el peso de las texturas VAT generadas (124 MB de assets).
- [ ] Audio real (música, SFX, voz de Voss).
- [ ] Intro/final cinematográficos y pantalla de créditos completa.
- [ ] Opciones de dificultad y accesibilidad.
- [ ] Pruebas de resolución, rendimiento y build de Windows.

Los documentos marcados como `v0.1` son propuestas vivas: se actualizan cuando una prueba en el editor demuestre que algo no funciona.
