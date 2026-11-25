# 07 · Monstruos y objetos

> **Alcance final v1.0:** **5 enemigos** (Sabueso, Garfio, Espora, Manto, Torre) y **1 boss** (Nicolás Cero). El resto del bestiario y los bosses intermedios son ideas futuras. Todos los modelos y animaciones se hacen en **Blender**. Ver [11 · Alcance final](11_Alcance_Final_Juego_Terminado.md).

## Bestiario final (v1)

| Enemigo | Rol | Ataque | Prioridad |
|---|---|---|---|
| **Sabueso** | Presión básica | Embiste al trineo | Baja |
| **Garfio** | Ladrón de renos | Engancha y tira; ventana de rescate | Máxima |
| **Espora** | Kamikaze | Se pega y explota | Media |
| **Manto** | Francotirador | Láser de aviso y disparo | Media-alta |
| **Torre** | Escudo | Protege y embiste | Media |
| **Nicolás Cero** | Boss final | 3 fases | —

Fuera de v1: Coro, Tejedor, Ojo, El Recolector, La Catedral y El Convoy. No se modelan para esta versión.

---

## Lenguaje de diseño de los Yermos

- **Silueta:** angulosa, alargada, con extremidades de más. Leen como instrumentos, no como animales.
- **Superficie:** obsidiana húmeda, vetas de luz blanca, juntas que parecen grietas.
- **Movimiento:** flotan con una calma desagradable; atacan con un latigazo seco.
- **Sonido:** vidrio, coro grave, chasquido de radio.
- **Color de amenaza:** blanco frío para el cuerpo; el aviso de ataque usa el código del HUD (amarillo/rojo/morado).
- **Regla:** nunca usan verde brillante, tentáculos de pulpo ni ojos saltones. Son bellos y horribles.

---

## Enemigos comunes

### 1. Sabueso
- **Rol:** cazador básico.
- **Aspecto:** drones de cuatro patas, cabeza partida en dos mandíbulas de cristal.
- **Ataque:** se lanza contra el trineo (amarillo). Se puede esquivar con dash.
- **Puntos débiles:** mandíbula abierta, juntas de las patas.
- **Prioridad:** baja.
- **Uso:** presión constante, enseña a esquivar.

### 2. Garfio
- **Rol:** ladrón de renos.
- **Aspecto:** cuerpo flotante con un brazo-cable terminado en un gancho de tres puntas.
- **Ataque:** engancha a un reno (rojo). Ventana de rescate de 5 s.
- **Puntos débiles:** el cable (disparable), el núcleo del brazo.
- **Prioridad:** máxima.
- **Uso:** crea el momento más tenso del juego. Debe tener un sonido inconfundible.

### 3. Manto
- **Rol:** francotirador.
- **Aspecto:** silueta envuelta en tela de estática; solo se ve la línea del rifle.
- **Ataque:** rayo de precisión con láser de aviso de 1,2 s (amarillo).
- **Puntos débiles:** se revela al disparar.
- **Prioridad:** media-alta.
- **Uso:** obliga a moverse y a usar coberturas.

### 4. Coro
- **Rol:** supresión.
- **Aspecto:** tres rostros unidos por un cuello de campana.
- **Ataque:** onda sonora que desactiva las armas 2 s (morado).
- **Puntos débiles:** la boca central durante la nota.
- **Prioridad:** alta.
- **Uso:** rompe el ritmo de disparo; hay que anticiparlo.

### 5. Espora
- **Rol:** kamikaze.
- **Aspecto:** saco translúcido con una luz blanca dentro.
- **Ataque:** se pega al casco y explota (amarillo).
- **Puntos débiles:** cualquier impacto; explosión en cadena.
- **Prioridad:** media.
- **Uso:** enjambres; el Nogal y el Cascabel brillan.

### 6. Torre
- **Rol:** escudo.
- **Aspecto:** bloque de obsidiana con un escudo de luz proyectado.
- **Ataque:** protege a otros; embiste lentamente.
- **Puntos débiles:** la parte trasera; el escudo cae con la Estrella.
- **Prioridad:** media.
- **Uso:** enseña a cambiar de arma y a flanquear.

### 7. Tejedor
- **Rol:** control de zona.
- **Aspecto:** araña de patas largas que teje redes de luz.
- **Ataque:** redes que frenan el trineo y bloquean la visión (blanco).
- **Puntos débiles:** el abdomen; las redes se queman con el Carbón.
- **Prioridad:** media.
- **Uso:** obliga a destruir el entorno y a no quedarse quieto.

### 8. Ojo
- **Rol:** marcador.
- **Aspecto:** esfera flotante con un iris de luz.
- **Ataque:** no ataca; marca al jugador y aumenta el daño recibido.
- **Puntos débiles:** el iris.
- **Prioridad:** alta (si hay varios, el daño se acumula).
- **Uso:** prioridad estratégica; el jugador aprende a matar al spotter primero.

---

## Bosses (archivo de ideas)

> En v1 solo existe **Nicolás Cero**. Los tres bosses que siguen quedan como ideas para una secuela.

