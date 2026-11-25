# 05 · Mapas y ciudades

> **Archivo de ideas.** El juego v1 tiene **un único mapa**: Distrito Aurora. Los cinco capítulos de este documento **no forman parte del alcance a terminar**; quedan como ideas para una secuela o expansión. El mapa definitivo y su desglose están en [11 · Alcance final](11_Alcance_Final_Juego_Terminado.md).

## Mapa final (v1)

Una sola ruta continua, hecha en Blender como kit modular y montada en una única escena de Unity:

```
Afueras / autopista  →  Centro histórico  →  Distrito financiero  →  Refugio Aurora
```

- **Afueras:** tutorial de vuelo, camiones, primera nave.
- **Centro:** plaza, iglesia, árbol, civiles, primer Garfio.
- **Financiero:** torres, cables, puente, checkpoint, naves.
- **Refugio:** cúpula, faro, boss final y final de la historia.

No hay pantalla de selección de mapa ni carga entre zonas. Los cinco capítulos que siguen son material de archivo.

---

## Filosofía de mapas

Cada capítulo es un **corredor cinematográfico**, no un mundo abierto. El jugador vuela por un riel con libertad lateral, pero el mapa está construido para que siempre haya algo que mirar, algo que esquivar y un landmark que oriente.

Reglas:
- **Un landmark por capítulo.** Una torre, una catedral, un puente, un faro. Se ve desde lejos y se usa para orientarse.
- **Tres capas de lectura:** fondo (montañas/nave nodriza), medio (arquitectura), primer plano (árboles, cables, escombros).
- **El suelo nunca es plano.** Niebla, nieve acumulada, ríos, trincheras.
- **La luz cuenta la historia:** ventanas cálidas = vida; luz blanca = Yermos; aurora = esperanza.
- **Presupuesto:** 5 mapas, no 20. La calidad está en la composición, no en la cantidad.

---

## Progresión geográfica

```
Polo Norte (taller)
   ↓  bosque
Bosque de los Renos
   ↓  valle
Villaescarcha
   ↓  autopista
Autopista Helada
   ↓  ciudad
Distrito Aurora → Refugio Aurora-9
```

El viaje va de lo íntimo (el hogar de Santa) a lo colectivo (la ciudad refugio). La escala crece en cada capítulo.

---

## Capítulo 1 · Polo Norte — El Taller

**Hora:** 23:47, noche polar.
**Paleta:** azul profundo, madera cálida, rojo de Navidad, blanco estéril de los Yermos.
**Fantasy:** salir de casa mientras el mundo se cae.

### Layout
1. **Patio del taller.** Salida del trineo entre chimeneas y talleres. Tutorial de movimiento.
2. **Puente de hielo.** Se resquebraja al pasar. Primer turbo obligatorio.
3. **Establo en ruinas.** Los cuatro renos miran atrás. Los collares de los ausentes cuelgan de una viga.
4. **Acantilado.** El taller se derrumba detrás. Primera aparición de una nave Yerma.
5. **Mini-boss: Sabueso Alfa.** Enseña a esquivar y a priorizar.

### Set pieces
- Despegue con la bolsa arrastrando chispas.
- Derrumbe del taller en *matte painting* 3D.
- Primera visión del Ojo de Yermo entre las nubes.

### Enemigos
Sabuesos, Esporas. Ningún ataque a reno todavía: el jugador debe aprender a volar primero.

### Tono
Nostalgia + urgencia. La última vez que Santa ve su casa.

---

## Capítulo 2 · Bosque de los Renos

**Hora:** madrugada.
**Paleta:** verde azulado, negro de corteza, rojo de los collares, blanco de la nieve.
**Fantasy:** cruzar el cementerio de tu propia familia.

### Layout
1. **Avenida de pinos.** Visibilidad baja; los árboles son obstáculos reales.
2. **Río helado.** Vuelo rasante; el hielo se rompe y salen Esporas.
3. **Claro de los collares.** Momento de silencio: los cuatro collares de los renos perdidos. Sin combate durante 20 s.
4. **Puente colgante.** Los Garfios intentan robar a un reno por primera vez.
5. **Guarida del Recolector.** Nave con ganchos y jaulas. Boss.

### Set pieces
- La nieve negra cae al revés (VFX) durante el claro.
- Un reno perdido (Danzarín) aparece como sombra en la niebla; no se puede salvar. Es un espejismo.
- El Recolector arrastra jaulas con renos; el jugador puede liberar a uno (no jugable) para obtener Luz.

### Enemigos
Sabuesos, Garfios, Tejedores, Esporas. Primer ataque letal a un reno.

### Boss: El Recolector
Máquina de cuatro patas con brazos-gancho. Fases:
1. **Prueba:** engancha a un reno; el jugador debe disparar al cable.
2. **Furia:** lanza jaulas y divide la arena.
3. **Última:** se abre y muestra un núcleo de luz robada. Vulnerable a la Estrella.

### Recompensa
Arma **Nogal**. Primer punto de mejora real.

---

## Capítulo 3 · Villaescarcha

**Hora:** noche cerrada.
**Paleta:** ámbar de ventanas, azul de tejados, verde de abetos, morado alienígena.
**Fantasy:** la Navidad que todavía se puede salvar.

### Layout
1. **Valle de entrada.** Vista del pueblo: iglesia, plaza, teleférico. Landmark: la torre del reloj.
2. **Tejados.** Vuelo entre chimeneas; los civiles se asoman. No se puede disparar cerca de ellos.
3. **Plaza del mercado.** Primer convoy de evacuación. Defensa de zona con el Regalo.
4. **Teleférico.** Combate sobre cables; los Yermos cortan las cabinas.
5. **Iglesia.** Los Yermos construyen una catedral invertida alrededor de la iglesia. Boss.

