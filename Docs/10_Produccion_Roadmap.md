# 10 · Producción y roadmap

> **Alcance final v1.0:** un mapa, una misión, un boss, **sin niveles ni progresión**, y **todos los assets 3D hechos en Blender**. El desglose completo de contenido, assets y criterios está en [11 · Alcance final](11_Alcance_Final_Juego_Terminado.md). Este documento es el plan de producción resumido.

## Estado actual

| Entregable | Estado |
|---|---|
| Documentación de visión, historia, gameplay, arte, técnica y alcance | Hecha |
| Login jugable con perfil local | Hecho |
| Alcance final congelado (1 mapa, sin niveles) | Hecho |
| Escena `Game` | Pendiente |
| Mapa y assets de Blender | Pendiente |
| Audio | Pendiente |
| Build de prueba | Pendiente |

---

## Alcance cerrado

| Contenido | Cantidad |
|---|---|
| Misiones | 1 |
| Mapas | 1: Distrito Aurora |
| Renos | 4 (4 vidas) |
| Armas | 4, sin mejoras |
| Enemigos | 5 tipos |
| Boss | 1: Nicolás Cero |
| Naves alienígenas | 3 tipos |
| Pantallas | Login, HUD, pausa, resultados/créditos |
| Duración | 20–30 minutos |
| Arte 3D | 100 % Blender |
| Progresión | Ninguna (solo perfil y checkpoint en memoria) |

**No entra:** niveles, XP, monedas, mejoras, inventario, árbol de habilidades, varios mapas, selección de nivel, mundo abierto, multijugador, cooperativo, personalización, misiones secundarias, tienda, pases de batalla, VR, móvil, assets 3D de Asset Store.

---

## Fases de producción

### F0 · Alcance congelado
**Estado:** hecha.

- Un mapa, una misión, un boss, 4 armas, 5 enemigos.
- Sin niveles ni mejoras.
- Todos los 3D en Blender.
- Documento rector: `11_Alcance_Final_Juego_Terminado.md`.

### F1 · Blockout jugable (2 semanas)
- Escena `Game` con primitivas.
- `SleighController`, cámara, 4 renos, 2 armas y Sabueso.
- Corredor de prueba de 2–3 minutos.
- HUD mínimo.

**Salida:** volar, disparar y perder un reno ya funciona con placeholders.

### F2 · Kit de Blender v1 (4–6 semanas)
- Santa, trineo, reno base + 4 variantes.
- 4 armas, 5 enemigos, 3 naves.
- Kit del mapa: calles, edificios, iglesia, puente, refugio, props.
- Rigs y animaciones base.
- Export FBX, integración y prueba de escala/rendimiento.

**Salida:** el juego se ve como el producto final, aunque falten pulido y audio.

### F3 · Misión completa (3–4 semanas)
- Los 4 actos dentro del mapa.
- Oleadas, Garfio, checkpoint, pickups.
- Boss Nicolás Cero.
- Intro, final y créditos.
- HUD y resultados.

**Salida:** se puede jugar de principio a fin.

### F4 · Audio, UI y pulido (2–3 semanas)
- Música, SFX, voz de Voss.
- Subtítulos, accesibilidad y opciones.
- VFX finales.
- Ajuste de dificultad y duración.

**Salida:** experiencia completa y legible.

### F5 · QA y build (1–2 semanas)
- Pruebas completas.
- Optimización.
- Build de Windows.
- Corrección de bugs.
- Capturas/tráiler (opcional).

**Salida:** juego terminado.

---

## Estimación

| Equipo | Tiempo |
|---|---|
| 1 programador + 1 artista 3D + apoyo audio/UI | 12–16 semanas |
| 1 sola persona (todo) | 5–7 meses |
| Equipo de 3–4 con dedicación parcial | 4–6 meses |

La ruta crítica es el arte de Blender. El código puede avanzar con placeholders, pero el juego no se termina sin los modelos y animaciones.

---

## Reparto sugerido

| Rol | Responsabilidad |
|---|---|
| Diseño / dirección | Alcance, historia, tuning, revisión. |
| Programación | Sistemas, herramientas de editor, rendimiento, build. |
| Arte 3D / Blender | Mapa, Santa, trineo, renos, enemigos, naves, armas, props. |
| Animación (Blender) | Renos, Santa, trineo, enemigos, boss. |
| Audio | Música, SFX, voz, mezcla. |
| UI / 2D | HUD, iconos, resultados, créditos. |
| QA | Flujo completo, dificultad, accesibilidad, build. |

En un equipo de 2–3 personas, los roles se combinan; el alcance no.

---

## Riesgos y mitigación

| Riesgo | Impacto | Mitigación |
|---|---|---|
| El kit de Blender tarda más de lo previsto | Alto | Módulos reutilizables; no modelar 100 edificios únicos. |
| Los 4 renos requieren demasiada animación | Alto | Rig y animaciones compartidas; variantes por materiales/astas. |
| El mapa modular se ve repetitivo | Medio | 3–4 landmarks fuertes y variación de props. |
| El boss es demasiado complejo | Medio | 3 fases claras; reutilizar enemigos y naves. |
| El juego se alarga más de 30 min | Medio | Medir la ruta y cortar; la duración es parte del diseño. |
| Se cuela una idea de “nivel” o “mejora” | Alto | Volver a `11`; si no es historia o combate, no entra. |
| El realismo no llega | Medio | Priorizar silueta, luz y animación sobre detalle microscópico. |

---

## Próximas 10 tareas

1. Blockout del único mapa en Unity con primitivas.
2. Blockout de Santa, trineo y reno en Blender.
3. Primer módulo de calle + edificio + esquina en Blender.
4. Integrar ese módulo en Unity y comprobar escala, colisiones y FPS.
5. Rig y animación de galope del reno base.
6. Implementar `SleighController` y cámara.
7. Implementar `ReindeerSquad` y la pérdida de un reno.
8. Añadir Sabueso y Carbón.
9. Montar el HUD como prefab editable.
10. Definir el guion de radio de la intro y el final.

---

## Definición de “juego terminado”

- [ ] Login → intro → misión única → boss → final → créditos.
- [ ] 20–30 minutos de duración.
- [ ] 4 renos como vidas; se puede perder y reintentar.
- [ ] 4 armas, 5 enemigos y 1 boss funcionando.
- [ ] Checkpoint del puente.
- [ ] Todos los 3D finales son de Blender; sin primitivas en la escena final.
- [ ] 60 FPS a 1080p en PC medio.
- [ ] Sin errores en consola.
- [ ] Sin creación de escena/UI en runtime.
- [ ] Todos los valores de gameplay en Inspector/ScriptableObject.
- [ ] Build de Windows que arranca, se juega y se cierra.
- [ ] Historia comprensible sin leer la documentación.

---

## Lo que no se hará

- Sistema de niveles, XP o monedas.
- Mejoras de armas, trineo o renos.
- Inventario o árbol de habilidades.
- Varios mapas o capítulos.
- Selección de nivel.
- Mundo abierto.
- Multijugador o cooperativo.
- Personalización del trineo.
- Misiones secundarias.
- Tienda, pases o monetización.
- Guardado de progreso de misión.
- Doblaje completo o localización a varios idiomas.
- Logros.
- Móvil, consola o VR.
- Assets 3D de Asset Store.

Si algo de esta lista vuelve, se apunta para una secuela. No entra en la versión que hay que terminar.
