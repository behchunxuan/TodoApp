export interface TodoItemDto {
  todo_id: number;
  title: string;
  content: string;
  tag: string;
  status: string;
  priority: string;
  submitted_date: string; // ISO string from backend
  completed_date?: string | null;
  cancelled_date?: string | null;
}

export interface SaveTodoItemDto {
  todo_id?: number; // optional for POST
  title: string;
  content?: string;
  tag?: string;
  status?: string;
  priority?: string;
}

export interface TodoFilterModel {
  status?: string;
  priority?: string;
  tag?: string;
  year?: number;
  month?: number;
  search_text?: string;
  sort_field?: string;
  sort_direction?: string; // "asc" or "desc"
}