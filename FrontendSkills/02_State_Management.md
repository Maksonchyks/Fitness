# Управління станом (State Management)

У фітнес-застосунку стан дуже динамічний: таймери, поточний підхід, збереження прогресу в реальному часі.

## Рекомендовані інструменти

1. **Zustand або Redux Toolkit (RTK)**:
   - **Zustand** чудово підходить, якщо ви хочете легкий, швидкий і сучасний стейт-менеджер з мінімальною кількістю шаблонного коду (boilerplate).
   - **Redux Toolkit** краще підходить для дуже великих додатків, де потрібна сувора структура.

2. **Що зберігати в Глобальному Стані?**
   - **Дані Користувача (Auth)**: Токени доступу, ролі, інформація про профіль.
   - **Активне Тренування (Active Workout Session)**: Коли користувач почав тренування, вам треба зберігати стан підходів (виконано/не виконано), вагу, час відпочинку. Це має жити в глобальному стані, щоб при переході між екранами дані не втрачались.

3. **Що залишати в Локальному Стані (`useState`)?**
   - Стан UI: відкрита/закрита модалка, введений текст у формі (до відправки), стан завантаження конкретної кнопки.

## Приклад Zustand для тренування:
```typescript
import { create } from 'zustand';

interface WorkoutState {
  isActive: boolean;
  currentExerciseIndex: number;
  startWorkout: () => void;
  nextExercise: () => void;
}

export const useWorkoutStore = create<WorkoutState>((set) => ({
  isActive: false,
  currentExerciseIndex: 0,
  startWorkout: () => set({ isActive: true, currentExerciseIndex: 0 }),
  nextExercise: () => set((state) => ({ currentExerciseIndex: state.currentExerciseIndex + 1 })),
}));
```
