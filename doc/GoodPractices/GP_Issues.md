# Buenas Prácticas de Issues en GitHub

Este documento resume las buenas prácticas para **crear, gestionar, resolver y cerrar issues** en GitHub, incluyendo nomenclatura y flujo recomendado.

---
## 1. Nomenclatura Recomendada para Issues

| Tipo      | Prefijo sugerido | Uso principal | Ejemplo de título |
|-----------|-----------------|---------------|-----------------|
| Bug       | `bug:`          | Errores o fallos | `bug: D6 no lanza correctamente el valor 6` |
| Feature   | `feature:`      | Nueva funcionalidad | `feature: Implementar sistema de cartas con modificadores` |
| Task      | `task:`         | Tareas internas o subtareas | `task: Crear prefab del D6 y asignar sprites` |
| Documentation | `doc:`       | Actualización de documentación | `doc: Actualizar README con ejemplos de tiradas` |
| Refactor  | `refactor:`     | Cambios de código sin alterar comportamiento | `refactor: Simplificar lógica de tiradas de dados` |
| Question / Help | `question:` / `help:` | Dudas o solicitudes de ayuda | `question: ¿Qué rango de valores debería tener el D10?` |
| Improvement / Enhancement | `enhancement:` | Optimización o mejora menor | `enhancement: Mejorar animación de lanzamiento de dados` |

## 2. Creación de Issues

**Objetivo:** Ser claro y específico para que cualquiera entienda el problema o la tarea.

- **Título conciso y descriptivo**
  - ❌ Malo: `Bug en el juego`
  - ✅ Bueno: `bug: D6 no lanza correctamente el valor 6`
- **Descripción detallada**
  - Contexto del problema
  - Pasos para reproducirlo (si es un bug)
  - Resultados esperados vs actuales
  - Capturas de pantalla o GIFs
- **Etiquetas (Labels)**
  - `bug`, `enhancement`, `documentation`, `question`, `help wanted`, `good first issue`
- **Asignar responsables** (si aplica)
- **Referencias a PRs o commits**: `#issueNumber`

---

## 3. Uso y Seguimiento de Issues

- Mantener el issue actualizado con avances o bloqueos
- Discusión enfocada; evita desviarse del tema
- Prioridad y milestones:
  - Prioridades: `high`, `medium`, `low`
  - Milestones: `v1.0`, `v2.0`, etc.

---

## 4. Resolución y Pull Requests

- Crear PR desde el branch relacionado
- Referenciar el issue en la PR:
  - `Closes #issueNumber` o `Fixes #issueNumber`
- Revisar que la solución cumpla los criterios
- Confirmar que los tests pasen (si aplica)

---

## 5. Cierre de Issues

- Cerrar solo cuando el issue esté resuelto
- Documentar la solución: referenciar commit/PR exacto
- Archivar issues antiguos si no son relevantes

---

## 6. Buenas Prácticas Generales

- Un issue = un problema o tarea
- Usar plantillas de issues
- Comunicación transparente: actualizar cambios de prioridad o alcance
- Revisiones periódicas: limpieza de issues viejos

---

## 7. Uso de Issues para Features y Tareas

- **Features / Nuevas funcionalidades**
  - Prefijo: `feature:`
  - Descripción de la feature, impacto, dependencias, criterios de aceptación
- **Tareas / To-dos**
  - Prefijo: `task:`
  - Subtareas con checkboxes:
    ```markdown
    - [ ] Crear prefab del D6
    - [ ] Asignar sprites a cada cara
    - [ ] Probar física en la mesa de juego
    ```
- Organización:
  - Milestones: agrupar por versión o fase del proyecto
  - Projects (Kanban): mover issues a columnas `To Do`, `In Progress`, `Done`
  - Etiquetas: indicar prioridad y tipo

---


**Convenciones adicionales:**

1. **Módulo o contexto opcional entre corchetes**  
   - Ejemplo: `feature: [LevelManager] Añadir fase de boss`
2. **Longitud**: mantener ≤ 70 caracteres si es posible
3. **Verbos según tipo**:
   - `feature:` → infinitivo (`Implementar sistema de cartas`)  
   - `bug:` → describir fallo (`D6 no lanza correctamente`)  
   - `task:` → acción (`Crear prefab del D6`)
4. **Vinculación con PR o commits**: usar `#issueNumber`
5. **Checklist para tareas complejas o features**:
   ```markdown
   - [ ] Subtarea 1
   - [ ] Subtarea 2
