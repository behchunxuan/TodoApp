// src/components/TodoListSection.tsx
import React, { useState } from "react";
import { TodoItemDto } from "../types";
import { ChevronDown, ChevronRight, Eye, Pencil } from "lucide-react";

interface Props {
  title: string;
  items: TodoItemDto[];
  onEdit?: (item: TodoItemDto) => void;
  onView?: (item: TodoItemDto) => void;
  initiallyCollapsed?: boolean;
}

const TodoListSection: React.FC<Props> = ({
  title,
  items,
  onEdit,
  onView,
  initiallyCollapsed = false,
}) => {
  const [collapsed, setCollapsed] = useState(initiallyCollapsed);

  const formatDateTime = (isoString: string) => {
    const date = new Date(isoString);
    return date.toLocaleString("en-MY", {
      year: "numeric",
      month: "short",
      day: "numeric",
      hour: "2-digit",
      minute: "2-digit",
    });
  };

  return (
    <div className="mb-6 border rounded-lg overflow-hidden bg-white shadow">
      <button
        onClick={() => setCollapsed(!collapsed)}
        className="w-full flex justify-between items-center bg-gray-100 hover:bg-gray-200 px-4 py-2 text-left transition"
      >
        <span className="font-semibold text-md">{title}</span>
        <span className="transition-transform duration-300">
          {collapsed ? <ChevronRight size={18} /> : <ChevronDown size={18} />}
        </span>
      </button>

      <div
        className={`transition-all duration-300 ${
          collapsed ? "max-h-0 overflow-hidden" : "max-h-screen"
        }`}
      >
        <ul className="p-4 space-y-2">
          {items.map((item, index) => (
            <li
              key={item.todo_id}
              className="border p-4 rounded-lg flex flex-col gap-1 bg-white shadow-sm hover:shadow-md transition"
            >
              <div className="flex justify-between items-center">
                <span className="font-semibold text-md">
                  {index + 1}. {item.title}
                </span>
                <div className="flex gap-2">
                  {onView && (
                    <button
                      onClick={() => onView(item)}
                      className="flex items-center gap-1 text-gray-600 hover:text-gray-800 text-sm transition"
                      title="View Details"
                    >
                      <Eye size={16} />
                      View
                    </button>
                  )}
                  {onEdit && (
                    <button
                      onClick={() => onEdit(item)}
                      className="flex items-center gap-1 text-blue-500 hover:text-blue-700 text-sm transition"
                      title="Edit Details"
                    >
                      <Pencil size={16} />
                      Edit
                    </button>
                  )}
                </div>
              </div>

              <div className="text-sm text-gray-600 flex flex-wrap gap-4 mt-1">
                <span>🕒 Submitted: {formatDateTime(item.submitted_date)}</span>
                {item.status === "Completed" && item.completed_date && (
                  <span>
                    ✅ Completed: {formatDateTime(item.completed_date)}
                  </span>
                )}
                {item.status === "Cancelled" && item.cancelled_date && (
                  <span>
                    ❌ Cancelled: {formatDateTime(item.cancelled_date)}
                  </span>
                )}
                {item.tag && <span>🏷️ {item.tag}</span>}
                {item.priority && <span>🔥 Priority: {item.priority}</span>}
              </div>
            </li>
          ))}
          {items.length === 0 && (
            <p className="text-gray-500 text-sm">No To Do items</p>
          )}
        </ul>
      </div>
    </div>
  );
};

export default TodoListSection;
