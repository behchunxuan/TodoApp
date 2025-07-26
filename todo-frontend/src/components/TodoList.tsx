import React, { useEffect, useState } from "react";
import TodoModal from "./TodoModal";
import TodoListSection from "./TodoListSection";
import { getTodos } from "../api/todoApi";
import { TodoItemDto } from "../types";
import { useDebounce } from "../utils/useDebounce"

const TodoList: React.FC = () => {
  const [todos, setTodos] = useState<TodoItemDto[]>([]);
  const [showCancelled, setShowCancelled] = useState(false);
  const [selectedYear, setSelectedYear] = useState("All");
  const [selectedMonth, setSelectedMonth] = useState("All");
  const [searchTerm, setSearchTerm] = useState("");
  const [sortBy, setSortBy] = useState("submittedDate"); // "submittedDate" | "priority"
  const [editingItem, setEditingItem] = useState<TodoItemDto | null>(null);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [viewingItem, setViewingItem] = useState<TodoItemDto | null>(null);
  const debouncedSearchTerm = useDebounce(searchTerm, 700);

  useEffect(() => {
    loadTodos();
  }, [selectedYear, selectedMonth, debouncedSearchTerm, sortBy]);

  const loadTodos = async () => {
    const filter = {
      search_text: debouncedSearchTerm,
      year: selectedYear !== "All" ? parseInt(selectedYear) : undefined,
      month: selectedMonth !== "All" ? parseInt(selectedMonth) : undefined,
      sort_field: sortBy,
      sort_direction: "desc",
    };

    const data = await getTodos(filter);
    setTodos(data);
  };

  const pending = todos.filter((t) => t.status === "Pending");
  const completed = todos.filter((t) => t.status === "Completed");
  const cancelled = todos.filter((t) => t.status === "Cancelled");

  const currentYear = new Date().getFullYear();
  const years = Array.from({ length: currentYear - 2024 + 1 }, (_, i) => (2024 + i).toString()).reverse();

  const monthNames = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

  return (
    <div className="p-4">
      {/* Filters and Toolbar */}
      <div className="mb-4 flex flex-wrap gap-2 items-center justify-between">
        <div className="flex gap-2">
          <select value={selectedYear} onChange={(e) => setSelectedYear(e.target.value)} className="border px-2 py-1">
            <option value="All">All Years</option>
            {years.map((y) => (
              <option key={y} value={y}>{y}</option>
            ))}
          </select>
        <select value={selectedMonth} onChange={(e) => setSelectedMonth(e.target.value)} className="border px-2 py-1">
        <option value="All">All Months</option>
        {monthNames.map((name, i) => (
            <option key={i} value={(i + 1).toString()}>
            {name}
            </option>
        ))}
        </select>
          <select value={sortBy} onChange={(e) => setSortBy(e.target.value)} className="border px-2 py-1">
            <option value="submittedDate">Sort by Submitted Date</option>
            <option value="priority">Sort by Priority</option>
          </select>
          <div className="relative">
            <input
              type="text"
              placeholder="Search..."
              className="border px-3 py-1 w-64 rounded pr-8"
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
            />
            {searchTerm && (
              <button
                onClick={() => setSearchTerm("")}
                className="absolute right-1 top-1/2 -translate-y-1/2 text-gray-500 hover:text-black"
                title="Clear search"
              >
                ✖
              </button>
            )}
          </div>
        </div>

        <button
          onClick={() => setIsModalOpen(true)}
          className="bg-blue-500 text-white px-4 py-2 rounded">
          + Add Todo
        </button>
      </div>

      {/* Stats */}
      <div className="mb-4 flex flex-wrap gap-4 text-sm text-gray-700">
        <span>📌 Pending: {pending.length}</span>
        <span>✅ Completed: {completed.length}</span>
        <span>❌ Cancelled: {cancelled.length}</span>
        <span>📋 Total: {todos.length}</span>
      </div>

      {/* List Section */}
      <TodoListSection
        title={`📌 Pending Tasks (${pending.length})`}
        items={pending}
        onEdit={setEditingItem}
        onView={setViewingItem}
      />

      <TodoListSection
        title={`✅ Completed Tasks (${completed.length})`}
        items={completed}
        onView={setViewingItem}
      />

      <TodoListSection
        title={`❌ Cancelled Tasks (${cancelled.length})`}
        items={cancelled}
        onView={setViewingItem}
      />

      {/* Modal */}
      <TodoModal
        open={isModalOpen || !!editingItem}
        onClose={() => {
          setIsModalOpen(false);
          setEditingItem(null);
        }}
        item={editingItem}
        onSaved={() => {
          loadTodos();
          setIsModalOpen(false);
          setEditingItem(null);
        }}
      />
      {/* View-Only Modal */}
      <TodoModal
        open={!!viewingItem}
        readonly
        item={viewingItem}
        onClose={() => setViewingItem(null)}
        onSaved={() => {}}
      />
    </div>
  );
};

export default TodoList;