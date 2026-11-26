# CLOUS-END — Documentación de diseño

**Estado:** preproducción · documentación v0.2 · alcance final definido · lobby y blockout jugables
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

---

## Regla de oro del proyecto

> Si una decisión no mejora la sensación de **volar, defender o perder a un reno**, no entra en el juego terminado.

El alcance se protege con una lista cerrada: **1 mapa, 1 misión, 1 boss, 4 renos, 4 armas, 5 enemigos, 0 niveles**. Todo el 3D final se hace en Blender. Todo lo demás es post-lanzamiento.

**Documento rector:** [11 · Alcance final](11_Alcance_Final_Juego_Terminado.md).

**Siguiente paso:** revisar el blockout jugable de `Game` y empezar el primer kit modular del
mapa en Blender (calle, edificio, esquina y puente).

---

## Estado de implementación

- [x] Documentación de visión, historia, gameplay, mapas, personajes, monstruos, arte, técnica, producción y alcance final.
- [x] Lobby jugable con botón JUGAR, SALIR y diorama 3D.
- [x] Reglas de trabajo en `AGENTS.md` (sin creación en runtime, datos en Inspector/ScriptableObjects).
- [x] Blockout jugable de la escena `Game` con el Distrito Aurora, cámara, mira, disparo y HUD de pruebas.
- [ ] Pruebas de resolución y error de credenciales del login.
- [ ] Misión completa con boss.
- [ ] Assets de Blender integrados y audio.

Los documentos marcados como `v0.1` son propuestas vivas: se actualizan cuando una prueba en el editor demuestre que algo no funciona.
