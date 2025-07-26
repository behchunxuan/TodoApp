import axios from "axios";
import { TodoItemDto, SaveTodoItemDto, TodoFilterModel } from "../types";

const API_BASE = "https://localhost:7139/Todo";

const api = axios.create({
  baseURL: API_BASE,
  headers: { Accept: "application/json" },
});

// GET all todos
export const getTodos = async (filter: Partial<TodoFilterModel> = {}): Promise<TodoItemDto[]> => {
  const response = await api.post("/list", filter);
  const { success, data } = response.data;

  if (!success || !Array.isArray(data)) {
    throw new Error("Invalid API response format");
  }

  return data;
};

// POST new todo
export const createTodo = (dto: Omit<SaveTodoItemDto, "todo_id">) =>
  api.post("/", dto);

// PUT update todo
export const updateTodo = (dto: SaveTodoItemDto) =>
  api.put("/", dto);

// DELETE
export const deleteTodo = (id: number) =>
  api.delete(`/${id}`);

// PATCH complete
export const completeTodo = (id: number) =>
  api.put(`/complete/${id}`);

// PATCH cancel
export const cancelTodo = (id: number) =>
  api.put(`/cancel/${id}`);
