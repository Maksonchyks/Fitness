# Інтеграція з API (API Integration)

Зв'язок вашого React-фронтенду з .NET бекендом (Workout та Identity мікросервісами).

## Кращі практики

1. **Інструмент для запитів**:
   Використовуйте **Axios** (з інтерсепторами) або **RTK Query** / **React Query (TanStack Query)**. Останній є золотим стандартом зараз.

2. **React Query (TanStack Query)**:
   - Він автоматично кешує дані, обробляє стани `isLoading`, `isError`.
   - Замість `useEffect` для отримання даних:
     ```tsx
     const { data, isLoading } = useQuery(['programs'], fetchPrograms);
     ```

3. **Axios Interceptors (Перехоплювачі)**:
   Це критично важливо для вашого мікросервісу `Identity`.
   Використовуйте інтерсептори, щоб:
   - **Автоматично додавати JWT токен**: Додавати `Authorization: Bearer <token>` до кожного запиту.
   - **Обробка 401 Unauthorized**: Якщо запит повертає помилку 401 (токен прострочився), інтерсептор повинен призупинити запит, зробити виклик до `/api/auth/refresh-token`, оновити токени і автоматично повторити оригінальний запит.

4. **Централізований API Клієнт**:
   Не пишіть повні URL у кожному компоненті. Створіть окремий файл (наприклад `apiClient.ts`):
   ```typescript
   export const apiClient = axios.create({
       baseURL: process.env.REACT_APP_API_URL,
       withCredentials: true // важливо для HttpOnly cookies (Refresh Token)
   });
   ```
