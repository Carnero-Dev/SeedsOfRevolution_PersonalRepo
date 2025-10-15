# Buenas prácticas para el uso de los commits
### ⚙️ Tipos de commit (convención estándar)

| Tipo      | Uso                                             | Ejemplo                                                  |
|------------|--------------------------------------------------|-----------------------------------------------------------|
| **feat**   | Nueva funcionalidad                             | `feat(seed): add sub-RNG for shop generation`             |
| **fix**    | Corrección de bug                               | `fix(save): resolve corrupted JSON loading`               |
| **refactor** | Mejora de código sin cambiar el comportamiento | `refactor(core): clean up input handling logic`           |
| **style**  | Cambios de formato, nombres, espacios, etc.     | `style: rename variables to match camelCase`              |
| **docs**   | Cambios en documentación o comentarios           | `docs(readme): update setup instructions`                 |
| **test**   | Añadir o modificar tests                        | `test(dice): add unit tests for face detection`           |
| **perf**   | Mejoras de rendimiento                          | `perf(physics): reduce rigidbody update overhead`         |
| **chore**  | Tareas menores, mantenimiento, dependencias     | `chore: update FMOD integration package`                  |
| **build**  | Cambios en el sistema de build o dependencias   | `build: add editor scripting define for debug mode`       |
| **ci**     | Cambios en CI/CD o pipelines                    | `ci: add Unity test runner to GitHub Actions`             |

### 🌿 Buenas prácticas con ramas (Branching Guide)

| Tipo de cambio | Rama recomendada | Ejemplo de nombre | Cuándo crear una nueva rama | Notas |
|----------------|------------------|-------------------|------------------------------|--------|
| 🧱 Nueva funcionalidad | `feature/*` | `feature/dice-physics` | ✅ Siempre | Aísla el desarrollo de nuevas features. |
| 🐞 Corrección de bug | `fix/*` o `hotfix/*` | `fix/save-system-nullref` | ✅ Siempre | Usa `hotfix/` si el bug está en producción (`main`). |
| 🔧 Refactor / limpieza de código | `refactor/*` | `refactor/input-system` | ✅ Recomendado | No cambia el comportamiento, pero mejora la estructura. |
| ⚙️ Cambios triviales o mantenimiento | `develop` | — | ❌ No necesario | Pequeños cambios: `docs`, `chore`, `style`. |
| 📄 Documentación | `develop` | — | ❌ No necesario | Ejemplo: actualizar README, comentarios o licencias. |
| 🚀 Preparar versión / build | `release/*` | `release/v1.0.0` | ✅ Cuando se prepare una build estable | Permite pulir detalles y testear antes de mergear a `main`. |
| 🧩 Integración general | `develop` | — | — | Rama base donde se integran todas las features antes de `main`. |
| 🏁 Producción / versión estable | `main` | — | — | Solo se mergea desde `release/*` o `hotfix/*`. Siempre estable. |

---
⚡ Consejos rápidos

- Una feature = una rama (mantén los cambios pequeños).
- Nunca trabajes en main directamente.
- Usa develop para integrar, probar o cambios triviales.
- No reutilices ramas antiguas (usa -v2, -update, etc. si repites algo).
- Limpia ramas después del merge (git branch -d ...).
- Usa nombres claros: feature/dice-physics, fix/save-nullref, etc.
- Si el bug es urgente en producción → hotfix/* directo desde main.

---

### 🧼 Limpieza de ramas
Después de hacer *merge*:
```bash
git branch -d feature/dice-physics
git push origin --delete feature/dice-physics

### 🌳 Flujo de ramas (Branch Flow)

             ┌──────────────────────────────┐
             │          main                │
             │ (versión estable / builds)   │
             └────────────┬─────────────────┘
                          │
                 merge desde release / hotfix
                          │
             ┌────────────▼─────────────────┐
             │          develop             │
             │ (integración y pruebas)      │
             └────────────┬─────────────────┘
                          │
     ┌────────────────────┼────────────────────┐
     │                    │                    │
┌────▼───────┐      ┌─────▼──────┐       ┌─────▼──────┐
│ feature/*  │      │ refactor/* │       │ fix/*      │
│ nuevas     │      │ mejoras    │       │ correcciones│
│ features   │      │ internas   │       │ menores     │
└────────────┘      └────────────┘       └────────────┘

         ↑
         │ merge hacia develop
         │
 ┌───────▼────────┐
 │ release/*      │
 │ (preparar build│
 │  estable)      │
 └────────────────┘
         │
         └── merge hacia main y tag versión