### El Recolector (idea futura)
- **Fantasy:** el que se lleva a los renos.
- **Aspecto:** máquina de cuatro patas con jaulas y brazos-gancho; lleva collares colgando.
- **Fases:**
  1. **Enganche:** intenta robar a un reno. El jugador debe disparar al cable.
  2. **Jaulas:** lanza jaulas que bloquean el corredor; hay que destruirlas.
  3. **Núcleo:** se abre y muestra luz robada; vulnerable a la Estrella.
- **Debilidad:** los cables y el núcleo.
- **Recompensa:** Nogal.

### La Catedral (idea futura)
- **Fantasy:** una iglesia alienígena que devora la fe del pueblo.
- **Aspecto:** estructura invertida con campanas de vidrio y contrafuertes vivos.
- **Fases:**
  1. **Coro:** ondas que desactivan armas; cubrirse tras los contrafuertes.
  2. **Campanario:** destruir 4 campanas para abrir el núcleo.
  3. **Derrumbe:** caída libre; esquivar escombros y rematar.
- **Debilidad:** las campanas y el núcleo expuesto.
- **Recompensa:** Estrella.

### El Convoy / Tren Yermo (idea futura)
- **Fantasy:** la invasión como máquina imparable.
- **Aspecto:** locomotora de obsidiana con vagones-jaula y hocico de mandíbulas.
- **Fases:**
  1. **Persecución:** destruir vagones mientras esquiva el tráfico.
  2. **Costado:** se empareja y dispara; puntos débiles en los costados.
  3. **Choque:** embiste; usar el turbo para desviarlo al túnel.
- **Debilidad:** ruedas, enganches y la caldera.
- **Recompensa:** Cascabel + Regalo.

### Nicolás Cero (boss final v1)
- **Fantasy:** el anti-Santa. Lo que los Yermos creen que es la Navidad.
- **Aspecto:** santo hueco de obsidiana y terciopelo negro, trineo de vidrio, renos sin ojos, bolsa llena de bombas.
- **Fases:**
  1. **Procesión:** renos de obsidiana; destruirlos para bajarlo.
  2. **Reparto:** lanza regalos-bomba; esquivar o devolver.
  3. **Cuerpo a cuerpo:** en la cúpula de Aurora-9; solo queda el Carbón.
  4. **Sacrificio:** el Corazón de Estrella se desconecta; decisión final.
- **Debilidad:** el pecho vacío, donde debería estar el corazón.
- **Recompensa:** final.

---

## Objetos y pickups

> **En v1 no existe la moneda “Luz”.** Los pickups del juego son: munición, kit de casco, escudo de reno y las armas de historia. La Luz y el resto de objetos de la tabla quedan como ideas futuras.

### Recogibles
| Objeto | Efecto | Visual |
|---|---|---|
| **Luz** | Moneda de progreso | Chispa dorada que orbita el trineo |
| **Kit de casco** | +25 de integridad | Caja de madera con lazo rojo |
| **Munición** | Rellena el arma actual | Caja de cartón con etiqueta |
| **Cascabel de turbo** | Rellena el turbo | Cascabel de latón |
| **Escudo de Navidad** | Absorbe un impacto letal a un reno | Esfera de luces alrededor del reno |
| **Collar** | Coleccionable narrativo | Collar de cuero con inicial |
| **Regalo especial** | Mejora temporal | Caja que brilla en verde |

### Destructibles
- Chimeneas (explotan en ladrillos).
- Cables de teleférico (caen y bloquean).
- Farolas (se apagan y dejan humo).
- Coches abandonados (se incendian).
- Árboles de Navidad (sueltan Luz).
- Muros de hielo (se rompen con la Estrella).

### Peligros
- **Cables:** daño al casco y freno.
- **Hielo fino:** el trineo derrapa.
- **Fuego:** daño continuo; se apaga con nieve.
- **Esporas ambientales:** nubes que reducen la visión.
- **Escombros:** impactos físicos.
- **Tormenta:** viento lateral que empuja el trineo.

### Civiles
- No son escolta obligatoria, pero su supervivencia cambia el final. En v1 no dan moneda ni experiencia.
- Nunca se pueden matar por fuego amigo: las balas los atraviesan o se desvían.
- Si un civil muere por un enemigo, la radio de Voss reacciona.

---

## Reglas de spawn

- Presupuesto de amenaza por tramo, no listas fijas.
- Máximo 3 tipos de enemigo simultáneos.
- Un Garfio como máximo cada 40 s, y nunca dos a la vez.
- Los enemigos de reno aparecen con marca sonora y visual 1 s antes.
- Nunca aparecen enemigos detrás de la cámara sin aviso.
- Los bosses tienen *checkpoints* de fase, no de vida.

---

## Presupuesto de enemigos

| Elemento | Objetivo |
|---|---|
| Sabueso | 8 k tris, 1 material |
| Garfio | 12 k tris, 2 materiales |
| Manto | 10 k tris, 2 materiales |
| Coro | 14 k tris, 2 materiales |
| Espora | 4 k tris, 1 material |
| Torre | 10 k tris, 1 material |
| Tejedor | 12 k tris, 2 materiales |
| Ojo | 5 k tris, 1 material |
| Boss | 80–150 k tris, 4–6 materiales |

En pantalla: máximo 18 enemigos comunes + 1 boss. Los cadáveres se desvanecen en 2 s.