### Set pieces
- El árbol de Navidad gigante de la plaza se apaga y se vuelve a encender.
- Un niño saluda desde una ventana; si el jugador no destruye esa casa, obtiene Luz extra.
- La catedral alienígena emerge del suelo.

### Enemigos
Sabuesos, Mantos, Coros, Torres. Mezcla de precisión y control de masas.

### Boss: La Catedral
Una estructura viva con campanas de vidrio. Fases:
1. **Coro:** ondas sonoras que desactivan las armas; hay que esconderse tras los contrafuertes.
2. **Campanario:** destruir cuatro campanas para abrir el núcleo.
3. **Derrumbe:** la catedral cae; combate final en caída libre.

### Recompensa
Arma **Estrella**. La Fe de la ciudad se convierte en energía.

---

## Capítulo 4 · Autopista Helada

**Hora:** amanecer negro.
**Paleta:** gris de asfalto, hielo azul, naranja de balizas, blanco de faros.
**Fantasy:** velocidad pura. El capítulo más arcade.

### Layout
1. **Incorporación.** La autopista está colapsada; el trineo vuela bajo entre camiones.
2. **Túnel.** Oscuridad, luces de emergencia, enemigos que se pegan al techo.
3. **Puente atirantado.** Los cables se cortan; hay que esquivar y disparar.
4. **Convoy.** Escolta de autobuses con civiles; los Yermos intentan volcarlos.
5. **Estación de peaje.** El Convoy Yermo bloquea el paso. Boss.

### Set pieces
- Adelantamiento a un camión cisterna en llamas.
- El túnel se inunda de esporas; solo se ve con los faros del trineo.
- El puente se parte en dos y el trineo salta.

### Enemigos
Sabuesos, Esporas, Torres, Garfios. Enemigos que se adhieren al trineo y hay que sacudir.

### Boss: El Convoy (Tren Yermo)
Una locomotora alienígena con vagones-jaula. Fases:
1. **Persecución:** el tren va delante; hay que destruir los vagones.
2. **Costado:** se empareja y dispara; hay que esquivar y golpear los puntos débiles.
3. **Choque:** embiste; el jugador debe usar el turbo para entrar en el túnel y hacerlo descarrilar.

### Recompensa
Armas **Cascabel** y **Regalo**. Mejora de turbo.

---

## Capítulo 5 · Distrito Aurora

**Hora:** la hora más oscura, justo antes de la aurora.
**Paleta:** negro, cian, magenta, ámbar del refugio, rojo de Santa.
**Fantasy:** el último vuelo.

### Layout
1. **Avenida de los Héroes.** Ciudad en ruinas, rascacielos partidos, el Ojo de Yermo llenando el cielo.
2. **Rascacielos.** Combate vertical entre edificios; los renos tiran hacia arriba.
3. **Azotea del faro.** El refugio Aurora-9 se ve por fin. La esperanza es visual.
4. **Cúpula.** El escudo aguanta; los civiles entran. Voss pide que Santa no se detenga.
5. **El Ojo.** Nicolás Cero desciende. Boss final.

### Set pieces
- La aurora boreal se intensifica con cada reno vivo.
- El refugio enciende sus luces al ver llegar el trineo.
- La bolsa de regalos se vacía por completo: el último objeto es un juguete.

### Enemigos
Todos, en composiciones de élite. Los cuatro renos son más importantes que nunca.

### Boss: Nicolás Cero
El anti-Santa. Un santo hueco, hecho de fe robada, con un trineo de obsidiana y renos sin ojos. Fases:
1. **Procesión:** avanza por el cielo; hay que destruir los renos de obsidiana.
2. **Reparto:** lanza «regalos» que son bombas; hay que devolverlos o esquivarlos.
3. **Nicolás:** cuerpo a cuerpo en la cúpula. El jugador solo tiene el Carbón y lo que quede en la bolsa.
4. **Sacrificio:** el Corazón de Estrella se desconecta; decisión final.

### Recompensa
Final base o final secreto (ver documento 02).

---

## Hub entre capítulos

No hay un hub explorable en el MVP. Entre capítulos se muestra una **pantalla de ruta** sobre un mapa estilizado del viaje:

- Posición actual y siguiente destino.
- Luz disponible.
- Estado de los renos (retratos).
- Tres mejoras para elegir una.
- Botón `CONTINUAR` (no hay botones extra en el login, pero sí en la ruta).

La pantalla de ruta es estática y reutiliza los tokens visuales del login.

---

## Presupuesto técnico por mapa

| Elemento | Objetivo |
|---|---|
| Triángulos visibles | < 1,5 M |
| Draw calls | < 1.200 |
| Materiales únicos | < 80 |
| Luces en tiempo real | 1 direccional + 4 puntuales máx. |
| Partículas | < 3 sistemas simultáneos pesados |
| Texturas | 2K para héroes, 1K para arquitectura, 512 para props |
| Colisiones | Solo el corredor de vuelo; nada de física completa |
| Streaming | No en el MVP; escenas pequeñas y cargas en fundido |

---

## Reglas de blockout

1. El jugador debe ver el siguiente obstáculo con **1,5 segundos** de antelación a velocidad base.
2. Ningún elemento decorativo puede tapar un ataque rojo.
3. Los landmarks se colocan primero; la decoración después.
4. Cada 20 segundos debe cambiar la silueta del horizonte.
5. La nieve no puede ocultar la retícula ni los marcadores de reno.
