const API = 'http://localhost:5002/api';

function authHeaders(): HeadersInit {
  const token = localStorage.getItem('accessToken');
  return { 'Authorization': `Bearer ${token}`, 'Content-Type': 'application/json' };
}

export interface NutritionValue { calories: number; proteins: number; fats: number; carbs: number; }
export interface MealLog { id: string; mealName: string; mealType: string; nutrition: NutritionValue; loggedAt: string; }
export interface DailyProgress {
  consumed: NutritionValue; target: NutritionValue | null;
  progress: { caloriesPercent: number; proteinsPercent: number; fatsPercent: number; carbsPercent: number } | null;
  adjustment: { message: string; suggestedTarget: NutritionValue; adjustmentPercent: number } | null;
  meals: MealLog[];
}
export interface TemplateMeal { mealType: string; dishName: string; description: string; nutrition: NutritionValue; }
export interface MealPlanTemplate { id: string; name: string; description: string; totalNutrition: NutritionValue; meals: TemplateMeal[]; remainingTemplates: number; }
export interface WeightLogEntry { id: string; weight: number; loggedAt: string; }
export interface DailyTarget { id: string; goal: string; target: NutritionValue; createdAt: string; updatedAt: string | null; }

export async function fetchDailyProgress(date?: string): Promise<DailyProgress> {
  const url = date ? `${API}/progress?date=${date}` : `${API}/progress`;
  const res = await fetch(url, { headers: authHeaders() });
  if (!res.ok) throw new Error(`${res.status}`);
  return res.json();
}

export async function logMeal(data: { mealName: string; mealType: number; calories: number; proteins: number; fats: number; carbs: number }): Promise<MealLog> {
  const res = await fetch(`${API}/meallogs`, { method: 'POST', headers: authHeaders(), body: JSON.stringify(data) });
  if (!res.ok) throw new Error(`${res.status}`);
  return res.json();
}

export async function deleteMealLog(id: string): Promise<void> {
  const res = await fetch(`${API}/meallogs/${id}`, { method: 'DELETE', headers: authHeaders() });
  if (!res.ok) throw new Error(`${res.status}`);
}

export async function setDailyTarget(data: { goal: number; calories: number; proteins: number; fats: number; carbs: number }): Promise<DailyTarget> {
  const res = await fetch(`${API}/dailytarget`, { method: 'POST', headers: authHeaders(), body: JSON.stringify(data) });
  if (!res.ok) throw new Error(`${res.status}`);
  return res.json();
}

export async function getNextTemplate(): Promise<MealPlanTemplate> {
  const res = await fetch(`${API}/templates/next`, { headers: authHeaders() });
  if (!res.ok) throw new Error(`${res.status}`);
  return res.json();
}

export async function resetTemplateQueue(): Promise<void> {
  await fetch(`${API}/templates/reset`, { method: 'POST', headers: authHeaders() });
}

export async function logWeight(weight: number): Promise<WeightLogEntry> {
  const res = await fetch(`${API}/weight`, { method: 'POST', headers: authHeaders(), body: JSON.stringify({ weight }) });
  if (!res.ok) throw new Error(`${res.status}`);
  return res.json();
}

export async function getWeightHistory(days = 30): Promise<WeightLogEntry[]> {
  const res = await fetch(`${API}/weight?days=${days}`, { headers: authHeaders() });
  if (!res.ok) throw new Error(`${res.status}`);
  return res.json();
}
