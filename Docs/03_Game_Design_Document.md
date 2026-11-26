# 03 · Game Design Document

> **Alcance final v1.0:** una sola misión, un solo mapa, un solo boss, **sin niveles, XP, monedas ni mejoras**. Este documento describe el combate y el vuelo; cualquier referencia antigua a capítulos o mejoras queda como idea futura. Ver [11 · Alcance final](11_Alcance_Final_Juego_Terminado.md).

## Resumen

Shooter 3D de defensa sobre un trineo en movimiento. El jugador avanza por un riel invisible, se desplaza lateralmente para esquivar y apuntar, y protege dos cosas: el casco del trineo y los cuatro renos. Cada reno es una vida. El objetivo de la misión única es cruzar la ciudad, llegar al Refugio Aurora y derrotar a Nicolás Cero.

---

## Cámara y control

### Cámara
- Tercera persona detrás y ligeramente por encima del trineo.
- **Riel cinematográfico:** la posición base avanza sola; el jugador no controla el avance.
- **Libertad lateral:** el trineo se mueve en un corredor de ±8 m en horizontal y ±3 m en vertical.
- La cámara anticipa curvas, abre el plano en los bosses y hace *shake* contenido en impactos.
- En momentos de pérdida de reno: cámara lenta breve (0,4 s), foco en el reno, sin control.

### Controles (mando)

| Acción | Mando | Teclado/ratón |
|---|---|---|
| Mover trineo | Stick izquierdo | WASD |
| Apuntar | Stick derecho | Ratón |
| Disparar | RT | Clic izquierdo |
| Arma secundaria | LT | Clic derecho |
| Cambiar arma | LB / RB o cruceta | 1–4 / rueda |
| Turbo | A | Shift |
| Esquivar / dash | B | Espacio |
| Recargar | X | R |
| Pausa | Start | Esc |

### Sensación de vuelo
- El trineo **no** se mueve como una nave espacial: se inclina, derrapa y corrige con peso.
- La velocidad base es alta, pero el jugador siente el aire: líneas de velocidad, nieve levantada, cuerdas tensas, renos tirando.
- El turbo consume una barra; el dash tiene dos cargas y un enfriamiento corto.
- Chocar contra el entorno daña el casco, no mata directamente.

---

## Sistemas de vida y daño

### El casco del trineo
- 100 puntos de integridad.
- Se daña con impactos, colisiones y ataques de área.
- Se repara en el checkpoint del puente y con kits médicos (cajas de madera con lazo).
- A 0: el trineo pierde altura, entra en caída y la partida termina. No es una muerte instantánea: hay 5 segundos de emergencia para estabilizar si queda algún reno.

### Los renos = 4 vidas
- **Rodolfo, Donner, Vixen y Cometa.** Cada uno aporta una pasiva mientras esté vivo.
- Un reno puede estar **herido** (cojea, pierde sangre, su pasiva se desactiva temporalmente) sin que sea una vida perdida. Se cura en el checkpoint del puente.
- **Ataque letal:** un enemigo de élite impacta directamente. El reno muere. Se pierde una vida y su pasiva para siempre en esa partida.
- **Robo:** el Garfio engancha a un reno y tira de él. Se abre una ventana de **5 segundos** para disparar al cable o acercarse y cortarlo. Si se falla, el reno es arrancado del arnés y se pierde.
- **Sin renos:** el trineo cae. Game over. Se reinicia la misión desde el checkpoint del puente con el estado guardado en ese checkpoint; si se cae antes de llegar, se reinicia desde el principio.

> Regla de diseño: la pérdida de un reno debe ser **legible, evitable y recordable**. Nunca aleatoria.

### Santa
- No tiene barra de vida. La ficción manda: si el trineo cae, cae con él.
- Puede ser derribado por ataques de área: pierde el control 1,5 s, no vida.
- Su presencia se usa para momentos scripted (cubrirse, agarrar la bolsa, mirar atrás).

---

## Armas

Todas salen de la bolsa. Todas parecen juguetes o herramientas. Se cambian en caliente.

| Arma | Tipo | Munición | Función | Sensación |
|---|---|---|---|---|
| **Carbón** | Fusil automático | Infinita (calor) | DPS constante, precisión media | Traqueteo seco, chispas de carbón |
| **Nogal** | Escopeta | 8 cartuchos | Corta distancia, rompe escudos | Retroceso fuerte, nueces de acero |
| **Estrella** | Railgun | 3 cargas | Perfora y electrocuta | Carga luminosa, latigazo blanco |
| **Cascabel** | Cadena eléctrica | 6 cargas | Rebota entre 4 enemigos | Cascabeles que suenan antes de explotar |

**Fuera de v1:** el Regalo y la ultimate Aurora. El alcance final usa solo las cuatro armas de la tabla.

### Reglas
- El arma equipada se pierde si la bolsa cae; se recupera al recogerla.
- **No hay mejoras de armas.** Las armas se obtienen en momentos de la historia y se cambian con 1–4.
- El calor del Carbón obliga a ráfagas: no es un láser infinito.
- La Estrella es la respuesta a escudos; el Nogal, a enjambres; el Cascabel, a grupos.

---

## Enemigos y oleadas

### Lectura de amenazas
Cada enemigo anuncia su intención con **color, sonido y lenguaje corporal**:
- **Amarillo:** va a atacar al trineo.
- **Rojo:** va a atacar a un reno.
- **Blanco:** peligro de entorno.
- **Morado:** ataque especial / no interrumpible.

