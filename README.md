# CLOUS-END

> La última Navidad no se celebra. Se defiende.

**CLOUS-END** es un shooter cinemático de defensa sobre un trineo. Santa Claus sale del Polo Norte con cuatro renos, una bolsa de regalos que ya no contiene juguetes y una ciudad invadida por una flota alienígena. El jugador protege el trineo y a los cuatro renos —sus cuatro vidas— mientras escolta a los supervivientes hasta el Refugio Aurora.

- **Motor:** Unity 6.6 · URP 17.6
- **Género:** shooter 3D on-rails / defensa vehicular
- **Duración objetivo:** 20–30 minutos
- **Mapas:** 1 (Distrito Aurora)
- **Progresión:** ninguna (sin niveles, XP, monedas ni mejoras)
- **Arte 3D:** 100 % Blender (mapa, Santa, trineo, renos, naves, enemigos, armas)
- **Estado:** preproducción + login y demo jugables + **assets 3D integrados en `Game`** (trineo, Santa, 4 renos, armas, enemigos, jefe y props) con texturas VAT float

![Pantalla de login](Docs/Images/login_preview.png)

## Empezar

1. Abrir el proyecto en Unity 6.6.0f1.
2. Abrir `Assets/Scenes/Login.unity`.
3. Pulsar Play.

Para reconstruir la pantalla de acceso:

**Clous-End > Login > Construir escena de Login**

## Documentación

Toda la documentación está en [`Docs/`](Docs/README.md):

- [Visión general](Docs/01_Vision_General.md)
- [Historia y mundo](Docs/02_Historia_y_Mundo.md)
- [Game Design Document](Docs/03_Game_Design_Document.md)
- [Login y flujo](Docs/04_Login_y_Flujo.md)
- [Mapas y ciudades](Docs/05_Mapas_y_Ciudades.md)
- [Personajes y vehículos](Docs/06_Personajes_Vehiculos.md)
- [Monstruos y objetos](Docs/07_Monstruos_y_Objetos.md)
- [Dirección de arte y audio](Docs/08_Direccion_de_Arte_Audio.md)
- [Arquitectura técnica](Docs/09_Arquitectura_Tecnica.md)
- [Producción y roadmap](Docs/10_Produccion_Roadmap.md)
- [**Alcance final del juego terminado**](Docs/11_Alcance_Final_Juego_Terminado.md) — documento rector
- [Mapa blockout: escena Game](Docs/12_Mapa_Blockout_Game.md)
- [Demo web y publicación](Docs/13_Web_Demo_GitHub_Pages.md)
- [**Assets 3D y pipeline VAT**](Docs/14_Assets_3D_y_Pipeline_VAT.md) — qué modelos hay y cómo importarlos a Unity

## Alcance final

- **Una misión**, un mapa, un boss final.
- **4 renos** que son 4 vidas.
- **4 armas**, 5 enemigos y 3 naves.
- **Sin niveles, XP, monedas, mejoras ni inventario.**
- **Todos los assets 3D en Blender**; UI/2D y audio aparte.
- El plan completo de lo que falta está en [Docs/11](Docs/11_Alcance_Final_Juego_Terminado.md).

## Qué falta (resumen)

1. **Blender:** ~~kit del mapa, Santa, trineo, 4 renos, 4 armas, enemigos, naves, Nicolás Cero y animaciones~~ → hecho, ver [Docs/14](Docs/14_Assets_3D_y_Pipeline_VAT.md). Faltan el **kit del mapa** y el **horneado de texturas**.
2. **Unity:** ~~importar el paquete de assets~~ → hecho (28 prefabs y texturas float); la escena `Game` ya tiene el rig real, el combate, las armas en la mano y los props.
3. **UI/2D:** HUD final, iconos, pantalla de pausa, resultados y créditos.
4. **Audio:** música, SFX, voz de Voss y mezcla.
5. **QA/build:** misión de 20–30 minutos, 60 FPS y build de Windows.

Sin niveles, XP, monedas, mejoras, inventario ni mapas adicionales.

## Reglas del proyecto

Las reglas de trabajo están en [`AGENTS.md`](AGENTS.md). En resumen:

- Nada de crear escena o UI en runtime.
- La automatización se hace con Editor Scripts y el resultado queda guardado y editable.
- Todo valor de gameplay va en `[SerializeField]` o `ScriptableObject`.
- Scripts pequeños y desacoplados.

## Estado del login

- Escena: `Assets/Scenes/Login.unity`
- Configuración: `Assets/Config/LoginConfig.asset`
- Lógica: `Assets/Scripts/Login/`
- Construcción: `Assets/Editor/Login/`
- Fondo: `Assets/Art/Login/`

La escena `Game` existe con el blockout del Distrito Aurora (doc 12) y la demo web
está publicada (doc 13). Los modelos 3D finales están hechos y esperan importación
(doc 14).
