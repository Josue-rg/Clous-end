# 11 · Alcance de la versión

> **Documento rector.** Define qué contiene CLOUS-END en esta versión. Si otro documento contradice el contenido de esta ficha, prevalece este documento.

---

## Decisión de alcance

CLOUS-END es un **juego corto, lineal y completo**, no una campaña grande.

- **Un solo mapa.**
- **Una sola misión** de principio a fin.
- **Un solo jefe final.**
- **Sin sistema de niveles, XP, monedas, mejoras, inventario ni árbol de habilidades.**
- **Modelos 3D de Blender** para Santa, trineo, renos, armas, enemigos, naves, jefe y props.
- El mapa de esta versión es un **blockout jugable de Distrito Aurora** construido en Unity.
- La UI/2D y el audio se trabajan fuera de Blender (Figma/Photoshop y DAW/librerías); esta versión no incluye audio.

La prioridad no es la cantidad: es que la historia se entienda, que volar y disparar se sienta bien y que el jugador llegue al final con ganas de repetirlo.

---

## Ficha

| Campo | Valor |
|---|---|
| Nombre | CLOUS-END |
| Género | Shooter 3D de defensa sobre trineo, on-rails con movimiento lateral |
| Cámara | Tercera persona detrás del trineo |
| Jugadores | 1 |
| Estructura | Login → misión única → jefe → resultados → lobby |
| Mapas | 1: Distrito Aurora (blockout de 1,5 km) |
| Renos | 4, cada uno una vida |
| Armas | 4 |
| Enemigos | 5 tipos + 2 naves |
| Jefe | 1: Nicolás Cero + 2 renos de obsidiana |
| Oleadas | 11 |
| Enemigos totales | 55 |
| Pantallas | Login, HUD, pausa, resultados |
| Plataforma | PC (teclado/ratón) y WebGL 2 |
| Motor | Unity 6.6 · URP 17.6 |
| Arte 3D | Blender + pipeline VAT |
| Guardado | Sin progresión; checkpoint en memoria |
| Demo web | https://josue-rg.github.io/Clous-end/ |

---

## Bucle de juego

```
Login (JUGAR)
  ↓
Game · Distrito Aurora
  ├─ Afueras / autopista helada
  ├─ Centro histórico
  ├─ Distrito financiero · checkpoint del puente
  └─ Refugio Aurora · Nicolás Cero
  ↓
Resultados · tiempo, renos, casco y jefe
  ↓
Lobby o reintento
```

El trineo avanza; el jugador controla lateral/vertical, apunta con el ratón y dispara. Protege dos cosas: el casco del trineo y los cuatro renos. Si se pierden los cuatro, la partida termina. El checkpoint del puente permite reintentar sin repetir la primera mitad.

---

## Contenido

### El mapa: Distrito Aurora

| Zona | Fantasía | Elementos |
|---|---|---|
| **Afueras / autopista** | Salir del Polo Norte y ver la ciudad | Camiones, señales, naves industriales, pinos |
| **Centro histórico** | La Navidad que aún respira | Plaza, iglesia, árbol, civiles, Garfio |
| **Distrito financiero** | La invasión a escala | Torres, pasarelas, puente, cables, checkpoint |
| **Refugio Aurora** | La última luz | Cúpula, faro, explanada, jefe |

- Ruta continua de **1,5 km** con cuatro zonas.
- Geometría de blockout: calles, aceras, edificios, puente, iglesia, refugio y props.
- Streaming por zonas, LODs, niebla de distancia y props fusionados por material.
- La escena se construye desde el editor y queda guardada en `Assets/Scenes/Game.unity`.

### Jugador

- **Santa**, trineo y los cuatro renos: **Rodolfo, Donner, Vixen y Cometa**.
- Las 4 armas se muestran en la mano y cambian con la ranura equipada.
- Cámara en tercera persona a 7 m, con órbita, zoom y colisión.
- Apuntado con el ratón: el disparo sigue el rayo del puntero.
- El cuerpo de Santa gira hasta 40°; el pivote del arma completa el resto.

### Las 4 armas