El jugador no necesita un minimapa: necesita mirar.

### Director de oleadas
- Las oleadas se componen por **presupuesto de amenaza** y roles, no por listas fijas.
- Cada acto tiene un beat: presentación, presión, respiro, pico, boss.
- Nunca más de 3 tipos de enemigo a la vez en pantalla.
- Los enemigos que atacan a los renos son la prioridad y siempre aparecen con una marca sonora propia.

---

## Estructura de la misión

La misión es una sola, dentro del único mapa:

```
[Intro: radio + despegue]
  ↓
[Acto 1: afueras / autopista — tutorial de vuelo y disparo]
  ↓
[Acto 2: centro histórico — primer Garfio, civiles, árbol de Navidad]
  ↓
[Checkpoint del puente — reparar casco y guardar estado]
  ↓
[Acto 3: distrito financiero — naves, torres, set piece del puente]
  ↓
[Acto 4: Refugio Aurora — boss Nicolás Cero]
  ↓
[Final + créditos]
```

No hay selección de nivel ni pantalla de mapa. Los actos son beats dentro del mismo mapa.

---

## Sin progresión

No hay sistema de niveles, XP, monedas, mejoras, inventario ni árbol de habilidades.

- **Armas:** se obtienen en la historia; no se suben.
- **Trineo:** el casco se repara en el checkpoint y con kits; no hay mejoras permanentes.
- **Renos:** las pasivas son fijas; no se mejoran.
- **Rejugabilidad:** dominar el vuelo, el rescate y el boss. No hay desbloqueos que cambien las estadísticas.
- **Guardado:** no hay perfil ni credenciales; la misión no se guarda y el checkpoint vive en
  memoria. Volver a jugar recarga la escena.

---

## Resultados

Al terminar (o al perder), se muestra un resumen breve:

| Dato | Uso |
|---|---|
| Tiempo total | Saber cuánto ha durado la misión |
| Renos vivos / perdidos | El coste emocional de la partida |
| Integridad del casco | Saber si se llegó al límite |
| Boss derrotado | Sí / no |
| Botones | Reintentar misión · Volver al lobby |

La demo muestra **tiempo**, **renos vivos**, **integridad del casco** y si el **boss** cayó.
Los botones son **REINTENTAR** (nueva partida o checkpoint) y **LOBBY**.

Sin puntos, sin rango, sin monedas y sin desbloqueos. El resultado es información, no una economía.

---

## Dificultad y accesibilidad

### Modos de dificultad
- **Repartidor** (fácil): menos enemigos de reno, rescate de 8 s, casco 150.
- **Nochebuena** (normal): diseño base.
- **Clous-End** (difícil): más élites, rescate de 4 s, sin aim assist.

### Opciones de asistencia (siempre disponibles)
- Asistencia de puntería (0–100 %).
- Tiempo de rescate ampliado.
- Invencibilidad de reno tras recibir un golpe (2 s).
- Reducción de *screen shake*.
- Texto grande, subtítulos, alto contraste, remapeo completo.
- Velocidad de juego 80–100 %.

---

## HUD

- **Arriba izquierda:** cuatro retratos/siluetas de reno con estado (sano / herido / perdido).
- **Arriba centro:** nombre del acto y objetivo actual (texto corto).
- **Abajo izquierda:** casco del trineo (barra segmentada) y turbo.
- **Abajo derecha:** arma, munición/calor y cargas de dash.
- **Centro:** retícula dinámica; se abre al disparar y se cierra al apuntar.
- **Bordes:** marcadores de amenaza roja cuando un reno está siendo atacado.
- **Radio:** subtítulos de Voss en la parte inferior, sin tapar la acción.

Sin minimapa, sin barras de XP, sin números flotantes grandes. La información justa para decidir.

---

## Fallo y reintento

- **Muerte de todos los renos:** secuencia de caída (8 s), pantalla de resultados y botón «Reintentar misión».
- **Casco a 0:** caída de emergencia; si queda algún reno, mini-eventos de 5 s para estabilizar. Si falla, mismo flujo.
- **Checkpoint:** uno solo, en el puente. Reintentar restaura el estado guardado allí (renos vivos, casco y armas de ese momento). Si se cae antes de llegar, se reinicia desde el principio.
- Sin vidas extra, sin continues infinitos. La dificultad es parte del drama.

---

## Tuning inicial (para la misión)

| Parámetro | Valor |
|---|---|
| Velocidad base | 45 m/s |
| Turbo | ×1,6 durante 2,5 s |
| Corredor lateral | ±8 m (0,35 s de tránsito) |
| Cadencia Carbón | 8 disparos/s |
| Daño Carbón | 6 por disparo |
| Casco | 100 |
| Ventana de rescate | 5 s (normal) |
| Enfriamiento de dash | 2,5 s |

Estos valores son una hipótesis: se ajustan jugando, no en la hoja de cálculo.

---

## Lo que el juego terminado debe demostrar

1. Volar se siente bien antes de disparar.
2. Disparar se siente bien antes de que aparezca el primer enemigo difícil.
3. Perder un reno duele y se entiende.
4. El boss final justifica las armas y el movimiento.
5. El login y el primer minuto venden el tono.
6. La misión completa se puede terminar en 20–30 minutos.
