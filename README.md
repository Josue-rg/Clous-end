# CLOUS-END

> La última Navidad no se celebra. Se defiende.

**CLOUS-END** es un shooter cinemático de defensa sobre un trineo. Santa Claus cruza Distrito Aurora con cuatro renos —sus cuatro vidas—, cuatro armas y una misión: escoltar a los supervivientes hasta el Refugio Aurora y derrotar a Nicolás Cero.

**Demo web:** https://josue-rg.github.io/Clous-end/

---

## Ficha

| Campo | Valor |
|---|---|
| Motor | Unity 6.6.0f1 · URP 17.6 · Input System 1.20 |
| Género | Shooter 3D de defensa sobre trineo (on-rails con movimiento lateral) |
| Plataforma | PC (teclado y ratón) · WebGL 2 |
| Cámara | Tercera persona detrás del trineo |
| Escenas | `Login` y `Game` |
| Mapa | Distrito Aurora (blockout jugable de 1,5 km, 4 zonas) |
| Modelos 3D | Blender: Santa, trineo, 4 renos, 4 armas, enemigos, naves, jefe y props |
| Animación | VAT (Vertex Animation Texture) en GPU, sin Animator |
| Progresión | Ninguna: sin niveles, XP, monedas, mejoras ni inventario |

---

## Contenido de la versión

- **Una misión continua** por Distrito Aurora: afueras, centro histórico, distrito financiero y Refugio Aurora.
- **4 renos = 4 vidas**: Rodolfo, Donner, Vixen y Cometa. El Garfio puede robar uno y hay una ventana de rescate.
- **4 armas**: Carbón (calor), Nogal (escopeta), Estrella (perforante) y Cascabel (cadena eléctrica).
- **5 tipos de enemigo**: Sabueso, Garfio, Espora, Manto y Torre.
- **2 naves**: Caza y Recolector.
- **Jefe final**: Nicolás Cero en tres fases, con dos renos de obsidiana.
- **11 oleadas y 55 enemigos** repartidos por la ruta; persiguen, se recolocan según la orientación de la cámara y hacen zigzag.
- **Barras de vida** sobre los enemigos.
- Casco del trineo, pickups, checkpoint del puente, HUD, pausa, resultados y reinicio.
- **Disparo hacia el puntero**: se apunta con el ratón; no hace falta girar la cámara para atinar.

---

## Controles

| Acción | Tecla / ratón |
|---|---|
| Avanzar / retroceder | `W` / `S` o flechas arriba/abajo |
| Desplazarse lateralmente | `A` / `D` o flechas izquierda/derecha |
| Subir / bajar | `Espacio` / `Ctrl izquierdo` o `Q` |
| Apuntar | mover el ratón (la retícula sigue al puntero) |
| Disparar | clic izquierdo |
| Cambiar de arma | `1` Carbón · `2` Nogal · `3` Estrella · `4` Cascabel |
| Recargar | `R` |
| Girar la cámara | clic derecho + arrastrar |
| Zoom | rueda del ratón |
| Pausa | `Esc` |

---

## Cómo jugar

### En el navegador

Abrir **https://josue-rg.github.io/Clous-end/**. La primera carga descarga el build WebGL (63 MB); si aparece una versión anterior, recargar con **Ctrl + F5**.

### En Unity

1. Abrir el proyecto con **Unity 6.6.0f1**.
2. Abrir `Assets/Scenes/Login.unity`.
3. Pulsar **Play** y después **JUGAR**.

También se puede abrir directamente `Assets/Scenes/Game.unity`.

---

## Estructura del proyecto

```
Assets/
  Scenes/        Login, Game
  Scripts/       Lógica de login, gameplay, combate, cámara, UI y VAT
  Editor/        Builders de escenas, HUD, mapa y pipeline VAT
  Config/        ScriptableObjects de armas y enemigos
  Prefabs/       Personajes, enemigos, armas y props
  Art/           Modelos, materiales y texturas de Blender y UI
  Settings/      URP y perfiles de postproceso
BlenderSource/   .blend, FBX, texturas y scripts de generación
Docs/            Documentación del proyecto
```

La escena `Game` se construye desde el editor con **`Clous-End > Mapa > Construir escena Game`**. Los assets de Blender se importan con **`Clous-End > VAT > Importar todo`** y las texturas VAT se generan con **`Clous-End > VAT > 6 · Generar texturas VAT (float)`**.

---

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
- [Producción](Docs/10_Produccion_Roadmap.md)
- [**Alcance de la versión**](Docs/11_Alcance_Final_Juego_Terminado.md)
- [Mapa de la escena Game](Docs/12_Mapa_Blockout_Game.md)
- [Demo web y publicación](Docs/13_Web_Demo_GitHub_Pages.md)
- [Assets 3D y pipeline VAT](Docs/14_Assets_3D_y_Pipeline_VAT.md)
- [Combate, misión y jefe](Docs/15_Combate_Mision_y_Jefe.md)

---

## Reglas del proyecto

Las reglas de trabajo están en [`AGENTS.md`](AGENTS.md):

- Nada de crear escena o UI en runtime.
- La automatización vive en Editor Scripts y el resultado queda guardado y editable.
- Todos los valores de gameplay van en `[SerializeField]` o `ScriptableObject`.
- Scripts pequeños, desacoplados y sin dependencias ocultas.

---

## Notas de la versión

- El mapa de esta versión es un **blockout jugable de Distrito Aurora**; los personajes, enemigos, armas y props son modelos de Blender.
- Los materiales de los modelos usan **color plano por material**; las texturas PBR no están horneadas en esta versión.
- La versión actual **no incluye audio**.
- La demo web pesa **63 MB** y requiere **WebGL 2**.

---

## Créditos

Proyecto personal de **Josue RG**. Modelos, animaciones y escenarios 3D creados en Blender; interfaz y documentación en Unity/C#.