| Arma | Rol | Munición | Especial |
|---|---|---|---|
| **Carbón** | Fusil automático | Infinita (calor) | Se sobrecalienta si se mantiene el gatillo |
| **Nogal** | Escopeta | 8 cartuchos | Dispersión y ruptura de escudos |
| **Estrella** | Railgun | 3 cargas | Perfora hasta 5 enemigos en línea |
| **Cascabel** | Cadena eléctrica | 6 cargas | Rebota entre 4 enemigos en 18 m |

### Los 5 enemigos y 2 naves

| Enemigo | Función | Ataque |
|---|---|---|
| **Sabueso** | Presión básica | Embiste al trineo |
| **Garfio** | Roba renos | Engancha y tira; ventana de rescate de 5 s |
| **Espora** | Kamikaze | Se pega y explota |
| **Manto** | Francotirador | Láser de aviso y disparo a distancia |
| **Torre** | Tanque | Escudo y embestida |
| **Nave Caza** | Nave rápida | Dispara tras telegraph |
| **Nave Recolector** | Nave pesada | Más vida y ataque |

- **11 oleadas** y **55 enemigos** repartidos por la ruta.
- Las oleadas se activan al acercarse y **no se desactivan**: los enemigos persiguen, se recolocan según la orientación del jugador y hacen zigzag.
- Barras de vida sobre cada enemigo.

### El jefe

**Nicolás Cero**, el anti-Santa fabricado por los Yermos, con dos renos de obsidiana.

1. **Procesión:** los renos de obsidiana atacan; hay que destruirlos.
2. **Reparto:** lanza regalos-bomba; se pueden disparar.
3. **Cuerpo a cuerpo:** al 40 % de vida acelera bombas y voleas.

### Los renos como vidas

- Cuatro renos, cuatro vidas.
- El Garfio puede agarrar uno: hay **5 segundos** para matarlo y rescatarlo.
- El escudo de reno revive o protege al siguiente.
- El casco del trineo es independiente: 100 de integridad; a 0 se pierde.

### Pickups

Munición, kit de casco, escudo de reno y las armas de historia (Estrella y Cascabel), repartidos por la ruta.

### Interfaz

- Login/Lobby con **JUGAR** y **SALIR**.
- HUD: renos, casco, arma, calor/munición, objetivo, jefe y rescate.
- Barras de vida sobre enemigos.
- Pausa con **Esc**.
- Resultados: tiempo, renos, casco y jefe.
- Game over con reintento en el checkpoint, restaurando posición y cámara al frente.

---

## Técnica

- Unity 6.6.0f1, URP 17.6 e Input System 1.20.
- Escenas `Login` y `Game`.
- Animación VAT: sin Animator ni SkinnedMeshRenderer; el shader mueve los vértices en GPU.
- Texturas de posiciones generadas como assets `RGBAHalf` en `Assets/Art/Models/VAT/Generated`.
- 28 prefabs de modelos, 375 materiales y LODs/colisiones para los assets animados.
- Build WebGL de **63 MB** publicado en GitHub Pages.
- Escena, UI y prefabs construidos en el editor; el runtime solo lee datos y cambia estado.

---

## Fuera de alcance

| Fuera de la versión | Motivo |
|---|---|
| Sistema de niveles / XP | La misión es corta; no aporta. |
| Monedas / tienda / mejoras | No hay economía ni progresión. |
| Varios mapas / capítulos | Un solo mapa y una sola misión. |
| Multijugador / cooperativo | Fuera del alcance de esta versión. |
| Mundo abierto | Es un corredor cinematográfico. |
| Inventario / árbol de habilidades | No hay objetos que gestionar. |
| Personalización del trineo | No aporta a la historia. |
| Misiones secundarias | La misión principal es el juego. |
| Guardado de progreso de misión | Solo perfil local y checkpoint en memoria. |
| Doblaje completo / localización | La versión actual no incluye audio. |
| Logros | No son necesarios para terminar. |
| Móvil / consola / VR | PC y WebGL primero. |
| Assets 3D de Asset Store | Los modelos son de Blender. |

---

## Regla de oro

> Si una decisión no mejora la sensación de **volar, defender o perder a un reno**, no entra en el juego.

---

## Resumen

**CLOUS-END es una misión corta y terminada: un blockout jugable de Distrito Aurora, Santa y su trineo, cuatro renos que son cuatro vidas, cuatro armas, cinco enemigos, dos naves y Nicolás Cero como jefe final; sin niveles, monedas ni mejoras.**
